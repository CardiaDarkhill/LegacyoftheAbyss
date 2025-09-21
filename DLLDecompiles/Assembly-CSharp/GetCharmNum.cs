using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000678 RID: 1656
[ActionCategory("Hollow Knight")]
public class GetCharmNum : FsmStateAction
{
	// Token: 0x06003B50 RID: 15184 RVA: 0x001050E8 File Offset: 0x001032E8
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x06003B51 RID: 15185 RVA: 0x001050F8 File Offset: 0x001032F8
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			InvCharmBackboard component = gameObject.GetComponent<InvCharmBackboard>();
			if (component != null)
			{
				this.storeValue.Value = component.GetCharmNum();
			}
		}
		base.Finish();
	}

	// Token: 0x04003D97 RID: 15767
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x04003D98 RID: 15768
	public FsmInt storeValue;
}
