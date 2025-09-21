using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200113A RID: 4410
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends an event when a UI Button is clicked.")]
	public class UiButtonOnClickEvent : ComponentAction<Button>
	{
		// Token: 0x060076CA RID: 30410 RVA: 0x002437EC File Offset: 0x002419EC
		public override void Reset()
		{
			this.gameObject = null;
			this.sendEvent = null;
		}

		// Token: 0x060076CB RID: 30411 RVA: 0x002437FC File Offset: 0x002419FC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				base.LogError("Missing GameObject ");
				return;
			}
			if (this.button != null)
			{
				this.button.onClick.RemoveListener(new UnityAction(this.DoOnClick));
			}
			this.button = this.cachedComponent;
			if (this.button != null)
			{
				this.button.onClick.AddListener(new UnityAction(this.DoOnClick));
				return;
			}
			base.LogError("Missing UI.Button on " + ownerDefaultTarget.name);
		}

		// Token: 0x060076CC RID: 30412 RVA: 0x002438A7 File Offset: 0x00241AA7
		public override void OnExit()
		{
			if (this.button != null)
			{
				this.button.onClick.RemoveListener(new UnityAction(this.DoOnClick));
			}
		}

		// Token: 0x060076CD RID: 30413 RVA: 0x002438D3 File Offset: 0x00241AD3
		public void DoOnClick()
		{
			base.SendEvent(this.eventTarget, this.sendEvent);
		}

		// Token: 0x0400773D RID: 30525
		[RequiredField]
		[CheckForComponent(typeof(Button))]
		[Tooltip("The GameObject with the UI Button component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400773E RID: 30526
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x0400773F RID: 30527
		[Tooltip("Send this event when Clicked.")]
		public FsmEvent sendEvent;

		// Token: 0x04007740 RID: 30528
		private Button button;
	}
}
