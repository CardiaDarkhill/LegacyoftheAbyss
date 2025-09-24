using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006CE RID: 1742
public class MapKey : MonoBehaviour
{
	// Token: 0x06003EE3 RID: 16099 RVA: 0x00114C68 File Offset: 0x00112E68
	private void OnEnable()
	{
		foreach (object obj in this.keysParent)
		{
			((Transform)obj).gameObject.SetActive(true);
		}
		this.layoutGroup.ForceUpdateLayoutNoCanvas();
	}

	// Token: 0x04004089 RID: 16521
	[SerializeField]
	private Transform keysParent;

	// Token: 0x0400408A RID: 16522
	[SerializeField]
	private LayoutGroup layoutGroup;
}
