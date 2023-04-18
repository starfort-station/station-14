using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Content.Server.Shuttles.Components;
using Content.Server.Expedition;
using System.Threading.Tasks;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Content.Server.Salvage;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.GameObjects;

namespace Content.Server.StationEvents.Events;

public sealed class ExpeditionMapSpawn : StationEventSystem
{
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override string Prototype => "ExpeditionMapSpawn";

    public override void Added()
    {
        base.Added();

        // Announce event in chat
        var str = Loc.GetString("expedition-map-spawn-event-announcement");
        ChatSystem.DispatchGlobalAnnouncement(str, colorOverride: Color.FromHex("#6b8e23"));
    }

    public override void Started()
    {
        base.Started();

        // find free mapID or maps more than 25, crazy
        var smallestValue = 1;
        for (; smallestValue < 25; ++smallestValue)             // 25 is magic number
        {
            if (MapManager.MapExists(new MapId(smallestValue)))
                continue;
            break;
        }

        if (smallestValue == 25)
        {
            Logger.Error("ExpeditionMapSpawn event cant spawn map, because maps already more than 24");
            return;
        }

        var mapId = new MapId(smallestValue);
        var allFoundMaps = PrototypeManager.EnumeratePrototypes<ExpeditionMapPrototype>().ToList();

        if (!allFoundMaps.Any())
        {
            Logger.Error("ExpeditionMapSpawn event cant spawn map, because cant find any expedition map prototype");
            return;
        }

        var index = _random.Next(allFoundMaps.Count() - 1);

        var success = _mapLoader.TryLoad(mapId, allFoundMaps[index].MapPath.ToString(), out _);
        if (!success)
            throw new Exception("Map load failed");

        var entityMap = MapManager.GetMapEntityId(mapId);

        var ftlPoint = _entMan.CreateEntityUninitialized("FTLPoint");

        var metadata = _entMan.GetComponent<MetaDataComponent>(ftlPoint);
        metadata.EntityName = allFoundMaps[index].FTLName;

        var ftlComponent = _entMan.GetComponent<FTLDestinationComponent>(ftlPoint);
        ftlComponent.Whitelist ??= new EntityWhitelist();
        ftlComponent.Whitelist.Tags ??= new List<string>();
        ftlComponent.Whitelist.Tags.Add("ExpeditionConsole");

        _entMan.InitializeAndStartEntity(ftlPoint, mapId);
    }

}

