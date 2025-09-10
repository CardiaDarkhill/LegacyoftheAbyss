using System;
using TMProOld;
using UnityEngine;

// Token: 0x020006BE RID: 1726
public class InventoryArrowContainer : MonoBehaviour
{
	// Token: 0x06003E93 RID: 16019 RVA: 0x00113B47 File Offset: 0x00111D47
	protected void Start()
	{
		this.Setup();
		ManagerSingleton<InputHandler>.Instance.RefreshActiveControllerEvent += this.Setup;
	}

	// Token: 0x06003E94 RID: 16020 RVA: 0x00113B65 File Offset: 0x00111D65
	private void OnDestroy()
	{
		if (ManagerSingleton<InputHandler>.UnsafeInstance)
		{
			ManagerSingleton<InputHandler>.UnsafeInstance.RefreshActiveControllerEvent -= this.Setup;
		}
	}

	// Token: 0x06003E95 RID: 16021 RVA: 0x00113B8C File Offset: 0x00111D8C
	private void Setup()
	{
		bool isControllerImplicit = Platform.Current.IsControllerImplicit;
		this.arrowVariant.SetActive(!isControllerImplicit);
		this.promptVariant.SetActive(isControllerImplicit);
		if (isControllerImplicit && this.label)
		{
			Vector4 margin = this.label.margin;
			margin.x += this.labelLeftInset;
			margin.z += this.labelRightInset;
			this.label.margin = margin;
		}
		base.enabled = false;
	}

	// Token: 0x0400403B RID: 16443
	[SerializeField]
	private GameObject arrowVariant;

	// Token: 0x0400403C RID: 16444
	[SerializeField]
	private GameObject promptVariant;

	// Token: 0x0400403D RID: 16445
	[SerializeField]
	private TextMeshPro label;

	// Token: 0x0400403E RID: 16446
	[SerializeField]
	private float labelLeftInset;

	// Token: 0x0400403F RID: 16447
	[SerializeField]
	private float labelRightInset;
}
