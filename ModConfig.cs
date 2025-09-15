using System.IO;
using UnityEngine;

internal static class ModPaths
{
    internal static readonly string Root = Path.GetDirectoryName(typeof(ModPaths).Assembly.Location);
    internal static readonly string Assets = Path.Combine(Root, "Assets");
    internal static readonly string Logs = Path.Combine(Assets, "logs");
    internal static readonly string Config = Path.Combine(Assets, "config.json");
}

public class ModConfig
{
    public bool logDamage = true;
    public float hornetDamageMultiplier = 1f;
    public float shadeDamageMultiplier = 1f;
    public int bindHornetHeal = 3;
    public int bindShadeHeal = 2;
    public int focusHornetHeal = 1;
    public int focusShadeHeal = 1;

    private static ModConfig instance;

    public static ModConfig Instance => instance ??= Load();

    public static ModConfig Load()
    {
        try
        {
            if (File.Exists(ModPaths.Config))
            {
                string json = File.ReadAllText(ModPaths.Config);
                instance = UnityEngine.JsonUtility.FromJson<ModConfig>(json) ?? new ModConfig();
            }
            else
            {
                instance = new ModConfig();
                Save();
            }
        }
        catch
        {
            instance = new ModConfig();
        }
        return instance;
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ModPaths.Config));
            string json = UnityEngine.JsonUtility.ToJson(Instance, true);
            File.WriteAllText(ModPaths.Config, json);
        }
        catch { }
    }
}
