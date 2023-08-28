namespace Content.Server.Medical.Dropper;
using Content.Shared.Damage;

[RegisterComponent]
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


    public float LastInterval { get; set; } = 1f;

    public float LastQuantity { get; set; } = 0f;

    public EntityUid? Patient { get; set; } = null;
    public EntityUid? Needle { get; set; } = null;

    public bool NeedleStatus { get; set; } = true;
    [DataField("dropperNeedlePrototype")]
    public string NeedlePrototype { get; set; } = "DropperNeedle";

    [DataField("maxRange")]
    public float MaxRange { get; set; } = 1.5f;

    [DataField("damageIfExcessRange")]
    public DamageSpecifier DamageIfExcessRange = new()
    {
        DamageDict = new()
             {
                 { "Piercing", 5.0 },
             }
    };
    [DataField("bleedingIfExcessRange")]
    public float BleedingIfExcessRange { get; set; } = 5f;

    public TimeSpan LastActivation { get; set; }
}
