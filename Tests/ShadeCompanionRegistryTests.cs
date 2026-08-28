#nullable enable

using System.Linq;
using LegacyoftheAbyss.Shade;
using Xunit;

/// <summary>
/// The registry is process-wide static state, so it shares the serialised collection with the rest
/// of the ShadeRuntime tests and each test clears the secondaries it created.
/// </summary>
[Collection(ShadeRuntimeCollection.Name)]
public class ShadeCompanionRegistryTests
{
    [Fact]
    public void PrimaryAlwaysExistsSoTheStaticRuntimeApiHasSomethingToDelegateTo()
    {
        ShadeCompanionRegistry.Clear();

        Assert.NotNull(ShadeCompanionRegistry.Primary);
        Assert.Equal(ShadeCompanionRegistry.PrimaryId, ShadeCompanionRegistry.Primary.Id);
        Assert.True(ShadeCompanionRegistry.Primary.IsPrimary);
        Assert.Equal(1, ShadeCompanionRegistry.Count);
    }

    [Fact]
    public void RuntimeStateIsThePrimaryCompanionsState()
    {
        ShadeCompanionRegistry.Clear();

        Assert.Same(ShadeCompanionRegistry.Primary.State, ShadeRuntime.PersistentState);
        Assert.Same(ShadeCompanionRegistry.Primary.Charms, ShadeRuntime.Charms);
    }

    [Fact]
    public void SecondaryCompanionsKeepTheirOwnHealthAndCharms()
    {
        ShadeCompanionRegistry.Clear();
        var primary = ShadeCompanionRegistry.Primary;
        var second = ShadeCompanionRegistry.CreateNext();

        try
        {
            primary.State.Capture(3, 5, 0, 0, 40);
            second.State.Capture(1, 9, 0, 0, 12);

            Assert.Equal(3, primary.State.CurrentHP);
            Assert.Equal(5, primary.State.MaxHP);
            Assert.Equal(1, second.State.CurrentHP);
            Assert.Equal(9, second.State.MaxHP);

            primary.Charms.GrantCharm(ShadeCharmId.ShamanStone);

            Assert.Contains(ShadeCharmId.ShamanStone, primary.Charms.GetOwnedCharms());
            Assert.DoesNotContain(ShadeCharmId.ShamanStone, second.Charms.GetOwnedCharms());
        }
        finally
        {
            ShadeCompanionRegistry.Clear();
        }
    }

    [Fact]
    public void CreateNextFillsTheLowestFreeSlot()
    {
        ShadeCompanionRegistry.Clear();

        try
        {
            var first = ShadeCompanionRegistry.CreateNext();
            var second = ShadeCompanionRegistry.CreateNext();
            Assert.Equal(1, first.Id);
            Assert.Equal(2, second.Id);

            ShadeCompanionRegistry.Remove(first.Id);
            var reused = ShadeCompanionRegistry.CreateNext();
            Assert.Equal(1, reused.Id);
        }
        finally
        {
            ShadeCompanionRegistry.Clear();
        }
    }

    [Fact]
    public void ClearDropsSecondariesButKeepsThePrimary()
    {
        ShadeCompanionRegistry.Clear();
        ShadeCompanionRegistry.CreateNext();
        ShadeCompanionRegistry.CreateNext();
        Assert.Equal(3, ShadeCompanionRegistry.Count);

        ShadeCompanionRegistry.Clear();

        Assert.Equal(1, ShadeCompanionRegistry.Count);
        Assert.True(ShadeCompanionRegistry.Primary.IsPrimary);
    }

    [Fact]
    public void PrimaryCannotBeRemoved()
    {
        ShadeCompanionRegistry.Clear();

        Assert.False(ShadeCompanionRegistry.Remove(ShadeCompanionRegistry.PrimaryId));
        Assert.Equal(1, ShadeCompanionRegistry.Count);
    }

    [Fact]
    public void CharacterIsPerCompanionSoAKnightAndAShadeCanCoexist()
    {
        ShadeCompanionRegistry.Clear();
        var primary = ShadeCompanionRegistry.Primary;
        var second = ShadeCompanionRegistry.CreateNext(ShadeCharacterId.Knight);

        try
        {
            Assert.Equal(ShadeCharacterId.Shade, primary.Character);
            Assert.Equal(ShadeCharacterId.Knight, second.Character);
            Assert.Equal(ShadeRenderBackend.SpriteSheets, primary.CharacterDefinition.RenderBackend);
            Assert.Equal(ShadeRenderBackend.AssetBundle, second.CharacterDefinition.RenderBackend);
        }
        finally
        {
            ShadeCompanionRegistry.Clear();
        }
    }

    [Fact]
    public void AppearanceChangedFiresOnlyOnAnActualChange()
    {
        ShadeCompanionRegistry.Clear();
        var companion = ShadeCompanionRegistry.Primary;
        int raised = 0;
        void Handler(ShadeCompanion _) => raised++;

        companion.AppearanceChanged += Handler;
        try
        {
            companion.Character = ShadeCharacterId.Shade;
            Assert.Equal(0, raised);

            companion.Character = ShadeCharacterId.Knight;
            Assert.Equal(1, raised);

            companion.SkinId = "Cozy Shade";
            Assert.Equal(2, raised);

            companion.SkinId = "cozy shade";
            Assert.Equal(2, raised);
        }
        finally
        {
            companion.AppearanceChanged -= Handler;
            companion.Character = ShadeCharacterId.Shade;
            companion.SkinId = null;
            ShadeCompanionRegistry.Clear();
        }
    }

    [Fact]
    public void CharmsChangedIsTaggedWithTheCompanionThatChanged()
    {
        ShadeCompanionRegistry.Clear();
        var second = ShadeCompanionRegistry.CreateNext();
        ShadeCompanion? reported = null;
        void Handler(ShadeCompanion c) => reported = c;

        second.CharmsChanged += Handler;
        try
        {
            second.Charms.GrantCharm(ShadeCharmId.Longnail);
            Assert.Same(second, reported);
        }
        finally
        {
            second.CharmsChanged -= Handler;
            ShadeCompanionRegistry.Clear();
        }
    }

    [Fact]
    public void UnknownPersistedCharacterIdFallsBackToTheShade()
    {
        Assert.Equal(ShadeCharacterId.Shade, ShadeCharacterRegistry.Resolve(null).Id);
        Assert.Equal(ShadeCharacterId.Shade, ShadeCharacterRegistry.Resolve("Zote").Id);
        Assert.Equal(ShadeCharacterId.Knight, ShadeCharacterRegistry.Resolve("knight").Id);
        Assert.Equal(ShadeCharacterId.Knight, ShadeCharacterRegistry.Resolve("Knight").Id);
    }
}
