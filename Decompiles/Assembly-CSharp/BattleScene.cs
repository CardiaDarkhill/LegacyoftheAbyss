using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000298 RID: 664
public class BattleScene : MonoBehaviour
{
	// Token: 0x06001727 RID: 5927 RVA: 0x00068734 File Offset: 0x00066934
	private void Awake()
	{
		PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
		if (!string.IsNullOrEmpty(this.setPDBoolOnEnd))
		{
			if (component)
			{
				Object.Destroy(component);
			}
		}
		else if (component != null)
		{
			component.OnGetSaveState += delegate(out bool val)
			{
				val = this.completed;
			};
			component.OnSetSaveState += delegate(bool val)
			{
				if (!val)
				{
					return;
				}
				this.completed = true;
				this.BattleCompleted();
			};
		}
		if (this.musicCueStart)
		{
			this.musicCueStart.Preload(base.gameObject);
		}
		if (this.musicCueEnd)
		{
			this.musicCueEnd.Preload(base.gameObject);
		}
		this.audioSource = base.GetComponent<AudioSource>();
		foreach (BattleWave battleWave in this.waves)
		{
			battleWave.Init(this);
			if (this.toggleWavesAwake)
			{
				battleWave.SetActive(true);
			}
		}
		this.initialisables = base.GetComponentsInChildren<IInitialisable>(true);
		if (this.initialisables != null)
		{
			for (int i = 0; i < this.initialisables.Length; i++)
			{
				this.initialisables[i].OnAwake();
			}
		}
		this.boxCollider2D = base.GetComponent<BoxCollider2D>();
		this.polygonCollider2D = base.GetComponent<PolygonCollider2D>();
		foreach (object obj in this.gates.transform)
		{
			PlayMakerFSM component2 = ((Transform)obj).GetComponent<PlayMakerFSM>();
			if (component2)
			{
				this.gateFsms.Add(component2);
			}
		}
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x000688E8 File Offset: 0x00066AE8
	private void Start()
	{
		if (this.toggleWavesAwake)
		{
			foreach (BattleWave battleWave in this.waves)
			{
				battleWave.SetActive(false);
			}
		}
		if (this.initialisables != null)
		{
			for (int i = 0; i < this.initialisables.Length; i++)
			{
				this.initialisables[i].OnStart();
			}
			PersonalObjectPool[] componentsInChildren = base.GetComponentsInChildren<PersonalObjectPool>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].OnStart();
			}
		}
		this.initialisables = null;
		if (this.activeDuringBattle)
		{
			this.activeDuringBattle.SetActive(false);
		}
		string.IsNullOrEmpty(this.setPDBoolOnEnd);
		PlayerData.instance.GetBool(this.setPDBoolOnEnd);
		if (!string.IsNullOrEmpty(this.setPDBoolOnEnd) && PlayerData.instance.GetBool(this.setPDBoolOnEnd))
		{
			this.completed = true;
			this.BattleCompleted();
		}
		base.StartCoroutine(this.CheckCompletion());
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x00068A00 File Offset: 0x00066C00
	private void OnDisable()
	{
		this.checkEnemyRoutine = null;
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x00068A0C File Offset: 0x00066C0C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("Player"))
		{
			return;
		}
		HeroController component = collision.GetComponent<HeroController>();
		if (component && component.isHeroInPosition && this.currentWave == 0 && !this.completed && this.loopsUntilDeactivate <= 0)
		{
			this.StartBattle();
		}
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x00068A60 File Offset: 0x00066C60
	public void Update()
	{
		if (this.loopsUntilDeactivate > 0)
		{
			this.loopsUntilDeactivate--;
			if (this.loopsUntilDeactivate <= 0)
			{
				foreach (BattleWave battleWave in this.waves)
				{
					battleWave.SetActive(false);
				}
			}
		}
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x00068AD4 File Offset: 0x00066CD4
	public void SendEventToChildren(string eventName)
	{
		foreach (PlayMakerFSM playMakerFSM in this.gateFsms)
		{
			playMakerFSM.SendEvent(eventName);
		}
	}

	// Token: 0x0600172D RID: 5933 RVA: 0x00068B28 File Offset: 0x00066D28
	public void LockInBattle()
	{
		if (this.camLocks != null)
		{
			this.camLocks.SetActive(true);
		}
		this.SendEventToChildren("BG CLOSE");
	}

	// Token: 0x0600172E RID: 5934 RVA: 0x00068B4F File Offset: 0x00066D4F
	public void StartBattle()
	{
		base.StartCoroutine(this.DoStartBattle());
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x00068B5E File Offset: 0x00066D5E
	public IEnumerator DoStartBattle()
	{
		if (this.started)
		{
			yield break;
		}
		if (this.boxCollider2D != null)
		{
			this.boxCollider2D.enabled = false;
		}
		if (this.polygonCollider2D != null)
		{
			this.polygonCollider2D.enabled = false;
		}
		this.LockInBattle();
		TrackingTrail.FadeDownAll();
		if (this.snapshotSilent != null && !this.noStartSilence)
		{
			this.snapshotSilent.TransitionTo(0.5f);
		}
		yield return new WaitForSeconds(this.battleStartPause);
		if (this.activeDuringBattle)
		{
			this.activeDuringBattle.SetActive(true);
		}
		EventRegister.SendEvent(this.battleStartEventRegister, null);
		this.started = true;
		base.StartCoroutine(this.StartWave(0));
		if (this.audioSource != null && this.battleStartClip != null)
		{
			this.audioSource.PlayOneShot(this.battleStartClip);
		}
		yield return new WaitForSeconds(this.battleStartClipPause);
		if (this.musicCueNone != null)
		{
			GameManager.instance.AudioManager.ApplyMusicCue(this.musicCueNone, 0f, 0f, false);
		}
		if (this.musicCueStart != null)
		{
			GameManager.instance.AudioManager.ApplyMusicCue(this.musicCueStart, 0f, 0f, false);
		}
		if (this.snapshotStart != null)
		{
			this.snapshotStart.TransitionTo(this.snapshotStartTime);
		}
		yield break;
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x00068B6D File Offset: 0x00066D6D
	private IEnumerator StartWave(int waveNumber)
	{
		BattleWave wave = this.waves[waveNumber];
		wave.SetActive(true);
		wave.WaveStarting();
		PlayMakerFSM waveFSM = wave.Fsm;
		bool hasWaveFSM = wave.HasFSM;
		if (hasWaveFSM)
		{
			waveFSM.SendEvent("WAVE QUEUED");
		}
		yield return new WaitForSeconds(wave.startDelay);
		if (hasWaveFSM)
		{
			waveFSM.SendEvent("WAVE START");
		}
		wave.WaveStarted(this.waves[waveNumber].activateEnemiesOnStart, ref this.currentEnemies);
		this.enemiesToNext = this.waves[waveNumber].remainingEnemyToEnd;
		this.currentWave = waveNumber;
		yield return new WaitForSeconds(1f);
		if (this.checkEnemyRoutine != null)
		{
			base.StopCoroutine(this.checkEnemyRoutine);
			this.checkEnemyRoutine = null;
		}
		this.DoCheckEnemies();
		yield break;
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x00068B83 File Offset: 0x00066D83
	public void DecrementEnemy()
	{
		this.currentEnemies--;
		this.lastDecrementFrame = Time.frameCount;
		if (this.currentEnemies <= this.enemiesToNext)
		{
			this.WaveEnd();
			return;
		}
		this.DoCheckEnemies();
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x00068BB9 File Offset: 0x00066DB9
	public void DecrementBigEnemy()
	{
		base.StartCoroutine(this.DoDecrementBigEnemy());
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x00068BC8 File Offset: 0x00066DC8
	public IEnumerator DoDecrementBigEnemy()
	{
		yield return new WaitForSeconds(1f);
		this.currentEnemies--;
		if (this.currentEnemies <= this.enemiesToNext)
		{
			this.WaveEnd();
		}
		else
		{
			this.DoCheckEnemies();
		}
		yield break;
	}

	// Token: 0x06001734 RID: 5940 RVA: 0x00068BD7 File Offset: 0x00066DD7
	public void WaveEnd()
	{
		if (this.started)
		{
			if (this.currentWave >= this.waves.Count - 1)
			{
				this.DoEndBattle();
				return;
			}
			base.StartCoroutine(this.StartWave(this.currentWave + 1));
		}
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x00068C12 File Offset: 0x00066E12
	public void CheckEnemies()
	{
		if (!this.started)
		{
			return;
		}
		this.DoCheckEnemies();
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x00068C23 File Offset: 0x00066E23
	private void DoCheckEnemies()
	{
		this.DoCheckEnemiesNew();
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x00068C2C File Offset: 0x00066E2C
	private void DoCheckEnemiesNew()
	{
		if (this.checkEnemyRoutine != null)
		{
			base.StopCoroutine(this.checkEnemyRoutine);
			this.checkEnemyRoutine = null;
		}
		int num = 0;
		for (int i = 0; i < this.currentWave + 1; i++)
		{
			num += this.waves[i].transform.childCount;
		}
		if (num <= this.enemiesToNext)
		{
			this.currentEnemies = num;
			this.WaveEnd();
			return;
		}
		if (this.checkEnemyRoutine == null)
		{
			this.checkEnemyRoutine = base.StartCoroutine(this.CheckEnemyCount());
		}
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x00068CB3 File Offset: 0x00066EB3
	public IEnumerator CheckEnemyCount()
	{
		yield return new WaitForSeconds(0.1f);
		int num = 0;
		for (int i = 0; i < this.currentWave + 1; i++)
		{
			num += this.waves[i].transform.childCount;
		}
		if (num <= this.enemiesToNext)
		{
			this.currentEnemies = num;
			this.WaveEnd();
		}
		this.checkEnemyRoutine = null;
		yield break;
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x00068CC2 File Offset: 0x00066EC2
	public void DoEndBattle()
	{
		base.StartCoroutine(this.EndBattle(Time.frameCount == this.lastDecrementFrame));
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x00068CDE File Offset: 0x00066EDE
	private IEnumerator EndBattle(bool waitExtra)
	{
		if (this.completed)
		{
			yield break;
		}
		GameManager gm = GameManager.instance;
		this.started = false;
		this.completed = true;
		if (this.snapshotSilent != null && !this.noEndSilence)
		{
			this.snapshotSilent.TransitionTo(0.25f);
		}
		if (!this.noEndSlomo)
		{
			gm.FreezeMoment(FreezeMomentTypes.EnemyBattleEndSlow, null);
			if (this.audioSource != null && this.slomoEffectClip != null)
			{
				this.audioSource.PlayOneShot(this.slomoEffectClip);
			}
		}
		if (!string.IsNullOrEmpty(this.finalEnemyKillEventRegister))
		{
			EventRegister.SendEvent(this.finalEnemyKillEventRegister, null);
		}
		if (this.garbageCollectOnEnd)
		{
			if (waitExtra)
			{
				yield return null;
			}
			yield return null;
			TimeManager.TimeControlInstance timeControl = TimeManager.CreateTimeControl(0f, TimeManager.TimeControlInstance.Type.Multiplicative);
			try
			{
				GCManager.Collect();
				yield return null;
			}
			finally
			{
				timeControl.Release();
			}
			timeControl = null;
		}
		yield return new WaitForSeconds(1f);
		if (this.battleEndClip != null)
		{
			AudioSource fanfareEnemyBattleClear = GameManager.instance.AudioManager.FanfareEnemyBattleClear;
			fanfareEnemyBattleClear.clip = this.battleEndClip;
			fanfareEnemyBattleClear.Play();
		}
		yield return new WaitForSeconds(this.battleEndPause);
		if (!string.IsNullOrEmpty(this.setPDBoolOnEnd))
		{
			gm.playerData.SetBool(this.setPDBoolOnEnd, true);
		}
		if (!string.IsNullOrEmpty(this.setExtraPDBoolOnEnd))
		{
			gm.playerData.SetBool(this.setExtraPDBoolOnEnd, true);
		}
		if (this.camLocks != null && !this.dontDisableCamlocksOnEnd)
		{
			this.camLocks.SetActive(false);
		}
		if (this.openGatesOnEnd)
		{
			this.SendEventToChildren("BG OPEN");
		}
		if (this.endScene)
		{
			this.endScene.SendEvent("BATTLE END");
		}
		if (this.activeDuringBattle)
		{
			this.activeDuringBattle.SetActive(false);
		}
		if (this.disableActiveBeforeBattleAtEnd)
		{
			this.activeBeforeBattle.SetActive(false);
		}
		if (!string.IsNullOrEmpty(this.battleEndEventRegister))
		{
			EventRegister.SendEvent(this.battleEndEventRegister, null);
		}
		if (this.musicCueEnd != null)
		{
			gm.AudioManager.ApplyMusicCue(this.musicCueEnd, 0f, 0f, false);
		}
		if (this.snapshotEnd != null)
		{
			this.snapshotEnd.TransitionTo(2f);
		}
		yield break;
		yield break;
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x00068CF4 File Offset: 0x00066EF4
	public IEnumerator CheckCompletion()
	{
		yield return new WaitForSeconds(0.5f);
		if (this.activeBeforeBattle != null)
		{
			if (this.completed)
			{
				this.activeBeforeBattle.SetActive(false);
			}
			else
			{
				this.activeBeforeBattle.SetActive(true);
			}
		}
		if (this.activeAfterBattle != null)
		{
			if (this.completed)
			{
				this.activeAfterBattle.SetActive(true);
			}
			else
			{
				this.activeAfterBattle.SetActive(false);
			}
		}
		if (this.inactiveAfterBattle != null)
		{
			if (this.completed)
			{
				this.inactiveAfterBattle.SetActive(false);
			}
			else
			{
				this.inactiveAfterBattle.SetActive(true);
			}
		}
		yield break;
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x00068D04 File Offset: 0x00066F04
	public void BattleCompleted()
	{
		if (this.boxCollider2D != null)
		{
			this.boxCollider2D.enabled = false;
		}
		if (this.polygonCollider2D != null)
		{
			this.polygonCollider2D.enabled = false;
		}
		if (this.endScene)
		{
			this.endScene.SendEvent("BATTLE COMPLETED");
		}
		if (this.openGatesOnEnd)
		{
			this.SendEventToChildren("BG QUICK OPEN");
		}
		foreach (BattleWave battleWave in this.waves)
		{
			battleWave.gameObject.SetActive(false);
		}
	}

	// Token: 0x040015B6 RID: 5558
	public List<BattleWave> waves;

	// Token: 0x040015B7 RID: 5559
	public GameObject camLocks;

	// Token: 0x040015B8 RID: 5560
	public GameObject gates;

	// Token: 0x040015B9 RID: 5561
	public bool openGatesOnEnd = true;

	// Token: 0x040015BA RID: 5562
	public GameObject activeBeforeBattle;

	// Token: 0x040015BB RID: 5563
	[ModifiableProperty]
	[Conditional("activeBeforeBattle", true, true, true)]
	public bool disableActiveBeforeBattleAtEnd;

	// Token: 0x040015BC RID: 5564
	public GameObject activeAfterBattle;

	// Token: 0x040015BD RID: 5565
	public GameObject inactiveAfterBattle;

	// Token: 0x040015BE RID: 5566
	public GameObject activeDuringBattle;

	// Token: 0x040015BF RID: 5567
	public PlayMakerFSM endScene;

	// Token: 0x040015C0 RID: 5568
	[PlayerDataField(typeof(bool), false)]
	public string setPDBoolOnEnd;

	// Token: 0x040015C1 RID: 5569
	[PlayerDataField(typeof(bool), false)]
	public string setExtraPDBoolOnEnd;

	// Token: 0x040015C2 RID: 5570
	public string battleStartEventRegister;

	// Token: 0x040015C3 RID: 5571
	public string finalEnemyKillEventRegister;

	// Token: 0x040015C4 RID: 5572
	public string battleEndEventRegister;

	// Token: 0x040015C5 RID: 5573
	public float battleStartPause;

	// Token: 0x040015C6 RID: 5574
	public float battleEndPause = 2f;

	// Token: 0x040015C7 RID: 5575
	public MusicCue musicCueStart;

	// Token: 0x040015C8 RID: 5576
	public MusicCue musicCueNone;

	// Token: 0x040015C9 RID: 5577
	public AudioMixerSnapshot snapshotStart;

	// Token: 0x040015CA RID: 5578
	public float snapshotStartTime = 1f;

	// Token: 0x040015CB RID: 5579
	public MusicCue musicCueEnd;

	// Token: 0x040015CC RID: 5580
	public AudioMixerSnapshot snapshotEnd;

	// Token: 0x040015CD RID: 5581
	public AudioMixerSnapshot snapshotSilent;

	// Token: 0x040015CE RID: 5582
	public AudioClip battleStartClip;

	// Token: 0x040015CF RID: 5583
	public float battleStartClipPause;

	// Token: 0x040015D0 RID: 5584
	public AudioClip battleEndClip;

	// Token: 0x040015D1 RID: 5585
	public AudioClip slomoEffectClip;

	// Token: 0x040015D2 RID: 5586
	public bool noStartSilence;

	// Token: 0x040015D3 RID: 5587
	public bool noEndSilence;

	// Token: 0x040015D4 RID: 5588
	public bool noEndSlomo;

	// Token: 0x040015D5 RID: 5589
	public bool garbageCollectOnEnd;

	// Token: 0x040015D6 RID: 5590
	public bool dontDisableCamlocksOnEnd;

	// Token: 0x040015D7 RID: 5591
	[SerializeField]
	private bool toggleWavesAwake;

	// Token: 0x040015D8 RID: 5592
	private AudioSource audioSource;

	// Token: 0x040015D9 RID: 5593
	private bool started;

	// Token: 0x040015DA RID: 5594
	private bool completed;

	// Token: 0x040015DB RID: 5595
	public int currentWave;

	// Token: 0x040015DC RID: 5596
	public int currentEnemies;

	// Token: 0x040015DD RID: 5597
	public int enemiesToNext;

	// Token: 0x040015DE RID: 5598
	private int loopsUntilDeactivate = 2;

	// Token: 0x040015DF RID: 5599
	private BoxCollider2D boxCollider2D;

	// Token: 0x040015E0 RID: 5600
	private PolygonCollider2D polygonCollider2D;

	// Token: 0x040015E1 RID: 5601
	private List<PlayMakerFSM> gateFsms = new List<PlayMakerFSM>();

	// Token: 0x040015E2 RID: 5602
	private IInitialisable[] initialisables;

	// Token: 0x040015E3 RID: 5603
	private int lastDecrementFrame;

	// Token: 0x040015E4 RID: 5604
	private Coroutine checkEnemyRoutine;
}
