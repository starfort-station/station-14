using Content.Shared.Chemistry;
using Robust.Shared.Serialization;

namespace Content.Shared.Medical.Dropper
{
    public sealed class SharedDropper
    {
        public const string OutputSlotName = "dropperSlot";
    }
    /// <summary>
    /// Key representing which <see cref="BoundUserInterface"/> is currently open.
    /// Useful when there are multiple UI for an object. Here it's future-proofing only.
    /// </summary>
    [Serializable, NetSerializable]
    public enum DropperUiKey
    {
        Key,
    }

    #region Enums

    /// <summary>
    /// Used in <see cref="DropperVisualizer"/> to determine which visuals to update.
    /// </summary>
    [Serializable, NetSerializable]
    public enum DropperVisuals
    {
        Empty,
        PackInserted
    }

    #endregion

    /// <summary>
    /// Represents a <see cref="DropperComponent"/> state that can be sent to the client
    /// </summary>
    /// <summary>
    /// Represents a <see cref="GasCanisterComponent"/> state that can be sent to the client
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class DropperBoundUserInterfaceState : BoundUserInterfaceState
    {
        public float Quantity { get; }
        public float Frequency { get; }
        public string DropperNeedleStatus { get; }
        public bool DropperSolutionPackStatus { get; }
        public float QuantityMin { get; }
        public float QuantityMax { get; }
        public float FrequencyMin { get; }
        public float FrequencyMax { get; }

        public readonly ContainerInfo? OutputContainer;

        public DropperBoundUserInterfaceState(float quantity, float frequency, string dropperNeedleStatus, bool dropperSolutionPackStatus, float quantityMin, float quantityMax, float frequencyMin, float frequencyMax, ContainerInfo? outputContainer)
        {
            Quantity = quantity;
            Frequency = frequency;
            DropperNeedleStatus = dropperNeedleStatus;
            DropperSolutionPackStatus = dropperSolutionPackStatus;
            QuantityMin = quantityMin;
            QuantityMax = quantityMax;
            FrequencyMin = frequencyMin;
            FrequencyMax = frequencyMax;
            OutputContainer = outputContainer;
        }
    }

    [Serializable, NetSerializable]
    public sealed class DropperChangeFrequencyMessage : BoundUserInterfaceMessage
    {
        public float Frequency { get; }
        public DropperChangeFrequencyMessage(float frequency)
        {
            Frequency = frequency;
        }
    }

    [Serializable, NetSerializable]
    public sealed class DropperChangeQuantityMessage : BoundUserInterfaceMessage
    {
        public float Quantity { get; }
        public DropperChangeQuantityMessage(float quantity)
        {
            Quantity = quantity;
        }
    }

    [Serializable, NetSerializable]
    public sealed class DropperSolutionEjectMessage : BoundUserInterfaceMessage
    {
        public DropperSolutionEjectMessage()
        { }
    }

    [Serializable, NetSerializable]
    public sealed class DropperNeedleEjectMessage : BoundUserInterfaceMessage
    {
        public DropperNeedleEjectMessage()
        { }
    }

}
