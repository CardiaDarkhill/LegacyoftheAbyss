using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001104 RID: 4356
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Punches a GameObject's position, rotation, or scale\u00a0and springs back to starting state")]
	public class TweenPunch : TweenComponentBase<Transform>
	{
		// Token: 0x060075C7 RID: 30151 RVA: 0x0023FDB2 File Offset: 0x0023DFB2
		public override void Reset()
		{
			base.Reset();
			this.punchType = TweenPunch.PunchType.Position;
			this.value = null;
		}

		// Token: 0x060075C8 RID: 30152 RVA: 0x0023FDC8 File Offset: 0x0023DFC8
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.Finished)
			{
				return;
			}
			this.easeType.Value = EasingFunction.Ease.Punch;
			this.transform = this.cachedComponent;
			this.rectTransform = (this.transform as RectTransform);
			switch (this.punchType)
			{
			case TweenPunch.PunchType.Position:
				this.startVector3 = ((this.rectTransform != null) ? this.rectTransform.anchoredPosition3D : this.transform.position);
				this.endVector3 = this.startVector3 + this.value.Value;
				return;
			case TweenPunch.PunchType.Rotation:
				this.startRotation = this.transform.rotation;
				this.midRotation = this.startRotation * Quaternion.Euler(this.value.Value * 0.5f);
				this.endRotation = this.startRotation * Quaternion.Euler(this.value.Value);
				return;
			case TweenPunch.PunchType.Scale:
				this.startVector3 = this.transform.localScale;
				this.endVector3 = this.startVector3 + this.value.Value;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060075C9 RID: 30153 RVA: 0x0023FF0C File Offset: 0x0023E10C
		protected override void DoTween()
		{
			float num = base.easingFunction(0f, 1f, this.normalizedTime);
			switch (this.punchType)
			{
			case TweenPunch.PunchType.Position:
				if (this.rectTransform != null)
				{
					this.rectTransform.anchoredPosition = Vector3.Lerp(this.startVector3, this.endVector3, base.easingFunction(0f, 1f, this.normalizedTime));
					return;
				}
				this.transform.position = Vector3.Lerp(this.startVector3, this.endVector3, base.easingFunction(0f, 1f, this.normalizedTime));
				return;
			case TweenPunch.PunchType.Rotation:
				this.transform.rotation = (((double)num < 0.5) ? Quaternion.Slerp(this.startRotation, this.midRotation, num * 2f) : Quaternion.Slerp(this.midRotation, this.endRotation, (num - 0.5f) * 2f));
				return;
			case TweenPunch.PunchType.Scale:
				this.transform.localScale = Vector3.Lerp(this.startVector3, this.endVector3, num);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x04007634 RID: 30260
		[Tooltip("Punch position, rotation, or scale.")]
		public TweenPunch.PunchType punchType;

		// Token: 0x04007635 RID: 30261
		[Tooltip("Punch magnitude.")]
		public FsmVector3 value;

		// Token: 0x04007636 RID: 30262
		private Transform transform;

		// Token: 0x04007637 RID: 30263
		private RectTransform rectTransform;

		// Token: 0x04007638 RID: 30264
		private Vector3 startVector3;

		// Token: 0x04007639 RID: 30265
		private Vector3 endVector3;

		// Token: 0x0400763A RID: 30266
		private Quaternion startRotation;

		// Token: 0x0400763B RID: 30267
		private Quaternion midRotation;

		// Token: 0x0400763C RID: 30268
		private Quaternion endRotation;

		// Token: 0x02001BD1 RID: 7121
		public enum PunchType
		{
			// Token: 0x04009EE8 RID: 40680
			Position,
			// Token: 0x04009EE9 RID: 40681
			Rotation,
			// Token: 0x04009EEA RID: 40682
			Scale
		}
	}
}
