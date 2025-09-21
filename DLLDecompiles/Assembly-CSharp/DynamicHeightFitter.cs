using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000669 RID: 1641
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public sealed class DynamicHeightFitter : MonoBehaviour, ILayoutElement, IInitialisable
{
	// Token: 0x06003AD1 RID: 15057 RVA: 0x00103274 File Offset: 0x00101474
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.rectTransform = base.GetComponent<RectTransform>();
		if (this.parentRect == null)
		{
			RectTransform rectTransform = base.transform.parent as RectTransform;
			if (rectTransform != null)
			{
				this.parentRect = rectTransform;
			}
		}
		this.calculatedHeight = this.maxElementHeight;
		return true;
	}

	// Token: 0x06003AD2 RID: 15058 RVA: 0x001032D4 File Offset: 0x001014D4
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x06003AD3 RID: 15059 RVA: 0x001032EF File Offset: 0x001014EF
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x06003AD4 RID: 15060 RVA: 0x001032F8 File Offset: 0x001014F8
	private void OnEnable()
	{
		this.UpdateHeight();
	}

	// Token: 0x06003AD5 RID: 15061 RVA: 0x00103300 File Offset: 0x00101500
	private void UpdateHeight()
	{
		this.OnAwake();
		if (this.parentRect == null)
		{
			Debug.LogWarning("Parent RectTransform not assigned.");
			return;
		}
		VerticalLayoutGroup component = this.parentRect.GetComponent<VerticalLayoutGroup>();
		float num = 0f;
		float num2 = 0f;
		if (component != null)
		{
			num = component.spacing;
			num2 = (float)(component.padding.top + component.padding.bottom);
		}
		float num3 = this.parentRect.rect.height - num2;
		float num4 = 0f;
		int num5 = 0;
		foreach (object obj in this.parentRect)
		{
			RectTransform rectTransform = (RectTransform)obj;
			if (!(rectTransform == this.rectTransform))
			{
				num5++;
				ILayoutElement component2 = rectTransform.GetComponent<ILayoutElement>();
				if (component2 != null)
				{
					num4 += Mathf.Max(component2.minHeight, component2.preferredHeight);
				}
				else
				{
					num4 += rectTransform.rect.height;
				}
			}
		}
		num4 += num * (float)Mathf.Max(0, num5 - 1);
		float num6 = num3 - num4;
		if (num5 > 0)
		{
			num6 -= this.spacing;
		}
		this.calculatedHeight = Mathf.Clamp(num6, this.minElementHeight, this.maxElementHeight);
		this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.calculatedHeight);
	}

	// Token: 0x06003AD6 RID: 15062 RVA: 0x00103480 File Offset: 0x00101680
	public void CalculateLayoutInputVertical()
	{
		this.UpdateHeight();
	}

	// Token: 0x06003AD7 RID: 15063 RVA: 0x00103488 File Offset: 0x00101688
	public void CalculateLayoutInputHorizontal()
	{
	}

	// Token: 0x1700069D RID: 1693
	// (get) Token: 0x06003AD8 RID: 15064 RVA: 0x0010348A File Offset: 0x0010168A
	public float minHeight
	{
		get
		{
			return this.minElementHeight;
		}
	}

	// Token: 0x1700069E RID: 1694
	// (get) Token: 0x06003AD9 RID: 15065 RVA: 0x00103492 File Offset: 0x00101692
	public float preferredHeight
	{
		get
		{
			return this.calculatedHeight;
		}
	}

	// Token: 0x1700069F RID: 1695
	// (get) Token: 0x06003ADA RID: 15066 RVA: 0x0010349A File Offset: 0x0010169A
	public float flexibleHeight
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x170006A0 RID: 1696
	// (get) Token: 0x06003ADB RID: 15067 RVA: 0x001034A1 File Offset: 0x001016A1
	public float minWidth
	{
		get
		{
			return -1f;
		}
	}

	// Token: 0x170006A1 RID: 1697
	// (get) Token: 0x06003ADC RID: 15068 RVA: 0x001034A8 File Offset: 0x001016A8
	public float preferredWidth
	{
		get
		{
			return -1f;
		}
	}

	// Token: 0x170006A2 RID: 1698
	// (get) Token: 0x06003ADD RID: 15069 RVA: 0x001034AF File Offset: 0x001016AF
	public float flexibleWidth
	{
		get
		{
			return -1f;
		}
	}

	// Token: 0x170006A3 RID: 1699
	// (get) Token: 0x06003ADE RID: 15070 RVA: 0x001034B6 File Offset: 0x001016B6
	public int layoutPriority
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x06003ADF RID: 15071 RVA: 0x001034B9 File Offset: 0x001016B9
	private void OnValidate()
	{
		if (this.minElementHeight > this.maxElementHeight)
		{
			Debug.LogWarning("MinHeight cannot be greater than MaxHeight. Adjusting values.");
			this.minElementHeight = this.maxElementHeight;
		}
		if (!Application.isPlaying)
		{
			this.UpdateHeight();
		}
	}

	// Token: 0x06003AE1 RID: 15073 RVA: 0x00103515 File Offset: 0x00101715
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04003D34 RID: 15668
	[SerializeField]
	private float spacing = 1f;

	// Token: 0x04003D35 RID: 15669
	[SerializeField]
	private RectTransform parentRect;

	// Token: 0x04003D36 RID: 15670
	[SerializeField]
	private float minElementHeight = 100f;

	// Token: 0x04003D37 RID: 15671
	[SerializeField]
	private float maxElementHeight = 300f;

	// Token: 0x04003D38 RID: 15672
	private RectTransform rectTransform;

	// Token: 0x04003D39 RID: 15673
	private float calculatedHeight;

	// Token: 0x04003D3A RID: 15674
	private bool hasAwaken;

	// Token: 0x04003D3B RID: 15675
	private bool hasStarted;
}
