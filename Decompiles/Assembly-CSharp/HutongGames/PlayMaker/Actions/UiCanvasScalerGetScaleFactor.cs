using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001111 RID: 4369
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Get the ScaleFactor of a CanvasScaler.")]
	public class UiCanvasScalerGetScaleFactor : ComponentAction<CanvasScaler>
	{
		// Token: 0x06007614 RID: 30228 RVA: 0x00240DED File Offset: 0x0023EFED
		public override void Reset()
		{
			this.gameObject = null;
			this.scaleFactor = null;
			this.everyFrame = false;
		}

		// Token: 0x06007615 RID: 30229 RVA: 0x00240E04 File Offset: 0x0023F004
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.component = this.cachedComponent;
			}
			this.DoGetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007616 RID: 30230 RVA: 0x00240E4C File Offset: 0x0023F04C
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x06007617 RID: 30231 RVA: 0x00240E54 File Offset: 0x0023F054
		private void DoGetValue()
		{
			if (this.component != null)
			{
				this.scaleFactor.Value = this.component.scaleFactor;
			}
		}

		// Token: 0x0400767D RID: 30333
		[RequiredField]
		[CheckForComponent(typeof(CanvasScaler))]
		[Tooltip("The GameObject with a UI CanvasScaler component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400767E RID: 30334
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The scaleFactor of the CanvasScaler component.")]
		public FsmFloat scaleFactor;

		// Token: 0x0400767F RID: 30335
		[Tooltip("Repeats every frame, useful for animation")]
		public bool everyFrame;

		// Token: 0x04007680 RID: 30336
		private CanvasScaler component;
	}
}
