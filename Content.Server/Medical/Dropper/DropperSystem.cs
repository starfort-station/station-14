

using Robust.Shared.Containers;
using Content.Shared.Medical.Dropper;
using Content.Server.Chemistry.Components.SolutionManager;
using Robust.Server.GameObjects;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Chemistry;
using Content.Server.Chemistry.EntitySystems;
using System.Linq;
using Content.Server.Popups;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Body.Components;
using Content.Shared.DoAfter;
using Content.Shared.Alert;
using Robust.Shared.Timing;
using Content.Shared.Hands;
using Content.Server.Damage;
using Content.Shared.Damage;
using Content.Server.Body.Systems;
using Content.Server.Body.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Interaction.Events;
using Content.Server.Administration.Logs;

namespace Content.Server.Medical.Dropper
{
    public sealed class DropperSystem : SharedDropperConnectableSystem
    {
        [Dependency] private readonly SharedContainerSystem _container = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly UserInterfaceSystem _ui = default!;
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
        [Dependency] private readonly SolutionContainerSystem _solutionContainerSystem = default!;
        [Dependency] private readonly PopupSystem _popup = default!;
        [Dependency] private readonly SharedHandsSystem _hands = default!;
        [Dependency] private readonly IEntityManager _entManager = default!;
        [Dependency] private readonly AlertsSystem _alerts = default!;
        [Dependency] private readonly IGameTiming _time = default!;
        [Dependency] private readonly DamageableSystem _damageSystem = default!;
        [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
        [Dependency] private readonly SolutionContainerSystem _solution = default!;
        [Dependency] private readonly ReactiveSystem _reactiveSystem = default!;
        [Dependency] private readonly IAdminLogManager _adminLogger = default!;
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<DropperComponent, ComponentStartup>(OnDropperStartup);
            SubscribeLocalEvent<DropperComponent, EntInsertedIntoContainerMessage>(OnSolutionPackInserted);
            SubscribeLocalEvent<DropperComponent, EntRemovedFromContainerMessage>(OnSolutionPackRemoved);
            SubscribeLocalEvent<DropperComponent, InteractUsingEvent>(OnDropperInteractUsing);
            SubscribeLocalEvent<DropperComponent, IntervalReached>(OnIntervalReached);
            SubscribeLocalEvent<DropperComponent, DropperOutOfRange>(OnDropperOutOfRange);

            SubscribeLocalEvent<DropperComponent, DropperNeedleEjectMessage>(OnDropperNeedleEject);
            SubscribeLocalEvent<DropperComponent, DropperChangeFrequencyMessage>(OnFrequencyChange);
            SubscribeLocalEvent<DropperComponent, DropperChangeQuantityMessage>(OnQuantityChange);
            SubscribeLocalEvent<DropperConnectableComponent, DropperNeedleRemovedEvent>(OnNeedleRemove);
            SubscribeLocalEvent<DropperNeedleComponent, DroppedEvent>(OnNeedleDropped);


            SubscribeLocalEvent<DropperNeedleComponent, AfterInteractEvent>(OnAfterInteractWithNeedle);
        }


        private void OnNeedleDropped(EntityUid uid, DropperNeedleComponent needle, DroppedEvent args)
        {
            if (needle.dropper != null)
            {
                if (TryComp<DropperComponent>(needle.dropper, out var dropper))
                {
                    dropper.NeedleStatus = true;
                    dropper.Needle = null;
                    dropper.Patient = null;
                    DirtyUI(needle.dropper, dropper);
                    _popup.PopupEntity(Loc.GetString("dropper-needle-returns"), needle.dropper);
                }
            }
            QueueDel(uid);
        }

