using System;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class ActivateIfPlayerdataFalse : MonoBehaviour
{
	// Token: 0x0600001F RID: 31 RVA: 0x000025D8 File Offset: 0x000007D8
	private void Start()
	{
		this.gm = GameManager.instance;
		this.pd = this.gm.playerData;
		if (!this.pd.GetBool(this.boolName))
		{
			base.gameObject.SetActive(true);
			if (this.objectToActivate)
			{
				this.objectToActivate.SetActive(true);
			}
		}
	}

	// Token: 0x0400000B RID: 11
	[PlayerDataField(typeof(bool), true)]
	public string boolName;

	// Token: 0x0400000C RID: 12
	public GameObject objectToActivate;

	// Token: 0x0400000D RID: 13
	private GameManager gm;

	// Token: 0x0400000E RID: 14
	private PlayerData pd;
}
