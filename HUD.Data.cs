#nullable disable
using System;
using UnityEngine;
public partial class SimpleHUD
{
    private void ComputeShadeFromPlayer()
    {
        if (playerData == null)
        {
            shadeMax = 0;
            shadeHealth = 0;
            shadeLifebloodMax = 0;
            shadeLifeblood = 0;
            prevHornetMax = 0;
            prevHornetHealth = 0;
            suppressNextDamageSound = false;
            return;
        }
        prevHornetMax = playerData.maxHealth;
        prevHornetHealth = playerData.health;

        if (!hasExplicitShadeStats)
        {
            shadeMax = ModConfig.ComputeShadeMaskCount(playerData.maxHealth);
            shadeLifebloodMax = 0;
            shadeHealth = Mathf.Min(ModConfig.ComputeShadeMaskCount(playerData.health), shadeMax);
            shadeLifeblood = 0;
            suppressNextDamageSound = true;
        }
    }

    private void SyncShadeFromPlayer()
    {
        if (playerData == null) return;
        int newHornetMax = playerData.maxHealth;
        int newHornet = playerData.health;
        if (!hasExplicitShadeStats)
        {
            int newMax = ModConfig.ComputeShadeMaskCount(newHornetMax);
            if (newMax != shadeMax)
            {
                shadeMax = newMax;
                RebuildMasks();
                previousShadeTotalHealth = Mathf.Min(previousShadeTotalHealth, shadeMax + shadeLifebloodMax);
                shadeHealth = Mathf.Min(shadeHealth, shadeMax);
            }
        }
        prevHornetHealth = newHornet; prevHornetMax = newHornetMax;
    }
}


#nullable restore
