using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001118 RID: 4376
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Returns the EventSystem's currently select GameObject.")]
	public class UiGetSelectedGameObject : FsmStateAction
	{
		// Token: 0x06007632 RID: 30258 RVA: 0x00241A25 File Offset: 0x0023FC25
		public override void Reset()
		{
			this.StoreGameObject = null;
			this.everyFrame = false;
		}

		// Token: 0x06007633 RID: 30259 RVA: 0x00241A35 File Offset: 0x0023FC35
		public override void OnEnter()
		{
			this.GetCurrentSelectedGameObject();
			this.lastGameObject = this.StoreGameObject.Value;
		}

		// Token: 0x06007634 RID: 30260 RVA: 0x00241A50 File Offset: 0x0023FC50
		public override void OnUpdate()
		{
			this.GetCurrentSelectedGameObject();
			if (this.StoreGameObject.Value != this.lastGameObject && this.ObjectChangedEvent != null)
			{
				base.Fsm.Event(this.ObjectChangedEvent);
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007635 RID: 30261 RVA: 0x00241AA2 File Offset: 0x0023FCA2
		private void GetCurrentSelectedGameObject()
		{
			this.StoreGameObject.Value = EventSystem.current.currentSelectedGameObject;
		}

		// Token: 0x040076B1 RID: 30385
		[UIHint(UIHint.Variable)]
		[Tooltip("The currently selected GameObject")]
		public FsmGameObject StoreGameObject;

		// Token: 0x040076B2 RID: 30386
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when the selected GameObject changes")]
		public FsmEvent ObjectChangedEvent;

		// Token: 0x040076B3 RID: 30387
		[UIHint(UIHint.Variable)]
		[Tooltip("If true, each frame will check the currently selected GameObject")]
		public bool everyFrame;

		// Token: 0x040076B4 RID: 30388
		private GameObject lastGameObject;
	}
}
