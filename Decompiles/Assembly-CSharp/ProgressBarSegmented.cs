using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006F9 RID: 1785
public class ProgressBarSegmented : MonoBehaviour
{
	// Token: 0x06003FE4 RID: 16356 RVA: 0x00119B34 File Offset: 0x00117D34
	public void SetSegmentCount(int count)
	{
		if (this.bars == null)
		{
			this.bars = new List<ImageSlider>(count);
		}
		this.barTemplate.gameObject.SetActive(false);
		while (this.bars.Count < count)
		{
			this.bars.Add(Object.Instantiate<ImageSlider>(this.barTemplate, this.barTemplate.transform.parent));
		}
		for (int i = 0; i < this.bars.Count; i++)
		{
			this.bars[i].gameObject.SetActive(i < count);
		}
	}

	// Token: 0x06003FE5 RID: 16357 RVA: 0x00119BCC File Offset: 0x00117DCC
	public void SetSegmentProgress(int index, float progress)
	{
		this.bars[index].Value = progress;
	}

	// Token: 0x0400418D RID: 16781
	[SerializeField]
	private ImageSlider barTemplate;

	// Token: 0x0400418E RID: 16782
	private List<ImageSlider> bars;
}
