using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020000D5 RID: 213
public class DarknessRegion : TrackTriggerObjects, ISceneLintUpgrader
{
	// Token: 0x060006BE RID: 1726 RVA: 0x000223EF File Offset: 0x000205EF
	private bool ShouldHideDarknessLevel()
	{
		return FSMUtility.LocateFSM(base.gameObject, "Darkness Region");
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00022406 File Offset: 0x00020606
	protected override void Awake()
	{
		base.Awake();
		this.OnSceneLintUpgrade(true);
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00022418 File Offset: 0x00020618
	protected override void OnInsideStateChanged(bool isInside)
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		DarknessRegion._insideRegions.Remove(this);
		if (isInside)
		{
			DarknessRegion._insideRegions.Add(this);
		}
		int num;
		if (DarknessRegion._insideRegions.Count == 0)
		{
			num = (silentInstance ? silentInstance.sm.darknessLevel : 0);
		}
		else
		{
			List<DarknessRegion> insideRegions = DarknessRegion._insideRegions;
			DarknessRegion darknessRegion = insideRegions[insideRegions.Count - 1];
			for (int i = DarknessRegion._insideRegions.Count - 1; i >= 0; i--)
			{
				DarknessRegion darknessRegion2 = DarknessRegion._insideRegions[i];
				if (darknessRegion.priority > darknessRegion2.priority)
				{
					return;
				}
				darknessRegion = darknessRegion2;
			}
			num = darknessRegion.darknessLevel;
		}
		DarknessRegion.SetDarknessLevel(num);
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x000224CC File Offset: 0x000206CC
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "Darkness Region");
		if (!playMakerFSM)
		{
			return null;
		}
		if (!doUpgrade)
		{
			return "Darkness Region FSM needs upgrading to DarknessRegion script";
		}
		FsmInt fsmInt = playMakerFSM.FsmVariables.FindFsmInt("Darkness");
		this.darknessLevel = fsmInt.Value;
		Object.DestroyImmediate(playMakerFSM);
		return "Darkness Region FSM was upgraded to DarknessRegion script";
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00022525 File Offset: 0x00020725
	public static int GetDarknessLevel()
	{
		return HeroController.instance.vignetteFSM.FsmVariables.GetFsmInt("Darkness Level").Value;
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00022548 File Offset: 0x00020748
	public static void SetDarknessLevel(int darknessLevel)
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		PlayMakerFSM vignetteFSM = silentInstance.vignetteFSM;
		FsmInt fsmInt = vignetteFSM.FsmVariables.GetFsmInt("Darkness Level");
		if (fsmInt.Value == darknessLevel)
		{
			return;
		}
		silentInstance.SetDarkness(darknessLevel);
		fsmInt.Value = darknessLevel;
		vignetteFSM.SendEvent("SCENE RESET");
	}

	// Token: 0x0400069C RID: 1692
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShouldHideDarknessLevel", false, true, false)]
	private int darknessLevel;

	// Token: 0x0400069D RID: 1693
	[SerializeField]
	private int priority;

	// Token: 0x0400069E RID: 1694
	private static readonly List<DarknessRegion> _insideRegions = new List<DarknessRegion>();
}
