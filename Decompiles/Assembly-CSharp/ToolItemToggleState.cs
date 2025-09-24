using System;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005F5 RID: 1525
[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (Toggle State)")]
public class ToolItemToggleState : ToolItem
{
	// Token: 0x17000642 RID: 1602
	// (get) Token: 0x06003672 RID: 13938 RVA: 0x000F0695 File Offset: 0x000EE895
	public override LocalisedString DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x17000643 RID: 1603
	// (get) Token: 0x06003673 RID: 13939 RVA: 0x000F069D File Offset: 0x000EE89D
	public override LocalisedString Description
	{
		get
		{
			return this.description;
		}
	}

	// Token: 0x17000644 RID: 1604
	// (get) Token: 0x06003674 RID: 13940 RVA: 0x000F06A5 File Offset: 0x000EE8A5
	public override ToolItem.UsageOptions Usage
	{
		get
		{
			return this.CurrentState.Usage;
		}
	}

	// Token: 0x17000645 RID: 1605
	// (get) Token: 0x06003675 RID: 13941 RVA: 0x000F06B2 File Offset: 0x000EE8B2
	private ToolItemToggleState.State CurrentState
	{
		get
		{
			if (!Application.isPlaying)
			{
				return this.onState;
			}
			if (!PlayerData.instance.GetVariable(this.statePdBool))
			{
				return this.offState;
			}
			return this.onState;
		}
	}

	// Token: 0x17000646 RID: 1606
	// (get) Token: 0x06003676 RID: 13942 RVA: 0x000F06E1 File Offset: 0x000EE8E1
	public override bool DisplayTogglePrompt
	{
		get
		{
			return !string.IsNullOrEmpty(this.statePdBool);
		}
	}

	// Token: 0x17000647 RID: 1607
	// (get) Token: 0x06003677 RID: 13943 RVA: 0x000F06F1 File Offset: 0x000EE8F1
	public override bool CanToggle
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003678 RID: 13944 RVA: 0x000F06F4 File Offset: 0x000EE8F4
	public override Sprite GetInventorySprite(ToolItem.IconVariants iconVariant)
	{
		ToolItemToggleState.State currentState = this.CurrentState;
		Sprite result;
		if (iconVariant == ToolItem.IconVariants.Poison)
		{
			result = (currentState.InventorySpritePoison ? currentState.InventorySpritePoison : currentState.InventorySprite);
		}
		else
		{
			result = currentState.InventorySprite;
		}
		return result;
	}

	// Token: 0x06003679 RID: 13945 RVA: 0x000F0734 File Offset: 0x000EE934
	public override Sprite GetHudSprite(ToolItem.IconVariants iconVariant)
	{
		ToolItemToggleState.State currentState = this.CurrentState;
		Sprite result;
		if (iconVariant == ToolItem.IconVariants.Poison)
		{
			result = (currentState.HudSpritePoison ? currentState.HudSpritePoison : currentState.HudSprite);
		}
		else
		{
			result = currentState.HudSprite;
		}
		return result;
	}

	// Token: 0x0600367A RID: 13946 RVA: 0x000F0774 File Offset: 0x000EE974
	public override bool DoToggle(out bool didChangeVisually)
	{
		if (string.IsNullOrEmpty(this.statePdBool))
		{
			return base.DoToggle(out didChangeVisually);
		}
		PlayerData instance = PlayerData.instance;
		bool variable = instance.GetVariable(this.statePdBool);
		instance.SetVariable(this.statePdBool, !variable);
		didChangeVisually = true;
		return true;
	}

	// Token: 0x0600367B RID: 13947 RVA: 0x000F07BB File Offset: 0x000EE9BB
	public override void PlayToggleAudio(AudioSource audioSource)
	{
		this.toggleAudio.PlayOnSource(audioSource);
	}

	// Token: 0x04003958 RID: 14680
	[Header("Toggle State")]
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x04003959 RID: 14681
	[SerializeField]
	private LocalisedString description;

	// Token: 0x0400395A RID: 14682
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string statePdBool;

	// Token: 0x0400395B RID: 14683
	[Space]
	[SerializeField]
	private AudioEventRandom toggleAudio;

	// Token: 0x0400395C RID: 14684
	[Space]
	[SerializeField]
	private ToolItemToggleState.State offState;

	// Token: 0x0400395D RID: 14685
	[SerializeField]
	private ToolItemToggleState.State onState;

	// Token: 0x02001900 RID: 6400
	[Serializable]
	private class State
	{
		// Token: 0x04009414 RID: 37908
		public Sprite InventorySprite;

		// Token: 0x04009415 RID: 37909
		public Sprite InventorySpritePoison;

		// Token: 0x04009416 RID: 37910
		public Sprite HudSprite;

		// Token: 0x04009417 RID: 37911
		public Sprite HudSpritePoison;

		// Token: 0x04009418 RID: 37912
		[Space]
		public ToolItem.UsageOptions Usage;
	}
}
