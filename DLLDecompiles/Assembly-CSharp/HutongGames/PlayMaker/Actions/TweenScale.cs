using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001108 RID: 4360
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a GameObject's scale.")]
	public class TweenScale : TweenComponentBase<Transform>
	{
		// Token: 0x17000C0F RID: 3087
		// (get) Token: 0x060075DE RID: 30174 RVA: 0x002403EF File Offset: 0x0023E5EF
		// (set) Token: 0x060075DF RID: 30175 RVA: 0x002403F7 File Offset: 0x0023E5F7
		public Vector3 StartScale { get; private set; }

		// Token: 0x17000C10 RID: 3088
		// (get) Token: 0x060075E0 RID: 30176 RVA: 0x00240400 File Offset: 0x0023E600
		// (set) Token: 0x060075E1 RID: 30177 RVA: 0x00240408 File Offset: 0x0023E608
		public Vector3 EndScale { get; private set; }

		// Token: 0x060075E2 RID: 30178 RVA: 0x00240414 File Offset: 0x0023E614
		public override void Reset()
		{
			base.Reset();
			this.fromOptions = ScaleOptions.CurrentScale;
			this.fromTarget = null;
			this.fromScale = new FsmVector3
			{
				Value = Vector3.one
			};
			this.toOptions = ScaleOptions.LocalScale;
			this.toTarget = null;
			this.toScale = new FsmVector3
			{
				Value = Vector3.one
			};
		}

		// Token: 0x060075E3 RID: 30179 RVA: 0x00240470 File Offset: 0x0023E670
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.Finished)
			{
				return;
			}
			this.transform = this.cachedComponent;
			this.fromTransform = ((this.fromTarget.Value != null) ? this.fromTarget.Value.transform : null);
			this.toTransform = ((this.toTarget.Value != null) ? this.toTarget.Value.transform : null);
			this.InitStartScale();
			this.InitEndScale();
			this.DoTween();
		}

		// Token: 0x060075E4 RID: 30180 RVA: 0x00240502 File Offset: 0x0023E702
		private void InitStartScale()
		{
			this.StartScale = TweenHelpers.GetTargetScale(this.fromOptions, this.transform, this.fromTransform, this.fromScale.IsNone ? Vector3.one : this.fromScale.Value);
		}

		// Token: 0x060075E5 RID: 30181 RVA: 0x00240540 File Offset: 0x0023E740
		private void InitEndScale()
		{
			this.EndScale = TweenHelpers.GetTargetScale(this.toOptions, this.transform, this.toTransform, this.toScale.IsNone ? Vector3.one : this.toScale.Value);
		}

		// Token: 0x060075E6 RID: 30182 RVA: 0x0024057E File Offset: 0x0023E77E
		private void UpdateStartScale()
		{
			if (this.fromOptions == ScaleOptions.LocalScale || this.fromOptions == ScaleOptions.MatchGameObject)
			{
				this.InitStartScale();
			}
		}

		// Token: 0x060075E7 RID: 30183 RVA: 0x00240598 File Offset: 0x0023E798
		private void UpdateEndScale()
		{
			if (this.fromOptions == ScaleOptions.LocalScale || this.fromOptions == ScaleOptions.MatchGameObject)
			{
				this.InitEndScale();
			}
		}

		// Token: 0x060075E8 RID: 30184 RVA: 0x002405B2 File Offset: 0x0023E7B2
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.fromOptions == ScaleOptions.MatchGameObject)
			{
				this.InitStartScale();
			}
			if (this.fromOptions == ScaleOptions.MatchGameObject)
			{
				this.InitEndScale();
			}
		}

		// Token: 0x060075E9 RID: 30185 RVA: 0x002405D8 File Offset: 0x0023E7D8
		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, this.normalizedTime);
			this.transform.localScale = Vector3.Lerp(this.StartScale, this.EndScale, t);
		}

		// Token: 0x0400764A RID: 30282
		[ActionSection("From")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public ScaleOptions fromOptions;

		// Token: 0x0400764B RID: 30283
		[Tooltip("Tween from this Target GameObject.")]
		public FsmGameObject fromTarget;

		// Token: 0x0400764C RID: 30284
		[Tooltip("Tween from this Scale.")]
		public FsmVector3 fromScale;

		// Token: 0x0400764D RID: 30285
		[ActionSection("To")]
		[Title("Options")]
		[Tooltip("Setup where to tween to.")]
		public ScaleOptions toOptions;

		// Token: 0x0400764E RID: 30286
		[Tooltip("Tween to this Target GameObject.")]
		public FsmGameObject toTarget;

		// Token: 0x0400764F RID: 30287
		[Tooltip("Tween to this Scale.")]
		public FsmVector3 toScale;

		// Token: 0x04007650 RID: 30288
		private Transform transform;

		// Token: 0x04007651 RID: 30289
		private Transform fromTransform;

		// Token: 0x04007652 RID: 30290
		private Transform toTransform;
	}
}
