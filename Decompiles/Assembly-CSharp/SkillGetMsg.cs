using System;
using System.Collections.Generic;
using TMProOld;
using UnityEngine;

// Token: 0x0200072E RID: 1838
public class SkillGetMsg : UIMsgBase<ToolItemSkill>
{
	// Token: 0x060041B2 RID: 16818 RVA: 0x001211F9 File Offset: 0x0011F3F9
	protected override int GetHideAnimId()
	{
		if (!this.wasEquipped)
		{
			return SkillGetMsg._hideNoEquipProp;
		}
		return base.GetHideAnimId();
	}

	// Token: 0x060041B3 RID: 16819 RVA: 0x00121210 File Offset: 0x0011F410
	protected override void Setup(ToolItemSkill skill)
	{
		this.skillSprite.sprite = skill.PromptSprite;
		this.skillGlowSprite.sprite = skill.PromptGlowSprite;
		this.skillSilhouetteSprite.sprite = skill.PromptSilhouetteSprite;
		this.skillIconSprite.sprite = skill.InventorySpriteBase;
		this.prefixText.text = skill.MsgGetPrefix;
		this.nameText.text = skill.DisplayName;
		this.descText.text = skill.PromptDescription;
		ToolCrest crestByName = ToolItemManager.GetCrestByName(PlayerData.instance.CurrentCrestID);
		if (!crestByName)
		{
			return;
		}
		this.crestSprite.sprite = crestByName.CrestSprite;
		this.crestGlowSprite.sprite = crestByName.CrestGlow;
		int num = -1;
		List<ToolCrestsData.SlotData> slots = crestByName.SaveData.Slots;
		for (int i = 0; i < slots.Count; i++)
		{
			if (!(slots[i].EquippedTool != skill.name))
			{
				num = i;
				break;
			}
		}
		this.wasEquipped = (num >= 0 && num < crestByName.Slots.Length);
		if (!this.wasEquipped)
		{
			return;
		}
		ToolCrest.SlotInfo slotInfo = crestByName.Slots[num];
		this.crestGroup.SetLocalPosition2D(this.baseCrestPos - slotInfo.Position);
	}

	// Token: 0x060041B4 RID: 16820 RVA: 0x0012136C File Offset: 0x0011F56C
	public static void Spawn(SkillGetMsg prefab, ToolItemSkill skill, Action afterMsg)
	{
		SkillGetMsg.<>c__DisplayClass15_0 CS$<>8__locals1 = new SkillGetMsg.<>c__DisplayClass15_0();
		CS$<>8__locals1.afterMsg = afterMsg;
		CS$<>8__locals1.msg = null;
		PlayerData.instance.ToolPaneHasNew = true;
		CS$<>8__locals1.msg = (UIMsgBase<ToolItemSkill>.Spawn(skill, prefab, new Action(CS$<>8__locals1.<Spawn>g__AfterMsg|0)) as SkillGetMsg);
		if (!CS$<>8__locals1.msg)
		{
			return;
		}
		CS$<>8__locals1.msg.transform.SetLocalPosition2D(Vector2.zero);
		GameCameras.instance.HUDOut();
		HeroController.instance.AddInputBlocker(CS$<>8__locals1.msg);
	}

	// Token: 0x060041B5 RID: 16821 RVA: 0x001213F3 File Offset: 0x0011F5F3
	public void ReportBackgroundFadedOut()
	{
		EventRegister.SendEvent("SKILL GET MSG FADED OUT", null);
	}

	// Token: 0x04004339 RID: 17209
	[Space]
	[SerializeField]
	private Transform crestGroup;

	// Token: 0x0400433A RID: 17210
	[SerializeField]
	private Vector2 baseCrestPos;

	// Token: 0x0400433B RID: 17211
	[SerializeField]
	private SpriteRenderer crestSprite;

	// Token: 0x0400433C RID: 17212
	[SerializeField]
	private SpriteRenderer crestGlowSprite;

	// Token: 0x0400433D RID: 17213
	[Space]
	[SerializeField]
	private SpriteRenderer skillSprite;

	// Token: 0x0400433E RID: 17214
	[SerializeField]
	private SpriteRenderer skillGlowSprite;

	// Token: 0x0400433F RID: 17215
	[SerializeField]
	private SpriteRenderer skillSilhouetteSprite;

	// Token: 0x04004340 RID: 17216
	[SerializeField]
	private SpriteRenderer skillIconSprite;

	// Token: 0x04004341 RID: 17217
	[Space]
	[SerializeField]
	private TMP_Text prefixText;

	// Token: 0x04004342 RID: 17218
	[SerializeField]
	private TMP_Text nameText;

	// Token: 0x04004343 RID: 17219
	[SerializeField]
	private TMP_Text descText;

	// Token: 0x04004344 RID: 17220
	private bool wasEquipped;

	// Token: 0x04004345 RID: 17221
	private static readonly int _hideNoEquipProp = Animator.StringToHash("Hide NoEquip");
}
