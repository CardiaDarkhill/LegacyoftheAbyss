using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200034C RID: 844
public abstract class MutableCollection<T> : Mutable where T : IMutable
{
	// Token: 0x06001D47 RID: 7495 RVA: 0x00087515 File Offset: 0x00085715
	protected virtual void Awake()
	{
		this.mutables.RemoveAll((T o) => o == null);
	}

	// Token: 0x06001D48 RID: 7496 RVA: 0x00087544 File Offset: 0x00085744
	public override void OnMuteStateChanged(bool muted)
	{
		foreach (T t in this.mutables)
		{
			t.SetMute(muted);
		}
	}

	// Token: 0x04001C86 RID: 7302
	[SerializeField]
	private List<T> mutables = new List<T>();
}
