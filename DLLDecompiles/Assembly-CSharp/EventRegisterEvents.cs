using System;

// Token: 0x02000333 RID: 819
public static class EventRegisterEvents
{
	// Token: 0x04001BCE RID: 7118
	public static readonly int FsmCancel = EventRegister.GetEventHashCode("FSM CANCEL");

	// Token: 0x04001BCF RID: 7119
	public static readonly int HeroDeath = EventRegister.GetEventHashCode("HERO DEATH");

	// Token: 0x04001BD0 RID: 7120
	public static readonly int MaggotCheck = EventRegister.GetEventHashCode("MAGGOT CHECK");

	// Token: 0x04001BD1 RID: 7121
	public static readonly int HeroHealed = EventRegister.GetEventHashCode("HERO HEALED");

	// Token: 0x04001BD2 RID: 7122
	public static readonly int HeroHealedToMax = EventRegister.GetEventHashCode("HERO HEALED TO MAX");

	// Token: 0x04001BD3 RID: 7123
	public static readonly int HealthUpdate = EventRegister.GetEventHashCode("HEALTH UPDATE");

	// Token: 0x04001BD4 RID: 7124
	public static readonly int CharmIndicatorCheck = EventRegister.GetEventHashCode("CHARM INDICATOR CHECK");

	// Token: 0x04001BD5 RID: 7125
	public static readonly int HeroSurfaceEnter = EventRegister.GetEventHashCode("HERO SURFACE ENTER");

	// Token: 0x04001BD6 RID: 7126
	public static readonly int HeroSurfaceExit = EventRegister.GetEventHashCode("HERO SURFACE EXIT");

	// Token: 0x04001BD7 RID: 7127
	public static readonly int StartExtractM = EventRegister.GetEventHashCode("START EXTRACT M");

	// Token: 0x04001BD8 RID: 7128
	public static readonly int StartExtractSwamp = EventRegister.GetEventHashCode("START EXTRACT SWAMP");

	// Token: 0x04001BD9 RID: 7129
	public static readonly int StartExtractBlueblood = EventRegister.GetEventHashCode("START EXTRACT BLUEBLOOD");

	// Token: 0x04001BDA RID: 7130
	public static readonly int CinematicStart = EventRegister.GetEventHashCode("CINEMATIC START");

	// Token: 0x04001BDB RID: 7131
	public static readonly int CinematicEnd = EventRegister.GetEventHashCode("CINEMATIC END");

	// Token: 0x04001BDC RID: 7132
	public static readonly int CinematicSkipped = EventRegister.GetEventHashCode("CINEMATIC SKIPPED");

	// Token: 0x04001BDD RID: 7133
	public static readonly int FleaMusicStart = EventRegister.GetEventHashCode("FLEA MUSIC START");

	// Token: 0x04001BDE RID: 7134
	public static readonly int FleaMusicStop = EventRegister.GetEventHashCode("FLEA MUSIC STOP");

	// Token: 0x04001BDF RID: 7135
	public static readonly int BenchRegainControl = EventRegister.GetEventHashCode("BENCH REGAIN CONTROL");

	// Token: 0x04001BE0 RID: 7136
	public static readonly int BenchRelinquishControl = EventRegister.GetEventHashCode("BENCH RELINQUISH CONTROL");

	// Token: 0x04001BE1 RID: 7137
	public static readonly int LavaBellRecharging = EventRegister.GetEventHashCode("LAVA BELL RECHARGING");

	// Token: 0x04001BE2 RID: 7138
	public static readonly int FrostUpdateHealth = EventRegister.GetEventHashCode("FROST UPDATE HEALTH");

	// Token: 0x04001BE3 RID: 7139
	public static readonly int HeroDamagedExtra = EventRegister.GetEventHashCode("HERO DAMAGED EXTRA");

	// Token: 0x04001BE4 RID: 7140
	public static readonly int LavaBellUsed = EventRegister.GetEventHashCode("LAVA BELL USED");

	// Token: 0x04001BE5 RID: 7141
	public static readonly int ReminderBind = EventRegister.GetEventHashCode("REMINDER BIND");

	// Token: 0x04001BE6 RID: 7142
	public static readonly int WarriorRageEnded = EventRegister.GetEventHashCode("WARRIOR RAGE ENDED");

