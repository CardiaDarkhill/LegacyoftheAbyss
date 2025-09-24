using System;
using System.Collections;
using GlobalEnums;
using GlobalSettings;
using UnityEngine;

// Token: 0x020003C1 RID: 961
public class ToolPin : MonoBehaviour, ITinkResponder, IProjectile, IBreakableProjectile
{
	// Token: 0x06002073 RID: 8307 RVA: 0x00094938 File Offset: 0x00092B38
	private void Awake()
	{
		this.damageEnemies = base.GetComponent<DamageEnemies>();
		if (this.damageEnemies != null)
		{
			this.damageEnemies.WillDamageEnemyOptions += this.HitEnemy;
			this.damageEnemies.HitResponded += this.HitEnemy;
		}
		this.faceAngle = base.GetComponent<FaceAngleSimple>();
		this.spriteFlash = base.GetComponent<SpriteFlash>();
		this.poisonTint = Gameplay.PoisonPouchTintColour;
	}

	// Token: 0x06002074 RID: 8308 RVA: 0x000949B0 File Offset: 0x00092BB0
	private void OnEnable()
	{
		this.doBreak = 0;
		this.isBroken = false;
		this.rigidBody.WakeUp();
		this.rigidBody.isKinematic = false;
		this.boxCollider.enabled = true;
		this.thunkEffect.SetActive(false);
		if (this.reboundBox)
		{
			this.reboundBox.SetActive(false);
		}
		this.tinker.SetActive(true);
		this.damageEnemies.doesNotTink = false;
		this.spark.SetActive(false);
		this.slashImpact.SetActive(false);
		this.throwEffect.SetActive(true);
		this.animator.Play(this.flyClip);
		this.breakTimer = 4f;
		this.recycleTimer = 0f;
		this.meshRenderer.enabled = true;
		this.doDeflect = false;
		this.wasDeflected = false;
		this.tinked = false;
		this.rebounded = false;
		this.thunkedCollider = null;
		this.hasThunked = false;
		this.rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
		base.transform.rotation = Quaternion.identity;
		this.hitsLeft = this.hitsToBreak;
		if (this.initialGravityScale == null)
		{
			this.initialGravityScale = new float?(this.rigidBody.gravityScale);
		}
		else
		{
			this.rigidBody.gravityScale = this.initialGravityScale.Value;
		}
		this.rigidBody.linearVelocity = Vector2.zero;
		this.rigidBody.angularVelocity = 0f;
		if (this.faceAngle)
		{
			this.faceAngle.enabled = true;
		}
		if (!this.hasStarted)
		{
			return;
		}
		this.spawnSound.SpawnAndPlayOneShot(base.transform.position, null);
		this.damageEnemies.damageDealt = this.initialDamage;
		this.travelFrames = 0;
		this.CheckPoison();
	}

	// Token: 0x06002075 RID: 8309 RVA: 0x00094B87 File Offset: 0x00092D87
	private void OnDisable()
	{
		this.thunkedCollider = null;
	}

	// Token: 0x06002076 RID: 8310 RVA: 0x00094B90 File Offset: 0x00092D90
	private void Start()
	{
		this.spawnSound.SpawnAndPlayOneShot(base.transform.position, null);
		this.initialDamage = this.damageEnemies.damageDealt;
		this.hasStarted = true;
		this.CheckPoison();
	}

	// Token: 0x06002077 RID: 8311 RVA: 0x00094BC8 File Offset: 0x00092DC8
	private void CheckPoison()
	{
		if (Gameplay.PoisonPouchTool.IsEquipped && !ToolItemManager.IsCustomToolOverride)
		{
			if (this.getTintFrom)
			{
				this.sprite.EnableKeyword("CAN_HUESHIFT");
				this.sprite.SetFloat(PoisonTintBase.HueShiftPropId, this.getTintFrom.PoisonHueShift);
			}
			else
			{
				this.sprite.EnableKeyword("RECOLOUR");
				this.sprite.color = this.poisonTint;
			}
			this.ptShatter.main.startColor = this.poisonTint;
			this.ptPoisonIdle.Play();
			this.isPoison = true;
			return;
		}
		this.sprite.DisableKeyword("CAN_HUESHIFT");
		this.sprite.DisableKeyword("RECOLOUR");
		this.sprite.color = Color.white;
		this.ptShatter.main.startColor = this.ptShatterDefaultColour;
		this.isPoison = false;
	}

