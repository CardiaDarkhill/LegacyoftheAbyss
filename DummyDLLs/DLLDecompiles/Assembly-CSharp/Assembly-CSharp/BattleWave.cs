using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200029A RID: 666
public class BattleWave : MonoBehaviour
{
	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06001746 RID: 5958 RVA: 0x00068F3E File Offset: 0x0006713E
	// (set) Token: 0x06001747 RID: 5959 RVA: 0x00068F46 File Offset: 0x00067146
	public bool HasFSM { get; private set; }

	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06001748 RID: 5960 RVA: 0x00068F4F File Offset: 0x0006714F
	// (set) Token: 0x06001749 RID: 5961 RVA: 0x00068F57 File Offset: 0x00067157
	public PlayMakerFSM Fsm { get; private set; }

	// Token: 0x0600174A RID: 5962 RVA: 0x00068F60 File Offset: 0x00067160
	private void Start()
	{
		this.Init(null);
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x00068F6C File Offset: 0x0006716C
	public void Init(BattleScene battleScene)
	{
		if (this.init)
		{
			this.UpdateChildInfo();
			return;
		}
		if (battleScene == null)
		{
			battleScene = base.GetComponentInParent<BattleScene>();
		}
		this.battleScene = battleScene;
		this.init = true;
		this.enemies = base.GetComponentsInChildren<BattleSceneEnemy>();
		this.Fsm = base.GetComponent<PlayMakerFSM>();
		this.HasFSM = (this.Fsm != null);
		this.UpdateChildInfo();
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x00068FD8 File Offset: 0x000671D8
	private void UpdateChildInfo()
	{
		if (this.childCount == base.transform.childCount)
		{
			return;
		}
		this.childCount = base.transform.childCount;
		this.children.Clear();
		if (this.children.Capacity < this.childCount)
		{
			this.children.Capacity = this.childCount;
		}
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			this.children.Add(new BattleWave.ChildInfo(transform, this.battleScene.gameObject));
		}
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x0006909C File Offset: 0x0006729C
	public void SetActive(bool value)
	{
		this.Init(null);
		BattleSceneEnemy[] array = this.enemies;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value);
		}
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x000690CE File Offset: 0x000672CE
	public void WaveStarting()
	{
		if (!string.IsNullOrEmpty(this.startWaveEventRegister))
		{
			EventRegister.SendEvent(this.startWaveEventRegister, null);
		}
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x000690EC File Offset: 0x000672EC
	public void WaveStarted(bool activateEnemies, ref int currentEnemies)
	{
		this.Init(null);
		foreach (BattleWave.ChildInfo childInfo in this.children)
		{
			if (childInfo.hasHM)
			{
				HealthManager hm = childInfo.hm;
				if (this.clearDeathDrops)
				{
					hm.SetGeoSmall(0);
					hm.SetGeoMedium(0);
					hm.SetGeoLarge(0);
					hm.SetShellShards(0);
					hm.ClearItemDropsBattleScene();
				}
				currentEnemies++;
			}
			if (this.activateEnemiesOnStart)
			{
				childInfo.gameObject.SetActive(true);
			}
			if (childInfo.hasFSM)
			{
				FSMUtility.SendEventToGameObject(childInfo.gameObject, "BATTLE START", false);
			}
		}
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x000691AC File Offset: 0x000673AC
	private void OnTransformChildrenChanged()
	{
		if (base.transform.childCount == 0 && this.battleScene)
		{
			this.battleScene.CheckEnemies();
		}
	}

	// Token: 0x040015EB RID: 5611
	public float startDelay;

	// Token: 0x040015EC RID: 5612
	public int remainingEnemyToEnd;

	// Token: 0x040015ED RID: 5613
	public bool activateEnemiesOnStart;

	// Token: 0x040015EE RID: 5614
	[SerializeField]
	private string startWaveEventRegister;

	// Token: 0x040015EF RID: 5615
	[SerializeField]
	private bool clearDeathDrops = true;

	// Token: 0x040015F0 RID: 5616
	private BattleScene battleScene;

	// Token: 0x040015F1 RID: 5617
	private List<BattleWave.ChildInfo> children = new List<BattleWave.ChildInfo>();

	// Token: 0x040015F2 RID: 5618
	private BattleSceneEnemy[] enemies;

	// Token: 0x040015F5 RID: 5621
	private bool init;

	// Token: 0x040015F6 RID: 5622
	private int childCount;

	// Token: 0x02001568 RID: 5480
	private struct ChildInfo
	{
		// Token: 0x060086BC RID: 34492 RVA: 0x00273C98 File Offset: 0x00271E98
		public ChildInfo(Transform transform, GameObject battleScene)
		{
			this.gameObject = transform.gameObject;
			this.hm = transform.GetComponent<HealthManager>();
			this.hasHM = (this.hm != null);
			if (this.hasHM)
			{
				this.hm.ignorePersistence = true;
				this.hm.SetBattleScene(battleScene.gameObject);
			}
			this.hasFSM = (transform.GetComponent<PlayMakerFSM>() != null);
			SpecialQuestItemVariant[] componentsInChildren = transform.GetComponentsInChildren<SpecialQuestItemVariant>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetInactive();
			}
			EnemyDeathEffects component = this.gameObject.GetComponent<EnemyDeathEffects>();
			if (component != null)
			{
				component.DisableSpecialQuestDrops();
			}
		}

		// Token: 0x0400870C RID: 34572
		public GameObject gameObject;

		// Token: 0x0400870D RID: 34573
		public HealthManager hm;

		// Token: 0x0400870E RID: 34574
		public bool hasHM;

		// Token: 0x0400870F RID: 34575
		public bool hasFSM;
	}
}
