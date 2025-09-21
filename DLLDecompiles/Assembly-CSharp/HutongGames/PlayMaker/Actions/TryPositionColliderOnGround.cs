using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE0 RID: 3296
	public sealed class TryPositionColliderOnGround : FsmStateAction
	{
		// Token: 0x0600620E RID: 25102 RVA: 0x001EFEB2 File Offset: 0x001EE0B2
		public override void Reset()
		{
			this.Target = null;
			this.GroundPos = new FsmVector2
			{
				UseVariable = true
			};
			this.MaxCorrection = null;
			this.ClampOnFailure = null;
			this.SuccessEvent = null;
			this.FailedEvent = null;
		}

		// Token: 0x0600620F RID: 25103 RVA: 0x001EFEEC File Offset: 0x001EE0EC
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				Collider2D component = safe.GetComponent<Collider2D>();
				if (component)
				{
					Bounds bounds = component.bounds;
					Vector2 vector = bounds.center;
					Vector2 vector2 = new Vector2(vector.x, bounds.min.y);
					RaycastHit2D raycastHit2D;
					if (Helper.IsRayHittingNoTriggers((!this.GroundPos.IsNone) ? this.GroundPos.Value : vector, Vector2.down, 10f, 256, out raycastHit2D))
					{
						float num = safe.transform.position.y - vector2.y;
						Vector2 point = raycastHit2D.point;
						point.y += num;
						float num2 = safe.transform.position.y - point.y;
						if (this.MaxCorrection.Value > 0f && Mathf.Abs(num2) > this.MaxCorrection.Value)
						{
							base.Fsm.Event(this.FailedEvent);
							if (!this.ClampOnFailure.Value)
							{
								base.Finish();
								return;
							}
							point.y = safe.transform.position.y + Mathf.Clamp(num2, -this.MaxCorrection.Value, this.MaxCorrection.Value);
						}
						else
						{
							base.Fsm.Event(this.SuccessEvent);
						}
						safe.transform.SetPosition2D(point);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0400601D RID: 24605
		[CheckForComponent(typeof(Collider2D))]
		public FsmOwnerDefault Target;

		// Token: 0x0400601E RID: 24606
		public FsmVector2 GroundPos;

		// Token: 0x0400601F RID: 24607
		public FsmFloat MaxCorrection;

		// Token: 0x04006020 RID: 24608
		public FsmBool ClampOnFailure;

		// Token: 0x04006021 RID: 24609
		public FsmEvent SuccessEvent;

		// Token: 0x04006022 RID: 24610
		public FsmEvent FailedEvent;
	}
}
