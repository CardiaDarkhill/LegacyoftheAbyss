using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA3 RID: 2979
	public class AdjustColliderPosToEdgeV2 : FsmStateAction
	{
		// Token: 0x06005C0D RID: 23565 RVA: 0x001CF248 File Offset: 0x001CD448
		public override void Reset()
		{
			this.Target = null;
			this.Width = 1f;
		}

		// Token: 0x06005C0E RID: 23566 RVA: 0x001CF261 File Offset: 0x001CD461
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005C0F RID: 23567 RVA: 0x001CF270 File Offset: 0x001CD470
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

		// Token: 0x06005C10 RID: 23568 RVA: 0x001CF2DC File Offset: 0x001CD4DC
		public override void OnEnter()
		{
			this.eventProxy = CustomPlayMakerCollisionStay2D.GetEventSender(base.Fsm.Owner.gameObject);
			this.responder = this.eventProxy.Add(this, new Action<Collision2D>(this.DoCollisionStay2D));
			this.hasCollided = false;
			this.slideTimeLeft = 0f;
		}

		// Token: 0x06005C11 RID: 23569 RVA: 0x001CF335 File Offset: 0x001CD535
		public override void OnExit()
		{
			this.RemoveEventProxy();
		}

		// Token: 0x06005C12 RID: 23570 RVA: 0x001CF33D File Offset: 0x001CD53D
		public override void OnFixedUpdate()
		{
			if (this.slideTimeLeft <= 0f)
			{
				return;
			}
			this.slideTimeLeft -= Time.deltaTime;
			this.MoveBody();
		}

		// Token: 0x06005C13 RID: 23571 RVA: 0x001CF365 File Offset: 0x001CD565
		public override void DoCollisionStay2D(Collision2D collisionInfo)
		{
			if (this.hasCollided)
			{
				return;
			}
			this.hasCollided = true;
			this.UpdateEdgeAdjustX();
		}

		// Token: 0x06005C14 RID: 23572 RVA: 0x001CF380 File Offset: 0x001CD580
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
				this.edgeAdjustVelocity = num / this.slideTime.Value;
				this.slideTimeLeft = this.slideTime.Value;
				this.MoveBody();
				return;
			}
			this.edgeAdjustVelocity = 0f;
			this.slideTimeLeft = 0f;
		}

		// Token: 0x06005C15 RID: 23573 RVA: 0x001CF479 File Offset: 0x001CD679
		private void MoveBody()
		{
			this.body.MovePosition(this.body.position + new Vector2(this.edgeAdjustVelocity * Time.deltaTime, 0f));
		}

		// Token: 0x04005774 RID: 22388
		[CheckForComponent(typeof(Rigidbody2D), typeof(Collider2D))]
		public FsmOwnerDefault Target;

		// Token: 0x04005775 RID: 22389
		public FsmFloat Width;

		// Token: 0x04005776 RID: 22390
		public FsmFloat slideTime = 0.15f;

		// Token: 0x04005777 RID: 22391
		private Rigidbody2D body;

		// Token: 0x04005778 RID: 22392
		private bool hasCollided;

		// Token: 0x04005779 RID: 22393
		private float slideTimeLeft;

		// Token: 0x0400577A RID: 22394
		private float edgeAdjustVelocity;

		// Token: 0x0400577B RID: 22395
		private CustomPlayMakerCollisionStay2D eventProxy;

		// Token: 0x0400577C RID: 22396
		private CustomPlayMakerPhysicsEvent<Collision2D>.EventResponder responder;
	}
}
