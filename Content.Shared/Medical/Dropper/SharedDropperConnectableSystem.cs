using Content.Shared.Alert;
using Content.Shared.Verbs;
//using Content.Server.Medical.Dropper;

namespace Content.Shared.Medical.Dropper
{
    public abstract class SharedDropperConnectableSystem : EntitySystem
    {

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<DropperConnectableComponent, GetVerbsEvent<Verb>>(AddDropperNeedleRemoveVerb);


        }

        private void AddDropperNeedleRemoveVerb(EntityUid uid, DropperConnectableComponent component, GetVerbsEvent<Verb> args)
        {
            if (!args.CanAccess || component.dropper == null || !args.CanInteract)
                return;
            Verb verb = new()
            {
                Act = () => RemoveNeedle(uid),
                DoContactInteraction = true,
                Text = Loc.GetString("dropper-needle-remove-patient")
            };
            args.Verbs.Add(verb);
        }
        public void RemoveNeedle(EntityUid user)
        {
            var attempt = new DropperNeedleRemovedEvent(user);
            RaiseLocalEvent(user, ref attempt, true);
        }

    }
}
