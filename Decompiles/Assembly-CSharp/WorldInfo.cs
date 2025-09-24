using System;

// Token: 0x020007D5 RID: 2005
public static class WorldInfo
{
	// Token: 0x06004697 RID: 18071 RVA: 0x00131E84 File Offset: 0x00130084
	public static bool NameLooksLikeAdditiveLoadScene(string sceneName)
	{
		foreach (string value in WorldInfo.SubSceneNameSuffixes)
		{
			if (sceneName.EndsWith(value, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040046FF RID: 18175
	public static readonly string[] MenuScenes = new string[]
	{
		"Pre_Menu_Intro",
		"Menu_Title",
		"BetaEnd"
	};

	// Token: 0x04004700 RID: 18176
	public static readonly string[] NonGameplayScenes = new string[]
	{
		"Opening_Sequence",
		"Opening_Sequence_Act3",
		"Prologue_Excerpt",
		"Intro_Cutscene_Prologue",
		"Intro_Cutscene",
		"Cinematic_Stag_travel",
		"Cinematic_Ending_A",
		"Cinematic_Ending_B",
		"Cinematic_Ending_C",
		"End_Credits",
		"End_Credits_Scroll",
		"Cinematic_MrMushroom",
		"Menu_Credits",
		"End_Game_Completion",
		"PermaDeath",
		"PermaDeath_Unlock",
		"Cinematic_Ending_D",
		"Cinematic_Ending_E",
		"Demo Start",
		"Cinematic_Submarine_travel"
	};

	// Token: 0x04004701 RID: 18177
	public static readonly string[] SubSceneNameSuffixes = new string[]
	{
		"_boss_defeated",
		"_boss",
		"_preload",
		"_bellway",
		"_mapper",
		"_boss_golem",
		"_boss_golem_rest",
		"_boss_beastfly",
		"_additive",
		"_pre",
		"_caravan",
		"_festival"
	};
}
