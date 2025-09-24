using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200110F RID: 4367
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Set Group Alpha.")]
	public class UiCanvasGroupSetAlpha : ComponentAction<CanvasGroup>
	{
		// Token: 0x06007608 RID: 30216 RVA: 0x00240AAB File Offset: 0x0023ECAB
		public override void Reset()
		{
			this.gameObject = null;
			this.alpha = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x06007609 RID: 30217 RVA: 0x00240ACC File Offset: 0x0023ECCC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.component = this.cachedComponent;
			}
			this.originalValue = this.component.alpha;
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600760A RID: 30218 RVA: 0x00240B25 File Offset: 0x0023ED25
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x0600760B RID: 30219 RVA: 0x00240B2D File Offset: 0x0023ED2D
		private void DoSetValue()
		{
			if (this.component != null)
			{
				this.component.alpha = this.alpha.Value;
			}
		}

		// Token: 0x0600760C RID: 30220 RVA: 0x00240B53 File Offset: 0x0023ED53
		public override void OnExit()
		{
			if (this.component == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.component.alpha = this.originalValue;
			}
		}

		// Token: 0x0400766B RID: 30315
		[RequiredField]
		[CheckForComponent(typeof(CanvasGroup))]
		[Tooltip("The GameObject with a UI CanvasGroup component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400766C RID: 30316
		[RequiredField]
		[Tooltip("The alpha of the UI component.")]
		public FsmFloat alpha;

		// Token: 0x0400766D RID: 30317
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x0400766E RID: 30318
		[Tooltip("Repeats every frame, useful for animation")]
		public bool everyFrame;

		// Token: 0x0400766F RID: 30319
		private CanvasGroup component;

		// Token: 0x04007670 RID: 30320
		private float originalValue;
	}
}
