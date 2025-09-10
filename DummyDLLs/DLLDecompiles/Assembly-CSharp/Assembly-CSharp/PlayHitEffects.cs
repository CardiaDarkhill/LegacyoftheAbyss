using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002EC RID: 748
[ActionCategory("Hollow Knight")]
public class PlayHitEffects : FsmStateAction
{
	// Token: 0x06001AD0 RID: 6864 RVA: 0x0007CED0 File Offset: 0x0007B0D0
	public override void Awake()
	{
		base.Awake();
		this.hitEffectRecievers = new List<IHitEffectReciever>();
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x0007CEE3 File Offset: 0x0007B0E3
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
		this.attackDirection = new FsmFloat();
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x0007CEFC File Offset: 0x0007B0FC
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
					Direction = this.attackDirection.Value
				});
			}
			this.hitEffectRecievers.Clear();
		}
		base.Finish();
	}

	// Token: 0x040019E6 RID: 6630
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x040019E7 RID: 6631
	[UIHint(UIHint.Variable)]
	public FsmFloat attackDirection;

	// Token: 0x040019E8 RID: 6632
	private List<IHitEffectReciever> hitEffectRecievers;
}
