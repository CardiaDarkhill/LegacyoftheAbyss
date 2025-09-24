using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class NailAttackBase : MonoBehaviour
{
	// Token: 0x1400002D RID: 45
	// (add) Token: 0x06000FA5 RID: 4005 RVA: 0x0004B5F4 File Offset: 0x000497F4
	// (remove) Token: 0x06000FA6 RID: 4006 RVA: 0x0004B62C File Offset: 0x0004982C
	public event Action AttackStarting;

	// Token: 0x1400002E RID: 46
	// (add) Token: 0x06000FA7 RID: 4007 RVA: 0x0004B664 File Offset: 0x00049864
	// (remove) Token: 0x06000FA8 RID: 4008 RVA: 0x0004B69C File Offset: 0x0004989C
	public event Action<bool> EndedDamage;

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06000FA9 RID: 4009 RVA: 0x0004B6D1 File Offset: 0x000498D1
	// (set) Token: 0x06000FAA RID: 4010 RVA: 0x0004B6D9 File Offset: 0x000498D9
	private protected GameObject ExtraDamager { protected get; private set; }

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06000FAB RID: 4011 RVA: 0x0004B6E2 File Offset: 0x000498E2
	public DamageEnemies EnemyDamager
	{
		get
		{
			return this.enemyDamager;
		}
	}

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06000FAC RID: 4012 RVA: 0x0004B6EA File Offset: 0x000498EA
	public bool CanHitSpikes
	{
		get
		{
			return this.canHitSpikes;
		}
	}

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06000FAD RID: 4013 RVA: 0x0004B6F2 File Offset: 0x000498F2
	// (set) Token: 0x06000FAE RID: 4014 RVA: 0x0004B6FA File Offset: 0x000498FA
	public bool IsDamagerActive { get; protected set; }

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x06000FAF RID: 4015 RVA: 0x0004B703 File Offset: 0x00049903
	// (set) Token: 0x06000FB0 RID: 4016 RVA: 0x0004B70B File Offset: 0x0004990B
	public bool CanDamageEnemies { get; protected set; }

	// Token: 0x06000FB1 RID: 4017 RVA: 0x0004B714 File Offset: 0x00049914
	protected virtual void Awake()
	{
		this.hc = base.GetComponentInParent<HeroController>();
		this.slashSprite = base.GetComponent<tk2dSprite>();
		if (this.imbuedSlashAnim)
		{
			this.imbuedSlashMesh = this.imbuedSlashAnim.GetComponent<MeshRenderer>();
			this.imbuedSlashMesh.enabled = false;
			this.imbuedSlashSprite = this.imbuedSlashAnim.GetComponent<tk2dSprite>();
		}
		Transform transform = base.transform.Find("Clash Tink");
		if (transform)
		{
			this.clashTinkPoly = transform.GetComponent<PolygonCollider2D>();
		}
		if (this.clashTinkPoly)
		{
			this.clashTinkPoly.enabled = false;
		}
		this.damagers = new List<DamageEnemies>
		{
			this.enemyDamager
		};
		Transform transform2 = base.transform.Find("Extra Damager");
		if (transform2)
		{
			this.ExtraDamager = transform2.gameObject;
			DamageEnemies component = this.ExtraDamager.GetComponent<DamageEnemies>();
			this.damagers.Add(component);
		}
		foreach (DamageEnemies damageEnemies in this.damagers)
		{
			if (damageEnemies)
			{
				if (!damageEnemies.manualTrigger)
				{
					damageEnemies.EndedDamage += this.OnEndedDamage;
				}
				foreach (DamageEnemies damageEnemies2 in this.damagers)
				{
					if (!(damageEnemies == damageEnemies2))
					{
						DamageEnemies dmg = damageEnemies;
						damageEnemies2.WillDamageEnemyCollider += delegate(Collider2D col)
						{
							dmg.PreventDamage(col);
						};
					}
				}
			}
		}
		this.CanDamageEnemies = true;
		this.IsDamagerActive = true;
		this.activateOnSlash.SetAllActive(false);
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x0004B8F8 File Offset: 0x00049AF8
	public virtual void OnSlashStarting()
	{
		NailImbuementConfig currentImbuement = this.hc.NailImbuement.CurrentImbuement;
		if (currentImbuement != null)
		{
			this.SetNailImbuement(currentImbuement, this.hc.NailImbuement.CurrentElement);
		}
		Action attackStarting = this.AttackStarting;
		if (attackStarting != null)
		{
			attackStarting();
		}
		if (this.ExtraDamager)
		{
			this.ExtraDamager.SetActive(false);
		}
		this.activateOnSlash.SetAllActive(true);
		this.PlayVibration();
		if (!Gameplay.LongNeedleTool.IsEquipped)
		{
			base.transform.localScale = this.scale;
			return;
		}
		if (this.overrideLongNeedleScale)
		{
			base.transform.localScale = this.longNeedleScale;
			return;
		}
		Vector2 longNeedleMultiplier = Gameplay.LongNeedleMultiplier;
		Vector2 vector = this.hc.cState.upAttacking ? new Vector2(longNeedleMultiplier.y, longNeedleMultiplier.x) : longNeedleMultiplier;
		base.transform.localScale = new Vector3(this.scale.x * vector.x, this.scale.y * vector.y, this.scale.z);
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x0004BA19 File Offset: 0x00049C19
	public void SetLongNeedleHandled()
	{
		this.overrideLongNeedleScale = true;
		this.longNeedleScale = this.scale;
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x0004BA30 File Offset: 0x00049C30
	public void OnCancelAttack()
	{
		foreach (DamageEnemies damageEnemies in this.damagers)
		{
			if (damageEnemies)
			{
				damageEnemies.EndDamage();
				damageEnemies.NailElement = NailElements.None;
				damageEnemies.NailImbuement = null;
			}
		}
		this.OnAttackCancelled();
		if (this.imbuedSlashAnim)
		{
			this.imbuedSlashMesh.enabled = false;
		}
		if (this.slashSprite)
		{
			this.slashSprite.color = Color.white;
		}
		this.isNailImbued = false;
		this.nailImbuement = null;
		if (this.ExtraDamager)
		{
			this.ExtraDamager.SetActive(false);
		}
		if (this.cancelVibrationOnExit)
		{
			this.StopVibration();
		}
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x0004BB0C File Offset: 0x00049D0C
	protected virtual void OnAttackCancelled()
	{
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x0004BB10 File Offset: 0x00049D10
	public void FullCancelAttack()
	{
		foreach (DamageEnemies damageEnemies in this.damagers)
		{
			if (damageEnemies && damageEnemies)
			{
				HealthManager.CancelAllLagHitsForSource(damageEnemies.gameObject);
			}
		}
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x0004BB78 File Offset: 0x00049D78
	public void OnPlaySlash()
	{
		foreach (DamageEnemies damageEnemies in this.damagers)
		{
			if (!damageEnemies)
			{
				return;
			}
			damageEnemies.CopyDamagePropsFrom(this.enemyDamager);
			damageEnemies.StartDamage();
		}
		if (this.isNailImbued)
		{
			GameObject slashEffect = this.nailImbuement.SlashEffect;
			if (slashEffect)
			{
				Transform transform = slashEffect.Spawn(base.transform, Vector3.zero, Quaternion.identity).transform;
				transform.localScale = Vector3.one;
				int cardinalDirection = DirectionUtils.GetCardinalDirection(this.enemyDamager.direction);
				if (cardinalDirection != 1)
				{
					if (cardinalDirection == 3)
					{
						transform.localEulerAngles = new Vector3(0f, 0f, 45f);
					}
				}
				else
				{
					transform.localEulerAngles = new Vector3(0f, 0f, -90f);
				}
			}
			this.nailImbuement.ExtraSlashAudio.SpawnAndPlayOneShot(base.transform.position, null);
		}
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x0004BC9C File Offset: 0x00049E9C
	public void SetNailImbuement(NailImbuementConfig config, NailElements element)
	{
		this.isNailImbued = true;
		if (this.slashSprite)
		{
			this.slashSprite.color = config.NailTintColor;
		}
		if (this.imbuedSlashAnim)
		{
			this.imbuedSlashSprite.color = config.NailTintColor;
		}
		foreach (DamageEnemies damageEnemies in this.damagers)
		{
			if (damageEnemies)
			{
				damageEnemies.NailElement = element;
				damageEnemies.NailImbuement = config;
			}
		}
		this.nailImbuement = config;
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x0004BD48 File Offset: 0x00049F48
	private void OnEndedDamage(bool didHit)
	{
		this.IsDamagerActive = false;
		if (this.EndedDamage != null)
		{
			this.EndedDamage(didHit);
		}
		this.hc.SilkTauntEffectConsume();
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x0004BD71 File Offset: 0x00049F71
	public virtual void QueueBounce()
	{
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x0004BD74 File Offset: 0x00049F74
	protected void PlayVibration()
	{
		if (this.vibration)
		{
			this.emission = VibrationManager.PlayVibrationClipOneShot(this.vibration.VibrationData, null, false, "", false);
		}
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x0004BDB4 File Offset: 0x00049FB4
	protected void StopVibration()
	{
		if (this.emission != null)
		{
			this.emission.Stop();
			this.emission = null;
		}
	}

	// Token: 0x04000F3F RID: 3903
	[SerializeField]
	protected DamageEnemies enemyDamager;

	// Token: 0x04000F40 RID: 3904
	[SerializeField]
	private string imbuedSlashAnimName;

	// Token: 0x04000F41 RID: 3905
	[SerializeField]
	private tk2dSpriteAnimator imbuedSlashAnim;

	// Token: 0x04000F42 RID: 3906
	[SerializeField]
	private bool canHitSpikes = true;

	// Token: 0x04000F43 RID: 3907
	[SerializeField]
	private VibrationDataAsset vibration;

	// Token: 0x04000F44 RID: 3908
	[SerializeField]
	protected bool cancelVibrationOnExit;

	// Token: 0x04000F45 RID: 3909
	[SerializeField]
	private GameObject[] activateOnSlash;

	// Token: 0x04000F46 RID: 3910
	[Space]
	[SerializeField]
	private Vector3 scale = Vector3.one;

	// Token: 0x04000F47 RID: 3911
	[SerializeField]
	private bool overrideLongNeedleScale;

	// Token: 0x04000F48 RID: 3912
	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideLongNeedleScale", true, false, false)]
	private Vector3 longNeedleScale = Vector3.one;

	// Token: 0x04000F49 RID: 3913
	protected HeroController hc;

	// Token: 0x04000F4A RID: 3914
	private tk2dSprite slashSprite;

	// Token: 0x04000F4B RID: 3915
	private MeshRenderer imbuedSlashMesh;

	// Token: 0x04000F4C RID: 3916
	private tk2dSprite imbuedSlashSprite;

	// Token: 0x04000F4D RID: 3917
	private bool isNailImbued;

	// Token: 0x04000F4E RID: 3918
	private NailImbuementConfig nailImbuement;

	// Token: 0x04000F4F RID: 3919
	private List<DamageEnemies> damagers;

	// Token: 0x04000F50 RID: 3920
	private VibrationEmission emission;

	// Token: 0x04000F51 RID: 3921
	protected PolygonCollider2D clashTinkPoly;
}
