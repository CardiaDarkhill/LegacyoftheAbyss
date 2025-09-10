using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000654 RID: 1620
[ActionCategory("Hollow Knight")]
public class FadeGroupDown : FsmStateAction
{
	// Token: 0x06003A04 RID: 14852 RVA: 0x000FE8EC File Offset: 0x000FCAEC
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x06003A05 RID: 14853 RVA: 0x000FE8FC File Offset: 0x000FCAFC
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			FadeGroup component = gameObject.GetComponent<FadeGroup>();
			if (component != null)
			{
				if (this.fast.Value)
				{
					component.FadeDownFast();
				}
				else
				{
					component.FadeDown();
				}
			}
		}
		base.Finish();
	}

	// Token: 0x04003CA8 RID: 15528
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x04003CA9 RID: 15529
	public FsmBool fast;
}
