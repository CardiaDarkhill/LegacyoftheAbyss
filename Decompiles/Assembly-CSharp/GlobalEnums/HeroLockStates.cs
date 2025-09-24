using System;

namespace GlobalEnums
{
	// Token: 0x0200083B RID: 2107
	[Flags]
	public enum HeroLockStates
	{
		// Token: 0x04004AD5 RID: 19157
		None = 0,
		// Token: 0x04004AD6 RID: 19158
		AnimationLocked = 1,
		// Token: 0x04004AD7 RID: 19159
		ControlLocked = 2,
		// Token: 0x04004AD8 RID: 19160
		GravityLocked = 4,
		// Token: 0x04004AD9 RID: 19161
		All = -1
	}
}
