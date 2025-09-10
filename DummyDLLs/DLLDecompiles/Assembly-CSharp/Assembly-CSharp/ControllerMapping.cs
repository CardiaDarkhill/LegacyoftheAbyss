using System;
using GlobalEnums;
using InControl;

// Token: 0x020003C6 RID: 966
[Serializable]
public class ControllerMapping
{
	// Token: 0x04001FE4 RID: 8164
	public GamepadType gamepadType;

	// Token: 0x04001FE5 RID: 8165
	public InputControlType jump = InputControlType.Action1;

	// Token: 0x04001FE6 RID: 8166
	public InputControlType attack = InputControlType.Action3;

	// Token: 0x04001FE7 RID: 8167
	public InputControlType dash = InputControlType.RightTrigger;

	// Token: 0x04001FE8 RID: 8168
	public InputControlType cast = InputControlType.Action2;

	// Token: 0x04001FE9 RID: 8169
	public InputControlType superDash = InputControlType.LeftTrigger;

	// Token: 0x04001FEA RID: 8170
	public InputControlType dreamNail = InputControlType.Action4;

	// Token: 0x04001FEB RID: 8171
	public InputControlType quickMap = InputControlType.LeftBumper;

	// Token: 0x04001FEC RID: 8172
	public InputControlType quickCast = InputControlType.RightBumper;

	// Token: 0x04001FED RID: 8173
	public InputControlType taunt = InputControlType.RightStickButton;
}
