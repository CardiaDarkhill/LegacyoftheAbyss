using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class CreditsCardSection : CreditsSectionBase
{
	// Token: 0x06000366 RID: 870 RVA: 0x00011CA9 File Offset: 0x0000FEA9
	protected override IEnumerator ShowRoutine()
	{
		yield return new WaitForSeconds(this.holdDuration);
		yield break;
	}

	// Token: 0x0400030D RID: 781
	[SerializeField]
	private float holdDuration;
}
