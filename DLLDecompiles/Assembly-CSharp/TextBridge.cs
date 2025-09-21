using System;
using TMProOld;
using UnityEngine;

// Token: 0x02000733 RID: 1843
public class TextBridge : MonoBehaviour
{
	// Token: 0x1700077D RID: 1917
	// (get) Token: 0x060041E1 RID: 16865 RVA: 0x00121F04 File Offset: 0x00120104
	// (set) Token: 0x060041E2 RID: 16866 RVA: 0x00121F40 File Offset: 0x00120140
	public string Text
	{
		get
		{
			if (this.textMesh)
			{
				return this.textMesh.text;
			}
			if (this.tmpText)
			{
				return this.tmpText.text;
			}
			return string.Empty;
		}
		set
		{
			if (this.textMesh && this.textMesh.text != value)
			{
				this.textMesh.text = value;
			}
			if (this.tmpText)
			{
				this.tmpText.text = value;
			}
		}
	}

	// Token: 0x1700077E RID: 1918
	// (get) Token: 0x060041E3 RID: 16867 RVA: 0x00121F92 File Offset: 0x00120192
	// (set) Token: 0x060041E4 RID: 16868 RVA: 0x00121FCC File Offset: 0x001201CC
	public Color Color
	{
		get
		{
			if (this.textMesh)
			{
				return this.textMesh.color;
			}
			if (this.tmpText)
			{
				return this.tmpText.color;
			}
			return Color.white;
		}
		set
		{
			if (this.textMesh && this.textMesh.color != value)
			{
				this.textMesh.color = value;
			}
			if (this.tmpText)
			{
				this.tmpText.color = value;
			}
		}
	}

	// Token: 0x060041E5 RID: 16869 RVA: 0x0012201E File Offset: 0x0012021E
	public void FindComponent()
	{
		if (this.textMesh == null)
		{
			this.textMesh = base.GetComponent<TextMesh>();
		}
		if (this.tmpText == null)
		{
			this.tmpText = base.GetComponent<TMP_Text>();
		}
	}

	// Token: 0x0400436E RID: 17262
	[SerializeField]
	private TextMesh textMesh;

	// Token: 0x0400436F RID: 17263
	[SerializeField]
	private TMP_Text tmpText;
}
