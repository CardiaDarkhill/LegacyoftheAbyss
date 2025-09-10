using System;
using UnityEngine;

// Token: 0x0200015E RID: 350
public static class CameraInfoCache
{
	// Token: 0x170000EF RID: 239
	// (get) Token: 0x06000ACF RID: 2767 RVA: 0x0003120A File Offset: 0x0002F40A
	// (set) Token: 0x06000AD0 RID: 2768 RVA: 0x00031211 File Offset: 0x0002F411
	public static float Aspect { get; private set; }

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06000AD1 RID: 2769 RVA: 0x00031219 File Offset: 0x0002F419
	// (set) Token: 0x06000AD2 RID: 2770 RVA: 0x00031220 File Offset: 0x0002F420
	public static float PosX { get; private set; }

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000AD3 RID: 2771 RVA: 0x00031228 File Offset: 0x0002F428
	// (set) Token: 0x06000AD4 RID: 2772 RVA: 0x0003122F File Offset: 0x0002F42F
	public static float PosY { get; private set; }

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x00031237 File Offset: 0x0002F437
	// (set) Token: 0x06000AD6 RID: 2774 RVA: 0x0003123E File Offset: 0x0002F43E
	public static float PosZ { get; private set; }

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000AD7 RID: 2775 RVA: 0x00031246 File Offset: 0x0002F446
	// (set) Token: 0x06000AD8 RID: 2776 RVA: 0x0003124D File Offset: 0x0002F44D
	public static float UnitsWidth { get; private set; }

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000AD9 RID: 2777 RVA: 0x00031255 File Offset: 0x0002F455
	// (set) Token: 0x06000ADA RID: 2778 RVA: 0x0003125C File Offset: 0x0002F45C
	public static float UnitsHeight { get; private set; }

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000ADB RID: 2779 RVA: 0x00031264 File Offset: 0x0002F464
	// (set) Token: 0x06000ADC RID: 2780 RVA: 0x0003126B File Offset: 0x0002F46B
	public static float HalfWidth { get; private set; }

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000ADD RID: 2781 RVA: 0x00031273 File Offset: 0x0002F473
	// (set) Token: 0x06000ADE RID: 2782 RVA: 0x0003127A File Offset: 0x0002F47A
	public static float HalfHeight { get; private set; }

	// Token: 0x06000ADF RID: 2783 RVA: 0x00031284 File Offset: 0x0002F484
	public static void UpdateCache()
	{
		if (Time.frameCount == CameraInfoCache.cachedFrameCount)
		{
			return;
		}
		CameraInfoCache.cachedFrameCount = Time.frameCount;
		GameCameras instance = GameCameras.instance;
		if (instance == null)
		{
			return;
		}
		Camera mainCamera = instance.mainCamera;
		if (mainCamera == null)
		{
			return;
		}
		CameraInfoCache.Aspect = mainCamera.aspect;
		Vector3 position = mainCamera.transform.position;
		CameraInfoCache.PosX = position.x;
		CameraInfoCache.PosY = position.y;
		CameraInfoCache.PosZ = position.z;
		if (!Mathf.Approximately(CameraInfoCache.Aspect, CameraInfoCache.lastUnitsAspect) || !Mathf.Approximately(CameraInfoCache.PosZ, CameraInfoCache.lastUnitsZ))
		{
			CameraInfoCache.lastUnitsAspect = CameraInfoCache.Aspect;
			CameraInfoCache.lastUnitsZ = CameraInfoCache.PosZ;
			float num = Mathf.Abs(CameraInfoCache.PosZ);
			if (mainCamera.orthographic)
			{
				CameraInfoCache.UnitsHeight = mainCamera.orthographicSize * 2f;
				CameraInfoCache.UnitsWidth = CameraInfoCache.UnitsHeight * CameraInfoCache.Aspect;
			}
			else
			{
				float num2 = mainCamera.fieldOfView * 0.017453292f;
				CameraInfoCache.UnitsHeight = 2f * num * Mathf.Tan(num2 / 2f);
				CameraInfoCache.UnitsWidth = CameraInfoCache.UnitsHeight * CameraInfoCache.Aspect;
			}
			CameraInfoCache.HalfWidth = CameraInfoCache.UnitsWidth * 0.5f;
			CameraInfoCache.HalfHeight = CameraInfoCache.UnitsHeight * 0.5f;
		}
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x000313C4 File Offset: 0x0002F5C4
	public static bool IsWithinBounds(Vector2 position, Vector2 buffer)
	{
		CameraInfoCache.UpdateCache();
		return Mathf.Abs(CameraInfoCache.PosX - position.x) <= CameraInfoCache.HalfWidth + buffer.x && Mathf.Abs(CameraInfoCache.PosY - position.y) <= CameraInfoCache.HalfHeight + buffer.y;
	}

	// Token: 0x04000A54 RID: 2644
	private static int cachedFrameCount = -1;

	// Token: 0x04000A5D RID: 2653
	private static float lastUnitsAspect = -1f;

	// Token: 0x04000A5E RID: 2654
	private static float lastUnitsZ = float.NaN;
}
