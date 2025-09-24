using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000748 RID: 1864
public class UpdateTextWithSliderValue : MonoBehaviour
{
	// Token: 0x0600426D RID: 17005 RVA: 0x00125420 File Offset: 0x00123620
	private void Start()
	{
		this.textUI = base.GetComponent<Text>();
		this.textUI.text = this.slider.value.ToString();
	}

	// Token: 0x0600426E RID: 17006 RVA: 0x00125458 File Offset: 0x00123658
	public void UpdateValue()
	{
		this.textUI.text = this.slider.value.ToString();
	}

	// Token: 0x040043FF RID: 17407
	public Slider slider;

	// Token: 0x04004400 RID: 17408
	private Text textUI;

	// Token: 0x04004401 RID: 17409
	public float value;
}