        private void OnDropperOutOfRange(EntityUid uid, DropperComponent dropper, DropperOutOfRange args)
        {
            if (dropper.Patient != null)
            {
                var dmg = _damageSystem.TryChangeDamage(dropper.Patient.Value, dropper.DamageIfExcessRange, true, false);
                if (TryComp<BloodstreamComponent>(dropper.Patient, out var bloodstream))
                {
                    _bloodstreamSystem.TryModifyBleedAmount(dropper.Patient.Value, dropper.BleedingIfExcessRange);
                }
                if (dmg is not null)
                    _adminLogger.Add(Shared.Database.LogType.Damaged, Shared.Database.LogImpact.Medium, $"Patient {ToPrettyString(dropper.Patient.Value)} sustained damage: {dmg.Total} and bleeding: {dropper.BleedingIfExcessRange} due to going beyond the dropper: {ToPrettyString(uid)} range.");
                _popup.PopupEntity(Loc.GetString("dropper-needle-out-of-range-patient"), dropper.Patient.Value, dropper.Patient.Value);
                RemoveNeedle(dropper.Patient.Value);

            }
            else if (dropper.Needle != null)
            {

                dropper.NeedleStatus = true;
                dropper.Patient = null;
                QueueDel(dropper.Needle.Value);
                dropper.Needle = null;

            }
            DirtyUI(uid, dropper);


            _popup.PopupEntity(Loc.GetString("dropper-needle-returns"), uid);
        }

        private void OnIntervalReached(EntityUid uid, DropperComponent dropper, IntervalReached args)
        {
            var outputContainer = _itemSlotsSystem.GetItemOrNull(uid, SharedDropper.OutputSlotName);
            var outputContainerInfo = BuildOutputContainerInfo(outputContainer);
            if (dropper.Patient == null || dropper.LastQuantity == 0 || dropper.NeedleStatus ||
            outputContainer == null)
                return;
            if (outputContainerInfo is null)
                return;
            if (!TryComp<SolutionTransferComponent>(outputContainer, out var solutionTransferComp) || outputContainerInfo.CurrentVolume == 0)
                return;
            if (_solution.TryGetInjectableSolution(dropper.Patient.Value, out var injectableSolution)
                && _solutionContainerSystem.TryGetDrainableSolution(outputContainer.Value, out var ownerDrain))
            {
                var actualAmount = FixedPoint2.Min(dropper.LastQuantity, FixedPoint2.Min(ownerDrain.Volume, injectableSolution.AvailableVolume));
                var solution = _solutionContainerSystem.Drain(outputContainer.Value, ownerDrain, actualAmount);
                _reactiveSystem.DoEntityReaction(dropper.Patient.Value, solution, ReactionMethod.Injection);
                _solution.Inject(dropper.Patient.Value, injectableSolution, solution);
                _adminLogger.Add(Shared.Database.LogType.Dropper, Shared.Database.LogImpact.High, $"{SolutionContainerSystem.ToPrettyString(solution)}:solution injected into {ToPrettyString(dropper.Patient.Value)}:patient by {ToPrettyString(uid):dropper}");
            }

            DirtyUI(uid, dropper);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            List<DropperComponent> toUpdate = new();
            List<DropperComponent> outOfRange = new();
            foreach (var comp in EntityQuery<DropperComponent>())
            {


                if (!comp.NeedleStatus)
                {
                    var inRange = true;
                    if (comp.Patient != null)
                    {
                        inRange = Transform(comp.Owner).Coordinates.InRange(_entManager, Transform(comp.Patient.Value).Coordinates, comp.MaxRange);
                    }
                    else if (comp.Needle != null)
                    {
                        inRange = Transform(comp.Owner).Coordinates.InRange(_entManager,Transform(comp.Needle.Value).Coordinates, comp.MaxRange);
                    }
                    if (!inRange)
                        outOfRange.Add(comp);
                }

                var timeDif = _time.CurTime - comp.LastActivation;

                if (timeDif <= TimeSpan.FromSeconds(comp.LastInterval))
                    continue;

                toUpdate.Add(comp);
                comp.LastActivation = _time.CurTime;
            }
            foreach (var dropper in outOfRange)
            {

                RaiseLocalEvent(dropper.Owner, new DropperOutOfRange(), true);
            }

            foreach (var dropper in toUpdate)
            {
                if (dropper.LastQuantity > 0 && dropper.Patient is not null)
                    RaiseLocalEvent(dropper.Owner, new IntervalReached(), true);
            }
        }
        public sealed class IntervalReached : EntityEventArgs
        {

        }
        public sealed class DropperOutOfRange : EntityEventArgs
        {

        }
        private void OnNeedleRemove(EntityUid uid, DropperConnectableComponent component, ref DropperNeedleRemovedEvent args)
        {
            RemoveNeedle(uid);
            UpdateNeedleState(uid, component);
        }
        public void RemoveNeedle(EntityUid player)
        {
            if (!TryComp<DropperConnectableComponent>(player, out var patient))
                return;
            if (!TryComp<DropperComponent>(patient.dropper, out var dropperComp))
                return;
            if (patient.dropper == null)
                return;
            dropperComp.Patient = null;
            dropperComp.NeedleStatus = true;
            dropperComp.Needle = null;
            _popup.PopupEntity(Loc.GetString("dropper-needle-returns"), dropperComp.Owner);
            DirtyUI(patient.dropper.Value, dropperComp);
            patient.dropper = null;
            UpdateNeedleState(player, patient);

        }

