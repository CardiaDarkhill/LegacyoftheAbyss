using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE4 RID: 3300
	public class PulseOpacity : FsmStateAction
	{
		// Token: 0x06006220 RID: 25120 RVA: 0x001F05D8 File Offset: 0x001EE7D8
		public override void Reset()
		{
			this.MinOpacity = 0f;
			this.MaxOpacity = 1f;
			this.MinSpeed = 1f;
			this.MaxSpeed = 2f;
			this.IncreaseDuration = 1f;
			this.PulseCurve = new FsmAnimationCurve
			{
				curve = new AnimationCurve
				{
					keys = new Keyframe[]
					{
						new Keyframe(0f, 1f),
						new Keyframe(0.5f, 0f),
						new Keyframe(1f, 1f)
					},
					preWrapMode = WrapMode.Loop,
					postWrapMode = WrapMode.Loop
				}
			};
			this.IncreaseCurve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
		}

		// Token: 0x06006221 RID: 25121 RVA: 0x001F06D8 File Offset: 0x001EE8D8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.sprite = safe.GetComponent<tk2dSprite>();
				if (this.sprite)
				{
					this.elapsedTime = 0f;
					this.UpdateOpacity();
					return;
				}
			}
			base.Finish();
		}

		// Token: 0x06006222 RID: 25122 RVA: 0x001F072C File Offset: 0x001EE92C
		public override void OnUpdate()
		{
			float progress = this.GetProgress();
			float num = Mathf.Lerp(this.MinSpeed.Value, this.MaxSpeed.Value, this.IncreaseCurve.curve.Evaluate(progress));
			this.elapsedTime += Time.deltaTime * num;
			this.UpdateOpacity();
		}

		// Token: 0x06006223 RID: 25123 RVA: 0x001F0788 File Offset: 0x001EE988
		private void UpdateOpacity()
		{
			float t = this.PulseCurve.curve.Evaluate(this.elapsedTime);
			float a = Mathf.Lerp(this.MinOpacity.Value, this.MaxOpacity.Value, t);
			Color color = this.sprite.color;
			color.a = a;
			this.sprite.color = color;
		}

		// Token: 0x06006224 RID: 25124 RVA: 0x001F07E9 File Offset: 0x001EE9E9
		public override float GetProgress()
		{
			if (this.IncreaseDuration.Value <= 0f)
			{
				return 1f;
			}
			return base.State.StateTime / this.IncreaseDuration.Value;
		}

		// Token: 0x04006039 RID: 24633
		public FsmOwnerDefault Target;

		// Token: 0x0400603A RID: 24634
		public FsmFloat MinOpacity;

		// Token: 0x0400603B RID: 24635
		public FsmFloat MaxOpacity;

		// Token: 0x0400603C RID: 24636
		public FsmFloat MinSpeed;

		// Token: 0x0400603D RID: 24637
		public FsmFloat MaxSpeed;

		// Token: 0x0400603E RID: 24638
		public FsmAnimationCurve PulseCurve;

		// Token: 0x0400603F RID: 24639
		public FsmFloat IncreaseDuration;

		// Token: 0x04006040 RID: 24640
		public FsmAnimationCurve IncreaseCurve;

		// Token: 0x04006041 RID: 24641
		private float elapsedTime;

		// Token: 0x04006042 RID: 24642
		private tk2dSprite sprite;
	}
}
