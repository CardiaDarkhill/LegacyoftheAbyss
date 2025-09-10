using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000512 RID: 1298
public class Lever_tk2d : MonoBehaviour, IHitResponder
{
	// Token: 0x06002E51 RID: 11857 RVA: 0x000CB778 File Offset: 0x000C9978
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.source = base.GetComponent<AudioSource>();
		if (this.source)
		{
			this.sourceVibrationPlayer = this.source.GetComponent<VibrationPlayer>();
		}
		if (this.activatedTinker)
		{
			this.activatedTinker.SetActive(false);
		}
	}

	// Token: 0x06002E52 RID: 11858 RVA: 0x000CB7D4 File Offset: 0x000C99D4
	private void Start()
	{
		PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
		if (component)
		{
			component.OnGetSaveState += delegate(out bool value)
			{
				value = this.activated;
			};
			component.OnSetSaveState += delegate(bool value)
			{
				this.activated = value;
				if (this.activated)
				{
					this.animator.Play("Activated");
					foreach (UnlockablePropBase unlockablePropBase in this.connectedGates)
					{
						if (unlockablePropBase)
						{
							unlockablePropBase.Opened();
						}
					}
					foreach (PlayMakerFSM playMakerFSM in this.fsmGates)
					{
						if (playMakerFSM)
						{
							try
							{
								playMakerFSM.SendEvent(this.fsmActivateEvent);
							}
							catch (Exception exception)
							{
								Debug.LogException(exception);
							}
						}
					}
					foreach (GameObject gameObject in this.camLocks)
					{
						if (gameObject)
						{
							gameObject.SetActive(false);
						}
					}
					this.CustomGateActivated.Invoke();
					if (this.activatedTinker)
					{
						this.activatedTinker.SetActive(true);
					}
				}
			};
		}
		if (this.activeCollider)
		{
			this.activeCollider.enabled = true;
		}
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x000CB830 File Offset: 0x000C9A30
	private bool? IsFsmEventValidRequiredFSM(PlayMakerFSM fsm)
	{
		if (fsm != null)
		{
			return fsm.IsEventValid(this.fsmActivateEvent, true);
		}
		return null;
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x000CB860 File Offset: 0x000C9A60
	private bool? IsFsmEventValidRequired(string eventName)
	{
		if (this.fsmGates != null)
		{
			bool value = false;
			foreach (PlayMakerFSM playMakerFSM in this.fsmGates)
			{
				if (!(playMakerFSM == null))
				{
					bool? flag = playMakerFSM.IsEventValid(eventName, true);
					bool flag2 = false;
					if (flag.GetValueOrDefault() == flag2 & flag != null)
					{
						return new bool?(false);
					}
					value = true;
				}
			}
			return new bool?(value);
		}
		return null;
	}

	// Token: 0x06002E55 RID: 11861 RVA: 0x000CB8D8 File Offset: 0x000C9AD8
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		bool flag = false;
		if (!damageInstance.IsNailDamage)
		{
			return IHitResponder.Response.None;
		}
		if (this.canHitTrigger && !this.canHitTrigger.IsInside)
		{
			return IHitResponder.Response.None;
		}
		if (!this.activated)
		{
			this.activated = true;
			flag = true;
			if (!string.IsNullOrEmpty(this.setPlayerDataBool))
			{
				GameManager.instance.playerData.SetBool(this.setPlayerDataBool, true);
			}
			if (this.activeCollider)
			{
				this.activeCollider.enabled = false;
			}
			base.StartCoroutine(this.Execute());
			if (this.hitSoundOnActivate)
			{
				this.PlaySoundOneShot(this.hitSound);
				if (this.hitVibration != null)
				{
					VibrationManager.PlayVibrationClipOneShot(this.hitVibration, null, false, "", false);
				}
			}
		}
		if (flag)
		{
			if (this.hitEffect)
			{
				this.hitEffect.Spawn(this.activeCollider.bounds.center);
			}
			if (this.effects)
			{
				this.effects.SetActive(true);
			}
			this.PlaySound();
			GameCameras instance = GameCameras.instance;
			if (instance)
			{
				instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			}
			GameManager.instance.FreezeMoment(1);
		}
		return flag ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}

	// Token: 0x06002E56 RID: 11862 RVA: 0x000CBA3B File Offset: 0x000C9C3B
	private IEnumerator Execute()
	{
		this.animator.Play("Hit");
		this.BeforeOpenDelay.Invoke();
		yield return new WaitForSeconds(this.openGateDelay);
		if (this.activatedTinker)
		{
			this.activatedTinker.SetActive(true);
		}
		foreach (UnlockablePropBase unlockablePropBase in this.connectedGates)
		{
			if (unlockablePropBase)
			{
				unlockablePropBase.Open();
			}
		}
		foreach (PlayMakerFSM playMakerFSM in this.fsmGates)
		{
			if (playMakerFSM)
			{
				try
				{
					playMakerFSM.SendEvent("OPEN");
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
		foreach (GameObject gameObject in this.camLocks)
		{
			if (gameObject)
			{
				gameObject.SetActive(false);
			}
		}
		if (!string.IsNullOrEmpty(this.sendEventToRegister))
		{
			EventRegister.SendEvent(this.sendEventToRegister, null);
		}
		this.CustomGateOpen.Invoke();
		yield break;
	}

	// Token: 0x06002E57 RID: 11863 RVA: 0x000CBA4A File Offset: 0x000C9C4A
	public void PlaySound()
	{
		if (this.source)
		{
			this.source.Play();
			if (this.sourceVibrationPlayer)
			{
				this.sourceVibrationPlayer.Play();
			}
		}
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x000CBA7C File Offset: 0x000C9C7C
	public void PlaySoundOneShot(AudioClip clip)
	{
		if (this.source && clip)
		{
			this.source.PlayOneShot(clip);
		}
	}

	// Token: 0x0400309F RID: 12447
	[SerializeField]
	private UnlockablePropBase[] connectedGates;

	// Token: 0x040030A0 RID: 12448
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidRequiredFSM")]
	private PlayMakerFSM[] fsmGates;

	// Token: 0x040030A1 RID: 12449
	[SerializeField]
	private GameObject[] camLocks;

	// Token: 0x040030A2 RID: 12450
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidRequired")]
	private string fsmActivateEvent = "ACTIVATE";

	// Token: 0x040030A3 RID: 12451
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string setPlayerDataBool;

	// Token: 0x040030A4 RID: 12452
	[SerializeField]
	private string sendEventToRegister;

	// Token: 0x040030A5 RID: 12453
	[Space]
	[SerializeField]
	private GameObject hitEffect;

	// Token: 0x040030A6 RID: 12454
	[Space]
	[SerializeField]
	private float openGateDelay = 1f;

	// Token: 0x040030A7 RID: 12455
	[Space]
	[SerializeField]
	private Collider2D activeCollider;

	// Token: 0x040030A8 RID: 12456
	[SerializeField]
	private GameObject effects;

	// Token: 0x040030A9 RID: 12457
	[SerializeField]
	private GameObject activatedTinker;

	// Token: 0x040030AA RID: 12458
	[Space]
	[SerializeField]
	private AudioClip hitSound;

	// Token: 0x040030AB RID: 12459
	[SerializeField]
	private VibrationDataAsset hitVibration;

	// Token: 0x040030AC RID: 12460
	[SerializeField]
	private bool hitSoundOnActivate;

	// Token: 0x040030AD RID: 12461
	[Space]
	[SerializeField]
	private bool checkHeroRange;

	// Token: 0x040030AE RID: 12462
	[SerializeField]
	private float xMin;

	// Token: 0x040030AF RID: 12463
	[SerializeField]
	private float xMax = 9999f;

	// Token: 0x040030B0 RID: 12464
	[SerializeField]
	private float yMin;

	// Token: 0x040030B1 RID: 12465
	[SerializeField]
	private float yMax = 9999f;

	// Token: 0x040030B2 RID: 12466
	[SerializeField]
	private TrackTriggerObjects canHitTrigger;

	// Token: 0x040030B3 RID: 12467
	[Space]
	public UnityEvent BeforeOpenDelay;

	// Token: 0x040030B4 RID: 12468
	public UnityEvent CustomGateOpen;

	// Token: 0x040030B5 RID: 12469
	public UnityEvent CustomGateActivated;

	// Token: 0x040030B6 RID: 12470
	private AudioSource source;

	// Token: 0x040030B7 RID: 12471
	private tk2dSpriteAnimator animator;

	// Token: 0x040030B8 RID: 12472
	private VibrationPlayer sourceVibrationPlayer;

	// Token: 0x040030B9 RID: 12473
	private bool activated;
}
