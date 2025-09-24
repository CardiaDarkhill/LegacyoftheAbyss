using System;
using UnityEngine;

// Token: 0x0200035B RID: 859
public class OpeningGameplayCredits : MonoBehaviour
{
	// Token: 0x06001DA6 RID: 7590 RVA: 0x00088BE0 File Offset: 0x00086DE0
	private void Start()
	{
		this.pd = PlayerData.instance;
		if (!this.pd.openingCreditsPlayed)
		{
			if (this.animator)
			{
				this.animator.SetBool("playCredits", true);
			}
			this.pd.openingCreditsPlayed = true;
		}
	}

	// Token: 0x04001CD7 RID: 7383
	public Animator animator;

	// Token: 0x04001CD8 RID: 7384
	private PlayerData pd;
}
