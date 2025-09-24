using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200054A RID: 1354
public class ScuttlerControl : MonoBehaviour, IHitResponder
{
	// Token: 0x06003065 RID: 12389 RVA: 0x000D5A89 File Offset: 0x000D3C89
	private void Awake()
	{
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06003066 RID: 12390 RVA: 0x000D5AB0 File Offset: 0x000D3CB0
	private void Start()
	{
		base.transform.SetScaleMatching(Random.Range(1.35f, 1.5f));
		this.maxSpeed = Random.Range(6f, 9f);
		this.hero = HeroController.instance.transform;
		this.activateTime = Time.timeAsDouble + (double)this.activateDelay;
		Collider2D component = base.GetComponent<Collider2D>();
		if (component)
		{
			this.rayLength = component.bounds.size.x / 2f + 0.1f;
			this.rayOrigin = component.bounds.center - base.transform.position;
		}
		this.source.enabled = false;
		if (this.healthScuttler)
		{
			this.reverseRun = GameManager.instance.playerData.GetBool("equippedCharm_27");
		}
		if (!this.startRunning && !this.startIdle)
		{
			CollisionEnterEvent componentInChildren = base.GetComponentInChildren<CollisionEnterEvent>();
			if (componentInChildren)
			{
				componentInChildren.CollisionEnteredDirectional += delegate(CollisionEnterEvent.Direction direction, Collision2D collision)
				{
					if (!this.landed && direction == CollisionEnterEvent.Direction.Bottom)
					{
						this.landed = true;
						base.StartCoroutine(this.Land());
					}
				};
				return;
			}
		}
		else
		{
			if (this.startIdle && this.heroAlert)
			{
				this.heroAlert.OnTriggerEntered += delegate(Collider2D collider, GameObject sender)
				{
					if (!this.landed && collider.tag == "Player")
					{
						this.landed = true;
						base.StartCoroutine(this.Land());
					}
				};
				return;
			}
			if (this.startRunning)
			{
				this.runRoutine = base.StartCoroutine(this.Run());
			}
		}
	}

	// Token: 0x06003067 RID: 12391 RVA: 0x000D5C14 File Offset: 0x000D3E14
	private void Update()
	{
		if (!this.alive)
		{
			return;
		}
		if (Helper.Raycast2D(base.transform.position + this.rayOrigin, new Vector2(Mathf.Sign(this.body.linearVelocity.x), 0f), this.rayLength, 256).collider != null && this.bounceRoutine == null && this.runRoutine != null)
		{
			base.StopCoroutine(this.runRoutine);
			this.bounceRoutine = base.StartCoroutine((this.body.linearVelocity.x > 0f) ? this.Bounce(110f, 130f) : this.Bounce(50f, 70f));
		}
	}

	// Token: 0x06003068 RID: 12392 RVA: 0x000D5CE5 File Offset: 0x000D3EE5
	private IEnumerator Land()
	{
		yield return base.StartCoroutine(this.anim.PlayAnimWait(this.landAnim, null));
		this.source.enabled = true;
		this.runRoutine = base.StartCoroutine(this.Run());
		yield break;
	}

	// Token: 0x06003069 RID: 12393 RVA: 0x000D5CF4 File Offset: 0x000D3EF4
	private IEnumerator Run()
	{
		this.anim.Play(this.runAnim);
		this.source.enabled = true;
		Vector3 velocity = this.body.linearVelocity;
		for (;;)
		{
			float num = Mathf.Sign(this.hero.position.x - base.transform.position.x) * (float)(this.reverseRun ? -1 : 1);
			float currentDirection = num;
			base.transform.SetScaleX(Mathf.Abs(base.transform.localScale.x) * num);
			while (currentDirection == num)
			{
				velocity.x += this.acceleration * -num;
				velocity.x = Mathf.Clamp(velocity.x, -this.maxSpeed, this.maxSpeed);
				velocity.y = this.body.linearVelocity.y;
				this.body.linearVelocity = velocity;
				yield return null;
				num = Mathf.Sign(this.hero.position.x - base.transform.position.x) * (float)(this.reverseRun ? -1 : 1);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600306A RID: 12394 RVA: 0x000D5D03 File Offset: 0x000D3F03
	private IEnumerator Bounce(float angleMin, float angleMax)
	{
		this.source.enabled = false;
		this.bounceSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		Vector2 zero = Vector2.zero;
		float num = Random.Range(angleMin, angleMax);
		zero.x = 5f * Mathf.Cos(num * 0.017453292f);
		zero.y = 5f * Mathf.Sin(num * 0.017453292f);
		this.body.linearVelocity = zero;
		yield return new WaitForSeconds(0.5f);
		this.source.enabled = true;
		this.bounceRoutine = null;
		this.runRoutine = base.StartCoroutine(this.Run());
		yield break;
	}

	// Token: 0x0600306B RID: 12395 RVA: 0x000D5D20 File Offset: 0x000D3F20
	private IEnumerator Heal()
	{
		Action doHeal = null;
		doHeal = delegate()
		{
			GameManager instance = GameManager.instance;
			instance.AddBlueHealthQueued();
			instance.UnloadingLevel -= doHeal;
			doHeal = null;
		};
		GameManager.instance.UnloadingLevel += doHeal;
		if (HeroController.instance && Vector2.Distance(base.transform.position, HeroController.instance.transform.position) > 40f)
		{
			base.gameObject.SetActive(false);
		}
		yield return new WaitForSeconds(1.2f);
		if (this.screenFlash)
		{
			GameObject gameObject = this.screenFlash.Spawn();
			gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(0f, 0.7f, 1f));
			PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(gameObject, "Fade Away");
			if (playMakerFSM)
			{
				FSMUtility.SetFloat(playMakerFSM, "Alpha", 0.75f);
			}
		}
		if (doHeal != null)
		{
			doHeal();
		}
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x000D5D30 File Offset: 0x000D3F30
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (Time.timeAsDouble < this.activateTime)
		{
			return IHitResponder.Response.None;
		}
		if (this.alive)
		{
			this.alive = false;
			if (this.runRoutine != null)
			{
				base.StopCoroutine(this.runRoutine);
			}
			if (this.bounceRoutine != null)
			{
				base.StopCoroutine(this.bounceRoutine);
			}
			if (this.corpsePrefab)
			{
				Object.Instantiate<GameObject>(this.corpsePrefab, base.transform.position, base.transform.rotation);
			}
			if (this.splatEffectChild)
			{
				this.splatEffectChild.SetActive(true);
			}
			PlayerData playerData = GameManager.instance.playerData;
			if (playerData.GetBool("hasJournal"))
			{
				if (!playerData.GetBool(this.killedPDBool))
				{
					playerData.SetBool(this.killedPDBool, true);
					if (this.journalUpdateMsgPrefab)
					{
						Object.Instantiate<GameObject>(this.journalUpdateMsgPrefab);
					}
				}
				int num = playerData.GetInt(this.killsPDBool);
				if (num > 0)
				{
					num--;
					playerData.SetInt(this.killsPDBool, num);
					if (num <= 0 && this.journalUpdateMsgPrefab)
					{
						PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(Object.Instantiate<GameObject>(this.journalUpdateMsgPrefab), "Journal Msg");
						if (playMakerFSM)
						{
							FSMUtility.SetBool(playMakerFSM, "Full", true);
						}
					}
				}
				playerData.SetBool(this.newDataPDBool, true);
			}
			bool flag = false;
			if (!this.healthScuttler)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				if (damageInstance.IsNailDamage)
				{
					flag = true;
					if (this.strikeNailPrefab)
					{
						this.strikeNailPrefab.Spawn(base.transform.position);
					}
					if (this.slashImpactPrefab)
					{
						GameObject gameObject = this.slashImpactPrefab.Spawn(base.transform.position);
						float direction = damageInstance.Direction;
						if (direction < 45f)
						{
							gameObject.transform.SetRotation2D(Random.Range(340f, 380f));
							gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
						}
						else if (direction < 135f)
						{
							gameObject.transform.SetRotation2D(Random.Range(70f, 110f));
							gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
						}
						else if (direction < 225f)
						{
							gameObject.transform.SetRotation2D(Random.Range(340f, 380f));
							gameObject.transform.localScale = new Vector3(-0.9f, 0.9f, 1f);
						}
						else if (direction < 360f)
						{
							gameObject.transform.SetRotation2D(Random.Range(250f, 290f));
							gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
						}
					}
				}
				else if (damageInstance.AttackType == AttackTypes.Spell || damageInstance.AttackType == AttackTypes.NailBeam)
				{
					flag = true;
					if (this.fireballHitPrefab)
					{
						GameObject gameObject2 = this.fireballHitPrefab.Spawn(base.transform.position);
						gameObject2.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
						gameObject2.transform.SetPositionZ(0.0031f);
					}
				}
				else if (damageInstance.AttackType == AttackTypes.Generic)
				{
					flag = true;
				}
				this.deathSound1.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
				this.deathSound2.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
				GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
				if (gameCameras)
				{
					gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
				}
				BloodSpawner.SpawnBlood(base.transform.position, 12, 18, 4f, 22f, 30f, 150f, new Color?(this.bloodColor), 0f);
				Renderer component = base.GetComponent<Renderer>();
				if (component)
				{
					component.enabled = false;
				}
				if (flag)
				{
					if (this.pool)
					{
						this.pool.transform.SetPositionZ(-0.2f);
						FlingUtils.FlingChildren(new FlingUtils.ChildrenConfig
						{
							Parent = this.pool,
							AmountMin = 8,
							AmountMax = 10,
							SpeedMin = 15f,
							SpeedMax = 20f,
							AngleMin = 30f,
							AngleMax = 150f,
							OriginVariationX = 0f,
							OriginVariationY = 0f
						}, base.transform, Vector3.zero, null);
					}
					base.StartCoroutine(this.Heal());
				}
				else
				{
					base.gameObject.SetActive(false);
				}
			}
			return IHitResponder.Response.GenericHit;
		}
		return IHitResponder.Response.None;
	}

	// Token: 0x0400334D RID: 13133
	[Header("Instance Variables")]
	public bool startIdle;

	// Token: 0x0400334E RID: 13134
	public bool startRunning;

	// Token: 0x0400334F RID: 13135
	[Header("Other Variables")]
	public string killedPDBool = "killedOrangeScuttler";

	// Token: 0x04003350 RID: 13136
	public string killsPDBool = "killsOrangeScuttler";

	// Token: 0x04003351 RID: 13137
	public string newDataPDBool = "newDataOrangeScuttler";

	// Token: 0x04003352 RID: 13138
	[Space]
	public string runAnim = "Run";

	// Token: 0x04003353 RID: 13139
	public string landAnim = "Land";

	// Token: 0x04003354 RID: 13140
	[Space]
	public GameObject corpsePrefab;

	// Token: 0x04003355 RID: 13141
	public GameObject splatEffectChild;

	// Token: 0x04003356 RID: 13142
	public GameObject journalUpdateMsgPrefab;

	// Token: 0x04003357 RID: 13143
	[Space]
	public AudioSource audioSourcePrefab;

	// Token: 0x04003358 RID: 13144
	public AudioEvent bounceSound;

	// Token: 0x04003359 RID: 13145
	public TriggerEnterEvent heroAlert;

	// Token: 0x0400335A RID: 13146
	[Space]
	public bool healthScuttler;

	// Token: 0x0400335B RID: 13147
	[Header("Health Scuttler Variables")]
	public GameObject strikeNailPrefab;

	// Token: 0x0400335C RID: 13148
	public GameObject slashImpactPrefab;

	// Token: 0x0400335D RID: 13149
	public GameObject fireballHitPrefab;

	// Token: 0x0400335E RID: 13150
	public AudioEvent deathSound1;

	// Token: 0x0400335F RID: 13151
	public AudioEvent deathSound2;

	// Token: 0x04003360 RID: 13152
	public GameObject pool;

	// Token: 0x04003361 RID: 13153
	public GameObject screenFlash;

	// Token: 0x04003362 RID: 13154
	public Color bloodColor;

	// Token: 0x04003363 RID: 13155
	private Transform hero;

	// Token: 0x04003364 RID: 13156
	private float maxSpeed;

	// Token: 0x04003365 RID: 13157
	private float acceleration = 0.3f;

	// Token: 0x04003366 RID: 13158
	private bool landed;

	// Token: 0x04003367 RID: 13159
	private Coroutine runRoutine;

	// Token: 0x04003368 RID: 13160
	private Coroutine bounceRoutine;

	// Token: 0x04003369 RID: 13161
	private float rayLength;

	// Token: 0x0400336A RID: 13162
	private Vector2 rayOrigin;

	// Token: 0x0400336B RID: 13163
	private tk2dSpriteAnimator anim;

	// Token: 0x0400336C RID: 13164
	private Rigidbody2D body;

	// Token: 0x0400336D RID: 13165
	private AudioSource source;

	// Token: 0x0400336E RID: 13166
	private bool alive = true;

	// Token: 0x0400336F RID: 13167
	private bool reverseRun;

	// Token: 0x04003370 RID: 13168
	private float activateDelay = 0.25f;

	// Token: 0x04003371 RID: 13169
	private double activateTime;
}
