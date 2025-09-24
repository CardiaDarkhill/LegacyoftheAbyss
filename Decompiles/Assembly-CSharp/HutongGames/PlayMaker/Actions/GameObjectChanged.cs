using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F4E RID: 3918
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if the value of a GameObject variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class GameObjectChanged : FsmStateAction
	{
		// Token: 0x06006CF0 RID: 27888 RVA: 0x0021EFC3 File Offset: 0x0021D1C3
		public override void Reset()
		{
			this.gameObjectVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006CF1 RID: 27889 RVA: 0x0021EFDA File Offset: 0x0021D1DA
		public override void OnEnter()
		{
			if (this.gameObjectVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.gameObjectVariable.Value;
		}

		// Token: 0x06006CF2 RID: 27890 RVA: 0x0021F004 File Offset: 0x0021D204
		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.gameObjectVariable.Value != this.previousValue)
			{
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}

		// Token: 0x04006CB0 RID: 27824
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The GameObject variable to watch for a change.")]
		public FsmGameObject gameObjectVariable;

		// Token: 0x04006CB1 RID: 27825
		[Tooltip("Event to send if the variable changes.")]
		public FsmEvent changedEvent;

		// Token: 0x04006CB2 RID: 27826
		[UIHint(UIHint.Variable)]
		[Tooltip("Set to True if the variable changes.")]
		public FsmBool storeResult;

		// Token: 0x04006CB3 RID: 27827
		private GameObject previousValue;
	}
}
