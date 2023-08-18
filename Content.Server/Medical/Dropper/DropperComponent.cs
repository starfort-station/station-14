namespace Content.Server.Medical.Dropper;

[RegisterComponent, Access(typeof(DropperSystem))]
public sealed class DropperComponent : Component
{

    [DataField("maxQuantity")]
    public float MaxQuantity { get; set; } = 15f;

    [DataField("maxFrequency")]
    public float MaxFrequency { get; set; } = 60f;

    [DataField("minQuantity")]
    public float MinQuantity { get; set; } = 0f;

    [DataField("minFrequency")]
    public float MinFrequency { get; set; } = 0.1f;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("container")]
    public string ContainerName { get; set; } = "dropperSlot";

    public float LastFrequency { get; set; } = 0.1f;

    public float LastQuantity { get; set; } = 0f;

}
