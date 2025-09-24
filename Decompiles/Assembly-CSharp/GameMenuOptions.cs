using System;
using GlobalEnums;
using HKMenu;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000668 RID: 1640
public class GameMenuOptions : MonoBehaviour, IMenuOptionLayout
{
	// Token: 0x06003ACE RID: 15054 RVA: 0x00103114 File Offset: 0x00101314
	private void OnEnable()
	{
		if (this.reconfigureOnEnable)
		{
			this.ConfigureNavigation();
		}
	}

	// Token: 0x06003ACF RID: 15055 RVA: 0x00103124 File Offset: 0x00101324
	public void ConfigureNavigation()
	{
		if (GameManager.instance.GameState != GameState.MAIN_MENU)
		{
			this.languageOption.interactable = false;
			this.languageOption.transform.parent.gameObject.SetActive(true);
			this.languageOptionDescription.SetActive(true);
			Navigation navigation = this.backerOption.navigation;
			Navigation navigation2 = this.applyButton.navigation;
			navigation.selectOnUp = this.applyButton;
			navigation2.selectOnDown = this.backerOption;
			this.backerOption.navigation = navigation;
			this.applyButton.navigation = navigation2;
			this.gameOptionsMenuScreen.defaultHighlight = this.backerOption;
		}
		else
		{
			this.languageOption.interactable = true;
			this.languageOption.transform.parent.gameObject.SetActive(true);
			this.languageOptionDescription.SetActive(false);
			this.gameOptionsMenuScreen.defaultHighlight = this.languageOption;
		}
		if (this.languageOption && this.languageOption is MenuLanguageSetting)
		{
			((MenuLanguageSetting)this.languageOption).UpdateAlpha();
		}
		if (GameManager.instance.gameConfig.hideAchievementsMenu)
		{
			this.nativeAchievementsOption.transform.parent.gameObject.SetActive(false);
		}
	}

	// Token: 0x04003D2C RID: 15660
	public MenuScreen gameOptionsMenuScreen;

	// Token: 0x04003D2D RID: 15661
	public MenuSelectable languageOption;

	// Token: 0x04003D2E RID: 15662
	public GameObject languageOptionDescription;

	// Token: 0x04003D2F RID: 15663
	public MenuSelectable backerOption;

	// Token: 0x04003D30 RID: 15664
	public MenuSelectable nativeAchievementsOption;

	// Token: 0x04003D31 RID: 15665
	public MenuSelectable resetButton;

	// Token: 0x04003D32 RID: 15666
	public MenuSelectable applyButton;

	// Token: 0x04003D33 RID: 15667
	public bool reconfigureOnEnable;
}
