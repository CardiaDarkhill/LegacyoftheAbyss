using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200004A RID: 74
public class RealtimeReflections : MonoBehaviour
{
	// Token: 0x0600020E RID: 526 RVA: 0x0000D3B2 File Offset: 0x0000B5B2
	private void OnEnable()
	{
		this.layerMask.value = -1;
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0000D3C0 File Offset: 0x0000B5C0
	private void Start()
	{
		foreach (ReflectionProbe reflectionProbe in this.reflectionProbes)
		{
			reflectionProbe.mode = ReflectionProbeMode.Realtime;
			reflectionProbe.boxProjection = true;
			reflectionProbe.resolution = this.cubemapSize;
			reflectionProbe.transform.parent = base.transform.parent;
			reflectionProbe.transform.localPosition = Vector3.zero;
		}
		if (this.materials.Length == 0)
		{
			return;
		}
		this.UpdateCubemap(63);
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000D438 File Offset: 0x0000B638
	private void LateUpdate()
	{
		if (this.materials.Length == 0)
		{
			return;
		}
		if (this.oneFacePerFrame)
		{
			int num = Time.frameCount % 6;
			int faceMask = 1 << num;
			this.UpdateCubemap(faceMask);
			return;
		}
		this.UpdateCubemap(63);
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000D478 File Offset: 0x0000B678
	private void UpdateCubemap(int faceMask)
	{
		if (!this.cam)
		{
			this.cam = new GameObject("CubemapCamera", new Type[]
			{
				typeof(Camera)
			})
			{
				hideFlags = HideFlags.HideAndDontSave,
				transform = 
				{
					position = base.transform.position,
					rotation = Quaternion.identity
				}
			}.GetComponent<Camera>();
			this.cam.cullingMask = this.layerMask;
			this.cam.nearClipPlane = this.nearClip;
			this.cam.farClipPlane = this.farClip;
			this.cam.enabled = false;
		}
		if (!this.renderTexture)
		{
			this.renderTexture = new RenderTexture(this.cubemapSize, this.cubemapSize, 16);
			this.renderTexture.name = "RealtimeReflections" + base.GetInstanceID().ToString();
			this.renderTexture.isPowerOfTwo = true;
			this.renderTexture.dimension = TextureDimension.Cube;
			this.renderTexture.hideFlags = HideFlags.HideAndDontSave;
			Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				foreach (Material material in componentsInChildren[i].sharedMaterials)
				{
					if (material.HasProperty("_Cube"))
					{
						material.SetTexture("_Cube", this.renderTexture);
					}
				}
			}
			ReflectionProbe[] array = this.reflectionProbes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].customBakedTexture = this.renderTexture;
			}
		}
		this.cam.transform.position = base.transform.position;
		this.cam.RenderToCubemap(this.renderTexture, faceMask);
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000D64C File Offset: 0x0000B84C
	private void OnDisable()
	{
		Object.DestroyImmediate(this.cam);
		Object.DestroyImmediate(this.renderTexture);
	}

	// Token: 0x040001B9 RID: 441
	public int cubemapSize = 128;

	// Token: 0x040001BA RID: 442
	public float nearClip = 0.01f;

	// Token: 0x040001BB RID: 443
	public float farClip = 500f;

	// Token: 0x040001BC RID: 444
	public bool oneFacePerFrame;

	// Token: 0x040001BD RID: 445
	public Material[] materials;

	// Token: 0x040001BE RID: 446
	public ReflectionProbe[] reflectionProbes;

	// Token: 0x040001BF RID: 447
	public LayerMask layerMask = -1;

	// Token: 0x040001C0 RID: 448
	private Camera cam;

	// Token: 0x040001C1 RID: 449
	private RenderTexture renderTexture;
}
