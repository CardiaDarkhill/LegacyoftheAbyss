using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000394 RID: 916
public class BossSequenceDoor : MonoBehaviour
{
	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06001EF1 RID: 7921 RVA: 0x0008D500 File Offset: 0x0008B700
	// (set) Token: 0x06001EF2 RID: 7922 RVA: 0x0008D508 File Offset: 0x0008B708
	public BossSequenceDoor.Completion CurrentCompletion
	{
		get
		{
			return this.completion;
		}
		set
		{
			this.completion = value;
			this.SaveState();
		}
	}

	// Token: 0x06001EF3 RID: 7923 RVA: 0x0008D518 File Offset: 0x0008B718
	private void Start()
	{
		this.completion = (string.IsNullOrEmpty(this.playerDataString) ? BossSequenceDoor.Completion.None : GameManager.instance.GetPlayerDataVariable<BossSequenceDoor.Completion>(this.playerDataString));
		if (this.IsUnlocked() || this.completion.canUnlock)
		{
			this.SetDisplayState(this.completion);
			if (this.completion.unlocked || !this.doLockBreakSequence)
			{
				if (this.lockSet)
				{
					this.lockSet.SetActive(false);
				}
				if (this.unlockedSet)
				{
					this.unlockedSet.SetActive(true);
				}
			}
			else
			{
				this.doUnlockSequence = true;
				if (this.lockInteractPrompt)
				{
					this.lockInteractPrompt.SetActive(false);
				}
				if (this.unlockedSet)
				{
					this.unlockedSet.SetActive(false);
				}
			}
		}
		else
		{
			this.SetDisplayState(BossSequenceDoor.Completion.None);
			if (this.lockSet)
			{
				this.lockSet.SetActive(true);
			}
			if (this.unlockedSet)
			{
				this.unlockedSet.SetActive(false);
			}
			if (this.lockedUIPrefab && !BossSequenceDoor.lockedUI)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.lockedUIPrefab);
				BossSequenceDoor.lockedUI = gameObject.GetComponent<BossDoorLockUI>();
				gameObject.SetActive(false);
			}
		}
		if (this.challengeFSM && this.bossSequence && this.bossSequence.Count > 0)
		{
			this.challengeFSM.FsmVariables.FindFsmString("To Scene").Value = this.bossSequence.GetSceneAt(0);
		}
		if (this.dreamReturnGate)
		{
			GameObject gameObject2 = this.dreamReturnGate;
			gameObject2.name = gameObject2.name + "_" + base.gameObject.name;
		}
	}

	// Token: 0x06001EF4 RID: 7924 RVA: 0x0008D6F3 File Offset: 0x0008B8F3
	private void OnDestroy()
	{
		if (this.material != null)
		{
			Object.Destroy(this.material);
			this.material = null;
		}
	}

	// Token: 0x06001EF5 RID: 7925 RVA: 0x0008D715 File Offset: 0x0008B915
	private void SaveState()
	{
		GameManager.instance.SetPlayerDataVariable<BossSequenceDoor.Completion>(this.playerDataString, this.completion);
	}

	// Token: 0x06001EF6 RID: 7926 RVA: 0x0008D72D File Offset: 0x0008B92D
	private bool IsUnlocked()
	{
		return this.completion.unlocked || (this.bossSequence && this.bossSequence.IsUnlocked());
	}

	// Token: 0x06001EF7 RID: 7927 RVA: 0x0008D75C File Offset: 0x0008B95C
	private void SetDisplayState(BossSequenceDoor.Completion completion)
	{
		if (this.completedDisplay)
		{
			this.completedDisplay.SetActive(completion.completed);
		}
		if (this.completedAllDisplay)
		{
			this.completedAllDisplay.SetActive(completion.allBindings);
		}
		if (this.completedNoHitsDisplay)
		{
			this.completedNoHitsDisplay.SetActive(completion.noHits);
		}
		if (this.boundAllDisplay)
		{
			this.boundAllDisplay.SetActive(completion.allBindings);
		}
		if (this.boundAllBackboard)
		{
			this.boundAllBackboard.SetActive(completion.allBindings);
		}
		if (this.boundNailDisplay)
		{
			this.boundNailDisplay.SetActive(completion.boundNail && !completion.allBindings);
		}
		if (this.boundHeartDisplay)
		{
			this.boundHeartDisplay.SetActive(completion.boundShell && !completion.allBindings);
		}
		if (this.boundCharmsDisplay)
		{
			this.boundCharmsDisplay.SetActive(completion.boundCharms && !completion.allBindings);
		}
		if (this.boundSoulDisplay)
		{
			this.boundSoulDisplay.SetActive(completion.boundSoul && !completion.allBindings);
		}
	}

	// Token: 0x06001EF8 RID: 7928 RVA: 0x0008D8AF File Offset: 0x0008BAAF
	public void ShowLockUI(bool value)
	{
		if (BossSequenceDoor.lockedUI)
		{
			if (value)
			{
				BossSequenceDoor.lockedUI.Show(this);
				return;
			}
			BossSequenceDoor.lockedUI.Hide();
		}
	}

	// Token: 0x06001EF9 RID: 7929 RVA: 0x0008D8D8 File Offset: 0x0008BAD8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.doUnlockSequence && collision.gameObject.tag == "Player" && HeroController.instance.isHeroInPosition)
		{
			BossSequenceDoor[] array = this.requiredComplete;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].CurrentCompletion.completed)
				{
					return;
				}
			}
			this.doUnlockSequence = false;
			this.completion.unlocked = true;
			this.SaveState();
			base.StartCoroutine(this.DoorUnlockSequence());
		}
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x0008D95C File Offset: 0x0008BB5C
	private void StartShake()
	{
		FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingMed", true);
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HeroController.instance.gameObject, "Roar and Wound States");
		if (playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmGameObject("Roar Object").Value = base.gameObject;
		}
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR ENTER", false);
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x0008D9CB File Offset: 0x0008BBCB
	private void StopShake()
	{
		FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingMed", false);
		GameCameras.instance.cameraShakeFSM.SendEvent("StopRumble");
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR EXIT", false);
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x0008DA0B File Offset: 0x0008BC0B
	private IEnumerator DoorUnlockSequence()
	{
		this.StartShake();
		if (this.cameraLock)
		{
			this.cameraLock.SetActive(true);
		}
		if (this.lockBreakAnticEffects)
		{
			this.lockBreakAnticEffects.SetActive(true);
		}
		if (this.lockBreakRumbleSound)
		{
			this.lockBreakRumbleSound.SetActive(true);
		}
		if (this.glowParticles)
		{
			this.glowParticles.main.duration = this.lockBreakAnticTime;
			this.glowParticles.Play();
		}
		if (this.glowSprites.Length != 0)
		{
			if (this.material == null)
			{
				this.material = new Material(this.spriteFlashMaterial);
			}
			SpriteRenderer[] array = this.glowSprites;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sharedMaterial = this.material;
			}
			Color[] startColors = new Color[this.fadeSprites.Length];
			for (int j = 0; j < startColors.Length; j++)
			{
				startColors[j] = this.fadeSprites[j].color;
				this.fadeSprites[j].color = Color.clear;
				this.fadeSprites[j].gameObject.SetActive(true);
			}
			for (float elapsed = 0f; elapsed < this.lockBreakAnticTime; elapsed += Time.deltaTime)
			{
				float num = this.glowCurve.Evaluate(elapsed / this.lockBreakAnticTime);
				this.material.SetFloat("_FlashAmount", num);
				for (int k = 0; k < startColors.Length; k++)
				{
					Color color = startColors[k];
					color.a *= num;
					this.fadeSprites[k].color = color;
				}
				yield return null;
			}
			startColors = null;
		}
		else
		{
			yield return new WaitForSeconds(this.lockBreakAnticTime);
		}
		this.StopShake();
		if (this.cameraLock)
		{
			this.cameraLock.SetActive(false);
		}
		if (this.lockBreakRumbleSound)
		{
			this.lockBreakRumbleSound.SetActive(false);
		}
		if (this.lockSet)
		{
			this.lockSet.SetActive(false);
		}
		if (this.lockBreakEffects)
		{
			this.lockBreakEffects.SetActive(true);
		}
		if (this.unlockedSet)
		{
			this.unlockedSet.SetActive(true);
		}
		GameCameras.instance.cameraShakeFSM.SendEvent("BigShake");
		yield break;
	}

	// Token: 0x04001DB4 RID: 7604
	[Header("Door-specific")]
	public string playerDataString;

	// Token: 0x04001DB5 RID: 7605
	private BossSequenceDoor.Completion completion;

	// Token: 0x04001DB6 RID: 7606
	public BossSequence bossSequence;

	// Token: 0x04001DB7 RID: 7607
	[Space]
	public string titleSuperKey;

	// Token: 0x04001DB8 RID: 7608
	public string titleSuperSheet;

	// Token: 0x04001DB9 RID: 7609
	public string titleMainKey;

	// Token: 0x04001DBA RID: 7610
	public string titleMainSheet;

	// Token: 0x04001DBB RID: 7611
	public string descriptionKey;

	// Token: 0x04001DBC RID: 7612
	public string descriptionSheet;

	// Token: 0x04001DBD RID: 7613
	[Space]
	public BossSequenceDoor[] requiredComplete;

	// Token: 0x04001DBE RID: 7614
	[Header("Prefab")]
	public GameObject completedDisplay;

	// Token: 0x04001DBF RID: 7615
	public GameObject completedAllDisplay;

	// Token: 0x04001DC0 RID: 7616
	public GameObject completedNoHitsDisplay;

	// Token: 0x04001DC1 RID: 7617
	[Space]
	public GameObject boundNailDisplay;

	// Token: 0x04001DC2 RID: 7618
	public GameObject boundHeartDisplay;

	// Token: 0x04001DC3 RID: 7619
	public GameObject boundCharmsDisplay;

	// Token: 0x04001DC4 RID: 7620
	public GameObject boundSoulDisplay;

	// Token: 0x04001DC5 RID: 7621
	public GameObject boundAllDisplay;

	// Token: 0x04001DC6 RID: 7622
	public GameObject boundAllBackboard;

	// Token: 0x04001DC7 RID: 7623
	[Space]
	public GameObject lockSet;

	// Token: 0x04001DC8 RID: 7624
	public GameObject lockInteractPrompt;

	// Token: 0x04001DC9 RID: 7625
	public GameObject cameraLock;

	// Token: 0x04001DCA RID: 7626
	public GameObject unlockedSet;

	// Token: 0x04001DCB RID: 7627
	public PlayMakerFSM challengeFSM;

	// Token: 0x04001DCC RID: 7628
	public GameObject dreamReturnGate;

	// Token: 0x04001DCD RID: 7629
	[Header("Lock Break Effects")]
	public bool doLockBreakSequence = true;

	// Token: 0x04001DCE RID: 7630
	public GameObject lockBreakAnticEffects;

	// Token: 0x04001DCF RID: 7631
	public GameObject lockBreakRumbleSound;

	// Token: 0x04001DD0 RID: 7632
	public SpriteRenderer[] glowSprites;

	// Token: 0x04001DD1 RID: 7633
	public Material spriteFlashMaterial;

	// Token: 0x04001DD2 RID: 7634
	public SpriteRenderer[] fadeSprites;

	// Token: 0x04001DD3 RID: 7635
	public AnimationCurve glowCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04001DD4 RID: 7636
	public ParticleSystem glowParticles;

	// Token: 0x04001DD5 RID: 7637
	public float lockBreakAnticTime = 3.3f;

	// Token: 0x04001DD6 RID: 7638
	public GameObject lockBreakEffects;

	// Token: 0x04001DD7 RID: 7639
	private bool doUnlockSequence;

	// Token: 0x04001DD8 RID: 7640
	[Space]
	public GameObject lockedUIPrefab;

	// Token: 0x04001DD9 RID: 7641
	private Material material;

	// Token: 0x04001DDA RID: 7642
	private static BossDoorLockUI lockedUI;

	// Token: 0x02001635 RID: 5685
	[Serializable]
	public struct Completion
	{
		// Token: 0x17000E2A RID: 3626
		// (get) Token: 0x06008942 RID: 35138 RVA: 0x0027C614 File Offset: 0x0027A814
		public static BossSequenceDoor.Completion None
		{
			get
			{
				return new BossSequenceDoor.Completion
				{
					canUnlock = false,
					unlocked = false,
					completed = false,
					allBindings = false,
					noHits = false,
					boundNail = false,
					boundShell = false,
					boundCharms = false,
					boundSoul = false,
					viewedBossSceneCompletions = new List<string>()
				};
			}
		}

		// Token: 0x040089FE RID: 35326
		public bool canUnlock;

		// Token: 0x040089FF RID: 35327
		public bool unlocked;

		// Token: 0x04008A00 RID: 35328
		public bool completed;

		// Token: 0x04008A01 RID: 35329
		public bool allBindings;

		// Token: 0x04008A02 RID: 35330
		public bool noHits;

		// Token: 0x04008A03 RID: 35331
		public bool boundNail;

		// Token: 0x04008A04 RID: 35332
		public bool boundShell;

		// Token: 0x04008A05 RID: 35333
		public bool boundCharms;

		// Token: 0x04008A06 RID: 35334
		public bool boundSoul;

		// Token: 0x04008A07 RID: 35335
		public List<string> viewedBossSceneCompletions;
	}
}
