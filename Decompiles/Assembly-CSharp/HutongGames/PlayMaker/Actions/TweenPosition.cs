using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001103 RID: 4355
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a GameObject's position. Note: This action assumes that GameObject targets do not change during the tween.")]
	public class TweenPosition : TweenComponentBase<Transform>
	{
		// Token: 0x17000C0B RID: 3083
		// (get) Token: 0x060075BD RID: 30141 RVA: 0x0023FC2C File Offset: 0x0023DE2C
		// (set) Token: 0x060075BE RID: 30142 RVA: 0x0023FC34 File Offset: 0x0023DE34
		public Vector3 StartPosition { get; private set; }

		// Token: 0x17000C0C RID: 3084
		// (get) Token: 0x060075BF RID: 30143 RVA: 0x0023FC3D File Offset: 0x0023DE3D
		// (set) Token: 0x060075C0 RID: 30144 RVA: 0x0023FC45 File Offset: 0x0023DE45
		public Vector3 EndPosition { get; private set; }

		// Token: 0x060075C1 RID: 30145 RVA: 0x0023FC4E File Offset: 0x0023DE4E
		public override void Reset()
		{
			base.Reset();
			this.fromOption = PositionOptions.CurrentPosition;
			this.fromTarget = null;
			this.fromPosition = null;
			this.toOption = PositionOptions.WorldPosition;
			this.toTarget = null;
			this.toPosition = null;
		}

		// Token: 0x060075C2 RID: 30146 RVA: 0x0023FC80 File Offset: 0x0023DE80
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
			this.InitStartPosition();
			this.InitEndPosition();
			this.transform.position = this.StartPosition;
		}

		// Token: 0x060075C3 RID: 30147 RVA: 0x0023FD1D File Offset: 0x0023DF1D
		private void InitStartPosition()
		{
			this.StartPosition = TweenHelpers.GetTargetPosition(this.fromOption, this.transform, this.fromTransform, this.fromPosition.Value);
		}

		// Token: 0x060075C4 RID: 30148 RVA: 0x0023FD47 File Offset: 0x0023DF47
		private void InitEndPosition()
		{
			this.EndPosition = TweenHelpers.GetTargetPosition(this.toOption, this.transform, this.toTransform, this.toPosition.Value);
		}

		// Token: 0x060075C5 RID: 30149 RVA: 0x0023FD71 File Offset: 0x0023DF71
		protected override void DoTween()
		{
			this.transform.position = Vector3.Lerp(this.StartPosition, this.EndPosition, base.easingFunction(0f, 1f, this.normalizedTime));
		}

		// Token: 0x04007629 RID: 30249
		[ActionSection("From")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public PositionOptions fromOption;

		// Token: 0x0400762A RID: 30250
		[Tooltip("Optionally use a GameObject as the from position.")]
		public FsmGameObject fromTarget;

		// Token: 0x0400762B RID: 30251
		[Tooltip("Position to tween from.")]
		public FsmVector3 fromPosition;

		// Token: 0x0400762C RID: 30252
		[ActionSection("To")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public PositionOptions toOption;

		// Token: 0x0400762D RID: 30253
		[Tooltip("Optionally use a GameObject as the to position.")]
		public FsmGameObject toTarget;

		// Token: 0x0400762E RID: 30254
		[Tooltip("Position to tween to.")]
		public FsmVector3 toPosition;

		// Token: 0x0400762F RID: 30255
		[NonSerialized]
		private Transform transform;

		// Token: 0x04007630 RID: 30256
		[NonSerialized]
		private Transform fromTransform;

		// Token: 0x04007631 RID: 30257
		[NonSerialized]
		private Transform toTransform;
	}
}
