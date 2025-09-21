using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000514 RID: 1300
public class LifebloodPustule : MonoBehaviour, IHitResponder
{
	// Token: 0x17000530 RID: 1328
	// (get) Token: 0x06002E68 RID: 11880 RVA: 0x000CBF2B File Offset: 0x000CA12B
	private Transform EffectSpawnTransform
	{
		get
		{
			if (!this.pustule)
			{
				return base.transform;
			}
			return this.pustule.transform;
		}
	}

	// Token: 0x06002E69 RID: 11881 RVA: 0x000CBF4C File Offset: 0x000CA14C
	private void Awake()
	{
		this.nonBouncer = base.GetComponent<NonBouncer>();
		this.jitters = new List<JitterSelf>();
		List<JitterSelf> list = new List<JitterSelf>();
		if (this.witherParent)
		{
			this.witherSubs = this.witherParent.GetComponentsInChildren<LifebloodPustuleSubWither>();
			this.witherParent.GetComponentsInChildren<JitterSelf>(list);
			this.jitters.AddRange(list);
		}
		if (this.extractedBreakDisable)
		{
			this.extractedBreakDisable.GetComponentsInChildren<JitterSelf>(list);
			this.jitters.AddRange(list);
		}
		if (this.persistentBroken)
		{
			this.persistentBroken.OnGetSaveState += delegate(out bool value)
			{
				value = this.isExtracted;
			};
			this.persistentBroken.OnSetSaveState += delegate(bool value)
			{
				this.isExtracted = value;
				this.UpdateInitialState();
			};
		}
		EventRegister.GetRegisterGuaranteed(base.gameObject, "EXTRACT FINISH").ReceivedEvent += delegate()
		{
			if (!this.isExtracting)
			{
				return;
			}
			this.isExtracting = false;
			this.isExtracted = true;
			this.Break(this.EffectSpawnTransform, true);
		};
		EventRegister.GetRegisterGuaranteed(base.gameObject, "EXTRACT CANCEL").ReceivedEvent += delegate()
		{
			if (this.isExtracting)
			{
				this.isExtracting = false;
			}
		};
		this.UpdateInitialState();
		if (this.extractedBreakEffects)
		{
			this.extractedBreakEffects.SetActive(false);
		}
		this.collider2D = base.GetComponent<Collider2D>();
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x000CC07C File Offset: 0x000CA27C
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.isExtracted || this.isBroken)
		{
			return IHitResponder.Response.None;
		}
		Transform effectSpawnTransform = this.EffectSpawnTransform;
		Vector3 position = effectSpawnTransform.position;
		bool flag = damageInstance.AttackType == AttackTypes.ExtractMoss;
		if (flag)
		{
			this.isExtracting = true;
		}
		if (this.isExtracting)
		{
			EventRegister.SendEvent(EventRegisterEvents.StartExtractB, null);
			HeroController instance = HeroController.instance;
			Vector2 position2 = instance.Body.position;
			float num = base.transform.position.y + this.heroPosOffset.y;
			bool flag2 = false;
			if (this.collider2D != null)
			{
				float x = this.collider2D.bounds.center.x;
				if (instance.cState.facingRight)
				{
					position2.x = x - this.heroPosOffset.x;
				}
				else
				{
					position2.x = x + this.heroPosOffset.x;
				}
				flag2 = true;
			}
			if (flag2 || num > position2.y)
			{
				Vector2 position3 = new Vector2(position2.x, num);
				instance.Body.MovePosition(position3);
				instance.Body.linearVelocity = Vector2.zero;
				this.EnsureExtractPosition(instance, position3);
			}
		}
		this.hitAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		this.hitCount++;
		if (this.hitCount < this.hitsToBreak || this.isExtracting)
		{
			if (this.spriteFlash)
			{
				this.spriteFlash.flashFocusGet();
			}
			BloodSpawner.SpawnBlood(this.hitBlood, effectSpawnTransform, new Color?(this.bloodColor));
			this.onHit.Invoke();
		}
		else
		{
			this.Break(effectSpawnTransform, flag);
		}
		foreach (GameObject gameObject in this.hitEffectPrefabs)
		{
			if (gameObject)
			{
				gameObject.Spawn(position);
			}
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x000CC274 File Offset: 0x000CA474
	private void Break(Transform effectSpawnTransform, bool isExtracted)
	{
		if (!isExtracted)
		{
			this.isExtracting = false;
			EventRegister.SendEvent(EventRegisterEvents.ExtractCancel, null);
		}
		if (this.pustule)
		{
			this.pustule.SetActive(false);
		}
		BloodSpawner.SpawnBlood(this.breakBlood, effectSpawnTransform, new Color?(this.bloodColor));
		this.isBroken = true;
		if (this.nonBouncer)
		{
			this.nonBouncer.active = true;
		}
		this.breakCameraShake.DoShake(this, true);
		this.onBreak.Invoke();
		this.breakAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		base.StartCoroutine(isExtracted ? this.ExtractedBreak() : this.Regenerate());
	}

	// Token: 0x06002E6C RID: 11884 RVA: 0x000CC330 File Offset: 0x000CA530
	private void UpdateInitialState()
	{
		if (this.pustule)
		{
			this.pustule.SetActive(!this.isExtracted);
		}
		if (this.nonBouncer)
		{
			this.nonBouncer.active = this.isExtracted;
		}
		if (this.extractedBreakDisable)
		{
			this.extractedBreakDisable.SetActive(!this.isExtracted);
		}
		if (this.extractedBreakEnable)
		{
			this.extractedBreakEnable.SetActive(this.isExtracted);
		}
		if (this.isExtracted && this.witherSubs != null)
		{
			LifebloodPustuleSubWither[] array = this.witherSubs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StartWithered();
			}
		}
	}

