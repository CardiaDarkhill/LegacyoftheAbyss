using Xunit;

public class ModConfigTests
{
    [Fact]
    public void SavesAndLoadsDifficulty()
    {
        var cfg = ModConfig.Instance;
        cfg.hornetDamageMultiplier = 1.5f;
        cfg.shadeDamageMultiplier = 0.8f;
        cfg.bindHornetHeal = 4;
        cfg.bindShadeHeal = 1;
        cfg.focusHornetHeal = 2;
        cfg.focusShadeHeal = 3;
        cfg.logDamage = true;
        ModConfig.Save();
        var loaded = ModConfig.Load();
        Assert.Equal(1.5f, loaded.hornetDamageMultiplier, 3);
        Assert.Equal(0.8f, loaded.shadeDamageMultiplier, 3);
        Assert.Equal(4, loaded.bindHornetHeal);
        Assert.Equal(1, loaded.bindShadeHeal);
        Assert.Equal(2, loaded.focusHornetHeal);
        Assert.Equal(3, loaded.focusShadeHeal);
        Assert.True(loaded.logDamage);
    }
}
