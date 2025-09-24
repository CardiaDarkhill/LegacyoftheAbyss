using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200110D RID: 4365
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Enable or disable Canvas Raycasting. Optionally reset on state exit")]
	public class UiCanvasEnableRaycast : ComponentAction<PlayMakerCanvasRaycastFilterProxy>
	{
		// Token: 0x060075FF RID: 30207 RVA: 0x0024096C File Offset: 0x0023EB6C
		public override void Reset()
		{
			this.gameObject = null;
			this.enableRaycasting = false;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x06007600 RID: 30208 RVA: 0x00240990 File Offset: 0x0023EB90
		public override void OnPreprocess()
		{
			if (this.gameObject == null)
			{
				this.gameObject = new FsmOwnerDefault();
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCacheAddComponent(ownerDefaultTarget))
			{
				this.raycastFilterProxy = this.cachedComponent;
			}
		}

		// Token: 0x06007601 RID: 30209 RVA: 0x002409D8 File Offset: 0x0023EBD8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCacheAddComponent(ownerDefaultTarget))
			{
				this.raycastFilterProxy = this.cachedComponent;
				this.originalValue = this.raycastFilterProxy.RayCastingEnabled;
			}
			this.DoAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007602 RID: 30210 RVA: 0x00240A31 File Offset: 0x0023EC31
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007603 RID: 30211 RVA: 0x00240A39 File Offset: 0x0023EC39
		private void DoAction()
		{
			if (this.raycastFilterProxy != null)
			{
				this.raycastFilterProxy.RayCastingEnabled = this.enableRaycasting.Value;
			}
		}

		// Token: 0x06007604 RID: 30212 RVA: 0x00240A5F File Offset: 0x0023EC5F
		public override void OnExit()
		{
			if (this.raycastFilterProxy == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.raycastFilterProxy.RayCastingEnabled = this.originalValue;
			}
		}

		// Token: 0x04007665 RID: 30309
		[RequiredField]
		[Tooltip("The GameObject to enable or disable Canvas Raycasting on.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007666 RID: 30310
		[Tooltip("Enable/disable raycasting. Uncheck to disable.")]
		public FsmBool enableRaycasting;

		// Token: 0x04007667 RID: 30311
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007668 RID: 30312
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007669 RID: 30313
		[SerializeField]
		private PlayMakerCanvasRaycastFilterProxy raycastFilterProxy;

		// Token: 0x0400766A RID: 30314
		private bool originalValue;
	}
}
