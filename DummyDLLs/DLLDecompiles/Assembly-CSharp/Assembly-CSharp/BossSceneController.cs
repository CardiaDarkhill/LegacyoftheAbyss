using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200037D RID: 893
public class BossSceneController : MonoBehaviour
{
	// Token: 0x14000057 RID: 87
	// (add) Token: 0x06001E71 RID: 7793 RVA: 0x0008C16C File Offset: 0x0008A36C
	// (remove) Token: 0x06001E72 RID: 7794 RVA: 0x0008C1A4 File Offset: 0x0008A3A4
	public event Action OnBossesDead;

	// Token: 0x14000058 RID: 88
	// (add) Token: 0x06001E73 RID: 7795 RVA: 0x0008C1DC File Offset: 0x0008A3DC
	// (remove) Token: 0x06001E74 RID: 7796 RVA: 0x0008C214 File Offset: 0x0008A414
	public event Action OnBossSceneComplete;

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06001E75 RID: 7797 RVA: 0x0008C249 File Offset: 0x0008A449
	public static bool IsBossScene
	{
		get
		{
			return BossSceneController.Instance != null;
		}
	}

	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06001E76 RID: 7798 RVA: 0x0008C256 File Offset: 0x0008A456
	// (set) Token: 0x06001E77 RID: 7799 RVA: 0x0008C25E File Offset: 0x0008A45E
	public bool HasTransitionedIn { get; private set; }

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06001E78 RID: 7800 RVA: 0x0008C267 File Offset: 0x0008A467
	public static bool IsTransitioning
	{
		get
		{
			return BossSceneController.Instance != null && BossSceneController.Instance.isTransitioningOut;
		}
	}

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x06001E79 RID: 7801 RVA: 0x0008C282 File Offset: 0x0008A482
	// (set) Token: 0x06001E7A RID: 7802 RVA: 0x0008C28A File Offset: 0x0008A48A
	public bool CanTransition
	{
		get
		{
			return this.canTransition;
		}
		set
		{
			this.canTransition = value;
		}
	}

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x06001E7B RID: 7803 RVA: 0x0008C293 File Offset: 0x0008A493
	// (set) Token: 0x06001E7C RID: 7804 RVA: 0x0008C29B File Offset: 0x0008A49B
	public int BossLevel
	{
		get
		{
			return this.bossLevel;
		}
		set
		{
			this.bossLevel = value;
		}
	}

	// Token: 0x1700030A RID: 778
	// (get) Token: 0x06001E7D RID: 7805 RVA: 0x0008C2A4 File Offset: 0x0008A4A4
	// (set) Token: 0x06001E7E RID: 7806 RVA: 0x0008C2AC File Offset: 0x0008A4AC
	public string DreamReturnEvent { get; set; }

	// Token: 0x1700030B RID: 779
	// (get) Token: 0x06001E7F RID: 7807 RVA: 0x0008C2B5 File Offset: 0x0008A4B5
	// (set) Token: 0x06001E80 RID: 7808 RVA: 0x0008C2BD File Offset: 0x0008A4BD
	public Dictionary<HealthManager, BossSceneController.BossHealthDetails> BossHealthLookup { get; private set; }

	// Token: 0x06001E81 RID: 7809 RVA: 0x0008C2C8 File Offset: 0x0008A4C8
	private void Awake()
	{
		BossSceneController.Instance = this;
		this.BossHealthLookup = new Dictionary<HealthManager, BossSceneController.BossHealthDetails>();
		if (BossSceneController.SetupEvent == null)
		{
			BossSequenceController.CheckLoadSequence(this);
			this.doTransition = false;
		}
		if (BossSceneController.SetupEvent != null)
		{
			BossSceneController.SetupEventDelegate setupEvent = BossSceneController.SetupEvent;
			BossSceneController.SetupEvent = null;
			setupEvent(this);
			this.Setup();
		}
		if (this.customExitPoint)
		{
			this.customExitPoint.OnBeforeTransition += delegate()
			{
				this.restoreBindingsOnDestroy = false;
			};
		}
	}

