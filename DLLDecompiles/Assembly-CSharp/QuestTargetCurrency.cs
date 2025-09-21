using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020005A7 RID: 1447
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target Currency")]
public class QuestTargetCurrency : QuestTargetCounter
{
	// Token: 0x170005B9 RID: 1465
	// (get) Token: 0x0600340A RID: 13322 RVA: 0x000E7E41 File Offset: 0x000E6041
	public CurrencyType CurrencyType
	{
		get
		{
			return this.currencyType;
		}
	}

	// Token: 0x170005BA RID: 1466
	// (get) Token: 0x0600340B RID: 13323 RVA: 0x000E7E49 File Offset: 0x000E6049
	public string GivePromptText
	{
		get
		{
			return this.givePromptText;
		}
	}

	// Token: 0x170005BB RID: 1467
	// (get) Token: 0x0600340C RID: 13324 RVA: 0x000E7E56 File Offset: 0x000E6056
	protected override bool ShowCounterOnConsume
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600340D RID: 13325 RVA: 0x000E7E59 File Offset: 0x000E6059
	public override bool CanGetMore()
	{
		return true;
	}

	// Token: 0x0600340E RID: 13326 RVA: 0x000E7E5C File Offset: 0x000E605C
	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return CurrencyManager.GetCurrencyAmount(this.currencyType);
	}

	// Token: 0x170005BC RID: 1468
	// (get) Token: 0x0600340F RID: 13327 RVA: 0x000E7E69 File Offset: 0x000E6069
	public override bool CanConsume
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003410 RID: 13328 RVA: 0x000E7E6C File Offset: 0x000E606C
	public override void Consume(int amount, bool showCounter)
	{
		CurrencyManager.TakeCurrency(amount, this.currencyType, showCounter);
	}

	// Token: 0x06003411 RID: 13329 RVA: 0x000E7E7B File Offset: 0x000E607B
	public override Sprite GetPopupIcon()
	{
		return this.questCounterSprite;
	}

	// Token: 0x040037A4 RID: 14244
	[SerializeField]
	private CurrencyType currencyType;

	// Token: 0x040037A5 RID: 14245
	[SerializeField]
	private LocalisedString givePromptText;

	// Token: 0x040037A6 RID: 14246
	[SerializeField]
	private Sprite questCounterSprite;
}