	// Token: 0x04001BE7 RID: 7143
	public static readonly int RegeneratedSilkChunk = EventRegister.GetEventHashCode("REGENERATED SILK CHUNK");

	// Token: 0x04001BE8 RID: 7144
	public static readonly int SilkCursedUpdate = EventRegister.GetEventHashCode("SILK CURSED UPDATE");

	// Token: 0x04001BE9 RID: 7145
	public static readonly int WarriorRageStarted = EventRegister.GetEventHashCode("WARRIOR RAGE STARTED");

	// Token: 0x04001BEA RID: 7146
	public static readonly int HazardRespawnReset = EventRegister.GetEventHashCode("HAZARD RESPAWN RESET");

	// Token: 0x04001BEB RID: 7147
	public static readonly int HeroHazardDeath = EventRegister.GetEventHashCode("HERO HAZARD DEATH");

	// Token: 0x04001BEC RID: 7148
	public static readonly int SpoolUnbroken = EventRegister.GetEventHashCode("SPOOL UNBROKEN");

	// Token: 0x04001BED RID: 7149
	public static readonly int ClearEffects = EventRegister.GetEventHashCode("CLEAR EFFECTS");

	// Token: 0x04001BEE RID: 7150
	public static readonly int LavaBellReset = EventRegister.GetEventHashCode("LAVA BELL RESET");

	// Token: 0x04001BEF RID: 7151
	public static readonly int ItemCollected = EventRegister.GetEventHashCode("ITEM COLLECTED");

	// Token: 0x04001BF0 RID: 7152
	public static readonly int BreakHeroCorpse = EventRegister.GetEventHashCode("BREAK HERO CORPSE");

	// Token: 0x04001BF1 RID: 7153
	public static readonly int DeliveryHudRefresh = EventRegister.GetEventHashCode("DELIVERY HUD REFRESH");

	// Token: 0x04001BF2 RID: 7154
	public static readonly int DeliveryHudHit = EventRegister.GetEventHashCode("DELIVERY HUD HIT");

	// Token: 0x04001BF3 RID: 7155
	public static readonly int DeliveryHudBreak = EventRegister.GetEventHashCode("DELIVERY HUD BREAK");

	// Token: 0x04001BF4 RID: 7156
	public static readonly int InventoryCancel = EventRegister.GetEventHashCode("INVENTORY CANCEL");

	// Token: 0x04001BF5 RID: 7157
	public static readonly int GgTransitionOutInstant = EventRegister.GetEventHashCode("GG TRANSITION OUT INSTANT");

	// Token: 0x04001BF6 RID: 7158
	public static readonly int GgTransitionIn = EventRegister.GetEventHashCode("GG TRANSITION IN");

	// Token: 0x04001BF7 RID: 7159
	public static readonly int GgTransitionFinal = EventRegister.GetEventHashCode("GG TRANSITION FINAL");

	// Token: 0x04001BF8 RID: 7160
	public static readonly int GgTransitionOutStatue = EventRegister.GetEventHashCode("GG TRANSITION OUT STATUE");

	// Token: 0x04001BF9 RID: 7161
	public static readonly int ShowBoundNail = EventRegister.GetEventHashCode("SHOW BOUND NAIL");

	// Token: 0x04001BFA RID: 7162
	public static readonly int ShowBoundCharms = EventRegister.GetEventHashCode("SHOW BOUND CHARMS");

	// Token: 0x04001BFB RID: 7163
	public static readonly int UpdateBlueHealth = EventRegister.GetEventHashCode("UPDATE BLUE HEALTH");

	// Token: 0x04001BFC RID: 7164
	public static readonly int BindVesselOrb = EventRegister.GetEventHashCode("BIND VESSEL ORB");

	// Token: 0x04001BFD RID: 7165
	public static readonly int HideBoundNail = EventRegister.GetEventHashCode("HIDE BOUND NAIL");

	// Token: 0x04001BFE RID: 7166
	public static readonly int HideBoundCharms = EventRegister.GetEventHashCode("HIDE BOUND CHARMS");

	// Token: 0x04001BFF RID: 7167
	public static readonly int UnbindVesselOrb = EventRegister.GetEventHashCode("UNBIND VESSEL ORB");

	// Token: 0x04001C00 RID: 7168
	public static readonly int ToolPinThunked = EventRegister.GetEventHashCode("TOOL PIN THUNKED");

