#nullable disable
using LegacyoftheAbyss.Shade;
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        public void RecomputeCharmLoadout()
        {
            maxDistance = baseMaxDistance;
            softLeashRadius = baseSoftLeashRadius;
            hardLeashRadius = baseHardLeashRadius;
            snapLeashRadius = baseSnapLeashRadius;
            sprintMultiplier = baseSprintMultiplier;
            fireCooldown = baseFireCooldown;
            nailCooldown = baseNailCooldown;
            focusSoulCost = baseFocusSoulCost;
            projectileSoulCost = baseProjectileSoulCost;
            shriekSoulCost = baseShriekSoulCost;
            quakeSoulCost = baseQuakeSoulCost;

            var inventory = ShadeRuntime.Charms;
            if (inventory == null)
            {
                return;
            }

            foreach (var charmId in inventory.GetEquipped())
            {
                switch (charmId)
                {
                    case ShadeCharmId.AbyssalCore:
                        focusSoulCost = Mathf.Max(10, Mathf.RoundToInt(baseFocusSoulCost * 0.8f));
                        break;
                    case ShadeCharmId.PhantomStride:
                        sprintMultiplier = baseSprintMultiplier + 0.75f;
                        break;
                    case ShadeCharmId.EchoOfBlades:
                        fireCooldown = Mathf.Max(0.1f, baseFireCooldown * 0.85f);
                        nailCooldown = Mathf.Max(0.12f, baseNailCooldown * 0.85f);
                        break;
                    case ShadeCharmId.DuskShroud:
                        projectileSoulCost = Mathf.Max(10, Mathf.RoundToInt(baseProjectileSoulCost * 0.85f));
                        shriekSoulCost = Mathf.Max(10, Mathf.RoundToInt(baseShriekSoulCost * 0.9f));
                        quakeSoulCost = Mathf.Max(10, Mathf.RoundToInt(baseQuakeSoulCost * 0.9f));
                        break;
                    case ShadeCharmId.TwinSoulBond:
                        maxDistance = baseMaxDistance + 6f;
                        softLeashRadius = baseSoftLeashRadius + 4f;
                        hardLeashRadius = baseHardLeashRadius + 6f;
                        snapLeashRadius = baseSnapLeashRadius + 6f;
                        break;
                    case ShadeCharmId.LuminousGuard:
                        projectileSoulCost = Mathf.Max(10, Mathf.RoundToInt(projectileSoulCost * 0.9f));
                        break;
                }
            }

            maxDistance = Mathf.Max(6f, maxDistance);
            softLeashRadius = Mathf.Max(4f, softLeashRadius);
            hardLeashRadius = Mathf.Max(6f, hardLeashRadius);
            snapLeashRadius = Mathf.Max(softLeashRadius, snapLeashRadius);

            PushSoulToHud();
        }
    }
}
