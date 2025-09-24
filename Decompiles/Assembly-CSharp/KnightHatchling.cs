using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002FE RID: 766
public class KnightHatchling : MonoBehaviour
{
	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x06001B4A RID: 6986 RVA: 0x0007EB73 File Offset: 0x0007CD73
	public bool IsGrounded
	{
		get
		{
			return this.groundColliders.Count > 0;
		}
	}

	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x06001B4B RID: 6987 RVA: 0x0007EB83 File Offset: 0x0007CD83
	// (set) Token: 0x06001B4C RID: 6988 RVA: 0x0007EB8B File Offset: 0x0007CD8B
	public KnightHatchling.State CurrentState
	{
		get
		{
			return this.currentState;
		}
		private set
		{
			if (this.currentState != value)
			{
				this.PreviousState = this.currentState;
			}
			this.currentState = value;
		}
	}

	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x06001B4D RID: 6989 RVA: 0x0007EBA9 File Offset: 0x0007CDA9
	// (set) Token: 0x06001B4E RID: 6990 RVA: 0x0007EBB1 File Offset: 0x0007CDB1
	public KnightHatchling.State LastFrameState { get; private set; }

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x06001B4F RID: 6991 RVA: 0x0007EBBA File Offset: 0x0007CDBA
	// (set) Token: 0x06001B50 RID: 6992 RVA: 0x0007EBC2 File Offset: 0x0007CDC2
	public KnightHatchling.State PreviousState { get; private set; }

