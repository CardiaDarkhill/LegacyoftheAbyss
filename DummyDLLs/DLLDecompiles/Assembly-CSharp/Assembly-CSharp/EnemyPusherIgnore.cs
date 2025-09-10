using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200032F RID: 815
[ActionCategory("Hollow Knight")]
public class EnemyPusherIgnore : FsmStateAction
{
	// Token: 0x06001C8B RID: 7307 RVA: 0x00085144 File Offset: 0x00083344
	public override void Reset()
	{
		this.target = null;
		this.other = new FsmGameObject
		{
			UseVariable = true
		};
	}

	// Token: 0x06001C8C RID: 7308 RVA: 0x00085160 File Offset: 0x00083360
	public override void OnEnter()
	{
		GameObject safe = this.target.GetSafe(this);
		if (safe && this.other.Value)
		{
			EnemyPusher componentInChildren = this.other.Value.GetComponentInChildren<EnemyPusher>();
			if (componentInChildren)
			{
				Collider2D component = safe.GetComponent<Collider2D>();
				Collider2D component2 = componentInChildren.GetComponent<Collider2D>();
				if (component && component2)
				{
					Physics2D.IgnoreCollision(component, component2);
				}
			}
		}
		base.Finish();
	}

	// Token: 0x04001BBC RID: 7100
	public FsmOwnerDefault target;

	// Token: 0x04001BBD RID: 7101
	public FsmGameObject other;
}
