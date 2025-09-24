using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000711 RID: 1809
public class SaveProfileSilkBar : MonoBehaviour
{
	// Token: 0x06004086 RID: 16518 RVA: 0x0011BD08 File Offset: 0x00119F08
	public void ShowSilk(bool isBroken, int maxSilk, bool isCursed)
	{
		this.cursedAlt.SetActive(isCursed);
		this.notBroken.SetActive(!isBroken && !isCursed);
		this.brokenAlt.SetActive(isBroken && !isCursed);
		this.sizer.preferredWidth = (float)maxSilk * this.widthPerSilk - this.baseWidth;
		this.silkChunkTemplate.gameObject.SetActive(false);
		if (this.silkChunkTemplate.transform.parent == this.silkChunkParent)
		{
			this.silkChunkTemplate.transform.SetParent(this.silkChunkParent.parent);
		}
		for (int i = maxSilk - this.silkChunkParent.childCount; i > 0; i--)
		{
			Object.Instantiate<Image>(this.silkChunkTemplate, this.silkChunkParent);
		}
		for (int j = 0; j < this.silkChunkParent.childCount; j++)
		{
			Transform child = this.silkChunkParent.GetChild(j);
			if (!isBroken && j < maxSilk)
			{
				child.GetComponent<Image>().sprite = this.silkChunkVariants[Random.Range(0, this.silkChunkVariants.Length)];
				child.gameObject.SetActive(true);
			}
			else
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x04004218 RID: 16920
	[SerializeField]
	private LayoutElement sizer;

	// Token: 0x04004219 RID: 16921
	[SerializeField]
	private float widthPerSilk;

	// Token: 0x0400421A RID: 16922
	[SerializeField]
	private float baseWidth;

	// Token: 0x0400421B RID: 16923
	[SerializeField]
	private Image silkChunkTemplate;

	// Token: 0x0400421C RID: 16924
	[SerializeField]
	private Sprite[] silkChunkVariants;

	// Token: 0x0400421D RID: 16925
	[SerializeField]
	private Transform silkChunkParent;

	// Token: 0x0400421E RID: 16926
	[Space]
	[SerializeField]
	private GameObject notBroken;

	// Token: 0x0400421F RID: 16927
	[SerializeField]
	private GameObject brokenAlt;

	// Token: 0x04004220 RID: 16928
	[SerializeField]
	private GameObject cursedAlt;
}
