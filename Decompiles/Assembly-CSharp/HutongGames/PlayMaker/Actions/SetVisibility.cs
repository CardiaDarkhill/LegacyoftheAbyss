using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F6F RID: 3951
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the visibility of a GameObject. Note: this action sets the GameObject Renderer's enabled state.")]
	public class SetVisibility : ComponentAction<Renderer>
	{
		// Token: 0x06006D8E RID: 28046 RVA: 0x00220DDF File Offset: 0x0021EFDF
		public override void Reset()
		{
			this.gameObject = null;
			this.toggle = false;
			this.visible = false;
			this.resetOnExit = true;
			this.initialVisibility = false;
		}

		// Token: 0x06006D8F RID: 28047 RVA: 0x00220E0E File Offset: 0x0021F00E
		public override void OnEnter()
		{
			this.DoSetVisibility(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x06006D90 RID: 28048 RVA: 0x00220E30 File Offset: 0x0021F030
		private void DoSetVisibility(GameObject go)
		{
			if (!base.UpdateCache(go))
			{
				return;
			}
			this.initialVisibility = base.renderer.enabled;
			if (!this.toggle.Value)
			{
				base.renderer.enabled = this.visible.Value;
				return;
			}
			base.renderer.enabled = !base.renderer.enabled;
		}

		// Token: 0x06006D91 RID: 28049 RVA: 0x00220E95 File Offset: 0x0021F095
		public override void OnExit()
		{
			if (this.resetOnExit)
			{
				this.ResetVisibility();
			}
		}

		// Token: 0x06006D92 RID: 28050 RVA: 0x00220EA5 File Offset: 0x0021F0A5
		private void ResetVisibility()
		{
			if (base.renderer != null)
			{
				base.renderer.enabled = this.initialVisibility;
			}
		}

		// Token: 0x04006D52 RID: 27986
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject to effect. Note: Needs a Renderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006D53 RID: 27987
		[Tooltip("Should the object visibility be toggled?\nHas priority over the 'visible' setting")]
		public FsmBool toggle;

		// Token: 0x04006D54 RID: 27988
		[Tooltip("Should the object be set to visible or invisible?")]
		public FsmBool visible;

		// Token: 0x04006D55 RID: 27989
		[Tooltip("Resets to the initial visibility when it leaves the state")]
		public bool resetOnExit;

		// Token: 0x04006D56 RID: 27990
		private bool initialVisibility;
	}
}
