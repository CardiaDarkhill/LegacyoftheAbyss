using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004D0 RID: 1232
public class DreamPlant : MonoBehaviour
{
	// Token: 0x06002C4D RID: 11341 RVA: 0x000C1EAC File Offset: 0x000C00AC
	private void Awake()
	{
		this.spriteFlash = base.GetComponent<SpriteFlash>();
		PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
		if (component)
		{
			component.OnGetSaveState += delegate(out bool value)
			{
				value = this.completed;
			};
			component.OnSetSaveState += delegate(bool value)
			{
				this.completed = value;
				if (this.completed)
				{
					this.activated = true;
					if (this.anim)
					{
						this.anim.Play("Completed");
					}
					if (this.dreamDialogue)
					{
						this.dreamDialogue.SetActive(true);
					}
				}
			};
		}
		this.audioSource = base.GetComponent<AudioSource>();
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06002C4E RID: 11342 RVA: 0x000C1F10 File Offset: 0x000C0110
	private void Start()
	{
		this.hasDreamNail = GameManager.instance.GetPlayerDataBool("hasDreamNail");
		this.seenDreamNailPrompt = GameManager.instance.GetPlayerDataBool("seenDreamNailPrompt");
		if (this.heroDetector && this.hasDreamNail)
		{
			this.heroDetector.OnEnter += delegate(Collider2D <p0>)
			{
				this.ShowPrompt(true);
			};
			this.heroDetector.OnExit += delegate(Collider2D <p0>)
			{
				this.ShowPrompt(false);
			};
		}
		if (this.completed && this.playerdataBool != "")
		{
			GameManager.instance.SetPlayerDataBool(this.playerdataBool, true);
		}
		if (this.hasDreamNail && !this.activated && this.dreamAreaEffect)
		{
			this.spawnedDreamAreaEffect = Object.Instantiate<GameObject>(this.dreamAreaEffect);
			this.spawnedDreamAreaEffect.SetActive(false);
		}
		if (this.whiteFlash)
		{
			this.whiteFlash.SetActive(true);
			this.whiteFlash.SetActive(false);
		}
		this.dreamOrbs = Object.FindObjectsOfType<DreamPlantOrb>();
		DreamPlantOrb.plant = this;
	}

	// Token: 0x06002C4F RID: 11343 RVA: 0x000C2028 File Offset: 0x000C0228
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.activated)
		{
			return;
		}
		if (collision.tag == "Dream Attack")
		{
			this.activated = true;
			DreamPlantOrb[] array = this.dreamOrbs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Show();
			}
			if (this.spriteFlash)
			{
				this.spriteFlash.flashFocusHeal();
			}
			if (this.glowFader)
			{
				this.glowFader.Fade(false);
			}
			if (this.anim)
			{
				this.anim.Play("Activate");
			}
			if (this.audioSource && this.hitSound)
			{
				this.audioSource.PlayOneShot(this.hitSound);
			}
			if (this.spawnedDreamAreaEffect)
			{
				this.spawnedDreamAreaEffect.SetActive(true);
			}
			if (this.whiteFlash)
			{
				this.whiteFlash.SetActive(true);
			}
			if (this.activateParticles)
			{
				this.activateParticles.gameObject.SetActive(true);
			}
			if (this.activatedParticles)
			{
				this.activatedParticles.gameObject.SetActive(true);
			}
			if (this.dreamImpact)
			{
				Vector3 vector = collision.bounds.center;
				Collider2D component = base.GetComponent<Collider2D>();
				if (component)
				{
					vector += component.bounds.center;
					vector /= 2f;
				}
				this.dreamImpact.Spawn(vector);
			}
			GameCameras instance = GameCameras.instance;
			if (instance)
			{
				instance.cameraShakeFSM.SendEvent("AverageShake");
			}
			EventRegister.SendEvent(EventRegisterEvents.DreamPlantHit, null);
		}
	}

	// Token: 0x06002C50 RID: 11344 RVA: 0x000C21E5 File Offset: 0x000C03E5
	public void AddOrbCount()
	{
		this.spawnedOrbs++;
		if (this.checkOrbRoutine == null)
		{
			this.checkOrbRoutine = base.StartCoroutine(this.CheckOrbs());
		}
	}

	// Token: 0x06002C51 RID: 11345 RVA: 0x000C220F File Offset: 0x000C040F
	public void RemoveOrbCount()
	{
		this.spawnedOrbs--;
	}

	// Token: 0x06002C52 RID: 11346 RVA: 0x000C2220 File Offset: 0x000C0420
	private void ShowPrompt(bool show)
	{
		if (this.activated)
		{
			return;
		}
		if (show)
		{
			if (!this.seenDreamNailPrompt)
			{
				this.seenDreamNailPrompt = true;
				GameManager.instance.SetPlayerDataBool("seenDreamNailPrompt", true);
				PlayMakerFSM.BroadcastEvent("REMINDER DREAM NAIL");
			}
			if (this.audioSource && this.glowSound)
			{
				this.audioSource.PlayOneShot(this.glowSound);
			}
			if (this.wiltedParticles)
			{
				this.wiltedParticles.Play();
			}
			base.SendMessage("flashWhitePulse");
			if (this.glowFader)
			{
				this.glowFader.Fade(true);
				return;
			}
		}
		else if (this.glowFader)
		{
			this.glowFader.Fade(false);
		}
	}

	// Token: 0x06002C53 RID: 11347 RVA: 0x000C22E6 File Offset: 0x000C04E6
	private IEnumerator CheckOrbs()
	{
		while (this.spawnedOrbs > 0)
		{
			yield return null;
		}
		this.completed = true;
		if (this.playerdataBool != "")
		{
			GameManager.instance.SetPlayerDataBool(this.playerdataBool, true);
		}
		GameManager.instance.SendMessage("AddToDreamPlantCList");
		yield return new WaitForSeconds(1f);
		PlayMakerFSM.BroadcastEvent("DREAM AREA DISABLE");
		if (this.activatedParticles)
		{
			this.activatedParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		if (this.completeGlowFader)
		{
			this.completeGlowFader.Fade(true);
		}
		if (this.audioSource && this.growChargeSound)
		{
			this.audioSource.PlayOneShot(this.growChargeSound);
		}
		if (this.completeChargeParticles)
		{
			this.completeChargeParticles.gameObject.SetActive(true);
		}
		yield return new WaitForSeconds(1f);
		if (this.completeChargeParticles)
		{
			this.completeChargeParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		if (this.audioSource && this.growSound)
		{
			this.audioSource.PlayOneShot(this.growSound);
		}
		if (this.anim)
		{
			this.anim.Play("Complete");
		}
		if (this.whiteFlash)
		{
			this.whiteFlash.SetActive(true);
		}
		if (this.completeGlowFader)
		{
			this.completeGlowFader.Fade(false);
		}
		if (this.growParticles)
		{
			this.growParticles.gameObject.SetActive(true);
		}
		GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
		if (gameCameras)
		{
			gameCameras.cameraShakeFSM.SendEvent("AverageShake");
		}
		if (this.dreamDialogue)
		{
			this.dreamDialogue.SetActive(true);
		}
		yield break;
	}

	// Token: 0x04002DC9 RID: 11721
	public HeroDetect heroDetector;

	// Token: 0x04002DCA RID: 11722
	public AudioClip glowSound;

	// Token: 0x04002DCB RID: 11723
	private AudioSource audioSource;

	// Token: 0x04002DCC RID: 11724
	public ParticleSystem wiltedParticles;

	// Token: 0x04002DCD RID: 11725
	[Space]
	public ColorFader glowFader;

	// Token: 0x04002DCE RID: 11726
	public ColorFader completeGlowFader;

	// Token: 0x04002DCF RID: 11727
	[Space]
	public AudioClip hitSound;

	// Token: 0x04002DD0 RID: 11728
	public GameObject dreamImpact;

	// Token: 0x04002DD1 RID: 11729
	public GameObject dreamAreaEffect;

	// Token: 0x04002DD2 RID: 11730
	private GameObject spawnedDreamAreaEffect;

	// Token: 0x04002DD3 RID: 11731
	public ParticleSystem activateParticles;

	// Token: 0x04002DD4 RID: 11732
	public ParticleSystem activatedParticles;

	// Token: 0x04002DD5 RID: 11733
	public GameObject whiteFlash;

	// Token: 0x04002DD6 RID: 11734
	[Space]
	public AudioClip growChargeSound;

	// Token: 0x04002DD7 RID: 11735
	public AudioClip growSound;

	// Token: 0x04002DD8 RID: 11736
	public ParticleSystem completeChargeParticles;

	// Token: 0x04002DD9 RID: 11737
	public ParticleSystem growParticles;

	// Token: 0x04002DDA RID: 11738
	public GameObject dreamDialogue;

	// Token: 0x04002DDB RID: 11739
	[Space]
	public string playerdataBool;

	// Token: 0x04002DDC RID: 11740
	private tk2dSpriteAnimator anim;

	// Token: 0x04002DDD RID: 11741
	private bool activated;

	// Token: 0x04002DDE RID: 11742
	private bool completed;

	// Token: 0x04002DDF RID: 11743
	private bool hasDreamNail;

	// Token: 0x04002DE0 RID: 11744
	private bool seenDreamNailPrompt;

	// Token: 0x04002DE1 RID: 11745
	private int spawnedOrbs;

	// Token: 0x04002DE2 RID: 11746
	private Coroutine checkOrbRoutine;

	// Token: 0x04002DE3 RID: 11747
	private DreamPlantOrb[] dreamOrbs;

	// Token: 0x04002DE4 RID: 11748
	private SpriteFlash spriteFlash;
}
