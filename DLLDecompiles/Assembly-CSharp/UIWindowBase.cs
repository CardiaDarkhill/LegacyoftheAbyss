using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200000E RID: 14
public class UIWindowBase : MonoBehaviour, IDragHandler, IEventSystemHandler
{
	// Token: 0x0600006A RID: 106 RVA: 0x00004065 File Offset: 0x00002265
	private void Start()
	{
		this.m_transform = base.GetComponent<RectTransform>();
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00004073 File Offset: 0x00002273
	public void OnDrag(PointerEventData eventData)
	{
		this.m_transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
	}

	// Token: 0x0600006C RID: 108 RVA: 0x000040A6 File Offset: 0x000022A6
	public void ChangeStrength(float value)
	{
		base.GetComponent<Image>().material.SetFloat("_Size", value);
	}

	// Token: 0x0600006D RID: 109 RVA: 0x000040BE File Offset: 0x000022BE
	public void ChangeVibrancy(float value)
	{
		base.GetComponent<Image>().material.SetFloat("_Vibrancy", value);
	}

	// Token: 0x04000059 RID: 89
	private RectTransform m_transform;
}
