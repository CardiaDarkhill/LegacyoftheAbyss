using LegacyoftheAbyss.Shade;
using Xunit;

public class ShadeNotificationTests
{
    public ShadeNotificationTests()
    {
        ShadeRuntime.Clear();
    }

    [Fact]
    public void EnqueueNotificationSuppressesDuplicates()
    {
        ShadeRuntime.Clear();

        bool first = ShadeRuntime.EnqueueNotification("test-key", "Test message", ShadeUnlockNotificationType.Ability);
        bool second = ShadeRuntime.EnqueueNotification("test-key", "Test message", ShadeUnlockNotificationType.Ability);

        Assert.True(first);
        Assert.False(second);
        Assert.True(ShadeRuntime.HasPendingNotifications);

        Assert.True(ShadeRuntime.TryDequeueNotification(out var notification));
        Assert.Equal("Test message", notification.Message);
        Assert.False(ShadeRuntime.HasPendingNotifications);
    }

    [Fact]
    public void SpellProgressQueuesNotification()
    {
        ShadeRuntime.Clear();
        ShadeRuntime.SyncSpellProgress(0);
        ShadeRuntime.NotifyHornetSpellUnlocked();

        Assert.True(ShadeRuntime.HasPendingNotifications);
        Assert.True(ShadeRuntime.TryDequeueNotification(out var notification));
        Assert.Contains("Shade", notification.Message);
    }
}

