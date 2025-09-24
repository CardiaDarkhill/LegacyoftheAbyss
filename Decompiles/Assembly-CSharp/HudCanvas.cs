using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200043A RID: 1082
public sealed class HudCanvas : MonoBehaviour
{
	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x0600253D RID: 9533 RVA: 0x000AAF45 File Offset: 0x000A9145
	public static bool IsVisible
	{
		get
		{
			return HudCanvas.fsmBool == null || HudCanvas.fsmBool.Value;
		}
	}

	// Token: 0x0600253E RID: 9534 RVA: 0x000AAF5A File Offset: 0x000A915A
	private void Awake()
	{
		if (HudCanvas.instance == null)
		{
			HudCanvas.instance = this;
			if (this.targetFsm)
			{
				HudCanvas.fsmBool = this.GetFsmBool();
			}
		}
	}

	// Token: 0x0600253F RID: 9535 RVA: 0x000AAF87 File Offset: 0x000A9187
	private void OnDestroy()
	{
		if (HudCanvas.instance == this)
		{
			HudCanvas.instance = null;
			HudCanvas.fsmBool = null;
		}
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x000AAFA2 File Offset: 0x000A91A2
	private bool? IsFsmBoolValid(string boolName)
	{
		return new bool?(this.GetFsmBool(boolName) != null);
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x000AAFB3 File Offset: 0x000A91B3
	private FsmBool GetFsmBool(string boolName)
	{
		if (!this.targetFsm || string.IsNullOrEmpty(boolName))
		{
			return null;
		}
		return this.targetFsm.FsmVariables.FindFsmBool(boolName);
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x000AAFDD File Offset: 0x000A91DD
	private FsmBool GetFsmBool()
	{
		return this.GetFsmBool(this.visibilityBool);
	}

	// Token: 0x040022FC RID: 8956
	[SerializeField]
	private PlayMakerFSM targetFsm;

	// Token: 0x040022FD RID: 8957
	[SerializeField]
	[ModifiableProperty]
	[Conditional("targetFsm", true, false, false)]
	[InspectorValidation("IsFsmBoolValid")]
	private string visibilityBool = "Is Visible";

	// Token: 0x040022FE RID: 8958
	private static HudCanvas instance;

	// Token: 0x040022FF RID: 8959
	private static FsmBool fsmBool;
}
