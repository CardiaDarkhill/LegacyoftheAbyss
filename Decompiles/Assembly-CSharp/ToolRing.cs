using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003C2 RID: 962
public class ToolRing : MonoBehaviour
{
	// Token: 0x0600208C RID: 8332 RVA: 0x00095714 File Offset: 0x00093914
	private void OnValidate()
	{
		if (this.hitsToBreak < 1)
		{
			this.hitsToBreak = 1;
		}
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x00095728 File Offset: 0x00093928
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.damageEnemies.DamagedEnemyGameObject += this.OnDamagedEnemy;
		if (this.inertCog)
		{
			Transform transform = this.inertCog.transform;
			this.inertCogInitialPos = transform.localPosition;
			this.inertCogInitialRot = transform.localRotation;
			this.inertCog.FallenCleanup += this.OnInertCogCleanup;
		}
		this.poisonTint = Gameplay.PoisonPouchTintColour;
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x000957AB File Offset: 0x000939AB
	private void Start()
	{
		this.CheckPoison();
	}

	// Token: 0x0600208F RID: 8335 RVA: 0x000957B4 File Offset: 0x000939B4
	private void OnEnable()
	{
		this.hitsLeft = this.hitsToBreak;
		this.hitEnemies.Clear();
		this.hitEnemies.Add(base.gameObject);
		this.sprite.enabled = true;
		this.inactiveOnBreak.SetAllActive(true);
		this.breakEffects.SetActive(false);
		this.breakTimer = 0.7f;
		this.recycleTimer = 0f;
		this.inertCog.gameObject.SetActive(false);
		Transform transform = this.inertCog.transform;
		transform.localPosition = this.inertCogInitialPos;
		transform.localRotation = this.inertCogInitialRot;
		HeroController silentInstance = HeroController.SilentInstance;
		if (silentInstance && silentInstance.RingTauntEffectConsume())
		{
			this.damageEnemies.DamageMultiplier = 1.5f;
			if (this.spriteFlash)
			{
				this.spriteFlash.flashWhiteQuick();
			}
		}
		else
		{
			this.damageEnemies.DamageMultiplier = 1f;
		}
		this.CheckPoison();
	}

	// Token: 0x06002090 RID: 8336 RVA: 0x000958B0 File Offset: 0x00093AB0
	private void CheckPoison()
	{
		if (Gameplay.PoisonPouchTool.IsEquipped && !ToolItemManager.IsCustomToolOverride)
		{
			if (this.materialState != ToolRing.MaterialState.Poison)
			{
				this.materialState = ToolRing.MaterialState.Poison;
				this.SetMaterialPoison(this.sprite.material);
				this.SetMaterialPoison(this.flattenSprite.material);
				if (this.fallenSprite)
				{
					this.SetMaterialPoison(this.fallenSprite.material);
				}
				this.ptRingTrail.main.startColor = this.poisonTint;
				this.ptShatter.main.startColor = this.poisonTint;
			}
			this.ptPoisonIdle.Play();
			return;
		}
		if (this.materialState != ToolRing.MaterialState.Normal)
		{
			this.materialState = ToolRing.MaterialState.Normal;
			this.SetMaterialNormal(this.sprite.material);
			this.SetMaterialNormal(this.flattenSprite.material);
			if (this.fallenSprite)
			{
				this.SetMaterialNormal(this.fallenSprite.material);
			}
			this.ptRingTrail.main.startColor = this.ptRingTrailDefaultColour;
			this.ptShatter.main.startColor = this.ptShatterDefaultColour;
		}
	}

	// Token: 0x06002091 RID: 8337 RVA: 0x00095A00 File Offset: 0x00093C00
	private void SetMaterialNormal(Material material)
	{
		material.DisableKeyword("CAN_HUESHIFT");
		material.DisableKeyword("RECOLOUR");
		material.color = Color.white;
	}

	// Token: 0x06002092 RID: 8338 RVA: 0x00095A24 File Offset: 0x00093C24
	private void SetMaterialPoison(Material material)
	{
		if (this.getTintFrom)
		{
			material.EnableKeyword("CAN_HUESHIFT");
			material.SetFloat(PoisonTintBase.HueShiftPropId, this.getTintFrom.PoisonHueShift);
			return;
		}
		material.EnableKeyword("RECOLOUR");
		material.color = this.poisonTint;
	}

	// Token: 0x06002093 RID: 8339 RVA: 0x00095A77 File Offset: 0x00093C77
	private void OnDisable()
	{
		this.cleanupQueue.Remove(this);
		this.hitEnemies.Clear();
	}

	// Token: 0x06002094 RID: 8340 RVA: 0x00095A94 File Offset: 0x00093C94
	private void Update()
	{
		if (this.breakTimer > 0f)
		{
			this.breakTimer -= Time.deltaTime;
			if (this.breakTimer <= 0f)
			{
				this.Break();
			}
		}
		if (this.recycleTimer > 0f)
		{
			this.recycleTimer -= Time.deltaTime;
			if (this.recycleTimer <= 0f)
			{
				base.gameObject.Recycle();
			}
		}
	}

	// Token: 0x06002095 RID: 8341 RVA: 0x00095B0C File Offset: 0x00093D0C
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.hitsLeft <= 0)
		{
			return;
		}
		this.hitsLeft--;
		Collision2DUtils.Collision2DSafeContact safeContact = collision.GetSafeContact();
		if (Random.Range(0f, 1f) < 0.15f && this.inertCog)
		{
			if (this.terrainHitPrefab)
			{
				this.terrainHitPrefab.Spawn(safeContact.Point);
			}
			this.DoCogRoll(safeContact);
			return;
		}
		if (this.hitsLeft <= 0)
		{
			this.Break();
			return;
		}
		this.breakTimer = 0.7f;
		if (this.terrainHitPrefab)
		{
			this.terrainHitPrefab.Spawn(safeContact.Point);
		}
		if (!this.TryBounceTowardsEnemy())
		{
			Vector2 a = (Vector2.Reflect(-collision.relativeVelocity, safeContact.Normal).DirectionToAngle() + Random.Range(-20f, 20f)).AngleToDirection();
			this.body.linearVelocity = a * this.speed;
			this.lastBounceFrame = CustomPlayerLoop.FixedUpdateCycle;
		}
	}

	// Token: 0x06002096 RID: 8342 RVA: 0x00095C21 File Offset: 0x00093E21
	private void OnInertCogCleanup()
	{
		base.gameObject.Recycle();
	}

	// Token: 0x06002097 RID: 8343 RVA: 0x00095C30 File Offset: 0x00093E30
	private void OnDamagedEnemy(GameObject enemy)
	{
		if (this.hitsLeft <= 0)
		{
			return;
		}
		this.hitsLeft--;
		if (this.hitsLeft <= 0)
		{
			this.Break();
			return;
		}
		this.breakTimer = 0.7f;
		this.hitEnemies.Add(enemy);
		if (!this.TryBounceTowardsEnemy())
		{
			Vector2 linearVelocity = (this.body.linearVelocity.DirectionToAngle() + Random.Range(150f, 220f)).AngleToDirection() * this.speed;
			this.body.linearVelocity = linearVelocity;
			this.lastBounceFrame = CustomPlayerLoop.FixedUpdateCycle;
		}
	}

	// Token: 0x06002098 RID: 8344 RVA: 0x00095CD0 File Offset: 0x00093ED0
	private bool TryBounceTowardsEnemy()
	{
		GameObject closestInsideLineOfSight = this.enemyRange.GetClosestInsideLineOfSight(base.transform.position, this.hitEnemies);
		if (!closestInsideLineOfSight)
		{
			return false;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = closestInsideLineOfSight.transform.position;
		float y = position2.y - position.y;
		float x = position2.x - position.x;
		Vector2 linearVelocity = (Mathf.Atan2(y, x) * 57.295776f + Random.Range(-10f, 10f)).AngleToDirection() * this.speed;
		this.body.linearVelocity = linearVelocity;
		this.lastBounceFrame = CustomPlayerLoop.FixedUpdateCycle;
		return true;
	}

	// Token: 0x06002099 RID: 8345 RVA: 0x00095D84 File Offset: 0x00093F84
	public void TinkBounce()
	{
		if (this.lastBounceFrame == CustomPlayerLoop.FixedUpdateCycle)
		{
			return;
		}
		this.lastBounceFrame = CustomPlayerLoop.FixedUpdateCycle;
		Vector2 linearVelocity = (this.body.linearVelocity.DirectionToAngle() + Random.Range(150f, 220f)).AngleToDirection() * this.speed;
		this.body.linearVelocity = linearVelocity;
	}

	// Token: 0x0600209A RID: 8346 RVA: 0x00095DE7 File Offset: 0x00093FE7
	public void Break()
	{
		this.Stop();
		this.breakEffects.SetActive(true);
		this.recycleTimer = 2f;
	}

	// Token: 0x0600209B RID: 8347 RVA: 0x00095E08 File Offset: 0x00094008
	private void Stop()
	{
		this.body.linearVelocity = Vector2.zero;
		this.sprite.enabled = false;
		this.inactiveOnBreak.SetAllActive(false);
		this.hitsLeft = 0;
		this.breakTimer = 0f;
		this.OnStop.Invoke();
	}

	// Token: 0x0600209C RID: 8348 RVA: 0x00095E5C File Offset: 0x0009405C
	private void DoCogRoll(Collision2DUtils.Collision2DSafeContact contact)
	{
		this.Stop();
		this.recycleTimer = 0f;
		this.inertCog.gameObject.SetActive(true);
		Rigidbody2D component = this.inertCog.GetComponent<Rigidbody2D>();
		float num = Random.Range(3f, 8f);
		component.linearVelocity = new Vector2((contact.Normal.x > 0f) ? num : (-num), 10f);
		this.cleanupQueue.Add(this);
		if (this.cleanupQueue.Count > 3)
		{
			ToolRing toolRing = this.cleanupQueue[0];
			this.cleanupQueue.RemoveAt(0);
			toolRing.inertCog.MarkForCleanup();
		}
	}

	// Token: 0x04001FB9 RID: 8121
	[SerializeField]
	private SpriteRenderer sprite;

	// Token: 0x04001FBA RID: 8122
	[SerializeField]
	private SpriteFlash spriteFlash;

	// Token: 0x04001FBB RID: 8123
	[SerializeField]
	private DamageEnemies damageEnemies;

	// Token: 0x04001FBC RID: 8124
	[SerializeField]
	private TrackTriggerObjects enemyRange;

	// Token: 0x04001FBD RID: 8125
	[SerializeField]
	private GameObject breakEffects;

	// Token: 0x04001FBE RID: 8126
	[SerializeField]
	private GameObject terrainHitPrefab;

	// Token: 0x04001FBF RID: 8127
	[SerializeField]
	private float speed;

	// Token: 0x04001FC0 RID: 8128
	[SerializeField]
	private int hitsToBreak;

	// Token: 0x04001FC1 RID: 8129
	[SerializeField]
	private GameObject[] inactiveOnBreak;

	// Token: 0x04001FC2 RID: 8130
	[SerializeField]
	private CogRollThenFallOver inertCog;

	// Token: 0x04001FC3 RID: 8131
	[Header("Poison")]
	[SerializeField]
	private ToolItem getTintFrom;

	// Token: 0x04001FC4 RID: 8132
	public ParticleSystem ptPoisonIdle;

	// Token: 0x04001FC5 RID: 8133
	public SpriteRenderer flattenSprite;

	// Token: 0x04001FC6 RID: 8134
	[SerializeField]
	private SpriteRenderer fallenSprite;

	// Token: 0x04001FC7 RID: 8135
	public ParticleSystem ptRingTrail;

	// Token: 0x04001FC8 RID: 8136
	public Color ptRingTrailDefaultColour;

	// Token: 0x04001FC9 RID: 8137
	public ParticleSystem ptShatter;

	// Token: 0x04001FCA RID: 8138
	public Color ptShatterDefaultColour;

	// Token: 0x04001FCB RID: 8139
	[Space]
	[SerializeField]
	private UnityEvent OnStop;

	// Token: 0x04001FCC RID: 8140
	private Color poisonTint;

	// Token: 0x04001FCD RID: 8141
	private float breakTimer;

	// Token: 0x04001FCE RID: 8142
	private float recycleTimer;

	// Token: 0x04001FCF RID: 8143
	private Vector3 inertCogInitialPos;

	// Token: 0x04001FD0 RID: 8144
	private Quaternion inertCogInitialRot;

	// Token: 0x04001FD1 RID: 8145
	private Rigidbody2D body;

	// Token: 0x04001FD2 RID: 8146
	private int hitsLeft;

	// Token: 0x04001FD3 RID: 8147
	private readonly HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

	// Token: 0x04001FD4 RID: 8148
	private readonly List<ToolRing> cleanupQueue = new List<ToolRing>();

	// Token: 0x04001FD5 RID: 8149
	private int lastBounceFrame;

	// Token: 0x04001FD6 RID: 8150
	private ToolRing.MaterialState materialState;

	// Token: 0x0200167A RID: 5754
	private enum MaterialState
	{
		// Token: 0x04008AFC RID: 35580
		None,
		// Token: 0x04008AFD RID: 35581
		Normal,
		// Token: 0x04008AFE RID: 35582
		Poison
	}
}
