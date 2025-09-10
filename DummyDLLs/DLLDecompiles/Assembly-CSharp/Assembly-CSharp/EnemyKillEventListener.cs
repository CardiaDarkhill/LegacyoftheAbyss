using System;
using UnityEngine;

// Token: 0x020002E0 RID: 736
public class EnemyKillEventListener : MonoBehaviour
{
	// Token: 0x06001A1A RID: 6682 RVA: 0x00078544 File Offset: 0x00076744
	private void Awake()
	{
		this.healthManager = base.GetComponent<HealthManager>();
	}

	// Token: 0x06001A1B RID: 6683 RVA: 0x00078552 File Offset: 0x00076752
	private void OnEnable()
	{
		if (this.killEvent)
		{
			this.killEvent.ReceivedEvent += this.Die;
		}
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x00078578 File Offset: 0x00076778
	private void OnDisable()
	{
		if (this.killEvent)
		{
			this.killEvent.ReceivedEvent -= this.Die;
		}
	}

	// Token: 0x06001A1D RID: 6685 RVA: 0x000785A0 File Offset: 0x000767A0
	private void Die()
	{
		if (this.healthManager)
		{
			this.healthManager.Hit(new HitInstance
			{
				AttackType = AttackTypes.Generic,
				CircleDirection = false,
				DamageDealt = 9999,
				Direction = 0f,
				IgnoreInvulnerable = true,
				Multiplier = 1f
			});
		}
	}

	// Token: 0x0400190D RID: 6413
	public EventRegister killEvent;

	// Token: 0x0400190E RID: 6414
	private HealthManager healthManager;
}
