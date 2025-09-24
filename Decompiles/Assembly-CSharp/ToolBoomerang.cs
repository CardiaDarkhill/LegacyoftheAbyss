using System;
using System.Runtime.CompilerServices;
using GlobalSettings;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020003C0 RID: 960
[RequireComponent(typeof(Rigidbody2D))]
public class ToolBoomerang : MonoBehaviour, IHitResponder, ITinkResponder
{
	// Token: 0x06002051 RID: 8273 RVA: 0x00093458 File Offset: 0x00091658
	[UsedImplicitly]
	private bool? CheckAnim(string animName)
	{
		if (!this.animator)
		{
			return null;
		}
		return new bool?(this.animator.GetClipByName(animName) != null);
	}

	// Token: 0x06002052 RID: 8274 RVA: 0x00093490 File Offset: 0x00091690
	private void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
		}
		Vector2 from = Application.isPlaying ? this.initialPosition : Vector2.zero;
		Vector2 to = Application.isPlaying ? this.targetPosition : this.flyOutPosition;
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.flyOutPosition, 0.2f);
		Gizmos.color = Color.yellow;
		for (float num = 0.05f; num <= 1f; num += 0.05f)
		{
			Gizmos.DrawWireSphere(this.SamplePosAtTime(from, to, num), 0.1f);
		}
		Gizmos.color = Color.red;
		for (float num2 = 1f; num2 <= 2f; num2 += 0.05f)
		{
			Gizmos.DrawWireSphere(this.SamplePosAtTime(from, to, num2), 0.1f);
		}
		Vector2 v = this.SamplePosAtTime(from, to, this.flyOutReturnPoint);
		Gizmos.color = new Color(1f, 0.5f, 0f);
		Gizmos.DrawWireSphere(v, 0.2f);
	}

	// Token: 0x06002053 RID: 8275 RVA: 0x000935A8 File Offset: 0x000917A8
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.collider = base.GetComponent<Collider2D>();
		this.audioSource = base.GetComponent<AudioSource>();
		this.poisonTint = Gameplay.PoisonPouchTintColour;
		this.damager.WillDamageEnemyOptions += this.OnWillDamageEnemy;
		this.damager.DamagedEnemy += this.OnDamagedEnemy;
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x00093614 File Offset: 0x00091814
	private void OnEnable()
	{
		this.doSetup = true;
		this.ResetStuckDetection();
		this.body.isKinematic = true;
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
		this.doFlattenVelocity = false;
		this.audioSource.Play();
		this.initialDamageMult = this.damager.magnitudeMult;
		this.initialDamage = this.damager.damageDealt;
		this.childStickEffect.SetAllActive(false);
		this.childBreakEffects.SetAllActive(false);
		if (this.renderer)
		{
			this.renderer.enabled = true;
		}
		this.currentState = ToolBoomerang.States.Translating;
		this.stuckCollider = null;
		this.initialScale = base.transform.localScale;
		this.damagerCollider.enabled = true;
		this.CheckPoison();
	}

	// Token: 0x06002055 RID: 8277 RVA: 0x000936F0 File Offset: 0x000918F0
	private void Start()
	{
		this.CheckPoison();
	}

	// Token: 0x06002056 RID: 8278 RVA: 0x000936F8 File Offset: 0x000918F8
	private void CheckPoison()
	{
		tk2dSprite component = base.GetComponent<tk2dSprite>();
		if (Gameplay.PoisonPouchTool.IsEquipped)
		{
			if (this.getTintFrom)
			{
				component.EnableKeyword("CAN_HUESHIFT");
				component.SetFloat(PoisonTintBase.HueShiftPropId, this.getTintFrom.PoisonHueShift);
			}
			else
			{
				component.EnableKeyword("RECOLOUR");
				component.color = this.poisonTint;
			}
			this.ptBreak.main.startColor = this.poisonTint;
			this.ptPoisonIdle.Play();
			return;
		}
		component.DisableKeyword("CAN_HUESHIFT");
		component.DisableKeyword("RECOLOUR");
		component.color = Color.white;
		this.ptBreak.main.startColor = Color.white;
	}

	// Token: 0x06002057 RID: 8279 RVA: 0x000937C7 File Offset: 0x000919C7
	private void OnDisable()
	{
		base.transform.localScale = this.initialScale;
		this.wasNailHit = false;
		this.damager.magnitudeMult = this.initialDamageMult;
		this.damager.damageDealt = this.initialDamage;
	}

	// Token: 0x06002058 RID: 8280 RVA: 0x00093804 File Offset: 0x00091A04
	private void FixedUpdate()
	{
		if (this.bounceCoolDown > 0f)
		{
			this.bounceCoolDown -= Time.fixedDeltaTime;
			if (this.bounceCoolDown <= 0f && this.damager)
			{
				this.damager.damageDealt = this.initialDamage;
			}
		}
		if (this.doSetup)
		{
			this.doSetup = false;
			bool flag = base.transform.localScale.x > 0f;
			float randomValue = this.startRotation.GetRandomValue();
			base.transform.SetRotation2D(flag ? randomValue : (-randomValue));
			this.damager.direction = (float)(flag ? 0 : 180);
			this.damager.enabled = true;
			if (this.animator)
			{
				this.animator.Play(this.flyAnim);
			}
			this.initialPosition = base.transform.position;
			this.previousPosition = this.initialPosition;
			this.targetPosition = base.transform.TransformPoint(this.flyOutPosition);
			this.elapsedTime = 0f;
			this.currentDamageVelocityLerpTime = this.damageVelocityLerpTime;
			this.currentState = ToolBoomerang.States.Translating;
		}
		switch (this.currentState)
		{
		case ToolBoomerang.States.Translating:
			this.UpdateTranslating();
			return;
		case ToolBoomerang.States.Body:
			this.UpdateBody();
			return;
		case ToolBoomerang.States.Stuck:
			this.UpdateStuck();
			return;
		case ToolBoomerang.States.Broken:
			this.UpdateBroken();
			return;
		default:
			return;
		}
	}

	// Token: 0x06002059 RID: 8281 RVA: 0x00093980 File Offset: 0x00091B80
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer != 8)
		{
			return;
		}
		ToolBoomerang.States states = this.currentState;
		if (states != ToolBoomerang.States.Translating)
		{
			if (states == ToolBoomerang.States.Body)
			{
				this.stuckCollider = collision.collider;
				this.StickInWall(collision);
				return;
			}
		}
		else if (this.bounceCoolDown <= 0f)
		{
			if (this.elapsedTime / this.flyOutCurveDuration >= this.flyOutReturnPoint)
			{
				this.stuckCollider = collision.collider;
				this.StickInWall(collision);
				return;
			}
			this.HitWall();
			this.BounceOffWall();
		}
	}

	// Token: 0x0600205A RID: 8282 RVA: 0x00093A04 File Offset: 0x00091C04
	private ToolBoomerang.DirectionPos GetClosestWall()
	{
		CircleCollider2D circle = (CircleCollider2D)this.collider;
		if (circle)
		{
			Vector2 pos = base.transform.TransformPoint(circle.offset);
			float radius = base.transform.TransformRadius(circle.radius);
			return this.RunCastForDirections((Vector2 dir) => Physics2D.CircleCast(pos - dir * circle.radius, radius, dir, radius));
		}
		Debug.LogError("Collider type not implemented!", this);
		return ToolBoomerang.DirectionPos.GetDefault(base.transform);
	}

	// Token: 0x0600205B RID: 8283 RVA: 0x00093AA0 File Offset: 0x00091CA0
	private ToolBoomerang.DirectionPos RunCastForDirections(Func<Vector2, RaycastHit2D> func)
	{
		RaycastHit2D raycastHit2D = func(Vector2.right);
		if (raycastHit2D.collider != null)
		{
			return new ToolBoomerang.DirectionPos
			{
				Direction = ToolBoomerang.Direction.Right,
				Position = raycastHit2D.point
			};
		}
		raycastHit2D = func(Vector2.left);
		if (raycastHit2D.collider != null)
		{
			return new ToolBoomerang.DirectionPos
			{
				Direction = ToolBoomerang.Direction.Left,
				Position = raycastHit2D.point
			};
		}
		raycastHit2D = func(Vector2.up);
		if (raycastHit2D.collider != null)
		{
			return new ToolBoomerang.DirectionPos
			{
				Direction = ToolBoomerang.Direction.Up,
				Position = raycastHit2D.point
			};
		}
		raycastHit2D = func(Vector2.down);
		if (raycastHit2D.collider != null)
		{
			return new ToolBoomerang.DirectionPos
			{
				Direction = ToolBoomerang.Direction.Down,
				Position = raycastHit2D.point
			};
		}
		return ToolBoomerang.DirectionPos.GetDefault(base.transform);
	}

	// Token: 0x0600205C RID: 8284 RVA: 0x00093BA4 File Offset: 0x00091DA4
	private ToolBoomerang.Direction HitWall()
	{
		ToolBoomerang.DirectionPos closestWall = this.GetClosestWall();
		if (this.wallHitPrefab)
		{
			this.wallHitPrefab.Spawn(closestWall.Position);
		}
		return closestWall.Direction;
	}

	// Token: 0x0600205D RID: 8285 RVA: 0x00093BE4 File Offset: 0x00091DE4
	private void StickInWall(Collision2D collision)
	{
		ToolBoomerang.Direction direction = this.HitWall();
		bool flag = base.transform.localScale.x > 0f;
		this.damager.enabled = false;
		this.damagerCollider.enabled = false;
		float randomValue = this.stickRotation.GetRandomValue();
		base.transform.SetRotation2D(flag ? randomValue : (-randomValue));
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z + 0.006f);
		if (this.animator)
		{
			this.animator.Play(this.stickAnim);
		}
		this.childStickEffect.SetAllActive(true);
		if ((flag && collision.relativeVelocity.x > 0f) || (!flag && collision.relativeVelocity.x < 0f))
		{
			base.transform.FlipLocalScale(true, false, false);
		}
		if ((direction == ToolBoomerang.Direction.Up && base.transform.localScale.y > 0f) || (direction == ToolBoomerang.Direction.Down && base.transform.localScale.y < 0f))
		{
			base.transform.FlipLocalScale(false, true, false);
		}
		this.currentState = ToolBoomerang.States.Stuck;
		this.elapsedTime = 0f;
		this.body.isKinematic = true;
		this.audioSource.Stop();
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
	}

	// Token: 0x0600205E RID: 8286 RVA: 0x00093D7C File Offset: 0x00091F7C
	private void Stuck()
	{
		ToolBoomerang.Direction direction = this.HitWall();
		bool flag = base.transform.localScale.x > 0f;
		this.damager.enabled = false;
		this.damagerCollider.enabled = false;
		float randomValue = this.stickRotation.GetRandomValue();
		base.transform.SetRotation2D(flag ? randomValue : (-randomValue));
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z + 0.006f);
		if (this.animator)
		{
			this.animator.Play(this.stickAnim);
		}
		this.childStickEffect.SetAllActive(true);
		if ((direction == ToolBoomerang.Direction.Up && base.transform.localScale.y > 0f) || (direction == ToolBoomerang.Direction.Down && base.transform.localScale.y < 0f))
		{
			base.transform.FlipLocalScale(false, true, false);
		}
		this.currentState = ToolBoomerang.States.Stuck;
		this.elapsedTime = 0f;
		this.body.isKinematic = true;
		this.audioSource.Stop();
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
	}

	// Token: 0x0600205F RID: 8287 RVA: 0x00093EDC File Offset: 0x000920DC
	private void BounceOffWall()
	{
		Vector2 b = this.SamplePosAtTime(this.initialPosition, this.targetPosition, this.flyOutReturnPoint);
		Vector2 b2 = base.transform.position - b;
		this.initialPosition += b2;
		this.targetPosition += b2;
		this.elapsedTime = this.flyOutReturnPoint * this.flyOutCurveDuration;
		if (this.damager)
		{
			this.damager.damageDealt = 0;
		}
		this.bounceCoolDown = 0.1f;
	}

	// Token: 0x06002060 RID: 8288 RVA: 0x00093F74 File Offset: 0x00092174
	private void UpdateTranslating()
	{
		float t = this.elapsedTime / this.flyOutCurveDuration;
		Vector2 vector = this.SamplePosAtTime(this.initialPosition, this.targetPosition, t);
		if (this.previousPosition != vector)
		{
			Vector2 vector2 = vector - this.previousPosition;
			if (this.flyOutPosition.x > this.flyOutPosition.y)
			{
				this.damager.direction = (float)((vector2.x > 0f) ? 0 : 180);
			}
			else
			{
				this.damager.direction = (float)((vector2.y > 0f) ? 90 : 270);
			}
		}
		if (this.elapsedTime >= this.flyOutCurveDuration)
		{
			this.StartBodyControl();
			return;
		}
		this.previousPosition = this.body.position;
		this.body.MovePosition(vector);
		float num = this.body.rotation;
		num += ((base.transform.localScale.x > 0f) ? this.flySpinSpeed : (-this.flySpinSpeed)) * Time.deltaTime;
		this.body.MoveRotation(num);
		this.elapsedTime += Time.deltaTime * this.GetSpeedMult();
	}

	// Token: 0x06002061 RID: 8289 RVA: 0x000940B0 File Offset: 0x000922B0
	private void StartBodyControl()
	{
		bool flag = this.currentState != ToolBoomerang.States.Translating;
		this.currentState = ToolBoomerang.States.Body;
		this.body.isKinematic = false;
		if (!flag)
		{
			float num = Mathf.Clamp01(this.elapsedTime / this.flyOutCurveDuration);
			if (num > 0.01f)
			{
				float t = num - 0.01f;
				float d = 0.01f * this.flyOutCurveDuration;
				Vector2 a = this.SamplePosAtTime(this.initialPosition, this.targetPosition, num);
				Vector2 b = this.SamplePosAtTime(this.initialPosition, this.targetPosition, t);
				Vector2 vector = (a - b) / d;
				this.bodyVelocity = vector;
			}
		}
		this.body.angularVelocity = ((base.transform.localScale.x > 0f) ? this.flySpinSpeed : (-this.flySpinSpeed));
	}

	// Token: 0x06002062 RID: 8290 RVA: 0x00094178 File Offset: 0x00092378
	private void UpdateStuck()
	{
		if (this.elapsedTime >= this.stuckIdleDuration || (this.stuckCollider && !this.stuckCollider.isActiveAndEnabled))
		{
			this.Break();
		}
		this.elapsedTime += Time.deltaTime;
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x000941C5 File Offset: 0x000923C5
	private void UpdateBroken()
	{
		if (Time.timeAsDouble >= this.recycleTime)
		{
			base.gameObject.Recycle();
		}
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x000941E0 File Offset: 0x000923E0
	private void UpdateBody()
	{
		if (this.doFlattenVelocity)
		{
			this.bodyVelocity.y = Mathf.Lerp(this.bodyVelocity.y, 0f, this.horizontalKnockbackFlattenSpeed * Time.deltaTime);
		}
		this.body.linearVelocity = this.bodyVelocity * this.GetSpeedMult();
		if (this.CheckStuck())
		{
			this.Stuck();
		}
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x0009424C File Offset: 0x0009244C
	private bool CheckStuck()
	{
		ToolBoomerang.<>c__DisplayClass81_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.currentPosition = this.body.position;
		if (this.positionCount < 3)
		{
			this.positionCount++;
			this.<CheckStuck>g__AddCurrentPosition|81_0(ref CS$<>8__locals1);
			return false;
		}
		if ((this.positionHistory[this.positionIndex] - CS$<>8__locals1.currentPosition).sqrMagnitude > 0.0225f)
		{
			this.<CheckStuck>g__AddCurrentPosition|81_0(ref CS$<>8__locals1);
			return false;
		}
		return true;
	}

	// Token: 0x06002066 RID: 8294 RVA: 0x000942CA File Offset: 0x000924CA
	public void ResetStuckDetection()
	{
		this.positionIndex = 0;
		this.positionCount = 0;
	}

	// Token: 0x06002067 RID: 8295 RVA: 0x000942DA File Offset: 0x000924DA
	private float GetSpeedMult()
	{
		this.currentDamageVelocityLerpTime += Time.deltaTime;
		return this.damageVelocityLerpCurve.Evaluate(Mathf.Clamp01(this.currentDamageVelocityLerpTime / this.damageVelocityLerpTime));
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x0009430B File Offset: 0x0009250B
	private void OnDamagedEnemy()
	{
		this.currentDamageVelocityLerpTime = 0f;
		if (!this.wasNailHit)
		{
			return;
		}
		this.Break();
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x00094328 File Offset: 0x00092528
	public void Break()
	{
		this.body.isKinematic = true;
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
		this.childBreakEffects.SetAllActive(true);
		if (this.recycleDelay <= 0f)
		{
			base.gameObject.Recycle();
		}
		else
		{
			this.recycleTime = Time.timeAsDouble + (double)this.recycleDelay;
			if (this.renderer)
			{
				this.renderer.enabled = false;
			}
		}
		this.damager.enabled = false;
		this.audioSource.Stop();
		this.currentState = ToolBoomerang.States.Broken;
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x000943D4 File Offset: 0x000925D4
	private Vector2 SamplePosAtTime(Vector2 from, Vector2 to, float t)
	{
		Vector2 zero = Vector2.zero;
		zero.x = Mathf.LerpUnclamped(from.x, to.x, this.flyOutCurveX.Evaluate(t));
		zero.y = Mathf.LerpUnclamped(from.y, to.y, this.flyOutCurveY.Evaluate(t));
		return zero;
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x00094430 File Offset: 0x00092630
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.currentState == ToolBoomerang.States.Broken)
		{
			return IHitResponder.Response.None;
		}
		if (damageInstance.Source && damageInstance.Source.GetComponentInParent<ToolBoomerang>(true))
		{
			return IHitResponder.Response.None;
		}
		if (this.nailHitEffectPrefab)
		{
			this.nailHitEffectPrefab.Spawn(base.transform.position);
		}
		if (this.currentState == ToolBoomerang.States.Stuck)
		{
			return IHitResponder.Response.None;
		}
		if (damageInstance.IsNailDamage)
		{
			bool flag = base.transform.localScale.x > 0f;
			HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.Regular);
			if (hitDirection != HitInstance.HitDirection.Left)
			{
				if (hitDirection == HitInstance.HitDirection.Right)
				{
					this.doSetup = true;
					if (!flag)
					{
						base.transform.FlipLocalScale(true, false, false);
					}
					return IHitResponder.Response.GenericHit;
				}
			}
			else
			{
				this.doSetup = true;
				if (flag)
				{
					base.transform.FlipLocalScale(true, false, false);
				}
			}
			if (this.currentState == ToolBoomerang.States.Translating)
			{
				this.StartBodyControl();
			}
			this.NailKnockBack(damageInstance.GetHitDirection(HitInstance.TargetType.Regular));
			this.nailDeflectClip.SpawnAndPlayOneShot(base.transform.position, null);
		}
		else
		{
			this.WeakKnockBack(damageInstance.GetHitDirection(HitInstance.TargetType.Regular));
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x00094564 File Offset: 0x00092764
	private void NailKnockBack(HitInstance.HitDirection hitDir)
	{
		switch (hitDir)
		{
		case HitInstance.HitDirection.Left:
			this.bodyVelocity = new Vector2(-this.horizontalKnockbackDirection.x, this.horizontalKnockbackDirection.y).normalized * this.knockbackSpeed;
			this.damager.direction = 180f;
			this.doFlattenVelocity = true;
			break;
		case HitInstance.HitDirection.Right:
			this.bodyVelocity = this.horizontalKnockbackDirection.normalized * this.knockbackSpeed;
			this.damager.direction = 0f;
			this.doFlattenVelocity = true;
			break;
		case HitInstance.HitDirection.Up:
			this.bodyVelocity = new Vector2(0f, this.knockbackSpeed);
			this.damager.direction = 90f;
			break;
		case HitInstance.HitDirection.Down:
			if (HeroController.instance.cState.facingRight)
			{
				this.bodyVelocity = this.downKnockbackDirection;
				this.damager.direction = 0f;
			}
			else
			{
				this.bodyVelocity = new Vector2(-this.downKnockbackDirection.x, this.downKnockbackDirection.y);
				this.damager.direction = 180f;
			}
			this.bodyVelocity = this.bodyVelocity.normalized * this.knockbackSpeed;
			break;
		}
		this.body.linearVelocity = this.bodyVelocity * this.GetSpeedMult();
		this.damager.damageDealt = Mathf.RoundToInt((float)this.damager.damageDealt * this.hitDamageMult);
		this.damager.magnitudeMult *= this.hitDamageMult;
		this.wasNailHit = true;
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x00094718 File Offset: 0x00092918
	private void WeakKnockBack(HitInstance.HitDirection hitDir)
	{
		Vector2 zero = Vector2.zero;
		if (hitDir > HitInstance.HitDirection.Up)
		{
			if (hitDir == HitInstance.HitDirection.Down)
			{
				zero.y -= this.weakKnockbackAmount;
			}
		}
		else
		{
			zero.y += this.weakKnockbackAmount;
		}
		this.initialPosition += zero;
		this.targetPosition += zero;
		this.damager.damageDealt = Mathf.RoundToInt((float)this.damager.damageDealt * this.hitDamageMult);
		this.damager.magnitudeMult *= this.hitDamageMult;
	}

	// Token: 0x1700035B RID: 859
	// (get) Token: 0x0600206E RID: 8302 RVA: 0x000947B8 File Offset: 0x000929B8
	public ITinkResponder.TinkFlags ResponderType
	{
		get
		{
			return ITinkResponder.TinkFlags.Projectile;
		}
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x000947BB File Offset: 0x000929BB
	public void Tinked()
	{
		if (this.currentState != ToolBoomerang.States.Translating)
		{
			return;
		}
		this.BounceOffWall();
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x000947CC File Offset: 0x000929CC
	private void OnWillDamageEnemy(HealthManager enemyHealthManager, HitInstance damageInstance)
	{
		if (enemyHealthManager.IsBlockingByDirection(DirectionUtils.GetCardinalDirection(damageInstance.Direction), damageInstance.AttackType, SpecialTypes.None))
		{
			this.Tinked();
		}
	}

	// Token: 0x06002072 RID: 8306 RVA: 0x0009490E File Offset: 0x00092B0E
	[CompilerGenerated]
	private void <CheckStuck>g__AddCurrentPosition|81_0(ref ToolBoomerang.<>c__DisplayClass81_0 A_1)
	{
		this.positionHistory[this.positionIndex] = A_1.currentPosition;
		this.positionIndex = (this.positionIndex + 1) % 3;
	}

	// Token: 0x04001F4C RID: 8012
	[SerializeField]
	private float flySpinSpeed = 10f;

	// Token: 0x04001F4D RID: 8013
	[SerializeField]
	private MinMaxFloat startRotation;

	// Token: 0x04001F4E RID: 8014
	[SerializeField]
	private MinMaxFloat stickRotation;

	// Token: 0x04001F4F RID: 8015
	[Space]
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x04001F50 RID: 8016
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("CheckAnim")]
	private string flyAnim;

	// Token: 0x04001F51 RID: 8017
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("CheckAnim")]
	private string stickAnim;

	// Token: 0x04001F52 RID: 8018
	[Space]
	[SerializeField]
	private Vector2 flyOutPosition = new Vector2(10f, 0f);

	// Token: 0x04001F53 RID: 8019
	[SerializeField]
	private AnimationCurve flyOutCurveX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04001F54 RID: 8020
	[SerializeField]
	private AnimationCurve flyOutCurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04001F55 RID: 8021
	[SerializeField]
	private float flyOutCurveDuration = 1f;

	// Token: 0x04001F56 RID: 8022
	[SerializeField]
	private float stuckIdleDuration = 3f;

	// Token: 0x04001F57 RID: 8023
	[SerializeField]
	[Range(0f, 1f)]
	private float flyOutReturnPoint = 0.5f;

	// Token: 0x04001F58 RID: 8024
	[Space]
	[SerializeField]
	private DamageEnemies damager;

	// Token: 0x04001F59 RID: 8025
	[SerializeField]
	private CircleCollider2D damagerCollider;

	// Token: 0x04001F5A RID: 8026
	[SerializeField]
	private float hitDamageMult = 2f;

	// Token: 0x04001F5B RID: 8027
	[SerializeField]
	private AnimationCurve damageVelocityLerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04001F5C RID: 8028
	[SerializeField]
	private float damageVelocityLerpTime = 0.05f;

	// Token: 0x04001F5D RID: 8029
	[Space]
	[SerializeField]
	private GameObject[] childStickEffect;

	// Token: 0x04001F5E RID: 8030
	[SerializeField]
	private GameObject[] childBreakEffects;

	// Token: 0x04001F5F RID: 8031
	[SerializeField]
	private float recycleDelay;

	// Token: 0x04001F60 RID: 8032
	[SerializeField]
	private MeshRenderer renderer;

	// Token: 0x04001F61 RID: 8033
	[Space]
	[SerializeField]
	private float knockbackSpeed = 30f;

	// Token: 0x04001F62 RID: 8034
	[SerializeField]
	private Vector2 horizontalKnockbackDirection = new Vector2(2f, 1f);

	// Token: 0x04001F63 RID: 8035
	[SerializeField]
	private float horizontalKnockbackFlattenSpeed = 10f;

	// Token: 0x04001F64 RID: 8036
	[SerializeField]
	private Vector2 downKnockbackDirection = new Vector2(1f, -1f);

	// Token: 0x04001F65 RID: 8037
	[SerializeField]
	private float weakKnockbackAmount = 1f;

	// Token: 0x04001F66 RID: 8038
	[SerializeField]
	private AudioEvent nailDeflectClip;

	// Token: 0x04001F67 RID: 8039
	[SerializeField]
	private GameObject nailHitEffectPrefab;

	// Token: 0x04001F68 RID: 8040
	[SerializeField]
	private GameObject wallHitPrefab;

	// Token: 0x04001F69 RID: 8041
	[Header("Poison")]
	[SerializeField]
	private ToolItem getTintFrom;

	// Token: 0x04001F6A RID: 8042
	public ParticleSystem ptPoisonIdle;

	// Token: 0x04001F6B RID: 8043
	public ParticleSystem ptBreak;

	// Token: 0x04001F6C RID: 8044
	private Vector3 initialScale;

	// Token: 0x04001F6D RID: 8045
	private Vector2 initialPosition;

	// Token: 0x04001F6E RID: 8046
	private Vector2 targetPosition;

	// Token: 0x04001F6F RID: 8047
	private Color poisonTint;

	// Token: 0x04001F70 RID: 8048
	private float elapsedTime;

	// Token: 0x04001F71 RID: 8049
	private float currentDamageVelocityLerpTime;

	// Token: 0x04001F72 RID: 8050
	private Vector2 bodyVelocity;

	// Token: 0x04001F73 RID: 8051
	private bool doFlattenVelocity;

	// Token: 0x04001F74 RID: 8052
	private bool wasNailHit;

	// Token: 0x04001F75 RID: 8053
	private int initialDamage;

	// Token: 0x04001F76 RID: 8054
	private float initialDamageMult;

	// Token: 0x04001F77 RID: 8055
	private Vector2 previousPosition;

	// Token: 0x04001F78 RID: 8056
	private bool doSetup;

	// Token: 0x04001F79 RID: 8057
	private double recycleTime;

	// Token: 0x04001F7A RID: 8058
	private float bounceCoolDown;

	// Token: 0x04001F7B RID: 8059
	private ToolBoomerang.States currentState;

	// Token: 0x04001F7C RID: 8060
	private Collider2D stuckCollider;

	// Token: 0x04001F7D RID: 8061
	private Rigidbody2D body;

	// Token: 0x04001F7E RID: 8062
	private Collider2D collider;

	// Token: 0x04001F7F RID: 8063
	private AudioSource audioSource;

	// Token: 0x04001F80 RID: 8064
	private const int STUCK_FRAME_COUNT = 3;

	// Token: 0x04001F81 RID: 8065
	private const float STUCK_RADIUS = 0.15f;

	// Token: 0x04001F82 RID: 8066
	private const float STUCK_RADIUS_SQR = 0.0225f;

	// Token: 0x04001F83 RID: 8067
	private readonly Vector2[] positionHistory = new Vector2[3];

	// Token: 0x04001F84 RID: 8068
	private int positionIndex;

	// Token: 0x04001F85 RID: 8069
	private int positionCount;

	// Token: 0x02001674 RID: 5748
	private enum Direction
	{
		// Token: 0x04008AE6 RID: 35558
		Up,
		// Token: 0x04008AE7 RID: 35559
		Down,
		// Token: 0x04008AE8 RID: 35560
		Left,
		// Token: 0x04008AE9 RID: 35561
		Right
	}

	// Token: 0x02001675 RID: 5749
	private enum States
	{
		// Token: 0x04008AEB RID: 35563
		Translating,
		// Token: 0x04008AEC RID: 35564
		Body,
		// Token: 0x04008AED RID: 35565
		Stuck,
		// Token: 0x04008AEE RID: 35566
		Broken
	}

	// Token: 0x02001676 RID: 5750
	private struct DirectionPos
	{
		// Token: 0x06008A2A RID: 35370 RVA: 0x0027F200 File Offset: 0x0027D400
		public static ToolBoomerang.DirectionPos GetDefault(Transform transform)
		{
			return new ToolBoomerang.DirectionPos
			{
				Direction = ToolBoomerang.Direction.Right,
				Position = transform.position
			};
		}

		// Token: 0x04008AEF RID: 35567
		public ToolBoomerang.Direction Direction;

		// Token: 0x04008AF0 RID: 35568
		public Vector2 Position;
	}
}
