using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA2 RID: 3490
	public class TweenVelocity2D : FsmStateAction
	{
		// Token: 0x06006557 RID: 25943 RVA: 0x001FFAD0 File Offset: 0x001FDCD0
		public bool ShowFromComponents()
		{
			return !this.FromVelocityVector.IsNone;
		}

		// Token: 0x06006558 RID: 25944 RVA: 0x001FFAE0 File Offset: 0x001FDCE0
		public bool ShowToComponents()
		{
			return !this.ToVelocityVector.IsNone;
		}

		// Token: 0x06006559 RID: 25945 RVA: 0x001FFAF0 File Offset: 0x001FDCF0
		public override void Reset()
		{
			this.Target = null;
			this.FromVelocityVector = null;
			this.FromVelocityX = null;
			this.FromVelocityY = null;
			this.ToVelocityVector = null;
			this.ToVelocityX = null;
			this.ToVelocityY = null;
			this.Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f)
			};
			this.Duration = 1f;
		}

		// Token: 0x0600655A RID: 25946 RVA: 0x001FFB68 File Offset: 0x001FDD68
		public override void Awake()
		{
			this.OnPreprocess();
		}

		// Token: 0x0600655B RID: 25947 RVA: 0x001FFB70 File Offset: 0x001FDD70
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600655C RID: 25948 RVA: 0x001FFB80 File Offset: 0x001FDD80
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			this.body = safe.GetComponent<Rigidbody2D>();
			if (!this.body)
			{
				base.Finish();
				return;
			}
			this.elapsed = 0f;
			this.DoSetVelocity();
		}

		// Token: 0x0600655D RID: 25949 RVA: 0x001FFBDA File Offset: 0x001FDDDA
		public override void OnFixedUpdate()
		{
			this.elapsed += Time.fixedDeltaTime;
			this.DoSetVelocity();
		}

		// Token: 0x0600655E RID: 25950 RVA: 0x001FFBF4 File Offset: 0x001FDDF4
		private void DoSetVelocity()
		{
			if (this.elapsed > this.Duration.Value)
			{
				this.SetVelocity(1f);
				base.Finish();
				return;
			}
			float velocity = this.Curve.curve.Evaluate(this.elapsed / this.Duration.Value);
			this.SetVelocity(velocity);
		}

		// Token: 0x0600655F RID: 25951 RVA: 0x001FFC50 File Offset: 0x001FDE50
		private void SetVelocity(float curveTime)
		{
			Vector2 a = this.CollapseVectorVariables(this.FromVelocityVector, this.FromVelocityX, this.FromVelocityY);
			Vector2 b = this.CollapseVectorVariables(this.ToVelocityVector, this.ToVelocityX, this.ToVelocityY);
			Vector2 linearVelocity = Vector2.Lerp(a, b, curveTime);
			if (this.ToVelocityVector.IsNone)
			{
				Vector2 linearVelocity2 = this.body.linearVelocity;
				if (this.ToVelocityX.IsNone)
				{
					linearVelocity.x = linearVelocity2.x;
				}
				if (this.ToVelocityY.IsNone)
				{
					linearVelocity.y = linearVelocity2.y;
				}
			}
			this.body.linearVelocity = linearVelocity;
		}

		// Token: 0x06006560 RID: 25952 RVA: 0x001FFCEF File Offset: 0x001FDEEF
		private Vector2 CollapseVectorVariables(FsmVector2 vector, FsmFloat x, FsmFloat y)
		{
			if (!vector.IsNone)
			{
				return vector.Value;
			}
			return new Vector2(x.Value, y.Value);
		}

		// Token: 0x04006456 RID: 25686
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault Target;

		// Token: 0x04006457 RID: 25687
		public FsmVector2 FromVelocityVector;

		// Token: 0x04006458 RID: 25688
		[HideIf("ShowFromComponents")]
		public FsmFloat FromVelocityX;

		// Token: 0x04006459 RID: 25689
		[HideIf("ShowFromComponents")]
		public FsmFloat FromVelocityY;

		// Token: 0x0400645A RID: 25690
		public FsmVector2 ToVelocityVector;

		// Token: 0x0400645B RID: 25691
		[HideIf("ShowToComponents")]
		public FsmFloat ToVelocityX;

		// Token: 0x0400645C RID: 25692
		[HideIf("ShowToComponents")]
		public FsmFloat ToVelocityY;

		// Token: 0x0400645D RID: 25693
		public FsmAnimationCurve Curve;

		// Token: 0x0400645E RID: 25694
		public FsmFloat Duration;

		// Token: 0x0400645F RID: 25695
		private float elapsed;

		// Token: 0x04006460 RID: 25696
		private Rigidbody2D body;
	}
}
