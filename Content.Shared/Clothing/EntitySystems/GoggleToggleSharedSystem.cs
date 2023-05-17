using Content.Shared.Actions;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.Containers;

namespace Content.Shared.Clothing.EntitySystems;

public abstract class GoggleToggleSharedSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _sharedActions = default!;
    [Dependency] private readonly SharedContainerSystem _sharedContainer = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GoggleToggleComponent, GetItemActionsEvent>(OnGetActions);
        SubscribeLocalEvent<GoggleToggleComponent, ToggleGogglesEvent>(OnToggleAction);
        SubscribeLocalEvent<GoggleToggleComponent, GotEquippedEvent>(OnGotEquipped);
        SubscribeLocalEvent<GoggleToggleComponent, GotUnequippedEvent>(OnGotUnequipped);
    }

    private void OnToggleAction(EntityUid uid, GoggleToggleComponent goggles, ToggleGogglesEvent args)
    {
        goggles.On = !goggles.On;

        OnChanged(uid, goggles);

        if (_sharedContainer.TryGetContainingContainer(uid, out var container))
        {
            UpdateGogglesState(container.Owner, goggles);
        }
    }

    protected void OnChanged(EntityUid uid, GoggleToggleComponent goggles)
    {
        if (goggles.ToggleAction == null)
            return;
        _sharedActions.SetToggled(goggles.ToggleAction, goggles.On);
    }

    protected virtual void UpdateGogglesState(EntityUid uid, GoggleToggleComponent goggles) { }

    private void OnGetActions(EntityUid uid, GoggleToggleComponent component, GetItemActionsEvent args)
    {
        if (component.ToggleAction != null && args.SlotFlags == SlotFlags.EYES)
            args.Actions.Add(component.ToggleAction);
    }

    private void OnGotUnequipped(EntityUid uid, GoggleToggleComponent component, GotUnequippedEvent args)
    {
        if (args.Slot == "eyes")
        {
            component.On = false;
            if (component.ToggleAction != null)
            {
                _sharedActions.SetToggled(component.ToggleAction, component.On);
            }

            UpdateGogglesState(args.Equipee, component);
        }
    }

    private void OnGotEquipped(EntityUid uid, GoggleToggleComponent component, GotEquippedEvent args)
    {
        if (args.Slot == "eyes")
        {
            if (component.ToggleAction != null)
            {
                _sharedActions.SetToggled(component.ToggleAction, component.On);
            }

            UpdateGogglesState(args.Equipee, component);
        }
    }
}
