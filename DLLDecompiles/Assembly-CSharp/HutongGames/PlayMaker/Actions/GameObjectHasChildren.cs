using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F51 RID: 3921
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if a GameObject has children.")]
	public class GameObjectHasChildren : FsmStateAction
	{
		// Token: 0x06006CFE RID: 27902 RVA: 0x0021F1E2 File Offset: 0x0021D3E2
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CFF RID: 27903 RVA: 0x0021F207 File Offset: 0x0021D407
		public override void OnEnter()
		{
			this.DoHasChildren();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D00 RID: 27904 RVA: 0x0021F21D File Offset: 0x0021D41D
		public override void OnUpdate()
		{
			this.DoHasChildren();
		}

		// Token: 0x06006D01 RID: 27905 RVA: 0x0021F228 File Offset: 0x0021D428
		private void DoHasChildren()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			bool flag = ownerDefaultTarget.transform.childCount > 0;
			this.storeResult.Value = flag;
			base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x04006CC0 RID: 27840
		[RequiredField]
		[Tooltip("The GameObject to test.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006CC1 RID: 27841
		[Tooltip("Event to send if the GameObject has children.")]
		public FsmEvent trueEvent;

		// Token: 0x04006CC2 RID: 27842
		[Tooltip("Event to send if the GameObject does not have children.")]
		public FsmEvent falseEvent;

		// Token: 0x04006CC3 RID: 27843
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006CC4 RID: 27844
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