	// Token: 0x06002078 RID: 8312 RVA: 0x00094CD0 File Offset: 0x00092ED0
	private void Update()
	{
		if (this.breakTimer > 0f)
		{
			if (this.thunkedCollider && !this.thunkedCollider.isActiveAndEnabled)
			{
				this.breakTimer = 0f;
			}
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

	// Token: 0x06002079 RID: 8313 RVA: 0x00094D6B File Offset: 0x00092F6B
	private void LateUpdate()
	{
		if (this.doDeflect)
		{
			this.Deflect();
			this.doDeflect = false;
		}
	}

	// Token: 0x0600207A RID: 8314 RVA: 0x00094D84 File Offset: 0x00092F84
	private void FixedUpdate()
	{
		if (this.rotateWithVelocity && this.boxCollider.enabled)
		{
			this.VelocityWasSet();
		}
		if (this.travelFrames < 10)
		{
			this.travelFrames++;
		}
		if (this.doBreak > 0)
		{
			this.doBreak--;
			if (this.doBreak <= 0)
			{
				this.DoQueuedBreak();
			}
		}
	}

	// Token: 0x0600207B RID: 8315 RVA: 0x00094DEC File Offset: 0x00092FEC
	public void VelocityWasSet()
	{
		Vector2 normalized = this.rigidBody.linearVelocity.normalized;
		Vector3 a = new Vector3(normalized.y, -normalized.x);
		if (base.transform.localScale.x < 0f)
		{
			a = -a;
		}
		base.transform.rotation = Quaternion.LookRotation(Vector3.forward, -a);
		if (this.damageEnemies)
		{
			float direction = Vector2.Angle(Vector2.right, normalized);
			this.damageEnemies.direction = direction;
		}
	}

	// Token: 0x0600207C RID: 8316 RVA: 0x00094E80 File Offset: 0x00093080
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.recycleTimer > 0f && !this.rebounded)
		{
			return;
		}
		if (!this.wasDeflected && !this.tinked && collision.gameObject.GetComponent<TinkEffect>())
		{
			if (this.piercer)
			{
				BoxCollider2D component = collision.gameObject.GetComponent<BoxCollider2D>();
				if (component != null && !component.isTrigger)
				{
					this.Break();
					return;
				}
				PolygonCollider2D component2 = collision.gameObject.GetComponent<PolygonCollider2D>();
				if (component != null && !component2.isTrigger)
				{
					this.Break();
					return;
				}
			}
			return;
		}
		if (this.travelFrames < 2)
		{
			this.wallHitSound.SpawnAndPlayOneShot(base.transform.position, null);
			this.breakSound.SpawnAndPlayOneShot(base.transform.position, null);
			this.Break();
			return;
		}
		if (collision.contacts.Length != 0 && !this.tinked)
		{
			ContactPoint2D contactPoint2D = collision.contacts[0];
			if (Mathf.Abs(Vector2.Dot(base.transform.right, contactPoint2D.normal)) > 0.05f)
			{
				this.thunkedCollider = collision.collider;
				this.Thunk();
				return;
			}
		}
		this.breakSound.SpawnAndPlayOneShot(base.transform.position, null);
		this.Break();
	}

