using System;
using UnityEngine;

// Token: 0x02000676 RID: 1654
public class InvCharmBackboard : MonoBehaviour
{
	// Token: 0x06003B46 RID: 15174 RVA: 0x00104F00 File Offset: 0x00103100
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06003B47 RID: 15175 RVA: 0x00104F10 File Offset: 0x00103110
	private void OnEnable()
	{
		if (!this.positionedCharm)
		{
			this.charmObject.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z - 0.001f);
			this.positionedCharm = true;
		}
		if (this.playerData == null)
		{
			this.playerData = PlayerData.instance;
		}
		if (this.playerData.GetBool(this.gotCharmString) && this.playerData.GetBool(this.newCharmString))
		{
			this.newOrb.SetActive(true);
		}
		if (this.playerData.GetBool(this.gotCharmString) && !this.blanked)
		{
			this.spriteRenderer.sprite = this.blankSprite;
			this.blanked = true;
		}
		if (!this.playerData.GetBool(this.gotCharmString) && this.blanked)
		{
			this.spriteRenderer.sprite = this.activeSprite;
			this.blanked = false;
		}
	}

	// Token: 0x06003B48 RID: 15176 RVA: 0x00105022 File Offset: 0x00103222
	public void SelectCharm()
	{
		if (this.playerData.GetBool(this.newCharmString))
		{
			this.playerData.SetBool(this.newCharmString, false);
			this.newOrb.GetComponent<SimpleFadeOut>().FadeOut();
		}
	}

	// Token: 0x06003B49 RID: 15177 RVA: 0x00105059 File Offset: 0x00103259
	public int GetCharmNum()
	{
		return this.charmNum;
	}

	// Token: 0x06003B4A RID: 15178 RVA: 0x00105061 File Offset: 0x00103261
	public string GetCharmString()
	{
		return this.gotCharmString;
	}

	// Token: 0x06003B4B RID: 15179 RVA: 0x00105069 File Offset: 0x00103269
	public string GetCharmNumString()
	{
		return this.charmNumString;
	}

	// Token: 0x04003D89 RID: 15753
	public GameObject charmObject;

	// Token: 0x04003D8A RID: 15754
	public GameObject newOrb;

	// Token: 0x04003D8B RID: 15755
	public int charmNum;

	// Token: 0x04003D8C RID: 15756
	public string charmNumString;

	// Token: 0x04003D8D RID: 15757
	public string gotCharmString;

	// Token: 0x04003D8E RID: 15758
	public string newCharmString;

	// Token: 0x04003D8F RID: 15759
	public Sprite blankSprite;

	// Token: 0x04003D90 RID: 15760
	public Sprite activeSprite;

	// Token: 0x04003D91 RID: 15761
	private bool positionedCharm;

	// Token: 0x04003D92 RID: 15762
	private PlayerData playerData;

	// Token: 0x04003D93 RID: 15763
	private GameObject orb;

	// Token: 0x04003D94 RID: 15764
	private SpriteRenderer spriteRenderer;

	// Token: 0x04003D95 RID: 15765
	private bool blanked;
}
