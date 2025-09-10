using System;
using UnityEngine;

// Token: 0x0200036D RID: 877
public class TitleLogo : MonoBehaviour
{
	// Token: 0x06001E22 RID: 7714 RVA: 0x0008B2B0 File Offset: 0x000894B0
	public void AnimationFinished()
	{
		this.startManager.SwitchToMenuScene();
	}

	// Token: 0x04001D3C RID: 7484
	public StartManager startManager;
}
