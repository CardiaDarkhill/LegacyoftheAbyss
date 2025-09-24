using System;
using UnityEngine;

// Token: 0x02000234 RID: 564
public class FreezeMomentOnEnable : MonoBehaviour
{
	// Token: 0x060014B3 RID: 5299 RVA: 0x0005D3A2 File Offset: 0x0005B5A2
	private void OnEnable()
	{
		this.scheduledFreeze = true;
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x0005D3AB File Offset: 0x0005B5AB
	private void Update()
	{
		if (this.scheduledFreeze)
		{
			this.scheduledFreeze = false;
			this.DoFreeze();
		}
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x0005D3C2 File Offset: 0x0005B5C2
	private void DoFreeze()
	{
		GameManager.instance.FreezeMoment(this.freezeMoment);
	}

	// Token: 0x04001314 RID: 4884
	public int freezeMoment;

	// Token: 0x04001315 RID: 4885
	private bool scheduledFreeze = true;
}
