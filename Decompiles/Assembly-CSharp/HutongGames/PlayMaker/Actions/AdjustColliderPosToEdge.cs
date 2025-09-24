using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA2 RID: 2978
	public class AdjustColliderPosToEdge : FsmStateAction
	{
		// Token: 0x06005C03 RID: 23555 RVA: 0x001CEFE9 File Offset: 0x001CD1E9
		public override void Reset()
		{
			this.Target = null;
			this.Width = 1f;
		}

		// Token: 0x06005C04 RID: 23556 RVA: 0x001CF002 File Offset: 0x001CD202
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005C05 RID: 23557 RVA: 0x001CF010 File Offset: 0x001CD210
		private void RemoveEventProxy()
		{
			if (this.eventProxy)
			{
				if (this.responder != null)
				{
					this.eventProxy.Remove(this.responder);
					this.responder = null;
				}
				else
				{
					this.eventProxy.Remove(this);
				}
				this.eventProxy = null;
				return;
			}
			if (this.responder != null)
			{
				this.responder.pendingRemoval = true;
				this.responder = null;
			}
		}

		// Token: 0x06005C06 RID: 23558 RVA: 0x001CF07C File Offset: 0x001CD27C
		public override void OnEnter()
		{
			this.eventProxy = CustomPlayMakerCollisionStay2D.GetEventSender(base.Fsm.Owner.gameObject);
			this.responder = this.eventProxy.Add(this, new Action<Collision2D>(this.DoCollisionStay2D));
			this.hasCollided = false;
			this.slideTimeLeft = 0f;
		}

		// Token: 0x06005C07 RID: 23559 RVA: 0x001CF0D5 File Offset: 0x001CD2D5
		public override void OnExit()
		{
			this.RemoveEventProxy();
		}

		// Token: 0x06005C08 RID: 23560 RVA: 0x001CF0DD File Offset: 0x001CD2DD
		public override void OnFixedUpdate()
		{
			if (this.slideTimeLeft <= 0f)
			{
				return;
			}
			this.slideTimeLeft -= Time.deltaTime;
			this.MoveBody();
		}

		// Token: 0x06005C09 RID: 23561 RVA: 0x001CF105 File Offset: 0x001CD305
		public override void DoCollisionStay2D(Collision2D collisionInfo)
		{
			if (this.hasCollided)
			{
				return;
			}
			this.hasCollided = true;
			this.UpdateEdgeAdjustX();
		}

		// Token: 0x06005C0A RID: 23562 RVA: 0x001CF120 File Offset: 0x001CD320
		private void UpdateEdgeAdjustX()
		{
			float num = 0f;
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.body = safe.GetComponent<Rigidbody2D>();
				Collider2D component = safe.GetComponent<Collider2D>();
				float value = this.Width.Value;
				bool facingRight = this.body.transform.lossyScale.x > 0f;
				if (value != 0f)
				{
					float num2 = (component.bounds.size.x - value) * 0.5f;
					num = EdgeAdjustHelper.GetEdgeAdjustX(component, facingRight, num2, num2);
				}
				else
				{
					num = EdgeAdjustHelper.GetEdgeAdjustX(component, facingRight, 0f, 0f);
				}
			}
			if (num != 0f)
			{
				this.edgeAdjustVelocity = num / 0.15f;
				this.slideTimeLeft = 0.15f;
				this.MoveBody();
				return;
			}
			this.edgeAdjustVelocity = 0f;
			this.slideTimeLeft = 0f;
		}

		// Token: 0x06005C0B RID: 23563 RVA: 0x001CF20D File Offset: 0x001CD40D
		private void MoveBody()
		{
			this.body.MovePosition(this.body.position + new Vector2(this.edgeAdjustVelocity * Time.deltaTime, 0f));
		}

		// Token: 0x0400576B RID: 22379
		private const float SLIDE_TIME = 0.15f;

		// Token: 0x0400576C RID: 22380
		[CheckForComponent(typeof(Rigidbody2D), typeof(Collider2D))]
		public FsmOwnerDefault Target;

		// Token: 0x0400576D RID: 22381
		public FsmFloat Width;

		// Token: 0x0400576E RID: 22382
		private Rigidbody2D body;

		// Token: 0x0400576F RID: 22383
		private bool hasCollided;

		// Token: 0x04005770 RID: 22384
		private float slideTimeLeft;

		// Token: 0x04005771 RID: 22385
		private float edgeAdjustVelocity;

		// Token: 0x04005772 RID: 22386
		private CustomPlayMakerCollisionStay2D eventProxy;

		// Token: 0x04005773 RID: 22387
		private CustomPlayMakerPhysicsEvent<Collision2D>.EventResponder responder;
	}
}
