using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200017C RID: 380
[ActionCategory("Hollow Knight")]
public class FadeColorFader : FsmStateAction
{
	// Token: 0x06000C73 RID: 3187 RVA: 0x00036EDE File Offset: 0x000350DE
	public override void Reset()
	{
		this.target = null;
		this.fadeType = null;
		this.useChildren = new FsmBool(true);
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x00036F00 File Offset: 0x00035100
	public override void OnEnter()
	{
		GameObject safe = this.target.GetSafe(this);
		if (safe)
		{
			ColorFader[] array;
			if (this.useChildren.Value)
			{
				array = safe.GetComponentsInChildren<ColorFader>();
			}
			else
			{
				array = new ColorFader[]
				{
					safe.GetComponent<ColorFader>()
				};
			}
			ColorFader[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Fade((FadeColorFader.FadeType)this.fadeType.Value == FadeColorFader.FadeType.UP);
			}
		}
		base.Finish();
	}

	// Token: 0x04000BF0 RID: 3056
	public FsmOwnerDefault target;

	// Token: 0x04000BF1 RID: 3057
	[ObjectType(typeof(FadeColorFader.FadeType))]
	public FsmEnum fadeType;

	// Token: 0x04000BF2 RID: 3058
	public FsmBool useChildren;

	// Token: 0x020014AE RID: 5294
	public enum FadeType
	{
		// Token: 0x04008431 RID: 33841
		UP,
		// Token: 0x04008432 RID: 33842
		DOWN
	}
}
