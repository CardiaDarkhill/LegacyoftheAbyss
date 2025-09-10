using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000312 RID: 786
public class SendEventToRange : FsmStateAction
{
	// Token: 0x06001BC6 RID: 7110 RVA: 0x000818DB File Offset: 0x0007FADB
	public override void Reset()
	{
		this.Range = null;
		this.EventName = null;
		this.ExcludeThis = null;
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x000818F4 File Offset: 0x0007FAF4
	public override void OnEnter()
	{
		GameObject safe = this.Range.GetSafe(this);
		if (safe)
		{
			PlayMakerEventRange component = safe.GetComponent<PlayMakerEventRange>();
			if (component)
			{
				component.SendEvent(this.EventName.Value, this.ExcludeThis.Value);
			}
		}
		base.Finish();
	}

	// Token: 0x04001AD0 RID: 6864
	[CheckForComponent(typeof(PlayMakerEventRange))]
	public FsmOwnerDefault Range;

	// Token: 0x04001AD1 RID: 6865
	public FsmString EventName;

	// Token: 0x04001AD2 RID: 6866
	public FsmBool ExcludeThis;
}
