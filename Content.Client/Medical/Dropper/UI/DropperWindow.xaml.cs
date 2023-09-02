using System;
using Content.Shared.Medical.Dropper;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Input;
using Range = Robust.Client.UserInterface.Controls.Range;

namespace Content.Client.Medical.Dropper.UI
{
    /// <summary>
    /// Client-side UI used to control a dropper.
    /// </summary>
    [GenerateTypedNameReferences]
    public sealed partial class DropperWindow : DefaultWindow
    {
        public event Action? DropperNeedleEjectButtonPressed;
        public event Action? DropperSolutionEjectButtonPressed;
        public event Action<float>? QuantitySet;
        public event Action<float>? FrequencySet;
        public DropperWindow()
        {
            RobustXamlLoader.Load(this);
            DropperNeedleEjectButton.OnPressed += _ => DropperNeedleEjectButtonPressed?.Invoke();
            DropperSolutionEjectButton.OnPressed += _ => DropperSolutionEjectButtonPressed?.Invoke();
            QuantitySlider.OnKeyBindUp += OnQuantitySliderReleased;
            QuantitySlider.OnValueChanged += OnQuantitySliderChanged;
            Quantity.OnValueChanged += OnQuantityChanged;

            FrequencySlider.OnKeyBindUp += OnFrequencySliderReleased;
            FrequencySlider.OnValueChanged += OnFrequencySliderChanged;
            Frequency.OnValueChanged += OnFrequencyChanged;

        }

        private void OnFrequencyChanged(FloatSpinBox.FloatSpinBoxEventArgs args)
        {
            var value = Math.Clamp(args.Value, FrequencySlider.MinValue, FrequencySlider.MaxValue);

            FrequencySlider.SetValueWithoutEvent(value);
            FrequencySet?.Invoke(value);
        }

        private void OnFrequencySliderChanged(Range range)
        {
            Frequency.Value = range.Value;
        }

        private void OnFrequencySliderReleased(GUIBoundKeyEventArgs args)
        {
            if (args.Function != EngineKeyFunctions.UIClick)
                return;

            FrequencySet?.Invoke(FrequencySlider.Value);
        }

        private void OnQuantityChanged(FloatSpinBox.FloatSpinBoxEventArgs args)
        {
            var value = Math.Clamp(args.Value, QuantitySlider.MinValue, QuantitySlider.MaxValue);

            QuantitySlider.SetValueWithoutEvent(value);
            QuantitySet?.Invoke(value);
        }

        private void OnQuantitySliderChanged(Range range)
        {
            Quantity.Value = range.Value;
        }

        private void OnQuantitySliderReleased(GUIBoundKeyEventArgs args)
        {
            if (args.Function != EngineKeyFunctions.UIClick)
                return;

            QuantitySet?.Invoke(QuantitySlider.Value);
        }
        public void SetDropperNeedleStatus(string status)
        {
            DropperNeedleStatus.Text = status;
        }
        public void SetDropperSolutionPackResidual(string residual)
        {
            DropperSolutionPackResidual.Text = residual;//$"{residual}/{maxVolume}";
        }
        public void SetDropperSolutionPackStatus(bool status){
            if (status)
            {
                DropperSolutionPackStatus.Text = Loc.GetString("comp-dropper-ui-solution-pack-inserted");
            }
            else
            {
                DropperSolutionPackStatus.Text = Loc.GetString("comp-dropper-ui-solution-pack-not-inserted");
                SetDropperSolutionPackResidual(Loc.GetString("comp-dropper-ui-solution-pack-null-residual"));
            }
        }
        public void SetQuantityRange(float min, float max)
        {
            QuantitySlider.MinValue = min;
            QuantitySlider.MaxValue = max;
        }
        public void SetFrequencyRange(float min, float max)
        {
            FrequencySlider.MinValue = min;
            FrequencySlider.MaxValue = max;
        }
        public void SetQuantity(float quantity){
            if (MathHelper.CloseTo(quantity, Quantity.Value))
                return;

            if (!QuantitySlider.Grabbed)
                QuantitySlider.SetValueWithoutEvent(quantity);
            Quantity.Value = quantity;
        }
        public void SetFrequency(float frequency){
            if (MathHelper.CloseTo(frequency, Frequency.Value))
                return;

            if (!FrequencySlider.Grabbed)
                FrequencySlider.SetValueWithoutEvent(frequency);
            Frequency.Value = frequency;
        }
        public void UpdateContainerInfo(DropperBoundUserInterfaceState state){
            if (state.OutputContainer is null)
                return;
            DropperSolutionPackResidual.Text = $"{state.OutputContainer.CurrentVolume}/{state.OutputContainer.MaxVolume}";
        }
        public void UpdateState(BoundUserInterfaceState state){
            var castState = (DropperBoundUserInterfaceState) state;
            UpdateContainerInfo(castState);
            DropperSolutionEjectButton.Disabled = castState.OutputContainer is null;
            DropperNeedleEjectButton.Disabled = !castState.DropperNeedleStatus;
        }
    }
}
