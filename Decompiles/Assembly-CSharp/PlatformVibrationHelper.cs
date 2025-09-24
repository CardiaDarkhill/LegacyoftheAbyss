using System;
using InControl;
using UnityEngine;

// Token: 0x02000472 RID: 1138
public class PlatformVibrationHelper
{
	// Token: 0x060028AD RID: 10413 RVA: 0x000B2F21 File Offset: 0x000B1121
	public PlatformVibrationHelper()
	{
		this.vibrationMixer = new GamepadVibrationMixer(GamepadVibrationMixer.PlatformAdjustments.None);
	}

	// Token: 0x060028AE RID: 10414 RVA: 0x000B2F35 File Offset: 0x000B1135
	public void Destroy()
	{
		if (this.lastVibratingInputDevice != null)
		{
			this.lastVibratingInputDevice.StopVibration();
			this.lastVibratingInputDevice = null;
		}
	}

	// Token: 0x060028AF RID: 10415 RVA: 0x000B2F54 File Offset: 0x000B1154
	public void UpdateVibration()
	{
		this.vibrationMixer.Update(Time.unscaledDeltaTime);
		GamepadVibrationMixer.GamepadVibrationEmission.Values currentValues = this.vibrationMixer.CurrentValues;
		InputDevice activeDevice = InputManager.ActiveDevice;
		if (this.lastVibratingInputDevice != activeDevice)
		{
			if (this.lastVibratingInputDevice != null)
			{
				this.lastVibratingInputDevice.StopVibration();
				this.lastVibratingInputDevice = null;
			}
			this.lastVibratingInputDevice = activeDevice;
			if (this.lastVibratingInputDevice != null)
			{
				this.lastVibratingInputDevice.StopVibration();
			}
			this.lastVibrationWasEmpty = false;
		}
		if (this.lastVibratingInputDevice != null)
		{
			if (!this.lastVibrationWasEmpty || !currentValues.IsNearlyZero)
			{
				this.lastVibratingInputDevice.Vibrate(currentValues.Small, currentValues.Large);
			}
			this.lastVibrationWasEmpty = currentValues.IsNearlyZero;
		}
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x000B3004 File Offset: 0x000B1204
	public VibrationMixer GetMixer()
	{
		return this.vibrationMixer;
	}

	// Token: 0x040024B4 RID: 9396
	private readonly GamepadVibrationMixer vibrationMixer;

	// Token: 0x040024B5 RID: 9397
	private InputDevice lastVibratingInputDevice;

	// Token: 0x040024B6 RID: 9398
	private bool lastVibrationWasEmpty;
}
