using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200032D RID: 813
[ActionCategory("Hollow Knight")]
public class DestroyEnemyBullet : FsmStateAction
{
	// Token: 0x06001C86 RID: 7302 RVA: 0x0008508F File Offset: 0x0008328F
	public override void Reset()
	{
		this.target = null;
	}

	// Token: 0x06001C87 RID: 7303 RVA: 0x00085098 File Offset: 0x00083298
	public override void OnEnter()
	{
		GameObject safe = this.target.GetSafe(this);
		if (safe)
		{
			EnemyBullet component = safe.GetComponent<EnemyBullet>();
			if (component)
			{
				component.OrbitShieldHit(base.Owner.transform);
			}
		}
		base.Finish();
	}

	// Token: 0x04001BBB RID: 7099
	public FsmOwnerDefault target;
}
