using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200110A RID: 4362
	[ActionCategory(ActionCategory.Tween)]
	[ActionTarget(typeof(RectTransform), "", false)]
	[Tooltip("Tween the Width and Height of a UI object. NOTE: The size is also influenced by anchors!")]
	public class TweenUiSize : TweenComponentBase<RectTransform>
	{
		// Token: 0x060075F5 RID: 30197 RVA: 0x002407AE File Offset: 0x0023E9AE
		public override void Reset()
		{
			base.Reset();
			this.tweenDirection = TweenDirection.To;
			this.targetSize = null;
		}

		// Token: 0x060075F6 RID: 30198 RVA: 0x002407C4 File Offset: 0x0023E9C4
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.Finished)
			{
				return;
			}
			this.rectTransform = this.cachedComponent;
			if (this.tweenDirection == TweenDirection.From)
			{
				this.fromSize = this.targetSize.Value;
				this.toSize = this.rectTransform.sizeDelta;
				return;
			}
			this.fromSize = this.rectTransform.sizeDelta;
			this.toSize = this.targetSize.Value;
		}

		// Token: 0x060075F7 RID: 30199 RVA: 0x0024083C File Offset: 0x0023EA3C
		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, this.normalizedTime);
			this.rectTransform.sizeDelta = Vector2.Lerp(this.fromSize, this.toSize, t);
		}

		// Token: 0x04007660 RID: 30304
		[Tooltip("Tween To/From Target Size.")]
		public TweenDirection tweenDirection;

		// Token: 0x04007661 RID: 30305
		[Tooltip("Target Size. NOTE: The size is also influenced by anchors!")]
		public FsmVector2 targetSize;

		// Token: 0x04007662 RID: 30306
		private RectTransform rectTransform;

		// Token: 0x04007663 RID: 30307
		private Vector2 fromSize;

		// Token: 0x04007664 RID: 30308
		private Vector2 toSize;
	}
}
