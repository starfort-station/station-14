namespace Content.Server.Medical.Dropper;

[RegisterComponent, Access(typeof(DropperSystem))]
public sealed class DropperComponent : Component
{

    [DataField("quantity")]
    public float quantity { get; set; } = 0;

    [DataField("frequency")]
    public float frequency { get; set; } = 0;
}
