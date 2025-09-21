using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000320 RID: 800
[ActionCategory("Hollow Knight")]
public abstract class WalkerAction : FsmStateAction
{
	// Token: 0x06001C30 RID: 7216 RVA: 0x000837B3 File Offset: 0x000819B3
	public override void Reset()
	{
		base.Reset();
		this.target = new FsmOwnerDefault();
		this.everyFrame = false;
	}

	// Token: 0x06001C31 RID: 7217 RVA: 0x000837D0 File Offset: 0x000819D0
	public override void OnEnter()
	{
		base.OnEnter();
		GameObject safe = this.target.GetSafe(this);
		if (safe != null)
		{
			this.walker = safe.GetComponent<Walker>();
			if (this.walker != null)
			{
				this.Apply(this.walker);
			}
		}
		else
		{
			this.walker = null;
		}
		if (!this.everyFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x06001C32 RID: 7218 RVA: 0x00083836 File Offset: 0x00081A36
	public override void OnUpdate()
	{
		base.OnUpdate();
		if (this.walker != null)
		{
			this.Apply(this.walker);
		}
	}

	// Token: 0x06001C33 RID: 7219
	protected abstract void Apply(Walker walker);

	// Token: 0x04001B46 RID: 6982
	public FsmOwnerDefault target;

	// Token: 0x04001B47 RID: 6983
	public bool everyFrame;

	// Token: 0x04001B48 RID: 6984
	private Walker walker;
}
