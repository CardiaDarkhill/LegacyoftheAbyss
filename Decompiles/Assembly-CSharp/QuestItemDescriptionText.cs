using System;
using TMProOld;
using UnityEngine;

// Token: 0x020005A0 RID: 1440
public class QuestItemDescriptionText : MonoBehaviour
{
	// Token: 0x060033BF RID: 13247 RVA: 0x000E6EBC File Offset: 0x000E50BC
	private void Awake()
	{
		this.GetInitialValues();
	}

	// Token: 0x060033C0 RID: 13248 RVA: 0x000E6EC4 File Offset: 0x000E50C4
	private void GetInitialValues()
	{
		if (this.gotInitialValues)
		{
			return;
		}
		this.gotInitialValues = true;
		if (this.rangeText)
		{
			this.initialRangeText = this.rangeText.text;
		}
		if (this.rangeTextIcon)
		{
			this.initialRangeIconScale = this.rangeTextIcon.transform.localScale;
		}
	}

	// Token: 0x060033C1 RID: 13249 RVA: 0x000E6F24 File Offset: 0x000E5124
	public void ResetDisplay()
	{
		this.GetInitialValues();
		if (this.rangeText)
		{
			this.rangeText.text = this.initialRangeText;
		}
		if (this.rangeTextIcon)
		{
			this.rangeTextIcon.transform.localScale = this.initialRangeIconScale;
		}
	}

	// Token: 0x060033C2 RID: 13250 RVA: 0x000E6F78 File Offset: 0x000E5178
	public void SetDisplay(FullQuestBase fullQuest, FullQuestBase.QuestTarget target, int counter)
	{
		this.GetInitialValues();
		if (this.rangeText)
		{
			this.rangeText.text = string.Format(this.initialRangeText, counter, target.Count);
		}
		if (this.rangeTextIcon)
		{
			this.rangeTextIcon.sprite = fullQuest.GetCounterSpriteOverride(target, 0);
			float counterIconScale = fullQuest.CounterIconScale;
			this.rangeTextIcon.transform.localScale = new Vector3(counterIconScale, counterIconScale, 1f).MultiplyElements(this.initialRangeIconScale);
		}
	}

	// Token: 0x04003784 RID: 14212
	[SerializeField]
	private TMP_Text rangeText;

	// Token: 0x04003785 RID: 14213
	[SerializeField]
	private SpriteRenderer rangeTextIcon;

	// Token: 0x04003786 RID: 14214
	private bool gotInitialValues;

	// Token: 0x04003787 RID: 14215
	private string initialRangeText;

	// Token: 0x04003788 RID: 14216
	private Vector3 initialRangeIconScale;
}
