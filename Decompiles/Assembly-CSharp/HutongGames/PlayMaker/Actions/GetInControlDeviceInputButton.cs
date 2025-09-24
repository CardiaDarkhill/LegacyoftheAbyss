using System;
using InControl;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DAD RID: 3501
	[ActionCategory("InControl")]
	[Tooltip("Gets the pressed state of the specified InControl Button for a given Device and stores it in a Bool Variable.")]
	public class GetInControlDeviceInputButton : FsmStateAction
	{
		// Token: 0x060065A3 RID: 26019 RVA: 0x0020155C File Offset: 0x001FF75C
		public override void Reset()
		{
			this.deviceIndex = null;
			this.axis = InputControlType.Action1;
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x060065A4 RID: 26020 RVA: 0x0020157C File Offset: 0x001FF77C
		public override void OnEnter()
		{
			if (this.deviceIndex.Value == -1)
			{
				this._inputDevice = InputManager.ActiveDevice;
			}
			else
			{
				this._inputDevice = InputManager.Devices[this.deviceIndex.Value];
			}
			this.DoGetButton();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065A5 RID: 26021 RVA: 0x002015D3 File Offset: 0x001FF7D3
		public override void OnUpdate()
		{
			this.DoGetButton();
		}

		// Token: 0x060065A6 RID: 26022 RVA: 0x002015DB File Offset: 0x001FF7DB
		private void DoGetButton()
		{
			this.storeResult.Value = this._inputDevice.GetControl(this.axis).IsPressed;
		}

		// Token: 0x040064C7 RID: 25799
		[Tooltip("The index of the device.")]
		public FsmInt deviceIndex;

		// Token: 0x040064C8 RID: 25800
		public InputControlType axis;

		// Token: 0x040064C9 RID: 25801
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x040064CA RID: 25802
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040064CB RID: 25803
		private InputDevice _inputDevice;
	}
}
