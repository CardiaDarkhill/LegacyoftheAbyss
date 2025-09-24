using System;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using TMProOld;
using UnityEngine;

// Token: 0x020006C8 RID: 1736
public class LiquidMeter : MonoBehaviour
{
	// Token: 0x06003EC3 RID: 16067 RVA: 0x001142F0 File Offset: 0x001124F0
	public void SetDisplay(ToolItemStatesLiquid toolItem)
	{
		bool hasInfiniteRefills = toolItem.HasInfiniteRefills;
		this.liquidParent.SetActive(!hasInfiniteRefills);
		this.glandParent.SetActive(hasInfiniteRefills);
		if (hasInfiniteRefills)
		{
			return;
		}
		int refillsMax = toolItem.RefillsMax;
		int refillsLeft = toolItem.LiquidSavedData.RefillsLeft;
		float t = (float)refillsLeft / (float)refillsMax;
		this.liquidSprite.Color = toolItem.LiquidColor;
		if (refillsLeft <= 0)
		{
			this.liquidSprite.AlphaSelf = 0f;
		}
		this.liquidOffset.SetLocalPositionY(this.posRangeY.GetLerpedValue(t));
		if (this.countFormatText == null)
		{
			this.countFormatText = this.countText.text;
		}
		this.countText.text = string.Format(this.countFormatText, refillsLeft, refillsMax);
	}

	// Token: 0x0400405D RID: 16477
	[SerializeField]
	private GameObject liquidParent;

	// Token: 0x0400405E RID: 16478
	[SerializeField]
	private NestedFadeGroupSpriteRenderer liquidSprite;

	// Token: 0x0400405F RID: 16479
	[SerializeField]
	private Transform liquidOffset;

	// Token: 0x04004060 RID: 16480
	[SerializeField]
	private MinMaxFloat posRangeY;

	// Token: 0x04004061 RID: 16481
	[SerializeField]
	private TMP_Text countText;

	// Token: 0x04004062 RID: 16482
	[SerializeField]
	private GameObject glandParent;

	// Token: 0x04004063 RID: 16483
	private string countFormatText;
}
