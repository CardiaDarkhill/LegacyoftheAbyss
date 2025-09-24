using System;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000736 RID: 1846
[ExecuteInEditMode]
[RequireComponent(typeof(TextContainer), typeof(TMP_Text))]
public class TextMeshProContainerFitter : MonoBehaviour, ILayoutController
{
	// Token: 0x060041F3 RID: 16883 RVA: 0x001222D0 File Offset: 0x001204D0
	private void Awake()
	{
		this.GetComponents();
	}

	// Token: 0x060041F4 RID: 16884 RVA: 0x001222D8 File Offset: 0x001204D8
	private void Start()
	{
		this.UpdateValues(true, true);
	}

	// Token: 0x060041F5 RID: 16885 RVA: 0x001222E2 File Offset: 0x001204E2
	private void Update()
	{
		if (!this.text.text.Equals(this.previousText) || this.preferredWidth != this.previousPreferredWidth || this.preferredHeight != this.previousPreferredHeight)
		{
			this.UpdateValues(true, true);
		}
	}

	// Token: 0x060041F6 RID: 16886 RVA: 0x00122320 File Offset: 0x00120520
	private void GetComponents()
	{
		if (!this.text)
		{
			this.text = base.GetComponent<TMP_Text>();
		}
		if (!this.container)
		{
			this.container = base.GetComponent<TextContainer>();
		}
	}

	// Token: 0x060041F7 RID: 16887 RVA: 0x00122354 File Offset: 0x00120554
	private void UpdateValues(bool doWidth, bool doHeight)
	{
		this.GetComponents();
		this.previousText = this.text.text;
		this.previousPreferredHeight = this.preferredHeight;
		this.previousPreferredWidth = this.preferredWidth;
		Vector2 size = this.container.size;
		Vector2 preferredValues = this.text.GetPreferredValues(this.previousText, (doWidth && this.preferredWidth) ? float.PositiveInfinity : size.x, (doHeight && this.preferredHeight) ? float.PositiveInfinity : size.y);
		if (this.preferredWidth)
		{
			size.x = preferredValues.x;
		}
		if (this.preferredHeight)
		{
			size.y = preferredValues.y;
		}
		if (this.layoutElement)
		{
			if (this.preferredWidth)
			{
				this.layoutElement.minWidth = size.x;
			}
			if (this.preferredHeight)
			{
				this.layoutElement.minHeight = size.y;
				return;
			}
		}
		else
		{
			this.container.size = size;
		}
	}

	// Token: 0x060041F8 RID: 16888 RVA: 0x00122454 File Offset: 0x00120654
	public void SetLayoutHorizontal()
	{
		this.UpdateValues(true, false);
	}

	// Token: 0x060041F9 RID: 16889 RVA: 0x0012245E File Offset: 0x0012065E
	public void SetLayoutVertical()
	{
		this.UpdateValues(false, true);
	}

	// Token: 0x060041FA RID: 16890 RVA: 0x00122468 File Offset: 0x00120668
	public void SetLayoutAll()
	{
		this.UpdateValues(true, true);
	}

	// Token: 0x04004379 RID: 17273
	[SerializeField]
	private bool preferredWidth;

	// Token: 0x0400437A RID: 17274
	[SerializeField]
	private bool preferredHeight;

	// Token: 0x0400437B RID: 17275
	[SerializeField]
	private LayoutElement layoutElement;

	// Token: 0x0400437C RID: 17276
	private TMP_Text text;

	// Token: 0x0400437D RID: 17277
	private TextContainer container;

	// Token: 0x0400437E RID: 17278
	private string previousText;

	// Token: 0x0400437F RID: 17279
	private bool previousPreferredWidth;

	// Token: 0x04004380 RID: 17280
	private bool previousPreferredHeight;
}
