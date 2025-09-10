using System;
using UnityEngine.UI;

// Token: 0x020006CD RID: 1741
public class MainMenuOptions : PreselectOption
{
	// Token: 0x06003EE1 RID: 16097 RVA: 0x00114AF0 File Offset: 0x00112CF0
	public void ConfigureNavigation()
	{
		if (Platform.Current.HasNativeAchievementsDialog && GameManager.instance.gameConfig.hideExtrasMenu)
		{
			this.achievementsButton.gameObject.SetActive(false);
			this.extrasButton.gameObject.SetActive(false);
			Navigation navigation = this.optionsButton.navigation;
			Navigation navigation2 = this.quitButton.navigation;
			navigation.selectOnDown = this.quitButton;
			navigation2.selectOnUp = this.optionsButton;
			this.optionsButton.navigation = navigation;
			this.quitButton.navigation = navigation2;
			return;
		}
		if (Platform.Current.HasNativeAchievementsDialog)
		{
			this.achievementsButton.gameObject.SetActive(false);
			Navigation navigation3 = this.optionsButton.navigation;
			Navigation navigation4 = this.extrasButton.navigation;
			navigation3.selectOnDown = this.extrasButton;
			navigation4.selectOnUp = this.optionsButton;
			this.optionsButton.navigation = navigation3;
			this.extrasButton.navigation = navigation4;
			return;
		}
		if (GameManager.instance.gameConfig.hideExtrasMenu)
		{
			this.extrasButton.gameObject.SetActive(false);
			Navigation navigation5 = this.achievementsButton.navigation;
			Navigation navigation6 = this.quitButton.navigation;
			navigation5.selectOnDown = this.quitButton;
			navigation6.selectOnUp = this.achievementsButton;
			this.achievementsButton.navigation = navigation5;
			this.quitButton.navigation = navigation6;
		}
	}

	// Token: 0x04004084 RID: 16516
	public MenuButton startButton;

	// Token: 0x04004085 RID: 16517
	public MenuButton optionsButton;

	// Token: 0x04004086 RID: 16518
	public MenuButton achievementsButton;

	// Token: 0x04004087 RID: 16519
	public MenuButton extrasButton;

	// Token: 0x04004088 RID: 16520
	public MenuButton quitButton;
}
