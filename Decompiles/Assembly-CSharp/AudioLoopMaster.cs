using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class AudioLoopMaster : MonoBehaviour
{
	// Token: 0x060009E4 RID: 2532 RVA: 0x0002CD50 File Offset: 0x0002AF50
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.gm = GameManager.instance;
		this.syncAction = new AudioLoopMaster.AudioSyncInfo(this, this.action);
		this.syncSub = new AudioLoopMaster.AudioSyncInfo(this, this.sub);
		this.syncMainAlt = new AudioLoopMaster.AudioSyncInfo(this, this.mainAlt);
		this.syncTension = new AudioLoopMaster.AudioSyncInfo(this, this.tension);
		this.syncExtra = new AudioLoopMaster.AudioSyncInfo(this, this.extra);
		this.syncList.Add(this.syncAction);
		this.syncList.Add(this.syncSub);
		this.syncList.Add(this.syncMainAlt);
		this.syncList.Add(this.syncTension);
		this.syncList.Add(this.syncExtra);
		this.gm.NextSceneWillActivate += this.OnNextSceneWillActivate;
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x0002CE3A File Offset: 0x0002B03A
	private void OnDestroy()
	{
		if (this.gm)
		{
			this.gm.NextSceneWillActivate -= this.OnNextSceneWillActivate;
		}
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x0002CE60 File Offset: 0x0002B060
	private void OnNextSceneWillActivate()
	{
		this.ReSync();
		this.AllowSync();
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x0002CE6E File Offset: 0x0002B06E
	private void ReSync()
	{
		this.syncAction.AllowResync();
		this.syncSub.AllowResync();
		this.syncMainAlt.AllowResync();
		this.syncTension.AllowResync();
		this.syncExtra.AllowResync();
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x0002CEA8 File Offset: 0x0002B0A8
	private void Update()
	{
		if (CheatManager.DisableMusicSync)
		{
			return;
		}
		if (!this.isSyncing)
		{
			if (this.syncCooldown > 0f)
			{
				this.syncCooldown -= Time.deltaTime;
				return;
			}
			if (this.gm.IsInSceneTransition)
			{
				return;
			}
			this.ReSync();
			this.syncCooldown = 10f;
		}
		else if (Time.realtimeSinceStartup < this.lastSyncTime)
		{
			return;
		}
		if (!this.isSyncing || this.fullSync)
		{
			AudioClip clip = this.audioSource.clip;
			if (clip)
			{
				this.clipFrequency = clip.frequency;
			}
			else
			{
				this.clipFrequency = -1;
			}
		}
		this.isSyncing = false;
		for (int i = 0; i < this.syncList.Count; i++)
		{
			if (this.syncList[(i + this.index) % this.syncList.Count].Sync())
			{
				this.isSyncing = true;
				if (!this.fullSync)
				{
					this.index = (this.index + i) % this.syncList.Count;
					break;
				}
			}
		}
		this.lastSyncTime = Time.realtimeSinceStartup + 1f;
		this.fullSync = false;
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0002CFD3 File Offset: 0x0002B1D3
	public void AllowSync()
	{
		this.syncCooldown = 0f;
		this.fullSync = true;
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x0002CFE7 File Offset: 0x0002B1E7
	public void SetSyncAction(bool set)
	{
		this.syncAction.SetSync(set);
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x0002CFF5 File Offset: 0x0002B1F5
	public void SetSyncSub(bool set)
	{
		this.syncSub.SetSync(set);
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0002D003 File Offset: 0x0002B203
	public void SetSyncMainAlt(bool set)
	{
		this.syncMainAlt.SetSync(set);
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x0002D011 File Offset: 0x0002B211
	public void SetSyncTension(bool set)
	{
		this.syncTension.SetSync(set);
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x0002D01F File Offset: 0x0002B21F
	public void SetSyncExtra(bool set)
	{
		this.syncExtra.SetSync(set);
	}

	// Token: 0x0400096B RID: 2411
	private AudioSource audioSource;

	// Token: 0x0400096C RID: 2412
	public AudioSource action;

	// Token: 0x0400096D RID: 2413
	public AudioSource sub;

	// Token: 0x0400096E RID: 2414
	public AudioSource mainAlt;

	// Token: 0x0400096F RID: 2415
	public AudioSource tension;

	// Token: 0x04000970 RID: 2416
	public AudioSource extra;

	// Token: 0x04000971 RID: 2417
	private float syncCooldown;

	// Token: 0x04000972 RID: 2418
	private AudioLoopMaster.AudioSyncInfo syncAction;

	// Token: 0x04000973 RID: 2419
	private AudioLoopMaster.AudioSyncInfo syncSub;

	// Token: 0x04000974 RID: 2420
	private AudioLoopMaster.AudioSyncInfo syncMainAlt;

	// Token: 0x04000975 RID: 2421
	private AudioLoopMaster.AudioSyncInfo syncTension;

	// Token: 0x04000976 RID: 2422
	private AudioLoopMaster.AudioSyncInfo syncExtra;

	// Token: 0x04000977 RID: 2423
	private bool isSyncing;

	// Token: 0x04000978 RID: 2424
	private float lastSyncTime;

	// Token: 0x04000979 RID: 2425
	private int clipFrequency;

	// Token: 0x0400097A RID: 2426
	private GameManager gm;

	// Token: 0x0400097B RID: 2427
	private bool fullSync;

	// Token: 0x0400097C RID: 2428
	private List<AudioLoopMaster.AudioSyncInfo> syncList = new List<AudioLoopMaster.AudioSyncInfo>();

	// Token: 0x0400097D RID: 2429
	private int index;

	// Token: 0x02001474 RID: 5236
	private sealed class AudioSyncInfo
	{
		// Token: 0x17000CD7 RID: 3287
		// (get) Token: 0x06008392 RID: 33682 RVA: 0x002693E9 File Offset: 0x002675E9
		public AudioSource AudioSource
		{
			get
			{
				return this.audioSource;
			}
		}

		// Token: 0x06008393 RID: 33683 RVA: 0x002693F1 File Offset: 0x002675F1
		public AudioSyncInfo(AudioLoopMaster audioLoopMaster, AudioSource audioSource)
		{
			this.audioLoopMaster = audioLoopMaster;
			this.audioSource = audioSource;
		}

		// Token: 0x06008394 RID: 33684 RVA: 0x00269407 File Offset: 0x00267607
		public void ResetSyncStatus()
		{
			this.syncCount = 0;
			this.resync = false;
			this.useTime = false;
		}

		// Token: 0x06008395 RID: 33685 RVA: 0x0026941E File Offset: 0x0026761E
		public void AllowResync()
		{
			this.syncCount = 0;
			this.resync = true;
		}

		// Token: 0x06008396 RID: 33686 RVA: 0x00269430 File Offset: 0x00267630
		public void SetSync(bool shouldSync)
		{
			this.shouldSync = shouldSync;
			this.hasSynced = false;
			if (shouldSync)
			{
				this.ResetSyncStatus();
				if (this.AudioSource.clip)
				{
					this.maxSamples = this.AudioSource.clip.samples;
					this.frequency = this.AudioSource.clip.frequency;
					this.sampleDif = Mathf.CeilToInt((float)((double)this.frequency * 0.03));
					return;
				}
				this.maxSamples = 0;
				this.shouldSync = false;
				this.sampleDif = Mathf.CeilToInt(1230f);
			}
		}

		// Token: 0x06008397 RID: 33687 RVA: 0x002694D4 File Offset: 0x002676D4
		public bool Sync()
		{
			if (!this.shouldSync)
			{
				return false;
			}
			if (!this.useTime)
			{
				this.useTime = (this.frequency != this.audioLoopMaster.clipFrequency);
			}
			if (!this.useTime)
			{
				int timeSamples = this.audioLoopMaster.audioSource.timeSamples;
				if (Mathf.Abs(this.AudioSource.timeSamples - timeSamples) <= this.sampleDif)
				{
					this.syncCount = 0;
					return false;
				}
				if (timeSamples > this.maxSamples)
				{
					this.syncCount = 0;
					return false;
				}
			}
			else
			{
				float time = this.audioLoopMaster.audioSource.time;
				float time2 = this.audioSource.time;
				if (Mathf.Abs(time - time2) < 0.025f)
				{
					this.syncCount = 0;
					return false;
				}
				if (time > this.audioSource.clip.length)
				{
					this.syncCount = 0;
					return false;
				}
			}
			if (this.hasSynced && this.resync)
			{
				this.resync = false;
				this.syncCount = 3;
			}
			else if (this.syncCount >= 6)
			{
				this.syncCount = 0;
			}
			this.hasSynced = true;
			this.resync = false;
			this.syncCount++;
			if (!this.useTime)
			{
				this.AudioSource.timeSamples = this.audioLoopMaster.audioSource.timeSamples;
			}
			else
			{
				this.AudioSource.time = this.audioLoopMaster.audioSource.time;
			}
			return true;
		}

		// Token: 0x0400833B RID: 33595
		private const float ALLOWABLE_TIME_DIF = 0.025f;

		// Token: 0x0400833C RID: 33596
		private const double ALLOWABLE_SAMPLE_DIF = 0.03;

		// Token: 0x0400833D RID: 33597
		private const int STAGE_1_SYNC_ATTEMPTS = 3;

		// Token: 0x0400833E RID: 33598
		private const int STAGE_2_SYNC_ATTEMPTS = 6;

		// Token: 0x0400833F RID: 33599
		private const float STAGE_2_WAIT = 5f;

		// Token: 0x04008340 RID: 33600
		private AudioLoopMaster audioLoopMaster;

		// Token: 0x04008341 RID: 33601
		private AudioSource audioSource;

		// Token: 0x04008342 RID: 33602
		private bool shouldSync;

		// Token: 0x04008343 RID: 33603
		private int maxSamples;

		// Token: 0x04008344 RID: 33604
		private int frequency;

		// Token: 0x04008345 RID: 33605
		private int sampleDif;

		// Token: 0x04008346 RID: 33606
		private int syncCount;

		// Token: 0x04008347 RID: 33607
		private bool useTime;

		// Token: 0x04008348 RID: 33608
		private bool resync;

		// Token: 0x04008349 RID: 33609
		private bool hasSynced;
	}
}
