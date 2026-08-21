using Xunit;

/// <summary>
/// <see cref="LegacyoftheAbyss.Shade.ShadeRuntime"/> is a process-wide singleton (persistent state,
/// charm inventory, save-slot repository, debug-unlock flag) and several test classes reach into
/// game statics such as <c>GameManager._instance</c>. Running those in parallel let one class swap
/// state out from under another, which showed up as tests that pass alone and fail in a full run.
/// Every class that touches that shared state belongs in this collection.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ShadeRuntimeCollection
{
    public const string Name = "ShadeRuntime";
}
