using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020005F1 RID: 1521
[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (Skill)")]
public class ToolItemSkill : ToolItemBasic
{
	// Token: 0x1700062D RID: 1581
	// (get) Token: 0x06003647 RID: 13895 RVA: 0x000F0187 File Offset: 0x000EE387
	public LocalisedString MsgGetPrefix
	{
		get
		{
			return this.msgGetPrefix;
		}
	}

	// Token: 0x1700062E RID: 1582
	// (get) Token: 0x06003648 RID: 13896 RVA: 0x000F018F File Offset: 0x000EE38F
	public Sprite PromptSprite
	{
		get
		{
			return this.promptSprite;
		}
	}

	// Token: 0x1700062F RID: 1583
	// (get) Token: 0x06003649 RID: 13897 RVA: 0x000F0197 File Offset: 0x000EE397
	public Sprite PromptGlowSprite
	{
		get
		{
			return this.promptGlowSprite;
		}
	}

	// Token: 0x17000630 RID: 1584
	// (get) Token: 0x0600364A RID: 13898 RVA: 0x000F019F File Offset: 0x000EE39F
	public Sprite PromptSilhouetteSprite
	{
		get
		{
			return this.promptSilhouetteSprite;
		}
	}

	// Token: 0x17000631 RID: 1585
	// (get) Token: 0x0600364B RID: 13899 RVA: 0x000F01A7 File Offset: 0x000EE3A7
	public LocalisedString PromptDescription
	{
		get
		{
			return this.promptDescription;
		}
	}

	// Token: 0x17000632 RID: 1586
	// (get) Token: 0x0600364C RID: 13900 RVA: 0x000F01AF File Offset: 0x000EE3AF
	public Sprite HudGlowSprite
	{
		get
		{
			return this.hudGlowSprite;
		}
	}

	// Token: 0x17000633 RID: 1587
	// (get) Token: 0x0600364D RID: 13901 RVA: 0x000F01B7 File Offset: 0x000EE3B7
	public Sprite InvGlowSprite
	{
		get
		{
			return this.invGlowSprite;
		}
	}

	// Token: 0x04003946 RID: 14662
	[Header("Skill")]
	[SerializeField]
	private LocalisedString msgGetPrefix;

	// Token: 0x04003947 RID: 14663
	[SerializeField]
	private Sprite promptSprite;

	// Token: 0x04003948 RID: 14664
	[SerializeField]
	private Sprite promptGlowSprite;

	// Token: 0x04003949 RID: 14665
	[SerializeField]
	private Sprite promptSilhouetteSprite;

	// Token: 0x0400394A RID: 14666
	[SerializeField]
	private LocalisedString promptDescription;

	// Token: 0x0400394B RID: 14667
	[Space]
	[SerializeField]
	private Sprite hudGlowSprite;

	// Token: 0x0400394C RID: 14668
	[SerializeField]
	private Sprite invGlowSprite;
}
