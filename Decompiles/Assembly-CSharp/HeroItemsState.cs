using System;

// Token: 0x02000191 RID: 401
[Serializable]
public struct HeroItemsState
{
	// Token: 0x06000F8F RID: 3983 RVA: 0x0004B0AC File Offset: 0x000492AC
	public static HeroItemsState Record(HeroController hc)
	{
		PlayerData playerData = hc.playerData;
		return new HeroItemsState
		{
			IsRecorded = true,
			Health = playerData.health,
			Silk = playerData.silk,
			Rosaries = playerData.geo,
			ShellShards = playerData.ShellShards
		};
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x0004B108 File Offset: 0x00049308
	public void Apply(HeroController hc)
	{
		if (!this.IsRecorded)
		{
			return;
		}
		PlayerData playerData = hc.playerData;
		int num = this.Health - playerData.health;
		playerData.health = this.Health;
		playerData.silk = this.Silk;
		playerData.geo = this.Rosaries;
		playerData.ShellShards = this.ShellShards;
		if (this.DoFullHeal)
		{
			int healthBlue = playerData.healthBlue;
			playerData.MaxHealth();
			playerData.healthBlue = healthBlue;
			num = playerData.health - this.Health;
		}
		this.IsRecorded = false;
		if (num <= 0)
		{
			if (num < 0)
			{
				EventRegister.SendEvent(EventRegisterEvents.HealthUpdate, null);
			}
		}
		else
		{
			EventRegister.SendEvent(EventRegisterEvents.HeroHealed, null);
		}
		if (hc != null)
		{
			hc.SuppressRefillSound(2);
		}
	}

	// Token: 0x04000F25 RID: 3877
	public bool IsRecorded;

	// Token: 0x04000F26 RID: 3878
	public int Health;

	// Token: 0x04000F27 RID: 3879
	public int Silk;

	// Token: 0x04000F28 RID: 3880
	public int Rosaries;

	// Token: 0x04000F29 RID: 3881
	public int ShellShards;

	// Token: 0x04000F2A RID: 3882
	public bool DoFullHeal;
}
