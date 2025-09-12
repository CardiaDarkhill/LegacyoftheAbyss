using System;
using UnityEngine;
public partial class SimpleHUD
{
    private void ComputeShadeFromPlayer()
    {
        if (playerData == null) { shadeMax = 0; shadeHealth = 0; return; }
        shadeMax = (playerData.maxHealth + 1) / 2;
        prevHornetMax = playerData.maxHealth;
        prevHornetHealth = playerData.health;
        if (previousShadeHealth == 0 && shadeHealth == 0)
            shadeHealth = (playerData.health + 1) / 2;
    }

    private void SyncShadeFromPlayer()
    {
        if (playerData == null) return;
        int newHornetMax = playerData.maxHealth;
        int newHornet = playerData.health;
        int newMax = (newHornetMax + 1) / 2;
        if (newMax != shadeMax)
        {
            shadeMax = newMax; RebuildMasks(); previousShadeHealth = Mathf.Min(previousShadeHealth, shadeMax); shadeHealth = Mathf.Min(shadeHealth, shadeMax);
        }
        prevHornetHealth = newHornet; prevHornetMax = newHornetMax;
    }
}