	// Token: 0x06001B51 RID: 6993 RVA: 0x0007EBCC File Offset: 0x0007CDCC
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.col = base.GetComponent<Collider2D>();
		this.spriteFlash = base.GetComponent<SpriteFlash>();
	}

	// Token: 0x06001B52 RID: 6994 RVA: 0x0007EC24 File Offset: 0x0007CE24
	private void Start()
	{
		if (this.enemyRange)
		{
			this.enemyRange.OnTriggerStayed += delegate(Collider2D collision, GameObject obj)
			{
				if (!this.target)
				{
					if (collision.tag == "Hatchling Magnet")
					{
						this.target = collision.gameObject;
					}
					else if (collision.tag != "Ignore Hatchling" && Physics2D.Linecast(base.transform.position, collision.transform.position, LayerMask.GetMask(new string[]
					{
						"Terrain",
						"Soft Terrain"
					})).collider == null)
					{
						this.target = collision.gameObject;
					}
				}
				if (this.CurrentState == KnightHatchling.State.Follow && this.target)
				{
					this.CurrentState = KnightHatchling.State.Attack;
				}
			};
			this.enemyRange.OnTriggerExited += delegate(Collider2D collision, GameObject obj)
			{
				if (this.CurrentState != KnightHatchling.State.Attack && this.target && this.target == collision.gameObject)
				{
					this.target = null;
				}
			};
		}
		if (this.groundRange)
		{
			this.groundRange.OnTriggerEntered += delegate(Collider2D collision, GameObject obj)
			{
				if (!this.groundColliders.Contains(collision))
				{
					this.groundColliders.Add(collision);
				}
			};
			this.groundRange.OnTriggerExited += delegate(Collider2D collision, GameObject obj)
			{
				if (this.groundColliders.Contains(collision))
				{
					this.groundColliders.Remove(collision);
				}
			};
		}
		this.startZ = Random.Range(0.0041f, 0.0049f);
		this.sleepZ = Random.Range(0.003f, 0.0035f);
		base.transform.SetPositionZ(this.startZ);
	}

	// Token: 0x06001B53 RID: 6995 RVA: 0x0007ECE4 File Offset: 0x0007CEE4
	private void OnEnable()
	{
		if (GameManager.instance.entryGateName == "dreamGate")
		{
			this.dreamSpawn = true;
		}
		PlayerData playerData = GameManager.instance.playerData;
		this.details = this.normalDetails;
		if (this.audioSource)
		{
			this.audioSource.pitch = Random.Range(0.85f, 1.15f);
			if (this.loopClips.Length != 0)
			{
				this.audioSource.clip = this.loopClips[Random.Range(0, this.loopClips.Length)];
			}
		}
		if (this.dungPt)
		{
			if (this.details.dung && !this.dreamSpawn)
			{
				this.dungPt.Play();
			}
			else
			{
				this.dungPt.Stop();
			}
		}
		if (this.enemyRange)
		{
			this.enemyRange.gameObject.SetActive(false);
		}
		if (this.groundRange)
		{
			this.groundRange.gameObject.SetActive(true);
		}
		if (this.col)
		{
			this.col.enabled = false;
		}
		this.groundColliders.Clear();
		this.target = null;
		this.LastFrameState = KnightHatchling.State.None;
		this.CurrentState = KnightHatchling.State.None;
		if (this.terrainCollider)
		{
			this.terrainCollider.enabled = true;
		}
		if (this.meshRenderer)
		{
			this.meshRenderer.enabled = false;
		}
		base.StartCoroutine(this.Spawn());
	}

	// Token: 0x06001B54 RID: 6996 RVA: 0x0007EE63 File Offset: 0x0007D063
	private void OnDisable()
	{
		this.quickSpawn = false;
		this.dreamSpawn = false;
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x0007EE74 File Offset: 0x0007D074
	private void FixedUpdate()
	{
		KnightHatchling.State state = this.CurrentState;
		switch (this.CurrentState)
		{
		case KnightHatchling.State.Follow:
		{
			if (this.LastFrameState != KnightHatchling.State.Follow)
			{
				if (this.damageEnemies)
				{
					this.damageEnemies.damageDealt = 0;
				}
				if (this.col)
				{
					this.col.enabled = true;
				}
				if (this.enemyRange)
				{
					this.enemyRange.gameObject.SetActive(true);
				}
				if (this.animator)
				{
					this.animator.Play(this.details.flyAnim);
				}
				if (this.audioSource)
				{
					this.audioSource.Play();
				}
				this.body.isKinematic = false;
				this.targetRadius = Random.Range(0.1f, 0.75f);
				this.offset = new Vector3(Random.Range(-3.5f, 3.5f), Random.Range(1.25f, 3f));
				this.awayTimer = 0f;
				base.transform.SetPositionZ(this.startZ);
			}
			float heroDistance = this.GetHeroDistance();
			float speedMax = Mathf.Clamp(heroDistance + 4f, 4f, 18f);
			this.DoFace(false, true, this.details.turnFlyAnim, true, 0.5f);
			this.DoChase(HeroController.instance.transform, 2f, speedMax, 40f, this.targetRadius, 0.9f, this.offset);
			this.DoBuzz(0.75f, 1f, 18f, 80f, 110f, new Vector2(50f, 50f));
			if (heroDistance * 1.15f > 10f)
			{
				this.awayTimer += Time.fixedDeltaTime;
				if (this.awayTimer >= 4f)
				{
					state = KnightHatchling.State.Tele;
				}
			}
			else
			{
				this.awayTimer = 0f;
			}
			break;
		}
		case KnightHatchling.State.Tele:
			if (this.LastFrameState != KnightHatchling.State.Tele)
			{
				if (this.audioSource)
				{
					this.audioSource.Stop();
				}
				if (this.animator)
				{
					this.animator.Play(this.details.teleStartAnim);
				}
				if (this.enemyRange)
				{
					this.enemyRange.gameObject.SetActive(false);
				}
				if (this.groundRange)
				{
					this.groundRange.gameObject.SetActive(false);
				}
				if (this.terrainCollider)
				{
					this.terrainCollider.enabled = false;
				}
			}
			this.DoChase(HeroController.instance.transform, 2f, 25f, 150f, 0f, 0f, new Vector2(0f, -0.5f));
			if (this.GetHeroDistance() < 1f)
			{
				state = KnightHatchling.State.None;
				base.StartCoroutine(this.TeleEnd());
			}
			break;
		case KnightHatchling.State.Attack:
			if (this.LastFrameState != KnightHatchling.State.Attack)
			{
				if (this.audioSource)
				{
					this.audioSource.Stop();
					if (this.attackChargeClip)
					{
						this.audioSource.PlayOneShot(this.attackChargeClip);
					}
				}
				if (this.animator)
				{
					this.animator.Play(this.details.attackAnim);
				}
				if (this.enemyRange)
				{
					this.enemyRange.gameObject.SetActive(false);
				}
				if (this.damageEnemies)
				{
					this.damageEnemies.damageDealt = this.details.damage;
				}
				this.attackFinishTime = Time.timeAsDouble + 2.0;
			}
			if (Time.timeAsDouble > this.attackFinishTime || this.target == null)
			{
				this.target = null;
				state = KnightHatchling.State.Follow;
			}
			else
			{
				this.DoFace(false, true, this.details.turnAttackAnim, true, 0.1f);
				this.DoChaseSimple(this.target.transform, 25f, 100f, 0f, 0f);
			}
			break;
		case KnightHatchling.State.BenchRestStart:
			if (this.LastFrameState != KnightHatchling.State.BenchRestStart)
			{
				this.body.linearVelocity = Vector2.zero;
				if (this.animator)
				{
					this.animator.Play(this.details.flyAnim);
				}
				this.benchRestWaitTime = Time.timeAsDouble + (double)Random.Range(2f, 5f);
			}
			if (Time.timeAsDouble < this.benchRestWaitTime)
			{
				this.DoBuzz(0.75f, 1f, 2f, 30f, 50f, new Vector2(1f, 1f));
				this.DoFace(false, true, this.details.turnFlyAnim, true, 0.5f);
			}
			else
			{
				state = KnightHatchling.State.BenchRestLower;
			}
			break;
		case KnightHatchling.State.BenchRestLower:
		{
			if (this.LastFrameState != KnightHatchling.State.BenchRestLower)
			{
				if (this.animator)
				{
					this.animator.Play(this.details.flyAnim);
				}
				base.transform.SetPositionZ(this.sleepZ);
				this.body.isKinematic = false;
			}
			this.body.AddForce(new Vector2(0f, -5f));
			Vector2 linearVelocity = this.body.linearVelocity;
			linearVelocity.x *= 0.85f;
			this.body.linearVelocity = linearVelocity;
			this.DoFace(false, true, this.details.turnFlyAnim, true, 0.5f);
			if (this.IsGrounded)
			{
				state = KnightHatchling.State.BenchResting;
				if (this.details.dung)
				{
					this.dungSleepPlopSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
				}
				if (this.audioSource)
				{
					this.audioSource.Stop();
				}
				this.body.isKinematic = true;
				this.body.linearVelocity = Vector2.zero;
				if (this.animator)
				{
					this.animator.Play(this.details.restStartAnim);
				}
				if (this.details.groundPoint)
				{
					RaycastHit2D raycastHit2D = Helper.Raycast2D(base.transform.position, Vector2.down, 2f, 256);
					if (raycastHit2D.collider != null)
					{
						Vector2 point = raycastHit2D.point;
						Vector2 v = this.details.groundPoint.position - point;
						v.x = 0f;
						base.transform.position -= v;
					}
				}
			}
			break;
		}
		}
		this.LastFrameState = this.CurrentState;
		this.CurrentState = state;
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x0007F53F File Offset: 0x0007D73F
	private IEnumerator Spawn()
	{
		yield return null;
		if (this.dreamSpawn)
		{
			yield return new WaitForSeconds(Random.Range(1.5f, 2f));
		}
		if (this.audioSource)
		{
			this.audioSource.Play();
		}
		if (this.meshRenderer && !this.dreamSpawn)
		{
			this.meshRenderer.enabled = true;
		}
		if (!this.quickSpawn)
		{
			float finishDelay = 0f;
			if (this.animator)
			{
				tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName(this.details.hatchAnim);
				if (clipByName != null)
				{
					this.animator.Play(clipByName);
					finishDelay = clipByName.Duration;
				}
			}
			if (this.body)
			{
				float num = Random.Range(65f, 75f);
				float num2 = Random.Range(85f, 95f);
				Vector2 linearVelocity = default(Vector2);
				linearVelocity.x = num * Mathf.Cos(num2 * 0.017453292f);
				linearVelocity.y = num * Mathf.Sin(num2 * 0.017453292f);
				this.body.linearVelocity = linearVelocity;
			}
			for (float elapsed = 0f; elapsed < finishDelay; elapsed += Time.fixedDeltaTime)
			{
				if (this.body)
				{
					this.body.linearVelocity *= 0.8f;
				}
				yield return new WaitForFixedUpdate();
			}
			if (this.dreamSpawn)
			{
				this.meshRenderer.enabled = true;
				yield return base.StartCoroutine(this.animator.PlayAnimWait("Dreamgate In", null));
				if (this.dungPt && this.details.dung)
				{
					this.dungPt.Play();
				}
			}
		}
		this.openEffect.SetActive(true);
		if (GameManager.instance.playerData.atBench)
		{
			this.CurrentState = KnightHatchling.State.BenchRestStart;
		}
		else
		{
			this.CurrentState = KnightHatchling.State.Follow;
		}
		yield break;
	}

	// Token: 0x06001B57 RID: 6999 RVA: 0x0007F54E File Offset: 0x0007D74E
	private float GetHeroDistance()
	{
		return Vector2.Distance(base.transform.position, HeroController.instance.transform.position);
	}

	// Token: 0x06001B58 RID: 7000 RVA: 0x0007F579 File Offset: 0x0007D779
	private IEnumerator TeleEnd()
	{
		if (this.groundRange)
		{
			this.groundRange.gameObject.SetActive(true);
		}
		if (this.terrainCollider)
		{
			this.terrainCollider.enabled = true;
		}
		if (this.audioSource)
		{
			this.audioSource.Play();
		}
		if (this.animator)
		{
			tk2dSpriteAnimationClip clip = this.animator.GetClipByName(this.details.teleEndAnim);
			this.animator.Play(clip);
			for (float elapsed = 0f; elapsed < clip.Duration; elapsed += Time.fixedDeltaTime)
			{
				if (this.body)
				{
					this.body.linearVelocity *= 0.7f;
				}
				yield return new WaitForFixedUpdate();
			}
			clip = null;
		}
		this.CurrentState = KnightHatchling.State.Follow;
		yield break;
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x0007F588 File Offset: 0x0007D788
	public void FsmHitLanded()
	{
		this.CurrentState = KnightHatchling.State.None;
		if (this.damageEnemies)
		{
			this.damageEnemies.damageDealt = 0;
		}
		base.StartCoroutine(this.Explode());
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x0007F5B7 File Offset: 0x0007D7B7
	private IEnumerator Explode()
	{
		yield return new WaitForFixedUpdate();
		if (this.col)
		{
			this.col.enabled = false;
		}
		this.explodeSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		this.body.linearVelocity = Vector2.zero;
		if (this.spriteFlash)
		{
			this.spriteFlash.CancelFlash();
		}
		float seconds = 2f;
		if (this.details.dung)
		{
			if (this.dungPt)
			{
				this.dungPt.Stop();
			}
			this.dungExplodeSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
			if (this.dungExplosionPrefab)
			{
				this.dungExplosionPrefab.Spawn(base.transform.position);
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = false;
			}
			BloodSpawner.SpawnBlood(HeroController.instance.transform.position, 8, 8, 10f, 20f, 0f, 360f, new Color?(this.details.spatterColor), 0f);
		}
		else if (this.animator)
		{
			tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName("Burst");
			if (clipByName != null)
			{
				this.animator.Play(clipByName);
				seconds = clipByName.Duration;
			}
		}
		yield return new WaitForSeconds(seconds);
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x06001B5B RID: 7003 RVA: 0x0007F5C6 File Offset: 0x0007D7C6
	public void FsmCharmsEnd()
	{
		this.CurrentState = KnightHatchling.State.None;
		base.StartCoroutine(this.CharmsEnd());
	}

	// Token: 0x06001B5C RID: 7004 RVA: 0x0007F5DC File Offset: 0x0007D7DC
	private IEnumerator CharmsEnd()
	{
		float finishDelay = 0f;
		if (this.animator)
		{
			tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName("Tele Start");
			if (clipByName != null)
			{
				this.animator.Play(clipByName);
				finishDelay = clipByName.Duration;
			}
		}
		for (float elapsed = 0f; elapsed < finishDelay; elapsed += Time.fixedDeltaTime)
		{
			if (this.body)
			{
				this.body.linearVelocity *= 0.8f;
			}
			yield return new WaitForFixedUpdate();
		}
		this.meshRenderer.enabled = false;
		if (this.details.dung && this.dungPt)
		{
			this.dungPt.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			while (this.dungPt.IsAlive(true))
			{
				yield return null;
			}
		}
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x06001B5D RID: 7005 RVA: 0x0007F5EB File Offset: 0x0007D7EB
	public void FsmHazardReload()
	{
		base.gameObject.Recycle();
	}

	// Token: 0x06001B5E RID: 7006 RVA: 0x0007F5F8 File Offset: 0x0007D7F8
	public void FsmBenchRestStart()
	{
		this.CurrentState = KnightHatchling.State.BenchRestStart;
	}

	// Token: 0x06001B5F RID: 7007 RVA: 0x0007F601 File Offset: 0x0007D801
	public void FsmBenchRestEnd()
	{
		if (this.CurrentState == KnightHatchling.State.BenchResting)
		{
			base.StartCoroutine(this.WakeUp());
			return;
		}
		this.CurrentState = KnightHatchling.State.Follow;
	}

	// Token: 0x06001B60 RID: 7008 RVA: 0x0007F621 File Offset: 0x0007D821
	private IEnumerator WakeUp()
	{
		float seconds = 0f;
		if (this.animator)
		{
			tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName(this.details.restEndAnim);
			if (clipByName != null)
			{
				this.animator.Play(clipByName);
				seconds = clipByName.Duration;
			}
		}
		yield return new WaitForSeconds(seconds);
		this.CurrentState = KnightHatchling.State.Follow;
		yield break;
	}

	// Token: 0x06001B61 RID: 7009 RVA: 0x0007F630 File Offset: 0x0007D830
	public void FsmQuickSpawn()
	{
		this.quickSpawn = true;
	}

	// Token: 0x06001B62 RID: 7010 RVA: 0x0007F639 File Offset: 0x0007D839
	public void FsmDreamGateOut()
	{
		this.CurrentState = KnightHatchling.State.None;
		base.StartCoroutine(this.DreamGateOut());
	}

	// Token: 0x06001B63 RID: 7011 RVA: 0x0007F64F File Offset: 0x0007D84F
	private IEnumerator DreamGateOut()
	{
		float seconds = 0f;
		if (this.animator)
		{
			tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName("Dreamgate Out");
			if (clipByName != null)
			{
				this.animator.Play(clipByName);
				seconds = clipByName.Duration;
			}
		}
		yield return new WaitForSeconds(seconds);
		this.meshRenderer.enabled = false;
		if (this.details.dung && this.dungPt)
		{
			this.dungPt.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		yield break;
	}

	// Token: 0x06001B64 RID: 7012 RVA: 0x0007F660 File Offset: 0x0007D860
	private void DoFace(bool spriteFacesRight, bool playNewAnimation, string newAnimationClip, bool pauseBetweenTurns, float pauseTime)
	{
		if (this.body == null)
		{
			return;
		}
		ref Vector2 linearVelocity = this.body.linearVelocity;
		Vector3 localScale = base.transform.localScale;
		float x = linearVelocity.x;
		if (this.CurrentState != this.LastFrameState)
		{
			this.xScale = base.transform.localScale.x;
			this.pauseTimer = 0f;
		}
		if (this.xScale < 0f)
		{
			this.xScale *= -1f;
		}
		if (this.pauseTimer <= 0f || !pauseBetweenTurns)
		{
			if (x > 0f)
			{
				if (spriteFacesRight)
				{
					if (localScale.x != this.xScale)
					{
						this.pauseTimer = pauseTime;
						localScale.x = this.xScale;
						if (playNewAnimation)
						{
							this.animator.Play(newAnimationClip);
							this.animator.PlayFromFrame(0);
						}
					}
				}
				else if (localScale.x != -this.xScale)
				{
					this.pauseTimer = pauseTime;
					localScale.x = -this.xScale;
					if (playNewAnimation)
					{
						this.animator.Play(newAnimationClip);
						this.animator.PlayFromFrame(0);
					}
				}
			}
			else if (x <= 0f)
			{
				if (spriteFacesRight)
				{
					if (localScale.x != -this.xScale)
					{
						this.pauseTimer = pauseTime;
						localScale.x = -this.xScale;
						if (playNewAnimation)
						{
							this.animator.Play(newAnimationClip);
							this.animator.PlayFromFrame(0);
						}
					}
				}
				else if (localScale.x != this.xScale)
				{
					this.pauseTimer = pauseTime;
					localScale.x = this.xScale;
					if (playNewAnimation)
					{
						this.animator.Play(newAnimationClip);
						this.animator.PlayFromFrame(0);
					}
				}
			}
		}
		else
		{
			this.pauseTimer -= Time.deltaTime;
		}
		base.transform.localScale = new Vector3(localScale.x, base.transform.localScale.y, base.transform.localScale.z);
	}

	// Token: 0x06001B65 RID: 7013 RVA: 0x0007F878 File Offset: 0x0007DA78
	private void DoChase(Transform target, float distance, float speedMax, float accelerationForce, float targetRadius, float deceleration, Vector2 offset)
	{
		if (this.body == null)
		{
			return;
		}
		float num = Mathf.Sqrt(Mathf.Pow(base.transform.position.x - (target.position.x + offset.x), 2f) + Mathf.Pow(base.transform.position.y - (target.position.y + offset.y), 2f));
		Vector2 vector = this.body.linearVelocity;
		if (num <= distance - targetRadius || num >= distance + targetRadius)
		{
			Vector2 vector2 = new Vector2(target.position.x + offset.x - base.transform.position.x, target.position.y + offset.y - base.transform.position.y);
			vector2 = Vector2.ClampMagnitude(vector2, 1f);
			vector2 = new Vector2(vector2.x * accelerationForce, vector2.y * accelerationForce);
			if (num < distance)
			{
				vector2 = new Vector2(-vector2.x, -vector2.y);
			}
			this.body.AddForce(vector2);
			vector = Vector2.ClampMagnitude(vector, speedMax);
			this.body.linearVelocity = vector;
			return;
		}
		vector = this.body.linearVelocity;
		if (vector.x < 0f)
		{
			vector.x *= deceleration;
			if (vector.x > 0f)
			{
				vector.x = 0f;
			}
		}
		else if (vector.x > 0f)
		{
			vector.x *= deceleration;
			if (vector.x < 0f)
			{
				vector.x = 0f;
			}
		}
		if (vector.y < 0f)
		{
			vector.y *= deceleration;
			if (vector.y > 0f)
			{
				vector.y = 0f;
			}
		}
		else if (vector.y > 0f)
		{
			vector.y *= deceleration;
			if (vector.y < 0f)
			{
				vector.y = 0f;
			}
		}
		this.body.linearVelocity = vector;
	}

	// Token: 0x06001B66 RID: 7014 RVA: 0x0007FAAC File Offset: 0x0007DCAC
	private void DoBuzz(float waitMin, float waitMax, float speedMax, float accelerationMin, float accelerationMax, Vector2 roamingRange)
	{
		if (this.body == null)
		{
			return;
		}
		float num = 1.125f;
		if (this.CurrentState != this.LastFrameState)
		{
			this.startX = base.transform.position.x;
			this.startY = base.transform.position.y;
		}
		Vector2 linearVelocity = this.body.linearVelocity;
		if (base.transform.position.y < this.startY - roamingRange.y)
		{
			if (linearVelocity.y < 0f)
			{
				this.accelY = accelerationMax;
				this.accelY /= 2000f;
				linearVelocity.y /= num;
				this.waitTime = Random.Range(waitMin, waitMax);
			}
		}
		else if (base.transform.position.y > this.startY + roamingRange.y && linearVelocity.y > 0f)
		{
			this.accelY = -accelerationMax;
			this.accelY /= 2000f;
			linearVelocity.y /= num;
			this.waitTime = Random.Range(waitMin, waitMax);
		}
		if (base.transform.position.x < this.startX - roamingRange.x)
		{
			if (linearVelocity.x < 0f)
			{
				this.accelX = accelerationMax;
				this.accelX /= 2000f;
				linearVelocity.x /= num;
				this.waitTime = Random.Range(waitMin, waitMax);
			}
		}
		else if (base.transform.position.x > this.startX + roamingRange.x && linearVelocity.x > 0f)
		{
			this.accelX = -accelerationMax;
			this.accelX /= 2000f;
			linearVelocity.x /= num;
			this.waitTime = Random.Range(waitMin, waitMax);
		}
		if (this.waitTime <= Mathf.Epsilon)
		{
			if (base.transform.position.y < this.startY - roamingRange.y)
			{
				this.accelY = Random.Range(accelerationMin, accelerationMax);
			}
			else if (base.transform.position.y > this.startY + roamingRange.y)
			{
				this.accelY = Random.Range(-accelerationMax, accelerationMin);
			}
			else
			{
				this.accelY = Random.Range(-accelerationMax, accelerationMax);
			}
			if (base.transform.position.x < this.startX - roamingRange.x)
			{
				this.accelX = Random.Range(accelerationMin, accelerationMax);
			}
			else if (base.transform.position.x > this.startX + roamingRange.x)
			{
				this.accelX = Random.Range(-accelerationMax, accelerationMin);
			}
			else
			{
				this.accelX = Random.Range(-accelerationMax, accelerationMax);
			}
			this.accelY /= 2000f;
			this.accelX /= 2000f;
			this.waitTime = Random.Range(waitMin, waitMax);
		}
		if (this.waitTime > Mathf.Epsilon)
		{
			this.waitTime -= Time.deltaTime;
		}
		linearVelocity.x += this.accelX;
		linearVelocity.y += this.accelY;
		if (linearVelocity.x > speedMax)
		{
			linearVelocity.x = speedMax;
		}
		if (linearVelocity.x < -speedMax)
		{
			linearVelocity.x = -speedMax;
		}
		if (linearVelocity.y > speedMax)
		{
			linearVelocity.y = speedMax;
		}
		if (linearVelocity.y < -speedMax)
		{
			linearVelocity.y = -speedMax;
		}
		this.body.linearVelocity = linearVelocity;
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x0007FE68 File Offset: 0x0007E068
	private void DoChaseSimple(Transform target, float speedMax, float accelerationForce, float offsetX, float offsetY)
	{
		if (this.body == null)
		{
			return;
		}
		Vector2 vector = new Vector2(target.position.x + offsetX - base.transform.position.x, target.position.y + offsetY - base.transform.position.y);
		vector = Vector2.ClampMagnitude(vector, 1f);
		vector = new Vector2(vector.x * accelerationForce, vector.y * accelerationForce);
		this.body.AddForce(vector);
		Vector2 vector2 = this.body.linearVelocity;
		vector2 = Vector2.ClampMagnitude(vector2, speedMax);
		this.body.linearVelocity = vector2;
	}

	// Token: 0x04001A3C RID: 6716
	public TriggerEnterEvent enemyRange;

	// Token: 0x04001A3D RID: 6717
	public TriggerEnterEvent groundRange;

	// Token: 0x04001A3E RID: 6718
	public Collider2D terrainCollider;

	// Token: 0x04001A3F RID: 6719
	private List<Collider2D> groundColliders = new List<Collider2D>();

	// Token: 0x04001A40 RID: 6720
	private GameObject target;

	// Token: 0x04001A41 RID: 6721
	public KnightHatchling.TypeDetails normalDetails = new KnightHatchling.TypeDetails
	{
		damage = 9,
		dung = false,
		attackAnim = "Attack",
		flyAnim = "Fly",
		hatchAnim = "Hatch",
		teleEndAnim = "Tele End",
		teleStartAnim = "Tele Start",
		turnAttackAnim = "TurnToAttack",
		turnFlyAnim = "TurnToFly",
		restStartAnim = "Rest Start",
		restEndAnim = "Rest End",
		spatterColor = Color.black
	};

	// Token: 0x04001A42 RID: 6722
	public KnightHatchling.TypeDetails dungDetails = new KnightHatchling.TypeDetails
	{
		damage = 4,
		dung = true,
		attackAnim = "D Attack",
		flyAnim = "D Fly",
		hatchAnim = "D Hatch",
		teleEndAnim = "D Tele End",
		teleStartAnim = "D Tele Start",
		turnAttackAnim = "D TurnToAttack",
		turnFlyAnim = "D TurnToFly",
		restStartAnim = "D Rest Start",
		restEndAnim = "D Rest End",
		spatterColor = new Color(0.749f, 0.522f, 0.353f)
	};

	// Token: 0x04001A43 RID: 6723
	private KnightHatchling.TypeDetails details;

	// Token: 0x04001A44 RID: 6724
	public ParticleSystem dungPt;

	// Token: 0x04001A45 RID: 6725
	public AudioClip[] loopClips;

	// Token: 0x04001A46 RID: 6726
	public AudioClip attackChargeClip;

	// Token: 0x04001A47 RID: 6727
	public AudioSource audioSourcePrefab;

	// Token: 0x04001A48 RID: 6728
	public AudioEvent explodeSound = new AudioEvent
	{
		PitchMin = 0.85f,
		PitchMax = 1.15f,
		Volume = 1f
	};

	// Token: 0x04001A49 RID: 6729
	public AudioEvent dungExplodeSound = new AudioEvent
	{
		PitchMin = 0.9f,
		PitchMax = 1.1f,
		Volume = 1f
	};

	// Token: 0x04001A4A RID: 6730
	public AudioEventRandom dungSleepPlopSound = new AudioEventRandom
	{
		PitchMin = 0.9f,
		PitchMax = 1.1f,
		Volume = 1f
	};

	// Token: 0x04001A4B RID: 6731
	public GameObject openEffect;

	// Token: 0x04001A4C RID: 6732
	public GameObject dungExplosionPrefab;

	// Token: 0x04001A4D RID: 6733
	private KnightHatchling.State currentState;

	// Token: 0x04001A50 RID: 6736
	private float targetRadius;

	// Token: 0x04001A51 RID: 6737
	private Vector3 offset;

	// Token: 0x04001A52 RID: 6738
	private float awayTimer;

	// Token: 0x04001A53 RID: 6739
	private double attackFinishTime;

	// Token: 0x04001A54 RID: 6740
	private double benchRestWaitTime;

	// Token: 0x04001A55 RID: 6741
	private bool quickSpawn;

	// Token: 0x04001A56 RID: 6742
	private bool dreamSpawn;

	// Token: 0x04001A57 RID: 6743
	private float startZ;

	// Token: 0x04001A58 RID: 6744
	private float sleepZ;

	// Token: 0x04001A59 RID: 6745
	public DamageEnemies damageEnemies;

	// Token: 0x04001A5A RID: 6746
	private AudioSource audioSource;

	// Token: 0x04001A5B RID: 6747
	private MeshRenderer meshRenderer;

	// Token: 0x04001A5C RID: 6748
	private tk2dSpriteAnimator animator;

	// Token: 0x04001A5D RID: 6749
	private Rigidbody2D body;

	// Token: 0x04001A5E RID: 6750
	private Collider2D col;

	// Token: 0x04001A5F RID: 6751
	private SpriteFlash spriteFlash;

	// Token: 0x04001A60 RID: 6752
	private float pauseTimer;

	// Token: 0x04001A61 RID: 6753
	private float xScale;

	// Token: 0x04001A62 RID: 6754
	private float startX;

	// Token: 0x04001A63 RID: 6755
	private float startY;

	// Token: 0x04001A64 RID: 6756
	private float accelY;

	// Token: 0x04001A65 RID: 6757
	private float accelX;

	// Token: 0x04001A66 RID: 6758
	private float waitTime;

	// Token: 0x020015E5 RID: 5605
	[Serializable]
	public struct TypeDetails
	{
		// Token: 0x04008902 RID: 35074
		public int damage;

		// Token: 0x04008903 RID: 35075
		public AudioEvent birthSound;

		// Token: 0x04008904 RID: 35076
		public Color spatterColor;

		// Token: 0x04008905 RID: 35077
		public bool dung;

		// Token: 0x04008906 RID: 35078
		public Transform groundPoint;

		// Token: 0x04008907 RID: 35079
		public string attackAnim;

		// Token: 0x04008908 RID: 35080
		public string flyAnim;

		// Token: 0x04008909 RID: 35081
		public string hatchAnim;

		// Token: 0x0400890A RID: 35082
		public string teleEndAnim;

		// Token: 0x0400890B RID: 35083
		public string teleStartAnim;

		// Token: 0x0400890C RID: 35084
		public string turnAttackAnim;

		// Token: 0x0400890D RID: 35085
		public string turnFlyAnim;

		// Token: 0x0400890E RID: 35086
		public string restStartAnim;

		// Token: 0x0400890F RID: 35087
		public string restEndAnim;
	}

	// Token: 0x020015E6 RID: 5606
	public enum State
	{
		// Token: 0x04008911 RID: 35089
		None,
		// Token: 0x04008912 RID: 35090
		Follow,
		// Token: 0x04008913 RID: 35091
		Tele,
		// Token: 0x04008914 RID: 35092
		Attack,
		// Token: 0x04008915 RID: 35093
		BenchRestStart,
		// Token: 0x04008916 RID: 35094
		BenchRestLower,
		// Token: 0x04008917 RID: 35095
		BenchResting
	}
}
