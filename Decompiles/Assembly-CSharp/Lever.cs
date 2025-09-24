using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000511 RID: 1297
public class Lever : MonoBehaviour, IHitResponder
{
	// Token: 0x1700052F RID: 1327
	// (get) Token: 0x06002E43 RID: 11843 RVA: 0x000CB15A File Offset: 0x000C935A
	// (set) Token: 0x06002E44 RID: 11844 RVA: 0x000CB162 File Offset: 0x000C9362
	public bool HitBlocked
	{
		get
		{
			return this.hitBlocked;
		}
		set
		{
			this.hitBlocked = value;
			if (this.nonBouncer)
			{
				this.nonBouncer.active = value;
			}
		}
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x000CB184 File Offset: 0x000C9384
	private void Awake()
	{
		if (this.persistent != null)
		{
			if (!string.IsNullOrEmpty(this.playerDataBool))
			{
				Object.Destroy(this.persistent);
				this.persistent = null;
			}
			else
			{
				this.persistent.OnGetSaveState += delegate(out bool value)
				{
					value = this.activated;
				};
				this.persistent.OnSetSaveState += delegate(bool value)
				{
					if (value)
					{
						this.SetActivated(true);
					}
				};
			}
		}
		if (this.activatedTinker)
		{
			this.activatedTinker.SetActive(false);
		}
		this.HitBlocked = this.HitBlocked;
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x000CB213 File Offset: 0x000C9413
	private void Start()
	{
		if (!string.IsNullOrEmpty(this.playerDataBool) && PlayerData.instance.GetVariable(this.playerDataBool))
		{
			this.SetActivated();
		}
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x000CB23C File Offset: 0x000C943C
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!damageInstance.IsFirstHit)
		{
			return IHitResponder.Response.None;
		}
		if (this.activated || this.HitBlocked)
		{
			return IHitResponder.Response.None;
		}
		if (this.canHitTrigger && !this.canHitTrigger.IsInside)
		{
			return IHitResponder.Response.None;
		}
		if (!damageInstance.IsNailDamage)
		{
			return IHitResponder.Response.None;
		}
		this.OnHit.Invoke();
		if (!this.doesNotActivate)
		{
			this.activated = true;
			this.HitBlocked = true;
			if (!string.IsNullOrEmpty(this.playerDataBool))
			{
				PlayerData.instance.SetVariable(this.playerDataBool, true);
			}
		}
		else if (this.setHitBlocked)
		{
			this.HitBlocked = true;
		}
		HeroController instance = HeroController.instance;
		bool flag = false;
		float num;
		switch (damageInstance.GetHitDirection(HitInstance.TargetType.Regular))
		{
		case HitInstance.HitDirection.Left:
			num = -1f;
			flag = true;
			if (this.recoilHero && damageInstance.AttackType == AttackTypes.Nail)
			{
				instance.RecoilRight();
			}
			break;
		case HitInstance.HitDirection.Right:
			num = 1f;
			flag = true;
			if (this.recoilHero && damageInstance.AttackType == AttackTypes.Nail)
			{
				instance.RecoilLeft();
			}
			break;
		case HitInstance.HitDirection.Up:
			num = 1f;
			if (this.recoilHero && damageInstance.IsNailDamage)
			{
				instance.RecoilDown();
			}
			break;
		case HitInstance.HitDirection.Down:
			num = -1f;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (this.animator)
		{
			this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			if (this.animator.HasParameter(this.hitDirectionAnimParam, null))
			{
				float z = base.transform.eulerAngles.z;
				if (flag)
				{
					if (z > 90f && z < 270f)
					{
						num *= -1f;
					}
				}
				else if (z <= 10f || z >= 350f)
				{
					num = (float)(instance.cState.facingRight ? 1 : -1);
				}
				else if (z > 170f && z < 190f)
				{
					num = (float)(instance.cState.facingRight ? -1 : 1);
				}
				else if (z > 180f)
				{
					num *= -1f;
				}
				if (base.transform.lossyScale.x < 0f)
				{
					num *= -1f;
				}
				this.animator.SetFloat(this.hitDirectionAnimParam, num);
			}
			if (this.animator.HasParameter(this.hitAnimParam, null))
			{
				this.animator.SetTrigger(this.hitAnimParam);
			}
			if (this.retractAfterHit)
			{
				this.animator.SetBool(this.retractedAnimParam, true);
			}
		}
		base.StartCoroutine(this.Execute());
		if (this.hitEffect)
		{
			this.hitEffect.Spawn(this.hitEffectPoint ? this.hitEffectPoint.position : base.transform.position);
		}
		this.hitCameraShake.DoShake(this, true);
		this.hitSound.SpawnAndPlayOneShot(base.transform.position, null);
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x000CB547 File Offset: 0x000C9747
	private IEnumerator Execute()
	{
		yield return new WaitForSeconds(this.openGateDelay);
		if (this.activatedTinker && !this.retractAfterHit)
		{
			this.activatedTinker.SetActive(true);
		}
		this.OnHitDelayed.Invoke();
		foreach (UnlockablePropBase unlockablePropBase in this.unlockables)
		{
			if (unlockablePropBase && unlockablePropBase.gameObject.activeInHierarchy)
			{
				unlockablePropBase.Open();
			}
		}
		foreach (PlayMakerFSM playMakerFSM in this.fsmGates)
		{
			if (playMakerFSM)
			{
				playMakerFSM.SendEvent("OPEN");
			}
		}
		if (!string.IsNullOrEmpty(this.sendEventToRegister))
		{
			EventRegister.SendEvent(this.sendEventToRegister, null);
		}
		this.OnActivated.Invoke();
		yield break;
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x000CB556 File Offset: 0x000C9756
	public void SetActivated()
	{
		this.SetActivated(false);
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x000CB560 File Offset: 0x000C9760
	public void SetActivated(bool fromStart)
	{
		if (this.activated)
		{
			return;
		}
		this.activated = true;
		if (this.activatedTinker && !this.retractAfterHit)
		{
			this.activatedTinker.SetActive(true);
		}
		if (fromStart)
		{
			this.OnStartActivated.Invoke();
		}
		else
		{
			this.OnActivated.Invoke();
		}
		foreach (UnlockablePropBase unlockablePropBase in this.unlockables)
		{
			if (unlockablePropBase && unlockablePropBase.gameObject.activeInHierarchy)
			{
				unlockablePropBase.Opened();
			}
		}
		foreach (PlayMakerFSM playMakerFSM in this.fsmGates)
		{
			if (playMakerFSM)
			{
				playMakerFSM.SendEvent("ACTIVATE");
			}
		}
		this.SetActivatedAnim();
		this.HitBlocked = true;
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x000CB629 File Offset: 0x000C9829
	public void SetActivatedInert(bool value)
	{
		this.activated = value;
		this.HitBlocked = value;
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x000CB639 File Offset: 0x000C9839
	public void SetActivatedInertWithAnim(bool value)
	{
		this.activated = value;
		this.HitBlocked = value;
		this.SetActivatedAnim();
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x000CB650 File Offset: 0x000C9850
	private void SetActivatedAnim()
	{
		if (!this.animator)
		{
			return;
		}
		if (this.animator.HasParameter(this.activateAnimParam, null))
		{
			this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.animator.SetTrigger(this.activateAnimParam);
			return;
		}
		if (this.retractAfterHit)
		{
			this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.animator.SetBool(this.retractedAnimParam, true);
			this.animator.Play(this.retractAnimId, 0, 1f);
			return;
		}
		this.animator.gameObject.SetActive(false);
	}

	// Token: 0x04003082 RID: 12418
	private readonly int hitDirectionAnimParam = Animator.StringToHash("Hit Direction");

	// Token: 0x04003083 RID: 12419
	private readonly int hitAnimParam = Animator.StringToHash("Hit");

	// Token: 0x04003084 RID: 12420
	private readonly int activateAnimParam = Animator.StringToHash("Activate");

	// Token: 0x04003085 RID: 12421
	private readonly int retractedAnimParam = Animator.StringToHash("Retracted");

	// Token: 0x04003086 RID: 12422
	private readonly int retractAnimId = Animator.StringToHash("Retract");

	// Token: 0x04003087 RID: 12423
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04003088 RID: 12424
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBool;

	// Token: 0x04003089 RID: 12425
	[SerializeField]
	private Animator animator;

	// Token: 0x0400308A RID: 12426
	[Space]
	[SerializeField]
	private GameObject hitEffect;

	// Token: 0x0400308B RID: 12427
	[SerializeField]
	private Transform hitEffectPoint;

	// Token: 0x0400308C RID: 12428
	[SerializeField]
	private CameraShakeTarget hitCameraShake;

	// Token: 0x0400308D RID: 12429
	[SerializeField]
	private AudioEvent hitSound;

	// Token: 0x0400308E RID: 12430
	[SerializeField]
	private GameObject activatedTinker;

	// Token: 0x0400308F RID: 12431
	[SerializeField]
	private float openGateDelay = 1f;

	// Token: 0x04003090 RID: 12432
	[SerializeField]
	private bool doesNotActivate;

	// Token: 0x04003091 RID: 12433
	[SerializeField]
	[ModifiableProperty]
	[Conditional("doesNotActivate", true, false, false)]
	private bool setHitBlocked;

	// Token: 0x04003092 RID: 12434
	[SerializeField]
	private bool retractAfterHit;

	// Token: 0x04003093 RID: 12435
	[SerializeField]
	private TrackTriggerObjects canHitTrigger;

	// Token: 0x04003094 RID: 12436
	[SerializeField]
	private NonBouncer nonBouncer;

	// Token: 0x04003095 RID: 12437
	[SerializeField]
	private bool recoilHero;

	// Token: 0x04003096 RID: 12438
	[SerializeField]
	private string sendEventToRegister;

	// Token: 0x04003097 RID: 12439
	[Space]
	[SerializeField]
	[FormerlySerializedAs("connectedGates")]
	private UnlockablePropBase[] unlockables;

	// Token: 0x04003098 RID: 12440
	[SerializeField]
	private PlayMakerFSM[] fsmGates;

	// Token: 0x04003099 RID: 12441
	[Space]
	public UnityEvent OnHit;

	// Token: 0x0400309A RID: 12442
	public UnityEvent OnHitDelayed;

	// Token: 0x0400309B RID: 12443
	public UnityEvent OnActivated;

	// Token: 0x0400309C RID: 12444
	public UnityEvent OnStartActivated;

	// Token: 0x0400309D RID: 12445
	private bool hitBlocked;

	// Token: 0x0400309E RID: 12446
	private bool activated;
}
