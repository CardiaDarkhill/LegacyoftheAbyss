using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200067A RID: 1658
[ActionCategory("Hollow Knight")]
public class GetCharmNumString : FsmStateAction
{
	// Token: 0x06003B56 RID: 15190 RVA: 0x001051DB File Offset: 0x001033DB
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x06003B57 RID: 15191 RVA: 0x001051E8 File Offset: 0x001033E8
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			InvCharmBackboard component = gameObject.GetComponent<InvCharmBackboard>();
			if (component != null)
			{
				this.storeValue.Value = component.GetCharmNumString();
			}
		}
		base.Finish();
	}

	// Token: 0x04003D9B RID: 15771
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x04003D9C RID: 15772
	public FsmString storeValue;
}
