using System;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200009F RID: 159
public sealed class PlaymakerBoolConditionalTk2dAnimation : ConditionalAnimation
{
	// Token: 0x060004ED RID: 1261 RVA: 0x00019B1C File Offset: 0x00017D1C
	public override bool CanPlayAnimation()
	{
		if (string.IsNullOrEmpty(this.animationName))
		{
			return false;
		}
		if (string.IsNullOrEmpty(this.boolName))
		{
			return false;
		}
		if (this.targetFsm == null)
		{
			return false;
		}
		if (this.animator == null)
		{
			return false;
		}
		FsmBool fsmBool = this.targetFsm.FsmVariables.FindFsmBool(this.boolName);
		return fsmBool != null && fsmBool.Value == this.expectedValue;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00019B91 File Offset: 0x00017D91
	public override void PlayAnimation()
	{
		if (this.animator)
		{
			this.animator.Play(this.animationName);
		}
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00019BB1 File Offset: 0x00017DB1
	public override IEnumerator PlayAndWait()
	{
		if (this.animator != null)
		{
			yield return this.animator.PlayAnimWait(this.animationName, null);
		}
		yield break;
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x00019BC0 File Offset: 0x00017DC0
	private bool? IsFsmBoolValid(string boolName)
	{
		if (!this.targetFsm || string.IsNullOrEmpty(boolName))
		{
			return null;
		}
		return new bool?(this.targetFsm.FsmVariables.FindFsmBool(boolName) != null);
	}

	// Token: 0x040004C0 RID: 1216
	[SerializeField]
	private PlayMakerFSM targetFsm;

	// Token: 0x040004C1 RID: 1217
	[SerializeField]
	[ModifiableProperty]
	[Conditional("targetFsm", true, false, false)]
	[InspectorValidation("IsFsmBoolValid")]
	private string boolName;

	// Token: 0x040004C2 RID: 1218
	[SerializeField]
	private bool expectedValue;

	// Token: 0x040004C3 RID: 1219
	[Space]
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x040004C4 RID: 1220
	[SerializeField]
	private string animationName;
}
