using System;
using UnityEngine;

// Token: 0x0200024F RID: 591
public class MatchScale : MonoBehaviour
{
	// Token: 0x06001574 RID: 5492 RVA: 0x00061237 File Offset: 0x0005F437
	private void Start()
	{
		this.hasStarted = true;
		this.DoMatch();
	}

	// Token: 0x06001575 RID: 5493 RVA: 0x00061246 File Offset: 0x0005F446
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.DoMatch();
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x00061257 File Offset: 0x0005F457
	private void DoMatch()
	{
		base.transform.localScale = this.target.localScale;
	}

	// Token: 0x04001413 RID: 5139
	public Transform target;

	// Token: 0x04001414 RID: 5140
	private bool hasStarted;
}
