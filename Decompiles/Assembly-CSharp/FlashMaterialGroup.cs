using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000086 RID: 134
[RequireComponent(typeof(Renderer))]
public class FlashMaterialGroup : MonoBehaviour
{
	// Token: 0x060003C4 RID: 964 RVA: 0x00012CEC File Offset: 0x00010EEC
	private void Awake()
	{
		this.renderer = base.GetComponent<Renderer>();
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x00012CFC File Offset: 0x00010EFC
	private void Start()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		List<Renderer> list = new List<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer != this.renderer && renderer.sharedMaterial == this.renderer.sharedMaterial)
			{
				list.Add(renderer);
			}
		}
		this.material = new Material(this.renderer.sharedMaterial);
		this.renderer.sharedMaterial = this.material;
		foreach (Renderer renderer2 in list)
		{
			renderer2.material = this.material;
		}
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x00012DC4 File Offset: 0x00010FC4
	private void OnDestroy()
	{
		if (this.material != null)
		{
			Object.Destroy(this.material);
		}
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x00012DDF File Offset: 0x00010FDF
	private void Update()
	{
		if (this.material)
		{
			this.material.SetFloat("_FlashAmount", this.flashAmount);
		}
	}

	// Token: 0x04000362 RID: 866
	[Range(0f, 1f)]
	public float flashAmount;

	// Token: 0x04000363 RID: 867
	private Renderer renderer;

	// Token: 0x04000364 RID: 868
	private Material material;
}
