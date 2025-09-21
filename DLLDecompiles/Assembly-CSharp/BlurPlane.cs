using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021D RID: 541
[RequireComponent(typeof(MeshRenderer))]
public class BlurPlane : MonoBehaviour
{
	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06001417 RID: 5143 RVA: 0x0005AD69 File Offset: 0x00058F69
	public static int BlurPlaneCount
	{
		get
		{
			return BlurPlane._blurPlanes.Count;
		}
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x0005AD75 File Offset: 0x00058F75
	public static BlurPlane GetBlurPlane(int index)
	{
		return BlurPlane._blurPlanes[index];
	}

	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06001419 RID: 5145 RVA: 0x0005AD82 File Offset: 0x00058F82
	public static BlurPlane ClosestBlurPlane
	{
		get
		{
			if (BlurPlane._blurPlanes.Count <= 0)
			{
				return null;
			}
			return BlurPlane._blurPlanes[0];
		}
	}

	// Token: 0x1400003A RID: 58
	// (add) Token: 0x0600141A RID: 5146 RVA: 0x0005ADA0 File Offset: 0x00058FA0
	// (remove) Token: 0x0600141B RID: 5147 RVA: 0x0005ADD4 File Offset: 0x00058FD4
	public static event BlurPlane.BlurPlanesChangedDelegate BlurPlanesChanged;

	// Token: 0x1700023F RID: 575
	// (get) Token: 0x0600141C RID: 5148 RVA: 0x0005AE07 File Offset: 0x00059007
	public float PlaneZ
	{
		get
		{
			return base.transform.position.z;
		}
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x0005AE19 File Offset: 0x00059019
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		BlurPlane._blurPlanes = new List<BlurPlane>();
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x0005AE25 File Offset: 0x00059025
	protected void Awake()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.originalMaterial = this.meshRenderer.sharedMaterial;
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x0005AE44 File Offset: 0x00059044
	protected void OnEnable()
	{
		int i;
		for (i = 0; i < BlurPlane._blurPlanes.Count; i++)
		{
			BlurPlane blurPlane = BlurPlane._blurPlanes[i];
			if (this.PlaneZ <= blurPlane.PlaneZ)
			{
				break;
			}
		}
		BlurPlane._blurPlanes.Insert(i, this);
		if (BlurPlane.BlurPlanesChanged != null)
		{
			BlurPlane.BlurPlanesChanged();
		}
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x0005AE9D File Offset: 0x0005909D
	protected void OnDisable()
	{
		BlurPlane._blurPlanes.Remove(this);
		if (BlurPlane.BlurPlanesChanged != null)
		{
			BlurPlane.BlurPlanesChanged();
		}
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x0005AEBC File Offset: 0x000590BC
	public void SetPlaneVisibility(bool isVisible)
	{
		this.meshRenderer.enabled = isVisible;
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x0005AECA File Offset: 0x000590CA
	public void SetPlaneMaterial(Material material)
	{
		this.meshRenderer.sharedMaterial = ((material == null) ? this.originalMaterial : material);
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x0005AEE9 File Offset: 0x000590E9
	public static void SetVibranceOffset(float offset)
	{
		Shader.SetGlobalFloat(BlurPlane._blurPlaneVibranceOffset, offset);
	}

	// Token: 0x04001264 RID: 4708
	private MeshRenderer meshRenderer;

	// Token: 0x04001265 RID: 4709
	private Material originalMaterial;

	// Token: 0x04001266 RID: 4710
	private static List<BlurPlane> _blurPlanes;

	// Token: 0x04001268 RID: 4712
	private static readonly int _blurPlaneVibranceOffset = Shader.PropertyToID("_BlurPlaneVibranceOffset");

	// Token: 0x0200153D RID: 5437
	// (Invoke) Token: 0x06008629 RID: 34345
	public delegate void BlurPlanesChangedDelegate();
}
