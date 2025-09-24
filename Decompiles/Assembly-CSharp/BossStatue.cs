using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000398 RID: 920
public class BossStatue : MonoBehaviour
{
	// Token: 0x14000059 RID: 89
	// (add) Token: 0x06001F06 RID: 7942 RVA: 0x0008DC64 File Offset: 0x0008BE64
	// (remove) Token: 0x06001F07 RID: 7943 RVA: 0x0008DC9C File Offset: 0x0008BE9C
	public event BossStatue.StatueSwapEndEvent OnStatueSwapFinished;

	// Token: 0x1400005A RID: 90
	// (add) Token: 0x06001F08 RID: 7944 RVA: 0x0008DCD4 File Offset: 0x0008BED4
	// (remove) Token: 0x06001F09 RID: 7945 RVA: 0x0008DD0C File Offset: 0x0008BF0C
	public event BossStatue.SeenNewStatueEvent OnSeenNewStatue;

	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06001F0A RID: 7946 RVA: 0x0008DD41 File Offset: 0x0008BF41
	// (set) Token: 0x06001F0B RID: 7947 RVA: 0x0008DD50 File Offset: 0x0008BF50
	public bool UsingDreamVersion
	{
		get
		{
			return this.StatueState.usingAltVersion;
		}
		private set
		{
			BossStatue.Completion statueState = this.StatueState;
			statueState.usingAltVersion = value;
			this.StatueState = statueState;
		}
	}

	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06001F0C RID: 7948 RVA: 0x0008DD74 File Offset: 0x0008BF74
	// (set) Token: 0x06001F0D RID: 7949 RVA: 0x0008DDD9 File Offset: 0x0008BFD9
	public BossStatue.Completion StatueState
	{
		get
		{
			if (string.IsNullOrEmpty(this.statueStatePD))
			{
				return BossStatue.Completion.None;
			}
			BossStatue.Completion playerDataVariable = GameManager.instance.GetPlayerDataVariable<BossStatue.Completion>(this.statueStatePD);
			if (!playerDataVariable.isUnlocked && this.bossScene && (this.bossScene.IsUnlocked(BossSceneCheckSource.Statue) || this.isAlwaysUnlocked))
			{
				playerDataVariable.isUnlocked = true;
			}
			return playerDataVariable;
		}
		set
		{
			if (!string.IsNullOrEmpty(this.statueStatePD))
			{
				GameManager.instance.SetPlayerDataVariable<BossStatue.Completion>(this.statueStatePD, value);
			}
		}
	}

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06001F0E RID: 7950 RVA: 0x0008DDFC File Offset: 0x0008BFFC
	// (set) Token: 0x06001F0F RID: 7951 RVA: 0x0008DE61 File Offset: 0x0008C061
	public BossStatue.Completion DreamStatueState
	{
		get
		{
			if (string.IsNullOrEmpty(this.dreamStatueStatePD))
			{
				return BossStatue.Completion.None;
			}
			BossStatue.Completion playerDataVariable = GameManager.instance.GetPlayerDataVariable<BossStatue.Completion>(this.dreamStatueStatePD);
			if (!playerDataVariable.isUnlocked && this.dreamBossScene && (this.dreamBossScene.IsUnlocked(BossSceneCheckSource.Statue) || this.isAlwaysUnlockedDream))
			{
				playerDataVariable.isUnlocked = true;
			}
			return playerDataVariable;
		}
		set
		{
			if (!string.IsNullOrEmpty(this.dreamStatueStatePD))
			{
				GameManager.instance.SetPlayerDataVariable<BossStatue.Completion>(this.dreamStatueStatePD, value);
			}
		}
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06001F10 RID: 7952 RVA: 0x0008DE81 File Offset: 0x0008C081
	public bool HasRegularVersion
	{
		get
		{
			return this.bossScene && !string.IsNullOrEmpty(this.statueStatePD);
		}
	}

	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06001F11 RID: 7953 RVA: 0x0008DEA0 File Offset: 0x0008C0A0
	public bool HasDreamVersion
	{
		get
		{
			return this.dreamBossScene && !string.IsNullOrEmpty(this.dreamStatueStatePD);
		}
	}

	// Token: 0x06001F12 RID: 7954 RVA: 0x0008DEC0 File Offset: 0x0008C0C0
	private void Awake()
	{
		this.dreamToggle = base.GetComponentInChildren<IBossStatueToggle>(false);
		if (this.dreamReturnGate)
		{
			GameObject gameObject = this.dreamReturnGate;
			gameObject.name = gameObject.name + "_" + base.gameObject.name;
		}
		if (this.cameraLock)
		{
			this.cameraLock.cameraYMin = (this.cameraLock.cameraYMax = base.transform.position.y + this.inspectCameraHeight);
		}
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x0008DF4C File Offset: 0x0008C14C
	private void Start()
	{
		this.UpdateDetails();
		if (this.StatueState.isUnlocked)
		{
			foreach (GameObject gameObject in this.disableIfLocked)
			{
				if (gameObject)
				{
					gameObject.SetActive(true);
				}
			}
			GameObject[] array = this.enableIfLocked;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			if (this.statueDisplay)
			{
				this.statueDisplay.SetActive(true);
			}
		}
		else
		{
			GameObject[] array = this.disableIfLocked;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			array = this.enableIfLocked;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			if (this.statueDisplay)
			{
				this.statueDisplay.SetActive(false);
			}
		}
		if (this.statueDisplayAlt)
		{
			this.statueDisplayAlt.SetActive(false);
		}
		if (this.dreamToggle != null)
		{
			this.dreamToggle.SetOwner(this);
		}
		if (this.DreamStatueState.isUnlocked)
		{
			if (this.dreamToggle != null)
			{
				this.dreamToggle.SetState(true);
			}
		}
		else if (this.dreamToggle != null)
		{
			this.dreamToggle.SetState(false);
		}
		Animator component = this.statueDisplay.GetComponent<Animator>();
		if (component)
		{
			component.enabled = false;
		}
		component = this.statueDisplayAlt.GetComponent<Animator>();
		if (component)
		{
			component.enabled = false;
		}
		if (this.lightTrigger)
		{
			this.lightTrigger.OnTriggerEntered += delegate(Collider2D collision, GameObject sender)
			{
				bool flag = false;
				BossStatue.Completion completion = this.StatueState;
				if (completion.isUnlocked && !completion.hasBeenSeen && !this.isAlwaysUnlocked)
				{
					completion.hasBeenSeen = true;
					this.StatueState = completion;
					flag = true;
				}
				completion = this.DreamStatueState;
				if (completion.isUnlocked && !completion.hasBeenSeen && !this.isAlwaysUnlockedDream)
				{
					completion.hasBeenSeen = true;
					this.DreamStatueState = completion;
					flag = true;
				}
				if (flag && this.OnSeenNewStatue != null)
				{
					this.OnSeenNewStatue();
				}
			};
		}
		this.SetPlaquesVisible((this.StatueState.isUnlocked && this.StatueState.hasBeenSeen) || this.isAlwaysUnlocked);
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x0008E10C File Offset: 0x0008C30C
	public void SetPlaquesVisible(bool isEnabled)
	{
		this.regularPlaque.gameObject.SetActive(false);
		this.altPlaqueL.gameObject.SetActive(false);
		this.altPlaqueR.gameObject.SetActive(false);
		if (isEnabled)
		{
			if (this.bossScene && !this.dreamBossScene)
			{
				this.regularPlaque.gameObject.SetActive(true);
				this.SetPlaqueState(this.StatueState, this.regularPlaque, this.statueStatePD);
			}
			else if (this.bossScene && this.dreamBossScene)
			{
				this.altPlaqueL.gameObject.SetActive(true);
				this.altPlaqueR.gameObject.SetActive(true);
				this.SetPlaqueState(this.StatueState, this.altPlaqueL, this.statueStatePD);
				this.SetPlaqueState(this.DreamStatueState, this.altPlaqueR, this.dreamStatueStatePD);
			}
		}
		this.lockedPlaque.enabled = !isEnabled;
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x0008E214 File Offset: 0x0008C414
	public void SetPlaqueState(BossStatue.Completion statueState, BossStatueTrophyPlaque plaque, string playerDataKey)
	{
		PlayerData playerData = GameManager.instance.playerData;
		if (string.IsNullOrEmpty(playerData.currentBossStatueCompletionKey) || playerData.currentBossStatueCompletionKey != playerDataKey)
		{
			plaque.SetDisplay(BossStatueTrophyPlaque.GetDisplayType(statueState));
			return;
		}
		BossStatueTrophyPlaque.DisplayType displayType = BossStatueTrophyPlaque.GetDisplayType(statueState);
		plaque.SetDisplay(displayType);
		plaque.DoTierCompleteEffect((BossStatueTrophyPlaque.DisplayType)playerData.bossStatueTargetLevel);
		playerData.currentBossStatueCompletionKey = "";
		playerData.bossStatueTargetLevel = -1;
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x0008E280 File Offset: 0x0008C480
	public void SetDreamVersion(bool value, bool useAltStatue = false, bool doAnim = true)
	{
		this.UsingDreamVersion = value;
		if (useAltStatue && this.statueDisplayAlt && this.statueDisplay)
		{
			base.StartCoroutine(this.SwapStatues(doAnim));
		}
		else if (this.OnStatueSwapFinished != null)
		{
			this.OnStatueSwapFinished();
		}
		this.wasUsingDreamVersion = value;
		this.UpdateDetails();
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x0008E2E4 File Offset: 0x0008C4E4
	private void UpdateDetails()
	{
		if (this.bossUIControlFSM)
		{
			BossStatue.BossUIDetails bossUIDetails = this.UsingDreamVersion ? this.dreamBossDetails : this.bossDetails;
			this.bossUIControlFSM.FsmVariables.FindFsmString("Boss Name Key").Value = bossUIDetails.nameKey;
			this.bossUIControlFSM.FsmVariables.FindFsmString("Boss Name Sheet").Value = bossUIDetails.nameSheet;
			this.bossUIControlFSM.FsmVariables.FindFsmString("Description Key").Value = bossUIDetails.descriptionKey;
			this.bossUIControlFSM.FsmVariables.FindFsmString("Description Sheet").Value = bossUIDetails.descriptionSheet;
		}
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x0008E398 File Offset: 0x0008C598
	private IEnumerator SwapStatues(bool doAnim)
	{
		GameObject current = this.wasUsingDreamVersion ? this.statueDisplayAlt : this.statueDisplay;
		GameObject next = this.wasUsingDreamVersion ? this.statueDisplay : this.statueDisplayAlt;
		if (doAnim)
		{
			if (this.bossUIControlFSM)
			{
				FSMUtility.SendEventToGameObject(this.bossUIControlFSM.gameObject, "NPC CONTROL OFF", false);
			}
			yield return new WaitForSeconds(this.swapWaitTime);
			if (this.statueShakeParticles)
			{
				this.statueShakeParticles.Play();
			}
			if (this.statueShakeLoop)
			{
				this.statueShakeLoop.Play();
			}
			yield return base.StartCoroutine(this.Jitter(this.shakeTime, 0.1f, current));
			if (this.statueShakeLoop)
			{
				this.statueShakeLoop.Stop();
			}
			base.StartCoroutine(this.PlayAudioEventDelayed(this.statueDownSound, this.statueDownSoundDelay));
			yield return base.StartCoroutine(this.PlayAnimWait(current.GetComponent<Animator>(), "Down", 0f));
		}
		current.SetActive(false);
		if (doAnim)
		{
			yield return new WaitForSeconds(this.holdTime);
			base.StartCoroutine(this.PlayParticlesDelay(this.statueUpParticles, this.upParticleDelay));
			base.StartCoroutine(this.PlayAudioEventDelayed(this.statueUpSound, this.statueUpSoundDelay));
		}
		next.transform.position = current.transform.position;
		next.SetActive(true);
		if (doAnim)
		{
			yield return base.StartCoroutine(this.PlayAnimWait(next.GetComponent<Animator>(), "Up", 0f));
			if (this.bossUIControlFSM)
			{
				FSMUtility.SendEventToGameObject(this.bossUIControlFSM.gameObject, "CONVO CANCEL", false);
			}
		}
		if (this.OnStatueSwapFinished != null)
		{
			this.OnStatueSwapFinished();
		}
		yield break;
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x0008E3AE File Offset: 0x0008C5AE
	private IEnumerator Jitter(float duration, float magnitude, GameObject obj)
	{
		Transform sprite = obj.transform;
		Vector3 initialPos = sprite.position;
		float elapsed = 0f;
		float half = magnitude / 2f;
		while (elapsed < duration)
		{
			sprite.position = initialPos + new Vector3(Random.Range(-half, half), Random.Range(-half, half), 0f);
			yield return null;
			elapsed += Time.deltaTime;
		}
		sprite.position = initialPos;
		yield break;
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x0008E3CB File Offset: 0x0008C5CB
	private IEnumerator PlayAnimWait(Animator animator, string stateName, float normalizedTime)
	{
		if (animator)
		{
			animator.enabled = true;
			animator.Play(stateName, 0, normalizedTime);
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
			animator.enabled = false;
		}
		yield break;
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x0008E3E8 File Offset: 0x0008C5E8
	private IEnumerator PlayParticlesDelay(ParticleSystem system, float delay)
	{
		if (system)
		{
			yield return new WaitForSeconds(delay);
			system.Play();
		}
		yield break;
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x0008E3FE File Offset: 0x0008C5FE
	private IEnumerator PlayAudioEventDelayed(AudioEvent audioEvent, float delay)
	{
		yield return new WaitForSeconds(delay);
		audioEvent.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		yield break;
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x0008E41B File Offset: 0x0008C61B
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position + new Vector3(0f, this.inspectCameraHeight, 0f), 0.25f);
	}

	// Token: 0x04001DE8 RID: 7656
	[Header("Boss Data")]
	public BossScene bossScene;

	// Token: 0x04001DE9 RID: 7657
	public BossScene dreamBossScene;

	// Token: 0x04001DEA RID: 7658
	[Header("Statue Data")]
	[FormerlySerializedAs("statueStateInt")]
	public string statueStatePD;

	// Token: 0x04001DEB RID: 7659
	public BossStatue.BossUIDetails bossDetails;

	// Token: 0x04001DEC RID: 7660
	[Space]
	[FormerlySerializedAs("dreamStatueStateInt")]
	public string dreamStatueStatePD;

	// Token: 0x04001DED RID: 7661
	public BossStatue.BossUIDetails dreamBossDetails;

	// Token: 0x04001DEE RID: 7662
	[Space]
	public bool hasNoTiers;

	// Token: 0x04001DEF RID: 7663
	public bool dontCountCompletion;

	// Token: 0x04001DF0 RID: 7664
	public bool isAlwaysUnlocked;

	// Token: 0x04001DF1 RID: 7665
	public bool isAlwaysUnlockedDream;

	// Token: 0x04001DF2 RID: 7666
	public float inspectCameraHeight = 5.5f;

	// Token: 0x04001DF3 RID: 7667
	public bool isHidden;

	// Token: 0x04001DF4 RID: 7668
	[Header("Prefab Stuff")]
	public PlayMakerFSM bossUIControlFSM;

	// Token: 0x04001DF5 RID: 7669
	[Space]
	public GameObject[] disableIfLocked;

	// Token: 0x04001DF6 RID: 7670
	public GameObject[] enableIfLocked;

	// Token: 0x04001DF7 RID: 7671
	public BossStatueTrophyPlaque regularPlaque;

	// Token: 0x04001DF8 RID: 7672
	public BossStatueTrophyPlaque altPlaqueL;

	// Token: 0x04001DF9 RID: 7673
	public BossStatueTrophyPlaque altPlaqueR;

	// Token: 0x04001DFA RID: 7674
	public SpriteRenderer lockedPlaque;

	// Token: 0x04001DFB RID: 7675
	[Space]
	public GameObject dreamReturnGate;

	// Token: 0x04001DFC RID: 7676
	public TriggerEnterEvent lightTrigger;

	// Token: 0x04001DFD RID: 7677
	public CameraLockArea cameraLock;

	// Token: 0x04001DFE RID: 7678
	[Header("Animation")]
	public GameObject statueDisplay;

	// Token: 0x04001DFF RID: 7679
	public GameObject statueDisplayAlt;

	// Token: 0x04001E00 RID: 7680
	public ParticleSystem statueShakeParticles;

	// Token: 0x04001E01 RID: 7681
	public ParticleSystem statueUpParticles;

	// Token: 0x04001E02 RID: 7682
	public AudioSource statueShakeLoop;

	// Token: 0x04001E03 RID: 7683
	public AudioSource audioSourcePrefab;

	// Token: 0x04001E04 RID: 7684
	public AudioEvent statueDownSound;

	// Token: 0x04001E05 RID: 7685
	public float statueDownSoundDelay;

	// Token: 0x04001E06 RID: 7686
	public AudioEvent statueUpSound;

	// Token: 0x04001E07 RID: 7687
	public float statueUpSoundDelay = 0.3f;

	// Token: 0x04001E08 RID: 7688
	public float swapWaitTime = 0.25f;

	// Token: 0x04001E09 RID: 7689
	public float shakeTime = 1f;

	// Token: 0x04001E0A RID: 7690
	public float holdTime = 0.5f;

	// Token: 0x04001E0B RID: 7691
	public float upParticleDelay = 0.25f;

	// Token: 0x04001E0C RID: 7692
	private IBossStatueToggle dreamToggle;

	// Token: 0x04001E0D RID: 7693
	private bool wasUsingDreamVersion;

	// Token: 0x02001637 RID: 5687
	// (Invoke) Token: 0x0600894A RID: 35146
	public delegate void StatueSwapEndEvent();

	// Token: 0x02001638 RID: 5688
	// (Invoke) Token: 0x0600894E RID: 35150
	public delegate void SeenNewStatueEvent();

	// Token: 0x02001639 RID: 5689
	[Serializable]
	public struct Completion
	{
		// Token: 0x17000E2D RID: 3629
		// (get) Token: 0x06008951 RID: 35153 RVA: 0x0027C99C File Offset: 0x0027AB9C
		public static BossStatue.Completion None
		{
			get
			{
				return new BossStatue.Completion
				{
					hasBeenSeen = false,
					isUnlocked = false,
					completedTier1 = false,
					completedTier2 = false,
					completedTier3 = false,
					seenTier3Unlock = false,
					usingAltVersion = false
				};
			}
		}

		// Token: 0x04008A0D RID: 35341
		public bool hasBeenSeen;

		// Token: 0x04008A0E RID: 35342
		public bool isUnlocked;

		// Token: 0x04008A0F RID: 35343
		public bool completedTier1;

		// Token: 0x04008A10 RID: 35344
		public bool completedTier2;

		// Token: 0x04008A11 RID: 35345
		public bool completedTier3;

		// Token: 0x04008A12 RID: 35346
		public bool seenTier3Unlock;

		// Token: 0x04008A13 RID: 35347
		public bool usingAltVersion;
	}

	// Token: 0x0200163A RID: 5690
	[Serializable]
	public struct BossUIDetails
	{
		// Token: 0x04008A14 RID: 35348
		public string nameKey;

		// Token: 0x04008A15 RID: 35349
		public string nameSheet;

		// Token: 0x04008A16 RID: 35350
		public string descriptionKey;

		// Token: 0x04008A17 RID: 35351
		public string descriptionSheet;
	}
}
