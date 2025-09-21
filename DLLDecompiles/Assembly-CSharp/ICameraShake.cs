using System;
using UnityEngine;

// Token: 0x02000152 RID: 338
public interface ICameraShake
{
	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x06000A3E RID: 2622
	bool CanFinish { get; }

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x06000A3F RID: 2623 RVA: 0x0002E5C7 File Offset: 0x0002C7C7
	bool PersistThroughScenes
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x06000A40 RID: 2624
	int FreezeFrames { get; }

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x06000A41 RID: 2625
	ICameraShakeVibration CameraShakeVibration { get; }

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x06000A42 RID: 2626
	CameraShakeWorldForceIntensities WorldForceOnStart { get; }

	// Token: 0x06000A43 RID: 2627
	Vector2 GetOffset(float elapsedTime);

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06000A44 RID: 2628
	float Magnitude { get; }

	// Token: 0x06000A45 RID: 2629
	bool IsDone(float elapsedTime);
}
