using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD7 RID: 3031
	[ActionCategory("Enemy AI")]
	[Tooltip("Calculates the required initial velocity required to launch a Rigidbody2D and have it land exactly at the target position (assuming target is on the ground).")]
	public class CalculateProjectileVelocity : FsmStateAction
	{
		// Token: 0x06005CEF RID: 23791 RVA: 0x001D3399 File Offset: 0x001D1599
		public override void Reset()
		{
			this.Target = null;
			this.TargetPosition = null;
			this.FireAngle = null;
			this.ImpulseVelocity = null;
		}

		// Token: 0x06005CF0 RID: 23792 RVA: 0x001D33B8 File Offset: 0x001D15B8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			this.transform = safe.transform;
			this.body = safe.GetComponent<Rigidbody2D>();
			this.collider = safe.GetComponent<Collider2D>();
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CF1 RID: 23793 RVA: 0x001D340A File Offset: 0x001D160A
		public override void OnUpdate()
		{
			if (this.EveryFrame)
			{
				this.DoAction();
			}
		}

		// Token: 0x06005CF2 RID: 23794 RVA: 0x001D341C File Offset: 0x001D161C
		private void DoAction()
		{
			Vector2 value = this.TargetPosition.Value;
			Vector2 vector = this.collider ? this.transform.TransformPoint(this.collider.offset) : this.transform.position;
			float num = Mathf.Abs(Physics2D.gravity.y) * this.body.gravityScale;
			if (Math.Abs(num) < Mathf.Epsilon)
			{
				Debug.LogError("Projectile gravity is 0! Ballistic velocity will also be zero!", base.Owner);
			}
			Vector2 value2 = this.CalculateBallisticVelocity(vector, value, num, this.FireAngle.Value);
			Debug.DrawLine(vector, vector + value2.normalized);
			this.ImpulseVelocity.Value = value2;
		}

		// Token: 0x06005CF3 RID: 23795 RVA: 0x001D34EC File Offset: 0x001D16EC
		private Vector3 CalculateBallisticVelocity(Vector2 sourcePos, Vector2 targetPos, float gravity, float angle)
		{
			Vector2 vector = targetPos - sourcePos;
			float y = vector.y;
			vector.y = 0f;
			float num = vector.magnitude;
			float num2 = angle * 0.017453292f;
			vector.y = num * Mathf.Tan(num2);
			num += y / Mathf.Tan(num2);
			return Mathf.Sqrt(num * gravity / Mathf.Sin(2f * num2)) * vector.normalized;
		}

		// Token: 0x04005888 RID: 22664
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault Target;

		// Token: 0x04005889 RID: 22665
		private Transform transform;

		// Token: 0x0400588A RID: 22666
		private Rigidbody2D body;

		// Token: 0x0400588B RID: 22667
		private Collider2D collider;

		// Token: 0x0400588C RID: 22668
		public FsmVector2 TargetPosition;

		// Token: 0x0400588D RID: 22669
		public FsmFloat FireAngle;

		// Token: 0x0400588E RID: 22670
		[UIHint(UIHint.Variable)]
		public FsmVector2 ImpulseVelocity;

		// Token: 0x0400588F RID: 22671
		public bool EveryFrame;
	}
}
