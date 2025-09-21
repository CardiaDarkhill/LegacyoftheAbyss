using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6B RID: 3179
	[ActionCategory("Enemy AI")]
	[Tooltip("Finds the furthest ground point for an enemy to jump to without going off an edge.")]
	public class GetGroundPointClampedToEdge : FsmStateAction
	{
		// Token: 0x06006002 RID: 24578 RVA: 0x001E6728 File Offset: 0x001E4928
		public override void Reset()
		{
			this.TargetDistance = null;
			this.MinJumpDistance = new FsmFloat(1f);
			this.DefaultFacingRight = new FsmBool(true);
			this.ReductionDistance = new FsmFloat(1f);
			this.MaxGroundDistance = new FsmFloat(1.5f);
			this.GroundRayHeight = new FsmFloat(0.1f);
			this.DidFindGroundPoint = null;
			this.GroundPoint = null;
		}

		// Token: 0x06006003 RID: 24579 RVA: 0x001E67B0 File Offset: 0x001E49B0
		public override void OnEnter()
		{
			GameObject safe = this.Source.GetSafe(this);
			this.transform = safe.transform;
			this.collider = safe.GetComponent<BoxCollider2D>();
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006004 RID: 24580 RVA: 0x001E67F6 File Offset: 0x001E49F6
		public override void OnUpdate()
		{
			if (this.EveryFrame)
			{
				this.DoAction();
			}
		}

		// Token: 0x06006005 RID: 24581 RVA: 0x001E6808 File Offset: 0x001E4A08
		private void DoAction()
		{
			bool flag = this.DefaultFacingRight.Value;
			Vector2 vector = this.collider.offset - new Vector2(0f, this.collider.size.y / 2f - this.GroundRayHeight.Value);
			float num = this.TargetDistance.Value;
			if (num < 0f)
			{
				flag = !flag;
				num *= -1f;
			}
			float num2 = this.collider.size.x / 2f;
			Vector2 directionLocal = flag ? Vector2.right : Vector2.left;
			float length = num + num2;
			RaycastHit2D raycastHit2D = this.CastRayLocal(vector, directionLocal, length, false);
			if (raycastHit2D.collider != null)
			{
				num = raycastHit2D.distance - num2;
			}
			Vector2 down = Vector2.down;
			float length2 = this.MaxGroundDistance.Value + this.GroundRayHeight.Value;
			while (num >= this.MinJumpDistance.Value)
			{
				Vector2 vector2 = vector + new Vector2(flag ? num : (-num), 0f);
				RaycastHit2D raycastHit2D2 = this.CastRayLocal(vector2, down, length2, false);
				if (raycastHit2D2.collider != null)
				{
					Vector2 point = raycastHit2D2.point;
					point.y -= this.collider.offset.y - this.collider.size.y / 2f;
					float num3 = this.collider.offset.x + this.collider.size.x / 2f;
					if (this.CastRayLocal(vector2 + new Vector2(num3 - 0.1f, 0f), down, length2, true).collider != null)
					{
						point.x += num3 * this.transform.localScale.x;
					}
					if (this.CastRayLocal(vector2 - new Vector2(num3 - 0.1f, 0f), down, length2, true).collider != null)
					{
						point.x -= num3 * this.transform.localScale.x;
					}
					if (Mathf.Abs(point.x - this.transform.position.x) >= this.MinJumpDistance.Value)
					{
						this.GroundPoint.Value = point;
						this.DidFindGroundPoint.Value = true;
						return;
					}
					break;
				}
				else
				{
					num -= this.ReductionDistance.Value;
				}
			}
			this.DidFindGroundPoint.Value = false;
		}

		// Token: 0x06006006 RID: 24582 RVA: 0x001E6AAC File Offset: 0x001E4CAC
		private RaycastHit2D CastRayLocal(Vector2 originLocal, Vector2 directionLocal, float length, bool secondaryDebug = false)
		{
			Vector2 origin = this.transform.TransformPoint(originLocal);
			Vector2 direction = this.transform.TransformVector(directionLocal);
			return Helper.Raycast2D(origin, direction, length, 256);
		}

		// Token: 0x04005D53 RID: 23891
		private const float FIT_SKIN_WIDTH_SIDE = 0.1f;

		// Token: 0x04005D54 RID: 23892
		[RequiredField]
		[CheckForComponent(typeof(BoxCollider2D))]
		public FsmOwnerDefault Source;

		// Token: 0x04005D55 RID: 23893
		public FsmFloat TargetDistance;

		// Token: 0x04005D56 RID: 23894
		public FsmFloat MinJumpDistance;

		// Token: 0x04005D57 RID: 23895
		public FsmBool DefaultFacingRight;

		// Token: 0x04005D58 RID: 23896
		public FsmFloat ReductionDistance;

		// Token: 0x04005D59 RID: 23897
		public FsmFloat MaxGroundDistance;

		// Token: 0x04005D5A RID: 23898
		public FsmFloat GroundRayHeight;

		// Token: 0x04005D5B RID: 23899
		[UIHint(UIHint.Variable)]
		public FsmBool DidFindGroundPoint;

		// Token: 0x04005D5C RID: 23900
		[UIHint(UIHint.Variable)]
		public FsmVector2 GroundPoint;

		// Token: 0x04005D5D RID: 23901
		public bool EveryFrame;

		// Token: 0x04005D5E RID: 23902
		private Transform transform;

		// Token: 0x04005D5F RID: 23903
		private BoxCollider2D collider;
	}
}
