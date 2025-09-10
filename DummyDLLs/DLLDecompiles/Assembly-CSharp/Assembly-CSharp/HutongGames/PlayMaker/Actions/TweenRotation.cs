using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001107 RID: 4359
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a GameObject's rotation.")]
	public class TweenRotation : TweenComponentBase<Transform>
	{
		// Token: 0x17000C0D RID: 3085
		// (get) Token: 0x060075D1 RID: 30161 RVA: 0x002401B3 File Offset: 0x0023E3B3
		// (set) Token: 0x060075D2 RID: 30162 RVA: 0x002401BB File Offset: 0x0023E3BB
		public Quaternion StartRotation { get; private set; }

		// Token: 0x17000C0E RID: 3086
		// (get) Token: 0x060075D3 RID: 30163 RVA: 0x002401C4 File Offset: 0x0023E3C4
		// (set) Token: 0x060075D4 RID: 30164 RVA: 0x002401CC File Offset: 0x0023E3CC
		public Quaternion EndRotation { get; private set; }

		// Token: 0x060075D5 RID: 30165 RVA: 0x002401D5 File Offset: 0x0023E3D5
		public override void Reset()
		{
			base.Reset();
			this.fromOptions = RotationOptions.CurrentRotation;
			this.fromTarget = null;
			this.fromRotation = null;
			this.toOptions = RotationOptions.WorldRotation;
			this.toTarget = null;
			this.toRotation = null;
		}

		// Token: 0x060075D6 RID: 30166 RVA: 0x00240208 File Offset: 0x0023E408
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
			this.InitStartRotation();
			this.InitEndRotation();
			this.DoTween();
		}

		// Token: 0x060075D7 RID: 30167 RVA: 0x0024029A File Offset: 0x0023E49A
		private void InitStartRotation()
		{
			this.StartRotation = TweenHelpers.GetTargetRotation(this.fromOptions, this.transform, this.fromTransform, this.fromRotation.Value);
		}

		// Token: 0x060075D8 RID: 30168 RVA: 0x002402C4 File Offset: 0x0023E4C4
		private void InitEndRotation()
		{
			this.EndRotation = TweenHelpers.GetTargetRotation(this.toOptions, this.transform, this.toTransform, this.toRotation.Value);
			this.midRotation = TweenHelpers.GetTargetRotation(this.toOptions, this.transform, this.toTransform, this.toRotation.Value / 2f);
		}

		// Token: 0x060075D9 RID: 30169 RVA: 0x0024032B File Offset: 0x0023E52B
		private void UpdateStartRotation()
		{
			if (this.fromOptions == RotationOptions.MatchGameObjectRotation)
			{
				this.InitStartRotation();
			}
		}

		// Token: 0x060075DA RID: 30170 RVA: 0x0024033C File Offset: 0x0023E53C
		private void UpdateEndRotation()
		{
			if (this.toOptions == RotationOptions.MatchGameObjectRotation)
			{
				this.InitEndRotation();
			}
		}

		// Token: 0x060075DB RID: 30171 RVA: 0x0024034D File Offset: 0x0023E54D
		public override void OnUpdate()
		{
			base.OnUpdate();
			this.UpdateStartRotation();
			this.UpdateEndRotation();
		}

		// Token: 0x060075DC RID: 30172 RVA: 0x00240364 File Offset: 0x0023E564
		protected override void DoTween()
		{
			float num = base.easingFunction(0f, 1f, this.normalizedTime);
			if ((double)num < 0.5)
			{
				this.transform.rotation = Quaternion.Slerp(this.StartRotation, this.midRotation, num * 2f);
				return;
			}
			this.transform.rotation = Quaternion.Slerp(this.midRotation, this.EndRotation, (num - 0.5f) * 2f);
		}

		// Token: 0x0400763E RID: 30270
		[ActionSection("From")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public RotationOptions fromOptions;

		// Token: 0x0400763F RID: 30271
		[Tooltip("Use this GameObject's rotation.")]
		public FsmGameObject fromTarget;

		// Token: 0x04007640 RID: 30272
		[Tooltip("Tween from this rotation")]
		public FsmVector3 fromRotation;

		// Token: 0x04007641 RID: 30273
		[ActionSection("To")]
		[Title("Options")]
		[Tooltip("Setup where to tween to.")]
		public RotationOptions toOptions;

		// Token: 0x04007642 RID: 30274
		[Tooltip("Use this GameObject's rotation")]
		public FsmGameObject toTarget;

		// Token: 0x04007643 RID: 30275
		[Tooltip("Tween to this rotation.")]
		public FsmVector3 toRotation;

		// Token: 0x04007644 RID: 30276
		private Transform transform;

		// Token: 0x04007645 RID: 30277
		private Transform fromTransform;

		// Token: 0x04007646 RID: 30278
		private Transform toTransform;

		// Token: 0x04007649 RID: 30281
		private Quaternion midRotation;
	}
}
