using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000402 RID: 1026
public class AutoLocalizeTextUI : MonoBehaviour
{
	// Token: 0x17000399 RID: 921
	// (get) Token: 0x060022CD RID: 8909 RVA: 0x0009FD04 File Offset: 0x0009DF04
	// (set) Token: 0x060022CE RID: 8910 RVA: 0x0009FD0C File Offset: 0x0009DF0C
	public LocalisedString Text
	{
		get
		{
			return this.text;
		}
		set
		{
			this.text = value;
			this.RefreshTextFromLocalization();
		}
	}

	// Token: 0x1700039A RID: 922
	// (get) Token: 0x060022CF RID: 8911 RVA: 0x0009FD1B File Offset: 0x0009DF1B
	// (set) Token: 0x060022D0 RID: 8912 RVA: 0x0009FD28 File Offset: 0x0009DF28
	public string TextSheet
	{
		get
		{
			return this.Text.Sheet;
		}
		set
		{
			LocalisedString localisedString = this.Text;
			localisedString.Sheet = value;
			this.Text = localisedString;
		}
	}

	// Token: 0x1700039B RID: 923
	// (get) Token: 0x060022D1 RID: 8913 RVA: 0x0009FD4B File Offset: 0x0009DF4B
	// (set) Token: 0x060022D2 RID: 8914 RVA: 0x0009FD58 File Offset: 0x0009DF58
	public string TextKey
	{
		get
		{
			return this.Text.Key;
		}
		set
		{
			LocalisedString localisedString = this.Text;
			localisedString.Key = value;
			this.Text = localisedString;
		}
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x0009FD7C File Offset: 0x0009DF7C
	private void OnValidate()
	{
		if (!string.IsNullOrEmpty(this.sheetTitle) || !string.IsNullOrEmpty(this.textKey))
		{
			this.text = new LocalisedString(this.sheetTitle, this.textKey);
			this.sheetTitle = string.Empty;
			this.textKey = string.Empty;
		}
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x0009FDD0 File Offset: 0x0009DFD0
	private void Awake()
	{
		this.OnValidate();
		this.textAligner = base.GetComponent<FixVerticalAlign>();
		if (this.textAligner)
		{
			this.hasTextAligner = true;
		}
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x0009FDF8 File Offset: 0x0009DFF8
	private void OnEnable()
	{
		this.gm = GameManager.instance;
		if (this.gm)
		{
			this.gm.RefreshLanguageText += this.RefreshTextFromLocalization;
		}
		this.RefreshTextFromLocalization();
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x0009FE2F File Offset: 0x0009E02F
	private void OnDisable()
	{
		if (this.gm != null)
		{
			this.gm.RefreshLanguageText -= this.RefreshTextFromLocalization;
		}
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x0009FE56 File Offset: 0x0009E056
	public void RefreshTextFromLocalization()
	{
		this.textField.text = this.text;
		if (this.hasTextAligner)
		{
			this.textAligner.AlignText();
		}
	}

	// Token: 0x0400219E RID: 8606
	[SerializeField]
	[Tooltip("UI Text component to place text.")]
	private Text textField;

	// Token: 0x0400219F RID: 8607
	[SerializeField]
	private LocalisedString text;

	// Token: 0x040021A0 RID: 8608
	[SerializeField]
	[HideInInspector]
	private string sheetTitle;

	// Token: 0x040021A1 RID: 8609
	[SerializeField]
	[HideInInspector]
	private string textKey;

	// Token: 0x040021A2 RID: 8610
	private GameManager gm;

	// Token: 0x040021A3 RID: 8611
	private FixVerticalAlign textAligner;

	// Token: 0x040021A4 RID: 8612
	private bool hasTextAligner;
}
