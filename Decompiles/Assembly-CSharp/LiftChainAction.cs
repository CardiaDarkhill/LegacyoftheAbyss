using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020007BD RID: 1981
[ActionCategory("Hollow Knight")]
public abstract class LiftChainAction : FsmStateAction
{
	// Token: 0x060045D2 RID: 17874 RVA: 0x0012FAC7 File Offset: 0x0012DCC7
	public override void Reset()
	{
		base.Reset();
		this.target = new FsmOwnerDefault();
		this.everyFrame = false;
	}

	// Token: 0x060045D3 RID: 17875 RVA: 0x0012FAE4 File Offset: 0x0012DCE4
	public override void OnEnter()
	{
		base.OnEnter();
		GameObject safe = this.target.GetSafe(this);
		if (safe != null)
		{
			this.liftChain = safe.GetComponent<LiftChain>();
			if (this.liftChain != null)
			{
				this.Apply(this.liftChain);
			}
		}
		else
		{
			this.liftChain = null;
		}
		if (!this.everyFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x060045D4 RID: 17876 RVA: 0x0012FB4A File Offset: 0x0012DD4A
	public override void OnUpdate()
	{
		base.OnUpdate();
		if (this.liftChain != null)
		{
			this.Apply(this.liftChain);
		}
	}

	// Token: 0x060045D5 RID: 17877
	protected abstract void Apply(LiftChain liftChain);

	// Token: 0x0400467F RID: 18047
	public FsmOwnerDefault target;

	// Token: 0x04004680 RID: 18048
	public bool everyFrame;

	// Token: 0x04004681 RID: 18049
	private LiftChain liftChain;
}