	// Token: 0x06001E82 RID: 7810 RVA: 0x0008C33C File Offset: 0x0008A53C
	private void OnDestroy()
	{
		if (this.restoreBindingsOnDestroy)
		{
			this.RestoreBindings();
		}
		if (BossSceneController.Instance == this)
		{
			BossSceneController.Instance = null;
		}
	}

	// Token: 0x06001E83 RID: 7811 RVA: 0x0008C35F File Offset: 0x0008A55F
	private IEnumerator Start()
	{
		if (this.doTransition)
		{
			if (this.transitionPrefab)
			{
				Object.Instantiate<GameObject>(this.transitionPrefab);
			}
			else
			{
				Debug.LogError("Boss Scene Controller has no transition prefab assigned!", this);
			}
			if (this.doTransitionIn)
			{
				if (this.transitionInHoldTime > 0f)
				{
					EventRegister.SendEvent(EventRegisterEvents.GgTransitionOutInstant, null);
					yield return new WaitForSeconds(this.transitionInHoldTime);
				}
				EventRegister.SendEvent(EventRegisterEvents.GgTransitionIn, null);
			}
		}
		this.HasTransitionedIn = true;
		yield break;
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x0008C370 File Offset: 0x0008A570
	private void Update()
	{
		float timer = BossSequenceController.Timer;
		GameManager.instance.IncreaseGameTimer(ref timer);
		BossSequenceController.Timer = timer;
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x0008C398 File Offset: 0x0008A598
	private void Setup()
	{
		for (int i = 0; i < this.bosses.Length; i++)
		{
			if (this.bosses[i])
			{
				this.bossesLeft++;
				this.bosses[i].OnDeath += delegate()
				{
					this.bossesLeft--;
					this.CheckBossesDead();
				};
			}
		}
		if (!BossSequenceController.KnightDamaged && HeroController.instance)
		{
			HeroController.instance.OnTakenDamage += this.SetKnightDamaged;
			this.knightDamagedSubscribed = true;
		}
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x0008C41E File Offset: 0x0008A61E
	private void SetKnightDamaged()
	{
		BossSequenceController.KnightDamaged = true;
	}

	// Token: 0x06001E87 RID: 7815 RVA: 0x0008C426 File Offset: 0x0008A626
	private void CheckBossesDead()
	{
		if (this.bossesLeft > 0)
		{
			return;
		}
		this.EndBossScene();
	}

	// Token: 0x06001E88 RID: 7816 RVA: 0x0008C438 File Offset: 0x0008A638
	public void EndBossScene()
	{
		if (!this.endedScene)
		{
			this.endedScene = true;
			if (this.knightDamagedSubscribed)
			{
				HeroController.instance.OnTakenDamage -= this.SetKnightDamaged;
			}
			if (this.OnBossesDead != null)
			{
				this.OnBossesDead();
			}
			base.StartCoroutine(this.EndSceneDelayed());
		}
	}

	// Token: 0x06001E89 RID: 7817 RVA: 0x0008C492 File Offset: 0x0008A692
	private IEnumerator EndSceneDelayed()
	{
		yield return new WaitForSeconds(this.bossesDeadWaitTime);
		bool waitingForTransition = false;
		if (this.doTransitionOut)
		{
			if (this.endTransitionEvent)
			{
				this.isTransitioningOut = true;
				waitingForTransition = true;
				this.endTransitionEvent.ReceivedEvent += delegate()
				{
					waitingForTransition = false;
				};
			}
			else
			{
				Debug.LogError("Boss Scene controller has no end transition event assigned!", this);
			}
			if (BossSequenceController.IsInSequence)
			{
				if (BossSequenceController.IsLastBossScene)
				{
					EventRegister.SendEvent(EventRegisterEvents.GgTransitionFinal, null);
				}
				else
				{
					EventRegister.SendEvent(EventRegisterEvents.GgTransitionOut, null);
				}
			}
			else
			{
				EventRegister.SendEvent(EventRegisterEvents.GgTransitionOutStatue, null);
			}
		}
		yield return new WaitForSeconds(this.transitionOutHoldTime);
		while (waitingForTransition || HeroController.instance.cState.hazardRespawning || HeroController.instance.cState.hazardDeath || HeroController.instance.cState.spellQuake || !this.CanTransition)
		{
			yield return null;
		}
		if (HeroController.instance.cState.dead || HeroController.instance.cState.transitioning)
		{
			yield break;
		}
		this.restoreBindingsOnDestroy = false;
		if (this.OnBossSceneComplete != null)
		{
			this.OnBossSceneComplete();
		}
		yield break;
	}

	// Token: 0x06001E8A RID: 7818 RVA: 0x0008C4A4 File Offset: 0x0008A6A4
	public void DoDreamReturn()
	{
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Dream Return");
		if (playMakerFSM)
		{
			playMakerFSM.SendEvent(this.DreamReturnEvent);
		}
	}

	// Token: 0x06001E8B RID: 7819 RVA: 0x0008C4D6 File Offset: 0x0008A6D6
	public void ApplyBindings()
	{
		BossSequenceController.ApplyBindings();
	}

	// Token: 0x06001E8C RID: 7820 RVA: 0x0008C4DD File Offset: 0x0008A6DD
	public void RestoreBindings()
	{
		BossSequenceController.RestoreBindings();
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x0008C4E4 File Offset: 0x0008A6E4
	public static void ReportHealth(HealthManager healthManager, int baseHP, int adjustedHP, bool forceAdd = false)
	{
		if (BossSceneController.Instance)
		{
			bool flag = false;
			if (forceAdd)
			{
				flag = true;
			}
			else
			{
				HealthManager[] array = BossSceneController.Instance.bosses;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == healthManager)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				BossSceneController.Instance.BossHealthLookup[healthManager] = new BossSceneController.BossHealthDetails
				{
					baseHP = baseHP,
					adjustedHP = adjustedHP
				};
			}
		}
	}

	// Token: 0x04001D76 RID: 7542
	public static BossSceneController Instance;

	// Token: 0x04001D77 RID: 7543
	public static BossSceneController.SetupEventDelegate SetupEvent;

	// Token: 0x04001D7A RID: 7546
	public Transform heroSpawn;

	// Token: 0x04001D7B RID: 7547
	public GameObject transitionPrefab;

	// Token: 0x04001D7C RID: 7548
	public EventRegister endTransitionEvent;

	// Token: 0x04001D7D RID: 7549
	public bool doTransitionIn = true;

	// Token: 0x04001D7F RID: 7551
	public float transitionInHoldTime;

	// Token: 0x04001D80 RID: 7552
	public bool doTransitionOut = true;

	// Token: 0x04001D81 RID: 7553
	public float transitionOutHoldTime;

	// Token: 0x04001D82 RID: 7554
	private bool isTransitioningOut;

	// Token: 0x04001D83 RID: 7555
	private bool canTransition = true;

	// Token: 0x04001D84 RID: 7556
	[Space]
	[Tooltip("If scene end is handled elsewhere then leave empty. Only assign bosses here if you want the scene to end on HealthManager death event.")]
	public HealthManager[] bosses;

	// Token: 0x04001D85 RID: 7557
	private int bossesLeft;

	// Token: 0x04001D86 RID: 7558
	public float bossesDeadWaitTime = 5f;

	// Token: 0x04001D87 RID: 7559
	private int bossLevel;

	// Token: 0x04001D88 RID: 7560
	private bool endedScene;

	// Token: 0x04001D89 RID: 7561
	private bool knightDamagedSubscribed;

	// Token: 0x04001D8A RID: 7562
	private bool restoreBindingsOnDestroy = true;

	// Token: 0x04001D8B RID: 7563
	public TransitionPoint customExitPoint;

	// Token: 0x04001D8C RID: 7564
	private bool doTransition = true;

	// Token: 0x02001629 RID: 5673
	// (Invoke) Token: 0x06008920 RID: 35104
	public delegate void SetupEventDelegate(BossSceneController self);

	// Token: 0x0200162A RID: 5674
	public struct BossHealthDetails
	{
		// Token: 0x040089DB RID: 35291
		public int baseHP;

		// Token: 0x040089DC RID: 35292
		public int adjustedHP;
	}
}
