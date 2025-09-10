using System;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000737 RID: 1847
public sealed class TextMeshUpdateMinHeight : MonoBehaviour
{
	// Token: 0x060041FC RID: 16892 RVA: 0x0012247A File Offset: 0x0012067A
	private void Start()
	{
		this.started = true;
		this.UpdateMinValue();
	}

	// Token: 0x060041FD RID: 16893 RVA: 0x00122489 File Offset: 0x00120689
	private void OnEnable()
	{
		if (this.started)
		{
			this.UpdateMinValue();
		}
	}

	// Token: 0x060041FE RID: 16894 RVA: 0x00122499 File Offset: 0x00120699
	private void OnValidate()
	{
		if (this.textMeshPro == null)
		{
			this.textMeshPro = base.GetComponent<TextMeshPro>();
		}
		if (this.layoutElement == null)
		{
			this.layoutElement = base.GetComponent<LayoutElement>();
		}
	}

	// Token: 0x060041FF RID: 16895 RVA: 0x001224D0 File Offset: 0x001206D0
	private void UpdateMinValue()
	{
		if (this.textMeshPro && this.layoutElement)
		{
			Vector2 preferredValues = this.textMeshPro.GetPreferredValues();
			this.layoutElement.minHeight = preferredValues.y;
			Debug.Log(string.Format("{0} Setting height to {1} for {2}", this, this.layoutElement.minHeight, this.textMeshPro.text), this);
		}
	}

	// Token: 0x04004381 RID: 17281
	[SerializeField]
	private TextMeshPro textMeshPro;

	// Token: 0x04004382 RID: 17282
	[SerializeField]
	private LayoutElement layoutElement;

	// Token: 0x04004383 RID: 17283
	private bool started;
}
