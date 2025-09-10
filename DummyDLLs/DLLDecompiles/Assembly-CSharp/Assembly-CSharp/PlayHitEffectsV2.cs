using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002ED RID: 749
public class PlayHitEffectsV2 : FsmStateAction
{
	// Token: 0x06001AD4 RID: 6868 RVA: 0x0007CFAA File Offset: 0x0007B1AA
	public override void Awake()
	{
		base.Awake();
		this.hitEffectRecievers = new List<IHitEffectReciever>();
	}

	// Token: 0x06001AD5 RID: 6869 RVA: 0x0007CFBD File Offset: 0x0007B1BD
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
		this.attackDirection = new FsmFloat();
		this.forceNotWeakHit = null;
	}

	// Token: 0x06001AD6 RID: 6870 RVA: 0x0007CFDC File Offset: 0x0007B1DC
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			this.hitEffectRecievers.Clear();
			gameObject.GetComponents<IHitEffectReciever>(this.hitEffectRecievers);
			for (int i = 0; i < this.hitEffectRecievers.Count; i++)
			{
				this.hitEffectRecievers[i].ReceiveHitEffect(new HitInstance
				{
					Direction = this.attackDirection.Value,
					ForceNotWeakHit = this.forceNotWeakHit.Value
				});
			}
			this.hitEffectRecievers.Clear();
		}
		base.Finish();
	}

	// Token: 0x040019E9 RID: 6633
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x040019EA RID: 6634
	[UIHint(UIHint.Variable)]
	public FsmFloat attackDirection;

	// Token: 0x040019EB RID: 6635
	public FsmBool forceNotWeakHit;

	// Token: 0x040019EC RID: 6636
	private List<IHitEffectReciever> hitEffectRecievers;
}
