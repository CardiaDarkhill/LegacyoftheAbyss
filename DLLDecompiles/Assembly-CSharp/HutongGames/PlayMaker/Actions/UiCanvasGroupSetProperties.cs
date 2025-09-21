using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001110 RID: 4368
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets properties of a UI CanvasGroup component.")]
	public class UiCanvasGroupSetProperties : ComponentAction<CanvasGroup>
	{
		// Token: 0x0600760E RID: 30222 RVA: 0x00240B8C File Offset: 0x0023ED8C
		public override void Reset()
		{
			this.gameObject = null;
			this.alpha = new FsmFloat
			{
				UseVariable = true
			};
			this.interactable = new FsmBool
			{
				UseVariable = true
			};
			this.blocksRaycasts = new FsmBool
			{
				UseVariable = true
			};
			this.ignoreParentGroup = new FsmBool
			{
				UseVariable = true
			};
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x0600760F RID: 30223 RVA: 0x00240BF8 File Offset: 0x0023EDF8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.component = this.cachedComponent;
				if (this.component != null)
				{
					this.originalAlpha = this.component.alpha;
					this.originalInteractable = this.component.interactable;
					this.originalBlocksRaycasts = this.component.blocksRaycasts;
					this.originalIgnoreParentGroup = this.component.ignoreParentGroups;
				}
			}
			this.DoAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007610 RID: 30224 RVA: 0x00240C92 File Offset: 0x0023EE92
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007611 RID: 30225 RVA: 0x00240C9C File Offset: 0x0023EE9C
		private void DoAction()
		{
			if (this.component == null)
			{
				return;
			}
			if (!this.alpha.IsNone)
			{
				this.component.alpha = this.alpha.Value;
			}
			if (!this.interactable.IsNone)
			{
				this.component.interactable = this.interactable.Value;
			}
			if (!this.blocksRaycasts.IsNone)
			{
				this.component.blocksRaycasts = this.blocksRaycasts.Value;
			}
			if (!this.ignoreParentGroup.IsNone)
			{
				this.component.ignoreParentGroups = this.ignoreParentGroup.Value;
			}
		}

		// Token: 0x06007612 RID: 30226 RVA: 0x00240D44 File Offset: 0x0023EF44
		public override void OnExit()
		{
			if (this.component == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				if (!this.alpha.IsNone)
				{
					this.component.alpha = this.originalAlpha;
				}
				if (!this.interactable.IsNone)
				{
					this.component.interactable = this.originalInteractable;
				}
				if (!this.blocksRaycasts.IsNone)
				{
					this.component.blocksRaycasts = this.originalBlocksRaycasts;
				}
				if (!this.ignoreParentGroup.IsNone)
				{
					this.component.ignoreParentGroups = this.originalIgnoreParentGroup;
				}
			}
		}

		// Token: 0x04007671 RID: 30321
		[RequiredField]
		[CheckForComponent(typeof(CanvasGroup))]
		[Tooltip("The GameObject with the UI CanvasGroup component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007672 RID: 30322
		[Tooltip("Canvas group alpha. Ranges from 0.0 to 1.0.")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat alpha;

		// Token: 0x04007673 RID: 30323
		[Tooltip("Is the group interactable (are the elements beneath the group enabled). Leave as None for no effect")]
		public FsmBool interactable;

		// Token: 0x04007674 RID: 30324
		[Tooltip("Does this group block raycasting (allow collision). Leave as None for no effect")]
		public FsmBool blocksRaycasts;

		// Token: 0x04007675 RID: 30325
		[Tooltip("Should the group ignore parent groups? Leave as None for no effect")]
		public FsmBool ignoreParentGroup;

		// Token: 0x04007676 RID: 30326
		[Tooltip("Reset when exiting this state. Leave as None for no effect")]
		public FsmBool resetOnExit;

		// Token: 0x04007677 RID: 30327
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007678 RID: 30328
		private CanvasGroup component;

		// Token: 0x04007679 RID: 30329
		private float originalAlpha;

		// Token: 0x0400767A RID: 30330
		private bool originalInteractable;

		// Token: 0x0400767B RID: 30331
		private bool originalBlocksRaycasts;

		// Token: 0x0400767C RID: 30332
		private bool originalIgnoreParentGroup;
	}
}
