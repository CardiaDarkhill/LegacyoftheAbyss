using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200034E RID: 846
[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class FixVerticalAlign : MonoBehaviour
{
	// Token: 0x06001D4C RID: 7500 RVA: 0x000875B3 File Offset: 0x000857B3
	private void OnEnable()
	{
		this.AlignAuto();
	}

	// Token: 0x06001D4D RID: 7501 RVA: 0x000875BB File Offset: 0x000857BB
	private void Start()
	{
		this.AlignAuto();
	}

	// Token: 0x06001D4E RID: 7502 RVA: 0x000875C3 File Offset: 0x000857C3
	private void FindText()
	{
		if (!this.hasText)
		{
			this.text = base.GetComponent<Text>();
			this.hasText = this.text;
		}
	}

	// Token: 0x06001D4F RID: 7503 RVA: 0x000875EA File Offset: 0x000857EA
	public void AlignAuto()
	{
		if (this.labelFixType == FixVerticalAlign.LabelFixType.Normal)
		{
			this.AlignText();
			return;
		}
		if (this.labelFixType == FixVerticalAlign.LabelFixType.KeyMap)
		{
			this.AlignTextKeymap();
		}
	}

	// Token: 0x06001D50 RID: 7504 RVA: 0x0008760C File Offset: 0x0008580C
	public void AlignText()
	{
		this.FindText();
		if (!string.IsNullOrEmpty(this.text.text))
		{
			if (this.text.text[this.text.text.Length - 1] != '\n')
			{
				Text text = this.text;
				text.text += "\n";
			}
			this.text.lineSpacing = -0.33f;
		}
	}

	// Token: 0x06001D51 RID: 7505 RVA: 0x00087684 File Offset: 0x00085884
	public void AlignTextKeymap()
	{
		this.FindText();
		if (!string.IsNullOrEmpty(this.text.text))
		{
			if (this.text.text[this.text.text.Length - 1] != '\n')
			{
				Text text = this.text;
				text.text += "\n";
			}
			this.text.lineSpacing = -0.05f;
		}
	}

	// Token: 0x04001C87 RID: 7303
	private Text text;

	// Token: 0x04001C88 RID: 7304
	private bool hasText;

	// Token: 0x04001C89 RID: 7305
	public FixVerticalAlign.LabelFixType labelFixType;

	// Token: 0x0200160B RID: 5643
	public enum LabelFixType
	{
		// Token: 0x04008987 RID: 35207
		Normal,
		// Token: 0x04008988 RID: 35208
		KeyMap
	}
}
