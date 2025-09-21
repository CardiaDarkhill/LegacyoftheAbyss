using System;
using GlobalSettings;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006E8 RID: 1768
public class MenuButtonSaveQuitListCondition : MenuButtonListCondition
{
	// Token: 0x06003F90 RID: 16272 RVA: 0x00118609 File Offset: 0x00116809
	public override bool IsFulfilled()
	{
		return !PlayerData.instance.disableSaveQuit && !ScenePreloader.HasPendingOperations;
	}

	// Token: 0x06003F91 RID: 16273 RVA: 0x00118621 File Offset: 0x00116821
	public override bool AlwaysVisible()
	{
		return true;
	}

	// Token: 0x06003F92 RID: 16274 RVA: 0x00118624 File Offset: 0x00116824
	public override void OnActiveStateSet(bool isActive)
	{
		if (this.buttonText)
		{
			this.buttonText.color = (isActive ? Color.white : UI.DisabledUiTextColor);
		}
	}

	// Token: 0x0400413F RID: 16703
	[SerializeField]
	private Text buttonText;
}
