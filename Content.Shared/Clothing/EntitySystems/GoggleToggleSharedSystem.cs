using Content.Shared.Actions;
using Content.Shared.Alert;
using Content.Shared.Clothing.Components;
using Content.Shared.Eye.DarkVision;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.Containers;

namespace Content.Shared.Clothing;

public abstract class GoggleToggleSharedSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly SharedActionsSystem _sharedActions = default!;
    [Dependency] private readonly SharedContainerSystem _sharedContainer = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GoggleToggleComponent, GetItemActionsEvent>(OnGetActions);
        SubscribeLocalEvent<GoggleToggleComponent, ToggleGogglesEvent>(OnToggleAction);
    }

    private void OnToggleAction(EntityUid uid, GoggleToggleComponent goggles, ToggleGogglesEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        goggles.On = !goggles.On;

        if (_sharedContainer.TryGetContainingContainer(uid, out var container))
        {
            UpdateGogglesState(container.Owner, goggles);
        }

        if (goggles.ToggleAction == null)
            return;

        _sharedActions.SetToggled(goggles.ToggleAction, goggles.On);
        Dirty(goggles);
    }

    protected virtual void UpdateGogglesState(EntityUid uid, GoggleToggleComponent goggles) { }

    private void OnGetActions(EntityUid uid, GoggleToggleComponent component, GetItemActionsEvent args)
    {
        if (component.ToggleAction != null && args.SlotFlags == SlotFlags.EYES)
            args.Actions.Add(component.ToggleAction);
    }
}
