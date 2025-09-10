using System;
using TMProOld;
using UnityEngine;

// Token: 0x02000701 RID: 1793
public class ReplaceTextLineBreaks : MonoBehaviour
{
	// Token: 0x06004011 RID: 16401 RVA: 0x0011A608 File Offset: 0x00118808
	private void Start()
	{
		this.textMesh = base.GetComponent<TextMeshPro>();
		string text = this.textMesh.text;
		text = text.Replace("<br>", "\n");
		this.textMesh.text = text;
	}

	// Token: 0x040041BF RID: 16831
	private TextMeshPro textMesh;
}