	// Token: 0x04001C01 RID: 7169
	public static readonly int SceneTransitionBegan = EventRegister.GetEventHashCode("SCENE TRANSITION BEGAN");

	// Token: 0x04001C02 RID: 7170
	public static readonly int HazardFade = EventRegister.GetEventHashCode("HAZARD FADE");

	// Token: 0x04001C03 RID: 7171
	public static readonly int HazardReload = EventRegister.GetEventHashCode("HAZARD RELOAD");

	// Token: 0x04001C04 RID: 7172
	public static readonly int HeroEnteredScene = EventRegister.GetEventHashCode("HERO ENTERED SCENE");

	// Token: 0x04001C05 RID: 7173
	public static readonly int CustomCutsceneSkip = EventRegister.GetEventHashCode("CUSTOM CUTSCENE SKIP");

	// Token: 0x04001C06 RID: 7174
	public static readonly int DreamPlantHit = EventRegister.GetEventHashCode("DREAM PLANT HIT");

	// Token: 0x04001C07 RID: 7175
	public static readonly int DreamOrbCollect = EventRegister.GetEventHashCode("DREAM ORB COLLECT");

	// Token: 0x04001C08 RID: 7176
	public static readonly int EnviroUpdate = EventRegister.GetEventHashCode("ENVIRO UPDATE");

	// Token: 0x04001C09 RID: 7177
	public static readonly int AddBlueHealth = EventRegister.GetEventHashCode("ADD BLUE HEALTH");

	// Token: 0x04001C0A RID: 7178
	public static readonly int AddQueuedBlueHealth = EventRegister.GetEventHashCode("ADD QUEUED BLUE HEALTH");

	// Token: 0x04001C0B RID: 7179
	public static readonly int StartExtractB = EventRegister.GetEventHashCode("START EXTRACT B");

	// Token: 0x04001C0C RID: 7180
	public static readonly int ShowCustomToolReminder = EventRegister.GetEventHashCode("SHOW CUSTOM TOOL REMINDER");

	// Token: 0x04001C0D RID: 7181
	public static readonly int HideCustomToolReminder = EventRegister.GetEventHashCode("HIDE CUSTOM TOOL REMINDER");

	// Token: 0x04001C0E RID: 7182
	public static readonly int HudFrameChanged = EventRegister.GetEventHashCode("HUD FRAME CHANGED");

	// Token: 0x04001C0F RID: 7183
	public static readonly int DialogueBoxAppearing = EventRegister.GetEventHashCode("DIALOGUE BOX APPEARING");

	// Token: 0x04001C10 RID: 7184
	public static readonly int ResetShopWindow = EventRegister.GetEventHashCode("RESET SHOP WINDOW");

	// Token: 0x04001C11 RID: 7185
	public static readonly int GgTransitionOut = EventRegister.GetEventHashCode("GG TRANSITION OUT");

	// Token: 0x04001C12 RID: 7186
	public static readonly int HeroDamaged = EventRegister.GetEventHashCode("HERO DAMAGED");

	// Token: 0x04001C13 RID: 7187
	public static readonly int CogDamage = EventRegister.GetEventHashCode("COG DAMAGE");

	// Token: 0x04001C14 RID: 7188
	public static readonly int BindFailedNotEnough = EventRegister.GetEventHashCode("BIND FAILED NOT ENOUGH");

	// Token: 0x04001C15 RID: 7189
	public static readonly int ExtractCancel = EventRegister.GetEventHashCode("EXTRACT CANCEL");

	// Token: 0x04001C16 RID: 7190
	public static readonly int InventoryOpenComplete = EventRegister.GetEventHashCode("INVENTORY OPEN COMPLETE");

	// Token: 0x04001C17 RID: 7191
	public static readonly int FlintSlateExpire = EventRegister.GetEventHashCode("FLINT SLATE EXPIRE");

	// Token: 0x04001C18 RID: 7192
	public static readonly int EquipsChangedEvent = EventRegister.GetEventHashCode("TOOL EQUIPS CHANGED");

	// Token: 0x04001C19 RID: 7193
	public static readonly int EquipsChangedPostEvent = EventRegister.GetEventHashCode("POST TOOL EQUIPS CHANGED");
}
