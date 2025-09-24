using System;
using UnityEngine;

// Token: 0x02000458 RID: 1112
public class GraphicsTiersActivator : MonoBehaviour
{
	// Token: 0x06002768 RID: 10088 RVA: 0x000B0D58 File Offset: 0x000AEF58
	private void Reset()
	{
		this.disableObj = base.gameObject;
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000B0D66 File Offset: 0x000AEF66
	private void Awake()
	{
		Platform.GraphicsTierChanged += this.OnGraphicsTierChanged;
		this.OnGraphicsTierChanged(Platform.Current.GraphicsTier);
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000B0D89 File Offset: 0x000AEF89
	private void OnDestroy()
	{
		Platform.GraphicsTierChanged -= this.OnGraphicsTierChanged;
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000B0D9C File Offset: 0x000AEF9C
	private void OnGraphicsTierChanged(Platform.GraphicsTiers graphicsTier)
	{
		bool flag = graphicsTier > this.disableThreshold;
		if (this.disableObj)
		{
			this.disableObj.SetActive(flag);
		}
		if (this.altObj)
		{
			this.altObj.SetActive(!flag);
		}
	}

	// Token: 0x0400242F RID: 9263
	[SerializeField]
	private GameObject disableObj;

	// Token: 0x04002430 RID: 9264
	[SerializeField]
	private GameObject altObj;

	// Token: 0x04002431 RID: 9265
	[Space]
	[SerializeField]
	private Platform.GraphicsTiers disableThreshold = Platform.GraphicsTiers.Low;
}
