using System;
using UnityEngine;

// Token: 0x02000383 RID: 899
[CreateAssetMenu(fileName = "New Boss Sequence", menuName = "Hollow Knight/Boss Sequence List")]
public class BossSequence : ScriptableObject
{
	// Token: 0x1700030D RID: 781
	// (get) Token: 0x06001EA0 RID: 7840 RVA: 0x0008C74E File Offset: 0x0008A94E
	public int Count
	{
		get
		{
			return this.bossScenes.Length;
		}
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x0008C758 File Offset: 0x0008A958
	public string GetSceneAt(int index)
	{
		return this.GetBossScene(index).sceneName;
	}

	// Token: 0x06001EA2 RID: 7842 RVA: 0x0008C766 File Offset: 0x0008A966
	public string GetSceneObjectName(int index)
	{
		return this.GetBossScene(index).name;
	}

	// Token: 0x06001EA3 RID: 7843 RVA: 0x0008C774 File Offset: 0x0008A974
	public bool CanLoad(int index)
	{
		return !this.GetBossScene(index).isHidden || this.GetBossScene(index).IsUnlocked(BossSceneCheckSource.Sequence);
	}

	// Token: 0x06001EA4 RID: 7844 RVA: 0x0008C794 File Offset: 0x0008A994
	public BossScene GetBossScene(int index)
	{
		BossScene bossScene = this.bossScenes[index];
		if (!bossScene.IsUnlockedSelf(BossSceneCheckSource.Sequence) && bossScene.baseBoss && bossScene.substituteBoss)
		{
			bossScene = bossScene.baseBoss;
		}
		return bossScene;
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x0008C7D0 File Offset: 0x0008A9D0
	public bool IsUnlocked()
	{
		if (this.useSceneUnlocks)
		{
			foreach (BossScene bossScene in this.bossScenes)
			{
				if (!bossScene.isHidden && !bossScene.IsUnlocked(BossSceneCheckSource.Sequence))
				{
					return false;
				}
			}
		}
		if (this.tests != null && this.tests.Length != 0)
		{
			BossScene.BossTest[] array2 = this.tests;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i].IsUnlocked())
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	// Token: 0x06001EA6 RID: 7846 RVA: 0x0008C846 File Offset: 0x0008AA46
	public bool IsSceneHidden(int index)
	{
		return this.GetBossScene(index).isHidden;
	}

	// Token: 0x04001D96 RID: 7574
	[SerializeField]
	private BossScene[] bossScenes;

	// Token: 0x04001D97 RID: 7575
	public bool useSceneUnlocks = true;

	// Token: 0x04001D98 RID: 7576
	public BossScene.BossTest[] tests;

	// Token: 0x04001D99 RID: 7577
	[Space]
	public string achievementKey;

	// Token: 0x04001D9A RID: 7578
	[Space]
	public string customEndScene;

	// Token: 0x04001D9B RID: 7579
	public string customEndScenePlayerData;

	// Token: 0x04001D9C RID: 7580
	[Header("Bindings")]
	public int nailDamage = 5;

	// Token: 0x04001D9D RID: 7581
	[Tooltip("If nail damage is already at or below the above nailDamage value, use a percentage instead.")]
	[Range(0f, 1f)]
	public float lowerNailDamagePercentage = 0.8f;

	// Token: 0x04001D9E RID: 7582
	public int maxHealth = 5;
}
