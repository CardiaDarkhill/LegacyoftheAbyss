using System;
using HKMenu;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000749 RID: 1865
public class VideoMenuOptions : MonoBehaviour, IMenuOptionLayout
{
	// Token: 0x06004270 RID: 17008 RVA: 0x0012548C File Offset: 0x0012368C
	public void ConfigureNavigation()
	{
		if (GameManager.instance.gameConfig.enableTFRSetting)
		{
			this.frameCapOption.transform.parent.gameObject.SetActive(true);
			Navigation navigation = this.vsyncOption.navigation;
			navigation.selectOnDown = this.frameCapOption;
			this.vsyncOption.navigation = navigation;
			Navigation navigation2 = this.screenScaleOption.navigation;
			navigation2.selectOnUp = this.frameCapOption;
			this.screenScaleOption.navigation = navigation2;
		}
	}

	// Token: 0x04004402 RID: 17410
	public MenuScreen videoMenuScreen;

	// Token: 0x04004403 RID: 17411
	public MenuSelectable vsyncOption;

	// Token: 0x04004404 RID: 17412
	public MenuSelectable frameCapOption;

	// Token: 0x04004405 RID: 17413
	public MenuSelectable screenScaleOption;

	// Token: 0x04004406 RID: 17414
	public MenuSelectable resetButton;

	// Token: 0x04004407 RID: 17415
	public MenuSelectable applyButton;
}
