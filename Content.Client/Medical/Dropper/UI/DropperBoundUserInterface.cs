using Content.Shared.Containers.ItemSlots;
using Content.Shared.Medical.Dropper;
using Content.Client.Medical.Dropper;
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
            _window.DropperNeedleEjectButtonPressed += OnDropperNeedleEjectButtonPressed;
            _window.DropperSolutionEjectButtonPressed += OnDropperSolutionEjectButtonPressed;
            _window.QuantitySet += OnQuantitySet;
            _window.FrequencySet += OnFrequencySet;
        }

        private void OnFrequencySet(float value)
        {
            SendMessage(new DropperChangeFrequencyMessage(value));
        }

        private void OnQuantitySet(float value)
        {
            SendMessage(new DropperChangeQuantityMessage(value));
        }

        private void OnDropperSolutionEjectButtonPressed()
        {
            SendMessage(new DropperSolutionEjectMessage());
            SendMessage(new ItemSlotButtonPressedEvent(SharedDropper.OutputSlotName));
        }

        private void OnDropperNeedleEjectButtonPressed()
        {
            SendMessage(new DropperNeedleEjectMessage());
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

            _window.SetDropperNeedleStatus(cast.DropperNeedleStatus);
            //_window.SetDropperSolutionPackResidual(cast.DropperSolutionPackResidual);
            _window.SetDropperSolutionPackStatus(cast.DropperSolutionPackStatus);
            _window.SetFrequency(cast.Frequency);
            _window.SetFrequencyRange(cast.FrequencyMin, cast.FrequencyMax);
            _window.SetQuantity(cast.Quantity);
            _window.SetQuantityRange(cast.QuantityMin, cast.QuantityMax);
            var castState = (DropperBoundUserInterfaceState) state;
            _window?.UpdateState(castState);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            _window?.Dispose();
        }

    }
}
