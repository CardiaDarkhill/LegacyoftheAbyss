using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200060D RID: 1549
public class BrightnessSetting : MonoBehaviour
{
	// Token: 0x06003759 RID: 14169 RVA: 0x000F42F0 File Offset: 0x000F24F0
	private void Start()
	{
		this.gs = GameManager.instance.gameSettings;
		this.UpdateValue();
	}

	// Token: 0x0600375A RID: 14170 RVA: 0x000F4308 File Offset: 0x000F2508
	public void UpdateValue()
	{
		this.textUI.text = (this.slider.value * this.valueMultiplier).ToString() + "%";
	}

	// Token: 0x0600375B RID: 14171 RVA: 0x000F4344 File Offset: 0x000F2544
	public void UpdateTextValue(float value)
	{
		this.textUI.text = (value * this.valueMultiplier).ToString() + "%";
	}

	// Token: 0x0600375C RID: 14172 RVA: 0x000F4376 File Offset: 0x000F2576
	public void SetBrightness(float value)
	{
		GameCameras.instance.brightnessEffect.SetBrightness(value / 20f);
		this.gs.brightnessAdjustment = value;
	}

	// Token: 0x0600375D RID: 14173 RVA: 0x000F439C File Offset: 0x000F259C
	public void RefreshValueFromSettings()
	{
		if (this.gs == null)
		{
			this.gs = GameManager.instance.gameSettings;
		}
		this.slider.value = this.gs.brightnessAdjustment;
		this.slider.onValueChanged.Invoke(this.slider.value);
		this.UpdateValue();
	}

	// Token: 0x0600375E RID: 14174 RVA: 0x000F43F8 File Offset: 0x000F25F8
	public void DoneMode()
	{
		this.doneButton.gameObject.SetActive(true);
		this.backButton.gameObject.SetActive(false);
		base.GetComponentInParent<MenuScreen>(true).backButton = this.doneButton;
		this.slider.navigation = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = this.doneButton,
			selectOnUp = this.doneButton
		};
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x000F4470 File Offset: 0x000F2670
	public void NormalMode()
	{
		this.doneButton.gameObject.SetActive(false);
		this.backButton.gameObject.SetActive(true);
		base.GetComponentInParent<MenuScreen>(true).backButton = this.backButton;
		this.slider.navigation = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = this.backButton,
			selectOnUp = this.backButton
		};
	}

	// Token: 0x04003A3D RID: 14909
	private GameSettings gs;

	// Token: 0x04003A3E RID: 14910
	private float valueMultiplier = 5f;

	// Token: 0x04003A3F RID: 14911
	public Slider slider;

	// Token: 0x04003A40 RID: 14912
	public MenuButton doneButton;

	// Token: 0x04003A41 RID: 14913
	public MenuButton backButton;

	// Token: 0x04003A42 RID: 14914
	public Text textUI;
}
