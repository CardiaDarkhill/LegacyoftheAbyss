using System;
using System.Reflection;
using System.Runtime.Serialization;
using GlobalEnums;

/// <summary>
/// Stands a bare <c>GameManager</c> and <c>PlayerData</c> up as the game's singletons for the
/// length of a test, and puts back whatever was there before.
/// <para>
/// Both are built with <see cref="FormatterServices.GetUninitializedObject"/> rather than
/// constructed: they are <c>MonoBehaviour</c>s, and <c>new</c> on one of those is an extern call
/// that throws in a plain test host. The singletons are then written through reflection because
/// <c>_instance</c> is private, which is also why <see cref="Dispose"/> has to put the previous
/// values back - xUnit shares a process across a whole collection.
/// </para>
/// <para>
/// Two fixtures grew their own copy of all of this. What they actually differ in is which handful
/// of fields the code under test reads, so that is all a subclass sets.
/// </para>
/// </summary>
internal abstract class GameStaticsScope : IDisposable
{
    private readonly object originalGameManager;
    private readonly object originalPlayerData;

    protected GameStaticsScope()
    {
        originalGameManager = GetStaticField(typeof(GameManager), "_instance");
        originalPlayerData = GetStaticField(typeof(PlayerData), "_instance");

        Gm = (GameManager)FormatterServices.GetUninitializedObject(typeof(GameManager));
        Data = (PlayerData)FormatterServices.GetUninitializedObject(typeof(PlayerData));

        SetProperty(Gm, "GameState", GameState.PLAYING);

        SetStaticField(typeof(GameManager), "_instance", Gm);
        SetStaticField(typeof(PlayerData), "_instance", Data);
    }

    protected GameManager Gm { get; }

    protected PlayerData Data { get; }

    public void Dispose()
    {
        SetStaticField(typeof(GameManager), "_instance", originalGameManager);
        SetStaticField(typeof(PlayerData), "_instance", originalPlayerData);
    }

    internal void ClearGameManager() => SetStaticField(typeof(GameManager), "_instance", null);

    protected static object GetStaticField(Type type, string name)
    {
        var field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return field?.GetValue(null);
    }

    protected static void SetStaticField(Type type, string name, object value)
    {
        var field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        field?.SetValue(null, value);
    }

    protected static void SetProperty(object target, string name, object value)
    {
        if (target == null)
        {
            return;
        }

        var property = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property?.SetValue(target, value, null);
    }
}
