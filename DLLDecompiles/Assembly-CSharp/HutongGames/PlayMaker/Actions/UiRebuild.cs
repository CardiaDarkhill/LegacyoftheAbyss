using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001162 RID: 4450
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Rebuild a UI Graphic component.")]
	public class UiRebuild : ComponentAction<Graphic>
	{
		// Token: 0x0600778A RID: 30602 RVA: 0x0024580E File Offset: 0x00243A0E
		public override void Reset()
		{
			this.gameObject = null;
			this.canvasUpdate = CanvasUpdate.LatePreRender;
			this.rebuildOnExit = false;
		}

		// Token: 0x0600778B RID: 30603 RVA: 0x00245828 File Offset: 0x00243A28
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.graphic = this.cachedComponent;
			}
			if (!this.rebuildOnExit)
			{
				this.DoAction();
			}
			base.Finish();
		}

		// Token: 0x0600778C RID: 30604 RVA: 0x00245870 File Offset: 0x00243A70
		private void DoAction()
		{
			if (this.graphic != null)
			{
				this.graphic.Rebuild(this.canvasUpdate);
			}
		}

		// Token: 0x0600778D RID: 30605 RVA: 0x00245891 File Offset: 0x00243A91
		public override void OnExit()
		{
			if (this.rebuildOnExit)
			{
				this.DoAction();
			}
		}

		// Token: 0x04007809 RID: 30729
		[RequiredField]
		[CheckForComponent(typeof(Graphic))]
		[Tooltip("The GameObject with the UI Graphic component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400780A RID: 30730
		[Tooltip("When to Update.")]
		public CanvasUpdate canvasUpdate;

		// Token: 0x0400780B RID: 30731
		[Tooltip("Only Rebuild when state exits.")]
		public bool rebuildOnExit;

		// Token: 0x0400780C RID: 30732
		private Graphic graphic;
	}
}
