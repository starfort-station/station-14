using Robust.Shared.Serialization;

namespace Content.Shared.Medical.Dropper
{
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
        /*public string CanisterLabel { get; }
        public float CanisterPressure { get; }
        public bool PortStatus { get; }
        public string? TankLabel { get; }
        public float TankPressure { get; }
        public float ReleasePressure { get; }
        public bool ReleaseValve { get; }
        public float ReleasePressureMin { get; }
        public float ReleasePressureMax { get; }

        public GasCanisterBoundUserInterfaceState(string canisterLabel, float canisterPressure, bool portStatus, string? tankLabel, float tankPressure, float releasePressure, bool releaseValve, float releaseValveMin, float releaseValveMax)
        {
            CanisterLabel = canisterLabel;
            CanisterPressure = canisterPressure;
            PortStatus = portStatus;
            TankLabel = tankLabel;
            TankPressure = tankPressure;
            ReleasePressure = releasePressure;
            ReleaseValve = releaseValve;
            ReleasePressureMin = releaseValveMin;
            ReleasePressureMax = releaseValveMax;
        }*/
    }

}
