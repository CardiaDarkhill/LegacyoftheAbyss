using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D64 RID: 3428
	[ActionCategory("Physics 2d")]
	public class ShoveFromWall : RigidBody2dActionBase
	{
		// Token: 0x06006439 RID: 25657 RVA: 0x001F91D5 File Offset: 0x001F73D5
		public override void Reset()
		{
			this.gameObject = null;
			this.shoveForce = null;
			this.rayLength = null;
			this.everyFrame = false;
			this.checkUp = true;
			this.checkDown = true;
			this.checkLeft = true;
			this.checkRight = true;
		}

		// Token: 0x0600643A RID: 25658 RVA: 0x001F920F File Offset: 0x001F740F
		public override void OnPreprocess()
		{
			base.OnPreprocess();
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600643B RID: 25659 RVA: 0x001F9223 File Offset: 0x001F7423
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoShove();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600643C RID: 25660 RVA: 0x001F9250 File Offset: 0x001F7450
		public override void OnFixedUpdate()
		{
			this.DoShove();
		}

		// Token: 0x0600643D RID: 25661 RVA: 0x001F9258 File Offset: 0x001F7458
		private void DoShove()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
			}
			if (!this.rb2d)
			{
				return;
			}
			Vector2 origin = new Vector2(this.go.transform.position.x, this.go.transform.position.y);
			Vector2 vector = new Vector2(0f, 0f);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			int layerMask = 256;
			RaycastHit2D raycastHit2D;
			if (this.checkUp && Helper.Raycast2DHit(origin, Vector2.up, this.rayLength.Value, layerMask, out raycastHit2D))
			{
				flag = true;
			}
			if (this.checkDown && Helper.Raycast2DHit(origin, Vector2.down, this.rayLength.Value, layerMask, out raycastHit2D))
			{
				flag2 = true;
			}
			if (this.checkLeft && Helper.Raycast2DHit(origin, Vector2.left, this.rayLength.Value, layerMask, out raycastHit2D))
			{
				flag3 = true;
			}
			if (this.checkRight && Helper.Raycast2DHit(origin, Vector2.right, this.rayLength.Value, layerMask, out raycastHit2D))
			{
				flag4 = true;
			}
			if ((flag && flag2) || (!flag && !flag2))
			{
				vector = new Vector2(vector.x, 0f);
			}
			else if (flag)
			{
				vector = new Vector2(vector.x, -this.shoveForce.Value);
			}
			else if (flag2)
			{
				vector = new Vector2(vector.x, this.shoveForce.Value);
			}
			if ((flag3 && flag4) || (!flag3 && !flag4))
			{
				vector = new Vector2(0f, vector.y);
			}
			else if (flag3)
			{
				vector = new Vector2(this.shoveForce.Value, vector.y);
			}
			else if (flag4)
			{
				vector = new Vector2(-this.shoveForce.Value, vector.y);
			}
			this.rb2d.AddForce(vector);
		}

		// Token: 0x040062AB RID: 25259
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040062AC RID: 25260
		public FsmFloat shoveForce;

		// Token: 0x040062AD RID: 25261
		public FsmFloat rayLength;

		// Token: 0x040062AE RID: 25262
		public bool checkUp;

		// Token: 0x040062AF RID: 25263
		public bool checkDown;

		// Token: 0x040062B0 RID: 25264
		public bool checkLeft;

		// Token: 0x040062B1 RID: 25265
		public bool checkRight;

		// Token: 0x040062B2 RID: 25266
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x040062B3 RID: 25267
		private GameObject go;
	}
}
