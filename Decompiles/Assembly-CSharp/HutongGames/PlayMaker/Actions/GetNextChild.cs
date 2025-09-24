using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EB2 RID: 3762
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Each time this action is called it gets the next child of a GameObject. This lets you quickly loop through all the children of an object to perform actions on them. NOTE: To find a specific child use Find Child.")]
	public class GetNextChild : FsmStateAction
	{
		// Token: 0x06006A7D RID: 27261 RVA: 0x00214192 File Offset: 0x00212392
		public override void Reset()
		{
			this.gameObject = null;
			this.storeNextChild = null;
			this.loopEvent = null;
			this.finishedEvent = null;
			this.resetFlag = null;
		}

		// Token: 0x06006A7E RID: 27262 RVA: 0x002141B7 File Offset: 0x002123B7
		public override void OnEnter()
		{
			if (this.resetFlag.Value)
			{
				this.nextChildIndex = 0;
				this.resetFlag.Value = false;
			}
			this.DoGetNextChild(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x06006A7F RID: 27263 RVA: 0x002141F8 File Offset: 0x002123F8
		private void DoGetNextChild(GameObject parent)
		{
			if (parent == null)
			{
				return;
			}
			if (this.go != parent)
			{
				this.go = parent;
				this.nextChildIndex = 0;
			}
			if (this.nextChildIndex >= this.go.transform.childCount)
			{
				this.nextChildIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.storeNextChild.Value = parent.transform.GetChild(this.nextChildIndex).gameObject;
			if (this.nextChildIndex >= this.go.transform.childCount)
			{
				this.nextChildIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.nextChildIndex++;
			if (this.loopEvent != null)
			{
				base.Fsm.Event(this.loopEvent);
			}
		}

		// Token: 0x040069CF RID: 27087
		[RequiredField]
		[Tooltip("The parent GameObject. Note, if GameObject changes, this action will reset and start again at the first child.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069D0 RID: 27088
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the next child in a GameObject variable.")]
		public FsmGameObject storeNextChild;

		// Token: 0x040069D1 RID: 27089
		[Tooltip("Event to send to get the next child.")]
		public FsmEvent loopEvent;

		// Token: 0x040069D2 RID: 27090
		[Tooltip("Event to send when there are no more children.")]
		public FsmEvent finishedEvent;

		// Token: 0x040069D3 RID: 27091
		[Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again")]
		[UIHint(UIHint.Variable)]
		public FsmBool resetFlag;

		// Token: 0x040069D4 RID: 27092
		private GameObject go;

		// Token: 0x040069D5 RID: 27093
		private int nextChildIndex;
	}
}
