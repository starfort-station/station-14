using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using System.ComponentModel;

namespace Content.Shared.Eye.DarkVision;

public abstract class DarkVisionSharedSystem : EntitySystem
{
    public virtual void ForceUpdate(EntityUid uid, DarkVisionComponent component) { }
}
