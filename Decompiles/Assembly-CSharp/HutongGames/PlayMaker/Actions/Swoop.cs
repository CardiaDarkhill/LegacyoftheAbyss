using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D84 RID: 3460
	[ActionCategory("Enemy AI")]
	public class Swoop : FsmStateAction
	{
		// Token: 0x060064C1 RID: 25793 RVA: 0x001FCCB8 File Offset: 0x001FAEB8
		public override void Reset()
		{
			this.Swooper = null;
			this.Target = null;
			this.TargetPosition = null;
			this.Speed = null;
			this.SwoopCurveX = null;
			this.SwoopCurveY = null;
			this.EndEvent = null;
		}

		// Token: 0x060064C2 RID: 25794 RVA: 0x001FCCEB File Offset: 0x001FAEEB
		public bool IsTargetSpecified()
		{
			return !this.Target.IsNone;
		}

		// Token: 0x060064C3 RID: 25795 RVA: 0x001FCCFB File Offset: 0x001FAEFB
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x060064C4 RID: 25796 RVA: 0x001FCD09 File Offset: 0x001FAF09
		public override void Awake()
		{
			this.OnPreprocess();
		}

		// Token: 0x060064C5 RID: 25797 RVA: 0x001FCD14 File Offset: 0x001FAF14
		public override void OnEnter()
		{
			if (this.Target.Value)
			{
				this.target = this.Target.Value.transform;
			}
			GameObject safe = this.Swooper.GetSafe(this);
			if (safe)
			{
				this.transform = safe.transform;
				this.body = safe.GetComponent<Rigidbody2D>();
			}
			if (this.body == null || this.Speed.Value <= 0f)
			{
				this.End();
				return;
			}
			this.elapsed = 0f;
			this.startPosition = this.transform.position;
			this.targetPosition = (this.target ? this.target.position : this.TargetPosition.Value);
			this.duration = Vector2.Distance(this.startPosition, this.targetPosition) / this.Speed.Value;
			this.Evaluate(0f);
		}

		// Token: 0x060064C6 RID: 25798 RVA: 0x001FCE1C File Offset: 0x001FB01C
		public override void OnFixedUpdate()
		{
			if (this.elapsed < this.duration)
			{
				float t = this.elapsed / this.duration;
				this.Evaluate(t);
			}
			else
			{
				this.End();
			}
			this.elapsed += Time.fixedDeltaTime;
		}

		// Token: 0x060064C7 RID: 25799 RVA: 0x001FCE68 File Offset: 0x001FB068
		private void Evaluate(float t)
		{
			Vector2 b = this.transform.position;
			float t2 = this.SwoopCurveX.curve.Evaluate(t);
			float t3 = this.SwoopCurveY.curve.Evaluate(t);
			float x = Mathf.LerpUnclamped(this.startPosition.x, this.targetPosition.x, t2);
			float y = Mathf.LerpUnclamped(this.startPosition.y, this.targetPosition.y, t3);
			Vector2 linearVelocity = (new Vector2(x, y) - b) / Time.fixedDeltaTime;
			this.body.linearVelocity = linearVelocity;
		}

		// Token: 0x060064C8 RID: 25800 RVA: 0x001FCF0C File Offset: 0x001FB10C
		private void End()
		{
			base.Fsm.Event(this.EndEvent);
			base.Finish();
		}

		// Token: 0x040063B7 RID: 25527
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault Swooper;

		// Token: 0x040063B8 RID: 25528
		private Transform transform;

		// Token: 0x040063B9 RID: 25529
		private Rigidbody2D body;

		// Token: 0x040063BA RID: 25530
		public FsmGameObject Target;

		// Token: 0x040063BB RID: 25531
		private Transform target;

		// Token: 0x040063BC RID: 25532
		[HideIf("IsTargetSpecified")]
		public FsmVector2 TargetPosition;

		// Token: 0x040063BD RID: 25533
		public FsmFloat Speed;

		// Token: 0x040063BE RID: 25534
		private float duration;

		// Token: 0x040063BF RID: 25535
		private float elapsed;

		// Token: 0x040063C0 RID: 25536
		public FsmAnimationCurve SwoopCurveX;

		// Token: 0x040063C1 RID: 25537
		public FsmAnimationCurve SwoopCurveY;

		// Token: 0x040063C2 RID: 25538
		public FsmEvent EndEvent;

		// Token: 0x040063C3 RID: 25539
		private Vector2 startPosition;

		// Token: 0x040063C4 RID: 25540
		private Vector2 targetPosition;
	}
}