	// Token: 0x0600207D RID: 8317 RVA: 0x00094FD0 File Offset: 0x000931D0
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Deflector"))
		{
			this.doDeflect = true;
		}
		DamageHero component = collision.gameObject.GetComponent<DamageHero>();
		if (component)
		{
			if (component.hazardType == HazardType.LAVA)
			{
				this.Break();
			}
			if (component.canClashTink)
			{
				this.doDeflect = true;
			}
		}
		if (this.tinked && collision.gameObject.CompareTag("Nail Attack"))
		{
			base.StartCoroutine(this.Rebound(collision.gameObject));
		}
	}

	// Token: 0x0600207E RID: 8318 RVA: 0x00095058 File Offset: 0x00093258
	public void Deflect()
	{
		this.damageEnemies.damageDealt = 0;
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (Random.Range(1, 100) < 51)
		{
			localEulerAngles.z += (float)Random.Range(9, 12);
		}
		else
		{
			localEulerAngles.z += (float)Random.Range(-9, -12);
		}
		base.transform.localEulerAngles = localEulerAngles;
		Rigidbody2D component = base.GetComponent<Rigidbody2D>();
		float num = localEulerAngles.z;
		if (base.transform.localScale.x < 0f)
		{
			num += 180f;
		}
		float magnitude = component.linearVelocity.magnitude;
		float x = magnitude * Mathf.Cos(num * 0.017453292f);
		float y = magnitude * Mathf.Sin(num * 0.017453292f);
		Vector2 linearVelocity = new Vector2(x, y);
		component.linearVelocity = linearVelocity;
		this.tinker.SetActive(false);
		this.damageEnemies.doesNotTink = true;
		this.wasDeflected = true;
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x00095148 File Offset: 0x00093348
	private void Thunk()
	{
		this.rigidBody.isKinematic = true;
		this.rigidBody.linearVelocity = new Vector2(0f, 0f);
		this.boxCollider.enabled = false;
		this.thunkEffect.SetActive(true);
		this.animator.Play(this.thunkClip);
		this.tinker.SetActive(false);
		this.damageEnemies.doesNotTink = true;
		this.tinked = false;
		this.breakTimer = Random.Range(1.8f, 2.2f);
		this.wallHitSound.SpawnAndPlayOneShot(base.transform.position, null);
		this.ptPoisonIdle.Stop();
		if (this.isPoison)
		{
			this.ptPoisonThunk.Play();
		}
		this.ReportEnded();
	}

	// Token: 0x06002080 RID: 8320 RVA: 0x00095214 File Offset: 0x00093414
	public void Break()
	{
		if (this.isBroken)
		{
			return;
		}
		this.doBreak = 0;
		this.isBroken = true;
		this.breakTimer = 0f;
		this.rigidBody.isKinematic = true;
		this.rigidBody.linearVelocity = Vector2.zero;
		this.rigidBody.angularVelocity = 0f;
		this.rigidBody.Sleep();
		if (this.meshRenderer.isVisible)
		{
			this.spark.SetActive(true);
			this.ptShatter.Play();
		}
		this.boxCollider.enabled = false;
		this.meshRenderer.enabled = false;
		this.tinker.SetActive(false);
		this.damageEnemies.doesNotTink = true;
		if (this.reboundBox)
		{
			this.reboundBox.SetActive(false);
		}
		this.tinked = false;
		this.recycleTimer = 1f;
		this.ptPoisonIdle.Stop();
		this.ReportEnded();
	}

	// Token: 0x06002081 RID: 8321 RVA: 0x0009530A File Offset: 0x0009350A
	public void QueueBreak(IBreakableProjectile.HitInfo hitInfo)
	{
		if (this.isBroken)
		{
			return;
		}
		if (this.doBreak > 0)
		{
			return;
		}
		this.doBreak = 2;
		this.hitInfo = hitInfo;
	}

	// Token: 0x06002082 RID: 8322 RVA: 0x0009532D File Offset: 0x0009352D
	private void DoQueuedBreak()
	{
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x0009532F File Offset: 0x0009352F
	private void ReportEnded()
	{
		if (this.hasThunked)
		{
			return;
		}
		this.hasThunked = true;
		EventRegister.SendEvent(EventRegisterEvents.ToolPinThunked, null);
	}

	// Token: 0x06002084 RID: 8324 RVA: 0x0009534C File Offset: 0x0009354C
	private void HitEnemy(HealthManager enemyHealthManager, HitInstance damageInstance)
	{
		if (enemyHealthManager.IsBlockingByDirection(DirectionUtils.GetCardinalDirection(damageInstance.Direction), damageInstance.AttackType, SpecialTypes.None) && !this.piercer)
		{
			this.Tinked();
			return;
		}
		this.HitEnemy();
	}

	// Token: 0x06002085 RID: 8325 RVA: 0x0009537D File Offset: 0x0009357D
	private void HitEnemy(DamageEnemies.HitResponse hitResponse)
	{
		if (hitResponse.Target.gameObject.GetComponent<AttackDetonator>() != null)
		{
			this.HitEnemy();
		}
	}

	// Token: 0x06002086 RID: 8326 RVA: 0x000953A0 File Offset: 0x000935A0
	private void HitEnemy()
	{
		if (this.spriteFlash)
		{
			this.spriteFlash.flashWhiteQuick();
		}
		this.slashImpact.SetActive(true);
		if (this.hitsToBreak <= 0)
		{
			return;
		}
		this.hitsLeft--;
		if (this.hitsLeft > 0)
		{
			return;
		}
		this.breakTimer = 0f;
		this.rigidBody.Sleep();
		this.boxCollider.enabled = false;
		this.meshRenderer.enabled = false;
		this.tinked = false;
		this.recycleTimer = 0.5f;
		this.tinker.SetActive(false);
		this.damageEnemies.doesNotTink = true;
		this.ptPoisonIdle.Stop();
		if (this.isPoison)
		{
			this.ptPoisonThunk.Play();
		}
	}

	// Token: 0x1700035C RID: 860
	// (get) Token: 0x06002087 RID: 8327 RVA: 0x00095469 File Offset: 0x00093669
	public ITinkResponder.TinkFlags ResponderType
	{
		get
		{
			return ITinkResponder.TinkFlags.Projectile;
		}
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x0009546C File Offset: 0x0009366C
	public void Tinked()
	{
		if (!this.piercer)
		{
			if (this.rebounded || this.breaksOnTink)
			{
				this.Break();
				return;
			}
			this.boxCollider.enabled = false;
			this.reboundBox.SetActive(true);
			this.breakTimer = 2f;
			if (this.faceAngle)
			{
				this.faceAngle.enabled = false;
			}
			this.rigidBody.gravityScale = 2.75f;
			float num = Random.Range(13f, 16f);
			float y = Random.Range(28f, 33f);
			float num2 = Random.Range(5f, 8f) * 360f;
			this.rigidBody.constraints = RigidbodyConstraints2D.None;
			if (base.transform.localScale.x > 0f)
			{
				this.rigidBody.linearVelocity = new Vector2(-num, y);
				this.rigidBody.angularVelocity = num2;
			}
			else
			{
				this.rigidBody.linearVelocity = new Vector2(num, y);
				this.rigidBody.angularVelocity = -num2;
			}
			this.tinked = true;
			this.damageEnemies.doesNotTink = true;
			if (!string.IsNullOrEmpty(this.tinkClip))
			{
				this.animator.Play(this.tinkClip);
			}
		}
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x000955B4 File Offset: 0x000937B4
	public void TornadoEffect()
	{
		this.boxCollider.enabled = false;
		if (this.reboundBox)
		{
			this.reboundBox.SetActive(true);
		}
		this.breakTimer = 2f;
		if (this.faceAngle)
		{
			this.faceAngle.enabled = false;
		}
		this.rigidBody.gravityScale = 2.75f;
		float num = Random.Range(13f, 16f);
		float y = Random.Range(28f, 33f);
		float num2 = Random.Range(5f, 8f) * 360f;
		this.rigidBody.constraints = RigidbodyConstraints2D.None;
		if (base.transform.localScale.x > 0f)
		{
			this.rigidBody.linearVelocity = new Vector2(-num, y);
			this.rigidBody.angularVelocity = num2;
		}
		else
		{
			this.rigidBody.linearVelocity = new Vector2(num, y);
			this.rigidBody.angularVelocity = -num2;
		}
		this.tinked = true;
		this.damageEnemies.doesNotTink = true;
		if (!string.IsNullOrEmpty(this.tinkClip))
		{
			this.animator.Play(this.tinkClip);
		}
	}

	// Token: 0x0600208A RID: 8330 RVA: 0x000956E4 File Offset: 0x000938E4
	public IEnumerator Rebound(GameObject attackObject)
	{
		this.tinked = false;
		this.rebounded = true;
		this.boxCollider.enabled = true;
		this.reboundBox.SetActive(false);
		this.breakTimer = 0f;
		if (this.initialGravityScale == null)
		{
			this.initialGravityScale = new float?(this.rigidBody.gravityScale);
		}
		else
		{
			this.rigidBody.gravityScale = this.initialGravityScale.Value;
		}
		this.damageEnemies.damageDealt = 5;
		this.rigidBody.angularVelocity = 0f;
		this.animator.Play(this.flyClip);
		this.breakTimer = 4f;
		this.recycleTimer = 0f;
		this.reboundSound.SpawnAndPlayOneShot(base.transform.position, null);
		this.reboundHitEffectGameobject.Spawn(base.transform.position);
		float launchAngle = attackObject.GetComponent<DamageEnemies>().direction;
		float f = 0.017453292f * launchAngle;
		Vector2 vector = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		vector *= 50f;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		this.rigidBody.linearVelocity = vector;
		if (launchAngle >= 80f && launchAngle <= 100f)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z);
		}
		yield return new WaitForSeconds(0.01f);
		base.transform.localEulerAngles = new Vector3(0f, 0f, launchAngle);
		yield break;
	}

	// Token: 0x04001F86 RID: 8070
	public string flyClip;

	// Token: 0x04001F87 RID: 8071
	public string thunkClip;

	// Token: 0x04001F88 RID: 8072
	public string tinkClip;

	// Token: 0x04001F89 RID: 8073
	public bool breaksOnTink;

	// Token: 0x04001F8A RID: 8074
	public bool piercer;

	// Token: 0x04001F8B RID: 8075
	public tk2dSprite sprite;

	// Token: 0x04001F8C RID: 8076
	public tk2dSpriteAnimator animator;

	// Token: 0x04001F8D RID: 8077
	public BoxCollider2D boxCollider;

	// Token: 0x04001F8E RID: 8078
	public Rigidbody2D rigidBody;

	// Token: 0x04001F8F RID: 8079
	public MeshRenderer meshRenderer;

	// Token: 0x04001F90 RID: 8080
	public GameObject terrainBox;

	// Token: 0x04001F91 RID: 8081
	public GameObject reboundBox;

	// Token: 0x04001F92 RID: 8082
	public GameObject tinker;

	// Token: 0x04001F93 RID: 8083
	public GameObject thunkEffect;

	// Token: 0x04001F94 RID: 8084
	public GameObject spark;

	// Token: 0x04001F95 RID: 8085
	public GameObject slashImpact;

	// Token: 0x04001F96 RID: 8086
	public GameObject throwEffect;

	// Token: 0x04001F97 RID: 8087
	public GameObject reboundHitEffectGameobject;

	// Token: 0x04001F98 RID: 8088
	public ParticleSystem ptShatter;

	// Token: 0x04001F99 RID: 8089
	public ParticleSystem ptSpeedLines;

	// Token: 0x04001F9A RID: 8090
	[SerializeField]
	private AudioEventRandom spawnSound;

	// Token: 0x04001F9B RID: 8091
	[SerializeField]
	private AudioEventRandom wallHitSound;

	// Token: 0x04001F9C RID: 8092
	[SerializeField]
	private AudioEventRandom reboundSound;

	// Token: 0x04001F9D RID: 8093
	[SerializeField]
	private AudioEventRandom breakSound;

	// Token: 0x04001F9E RID: 8094
	[SerializeField]
	private int hitsToBreak = 1;

	// Token: 0x04001F9F RID: 8095
	[SerializeField]
	private bool rotateWithVelocity;

	// Token: 0x04001FA0 RID: 8096
	[Header("Poison")]
	[SerializeField]
	private ToolItem getTintFrom;

	// Token: 0x04001FA1 RID: 8097
	public Color ptShatterDefaultColour = Color.white;

	// Token: 0x04001FA2 RID: 8098
	public ParticleSystem ptPoisonIdle;

	// Token: 0x04001FA3 RID: 8099
	public ParticleSystem ptPoisonThunk;

	// Token: 0x04001FA4 RID: 8100
	private Color poisonTint;

	// Token: 0x04001FA5 RID: 8101
	private float breakTimer;

	// Token: 0x04001FA6 RID: 8102
	private float recycleTimer;

	// Token: 0x04001FA7 RID: 8103
	private int hitsLeft;

	// Token: 0x04001FA8 RID: 8104
	private float? initialGravityScale;

	// Token: 0x04001FA9 RID: 8105
	private bool hasStarted;

	// Token: 0x04001FAA RID: 8106
	private bool doDeflect;

	// Token: 0x04001FAB RID: 8107
	private int doBreak;

	// Token: 0x04001FAC RID: 8108
	private bool wasDeflected;

	// Token: 0x04001FAD RID: 8109
	private bool tinked;

	// Token: 0x04001FAE RID: 8110
	private bool rebounded;

	// Token: 0x04001FAF RID: 8111
	private bool hasThunked;

	// Token: 0x04001FB0 RID: 8112
	private int travelFrames;

	// Token: 0x04001FB1 RID: 8113
	private bool isPoison;

	// Token: 0x04001FB2 RID: 8114
	private bool isBroken;

	// Token: 0x04001FB3 RID: 8115
	private Collider2D thunkedCollider;

	// Token: 0x04001FB4 RID: 8116
	private int initialDamage;

	// Token: 0x04001FB5 RID: 8117
	private DamageEnemies damageEnemies;

	// Token: 0x04001FB6 RID: 8118
	private FaceAngleSimple faceAngle;

	// Token: 0x04001FB7 RID: 8119
	private SpriteFlash spriteFlash;

	// Token: 0x04001FB8 RID: 8120
	private IBreakableProjectile.HitInfo hitInfo;
}
