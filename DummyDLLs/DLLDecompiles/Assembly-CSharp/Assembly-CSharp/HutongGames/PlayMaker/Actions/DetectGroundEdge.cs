using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C14 RID: 3092
	public class DetectGroundEdge : FsmStateAction
	{
		// Token: 0x06005E42 RID: 24130 RVA: 0x001DB34C File Offset: 0x001D954C
		public override void Reset()
		{
			this.Body = null;
			this.EdgeDistance = 0.5f;
			this.GroundDistance = 1f;
			this.EveryFixedUpdate = true;
		}

		// Token: 0x06005E43 RID: 24131 RVA: 0x001DB37C File Offset: 0x001D957C
		public override void OnEnter()
		{
			GameObject safe = this.Body.GetSafe(this);
			if (safe)
			{
				this.collider = safe.GetComponent<Collider2D>();
				this.body = safe.GetComponent<Rigidbody2D>();
			}
			if (!this.collider || !this.body)
			{
				base.Finish();
				return;
			}
			this.DoAction();
			if (!this.EveryFixedUpdate)
			{
				base.Finish();
			}
		}

		// Token: 0x06005E44 RID: 24132 RVA: 0x001DB3EB File Offset: 0x001D95EB
		public override void OnFixedUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06005E45 RID: 24133 RVA: 0x001DB3F4 File Offset: 0x001D95F4
		private void DoAction()
		{
			bool flag = this.body.linearVelocity.x > 0f;
			Bounds bounds = this.collider.bounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.center;
			ref Vector2 ptr = bounds.extents;
			int layerMask = 256;
			Vector2 vector3 = new Vector2(vector2.x, vector.y + 0.5f);
			float num = ptr.x + this.EdgeDistance.Value;
			Vector2 vector4 = flag ? Vector2.right : Vector2.left;
			Vector2 vector5 = vector3 + vector4 * num;
			Debug.DrawLine(vector3, vector5);
			if (Helper.Raycast2D(vector3, vector4, num, layerMask).collider != null)
			{
				base.Fsm.Event(this.FoundWall);
			}
			float num2 = 0.5f + this.GroundDistance.Value;
			Debug.DrawLine(vector5, vector5 + Vector2.down * num2);
			if (Helper.Raycast2D(vector5, Vector2.down, num2, layerMask).collider == null)
			{
				base.Fsm.Event(this.FoundEdge);
			}
		}

		// Token: 0x04005A8A RID: 23178
		private const float RAY_HEIGHT = 0.5f;

		// Token: 0x04005A8B RID: 23179
		[CheckForComponent(typeof(Collider2D), typeof(Rigidbody2D))]
		public FsmOwnerDefault Body;

		// Token: 0x04005A8C RID: 23180
		public FsmFloat EdgeDistance;

		// Token: 0x04005A8D RID: 23181
		public FsmFloat GroundDistance;

		// Token: 0x04005A8E RID: 23182
		public bool EveryFixedUpdate;

		// Token: 0x04005A8F RID: 23183
		public FsmEvent FoundWall;

		// Token: 0x04005A90 RID: 23184
		public FsmEvent FoundEdge;

		// Token: 0x04005A91 RID: 23185
		private Collider2D collider;

		// Token: 0x04005A92 RID: 23186
		private Rigidbody2D body;
	}
}
