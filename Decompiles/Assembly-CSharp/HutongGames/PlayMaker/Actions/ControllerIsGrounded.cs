using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E55 RID: 3669
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Tests if a Character Controller on a Game Object was touching the ground during the last move.")]
	public class ControllerIsGrounded : ComponentAction<CharacterController>
	{
		// Token: 0x17000BE8 RID: 3048
		// (get) Token: 0x060068D7 RID: 26839 RVA: 0x0020EB8B File Offset: 0x0020CD8B
		private CharacterController controller
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060068D8 RID: 26840 RVA: 0x0020EB93 File Offset: 0x0020CD93
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060068D9 RID: 26841 RVA: 0x0020EBB8 File Offset: 0x0020CDB8
		public override void OnEnter()
		{
			this.DoControllerIsGrounded();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068DA RID: 26842 RVA: 0x0020EBCE File Offset: 0x0020CDCE
		public override void OnUpdate()
		{
			this.DoControllerIsGrounded();
		}

		// Token: 0x060068DB RID: 26843 RVA: 0x0020EBD8 File Offset: 0x0020CDD8
		private void DoControllerIsGrounded()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			bool isGrounded = this.controller.isGrounded;
			this.storeResult.Value = isGrounded;
			base.Fsm.Event(isGrounded ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x04006812 RID: 26642
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject to check.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006813 RID: 26643
		[Tooltip("Event to send if touching the ground.")]
		public FsmEvent trueEvent;

		// Token: 0x04006814 RID: 26644
		[Tooltip("Event to send if not touching the ground.")]
		public FsmEvent falseEvent;

		// Token: 0x04006815 RID: 26645
		[Tooltip("Store the result in a bool variable.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		// Token: 0x04006816 RID: 26646
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
