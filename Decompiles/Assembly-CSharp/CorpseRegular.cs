using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020002B3 RID: 691
public class CorpseRegular : Corpse, AntRegion.ICheck
{
	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06001888 RID: 6280 RVA: 0x00070B5C File Offset: 0x0006ED5C
	protected override bool DoLandEffectsInstantly
	{
		get
		{
			return this.doLandInstantly;
		}
	}

	// Token: 0x1700027E RID: 638
	// (get) Token: 0x06001889 RID: 6281 RVA: 0x00070B64 File Offset: 0x0006ED64
	// (set) Token: 0x0600188A RID: 6282 RVA: 0x00070B6C File Offset: 0x0006ED6C
	public bool CanEnterAntRegion { get; private set; } = true;

	// Token: 0x0600188B RID: 6283 RVA: 0x00070B75 File Offset: 0x0006ED75
	public override bool OnAwake()
	{
		if (base.OnAwake() && this.profile)
		{
			this.profile.EnsurePersonalPool(base.gameObject);
		}
		return false;
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x00070B9E File Offset: 0x0006ED9E
	private new void OnDisable()
	{
		if (this.loopingStunEffect != null)
		{
			if (this.loopingStunEffect.activeSelf)
			{
				this.loopingStunEffect.Recycle();
			}
			this.loopingStunEffect = null;
		}
	}

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x0600188D RID: 6285 RVA: 0x00070BCD File Offset: 0x0006EDCD
	protected override bool DesaturateOnLand
	{
		get
		{
			return this.desaturateOnLand;
		}
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x00070BD8 File Offset: 0x0006EDD8
	protected override void Begin()
	{
		if (this.startVoiceAudioTable && this.audioLoopVoice)
		{
			this.audioLoopVoice.Stop();
			this.audioLoopVoice.loop = false;
			this.audioLoopVoice.clip = this.startVoiceAudioTable.SelectClip(false);
			this.audioLoopVoice.volume = this.startVoiceAudioTable.SelectVolume();
			this.audioLoopVoice.pitch = this.startVoiceAudioTable.SelectPitch();
			this.audioLoopVoice.Play();
		}
		if (this.profile)
		{
			Vector2 pos = base.transform.position;
			foreach (GameObject effectPrefab in this.profile.SpawnOnStart)
			{
				this.SpawnEffect(effectPrefab, pos);
			}
			if (this.profile.StunTime > 0f)
			{
				base.PlayStartAudio();
				base.StartCoroutine(this.DoStunEffects());
				return;
			}
		}
		base.Begin();
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x00070CD4 File Offset: 0x0006EED4
	public bool SpawnElementalEffects(ElementalEffectType elementType)
	{
		if (elementType == ElementalEffectType.None)
		{
			return false;
		}
		if (this.profile == null)
		{
			return false;
		}
		if (elementType < ElementalEffectType.None || elementType >= (ElementalEffectType)this.profile.ElementalEffects.Length)
		{
			return false;
		}
		CorpseRegularEffectsProfile.EffectList effectList = this.profile.ElementalEffects[(int)elementType];
		if (effectList == null)
		{
			return false;
		}
		Vector2 pos = base.transform.position;
		foreach (GameObject effectPrefab in effectList.effects)
		{
			this.SpawnEffect(effectPrefab, pos);
		}
		return effectList.effects.Count > 0;
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x00070D8C File Offset: 0x0006EF8C
	protected override void LandEffects()
	{
		base.LandEffects();
		base.StartCoroutine(this.DoLandEffects(this.splashed));
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x00070DA7 File Offset: 0x0006EFA7
	private IEnumerator DoStunEffects()
	{
		this.CanEnterAntRegion = false;
		if (this.body)
		{
			this.body.isKinematic = true;
			this.body.linearVelocity = Vector2.zero;
		}
		if (this.spriteFlash)
		{
			this.spriteFlash.FlashingSuperDash();
		}
		if (this.spriteAnimator && !this.spriteAnimator.TryPlay("Death Stun"))
		{
			this.spriteAnimator.Play("Death Air");
		}
		if (this.loopingStunEffect != null)
		{
			this.loopingStunEffect.Recycle();
		}
		this.loopingStunEffect = (this.profile.LoopingStunEffectPrefab ? this.profile.LoopingStunEffectPrefab.Spawn(base.transform.position) : null);
		yield return new WaitForSeconds(this.profile.StunTime);
		if (this.loopingStunEffect != null)
		{
			ParticleSystem[] componentsInChildren = this.loopingStunEffect.GetComponentsInChildren<ParticleSystem>();
			this.loopingStunEffect = null;
			ParticleSystem[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(false, ParticleSystemStopBehavior.StopEmitting);
			}
		}
		this.CanEnterAntRegion = true;
		if (this.body)
		{
			this.body.isKinematic = false;
			bool flag = false;
			if ((!this.spriteFacesRight && base.transform.localScale.x < 0f) || (this.spriteFacesRight && base.transform.localScale.x > 0f))
			{
				flag = true;
			}
			if (!flag)
			{
				this.body.linearVelocity = new Vector2(10f, 15f);
			}
			else
			{
				this.body.linearVelocity = new Vector2(-10f, 15f);
			}
		}
		if (this.spriteFlash)
		{
			this.spriteFlash.CancelFlash();
		}
		if (this.activateOnStunEnd)
		{
			this.activateOnStunEnd.SetActive(true);
		}
		if (this.stunEndVoiceAudioTable && this.audioLoopVoice)
		{
			this.audioLoopVoice.Stop();
			this.audioLoopVoice.loop = false;
			this.audioLoopVoice.clip = this.stunEndVoiceAudioTable.SelectClip(false);
			this.audioLoopVoice.volume = this.stunEndVoiceAudioTable.SelectVolume();
			this.audioLoopVoice.pitch = this.stunEndVoiceAudioTable.SelectPitch();
			this.audioLoopVoice.Play();
		}
		this.profile.StunEndShake.DoShake(this, true);
		Vector2 pos = base.transform.position;
		foreach (GameObject effectPrefab in this.profile.SpawnOnStunEnd)
		{
			this.SpawnEffect(effectPrefab, pos);
		}
		base.Begin();
		yield break;
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x00070DB6 File Offset: 0x0006EFB6
	private IEnumerator DoLandEffects(bool didSplash)
	{
		if (this.flingChildren.Parent)
		{
			FlingUtils.ChildrenConfig childrenConfig = this.flingChildren;
			childrenConfig.Parent.SetActive(true);
			childrenConfig.Parent.transform.SetParent(null, true);
			if (base.transform.localScale.x < 0f)
			{
				childrenConfig.AngleMin = global::Helper.GetReflectedAngle(childrenConfig.AngleMin, true, false, false);
				childrenConfig.AngleMax = global::Helper.GetReflectedAngle(childrenConfig.AngleMax, true, false, false);
			}
			FlingUtils.FlingChildren(childrenConfig, base.transform, Vector3.zero, new MinMaxFloat?(new MinMaxFloat(0f, 0.001f)));
		}
		Vector2 spawnPos = base.transform.position;
		if (this.profile)
		{
			foreach (GameObject effectPrefab in this.profile.SpawnOnLand)
			{
				this.SpawnEffect(effectPrefab, spawnPos);
			}
		}
		if (this.landVoiceAudioTable && this.audioLoopVoice)
		{
			this.audioLoopVoice.Stop();
			this.audioLoopVoice.loop = false;
			this.audioLoopVoice.clip = this.landVoiceAudioTable.SelectClip(false);
			this.audioLoopVoice.volume = this.landVoiceAudioTable.SelectVolume();
			this.audioLoopVoice.pitch = this.landVoiceAudioTable.SelectPitch();
			this.audioLoopVoice.Play();
		}
		if (!this.explodeOnLand)
		{
			yield break;
		}
		if (didSplash && this.splashLandDelay > 0f)
		{
			yield return new WaitForSeconds(this.splashLandDelay);
		}
		if (this.body)
		{
			this.body.linearVelocity = Vector2.zero;
			this.body.angularVelocity = 0f;
		}
		if (this.profile)
		{
			BloodSpawner.SpawnBlood(this.profile.ExplodeBlood, base.transform, this.bloodColorOverride);
		}
		if (this.splatAudioClipTable)
		{
			this.splatAudioClipTable.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, false, 1f, null);
		}
		if (this.activateOnExplode)
		{
			this.activateOnExplode.SetActive(true);
		}
		if (this.audioLoopVoice != null)
		{
			this.audioLoopVoice.Stop();
		}
		if (this.spriteAnimator)
		{
			yield return base.StartCoroutine(this.spriteAnimator.PlayAnimWait("Death Land", null));
		}
		if (this.meshRenderer)
		{
			this.meshRenderer.enabled = false;
		}
		if (this.profile != null && this.profile.SpawnOnExplode != null)
		{
			foreach (GameObject effectPrefab2 in this.profile.SpawnOnExplode)
			{
				this.SpawnEffect(effectPrefab2, spawnPos);
			}
		}
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
		if (component)
		{
			component.enabled = false;
		}
		if (this.body)
		{
			this.body.isKinematic = true;
		}
		yield break;
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x00070DCC File Offset: 0x0006EFCC
	private void SpawnEffect(GameObject effectPrefab, Vector2 pos)
	{
		GameObject gameObject = effectPrefab.Spawn();
		gameObject.transform.SetPosition2D(pos);
		float blackThreadAmount = base.GetBlackThreadAmount();
		if (blackThreadAmount > 0f)
		{
			BlackThreadEffectRendererGroup component = gameObject.GetComponent<BlackThreadEffectRendererGroup>();
			if (component != null)
			{
				component.SetBlackThreadAmount(blackThreadAmount);
			}
		}
		if (this.bloodColorOverride == null)
		{
			return;
		}
		SpawnedCorpseEffectTint component2 = gameObject.GetComponent<SpawnedCorpseEffectTint>();
		if (component2)
		{
			component2.SetTint(this.bloodColorOverride.Value);
		}
	}

	// Token: 0x04001782 RID: 6018
	[Space]
	[SerializeField]
	private FlingUtils.ChildrenConfig flingChildren = new FlingUtils.ChildrenConfig
	{
		SpeedMin = 15f,
		SpeedMax = 20f,
		AngleMin = 60f,
		AngleMax = 120f
	};

	// Token: 0x04001783 RID: 6019
	[Space]
	[SerializeField]
	private CorpseRegularEffectsProfile profile;

	// Token: 0x04001784 RID: 6020
	[SerializeField]
	private bool doLandInstantly;

	// Token: 0x04001785 RID: 6021
	[SerializeField]
	private bool explodeOnLand;

	// Token: 0x04001786 RID: 6022
	[SerializeField]
	private GameObject activateOnExplode;

	// Token: 0x04001787 RID: 6023
	[SerializeField]
	private GameObject activateOnStunEnd;

	// Token: 0x04001788 RID: 6024
	[SerializeField]
	private bool desaturateOnLand;

	// Token: 0x04001789 RID: 6025
	[SerializeField]
	private bool spriteFacesRight;

	// Token: 0x0400178A RID: 6026
	[SerializeField]
	private RandomAudioClipTable startVoiceAudioTable;

	// Token: 0x0400178B RID: 6027
	[SerializeField]
	private RandomAudioClipTable stunEndVoiceAudioTable;

	// Token: 0x0400178C RID: 6028
	[SerializeField]
	private RandomAudioClipTable landVoiceAudioTable;

	// Token: 0x0400178D RID: 6029
	[SerializeField]
	private AudioSource audioLoopVoice;

	// Token: 0x0400178F RID: 6031
	private GameObject loopingStunEffect;
}
