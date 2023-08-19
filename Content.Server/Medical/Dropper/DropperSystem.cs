

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

namespace Content.Server.Medical.Dropper
{
    public sealed class DropperSystem : EntitySystem
    {
        [Dependency] private readonly SharedContainerSystem _container = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly UserInterfaceSystem _ui = default!;
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
        [Dependency] private readonly SolutionContainerSystem _solutionContainerSystem = default!;
        [Dependency] private readonly PopupSystem _popup = default!;
        [Dependency] private readonly SharedHandsSystem _hands = default!;
        public override void Initialize()
        {
            base.Initialize();
            //SubscribeLocalEvent<HealthAnalyzerComponent, AfterInteractEvent>(OnAfterInteract);
            //SubscribeLocalEvent<HealthAnalyzerComponent, HealthAnalyzerDoAfterEvent>(OnDoAfter);
            SubscribeLocalEvent<DropperComponent, ComponentStartup>(OnDropperStartup);
            SubscribeLocalEvent<DropperComponent, EntInsertedIntoContainerMessage>(OnSolutionPackInserted);
            SubscribeLocalEvent<DropperComponent, EntRemovedFromContainerMessage>(OnSolutionPackRemoved);
            SubscribeLocalEvent<DropperComponent, InteractUsingEvent>(OnDropperInteractUsing);
            // При вводе вещества если открыть UI сделать динамический показ остатка пакета!!
            // UI
            //SubscribeLocalEvent<DropperComponent, DropperSolutionEjectMessage>(OnSolutionPackEject);
            SubscribeLocalEvent<DropperComponent, DropperNeedleEjectMessage>(OnDropperNeedleEject);
            SubscribeLocalEvent<DropperComponent, DropperChangeFrequencyMessage>(OnFrequencyChange);
            SubscribeLocalEvent<DropperComponent, DropperChangeQuantityMessage>(OnQuantityChange);
        }

        private void OnDropperInteractUsing(EntityUid uid, DropperComponent component, InteractUsingEvent args)
        {
            if (!HasComp<DropperNeedleComponent>(args.Used)){
                return;
            }
            if (component.NeedleStatus)
            {
                _popup.PopupEntity(Loc.GetString("comp-dropper-popup-needle-already-inserted"), uid);
                return;
            }
            QueueDel(args.Used);
            component.NeedleStatus = true;
        }

        private void OnFrequencyChange(EntityUid uid, DropperComponent component, DropperChangeFrequencyMessage args)
        {
            if (MathHelper.CloseToPercent(args.Frequency, component.LastInterval))
                return;
            component.LastInterval = args.Frequency;
            DirtyUI(uid, component);
        }
        private void OnQuantityChange(EntityUid uid, DropperComponent component, DropperChangeQuantityMessage args)
        {
            if (MathHelper.CloseToPercent(args.Quantity, component.LastQuantity))
                return;
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
            _hands.TryPickupAnyHand(args.Session.AttachedEntity.Value, spawned);
            //_hands.CanPickupAnyHand(args.Session.AttachedEntity, )
            component.NeedleStatus = false;

        }

        //private void OnSolutionPackEject(EntityUid uid, DropperComponent component, DropperSolutionEjectMessage args)
        //{
            //if (!TryComp<ContainerManagerComponent>(uid, out var containerManager)
            //    || !containerManager.TryGetContainer(component.ContainerName, out var container))
            //    return;
            //if(container.ContainedEntities.Count == 0)
            //    return;
            //container.Remove(container.ContainedEntities[0]);
            //DirtyUI(uid, component);
        //}

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
            string needleStatus = "еблан";
            var solutionPackStatus = false;
            var interval = dropper.LastInterval;
            var quantity = dropper.LastQuantity;
            var outputContainer = _itemSlotsSystem.GetItemOrNull(uid, SharedDropper.OutputSlotName);
            var outputContainerInfo = BuildOutputContainerInfo(outputContainer);
            if (containerManager.TryGetContainer(SharedDropper.OutputSlotName, out var solutionPack)
            && solutionPack.ContainedEntities.Count > 0)
            {
                // var pack = solutionPack.ContainedEntities[0];
                //var packComponent = Comp<SolutionContainerManagerComponent>(pack);
                //_ui.
                solutionPackStatus = true;

            }
            _ui.TrySetUiState(uid, DropperUiKey.Key,
                new DropperBoundUserInterfaceState(quantity, interval, needleStatus, solutionPackStatus,
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
