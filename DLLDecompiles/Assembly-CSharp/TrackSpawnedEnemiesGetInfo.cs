using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200031E RID: 798
[ActionCategory("Hollow Knight")]
public class TrackSpawnedEnemiesGetInfo : FsmStateAction
{
	// Token: 0x06001C14 RID: 7188 RVA: 0x00082CE3 File Offset: 0x00080EE3
	public override void Reset()
	{
		this.Target = null;
		this.TotalTracked = null;
		this.TotalAlive = null;
	}

	// Token: 0x06001C15 RID: 7189 RVA: 0x00082CFC File Offset: 0x00080EFC
	public override void OnEnter()
	{
		GameObject safe = this.Target.GetSafe(this);
		if (safe)
		{
			TrackSpawnedEnemies component = safe.GetComponent<TrackSpawnedEnemies>();
			if (component)
			{
				if (!this.TotalTracked.IsNone)
				{
					this.TotalTracked.Value = component.TotalTracked;
				}
				if (!this.TotalAlive.IsNone)
				{
					this.TotalAlive.Value = component.TotalAlive;
				}
			}
		}
		base.Finish();
	}

	// Token: 0x04001B11 RID: 6929
	public FsmOwnerDefault Target;

	// Token: 0x04001B12 RID: 6930
	[UIHint(UIHint.Variable)]
	public FsmInt TotalTracked;

	// Token: 0x04001B13 RID: 6931
	[UIHint(UIHint.Variable)]
	public FsmInt TotalAlive;
}