        private void OnAfterInteractWithNeedle(EntityUid uid, DropperNeedleComponent component, AfterInteractEvent args)
        {
            if (args.Target == null || !args.CanReach || !HasComp<BodyComponent>(args.Target))
                return;
            if (!TryComp<DropperComponent>(component.dropper, out var dropper))
                return;
            if (!TryComp<DropperConnectableComponent>(args.Target, out var player))
                return;
            var inRange = Transform(component.dropper).Coordinates.InRange(_entManager,Transform(args.Target.Value).Coordinates, dropper.MaxRange);
            if (!inRange)
            {
                _popup.PopupEntity(Loc.GetString("dropper-needle-out-of-range"), args.Target.Value, args.User);
                return;
            }
            player.dropper = component.dropper;
            dropper.Patient = args.Target;
            dropper.Needle = null;
            _adminLogger.Add(Shared.Database.LogType.Dropper, Shared.Database.LogImpact.Medium, $"{ToPrettyString(args.User)} injected {ToPrettyString(component.dropper)} needle into {ToPrettyString(args.Target.Value)}");
            _popup.PopupEntity(Loc.GetString("dropper-needle-inserted-into-patient"), args.Target.Value, args.User);

            QueueDel(uid);
            DirtyUI(component.dropper, dropper);
            UpdateNeedleState(args.Target.Value, player);
        }
        public void UpdateNeedleState(EntityUid uid, DropperConnectableComponent component)
        {
            if (component.dropper != null)
                _alerts.ShowAlert(uid, AlertType.ConnectedToDropper);
            else
                _alerts.ClearAlert(uid, AlertType.ConnectedToDropper);

        }

        private void OnDropperInteractUsing(EntityUid uid, DropperComponent component, InteractUsingEvent args)
        {
            if (!HasComp<DropperNeedleComponent>(args.Used))
            {
                return;
            }
            if (component.NeedleStatus)
            {
                _popup.PopupEntity(Loc.GetString("comp-dropper-popup-needle-already-inserted"), uid);
                return;
            }
            if(!TryComp<DropperNeedleComponent>(args.Used, out var needleComp))
            {
                return;
            }
            if (needleComp.dropper != uid){
                _popup.PopupEntity(Loc.GetString("comp-dropper-popup-needle-different-dropper"), uid);
                return;
            }
            component.Needle = null;
            _popup.PopupEntity(Loc.GetString("dropper-needle-returns"), uid);
            QueueDel(args.Used);
            component.NeedleStatus = true;
            DirtyUI(uid, component);
        }

        private void OnFrequencyChange(EntityUid uid, DropperComponent component, DropperChangeFrequencyMessage args)
        {
            if (MathHelper.CloseToPercent(args.Frequency, component.LastInterval) || args.Session.AttachedEntity is null)
                return;
            _adminLogger.Add(Shared.Database.LogType.Dropper, Shared.Database.LogImpact.Medium, $"{ToPrettyString(args.Session.AttachedEntity.Value)} change frequency to {args.Frequency} on dropper: {ToPrettyString(uid)}");
            component.LastInterval = args.Frequency;
            DirtyUI(uid, component);
        }
        private void OnQuantityChange(EntityUid uid, DropperComponent component, DropperChangeQuantityMessage args)
        {
            if (MathHelper.CloseToPercent(args.Quantity, component.LastQuantity) || args.Session.AttachedEntity is null)
                return;
            _adminLogger.Add(Shared.Database.LogType.Dropper, Shared.Database.LogImpact.Medium, $"{ToPrettyString(args.Session.AttachedEntity.Value)} change quantity to {args.Quantity} on dropper: {ToPrettyString(uid)}");
            component.LastQuantity = args.Quantity;
            DirtyUI(uid, component);
        }

