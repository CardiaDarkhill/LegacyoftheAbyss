using System;
using UnityEngine;

// Token: 0x0200004F RID: 79
public class ActivateIfPlayerdataTrue : MonoBehaviour
{
	// Token: 0x06000224 RID: 548 RVA: 0x0000D954 File Offset: 0x0000BB54
	private void Start()
	{
		this.gm = GameManager.instance;
		this.pd = this.gm.playerData;
		if (this.pd.GetBool(this.boolName))
		{
			base.gameObject.SetActive(true);
			if (this.objectToActivate)
			{
				this.objectToActivate.SetActive(true);
			}
		}
	}

	// Token: 0x040001D3 RID: 467
	[PlayerDataField(typeof(bool), true)]
	public string boolName;

	// Token: 0x040001D4 RID: 468
	public GameObject objectToActivate;

	// Token: 0x040001D5 RID: 469
	private GameManager gm;

	// Token: 0x040001D6 RID: 470
	private PlayerData pd;
}
