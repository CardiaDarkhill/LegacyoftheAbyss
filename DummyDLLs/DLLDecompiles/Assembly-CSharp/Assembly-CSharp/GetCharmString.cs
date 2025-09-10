using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000679 RID: 1657
[ActionCategory("Hollow Knight")]
public class GetCharmString : FsmStateAction
{
	// Token: 0x06003B53 RID: 15187 RVA: 0x00105163 File Offset: 0x00103363
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x06003B54 RID: 15188 RVA: 0x00105170 File Offset: 0x00103370
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			InvCharmBackboard component = gameObject.GetComponent<InvCharmBackboard>();
			if (component != null)
			{
				this.storeValue.Value = component.GetCharmString();
			}
		}
		base.Finish();
	}

	// Token: 0x04003D99 RID: 15769
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x04003D9A RID: 15770
	public FsmString storeValue;
}
