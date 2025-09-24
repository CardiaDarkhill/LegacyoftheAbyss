using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x020003B6 RID: 950
public class HeroDamageSpawn : MonoBehaviour
{
	// Token: 0x06001FEE RID: 8174 RVA: 0x00091445 File Offset: 0x0008F645
	private void Awake()
	{
		this.hc = base.GetComponent<HeroController>();
		this.hc.OnTakenDamageExtra += this.OnTakenDamage;
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x0009146A File Offset: 0x0008F66A
	private void OnDestroy()
	{
		if (this.hc != null)
		{
			this.hc.OnTakenDamageExtra -= this.OnTakenDamage;
		}
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x00091494 File Offset: 0x0008F694
	private void OnTakenDamage(HeroController.DamageInfo damageInfo)
	{
		if (Time.timeAsDouble < this.canSpawnTime)
		{
			return;
		}
		this.canSpawnTime = Time.timeAsDouble + (double)this.hc.INVUL_TIME;
		bool flag = this.hc.playerData.health <= 0;
		HeroDamageSpawn.Spawn[] array = this.spawns;
		int i = 0;
		while (i < array.Length)
		{
			HeroDamageSpawn.Spawn spawn = array[i];
			HazardType hazardType = damageInfo.hazardType;
			if (hazardType != HazardType.SPIKES)
			{
				if (hazardType != HazardType.ZAP)
				{
					if (hazardType != HazardType.SINK)
					{
						goto IL_B5;
					}
					if (!spawn.ignoreFlags.HasFlag(HeroDamageSpawn.IgnoreFlags.Sink))
					{
						goto IL_B5;
					}
				}
				else if (!spawn.ignoreFlags.HasFlag(HeroDamageSpawn.IgnoreFlags.Zap))
				{
					goto IL_B5;
				}
			}
			else if (!spawn.ignoreFlags.HasFlag(HeroDamageSpawn.IgnoreFlags.Spike))
			{
				goto IL_B5;
			}
			IL_F3:
			i++;
			continue;
			IL_B5:
			if (spawn.EquippedTool && spawn.EquippedTool.IsEquipped && (spawn.SpawnOnDeath || !flag))
			{
				spawn.Prefab.Spawn(base.transform.position);
				goto IL_F3;
			}
			goto IL_F3;
		}
	}

	// Token: 0x04001EF4 RID: 7924
	[SerializeField]
	private HeroDamageSpawn.Spawn[] spawns;

	// Token: 0x04001EF5 RID: 7925
	private double canSpawnTime;

	// Token: 0x04001EF6 RID: 7926
	private HeroController hc;

	// Token: 0x0200166A RID: 5738
	[Serializable]
	private class Spawn
	{
		// Token: 0x04008AB6 RID: 35510
		public ToolItem EquippedTool;

		// Token: 0x04008AB7 RID: 35511
		public GameObject Prefab;

		// Token: 0x04008AB8 RID: 35512
		public bool SpawnOnDeath;

		// Token: 0x04008AB9 RID: 35513
		public HeroDamageSpawn.IgnoreFlags ignoreFlags;
	}

	// Token: 0x0200166B RID: 5739
	[Flags]
	[Serializable]
	public enum IgnoreFlags
	{
		// Token: 0x04008ABB RID: 35515
		None = 0,
		// Token: 0x04008ABC RID: 35516
		Zap = 1,
		// Token: 0x04008ABD RID: 35517
		Sink = 2,
		// Token: 0x04008ABE RID: 35518
		Spike = 4
	}
}
