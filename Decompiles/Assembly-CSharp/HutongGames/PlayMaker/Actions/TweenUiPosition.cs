using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001109 RID: 4361
	[ActionCategory(ActionCategory.Tween)]
	[ActionTarget(typeof(Camera), "", false)]
	[Tooltip("Tween position of UI GameObjects.")]
	public class TweenUiPosition : TweenComponentBase<RectTransform>
	{
		// Token: 0x17000C11 RID: 3089
		// (get) Token: 0x060075EB RID: 30187 RVA: 0x00240626 File Offset: 0x0023E826
		// (set) Token: 0x060075EC RID: 30188 RVA: 0x0024062E File Offset: 0x0023E82E
		public Vector3 StartPosition { get; private set; }

		// Token: 0x17000C12 RID: 3090
		// (get) Token: 0x060075ED RID: 30189 RVA: 0x00240637 File Offset: 0x0023E837
		// (set) Token: 0x060075EE RID: 30190 RVA: 0x0024063F File Offset: 0x0023E83F
		public Vector3 EndPosition { get; private set; }

		// Token: 0x060075EF RID: 30191 RVA: 0x00240648 File Offset: 0x0023E848
		public override void Reset()
		{
			base.Reset();
			this.fromOption = UiPositionOptions.CurrentPosition;
			this.fromTarget = null;
			this.fromPosition = null;
			this.toOption = UiPositionOptions.Position;
			this.toTarget = null;
			this.toPosition = null;
		}

		// Token: 0x060075F0 RID: 30192 RVA: 0x0024067C File Offset: 0x0023E87C
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
			this.transform.anchoredPosition3D = this.StartPosition;
		}

		// Token: 0x060075F1 RID: 30193 RVA: 0x00240719 File Offset: 0x0023E919
		private void InitStartPosition()
		{
			this.StartPosition = TweenHelpers.GetUiTargetPosition(this.fromOption, this.transform, this.fromTransform, this.fromPosition.Value);
		}

		// Token: 0x060075F2 RID: 30194 RVA: 0x00240743 File Offset: 0x0023E943
		private void InitEndPosition()
		{
			this.EndPosition = TweenHelpers.GetUiTargetPosition(this.toOption, this.transform, this.toTransform, this.toPosition.Value);
		}

		// Token: 0x060075F3 RID: 30195 RVA: 0x0024076D File Offset: 0x0023E96D
		protected override void DoTween()
		{
			this.transform.anchoredPosition3D = Vector3.Lerp(this.StartPosition, this.EndPosition, base.easingFunction(0f, 1f, this.normalizedTime));
		}

		// Token: 0x04007655 RID: 30293
		[ActionSection("From")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public UiPositionOptions fromOption;

		// Token: 0x04007656 RID: 30294
		[Tooltip("Optionally use a GameObject as the from position.")]
		public FsmGameObject fromTarget;

		// Token: 0x04007657 RID: 30295
		[Tooltip("If a GameObject is specified, use this as an offset. Otherwise this is a world position.")]
		public FsmVector3 fromPosition;

		// Token: 0x04007658 RID: 30296
		[ActionSection("To")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public UiPositionOptions toOption;

		// Token: 0x04007659 RID: 30297
		[Tooltip("Optionally use a GameObject as the to position.")]
		public FsmGameObject toTarget;

		// Token: 0x0400765A RID: 30298
		[Tooltip("If a GameObject is specified, use this as an offset. Otherwise this is a world position.")]
		public FsmVector3 toPosition;

		// Token: 0x0400765B RID: 30299
		[NonSerialized]
		private RectTransform transform;

		// Token: 0x0400765C RID: 30300
		[NonSerialized]
		private Transform fromTransform;

		// Token: 0x0400765D RID: 30301
		[NonSerialized]
		private Transform toTransform;
	}
}
