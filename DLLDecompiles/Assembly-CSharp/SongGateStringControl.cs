using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200027F RID: 639
public class SongGateStringControl : MonoBehaviour
{
	// Token: 0x0600169B RID: 5787 RVA: 0x00065AE8 File Offset: 0x00063CE8
	private void Awake()
	{
		this.Init();
		if (!this.getChildren)
		{
			return;
		}
		this.strings.AddRange(base.GetComponentsInChildren<SongGateString>(true));
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x00065B0B File Offset: 0x00063D0B
	private void Init()
	{
		if (this.init)
		{
			return;
		}
		this.init = true;
		this.strings.RemoveAll((SongGateString o) => o == null);
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x00065B48 File Offset: 0x00063D48
	public void StrumStart()
	{
		this.Init();
		foreach (SongGateString songGateString in this.strings)
		{
			songGateString.StrumStart();
		}
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x00065BA0 File Offset: 0x00063DA0
	public void StrumEnd()
	{
		this.Init();
		foreach (SongGateString songGateString in this.strings)
		{
			songGateString.StrumEnd();
		}
	}

	// Token: 0x0600169F RID: 5791 RVA: 0x00065BF8 File Offset: 0x00063DF8
	public void QuickStrum()
	{
		this.Init();
		foreach (SongGateString songGateString in this.strings)
		{
			songGateString.QuickStrum();
		}
	}

	// Token: 0x04001519 RID: 5401
	[SerializeField]
	private List<SongGateString> strings;

	// Token: 0x0400151A RID: 5402
	[SerializeField]
	private bool getChildren;

	// Token: 0x0400151B RID: 5403
	private bool init;
}
