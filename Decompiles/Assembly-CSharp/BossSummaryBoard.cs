using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003A1 RID: 929
public class BossSummaryBoard : MonoBehaviour
{
	// Token: 0x06001F4E RID: 8014 RVA: 0x0008F144 File Offset: 0x0008D344
	private void Start()
	{
		if (this.bossSummaryUI)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.bossSummaryUI);
			this.ui = gameObject.GetComponent<BossSummaryUI>();
			if (this.ui)
			{
				this.ui.SetupUI(this.bossStatues);
			}
			gameObject.SetActive(false);
		}
	}

	// Token: 0x06001F4F RID: 8015 RVA: 0x0008F19B File Offset: 0x0008D39B
	public void Show()
	{
		if (this.ui)
		{
			this.ui.Show();
		}
	}

	// Token: 0x06001F50 RID: 8016 RVA: 0x0008F1B5 File Offset: 0x0008D3B5
	public void Hide()
	{
		if (this.ui)
		{
			this.ui.Hide();
		}
	}

	// Token: 0x04001E3C RID: 7740
	public List<BossStatue> bossStatues = new List<BossStatue>();

	// Token: 0x04001E3D RID: 7741
	[Space]
	public GameObject bossSummaryUI;

	// Token: 0x04001E3E RID: 7742
	private BossSummaryUI ui;
}
