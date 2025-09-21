using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C27 RID: 3111
	public class EnemyDetectTurn : FsmStateAction
	{
		// Token: 0x06005EAE RID: 24238 RVA: 0x001DF325 File Offset: 0x001DD525
		public override void Reset()
		{
			this.Target = null;
			this.WallDistance = 1f;
			this.GroundAheadDistance = 0.5f;
			this.DefaultFacingRight = null;
			this.EveryFrame = true;
		}

		// Token: 0x06005EAF RID: 24239 RVA: 0x001DF35C File Offset: 0x001DD55C
		public override void OnDrawActionGizmos()
		{
			this.CacheComponents();
			if (!this.box)
			{
				return;
			}
			this.IsRaysHittingWall(true);
			this.IsRaysHittingGroundFront(true);
			this.IsRaysHittingGroundCentre(true);
		}

		// Token: 0x06005EB0 RID: 24240 RVA: 0x001DF38A File Offset: 0x001DD58A
		public override void OnEnter()
		{
			this.CacheComponents();
			this.Evaluate();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005EB1 RID: 24241 RVA: 0x001DF3A6 File Offset: 0x001DD5A6
		public override void OnUpdate()
		{
			this.isMoving = (Mathf.Abs(this.body.linearVelocity.x) > 0f);
			this.Evaluate();
		}

		// Token: 0x06005EB2 RID: 24242 RVA: 0x001DF3D0 File Offset: 0x001DD5D0
		private void Evaluate()
		{
			if (this.IsRaysHittingWall(false) || (!this.IsRaysHittingGroundFront(false) && this.IsRaysHittingGroundCentre(false)))
			{
				base.Fsm.Event(this.ShouldTurn);
			}
		}

		// Token: 0x06005EB3 RID: 24243 RVA: 0x001DF404 File Offset: 0x001DD604
		private void CacheComponents()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.transform = safe.transform;
				this.box = safe.GetComponent<BoxCollider2D>();
			}
		}

		// Token: 0x06005EB4 RID: 24244 RVA: 0x001DF440 File Offset: 0x001DD640
		private bool IsRaysHittingWall(bool isDrawingGizmos = false)
		{
			float movingDirection = this.GetMovingDirection();
			Bounds bounds = this.box.bounds;
			Vector2 vector = bounds.max;
			Vector2 vector2 = bounds.min;
			Vector2 vector3 = (movingDirection > 0f) ? Vector2.right : Vector2.left;
			if (movingDirection < 0f)
			{
				vector.x = vector2.x;
			}
			else
			{
				vector2.x = vector.x;
			}
			vector.x -= 0.1f * movingDirection;
			vector2.x -= 0.1f * movingDirection;
			vector.y -= 0.5f;
			vector2.y += 0.5f;
			float num = (this.body ? Mathf.Max(this.WallDistance.Value, this.body.linearVelocity.x * Time.fixedDeltaTime) : this.WallDistance.Value) + 0.1f;
			if (isDrawingGizmos)
			{
				Gizmos.color = (this.isMoving ? Color.yellow : Color.green);
				Gizmos.DrawLine(vector, vector + vector3 * num);
				Gizmos.DrawLine(vector2, vector2 + vector3 * num);
				return false;
			}
			bool flag = Helper.IsRayHittingNoTriggers(vector, vector3, num, 33024);
			bool flag2 = Helper.IsRayHittingNoTriggers(vector2, vector3, num, 33024);
			return flag || flag2;
		}

		// Token: 0x06005EB5 RID: 24245 RVA: 0x001DF5C0 File Offset: 0x001DD7C0
		private bool IsRaysHittingGroundFront(bool isDrawingGizmos = false)
		{
			float movingDirection = this.GetMovingDirection();
			Bounds bounds = this.box.bounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.max;
			Vector2 vector3 = bounds.center;
			if (movingDirection > 0f)
			{
				vector3.x = vector2.x + this.GroundAheadDistance.Value;
			}
			else
			{
				vector3.x = vector.x - this.GroundAheadDistance.Value;
			}
			float num = vector3.y - vector.y + 0.5f;
			if (isDrawingGizmos)
			{
				Gizmos.color = (this.isMoving ? Color.yellow : Color.green);
				Gizmos.DrawLine(vector3, vector3 + Vector2.down * num);
				return false;
			}
			return Helper.IsRayHittingNoTriggers(vector3, Vector2.down, num, 33024);
		}

		// Token: 0x06005EB6 RID: 24246 RVA: 0x001DF6A8 File Offset: 0x001DD8A8
		private bool IsRaysHittingGroundCentre(bool isDrawingGizmos = false)
		{
			this.GetMovingDirection();
			Bounds bounds = this.box.bounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.center;
			float num = vector2.y - vector.y + 0.5f;
			if (isDrawingGizmos)
			{
				Gizmos.color = (this.isMoving ? Color.yellow : Color.green);
				Gizmos.DrawLine(vector2, vector2 + Vector2.down * num);
				return false;
			}
			return Helper.IsRayHittingNoTriggers(vector2, Vector2.down, num, 33024);
		}

		// Token: 0x06005EB7 RID: 24247 RVA: 0x001DF748 File Offset: 0x001DD948
		private float GetMovingDirection()
		{
			if (!this.body || this.body.linearVelocity.x == 0f)
			{
				return this.GetFacingDirection();
			}
			return Mathf.Sign(this.body.linearVelocity.x);
		}

		// Token: 0x06005EB8 RID: 24248 RVA: 0x001DF795 File Offset: 0x001DD995
		private float GetFacingDirection()
		{
			return Mathf.Sign(this.transform.localScale.x) * (float)(this.DefaultFacingRight.Value ? 1 : -1);
		}

		// Token: 0x04005B3D RID: 23357
		private const float SKIN_WIDTH = 0.1f;

		// Token: 0x04005B3E RID: 23358
		private const float TOP_RAY_PADDING = 0.5f;

		// Token: 0x04005B3F RID: 23359
		private const float BOTTOM_RAY_PADDING = 0.5f;

		// Token: 0x04005B40 RID: 23360
		private const float DOWN_RAY_DISTANCE = 0.5f;

		// Token: 0x04005B41 RID: 23361
		private const int LAYERMASK = 33024;

		// Token: 0x04005B42 RID: 23362
		public FsmOwnerDefault Target;

		// Token: 0x04005B43 RID: 23363
		public FsmFloat WallDistance;

		// Token: 0x04005B44 RID: 23364
		public FsmFloat GroundAheadDistance;

		// Token: 0x04005B45 RID: 23365
		public FsmBool DefaultFacingRight;

		// Token: 0x04005B46 RID: 23366
		public FsmEvent ShouldTurn;

		// Token: 0x04005B47 RID: 23367
		public bool EveryFrame;

		// Token: 0x04005B48 RID: 23368
		private bool isMoving;

		// Token: 0x04005B49 RID: 23369
		private BoxCollider2D box;

		// Token: 0x04005B4A RID: 23370
		private Rigidbody2D body;

		// Token: 0x04005B4B RID: 23371
		private Transform transform;
	}
}
