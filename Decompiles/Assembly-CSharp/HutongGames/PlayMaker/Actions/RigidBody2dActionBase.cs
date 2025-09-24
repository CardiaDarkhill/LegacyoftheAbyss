using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB2 RID: 3506
	public abstract class RigidBody2dActionBase : FsmStateAction
	{
		// Token: 0x060065BA RID: 26042 RVA: 0x002018C9 File Offset: 0x001FFAC9
		protected void CacheRigidBody2d(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			this.rb2d = go.GetComponent<Rigidbody2D>();
			if (this.rb2d == null)
			{
				base.LogWarning("Missing rigid body 2D: " + go.name);
				return;
			}
		}

		// Token: 0x040064DD RID: 25821
		protected Rigidbody2D rb2d;
	}
}
