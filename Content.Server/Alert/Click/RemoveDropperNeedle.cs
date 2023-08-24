using Content.Server.Medical.Dropper;
using Content.Shared.Alert;
using JetBrains.Annotations;

namespace Content.Server.Alert.Click
{
    /// <summary>
    ///     Try to remove handcuffs from yourself
    /// </summary>
    [UsedImplicitly]
    [DataDefinition]
    public sealed class RemoveDropperNeedle : IAlertClick
    {
        public void AlertClicked(EntityUid player)
        {
            var entityManager = IoCManager.Resolve<IEntityManager>();
            var dropperSys = entityManager.System<DropperSystem>();
            dropperSys.RemoveNeedle(player);
        }
    }
}
