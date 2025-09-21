using System;
using InControl;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DAE RID: 3502
	[ActionCategory("InControl")]
	[Tooltip("Sends an Event when the specified Incontrol control Axis for a given Device is pressed. Optionally store the control state in a bool variable.")]
	public class GetInControlDeviceInputButtonDown : FsmStateAction
	{
		// Token: 0x060065A8 RID: 26024 RVA: 0x00201606 File Offset: 0x001FF806
		public override void Reset()
		{
			this.deviceIndex = null;
			this.axis = InputControlType.Action1;
			this.sendEvent = null;
			this.storeResult = null;
		}

		// Token: 0x060065A9 RID: 26025 RVA: 0x00201625 File Offset: 0x001FF825
		public override void OnEnter()
		{
			if (this.deviceIndex.Value == -1)
			{
				this._inputDevice = InputManager.ActiveDevice;
				return;
			}
			this._inputDevice = InputManager.Devices[this.deviceIndex.Value];
		}

		// Token: 0x060065AA RID: 26026 RVA: 0x0020165C File Offset: 0x001FF85C
		public override void OnUpdate()
		{
			this.wasPressed = this._inputDevice.GetControl(this.axis).WasPressed;
			if (this.wasPressed)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = this.wasPressed;
		}

		// Token: 0x040064CC RID: 25804
		[Tooltip("The index of the Device.")]
		public FsmInt deviceIndex;

		// Token: 0x040064CD RID: 25805
		public InputControlType axis;

		// Token: 0x040064CE RID: 25806
		public FsmEvent sendEvent;

		// Token: 0x040064CF RID: 25807
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		// Token: 0x040064D0 RID: 25808
		private bool wasPressed;

		// Token: 0x040064D1 RID: 25809
		private InputDevice _inputDevice;
	}
}
