using System;
using InControl;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DAF RID: 3503
	[ActionCategory("InControl")]
	[Tooltip("Sends an Event when the specified Incontrol control Axis for a given Device is released. Optionally store the control state in a bool variable.")]
	public class GetInControlDeviceInputButtonUp : FsmStateAction
	{
		// Token: 0x060065AC RID: 26028 RVA: 0x002016B7 File Offset: 0x001FF8B7
		public override void Reset()
		{
			this.deviceIndex = null;
			this.axis = InputControlType.Action1;
			this.sendEvent = null;
			this.storeResult = null;
		}

		// Token: 0x060065AD RID: 26029 RVA: 0x002016D6 File Offset: 0x001FF8D6
		public override void OnEnter()
		{
			if (this.deviceIndex.Value == -1)
			{
				this._inputDevice = InputManager.ActiveDevice;
				return;
			}
			this._inputDevice = InputManager.Devices[this.deviceIndex.Value];
		}

		// Token: 0x060065AE RID: 26030 RVA: 0x00201710 File Offset: 0x001FF910
		public override void OnUpdate()
		{
			bool flag = !this._inputDevice.GetControl(this.axis).IsPressed;
			if (flag)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = flag;
		}

		// Token: 0x040064D2 RID: 25810
		[Tooltip("The index of the Device.")]
		public FsmInt deviceIndex;

		// Token: 0x040064D3 RID: 25811
		public InputControlType axis;

		// Token: 0x040064D4 RID: 25812
		public FsmEvent sendEvent;

		// Token: 0x040064D5 RID: 25813
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		// Token: 0x040064D6 RID: 25814
		private InputDevice _inputDevice;
	}
}
