using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000719 RID: 1817
[RequireComponent(typeof(RectTransform))]
public class ScrollBarHandle : MonoBehaviour
{
	// Token: 0x060040AB RID: 16555 RVA: 0x0011C3D3 File Offset: 0x0011A5D3
	private void Awake()
	{
		this.trans = base.GetComponent<RectTransform>();
		if (!this.scrollBar)
		{
			this.scrollBar = base.GetComponentInParent<Scrollbar>();
		}
	}

	// Token: 0x060040AC RID: 16556 RVA: 0x0011C3FA File Offset: 0x0011A5FA
	private void Start()
	{
		if (this.scrollBar)
		{
			this.scrollBar.onValueChanged.AddListener(new UnityAction<float>(this.UpdatePosition));
		}
	}

	// Token: 0x060040AD RID: 16557 RVA: 0x0011C428 File Offset: 0x0011A628
	private void UpdatePosition(float value)
	{
		this.trans.pivot = new Vector2(0.5f, value);
		this.trans.anchorMin = new Vector2(0.5f, value);
		this.trans.anchorMax = new Vector2(0.5f, value);
		this.trans.anchoredPosition.Set(this.trans.anchoredPosition.x, 0f);
	}

	// Token: 0x0400422F RID: 16943
	public Scrollbar scrollBar;

	// Token: 0x04004230 RID: 16944
	private RectTransform trans;
}
