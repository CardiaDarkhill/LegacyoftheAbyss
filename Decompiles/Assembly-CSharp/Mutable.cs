using System;
using UnityEngine;

// Token: 0x0200034B RID: 843
public abstract class Mutable : MonoBehaviour, IMutable
{
	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06001D42 RID: 7490 RVA: 0x000874DE File Offset: 0x000856DE
	public bool Muted
	{
		get
		{
			return this.muted;
		}
	}

	// Token: 0x06001D43 RID: 7491 RVA: 0x000874E6 File Offset: 0x000856E6
	protected virtual void Start()
	{
		this.OnMuteStateChanged(this.muted);
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x000874F4 File Offset: 0x000856F4
	public void SetMute(bool muted)
	{
		if (this.muted != muted)
		{
			this.muted = muted;
			this.OnMuteStateChanged(muted);
		}
	}

	// Token: 0x06001D45 RID: 7493
	public abstract void OnMuteStateChanged(bool muted);

	// Token: 0x04001C85 RID: 7301
	[SerializeField]
	private bool muted;
}
