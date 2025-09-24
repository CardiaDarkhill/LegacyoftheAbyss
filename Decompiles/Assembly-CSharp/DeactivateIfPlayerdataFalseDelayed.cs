using System;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class DeactivateIfPlayerdataFalseDelayed : MonoBehaviour
{
	// Token: 0x06001361 RID: 4961 RVA: 0x000588CD File Offset: 0x00056ACD
	private void Start()
	{
		this.gm = GameManager.instance;
		this.pd = this.gm.playerData;
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x000588EB File Offset: 0x00056AEB
	private void OnEnable()
	{
		if (this.delay <= 0f)
		{
			this.DoCheck();
			return;
		}
		base.Invoke("DoCheck", this.delay);
	}

	// Token: 0x06001363 RID: 4963 RVA: 0x00058914 File Offset: 0x00056B14
	private void DoCheck()
	{
		if (this.gm == null)
		{
			this.gm = GameManager.instance;
		}
		if (this.pd == null)
		{
			this.pd = this.gm.playerData;
		}
		if (!this.pd.GetBool(this.boolName))
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040011D0 RID: 4560
	[PlayerDataField(typeof(bool), true)]
	public string boolName;

	// Token: 0x040011D1 RID: 4561
	public float delay;

	// Token: 0x040011D2 RID: 4562
	private GameManager gm;

	// Token: 0x040011D3 RID: 4563
	private PlayerData pd;
}
