using Content.Shared.Medical.Dropper;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client.Medical.Dropper.UI
{
    /// <summary>
    /// Initializes a <see cref="DropperWindow"/> and updates it when new server messages are received.
    /// </summary>
    [UsedImplicitly]
    public sealed class DropperBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private DropperWindow? _window;

        public DropperBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();

            _window = new DropperWindow();

            if (State != null)
                UpdateState(State);

            _window.OpenCentered();

            _window.OnClose += Close;
        }


        /// <summary>
        /// Update the UI state based on server-sent info
        /// </summary>
        /// <param name="state"></param>
        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            if (_window == null || state is not DropperBoundUserInterfaceState cast)
                return;

        }

    }
}
