using System;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001119 RID: 4377
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Checks if Pointer is over a UI object, optionally takes a pointer ID, otherwise uses the current event.")]
	public class UiIsPointerOverUiObject : FsmStateAction
	{
		// Token: 0x06007637 RID: 30263 RVA: 0x00241AC1 File Offset: 0x0023FCC1
		public override void Reset()
		{
			this.pointerId = new FsmInt
			{
				UseVariable = true
			};
			this.pointerOverUI = null;
			this.pointerNotOverUI = null;
			this.isPointerOverUI = null;
			this.everyFrame = false;
		}

		// Token: 0x06007638 RID: 30264 RVA: 0x00241AF1 File Offset: 0x0023FCF1
		public override void OnEnter()
		{
			this.DoCheckPointer();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007639 RID: 30265 RVA: 0x00241B07 File Offset: 0x0023FD07
		public override void OnUpdate()
		{
			this.DoCheckPointer();
		}

		// Token: 0x0600763A RID: 30266 RVA: 0x00241B10 File Offset: 0x0023FD10
		private void DoCheckPointer()
		{
			bool flag = false;
			if (this.pointerId.IsNone)
			{
				flag = EventSystem.current.IsPointerOverGameObject();
			}
			else if (EventSystem.current.currentInputModule is PointerInputModule)
			{
				flag = (EventSystem.current.currentInputModule as PointerInputModule).IsPointerOverGameObject(this.pointerId.Value);
			}
			this.isPointerOverUI.Value = flag;
			base.Fsm.Event(flag ? this.pointerOverUI : this.pointerNotOverUI);
		}

		// Token: 0x040076B5 RID: 30389
		[Tooltip("Optional PointerId. Leave as None to use the current event")]
		public FsmInt pointerId;

		// Token: 0x040076B6 RID: 30390
		[Tooltip("Event to send when the Pointer is over an UI object.")]
		public FsmEvent pointerOverUI;

		// Token: 0x040076B7 RID: 30391
		[Tooltip("Event to send when the Pointer is NOT over an UI object.")]
		public FsmEvent pointerNotOverUI;

		// Token: 0x040076B8 RID: 30392
		[UIHint(UIHint.Variable)]
		[Tooltip("Store if the pointer is over a UI object in a Bool variable.")]
		public FsmBool isPointerOverUI;

		// Token: 0x040076B9 RID: 30393
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
