using System;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class MusicSnapshotMarker : SnapshotMarker
{
	// Token: 0x060008FD RID: 2301 RVA: 0x00029E6F File Offset: 0x0002806F
	protected override void AddMarker()
	{
		AudioManager.AddMusicMarker(this);
		if (this.forceUpdateOnAdd)
		{
			AudioManager.ForceMarkerUpdate();
		}
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00029E84 File Offset: 0x00028084
	protected override void RemoveMarker()
	{
		AudioManager.RemoveMusicMarker(this);
	}

	// Token: 0x040008B7 RID: 2231
	[SerializeField]
	private bool forceUpdateOnAdd;
}
