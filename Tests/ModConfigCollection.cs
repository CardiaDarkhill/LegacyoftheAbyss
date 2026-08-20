using Xunit;

/// <summary>
/// Tests that read or write the process-wide <see cref="ModConfig.Instance"/> must not run in
/// parallel with each other — <c>ModConfig.Load()</c> swaps the singleton out from under them.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ModConfigCollection
{
    public const string Name = "ModConfig";
}
