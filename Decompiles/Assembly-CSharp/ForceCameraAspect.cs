using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000169 RID: 361
public class ForceCameraAspect : MonoBehaviour
{
	// Token: 0x14000016 RID: 22
	// (add) Token: 0x06000B5C RID: 2908 RVA: 0x000341AC File Offset: 0x000323AC
	// (remove) Token: 0x06000B5D RID: 2909 RVA: 0x000341E0 File Offset: 0x000323E0
	public static event Action<float> ViewportAspectChanged;

	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06000B5E RID: 2910 RVA: 0x00034214 File Offset: 0x00032414
	// (remove) Token: 0x06000B5F RID: 2911 RVA: 0x00034248 File Offset: 0x00032448
	public static event Action<float> MainCamHeightMultChanged;

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x06000B60 RID: 2912 RVA: 0x0003427C File Offset: 0x0003247C
	// (remove) Token: 0x06000B61 RID: 2913 RVA: 0x000342B0 File Offset: 0x000324B0
	public static event Action<float> MainCamFovChanged;

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000B62 RID: 2914 RVA: 0x000342E3 File Offset: 0x000324E3
	// (set) Token: 0x06000B63 RID: 2915 RVA: 0x000342EA File Offset: 0x000324EA
	public static float CurrentViewportAspect { get; private set; }

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000B64 RID: 2916 RVA: 0x000342F2 File Offset: 0x000324F2
	// (set) Token: 0x06000B65 RID: 2917 RVA: 0x000342F9 File Offset: 0x000324F9
	public static float CurrentMainCamHeightMult { get; private set; }

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000B66 RID: 2918 RVA: 0x00034301 File Offset: 0x00032501
	// (set) Token: 0x06000B67 RID: 2919 RVA: 0x00034308 File Offset: 0x00032508
	public static float CurrentMainCamFov { get; private set; }

