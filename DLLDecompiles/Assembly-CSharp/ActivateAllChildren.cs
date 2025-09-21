using System;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class ActivateAllChildren : MonoBehaviour
{
	// Token: 0x060013AA RID: 5034 RVA: 0x000599DC File Offset: 0x00057BDC
	private void OnEnable()
	{
		if (this.playerdataBoolTest == "")
		{
			this.DoActivation();
			return;
		}
		if ((PlayerData.instance.GetBool(this.playerdataBoolTest) && !this.reverseTest) || (!PlayerData.instance.GetBool(this.playerdataBoolTest) && this.reverseTest))
		{
			this.DoActivation();
			return;
		}
		if (this.deactivateIfTestFailed)
		{
			this.DoDeactivation();
		}
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x00059A4C File Offset: 0x00057C4C
	private void DoActivation()
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(true);
		}
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x00059AA8 File Offset: 0x00057CA8
	private void DoDeactivation()
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
	}

	// Token: 0x04001217 RID: 4631
	public string playerdataBoolTest;

	// Token: 0x04001218 RID: 4632
	public bool reverseTest;

	// Token: 0x04001219 RID: 4633
	public bool deactivateIfTestFailed;
}
