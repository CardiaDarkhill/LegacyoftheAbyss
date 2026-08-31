using LegacyoftheAbyss.Shade;
using Xunit;

/// <summary>
/// The two things the new-game screen decides on the player's behalf: whether a slot has anything
/// worth offering to erase, and that erasing it actually erases it. Both are destructive and both
/// are one wrong index away from clearing the neighbouring save file, so neither is left to a
/// play-through to verify.
/// </summary>
[Collection(ShadeRuntimeCollection.Name)]
public class ShadeNewGameTests
{
    /// <summary>
    /// Order matters: clearing the runtime writes the live inventory back into the active slot, so
    /// the repository has to be wiped after that rather than before it.
    /// </summary>
    private static void StartFromNothing()
    {
        ShadeRuntime.Clear();
        ShadeRuntime.SaveSlots.ResetAll();
    }

    [Fact]
    public void AnUntouchedSlotHasNothingToReset()
    {
        StartFromNothing();

        Assert.False(ShadeRuntime.SlotHasShadeProgress(0));
        Assert.False(ShadeRuntime.SlotHasShadeProgress(1));
    }

    [Fact]
    public void ResettingASlotClearsItAndLeavesTheOthersAlone()
    {
        StartFromNothing();

        try
        {
            ShadeRuntime.SaveSlots.MarkCharmCollected(0, ShadeCharmId.SoulCatcher);
            ShadeRuntime.SaveSlots.MarkCharmCollected(1, ShadeCharmId.SoulEater);
            ShadeRuntime.SaveSlots.SetNotchCapacity(0, 5);

            Assert.True(ShadeRuntime.SlotHasShadeProgress(0));
            Assert.True(ShadeRuntime.SlotHasShadeProgress(1));

            ShadeRuntime.ResetSlotProgress(0);

            Assert.False(ShadeRuntime.SlotHasShadeProgress(0));
            Assert.Empty(ShadeRuntime.SaveSlots.GetCollectedCharms(0));

            // The neighbour is the whole reason this is tested: the game numbers its profiles from
            // one and this repository indexes from zero.
            Assert.True(ShadeRuntime.SlotHasShadeProgress(1));
            Assert.Contains(ShadeCharmId.SoulEater, ShadeRuntime.SaveSlots.GetCollectedCharms(1));
        }
        finally
        {
            StartFromNothing();
        }
    }
}