	// Token: 0x06002E6D RID: 11885 RVA: 0x000CC3E8 File Offset: 0x000CA5E8
	private IEnumerator Regenerate()
	{
		yield return new WaitForSeconds(this.regenerateDelay);
		if (this.pustule)
		{
			this.pustule.SetActive(true);
		}
		if (this.nonBouncer)
		{
			this.nonBouncer.active = false;
		}
		this.onRegenerateStart.Invoke();
		this.regenAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		this.animator.Play(LifebloodPustule._regenerateAnimId, 0, 0f);
		yield return null;
		yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		yield return null;
		this.animator.cullingMode = AnimatorCullingMode.CullCompletely;
		this.isBroken = false;
		this.hitCount = 0;
		yield break;
	}

	// Token: 0x06002E6E RID: 11886 RVA: 0x000CC3F7 File Offset: 0x000CA5F7
	private IEnumerator ExtractedBreak()
	{
		this.extractedBreakAnticRumble.DoShake(this, true);
		foreach (JitterSelf jitterSelf in this.jitters)
		{
			jitterSelf.StartJitter();
		}
		yield return new WaitForSeconds(this.extractedBreakAnticTime);
		foreach (JitterSelf jitterSelf2 in this.jitters)
		{
			jitterSelf2.StopJitterWithDecay();
		}
		this.extractedBreakAnticRumble.CancelShake();
		this.extractedBreakShake.DoShake(this, true);
		this.extractedBreakAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		if (this.extractedBreakDisable)
		{
			this.extractedBreakDisable.SetActive(false);
		}
		if (this.extractedBreakEnable)
		{
			this.extractedBreakEnable.SetActive(true);
		}
		if (this.witherSubs != null)
		{
			LifebloodPustuleSubWither[] array = this.witherSubs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].BeginWither(base.transform);
			}
		}
		if (this.extractedBreakEffects)
		{
			this.extractedBreakEffects.SetActive(true);
		}
		yield break;
	}

	// Token: 0x06002E6F RID: 11887 RVA: 0x000CC406 File Offset: 0x000CA606
	private void EnsureExtractPosition(HeroController hc, Vector2 position)
	{
		if (this.extractPositionRoutine == null)
		{
			this.extractPositionRoutine = base.StartCoroutine(this.ExtractPositionRoutine(hc, position));
		}
	}

	// Token: 0x06002E70 RID: 11888 RVA: 0x000CC424 File Offset: 0x000CA624
	private IEnumerator ExtractPositionRoutine(HeroController hc, Vector2 position)
	{
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		while (this.isExtracting)
		{
			hc.Body.MovePosition(position);
			yield return wait;
			if (Mathf.Abs(hc.transform.position.x - position.x) < 1E-45f)
			{
				break;
			}
		}
		hc.Body.linearVelocity = Vector2.zero;
		this.extractPositionRoutine = null;
		yield break;
	}

	// Token: 0x040030CE RID: 12494
	[SerializeField]
	private PersistentBoolItem persistentBroken;

	// Token: 0x040030CF RID: 12495
	[Space]
	[SerializeField]
	private int hitsToBreak;

	// Token: 0x040030D0 RID: 12496
	[SerializeField]
	private GameObject pustule;

	// Token: 0x040030D1 RID: 12497
	[SerializeField]
	private SpriteFlash spriteFlash;

	// Token: 0x040030D2 RID: 12498
	[SerializeField]
	private float regenerateDelay;

	// Token: 0x040030D3 RID: 12499
	[SerializeField]
	private RandomAudioClipTable regenAudioClipTable;

	// Token: 0x040030D4 RID: 12500
	[SerializeField]
	private Animator animator;

	// Token: 0x040030D5 RID: 12501
	[SerializeField]
	private float extractedBreakAnticTime;

	// Token: 0x040030D6 RID: 12502
	[SerializeField]
	private CameraShakeTarget extractedBreakAnticRumble;

	// Token: 0x040030D7 RID: 12503
	[SerializeField]
	private CameraShakeTarget extractedBreakShake;

	// Token: 0x040030D8 RID: 12504
	[SerializeField]
	private RandomAudioClipTable extractedBreakAudioClipTable;

	// Token: 0x040030D9 RID: 12505
	[SerializeField]
	private GameObject extractedBreakDisable;

	// Token: 0x040030DA RID: 12506
	[SerializeField]
	private GameObject extractedBreakEnable;

	// Token: 0x040030DB RID: 12507
	[SerializeField]
	private GameObject extractedBreakEffects;

	// Token: 0x040030DC RID: 12508
	[SerializeField]
	private GameObject witherParent;

	// Token: 0x040030DD RID: 12509
	[Space]
	[SerializeField]
	private GameObject[] hitEffectPrefabs;

	// Token: 0x040030DE RID: 12510
	[SerializeField]
	private BloodSpawner.Config hitBlood;

	// Token: 0x040030DF RID: 12511
	[SerializeField]
	private RandomAudioClipTable hitAudioClipTable;

	// Token: 0x040030E0 RID: 12512
	[SerializeField]
	private BloodSpawner.Config breakBlood;

	// Token: 0x040030E1 RID: 12513
	[SerializeField]
	private RandomAudioClipTable breakAudioClipTable;

	// Token: 0x040030E2 RID: 12514
	[SerializeField]
	private Color bloodColor;

	// Token: 0x040030E3 RID: 12515
	[SerializeField]
	private CameraShakeTarget breakCameraShake;

	// Token: 0x040030E4 RID: 12516
	[SerializeField]
	private Vector2 heroPosOffset;

	// Token: 0x040030E5 RID: 12517
	[Space]
	[SerializeField]
	private UnityEvent onHit;

	// Token: 0x040030E6 RID: 12518
	[SerializeField]
	private UnityEvent onBreak;

	// Token: 0x040030E7 RID: 12519
	[SerializeField]
	private UnityEvent onRegenerateStart;

	// Token: 0x040030E8 RID: 12520
	private bool isBroken;

	// Token: 0x040030E9 RID: 12521
	private bool isExtracting;

	// Token: 0x040030EA RID: 12522
	private bool isExtracted;

	// Token: 0x040030EB RID: 12523
	private int hitCount;

	// Token: 0x040030EC RID: 12524
	private NonBouncer nonBouncer;

	// Token: 0x040030ED RID: 12525
	private LifebloodPustuleSubWither[] witherSubs;

	// Token: 0x040030EE RID: 12526
	private List<JitterSelf> jitters;

	// Token: 0x040030EF RID: 12527
	private static readonly int _regenerateAnimId = Animator.StringToHash("Regenerate");

	// Token: 0x040030F0 RID: 12528
	private Collider2D collider2D;

	// Token: 0x040030F1 RID: 12529
	private Coroutine extractPositionRoutine;
}
