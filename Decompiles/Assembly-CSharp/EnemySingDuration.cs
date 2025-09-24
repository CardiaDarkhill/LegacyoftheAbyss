using System;
using UnityEngine;

// Token: 0x020002E2 RID: 738
public class EnemySingDuration : MonoBehaviour
{
	// Token: 0x06001A24 RID: 6692 RVA: 0x000786E7 File Offset: 0x000768E7
	private void Update()
	{
		if (this.cooldownTimer > 0f)
		{
			this.cooldownTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06001A25 RID: 6693 RVA: 0x00078708 File Offset: 0x00076908
	public void StartSingCooldown()
	{
		this.cooldownTimer = Random.Range(3f, 5f);
	}

	// Token: 0x06001A26 RID: 6694 RVA: 0x0007871F File Offset: 0x0007691F
	public bool CheckCanSing()
	{
		return this.cooldownTimer <= 0f;
	}

	// Token: 0x06001A27 RID: 6695 RVA: 0x00078731 File Offset: 0x00076931
	public void ResetSingCooldown()
	{
		this.cooldownTimer = 0f;
	}

	// Token: 0x04001913 RID: 6419
	private const float COOLDOWN_TIME_MIN = 3f;

	// Token: 0x04001914 RID: 6420
	private const float COOLDOWN_TIME_MAX = 5f;

	// Token: 0x04001915 RID: 6421
	private float cooldownTimer;
}
