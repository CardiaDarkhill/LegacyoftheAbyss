using System;
using GlobalEnums;
using GlobalSettings;

// Token: 0x020003BD RID: 957
public static class HeroSlabCapture
{
	// Token: 0x0600203D RID: 8253 RVA: 0x000930F8 File Offset: 0x000912F8
	public static void ApplyCaptured()
	{
		HeroController instance = HeroController.instance;
		CurrencyManager.TempStoreCurrency();
		instance.MaxHealth();
		ToolCrest cloaklessCrest = Gameplay.CloaklessCrest;
		BindOrbHudFrame.ForceNextInstant = true;
		ToolItemManager.AutoEquip(cloaklessCrest, false);
		BindOrbHudFrame.ForceNextInstant = false;
		GameManager.instance.SetDeathRespawnSimple("Caged Respawn Marker", 0, false);
		PlayerData instance2 = PlayerData.instance;
		instance2.respawnScene = "Slab_03";
		instance2.mapZone = MapZone.THE_SLAB;
		DeliveryQuestItem.BreakAllNoEffects();
	}
}
