using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class HeroBox : DebugDrawColliderRuntimeAdder, CustomPlayerLoop.ILateFixedUpdate
{
	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000CCB RID: 3275 RVA: 0x000392AD File Offset: 0x000374AD
	// (set) Token: 0x06000CCC RID: 3276 RVA: 0x000392B4 File Offset: 0x000374B4
	public static bool Inactive { get; set; }

	// Token: 0x06000CCD RID: 3277 RVA: 0x000392BC File Offset: 0x000374BC
	protected override void Awake()
	{
		CustomPlayerLoop.RegisterSuperLateFixedUpdate(this);
		base.Awake();
		this.box = base.GetComponent<BoxCollider2D>();
		this.heroCtrl = base.GetComponentInParent<HeroController>();
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x000392E2 File Offset: 0x000374E2
	private void OnDestroy()
	{
		CustomPlayerLoop.UnregisterSuperLateFixedUpdate(this);
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x000392EA File Offset: 0x000374EA
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (!HeroBox.Inactive)
		{
			this.CheckForDamage(otherCollider.gameObject);
		}
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x000392FF File Offset: 0x000374FF
	private void OnTriggerStay2D(Collider2D otherCollider)
	{
		if (!HeroBox.Inactive)
		{
			this.CheckForDamage(otherCollider.gameObject);
		}
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x00039314 File Offset: 0x00037514
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Enemy, false);
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x00039324 File Offset: 0x00037524
	public void CheckForDamage(GameObject otherGameObject)
	{
		if (FSMUtility.ContainsFSM(otherGameObject, "damages_hero"))
		{
			PlayMakerFSM fsm = FSMUtility.LocateFSM(otherGameObject, "damages_hero");
			int @int = FSMUtility.GetInt(fsm, "damageDealt");
			HazardType int2 = (HazardType)FSMUtility.GetInt(fsm, "hazardType");
			this.heroCtrl.TakeDamage(otherGameObject, (otherGameObject.transform.position.x > base.transform.position.x) ? CollisionSide.right : CollisionSide.left, @int, int2, DamagePropertyFlags.None);
			return;
		}
		DamageHero component = otherGameObject.GetComponent<DamageHero>();
		this.TakeDamageFromDamager(component, otherGameObject);
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x000393A8 File Offset: 0x000375A8
	public void TakeDamageFromDamager(DamageHero damageHero, GameObject damagingObject)
	{
		if (damageHero == null)
		{
			return;
		}
		if (!damageHero.CanCauseDamage)
		{
			return;
		}
		int num = damageHero.enabled ? damageHero.damageDealt : 0;
		if (num == 0)
		{
			if (damageHero.enabled && damageHero.forceParry && HeroBox.IsHitTypeBuffered(this.hazardType))
			{
				this.hazardType = damageHero.hazardType;
				this.lastDamageHero = damageHero;
				this.lastDamagingObject = damagingObject;
				if (!this.isHitBuffered)
				{
					this.isHitBuffered = true;
					this.damageDealt = 0;
				}
			}
			return;
		}
		if (!this.isHitBuffered || num > this.damageDealt)
		{
			this.damageDealt = num;
		}
		this.hazardType = damageHero.hazardType;
		this.lastDamageHero = damageHero;
		this.lastDamagingObject = damagingObject;
		if (damageHero.OverrideCollisionSide)
		{
			this.collisionSide = damageHero.CollisionSide;
		}
		else
		{
			float num2 = damagingObject.transform.position.x;
			float num3 = base.transform.position.x;
			if (damageHero.InvertCollisionSide)
			{
				float num4 = num3;
				float num5 = num2;
				num2 = num4;
				num3 = num5;
			}
			this.collisionSide = ((num2 > num3) ? CollisionSide.right : CollisionSide.left);
		}
		this.damagePropertyFlags = damageHero.damagePropertyFlags;
		if (!HeroBox.IsHitTypeBuffered(this.hazardType))
		{
			this.ApplyBufferedHit();
			return;
		}
		this.isHitBuffered = true;
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x000394D8 File Offset: 0x000376D8
	private static bool IsHitTypeBuffered(HazardType hazardType)
	{
		return hazardType <= HazardType.ENEMY;
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x000394E1 File Offset: 0x000376E1
	public void LateFixedUpdate()
	{
		if (this.isHitBuffered)
		{
			this.ApplyBufferedHit();
		}
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x000394F4 File Offset: 0x000376F4
	private void ApplyBufferedHit()
	{
		if (this.damageDealt == 0 && this.lastDamageHero != null && this.lastDamageHero.forceParry)
		{
			this.heroCtrl.CheckParry(this.lastDamageHero);
		}
		else
		{
			this.heroCtrl.TakeDamage(this.lastDamagingObject, this.collisionSide, this.damageDealt, this.hazardType, this.damagePropertyFlags);
		}
		this.lastDamageHero = null;
		this.lastDamagingObject = null;
		this.isHitBuffered = false;
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00039575 File Offset: 0x00037775
	public void HeroBoxOff()
	{
		this.box.enabled = false;
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00039584 File Offset: 0x00037784
	public void HeroBoxNormal()
	{
		if (this.heroCtrl.cState.onGround)
		{
			this.box.offset = new Vector2(0f, -0.3799f);
			this.box.size = new Vector2(0.4554f, 2.2498f);
		}
		else
		{
			this.box.offset = new Vector2(0f, -0.14f);
			this.box.size = new Vector2(0.4554f, 1.77f);
		}
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x00039620 File Offset: 0x00037820
	public void HeroBoxDownspike()
	{
		this.box.offset = new Vector2(0f, -0.1f);
		this.box.size = new Vector2(0.4554f, 1.218f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00039674 File Offset: 0x00037874
	public void HeroBoxDownDrill()
	{
		this.box.offset = new Vector2(0f, 0.1f);
		this.box.size = new Vector2(0.4554f, 1.5f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x000396C8 File Offset: 0x000378C8
	public void HeroBoxSprint()
	{
		this.box.offset = new Vector2(0f, -0.5844275f);
		this.box.size = new Vector2(0.4554f, 1.391145f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x0003971C File Offset: 0x0003791C
	public void HeroBoxAirdash()
	{
		this.box.offset = new Vector2(0f, -0.4780059f);
		this.box.size = new Vector2(0.4554f, 1.036407f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x00039770 File Offset: 0x00037970
	public void HeroBoxReaperDownSlash()
	{
		this.box.offset = new Vector2(0.37f, 0.6319103f);
		this.box.size = new Vector2(0.4554f, 1.601192f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x000397C4 File Offset: 0x000379C4
	public void HeroBoxWallSlide()
	{
		this.box.offset = new Vector2(0.2101475f, -0.2755051f);
		this.box.size = new Vector2(0.875695f, 1.280464f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x00039818 File Offset: 0x00037A18
	public void HeroBoxWallScramble()
	{
		this.box.offset = new Vector2(0.2101475f, 0.4f);
		this.box.size = new Vector2(0.875695f, 1.280464f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x0003986C File Offset: 0x00037A6C
	public void HeroBoxWallScuttle()
	{
		this.box.offset = new Vector2(-0.2101475f, -0.4755051f);
		this.box.size = new Vector2(0.875695f, 1.280464f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x000398C0 File Offset: 0x00037AC0
	public void HeroBoxScuttleDash()
	{
		this.box.offset = new Vector2(0.11f, -0.34f);
		this.box.size = new Vector2(1.02f, 0.06f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x00039914 File Offset: 0x00037B14
	public void HeroBoxScuttle()
	{
		this.box.offset = new Vector2(0.11f, -0.81f);
		this.box.size = new Vector2(1.02f, 1f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x00039968 File Offset: 0x00037B68
	public void HeroBoxParryStance()
	{
		this.box.offset = new Vector2(-0.017f, -0.14f);
		this.box.size = new Vector2(1.18f, 1.77f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x000399BC File Offset: 0x00037BBC
	public void HeroBoxHarpoon()
	{
		this.box.offset = new Vector2(0f, -0.33f);
		this.box.size = new Vector2(0.4554f, 1.46f);
		this.box.enabled = true;
		this.AddDebugDrawComponent();
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00039A17 File Offset: 0x00037C17
	bool CustomPlayerLoop.ILateFixedUpdate.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04000C5F RID: 3167
	private HeroController heroCtrl;

	// Token: 0x04000C60 RID: 3168
	private DamageHero lastDamageHero;

	// Token: 0x04000C61 RID: 3169
	private GameObject lastDamagingObject;

	// Token: 0x04000C62 RID: 3170
	private bool isHitBuffered;

	// Token: 0x04000C63 RID: 3171
	private int damageDealt;

	// Token: 0x04000C64 RID: 3172
	private HazardType hazardType;

	// Token: 0x04000C65 RID: 3173
	private CollisionSide collisionSide;

	// Token: 0x04000C66 RID: 3174
	private DamagePropertyFlags damagePropertyFlags;

	// Token: 0x04000C67 RID: 3175
	private BoxCollider2D box;
}
