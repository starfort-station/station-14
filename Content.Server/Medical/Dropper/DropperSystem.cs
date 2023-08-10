

namespace Content.Server.Medical.Dropper
{
    public sealed class DropperSystem : EntitySystem
    {

        public override void Initialize()
        {
            base.Initialize();
            //SubscribeLocalEvent<HealthAnalyzerComponent, AfterInteractEvent>(OnAfterInteract);
            //SubscribeLocalEvent<HealthAnalyzerComponent, HealthAnalyzerDoAfterEvent>(OnDoAfter);
        }


    }
}
