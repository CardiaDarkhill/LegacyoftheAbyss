using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020004EE RID: 1262
[ActionCategory("Hollow Knight")]
public class GlowResponseEnd : FsmStateAction
{
	// Token: 0x06002D3B RID: 11579 RVA: 0x000C581E File Offset: 0x000C3A1E
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x000C582C File Offset: 0x000C3A2C
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			GlowResponse component = gameObject.GetComponent<GlowResponse>();
			if (component != null)
			{
				component.FadeEnd();
			}
		}
		base.Finish();
	}

	// Token: 0x04002EE3 RID: 12003
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;
}
