using System;
using UnityEngine;

// Token: 0x0200079D RID: 1949
public class WaitFrameAndPaused : CustomYieldInstruction
{
	// Token: 0x170007B4 RID: 1972
	// (get) Token: 0x060044C4 RID: 17604 RVA: 0x0012CF10 File Offset: 0x0012B110
	public override bool keepWaiting
	{
		get
		{
			return Time.deltaTime <= Mathf.Epsilon;
		}
	}
}
