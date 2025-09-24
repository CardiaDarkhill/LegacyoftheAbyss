using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D63 RID: 3427
	[ActionCategory("Physics 2d")]
	public class ShoveFromBouncer : RigidBody2dActionBase
	{
		// Token: 0x06006433 RID: 25651 RVA: 0x001F8F52 File Offset: 0x001F7152
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

		// Token: 0x06006434 RID: 25652 RVA: 0x001F8F8C File Offset: 0x001F718C
		public override void OnPreprocess()
		{
			base.OnPreprocess();
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006435 RID: 25653 RVA: 0x001F8FA0 File Offset: 0x001F71A0
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoShove();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006436 RID: 25654 RVA: 0x001F8FCD File Offset: 0x001F71CD
		public override void OnFixedUpdate()
		{
			this.DoShove();
		}

		// Token: 0x06006437 RID: 25655 RVA: 0x001F8FD8 File Offset: 0x001F71D8
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
			int layerMask = 16777216;
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

		// Token: 0x040062A2 RID: 25250
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040062A3 RID: 25251
		public FsmFloat shoveForce;

		// Token: 0x040062A4 RID: 25252
		public FsmFloat rayLength;

		// Token: 0x040062A5 RID: 25253
		public bool checkUp;

		// Token: 0x040062A6 RID: 25254
		public bool checkDown;

		// Token: 0x040062A7 RID: 25255
		public bool checkLeft;

		// Token: 0x040062A8 RID: 25256
		public bool checkRight;

		// Token: 0x040062A9 RID: 25257
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x040062AA RID: 25258
		private GameObject go;
	}
}
