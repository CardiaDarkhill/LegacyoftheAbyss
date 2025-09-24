using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000677 RID: 1655
[ActionCategory("Hollow Knight")]
public class SelectCharmBackboard : FsmStateAction
{
	// Token: 0x06003B4D RID: 15181 RVA: 0x00105079 File Offset: 0x00103279
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x06003B4E RID: 15182 RVA: 0x00105088 File Offset: 0x00103288
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			InvCharmBackboard component = gameObject.GetComponent<InvCharmBackboard>();
			if (component != null)
			{
				component.SelectCharm();
			}
		}
		base.Finish();
	}

	// Token: 0x04003D96 RID: 15766
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;
}
