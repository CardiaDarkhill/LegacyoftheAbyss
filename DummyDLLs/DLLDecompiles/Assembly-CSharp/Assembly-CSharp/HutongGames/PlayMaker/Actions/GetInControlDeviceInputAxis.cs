using System;
using InControl;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DAB RID: 3499
	[ActionCategory("InControl")]
	[Tooltip("Gets the value of the specified Incontrol control Axis for a given device and stores it in a Float Variable.")]
	public class GetInControlDeviceInputAxis : FsmStateAction
	{
		// Token: 0x0600659B RID: 26011 RVA: 0x0020126B File Offset: 0x001FF46B
		public override void Reset()
		{
			this.deviceIndex = 0;
			this.axis = InputControlType.LeftStickRight;
			this.multiplier = 1f;
			this.store = null;
			this.everyFrame = true;
		}

		// Token: 0x0600659C RID: 26012 RVA: 0x0020129E File Offset: 0x001FF49E
		public override void OnEnter()
		{
			this.DoGetAxis();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600659D RID: 26013 RVA: 0x002012B4 File Offset: 0x001FF4B4
		public override void OnUpdate()
		{
			this.DoGetAxis();
		}

		// Token: 0x0600659E RID: 26014 RVA: 0x002012BC File Offset: 0x001FF4BC
		private void DoGetAxis()
		{
			if (this.deviceIndex.Value == -1)
			{
				this._inputDevice = InputManager.ActiveDevice;
			}
			else
			{
				this._inputDevice = InputManager.Devices[this.deviceIndex.Value];
			}
			float num = this._inputDevice.GetControl(this.axis).Value;
			if (!this.multiplier.IsNone)
			{
				num *= this.multiplier.Value;
			}
			this.store.Value = num;
		}

		// Token: 0x040064B7 RID: 25783
		[Tooltip("The index of the device. -1 to use the active device")]
		public FsmInt deviceIndex;

		// Token: 0x040064B8 RID: 25784
		public InputControlType axis;

		// Token: 0x040064B9 RID: 25785
		[Tooltip("Axis values are in the range -1 to 1. Use the multiplier to set a larger range.")]
		public FsmFloat multiplier;

		// Token: 0x040064BA RID: 25786
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a float variable.")]
		public FsmFloat store;

		// Token: 0x040064BB RID: 25787
		[Tooltip("Repeat every frame. Typically this would be set to True.")]
		public bool everyFrame;

		// Token: 0x040064BC RID: 25788
		private InputDevice _inputDevice;
	}
}
