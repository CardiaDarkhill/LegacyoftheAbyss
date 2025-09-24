using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006F1 RID: 1777
public class OverscanSetting : MonoBehaviour
{
	// Token: 0x06003FBC RID: 16316 RVA: 0x001191F8 File Offset: 0x001173F8
	private void Start()
	{
		this.gs = GameManager.instance.gameSettings;
		this.textUI.text = this.slider.value.ToString();
	}

	// Token: 0x06003FBD RID: 16317 RVA: 0x00119234 File Offset: 0x00117434
	public void UpdateValue()
	{
		this.textUI.text = this.slider.value.ToString();
	}

	// Token: 0x06003FBE RID: 16318 RVA: 0x0011925F File Offset: 0x0011745F
	public void UpdateTextValue(float value)
	{
		this.textUI.text = value.ToString();
	}

	// Token: 0x06003FBF RID: 16319 RVA: 0x00119274 File Offset: 0x00117474
	public void SetOverscan(float value)
	{
		float overscan = value * 0.01f;
		GameCameras.instance.SetOverscan(overscan);
	}

	// Token: 0x06003FC0 RID: 16320 RVA: 0x00119294 File Offset: 0x00117494
	public void RefreshValueFromSettings()
	{
		if (this.gs == null)
		{
			this.gs = GameManager.instance.gameSettings;
		}
		this.slider.value = this.gs.overScanAdjustment * 100f;
	}

	// Token: 0x06003FC1 RID: 16321 RVA: 0x001192CC File Offset: 0x001174CC
	public void DoneMode()
	{
		this.doneButton.gameObject.SetActive(true);
		this.backButton.gameObject.SetActive(false);
		this.slider.navigation = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = this.doneButton,
			selectOnUp = this.doneButton
		};
	}

	// Token: 0x06003FC2 RID: 16322 RVA: 0x00119334 File Offset: 0x00117534
	public void NormalMode()
	{
		this.doneButton.gameObject.SetActive(false);
		this.backButton.gameObject.SetActive(true);
		this.slider.navigation = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = this.backButton,
			selectOnUp = this.backButton
		};
	}

	// Token: 0x04004160 RID: 16736
	private GameSettings gs;

	// Token: 0x04004161 RID: 16737
	public Slider slider;

	// Token: 0x04004162 RID: 16738
	public MenuButton doneButton;

	// Token: 0x04004163 RID: 16739
	public MenuButton backButton;

	// Token: 0x04004164 RID: 16740
	public Text textUI;

	// Token: 0x04004165 RID: 16741
	public float value;
}
