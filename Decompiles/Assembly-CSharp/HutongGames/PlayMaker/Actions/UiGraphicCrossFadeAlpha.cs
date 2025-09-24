using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200113F RID: 4415
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Tweens the alpha of the CanvasRenderer color associated with this Graphic.")]
	public class UiGraphicCrossFadeAlpha : ComponentAction<Graphic>
	{
		// Token: 0x060076E0 RID: 30432 RVA: 0x00243C23 File Offset: 0x00241E23
		public override void Reset()
		{
			this.gameObject = null;
			this.alpha = null;
			this.duration = 1f;
			this.ignoreTimeScale = null;
		}

		// Token: 0x060076E1 RID: 30433 RVA: 0x00243C4C File Offset: 0x00241E4C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.uiComponent = this.cachedComponent;
			}
			this.uiComponent.CrossFadeAlpha(this.alpha.Value, this.duration.Value, this.ignoreTimeScale.Value);
			base.Finish();
		}

		// Token: 0x04007752 RID: 30546
		[RequiredField]
		[CheckForComponent(typeof(Graphic))]
		[Tooltip("The GameObject with an Unity UI component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007753 RID: 30547
		[Tooltip("The alpha target")]
		public FsmFloat alpha;

		// Token: 0x04007754 RID: 30548
		[Tooltip("The duration of the tween")]
		public FsmFloat duration;

		// Token: 0x04007755 RID: 30549
		[Tooltip("Should ignore Time.scale?")]
		public FsmBool ignoreTimeScale;

		// Token: 0x04007756 RID: 30550
		private Graphic uiComponent;
	}
}
