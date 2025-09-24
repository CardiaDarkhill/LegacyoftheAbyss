using System;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x02000687 RID: 1671
public class InventoryItemButtonPromptDisplay : MonoBehaviour, InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder
{
	// Token: 0x170006BE RID: 1726
	// (get) Token: 0x06003BAB RID: 15275 RVA: 0x001066B3 File Offset: 0x001048B3
	// (set) Token: 0x06003BAC RID: 15276 RVA: 0x001066BB File Offset: 0x001048BB
	public int order { get; set; }

	// Token: 0x06003BAD RID: 15277 RVA: 0x001066C4 File Offset: 0x001048C4
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003BAE RID: 15278 RVA: 0x001066D4 File Offset: 0x001048D4
	public void Show(InventoryItemButtonPromptData source, bool forceDisabled)
	{
		if (this.icon)
		{
			this.icon.SetAction(source.MenuAction);
		}
		if (this.useText)
		{
			this.useText.text = (source.UseText.IsEmpty ? string.Empty : source.UseText);
		}
		if (this.responseText)
		{
			this.responseText.text = source.ResponseText;
		}
		bool flag = !source.ConditionText.IsEmpty;
		if (this.mainGroup)
		{
			this.mainGroup.AlphaSelf = ((flag || forceDisabled) ? this.disabledGroupAlpha : 1f);
		}
		if (this.conditionText)
		{
			this.conditionText.text = (flag ? source.ConditionText : string.Empty);
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003BB0 RID: 15280 RVA: 0x001067D6 File Offset: 0x001049D6
	Transform InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder.get_transform()
	{
		return base.transform;
	}

	// Token: 0x04003DDD RID: 15837
	[SerializeField]
	private ActionButtonIcon icon;

	// Token: 0x04003DDE RID: 15838
	[SerializeField]
	private TMP_Text useText;

	// Token: 0x04003DDF RID: 15839
	[SerializeField]
	private TMP_Text responseText;

	// Token: 0x04003DE0 RID: 15840
	[SerializeField]
	private NestedFadeGroupBase mainGroup;

	// Token: 0x04003DE1 RID: 15841
	[SerializeField]
	[Range(0f, 1f)]
	private float disabledGroupAlpha;

	// Token: 0x04003DE2 RID: 15842
	[SerializeField]
	private TMP_Text conditionText;
}
