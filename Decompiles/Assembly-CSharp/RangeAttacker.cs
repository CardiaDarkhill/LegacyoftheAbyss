using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000536 RID: 1334
public class RangeAttacker : MonoBehaviour
{
	// Token: 0x17000549 RID: 1353
	// (get) Token: 0x06002FCD RID: 12237 RVA: 0x000D26C8 File Offset: 0x000D08C8
	// (set) Token: 0x06002FCE RID: 12238 RVA: 0x000D26D0 File Offset: 0x000D08D0
	public bool HasAppearTrigger { get; private set; }

	// Token: 0x1700054A RID: 1354
	// (get) Token: 0x06002FCF RID: 12239 RVA: 0x000D26D9 File Offset: 0x000D08D9
	public bool IsOut
	{
		get
		{
			return this.isOut;
		}
	}

	// Token: 0x1700054B RID: 1355
	// (get) Token: 0x06002FD0 RID: 12240 RVA: 0x000D26E1 File Offset: 0x000D08E1
	// (set) Token: 0x06002FD1 RID: 12241 RVA: 0x000D26E8 File Offset: 0x000D08E8
	public static Vector2 LastDamageSinkDirection { get; set; }

	// Token: 0x06002FD2 RID: 12242 RVA: 0x000D26F0 File Offset: 0x000D08F0
	private bool IsUsingMecanim()
	{
		return this.animator;
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x000D2700 File Offset: 0x000D0900
	private void Awake()
	{
		this.appearAnimId = Animator.StringToHash(this.appearAnim);
		this.loopAnimId = Animator.StringToHash(this.loopAnim);
		this.disappearAnimId = Animator.StringToHash(this.disappearAnim);
		this.singAnimId = Animator.StringToHash(this.singAnim);
		this.singEndAnimId = Animator.StringToHash(this.singEndAnim);
		this.HasAppearTrigger = (this.trigger != null);
		if (this.HasAppearTrigger)
		{
			this.trigger.InsideStateChanged += this.OnInsideStateChanged;
		}
		HeroPerformanceRegion.StartedPerforming += this.OnHeroStartedPerforming;
		HeroPerformanceRegion.StoppedPerforming += this.OnHeroStoppedPerforming;
		if (!this.isChildAttacker)
		{
			if (this.explosionTrigger)
			{
				this.explosionTrigger.OnTriggerEntered += this.OnExplosionTriggerEntered;
			}
			if (this.customDamageTrigger)
			{
				this.customDamageTrigger.OnTriggerEntered += this.OnCustomDamageTriggerEntered;
			}
			this.collider = base.GetComponent<Collider2D>();
		}
		if (this.tk2dSprite)
		{
			this.tk2dSpriteRenderer = this.tk2dSprite.GetComponent<MeshRenderer>();
		}
		this.hasCollider = (this.collider != null);
		this.healthManager = base.GetComponent<HealthManager>();
		this.hasHealthManager = (this.healthManager != null);
		if (this.hasHealthManager)
		{
			this.healthManager.TookDamage += delegate()
			{
				this.explosionDisappear = true;
			};
		}
		this.needolinTextOwner = base.GetComponent<NeedolinTextOwner>();
		this.hasVoiceSource = (this.voiceSource != null);
		this.hasAppearVoice = (this.appearVoice != null);
		this.hasHideVoice = (this.hideVoice != null);
		this.hasSingVoice = (this.singVoice != null);
	}

	// Token: 0x06002FD4 RID: 12244 RVA: 0x000D28D4 File Offset: 0x000D0AD4
	private void Start()
	{
		this.hasAnimator = this.animator;
		this.hasSprite = this.sprite;
		this.hasTk2dAnimator = this.tk2dAnimator;
		this.hasTk2dSprite = this.tk2dSprite;
		this.hasTk2dSpriteRenderer = this.tk2dSpriteRenderer;
		if (this.hasAnimator)
		{
			this.animator.enabled = true;
			this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.animator.Play(this.disappearAnim, 0, 1f);
			this.animator.enabled = false;
			this.animator.Update(0f);
		}
		else if (this.hasTk2dAnimator)
		{
			tk2dSpriteAnimationClip clipByName = this.tk2dAnimator.GetClipByName(this.disappearAnim);
			if (clipByName != null)
			{
				this.tk2dAnimator.PlayFromFrame(clipByName, clipByName.frames.Length - 1);
			}
		}
		if (this.hasSprite)
		{
			this.sprite.enabled = false;
		}
		else if (this.hasTk2dSpriteRenderer)
		{
			this.tk2dSpriteRenderer.enabled = false;
		}
		if (!this.isChildAttacker)
		{
			this.hasDamager = (this.damager != null);
		}
		if (this.hasCollider)
		{
			this.collider.enabled = false;
		}
		if (this.loopAudioSource)
		{
			this.loopAudioSource.Stop();
		}
		if (this.isChildAttacker)
		{
			this.OnInsideStateChanged(false);
			return;
		}
		if (this.trigger)
		{
			this.OnInsideStateChanged(this.trigger.IsInside);
		}
	}

	// Token: 0x06002FD5 RID: 12245 RVA: 0x000D2A5C File Offset: 0x000D0C5C
	private void OnDestroy()
	{
		if (this.trigger)
		{
			this.trigger.InsideStateChanged -= this.OnInsideStateChanged;
		}
		HeroPerformanceRegion.StartedPerforming -= this.OnHeroStartedPerforming;
		HeroPerformanceRegion.StoppedPerforming -= this.OnHeroStoppedPerforming;
		this.SetAudioLoopActive(false);
	}

	// Token: 0x06002FD6 RID: 12246 RVA: 0x000D2AB6 File Offset: 0x000D0CB6
	private void OnDisable()
	{
		this.insideMask = 0;
	}

	// Token: 0x06002FD7 RID: 12247 RVA: 0x000D2AC0 File Offset: 0x000D0CC0
	private void OnInsideStateChanged(bool isInside)
	{
		this.isInside = isInside;
		bool flag = isInside == this.targetInsideState;
		if (flag && Random.Range(0f, 1f) > this.appearChance)
		{
			return;
		}
		this.isTargetTrigger = flag;
		if (!this.isTargetTrigger)
		{
			return;
		}
		this.EnsureAnimStarted();
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x000D2B0F File Offset: 0x000D0D0F
	public void SetInsideState(int groupMask, bool isInside)
	{
		if (isInside)
		{
			this.insideMask |= groupMask;
		}
		else
		{
			this.insideMask &= ~groupMask;
		}
		this.SetInsideState(this.insideMask != 0);
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x000D2B42 File Offset: 0x000D0D42
	public void SetInsideState(bool isInside)
	{
		if (this.isInside == isInside)
		{
			return;
		}
		this.OnInsideStateChanged(isInside);
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x000D2B58 File Offset: 0x000D0D58
	private void OnHeroStartedPerforming()
	{
		if (!this.singRange || !this.singRange.IsInside)
		{
			return;
		}
		if (this.singExcludeRange && this.singExcludeRange.transform.IsOnHeroPlane() && this.singExcludeRange.IsInside)
		{
			return;
		}
		this.isHeroPerforming = true;
		this.EnsureAnimStarted();
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x000D2BBA File Offset: 0x000D0DBA
	private void OnHeroStoppedPerforming()
	{
		this.isHeroPerforming = false;
		if (this.needolinTextOwner && this.needolinTextOwner.RangeCheck == NeedolinTextOwner.NeedolinRangeCheckTypes.Manual)
		{
			this.needolinTextOwner.RemoveNeedolinText();
		}
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x000D2BE9 File Offset: 0x000D0DE9
	private void EnsureAnimStarted()
	{
		if (this.animRoutine == null)
		{
			this.animRoutine = base.StartCoroutine(this.Anim());
		}
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x000D2C05 File Offset: 0x000D0E05
	private IEnumerator Anim()
	{
		HeroController hc = HeroController.SilentInstance;
		if (!hc)
		{
			yield break;
		}
		while (this.isTargetTrigger || this.isHeroPerforming)
		{
			if (this.isHeroPerforming)
			{
				float waitTimeLeft = this.singAppearDelay.GetRandomValue();
				while (waitTimeLeft > 0f && this.isHeroPerforming)
				{
					waitTimeLeft -= Time.deltaTime;
					yield return null;
				}
				if (!this.isHeroPerforming && !this.isTargetTrigger)
				{
					break;
				}
			}
			this.explosionDisappear = false;
			if (!this.dontFlipX && Random.Range(0, 2) == 0)
			{
				base.transform.FlipLocalScale(true, false, false);
			}
			float appearDelayLeft = this.appearDelay.GetRandomValue();
			while (appearDelayLeft > 0f)
			{
				appearDelayLeft -= Time.deltaTime;
				yield return null;
			}
			if (this.hasSprite)
			{
				this.sprite.enabled = true;
			}
			else if (this.hasTk2dSpriteRenderer)
			{
				this.tk2dSpriteRenderer.enabled = true;
			}
			this.isOut = true;
			this.PlayAppearAudio();
			this.SetAudioLoopActive(true);
			yield return base.StartCoroutine(this.PlayAnimWait(this.appearAnim, this.appearAnimId));
			if (base.transform.IsOnHeroPlane() && this.hasCollider)
			{
				this.collider.enabled = true;
			}
			this.PlayAnim(this.loopAnim, this.loopAnimId);
			yield return null;
			if (this.hasAnimator)
			{
				this.animator.cullingMode = AnimatorCullingMode.CullCompletely;
			}
			for (;;)
			{
				float waitTimeLeft;
				if (this.isHeroPerforming)
				{
					waitTimeLeft = this.singStartDelay.GetRandomValue();
					while (waitTimeLeft > 0f)
					{
						waitTimeLeft -= Time.deltaTime;
						yield return null;
					}
					this.PlayAnim(this.singAnim, this.singAnimId);
					this.ToggleSingLoop(true);
					if (this.isHeroPerforming && this.needolinTextOwner && this.needolinTextOwner.RangeCheck == NeedolinTextOwner.NeedolinRangeCheckTypes.Manual)
					{
						this.needolinTextOwner.AddNeedolinText();
					}
					while (this.isHeroPerforming)
					{
						yield return null;
					}
					waitTimeLeft = this.singEndDelay.GetRandomValue();
					while (waitTimeLeft > 0f && !this.explosionDisappear)
					{
						waitTimeLeft -= Time.deltaTime;
						yield return null;
					}
					this.ToggleSingLoop(false);
					if (!this.explosionDisappear)
					{
						yield return base.StartCoroutine(this.PlayAnimWait(this.singEndAnim, this.singEndAnimId));
					}
					this.PlayAnim(this.loopAnim, this.loopAnimId);
					yield return null;
					if (this.hasAnimator)
					{
						this.animator.cullingMode = AnimatorCullingMode.CullCompletely;
					}
				}
				waitTimeLeft = this.minLoopTime.GetRandomValue();
				while (waitTimeLeft > 0f)
				{
					if (this.explosionDisappear)
					{
						break;
					}
					waitTimeLeft -= Time.deltaTime;
					yield return null;
				}
				for (;;)
				{
					if (!this.isTargetTrigger || this.explosionDisappear)
					{
						bool skipDisappear = false;
						while (hc.cState.hazardDeath || hc.cState.dead)
						{
							skipDisappear = true;
							yield return null;
						}
						if (skipDisappear)
						{
							goto Block_27;
						}
						if (this.explosionDisappear)
						{
							goto IL_4F7;
						}
						waitTimeLeft = this.disappearDelay.GetRandomValue();
						while (waitTimeLeft > 0f)
						{
							waitTimeLeft -= Time.deltaTime;
							yield return null;
						}
						if (!this.isTargetTrigger)
						{
							goto IL_4F7;
						}
					}
					else
					{
						yield return null;
						if (this.isHeroPerforming)
						{
							break;
						}
					}
				}
			}
			IL_540:
			this.SetAudioLoopActive(false);
			if (this.hasAnimator)
			{
				this.animator.enabled = false;
			}
			if (this.hasSprite)
			{
				this.sprite.enabled = false;
			}
			else if (this.hasTk2dSpriteRenderer)
			{
				this.tk2dSpriteRenderer.enabled = false;
			}
			this.isOut = false;
			if (this.healthManager)
			{
				this.healthManager.RefillHP();
				continue;
			}
			continue;
			Block_27:
			if (this.hasCollider)
			{
				this.collider.enabled = false;
				goto IL_540;
			}
			goto IL_540;
			IL_4F7:
			if (this.hasCollider)
			{
				this.collider.enabled = false;
			}
			this.PlayHideAudio();
			yield return base.StartCoroutine(this.PlayAnimWait(this.disappearAnim, this.disappearAnimId));
			goto IL_540;
		}
		this.animRoutine = null;
		yield break;
	}

	// Token: 0x06002FDE RID: 12254 RVA: 0x000D2C14 File Offset: 0x000D0E14
	private void PlayAnim(string animName, int animId)
	{
		if (this.hasAnimator)
		{
			this.animator.enabled = true;
			this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.animator.Play(animId);
			return;
		}
		if (this.hasTk2dAnimator)
		{
			this.tk2dAnimator.Play(animName);
		}
	}

	// Token: 0x06002FDF RID: 12255 RVA: 0x000D2C62 File Offset: 0x000D0E62
	private IEnumerator PlayAnimWait(string animName, int animId)
	{
		if (this.hasAnimator)
		{
			this.animator.enabled = true;
			this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.animator.Play(animId);
			yield return null;
			float waitTimeLeft = this.animator.GetCurrentAnimatorStateInfo(0).length;
			while (waitTimeLeft > 0f)
			{
				waitTimeLeft -= Time.deltaTime;
				yield return null;
			}
		}
		else if (this.hasTk2dAnimator)
		{
			tk2dSpriteAnimationClip clipByName = this.tk2dAnimator.GetClipByName(animName);
			if (clipByName != null)
			{
				this.tk2dAnimator.Play(clipByName);
				float waitTimeLeft = clipByName.Duration;
				while (waitTimeLeft > 0f)
				{
					waitTimeLeft -= Time.deltaTime;
					yield return null;
				}
			}
		}
		yield break;
	}

	// Token: 0x06002FE0 RID: 12256 RVA: 0x000D2C80 File Offset: 0x000D0E80
	private bool CanDamagerShred(DamageEnemies otherDamager)
	{
		if (otherDamager.attackType == AttackTypes.Explosion || otherDamager.CompareTag("Explosion"))
		{
			return true;
		}
		ToolItem representingTool = otherDamager.RepresentingTool;
		return representingTool && (representingTool.DamageFlags & ToolDamageFlags.Shredding) != ToolDamageFlags.None;
	}

	// Token: 0x06002FE1 RID: 12257 RVA: 0x000D2CC4 File Offset: 0x000D0EC4
	private void OnExplosionTriggerEntered(Collider2D collision, GameObject sender)
	{
		if (!this.isOut)
		{
			return;
		}
		DamageEnemies component = collision.GetComponent<DamageEnemies>();
		if (!component)
		{
			return;
		}
		if (!this.CanDamagerShred(component))
		{
			return;
		}
		this.ReactToExplosion();
	}

	// Token: 0x06002FE2 RID: 12258 RVA: 0x000D2CFC File Offset: 0x000D0EFC
	public void ReactToExplosion()
	{
		if (!base.transform.IsOnHeroPlane())
		{
			return;
		}
		this.explosionDisappear = true;
		if (this.journalRecord)
		{
			int randomValue = this.journalAmountPerKill.GetRandomValue(true);
			for (int i = 0; i < randomValue; i++)
			{
				EnemyJournalManager.RecordKill(this.journalRecord, true);
			}
		}
		if (this.explosionPrefab)
		{
			this.explosionPrefab.Spawn(base.transform).transform.SetParentReset(base.transform);
		}
	}

	// Token: 0x06002FE3 RID: 12259 RVA: 0x000D2D80 File Offset: 0x000D0F80
	private void OnCustomDamageTriggerEntered(Collider2D collision, GameObject sender)
	{
		if (collision.gameObject.layer != 20)
		{
			return;
		}
		if (collision.gameObject.GetComponentInParent<HeroController>() && CheatManager.Invincibility == CheatManager.InvincibilityStates.FullInvincible)
		{
			return;
		}
		if (this.sinkTarget)
		{
			Vector2 b = base.transform.position;
			RangeAttacker.LastDamageSinkDirection = (this.sinkTarget.position - b).normalized;
		}
		EventRegister.SendEvent(this.customDamageEventRegister, null);
	}

	// Token: 0x06002FE4 RID: 12260 RVA: 0x000D2E08 File Offset: 0x000D1008
	private void SetAudioLoopActive(bool value)
	{
		if (!this.loopAudioSource)
		{
			return;
		}
		if (!(value ? RangeAttacker._queuedLoopSources.Add(this.loopAudioSource) : RangeAttacker._queuedLoopSources.Remove(this.loopAudioSource)))
		{
			return;
		}
		HeroController silentInstance = HeroController.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		AudioSource audioSource = null;
		float num = float.MaxValue;
		Vector3 position = silentInstance.transform.position;
		foreach (AudioSource audioSource2 in RangeAttacker._queuedLoopSources)
		{
			float num2 = Vector3.Distance(audioSource2.transform.position, position);
			if (num2 <= num)
			{
				num = num2;
				audioSource = audioSource2;
			}
		}
		if (audioSource == RangeAttacker._previousLoopSource)
		{
			return;
		}
		float num3 = 0f;
		if (audioSource != null)
		{
			double num4 = AudioSettings.dspTime;
			float num5 = 0f;
			if (RangeAttacker._previousLoopSource)
			{
				num5 = 0.3f;
				num4 += (double)num5;
				num3 = RangeAttacker._previousLoopSource.time;
				RangeAttacker._previousLoopSource.SetScheduledEndTime(num4);
				RangeAttacker._previousLoopSource = null;
			}
			float num6 = num3 + num5;
			if (audioSource.clip)
			{
				float length = audioSource.clip.length;
				if (length > 0f)
				{
					while (num6 > length)
					{
						num6 -= length;
					}
				}
				else
				{
					num6 = 0f;
				}
			}
			audioSource.time = num6;
			audioSource.PlayScheduled(num4);
			RangeAttacker._previousLoopSource = audioSource;
			return;
		}
		if (RangeAttacker._previousLoopSource)
		{
			num3 = RangeAttacker._previousLoopSource.time;
			RangeAttacker._previousLoopSource.Stop();
			RangeAttacker._previousLoopSource = null;
		}
	}

	// Token: 0x06002FE5 RID: 12261 RVA: 0x000D2FB8 File Offset: 0x000D11B8
	private void PlayVoiceTableUnsafe(RandomAudioClipTable table)
	{
		if (!this.hasVoiceSource)
		{
			table.SpawnAndPlayOneShot(base.transform.position, false);
			return;
		}
		AudioClip audioClip = table.SelectClip(false);
		if (audioClip == null)
		{
			return;
		}
		this.voiceSource.pitch = table.SelectPitch();
		float volumeScale = table.SelectVolume();
		this.voiceSource.PlayOneShot(audioClip, volumeScale);
		table.ReportPlayed(audioClip, null);
	}

	// Token: 0x06002FE6 RID: 12262 RVA: 0x000D3020 File Offset: 0x000D1220
	private void PlayAppearAudio()
	{
		this.appearAudio.SpawnAndPlayOneShot(base.transform.position, null);
		if (this.hasAppearVoice)
		{
			this.PlayVoiceTableUnsafe(this.appearVoice);
		}
	}

	// Token: 0x06002FE7 RID: 12263 RVA: 0x000D304E File Offset: 0x000D124E
	private void PlayHideAudio()
	{
		this.disappearAudio.SpawnAndPlayOneShot(base.transform.position, null);
		if (this.hasHideVoice)
		{
			this.PlayVoiceTableUnsafe(this.hideVoice);
		}
	}

	// Token: 0x06002FE8 RID: 12264 RVA: 0x000D307C File Offset: 0x000D127C
	private void ToggleSingLoop(bool sing)
	{
		if (this.isSinging == sing)
		{
			return;
		}
		this.isSinging = sing;
		if (this.hasVoiceSource && this.hasSingVoice)
		{
			if (sing)
			{
				this.voiceSource.pitch = this.singVoice.SelectPitch();
				this.voiceSource.volume = this.singVoice.SelectVolume();
				this.voiceSource.clip = this.singVoice.SelectClip(false);
				this.voiceSource.Play();
				return;
			}
			this.voiceSource.Stop();
			this.voiceSource.volume = 1f;
		}
	}

	// Token: 0x06002FE9 RID: 12265 RVA: 0x000D3117 File Offset: 0x000D1317
	public Vector3 GetOrigin()
	{
		if (this.hasOrigin)
		{
			return this.origin;
		}
		return base.transform.position;
	}

	// Token: 0x06002FEA RID: 12266 RVA: 0x000D3133 File Offset: 0x000D1333
	public void SetOrigin(Vector3 localPosition)
	{
		this.hasOrigin = true;
		this.origin = base.transform.TransformPoint(localPosition);
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x000D3150 File Offset: 0x000D1350
	public void MarkChild()
	{
		this.isChildAttacker = true;
		if (this.HasAppearTrigger)
		{
			this.trigger.InsideStateChanged -= this.OnInsideStateChanged;
			this.trigger.gameObject.SetActive(false);
			this.HasAppearTrigger = false;
		}
		this.trigger = null;
		if (this.explosionTrigger)
		{
			this.explosionTrigger.OnTriggerEntered -= this.OnExplosionTriggerEntered;
			this.explosionTrigger.gameObject.SetActive(false);
			this.explosionTrigger = null;
		}
		if (this.customDamageTrigger)
		{
			this.customDamageTrigger.OnTriggerEntered -= this.OnCustomDamageTriggerEntered;
			this.customDamageTrigger.gameObject.SetActive(false);
			this.customDamageTrigger = null;
		}
		if (this.hasCollider)
		{
			this.collider.enabled = false;
			this.hasCollider = false;
		}
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x000D3238 File Offset: 0x000D1438
	public void CleanChild()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (this.trigger)
		{
			this.<CleanChild>g__DestroyGameObject|106_0(this.trigger.gameObject);
		}
		this.trigger = null;
		if (this.explosionTrigger)
		{
			this.<CleanChild>g__DestroyGameObject|106_0(this.explosionTrigger.gameObject);
		}
		this.explosionTrigger = null;
		if (this.customDamageTrigger)
		{
			this.<CleanChild>g__DestroyGameObject|106_0(this.customDamageTrigger.gameObject);
		}
		this.customDamageTrigger = null;
		if (this.damager)
		{
			this.<CleanChild>g__DestroyGameObject|106_0(this.damager.gameObject);
		}
		this.damager = null;
	}

	// Token: 0x06002FF0 RID: 12272 RVA: 0x000D331D File Offset: 0x000D151D
	[CompilerGenerated]
	private void <CleanChild>g__DestroyGameObject|106_0(GameObject target)
	{
		target != base.gameObject;
	}

	// Token: 0x04003293 RID: 12947
	[SerializeField]
	private TrackTriggerObjects trigger;

	// Token: 0x04003294 RID: 12948
	[SerializeField]
	private bool targetInsideState = true;

	// Token: 0x04003295 RID: 12949
	[Space]
	[SerializeField]
	private Animator animator;

	// Token: 0x04003296 RID: 12950
	[SerializeField]
	private SpriteRenderer sprite;

	// Token: 0x04003297 RID: 12951
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingMecanim", false, true, false)]
	private tk2dSpriteAnimator tk2dAnimator;

	// Token: 0x04003298 RID: 12952
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingMecanim", false, true, false)]
	private tk2dSprite tk2dSprite;

	// Token: 0x04003299 RID: 12953
	[Space]
	[SerializeField]
	private bool dontFlipX;

	// Token: 0x0400329A RID: 12954
	[SerializeField]
	[Range(0f, 1f)]
	private float appearChance = 1f;

	// Token: 0x0400329B RID: 12955
	[SerializeField]
	private MinMaxFloat appearDelay;

	// Token: 0x0400329C RID: 12956
	[SerializeField]
	private string appearAnim;

	// Token: 0x0400329D RID: 12957
	[SerializeField]
	private AudioEventRandom appearAudio;

	// Token: 0x0400329E RID: 12958
	[SerializeField]
	private string loopAnim;

	// Token: 0x0400329F RID: 12959
	[SerializeField]
	private MinMaxFloat minLoopTime;

	// Token: 0x040032A0 RID: 12960
	[SerializeField]
	private MinMaxFloat disappearDelay;

	// Token: 0x040032A1 RID: 12961
	[SerializeField]
	private string disappearAnim;

	// Token: 0x040032A2 RID: 12962
	[SerializeField]
	private AudioEventRandom disappearAudio;

	// Token: 0x040032A3 RID: 12963
	[Space]
	[SerializeField]
	private GameObject damager;

	// Token: 0x040032A4 RID: 12964
	[SerializeField]
	private TriggerEnterEvent explosionTrigger;

	// Token: 0x040032A5 RID: 12965
	[SerializeField]
	private GameObject explosionPrefab;

	// Token: 0x040032A6 RID: 12966
	[SerializeField]
	private AudioSource loopAudioSource;

	// Token: 0x040032A7 RID: 12967
	[Space]
	[SerializeField]
	private EnemyJournalRecord journalRecord;

	// Token: 0x040032A8 RID: 12968
	[SerializeField]
	private MinMaxInt journalAmountPerKill = new MinMaxInt(1, 1);

	// Token: 0x040032A9 RID: 12969
	[Space]
	[SerializeField]
	private TriggerEnterEvent customDamageTrigger;

	// Token: 0x040032AA RID: 12970
	[SerializeField]
	private string customDamageEventRegister;

	// Token: 0x040032AB RID: 12971
	[SerializeField]
	private Transform sinkTarget;

	// Token: 0x040032AC RID: 12972
	[Space]
	[SerializeField]
	private TrackTriggerObjects singRange;

	// Token: 0x040032AD RID: 12973
	[SerializeField]
	private TrackTriggerObjects singExcludeRange;

	// Token: 0x040032AE RID: 12974
	[SerializeField]
	private MinMaxFloat singAppearDelay;

	// Token: 0x040032AF RID: 12975
	[SerializeField]
	private MinMaxFloat singStartDelay;

	// Token: 0x040032B0 RID: 12976
	[SerializeField]
	private MinMaxFloat singEndDelay;

	// Token: 0x040032B1 RID: 12977
	[SerializeField]
	private string singAnim;

	// Token: 0x040032B2 RID: 12978
	[SerializeField]
	private string singEndAnim;

	// Token: 0x040032B3 RID: 12979
	[Header("Voice")]
	[SerializeField]
	private AudioSource voiceSource;

	// Token: 0x040032B4 RID: 12980
	[SerializeField]
	private RandomAudioClipTable appearVoice;

	// Token: 0x040032B5 RID: 12981
	[SerializeField]
	private RandomAudioClipTable hideVoice;

	// Token: 0x040032B6 RID: 12982
	[SerializeField]
	private RandomAudioClipTable singVoice;

	// Token: 0x040032B8 RID: 12984
	private MeshRenderer tk2dSpriteRenderer;

	// Token: 0x040032B9 RID: 12985
	private Collider2D collider;

	// Token: 0x040032BA RID: 12986
	private int appearAnimId;

	// Token: 0x040032BB RID: 12987
	private int loopAnimId;

	// Token: 0x040032BC RID: 12988
	private int disappearAnimId;

	// Token: 0x040032BD RID: 12989
	private int singAnimId;

	// Token: 0x040032BE RID: 12990
	private int singEndAnimId;

	// Token: 0x040032BF RID: 12991
	private Coroutine animRoutine;

	// Token: 0x040032C0 RID: 12992
	private bool isTargetTrigger;

	// Token: 0x040032C1 RID: 12993
	private bool isHeroPerforming;

	// Token: 0x040032C2 RID: 12994
	private bool isOut;

	// Token: 0x040032C3 RID: 12995
	private bool explosionDisappear;

	// Token: 0x040032C4 RID: 12996
	private HealthManager healthManager;

	// Token: 0x040032C5 RID: 12997
	private NeedolinTextOwner needolinTextOwner;

	// Token: 0x040032C6 RID: 12998
	private static readonly HashSet<AudioSource> _queuedLoopSources = new HashSet<AudioSource>();

	// Token: 0x040032C7 RID: 12999
	private static AudioSource _previousLoopSource;

	// Token: 0x040032C9 RID: 13001
	private bool hasSprite;

	// Token: 0x040032CA RID: 13002
	private bool hasAnimator;

	// Token: 0x040032CB RID: 13003
	private bool hasTk2dSprite;

	// Token: 0x040032CC RID: 13004
	private bool hasTk2dSpriteRenderer;

	// Token: 0x040032CD RID: 13005
	private bool hasTk2dAnimator;

	// Token: 0x040032CE RID: 13006
	private bool hasDamager;

	// Token: 0x040032CF RID: 13007
	private bool hasHealthManager;

	// Token: 0x040032D0 RID: 13008
	private bool hasCollider;

	// Token: 0x040032D1 RID: 13009
	private bool isChildAttacker;

	// Token: 0x040032D2 RID: 13010
	private bool hasOrigin;

	// Token: 0x040032D3 RID: 13011
	private Vector3 origin;

	// Token: 0x040032D4 RID: 13012
	private bool hasVoiceSource;

	// Token: 0x040032D5 RID: 13013
	private bool hasAppearVoice;

	// Token: 0x040032D6 RID: 13014
	private bool hasHideVoice;

	// Token: 0x040032D7 RID: 13015
	private bool hasSingVoice;

	// Token: 0x040032D8 RID: 13016
	private bool isSinging;

	// Token: 0x040032D9 RID: 13017
	private bool isInside;

	// Token: 0x040032DA RID: 13018
	private int insideMask;
}
