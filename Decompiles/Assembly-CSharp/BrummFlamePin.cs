using System;
using UnityEngine;

// Token: 0x0200060E RID: 1550
public class BrummFlamePin : MonoBehaviour
{
	// Token: 0x06003761 RID: 14177 RVA: 0x000F44FA File Offset: 0x000F26FA
	private void Start()
	{
		this.gm = GameManager.instance;
		this.pd = this.gm.playerData;
	}

	// Token: 0x06003762 RID: 14178 RVA: 0x000F4518 File Offset: 0x000F2718
	private void OnEnable()
	{
		base.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		if (this.gm == null)
		{
			this.gm = GameManager.instance;
		}
		if (this.pd == null)
		{
			this.pd = this.gm.playerData;
		}
		base.gameObject.GetComponent<SpriteRenderer>().enabled = true;
	}

	// Token: 0x04003A43 RID: 14915
	private GameManager gm;

	// Token: 0x04003A44 RID: 14916
	private PlayerData pd;
}
