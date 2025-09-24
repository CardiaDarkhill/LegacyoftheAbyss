using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

// Token: 0x0200071C RID: 1820
public class SetTextMeshProGameText : MonoBehaviour
{
	// Token: 0x17000756 RID: 1878
	// (get) Token: 0x060040B4 RID: 16564 RVA: 0x0011C5FC File Offset: 0x0011A7FC
	// (set) Token: 0x060040B5 RID: 16565 RVA: 0x0011C604 File Offset: 0x0011A804
	public LocalisedString Text
	{
		get
		{
			return this.text;
		}
		set
		{
			this.text = value;
			this.UpdateText();
		}
	}

	// Token: 0x060040B6 RID: 16566 RVA: 0x0011C614 File Offset: 0x0011A814
	private void OnValidate()
	{
		if (this.text.IsEmpty && (!string.IsNullOrEmpty(this.sheetName) || !string.IsNullOrEmpty(this.convName)))
		{
			this.text.Sheet = this.sheetName;
			this.text.Key = this.convName;
			this.sheetName = string.Empty;
			this.convName = string.Empty;
		}
	}

	// Token: 0x060040B7 RID: 16567 RVA: 0x0011C680 File Offset: 0x0011A880
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060040B8 RID: 16568 RVA: 0x0011C688 File Offset: 0x0011A888
	private void Start()
	{
		this.started = true;
		this.UpdateText();
	}

	// Token: 0x060040B9 RID: 16569 RVA: 0x0011C698 File Offset: 0x0011A898
	private void Init()
	{
		if (this.init)
		{
			return;
		}
		this.init = true;
		this.OnValidate();
		this.CleanSplitLines();
		if (this.splitLinesAcross.Count == 0 && !this.setTextOn)
		{
			this.setTextOn = base.GetComponent<TextMeshPro>();
		}
	}

	// Token: 0x060040BA RID: 16570 RVA: 0x0011C6E7 File Offset: 0x0011A8E7
	private void OnEnable()
	{
		if (CheatManager.ForceLanguageComponentUpdates && this.started)
		{
			this.UpdateText();
		}
	}

	// Token: 0x060040BB RID: 16571 RVA: 0x0011C6FE File Offset: 0x0011A8FE
	private void CleanSplitLines()
	{
		this.splitLinesAcross.RemoveAll((TMP_Text o) => o == null);
	}

	// Token: 0x060040BC RID: 16572 RVA: 0x0011C72C File Offset: 0x0011A92C
	[ContextMenu("Update Text")]
	public void UpdateText()
	{
		this.Init();
		string text = this.toSingleLine ? this.text.ToString().ToSingleLine() : this.text;
		if (this.setTextOn)
		{
			this.setTextOn.text = text;
		}
		if (this.splitLinesAcross.Count > 0)
		{
			string[] array = text.Split('\n', StringSplitOptions.None);
			this.CleanSplitLines();
			if (this.splitLinesAcross.Count == 0)
			{
				return;
			}
			StringBuilder[] array2 = new StringBuilder[this.splitLinesAcross.Count];
			int num = 0;
			foreach (string value in array)
			{
				StringBuilder stringBuilder = array2[num];
				if (stringBuilder == null)
				{
					stringBuilder = (array2[num] = new StringBuilder());
				}
				stringBuilder.AppendLine(value);
				num++;
				if (num >= this.splitLinesAcross.Count)
				{
					num = 0;
				}
			}
			for (int j = 0; j < array2.Length; j++)
			{
				StringBuilder stringBuilder2 = array2[j];
				if (stringBuilder2 != null)
				{
					this.splitLinesAcross[j].text = stringBuilder2.ToString();
				}
			}
		}
	}

	// Token: 0x0400423B RID: 16955
	[SerializeField]
	private LocalisedString text;

	// Token: 0x0400423C RID: 16956
	[SerializeField]
	private bool toSingleLine;

	// Token: 0x0400423D RID: 16957
	[SerializeField]
	private TMP_Text setTextOn;

	// Token: 0x0400423E RID: 16958
	[SerializeField]
	private List<TMP_Text> splitLinesAcross;

	// Token: 0x0400423F RID: 16959
	[HideInInspector]
	public string sheetName;

	// Token: 0x04004240 RID: 16960
	[HideInInspector]
	public string convName;

	// Token: 0x04004241 RID: 16961
	private bool started;

	// Token: 0x04004242 RID: 16962
	private bool init;
}
