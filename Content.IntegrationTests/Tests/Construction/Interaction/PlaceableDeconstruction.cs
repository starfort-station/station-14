using System.Threading.Tasks;
using Content.IntegrationTests.Tests.Interaction;
using Content.Shared.Placeable;
using NUnit.Framework;

namespace Content.IntegrationTests.Tests.Construction.Interaction;

public sealed class PlaceableDeconstruction : InteractionTest
{
    /// <summary>
    /// Checks that you can deconstruct placeable surfaces (i.e., placing a wrench on a table does not take priority).
    /// </summary>
    [Test]
    public async Task DeconstructTable()
    {
        await StartDeconstruction("Table");
        Assert.That(Comp<PlaceableSurfaceComponent>().IsPlaceable);
        await Interact(Wrench);
        AssertPrototype("TableFrame");
        await Interact(Wrench);
        AssertDeleted();
        await AssertEntityLookup((Steel, 1), (Rod, 2));
    }
}

