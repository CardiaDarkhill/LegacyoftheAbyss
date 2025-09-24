using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200013C RID: 316
public class VentRegion : MonoBehaviour
{
	// Token: 0x060009BE RID: 2494 RVA: 0x0002C408 File Offset: 0x0002A608
	private void Awake()
	{
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HERO HAZARD DEATH").ReceivedEvent += this.HeroHazardDeath;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HAZARD RESPAWNED").ReceivedEvent += this.HeroHazardRespawned;
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x0002C458 File Offset: 0x0002A658
	private void Start()
	{
		this.gm = GameManager.instance;
		this.gm.SceneInit += this.OnSceneInit;
		if (this.gm.hero_ctrl)
		{
			this.OnSceneInit();
		}
		this.windPlayerSource = this.windPlayer.GetComponent<AudioSource>();
		this.windPlayerTransform = this.windPlayer.transform;
		this.OnInsideStateChanged(this.trackTrigger.IsInside);
		this.trackTrigger.InsideStateChanged += this.OnInsideStateChanged;
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x0002C4E9 File Offset: 0x0002A6E9
	private void OnDestroy()
	{
		if (this.gm)
		{
			this.gm.SceneInit -= this.OnSceneInit;
			this.gm = null;
		}
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x0002C516 File Offset: 0x0002A716
	private void OnSceneInit()
	{
		this.heroTransform = this.gm.hero_ctrl.transform;
		if (this.resetToSceneEnviro)
		{
			this.enviroSnapshotExit = this.gm.GetSceneManager().GetComponent<CustomSceneManager>().enviroSnapshot;
		}
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x0002C551 File Offset: 0x0002A751
	private void OnInsideStateChanged(bool isInside)
	{
		if (this.invert)
		{
			isInside = !isInside;
		}
		if (isInside)
		{
			if (this.active && !this.entered)
			{
				this.EnterRegion();
				return;
			}
		}
		else if (this.active && this.entered)
		{
			this.ExitRegion();
		}
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x0002C594 File Offset: 0x0002A794
	private void EnterRegion()
	{
		if (this.enviroSnapshotEnter != null)
		{
			this.enviroSnapshotEnter.TransitionTo(this.transitionTime);
		}
		this.windPlayerSource.Play();
		this.windVolume = 0f;
		EventRegister.SendEvent("VENT REGION ENTER", null);
		if (!this.hasEntered)
		{
			GameManager.instance.GetSceneManager().GetComponent<CustomSceneManager>().CancelEnviroSnapshot();
			this.hasEntered = true;
		}
		if (this.rumbleManager)
		{
			this.rumbleManager.AddMagnitudeMultiplier(this, this.rumbleMagnitudeMultiplier);
		}
		this.entered = true;
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x0002C62C File Offset: 0x0002A82C
	private void ExitRegion()
	{
		if (this.enviroSnapshotExit != null && this.resetToSceneEnviro)
		{
			this.enviroSnapshotExit.TransitionTo(this.transitionTime);
		}
		EventRegister.SendEvent("VENT REGION EXIT", null);
		if (this.rumbleManager)
		{
			this.rumbleManager.RemoveMagnitudeMultiplier(this);
		}
		this.entered = false;
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x0002C68C File Offset: 0x0002A88C
	private void Update()
	{
		if (!this.active)
		{
			return;
		}
		if (this.entered)
		{
			if (this.heroTransform)
			{
				this.windPlayerTransform.position = this.heroTransform.position;
			}
			if (this.windVolume < 1f)
			{
				this.windVolume += this.transitionTime * Time.deltaTime;
				if (this.windVolume > 1f)
				{
					this.windVolume = 1f;
				}
				this.windPlayerSource.volume = this.windVolume;
				return;
			}
		}
		else if (this.windVolume > 0f)
		{
			this.windVolume -= this.transitionTime * Time.deltaTime;
			if (this.windVolume < 0f)
			{
				this.windVolume = 0f;
			}
			this.windPlayerSource.volume = this.windVolume;
		}
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0002C76F File Offset: 0x0002A96F
	public void HeroHazardDeath()
	{
		this.active = false;
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x0002C778 File Offset: 0x0002A978
	public void HeroHazardRespawned()
	{
		this.active = true;
	}

	// Token: 0x0400094D RID: 2381
	private const string ENTER_EVENT = "VENT REGION ENTER";

	// Token: 0x0400094E RID: 2382
	private const string EXIT_EVENT = "VENT REGION EXIT";

	// Token: 0x0400094F RID: 2383
	[SerializeField]
	private TrackTriggerObjects trackTrigger;

	// Token: 0x04000950 RID: 2384
	[SerializeField]
	private bool invert;

	// Token: 0x04000951 RID: 2385
	[Space]
	[SerializeField]
	private AudioMixerSnapshot enviroSnapshotEnter;

	// Token: 0x04000952 RID: 2386
	[SerializeField]
	private float transitionTime = 0.5f;

	// Token: 0x04000953 RID: 2387
	[SerializeField]
	private bool resetToSceneEnviro;

	// Token: 0x04000954 RID: 2388
	[SerializeField]
	private GameObject windPlayer;

	// Token: 0x04000955 RID: 2389
	[Space]
	[SerializeField]
	private WorldRumbleManager rumbleManager;

	// Token: 0x04000956 RID: 2390
	[SerializeField]
	private float rumbleMagnitudeMultiplier = 1f;

	// Token: 0x04000957 RID: 2391
	private AudioSource windPlayerSource;

	// Token: 0x04000958 RID: 2392
	private AudioMixerSnapshot enviroSnapshotExit;

	// Token: 0x04000959 RID: 2393
	private bool hasEntered;

	// Token: 0x0400095A RID: 2394
	private bool entered;

	// Token: 0x0400095B RID: 2395
	private bool active = true;

	// Token: 0x0400095C RID: 2396
	private float windVolume;

	// Token: 0x0400095D RID: 2397
	private Transform windPlayerTransform;

	// Token: 0x0400095E RID: 2398
	private Transform heroTransform;

	// Token: 0x0400095F RID: 2399
	private GameManager gm;
}
