using System;
using TMProOld;
using UnityEngine;

// Token: 0x02000735 RID: 1845
public sealed class TextMeshProColorPreserver : MonoBehaviour
{
	// Token: 0x060041EE RID: 16878 RVA: 0x001221EB File Offset: 0x001203EB
	private void Awake()
	{
		if (this.tmpText == null)
		{
			this.tmpText = base.GetComponent<TextMeshPro>();
		}
		if (this.tmpText != null)
		{
			this.tmpText.color = this.targetColor;
		}
	}

	// Token: 0x060041EF RID: 16879 RVA: 0x00122226 File Offset: 0x00120426
	private void Reset()
	{
		if (this.tmpText == null)
		{
			this.tmpText = base.GetComponent<TextMeshPro>();
		}
		if (this.tmpText != null)
		{
			this.targetColor = this.tmpText.color;
		}
	}

	// Token: 0x060041F0 RID: 16880 RVA: 0x00122261 File Offset: 0x00120461
	private void OnValidate()
	{
		if (this.tmpText == null)
		{
			this.tmpText = base.GetComponent<TextMeshPro>();
		}
		if (this.tmpText != null)
		{
			this.tmpText.color = this.targetColor;
		}
	}

	// Token: 0x060041F1 RID: 16881 RVA: 0x0012229C File Offset: 0x0012049C
	private void OnEnable()
	{
		if (this.tmpText != null)
		{
			this.tmpText.color = this.targetColor;
		}
	}

	// Token: 0x04004377 RID: 17271
	[SerializeField]
	private TextMeshPro tmpText;

	// Token: 0x04004378 RID: 17272
	[SerializeField]
	private Color targetColor = Color.white;
}