	// Token: 0x06000B68 RID: 2920 RVA: 0x00034310 File Offset: 0x00032510
	private void Awake()
	{
		this.tk2dCam = base.GetComponent<tk2dCamera>();
		ForceCameraAspect.CurrentViewportAspect = 1.7777778f;
		this.clearCamera.enabled = false;
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x00034334 File Offset: 0x00032534
	private void Start()
	{
		this.hudCam = GameCameras.instance.hudCamera;
		this.initialFov = this.tk2dCam.CameraSettings.fieldOfView;
		this.initialHudCamSize = this.hudCam.orthographicSize;
		this.AutoScaleViewport();
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x00034374 File Offset: 0x00032574
	private void Update()
	{
		if (this.lastX == Screen.width && this.lastY == Screen.height)
		{
			return;
		}
		float num = this.AutoScaleViewport();
		this.lastX = Screen.width;
		this.lastY = Screen.height;
		Action<float> viewportAspectChanged = ForceCameraAspect.ViewportAspectChanged;
		if (viewportAspectChanged != null)
		{
			viewportAspectChanged(num);
		}
		ForceCameraAspect.CurrentViewportAspect = num;
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x000343D0 File Offset: 0x000325D0
	public void SetOverscanViewport(float adjustment)
	{
		this.scaleAdjust = adjustment;
		this.AutoScaleViewport();
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x000343E0 File Offset: 0x000325E0
	private float AutoScaleViewport()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float clampedBetween = new MinMaxFloat(1.6f, 2.3916667f).GetClampedBetween(num);
		float num2 = num / clampedBetween;
		float num3 = 1f + this.scaleAdjust;
		Rect rect = this.tk2dCam.CameraSettings.rect;
		if (num2 < 1f)
		{
			rect.width = 1f * num3;
			rect.height = num2 * num3;
			float x = (1f - rect.width) / 2f;
			rect.x = x;
			float y = (1f - rect.height) / 2f;
			rect.y = y;
		}
		else
		{
			float num4 = 1f / num2;
			rect.width = num4 * num3;
			rect.height = 1f * num3;
			float x2 = (1f - rect.width) / 2f;
			rect.x = x2;
			float y2 = (1f - rect.height) / 2f;
			rect.y = y2;
		}
		this.tk2dCam.CameraSettings.rect = rect;
		this.hudCam.rect = rect;
		float num5;
		if (clampedBetween < 1.7777778f)
		{
			num5 = 1.7777778f / clampedBetween;
		}
		else
		{
			num5 = 1f;
		}
		Action<float> mainCamHeightMultChanged = ForceCameraAspect.MainCamHeightMultChanged;
		if (mainCamHeightMultChanged != null)
		{
			mainCamHeightMultChanged(num5);
		}
		ForceCameraAspect.CurrentMainCamHeightMult = num5;
		float num6 = (this.initialFov + this.fovOffset + this.extraFovOffset) * num5;
		this.tk2dCam.CameraSettings.fieldOfView = num6;
		Action<float> mainCamFovChanged = ForceCameraAspect.MainCamFovChanged;
		if (mainCamFovChanged != null)
		{
			mainCamFovChanged(num6);
		}
		ForceCameraAspect.CurrentMainCamFov = num6;
		this.hudCam.orthographicSize = this.initialHudCamSize * num5;
		if (this.anchorTopLeft)
		{
			this.anchorTopLeft.localPosition = new Vector3(0f, this.hudCam.orthographicSize - this.initialHudCamSize, 0f);
		}
		this.clearCamera.enabled = (rect.x > Mathf.Epsilon || rect.y > Mathf.Epsilon);
		return clampedBetween;
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x00034604 File Offset: 0x00032804
	public void SetFovOffset(float offset, float transitionTime, AnimationCurve curve)
	{
		if (this.fovTransitionRoutine != null)
		{
			base.StopCoroutine(this.fovTransitionRoutine);
			this.fovTransitionRoutine = null;
		}
		if (Mathf.Approximately(offset, this.fovOffset))
		{
			return;
		}
		if (transitionTime <= Mathf.Epsilon)
		{
			this.fovOffset = offset;
			this.AutoScaleViewport();
			return;
		}
		this.fovTransitionRoutine = base.StartCoroutine(this.TransitionFovOffset(offset, transitionTime, curve));
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x00034667 File Offset: 0x00032867
	private IEnumerator TransitionFovOffset(float newOffset, float transitionTime, AnimationCurve curve)
	{
		float initialOffset = this.fovOffset;
		for (float elapsed = 0f; elapsed < transitionTime; elapsed += Time.deltaTime)
		{
			float t = curve.Evaluate(elapsed / transitionTime);
			this.fovOffset = Mathf.Lerp(initialOffset, newOffset, t);
			this.AutoScaleViewport();
			yield return null;
		}
		this.fovOffset = newOffset;
		this.AutoScaleViewport();
		this.fovTransitionRoutine = null;
		yield break;
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0003468B File Offset: 0x0003288B
	public void SetExtraFovOffset(float value)
	{
		this.extraFovOffset = value;
		this.AutoScaleViewport();
	}

	// Token: 0x04000B04 RID: 2820
	[SerializeField]
	private Transform anchorTopLeft;

	// Token: 0x04000B05 RID: 2821
	[SerializeField]
	private Camera clearCamera;

	// Token: 0x04000B06 RID: 2822
	private tk2dCamera tk2dCam;

	// Token: 0x04000B07 RID: 2823
	private Camera hudCam;

	// Token: 0x04000B08 RID: 2824
	private float initialFov;

	// Token: 0x04000B09 RID: 2825
	private float initialHudCamSize;

	// Token: 0x04000B0A RID: 2826
	private int lastX;

	// Token: 0x04000B0B RID: 2827
	private int lastY;

	// Token: 0x04000B0C RID: 2828
	private float scaleAdjust;

	// Token: 0x04000B0D RID: 2829
	private float fovOffset;

	// Token: 0x04000B0E RID: 2830
	private float extraFovOffset;

	// Token: 0x04000B0F RID: 2831
	private Coroutine fovTransitionRoutine;
}
