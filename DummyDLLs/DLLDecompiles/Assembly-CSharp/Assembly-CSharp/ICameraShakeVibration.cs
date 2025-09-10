using System;

// Token: 0x02000153 RID: 339
public interface ICameraShakeVibration
{
	// Token: 0x06000A46 RID: 2630
	VibrationEmission PlayVibration(bool isRealtime);

	// Token: 0x06000A47 RID: 2631
	float GetVibrationStrength(float timeElapsed);
}
