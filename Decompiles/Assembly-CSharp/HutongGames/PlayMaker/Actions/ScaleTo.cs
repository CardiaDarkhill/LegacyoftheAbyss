using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D05 RID: 3333
	[ActionCategory("Hollow Knight")]
	[Tooltip("Scales a transform to a level.")]
	public class ScaleTo : FsmStateAction
	{
		// Token: 0x060062A5 RID: 25253 RVA: 0x001F2FB8 File Offset: 0x001F11B8
		public override void Reset()
		{
			base.Reset();
			this.gameObject = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.UseOwner
			};
			this.target = null;
			this.duration = 1f;
			this.delay = 0f;
			this.curve = ScaleToCurves.Linear;
		}

		// Token: 0x060062A6 RID: 25254 RVA: 0x001F300C File Offset: 0x001F120C
		public override void OnEnter()
		{
			base.OnEnter();
			this.timer = 0f;
			GameObject safe = this.gameObject.GetSafe(this);
			if (safe != null)
			{
				this.transform = safe.transform;
				this.startScale = this.transform.localScale;
			}
			else
			{
				this.transform = null;
			}
			this.UpdateScaling();
		}

		// Token: 0x060062A7 RID: 25255 RVA: 0x001F306C File Offset: 0x001F126C
		public override void OnUpdate()
		{
			base.OnUpdate();
			this.UpdateScaling();
		}

		// Token: 0x060062A8 RID: 25256 RVA: 0x001F307C File Offset: 0x001F127C
		private void UpdateScaling()
		{
			if (this.transform != null)
			{
				this.timer += Time.deltaTime;
				float curved = ScaleTo.GetCurved(Mathf.Clamp01((this.timer - this.delay.Value) / this.duration.Value), this.curve);
				this.transform.localScale = Vector3.Lerp(this.startScale, this.target.Value, curved);
				if (this.timer > this.duration.Value + this.delay.Value)
				{
					this.transform.localScale = this.target.Value;
					base.Finish();
					return;
				}
			}
			else
			{
				base.Finish();
			}
		}

		// Token: 0x060062A9 RID: 25257 RVA: 0x001F313F File Offset: 0x001F133F
		private static float GetCurved(float val, ScaleToCurves curve)
		{
			switch (curve)
			{
			default:
				return ScaleTo.Linear(val);
			case ScaleToCurves.QuadraticOut:
				return ScaleTo.QuadraticOut(val);
			case ScaleToCurves.SinusoidalOut:
				return ScaleTo.SinusoidalOut(val);
			}
		}

		// Token: 0x060062AA RID: 25258 RVA: 0x001F3167 File Offset: 0x001F1367
		private static float Linear(float val)
		{
			return val;
		}

		// Token: 0x060062AB RID: 25259 RVA: 0x001F316A File Offset: 0x001F136A
		private static float QuadraticOut(float val)
		{
			return val * (2f - val);
		}

		// Token: 0x060062AC RID: 25260 RVA: 0x001F3175 File Offset: 0x001F1375
		private static float SinusoidalOut(float val)
		{
			return Mathf.Sin(val * 3.1415927f * 0.5f);
		}

		// Token: 0x04006111 RID: 24849
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006112 RID: 24850
		[RequiredField]
		public FsmVector3 target;

		// Token: 0x04006113 RID: 24851
		public FsmFloat duration;

		// Token: 0x04006114 RID: 24852
		public FsmFloat delay;

		// Token: 0x04006115 RID: 24853
		public ScaleToCurves curve;

		// Token: 0x04006116 RID: 24854
		private float timer;

		// Token: 0x04006117 RID: 24855
		private Transform transform;

		// Token: 0x04006118 RID: 24856
		private Vector3 startScale;
	}
}
