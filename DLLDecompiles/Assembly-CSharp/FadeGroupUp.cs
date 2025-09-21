using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000653 RID: 1619
[ActionCategory("Hollow Knight")]
public class FadeGroupUp : FsmStateAction
{
	// Token: 0x06003A01 RID: 14849 RVA: 0x000FE87D File Offset: 0x000FCA7D
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x06003A02 RID: 14850 RVA: 0x000FE88C File Offset: 0x000FCA8C
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			FadeGroup component = gameObject.GetComponent<FadeGroup>();
			if (component != null)
			{
				component.FadeUp();
			}
		}
		base.Finish();
	}

	// Token: 0x04003CA7 RID: 15527
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;
}
