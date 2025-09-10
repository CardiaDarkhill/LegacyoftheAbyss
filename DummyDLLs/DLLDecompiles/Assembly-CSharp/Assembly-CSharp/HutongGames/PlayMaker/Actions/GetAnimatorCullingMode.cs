using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE6 RID: 3558
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns the culling of this Animator component. Optionally sends events.\nIf true ('AlwaysAnimate'): always animate the entire character. Object is animated even when offscreen.\nIf False ('BasedOnRenderers') animation is disabled when renderers are not visible.")]
	public class GetAnimatorCullingMode : ComponentAction<Animator>
	{
		// Token: 0x060066D2 RID: 26322 RVA: 0x00208800 File Offset: 0x00206A00
		public override void Reset()
		{
			this.gameObject = null;
			this.alwaysAnimate = null;
			this.alwaysAnimateEvent = null;
			this.basedOnRenderersEvent = null;
		}

		// Token: 0x060066D3 RID: 26323 RVA: 0x00208820 File Offset: 0x00206A20
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				bool flag = this.cachedComponent.cullingMode == AnimatorCullingMode.AlwaysAnimate;
				this.alwaysAnimate.Value = flag;
				base.Fsm.Event(flag ? this.alwaysAnimateEvent : this.basedOnRenderersEvent);
			}
			base.Finish();
		}

		// Token: 0x04006624 RID: 26148
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006625 RID: 26149
		[ActionSection("Results")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("If true, always animate the entire character, else animation is disabled when renderers are not visible")]
		public FsmBool alwaysAnimate;

		// Token: 0x04006626 RID: 26150
		[Tooltip("Event send if culling mode is 'AlwaysAnimate'")]
		public FsmEvent alwaysAnimateEvent;

		// Token: 0x04006627 RID: 26151
		[Tooltip("Event send if culling mode is 'BasedOnRenders'")]
		public FsmEvent basedOnRenderersEvent;
	}
}