        private void OnDropperNeedleEject(EntityUid uid, DropperComponent component, DropperNeedleEjectMessage args)
        {
            if (!component.NeedleStatus)
                return;
            if (args.Session.AttachedEntity == null){
                return;
            }
            if (!_hands.TryGetEmptyHand(args.Session.AttachedEntity.Value, out var hand))
            {
                return;
            }
            var xform = Transform(uid);

            var spawned = Spawn(component.NeedlePrototype, xform.Coordinates);
            var needleComp = _entManager.GetComponent<DropperNeedleComponent>(spawned);
            needleComp.dropper = uid;
            component.Needle = spawned;
            _hands.TryPickupAnyHand(args.Session.AttachedEntity.Value, spawned);
            component.NeedleStatus = false;
            DirtyUI(uid, component);

        }

        private void OnSolutionPackRemoved(EntityUid uid, DropperComponent component, EntRemovedFromContainerMessage args)
        {
            if (args.Container.ID != SharedDropper.OutputSlotName)
                return;

            DirtyUI(uid, component);

            _appearance.SetData(uid, DropperVisuals.PackInserted, false);
        }

        private void DirtyUI(EntityUid uid, DropperComponent? dropper = null, ContainerManagerComponent? containerManager = null)
        {
            if (!Resolve(uid, ref dropper, ref containerManager))
                return;
            var patient = Loc.GetString("comp-dropper-ui-needle-not-patient");
            if (dropper.Patient != null)
                patient = Name(dropper.Patient.Value);
            var solutionPackStatus = false;
            var interval = dropper.LastInterval;
            var quantity = dropper.LastQuantity;
            var outputContainer = _itemSlotsSystem.GetItemOrNull(uid, SharedDropper.OutputSlotName);
            var outputContainerInfo = BuildOutputContainerInfo(outputContainer);
            if (containerManager.TryGetContainer(SharedDropper.OutputSlotName, out var solutionPack)
            && solutionPack.ContainedEntities.Count > 0)
            {

                solutionPackStatus = true;

            }
            _ui.TrySetUiState(uid, DropperUiKey.Key,
                new DropperBoundUserInterfaceState(quantity, interval, patient, dropper.NeedleStatus, solutionPackStatus,
                    dropper.MinQuantity, dropper.MaxQuantity,
                    dropper.MinInterval, dropper.MaxInterval,
                    outputContainerInfo));
        }
        private ContainerInfo? BuildOutputContainerInfo(EntityUid? container)
        {
            if (container is not { Valid: true })
                return null;

            if (_solutionContainerSystem.TryGetFitsInDispenser(container.Value, out var solution))
            {
                var reagents = solution.Contents.Select(reagent => (reagent.ReagentId, reagent.Quantity)).ToList();
                return new ContainerInfo(Name(container.Value), true, solution.Volume, solution.MaxVolume, reagents);
            }

            return null;
        }
        private void OnSolutionPackInserted(EntityUid uid, DropperComponent component, EntInsertedIntoContainerMessage args)
        {
            if (args.Container.ID != SharedDropper.OutputSlotName)
                return;
            DirtyUI(uid, component);

            _appearance.SetData(uid, DropperVisuals.PackInserted, true);
        }

        private void OnDropperStartup(EntityUid uid, DropperComponent component, ComponentStartup args)
        {
            var containerManager = EnsureComp<ContainerManagerComponent>(uid);
            _container.EnsureContainer<ContainerSlot>(uid, SharedDropper.OutputSlotName, containerManager);
            DirtyUI(uid, component);
        }
    }
}
