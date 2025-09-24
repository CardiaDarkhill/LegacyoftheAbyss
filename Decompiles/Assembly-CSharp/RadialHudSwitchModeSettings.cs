using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000363 RID: 867
public class RadialHudSwitchModeSettings : SwitchPlatformModeUpdateHandler
{
	// Token: 0x06001DF2 RID: 7666 RVA: 0x0008A7B3 File Offset: 0x000889B3
	private void Awake()
	{
		this.hudIcon.Updated += this.OnHudIconUpdated;
	}

	// Token: 0x06001DF3 RID: 7667 RVA: 0x0008A7CC File Offset: 0x000889CC
	protected override void OnOperationModeChanged(bool newIsHandheld)
	{
		this.isHandheld = newIsHandheld;
		this.UpdateSprites();
	}

	// Token: 0x06001DF4 RID: 7668 RVA: 0x0008A7DB File Offset: 0x000889DB
	private void OnHudIconUpdated()
	{
		this.UpdateSprites();
	}

	// Token: 0x06001DF5 RID: 7669 RVA: 0x0008A7E4 File Offset: 0x000889E4
	private void UpdateSprites()
	{
		ToolItem currentTool = this.hudIcon.CurrentTool;
		if (!currentTool)
		{
			return;
		}
		Sprite sprite;
		Sprite sprite2;
		if (currentTool is ToolItemSkill && !this.hudIcon.GetIsEmpty())
		{
			sprite = this.glowRegular;
			sprite2 = this.glowHandheld;
		}
		else
		{
			sprite = this.regularMode;
			sprite2 = this.handheldMode;
		}
		Image[] array;
		if (this.isHandheld)
		{
			array = this.circleImages;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sprite = sprite2;
			}
			return;
		}
		array = this.circleImages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sprite = sprite;
		}
	}

	// Token: 0x04001D15 RID: 7445
	[SerializeField]
	private ToolHudIcon hudIcon;

	// Token: 0x04001D16 RID: 7446
	[SerializeField]
	private Image[] circleImages;

	// Token: 0x04001D17 RID: 7447
	[Space]
	[SerializeField]
	private Sprite regularMode;

	// Token: 0x04001D18 RID: 7448
	[SerializeField]
	private Sprite handheldMode;

	// Token: 0x04001D19 RID: 7449
	[SerializeField]
	private Sprite glowRegular;

	// Token: 0x04001D1A RID: 7450
	[SerializeField]
	private Sprite glowHandheld;

	// Token: 0x04001D1B RID: 7451
	private bool isHandheld;
}
