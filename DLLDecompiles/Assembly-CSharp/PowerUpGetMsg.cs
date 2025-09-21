using System;
using GlobalEnums;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

// Token: 0x020006F7 RID: 1783
public class PowerUpGetMsg : UIMsgBase<PowerUpGetMsg.PowerUps>
{
	// Token: 0x06003FDA RID: 16346 RVA: 0x0011980A File Offset: 0x00117A0A
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<PowerUpGetMsg.PowerUpInfo>(ref this.powerUpInfos, typeof(PowerUpGetMsg.PowerUps));
	}

	// Token: 0x06003FDB RID: 16347 RVA: 0x00119821 File Offset: 0x00117A21
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06003FDC RID: 16348 RVA: 0x0011982C File Offset: 0x00117A2C
	protected override void Setup(PowerUpGetMsg.PowerUps skill)
	{
		PowerUpGetMsg.PowerUpInfo powerUpInfo = this.powerUpInfos[(int)skill];
		this.prefixText.text = powerUpInfo.Prefix;
		this.nameText.text = powerUpInfo.Name;
		this.descTextTop.text = powerUpInfo.Desc1;
		this.descTextBot.text = powerUpInfo.Desc2;
		this.lineSprite.sprite = powerUpInfo.LineSprite;
		this.solidSprite.sprite = powerUpInfo.SolidSprite;
		this.glowSprite.sprite = powerUpInfo.GlowSprite;
		this.promptSprite.sprite = powerUpInfo.PromptSprite;
		if (powerUpInfo.PromptButtonText.IsEmpty)
		{
			this.promptGroup.gameObject.SetActive(false);
			return;
		}
		this.promptGroup.gameObject.SetActive(true);
		if (powerUpInfo.ModifierDirection == 0)
		{
			this.singleGroup.SetActive(true);
			this.modifierGroup.SetActive(false);
			this.promptButtonSingle.SetAction(powerUpInfo.PromptButton);
			this.promptButtonSingleText.text = powerUpInfo.PromptButtonText;
			return;
		}
		this.singleGroup.SetActive(false);
		this.modifierGroup.SetActive(true);
		this.promptButtonModifier.SetAction(powerUpInfo.PromptButton);
		this.promptButtonModifierText.text = powerUpInfo.PromptButtonText;
		this.upModifier.SetActive(powerUpInfo.ModifierDirection > 0);
		this.downModifier.SetActive(powerUpInfo.ModifierDirection < 0);
	}

	// Token: 0x06003FDD RID: 16349 RVA: 0x001199C4 File Offset: 0x00117BC4
	public static void Spawn(PowerUpGetMsg prefab, PowerUpGetMsg.PowerUps skill, Action afterMsg)
	{
		PowerUpGetMsg.<>c__DisplayClass23_0 CS$<>8__locals1 = new PowerUpGetMsg.<>c__DisplayClass23_0();
		CS$<>8__locals1.skill = skill;
		CS$<>8__locals1.afterMsg = afterMsg;
		CS$<>8__locals1.msg = null;
		PlayerData.instance.InvPaneHasNew = true;
		CS$<>8__locals1.msg = (UIMsgBase<PowerUpGetMsg.PowerUps>.Spawn(CS$<>8__locals1.skill, prefab, new Action(CS$<>8__locals1.<Spawn>g__AfterMsg|0)) as PowerUpGetMsg);
		if (!CS$<>8__locals1.msg)
		{
			return;
		}
		CS$<>8__locals1.msg.transform.SetLocalPosition2D(Vector2.zero);
		GameCameras.instance.HUDOut();
		HeroController.instance.AddInputBlocker(CS$<>8__locals1.msg);
	}

	// Token: 0x0400417A RID: 16762
	[Space]
	[SerializeField]
	[ArrayForEnum(typeof(PowerUpGetMsg.PowerUps))]
	private PowerUpGetMsg.PowerUpInfo[] powerUpInfos;

	// Token: 0x0400417B RID: 16763
	[Space]
	[SerializeField]
	private TMP_Text prefixText;

	// Token: 0x0400417C RID: 16764
	[SerializeField]
	private TMP_Text nameText;

	// Token: 0x0400417D RID: 16765
	[SerializeField]
	private TMP_Text descTextTop;

	// Token: 0x0400417E RID: 16766
	[SerializeField]
	private TMP_Text descTextBot;

	// Token: 0x0400417F RID: 16767
	[SerializeField]
	private SpriteRenderer lineSprite;

	// Token: 0x04004180 RID: 16768
	[SerializeField]
	private SpriteRenderer solidSprite;

	// Token: 0x04004181 RID: 16769
	[SerializeField]
	private SpriteRenderer glowSprite;

	// Token: 0x04004182 RID: 16770
	[SerializeField]
	private SpriteRenderer promptSprite;

	// Token: 0x04004183 RID: 16771
	[SerializeField]
	private Transform promptGroup;

	// Token: 0x04004184 RID: 16772
	[SerializeField]
	private GameObject singleGroup;

	// Token: 0x04004185 RID: 16773
	[SerializeField]
	private ActionButtonIcon promptButtonSingle;

	// Token: 0x04004186 RID: 16774
	[SerializeField]
	private TMP_Text promptButtonSingleText;

	// Token: 0x04004187 RID: 16775
	[SerializeField]
	private GameObject modifierGroup;

	// Token: 0x04004188 RID: 16776
	[SerializeField]
	private ActionButtonIcon promptButtonModifier;

	// Token: 0x04004189 RID: 16777
	[SerializeField]
	private TMP_Text promptButtonModifierText;

	// Token: 0x0400418A RID: 16778
	[SerializeField]
	private GameObject upModifier;

	// Token: 0x0400418B RID: 16779
	[SerializeField]
	private GameObject downModifier;

	// Token: 0x020019E7 RID: 6631
	public enum PowerUps
	{
		// Token: 0x040097A6 RID: 38822
		Sprint,
		// Token: 0x040097A7 RID: 38823
		WallJump,
		// Token: 0x040097A8 RID: 38824
		HarpoonDash,
		// Token: 0x040097A9 RID: 38825
		Needolin,
		// Token: 0x040097AA RID: 38826
		SuperJump,
		// Token: 0x040097AB RID: 38827
		EvaHeal
	}

	// Token: 0x020019E8 RID: 6632
	[Serializable]
	private struct PowerUpInfo
	{
		// Token: 0x040097AC RID: 38828
		public Sprite LineSprite;

		// Token: 0x040097AD RID: 38829
		public Sprite SolidSprite;

		// Token: 0x040097AE RID: 38830
		public Sprite GlowSprite;

		// Token: 0x040097AF RID: 38831
		public Sprite PromptSprite;

		// Token: 0x040097B0 RID: 38832
		[Space]
		public LocalisedString Prefix;

		// Token: 0x040097B1 RID: 38833
		public LocalisedString Name;

		// Token: 0x040097B2 RID: 38834
		public LocalisedString Desc1;

		// Token: 0x040097B3 RID: 38835
		public LocalisedString Desc2;

		// Token: 0x040097B4 RID: 38836
		[Space]
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString PromptButtonText;

		// Token: 0x040097B5 RID: 38837
		public HeroActionButton PromptButton;

		// Token: 0x040097B6 RID: 38838
		public int ModifierDirection;
	}
}
