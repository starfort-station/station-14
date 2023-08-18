namespace Content.Server.Medical.Dropper;

[RegisterComponent, Access(typeof(DropperSystem))]
public sealed class DropperComponent : Component
{

    [DataField("maxQuantity")]
    public float MaxQuantity { get; set; } = 10f;

    [DataField("maxInterval")]
    public float MaxInterval { get; set; } = 60f;

    [DataField("minQuantity")]
    public float MinQuantity { get; set; } = 0f;

    [DataField("minInterval")]
    public float MinInterval { get; set; } = 0.5f;

    //[ViewVariables(VVAccess.ReadWrite)]
    //public string ContainerName { get; set; } = "dropperSlot";

    public float LastInterval { get; set; } = 1f;

    public float LastQuantity { get; set; } = 0f;

}
