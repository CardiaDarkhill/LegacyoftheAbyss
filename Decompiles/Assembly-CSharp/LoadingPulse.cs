using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006CB RID: 1739
public class LoadingPulse : MonoBehaviour
{
	// Token: 0x06003ED5 RID: 16085 RVA: 0x001147A3 File Offset: 0x001129A3
	private void Start()
	{
		this.sprite = base.GetComponent<Image>();
		this.normalColor = this.sprite.color;
		this.pulsing = true;
		this.currentLerpTime = 0f;
	}

	// Token: 0x06003ED6 RID: 16086 RVA: 0x001147D4 File Offset: 0x001129D4
	private void Update()
	{
		if (this.pulsing)
		{
			if (!this.reverse)
			{
				this.currentLerpTime += Time.deltaTime;
				if (this.currentLerpTime > this.pulseDuration)
				{
					this.currentLerpTime = this.pulseDuration;
					this.reverse = true;
				}
			}
			else
			{
				this.currentLerpTime -= Time.deltaTime;
				if (this.currentLerpTime < 0f)
				{
					this.currentLerpTime = 0f;
					this.reverse = false;
				}
			}
			float t = this.currentLerpTime / this.pulseDuration;
			this.sprite.color = Color.Lerp(this.normalColor, this.pulseColor, t);
		}
	}

	// Token: 0x04004070 RID: 16496
	public Color pulseColor;

	// Token: 0x04004071 RID: 16497
	public float pulseDuration;

	// Token: 0x04004072 RID: 16498
	private Image sprite;

	// Token: 0x04004073 RID: 16499
	private Color normalColor;

	// Token: 0x04004074 RID: 16500
	private bool pulsing;

	// Token: 0x04004075 RID: 16501
	private bool reverse;

	// Token: 0x04004076 RID: 16502
	private float currentLerpTime;
}
