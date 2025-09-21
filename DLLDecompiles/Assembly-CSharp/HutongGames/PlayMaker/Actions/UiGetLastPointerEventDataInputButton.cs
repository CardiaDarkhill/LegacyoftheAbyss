using System;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001117 RID: 4375
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets pointer data Input Button on the last System event.")]
	public class UiGetLastPointerEventDataInputButton : FsmStateAction
	{
		// Token: 0x0600762E RID: 30254 RVA: 0x00241950 File Offset: 0x0023FB50
		public override void Reset()
		{
			this.inputButton = PointerEventData.InputButton.Left;
			this.leftClick = null;
			this.middleClick = null;
			this.rightClick = null;
		}

		// Token: 0x0600762F RID: 30255 RVA: 0x00241978 File Offset: 0x0023FB78
		public override void OnEnter()
		{
			this.ExecuteAction();
			base.Finish();
		}

		// Token: 0x06007630 RID: 30256 RVA: 0x00241988 File Offset: 0x0023FB88
		private void ExecuteAction()
		{
			if (UiGetLastPointerDataInfo.lastPointerEventData == null)
			{
				return;
			}
			if (!this.inputButton.IsNone)
			{
				this.inputButton.Value = UiGetLastPointerDataInfo.lastPointerEventData.button;
			}
			if (UiGetLastPointerDataInfo.lastPointerEventData.button == PointerEventData.InputButton.Left)
			{
				base.Fsm.Event(this.leftClick);
			}
			if (UiGetLastPointerDataInfo.lastPointerEventData.button == PointerEventData.InputButton.Middle)
			{
				base.Fsm.Event(this.middleClick);
			}
			if (UiGetLastPointerDataInfo.lastPointerEventData.button == PointerEventData.InputButton.Right)
			{
				base.Fsm.Event(this.rightClick);
			}
		}

		// Token: 0x040076AD RID: 30381
		[Tooltip("Store the Input Button pressed (Left, Right, Middle)")]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(PointerEventData.InputButton))]
		public FsmEnum inputButton;

		// Token: 0x040076AE RID: 30382
		[Tooltip("Event to send if Left Button clicked.")]
		public FsmEvent leftClick;

		// Token: 0x040076AF RID: 30383
		[Tooltip("Event to send if Middle Button clicked.")]
		public FsmEvent middleClick;

		// Token: 0x040076B0 RID: 30384
		[Tooltip("Event to send if Right Button clicked.")]
		public FsmEvent rightClick;
	}
}
