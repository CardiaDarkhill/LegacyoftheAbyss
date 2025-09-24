#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal interface IShadeCharmPlacementHandler
    {
        void Populate(in ShadeCharmPlacementContext context, IReadOnlyList<ShadeCharmPlacementDefinition> placements);
    }

    internal readonly struct ShadeCharmPlacementContext
    {
        internal ShadeCharmPlacementContext(string sceneName, Transform hero)
        {
            SceneName = sceneName;
            Hero = hero;
        }

        internal string SceneName { get; }

        internal Transform Hero { get; }
    }
}
