using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000328 RID: 808
public class WalkerV2 : MonoBehaviour
{
	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06001C49 RID: 7241 RVA: 0x00083A9A File Offset: 0x00081C9A
	// (set) Token: 0x06001C4A RID: 7242 RVA: 0x00083AA2 File Offset: 0x00081CA2
	public float RightDirection
	{
		get
		{
			return this.rightDirection;
		}
		set
		{
			if (value == 0f)
			{
				this.rightDirection = 1f;
				return;
			}
			this.rightDirection = Mathf.Sign(value);
		}
	}

	// Token: 0x06001C4B RID: 7243 RVA: 0x00083AC4 File Offset: 0x00081CC4
	private void OnDrawGizmosSelected()
	{
		if (!this.box)
		{
			this.box = base.GetComponent<BoxCollider2D>();
		}
		if (!this.box)
		{
			return;
		}
		this.IsRaysHittingWall(true);
		this.IsRaysHittingGroundFront(true);
		this.IsRaysHittingGroundCentre(true);
	}

	// Token: 0x06001C4C RID: 7244 RVA: 0x00083B10 File Offset: 0x00081D10
	private void OnValidate()
	{
		this.RightDirection = this.rightDirection;
	}

	// Token: 0x06001C4D RID: 7245 RVA: 0x00083B1E File Offset: 0x00081D1E
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.box = base.GetComponent<BoxCollider2D>();
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x06001C4E RID: 7246 RVA: 0x00083B44 File Offset: 0x00081D44
	private void Start()
	{
		if (this.hero)
		{
			return;
		}
		this.hero = HeroController.instance.transform;
		if (this.StartActive)
		{
			this.StartWalking();
			return;
		}
		this.StopWalking();
	}

	// Token: 0x06001C4F RID: 7247 RVA: 0x00083B79 File Offset: 0x00081D79
	private void OnDisable()
	{
		if (this.walkRoutine != null)
		{
			base.StopCoroutine(this.walkRoutine);
			this.walkRoutine = null;
		}
	}

	// Token: 0x06001C50 RID: 7248 RVA: 0x00083B98 File Offset: 0x00081D98
	public void StartWalking()
	{
		if (!this.hero)
		{
			this.hero = HeroController.instance.transform;
		}
		if (this.walkRoutine != null)
		{
			return;
		}
		this.moveDirection = this.GetFacingDirection();
		this.walkRoutine = base.StartCoroutine(this.Walking());
	}

	// Token: 0x06001C51 RID: 7249 RVA: 0x00083BEC File Offset: 0x00081DEC
	public void StopWalking()
	{
		if (this.walkRoutine != null)
		{
			base.StopCoroutine(this.walkRoutine);
		}
		this.walkRoutine = null;
		this.isMoving = false;
		if (this.walkAudioSource)
		{
			this.walkAudioSource.Stop();
		}
		if (this.runAudioSource)
		{
			this.runAudioSource.Stop();
		}
		this.body.SetVelocity(new float?(0f), null);
		WalkerV2.UnityBoolEvent onWalking = this.OnWalking;
		if (onWalking != null)
		{
			onWalking.Invoke(false);
		}
		WalkerV2.UnityBoolEvent onRunning = this.OnRunning;
		if (onRunning == null)
		{
			return;
		}
		onRunning.Invoke(false);
	}

	// Token: 0x06001C52 RID: 7250 RVA: 0x00083C8C File Offset: 0x00081E8C
	private IEnumerator Walking()
	{
		for (;;)
		{
			this.isMoving = true;
			if (this.walkAudioSource)
			{
				this.walkAudioSource.Play();
			}
			float restDuration = this.RestCooldownTime.GetRandomValue();
			double nextRestTime = Time.timeAsDouble + (double)restDuration;
			this.wasInAggro = this.IsInAggro();
			this.SetMoving(this.moveDirection);
			for (;;)
			{
				if (this.GetFacingDirection() != this.moveDirection && !this.isTurnAnimPlaying)
				{
					this.FlipScale();
				}
				if (Time.timeAsDouble >= nextRestTime && restDuration > 0f)
				{
					break;
				}
				if (this.isTurnAnimPlaying && !this.animator.IsPlaying(this.TurnAnim))
				{
					this.isTurnAnimPlaying = false;
					this.PlayMoveAnim();
					if (this.queuedTurnFlip && !this.FlipBeforeTurn)
					{
						this.queuedTurnFlip = false;
						this.FlipScale();
					}
				}
				if (((this.IsRaysHittingWall(false) || (!this.IsRaysHittingGroundFront(false) && this.IsRaysHittingGroundCentre(false))) && Time.timeAsDouble >= this.nextTurnTime) || this.forceTurn)
				{
					this.forceTurn = false;
					this.nextTurnTime = Time.timeAsDouble + (double)this.TurnCooldown;
					this.moveDirection = -this.moveDirection;
					this.wasInAggro = this.IsInAggro();
					this.SetMoving(this.moveDirection);
				}
				if (this.IsInAggro() && !this.isTurnAnimPlaying)
				{
					nextRestTime = Time.timeAsDouble + (double)restDuration;
					float directionToHero = Mathf.Sign(this.hero.position.x - base.transform.position.x);
					if (this.moveDirection != directionToHero)
					{
						this.body.SetVelocity(new float?(0f), null);
						this.wasInAggro = false;
						yield return new WaitForSeconds(this.SetFacing(directionToHero));
						if (this.queuedTurnFlip && !this.FlipBeforeTurn)
						{
							this.queuedTurnFlip = false;
							this.FlipScale();
						}
						this.moveDirection = directionToHero;
					}
					if (!this.wasInAggro)
					{
						this.wasInAggro = true;
						if (this.walkAudioSource)
						{
							this.walkAudioSource.Stop();
						}
						if (this.OnWalking != null)
						{
							this.OnWalking.Invoke(false);
						}
						this.aggroSound.SpawnAndPlayOneShot(base.transform.position, null);
						this.body.SetVelocity(new float?(0f), null);
						yield return base.StartCoroutine(this.animator.PlayAnimWait(this.StartleAnim, null));
						this.SetMoving(this.moveDirection);
					}
				}
				yield return null;
			}
			this.body.SetVelocity(new float?(0f), null);
			this.isMoving = false;
			if (this.walkAudioSource)
			{
				this.walkAudioSource.Stop();
			}
			if (this.runAudioSource)
			{
				this.runAudioSource.Stop();
			}
			if (this.OnWalking != null)
			{
				this.OnWalking.Invoke(false);
			}
			if (this.OnRunning != null)
			{
				this.OnRunning.Invoke(false);
			}
			this.animator.Play(this.IdleAnim);
			for (float restTime = this.RestPauseTime.GetRandomValue(); restTime > 0f; restTime -= Time.deltaTime)
			{
				if (this.IsInAggro())
				{
					break;
				}
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06001C53 RID: 7251 RVA: 0x00083C9C File Offset: 0x00081E9C
	private void PlayMoveAnim()
	{
		string name = this.wasInAggro ? this.RunAnim : this.WalkAnim;
		if (!this.animator.IsPlaying(name))
		{
			this.animator.Play(name);
		}
	}

	// Token: 0x06001C54 RID: 7252 RVA: 0x00083CDC File Offset: 0x00081EDC
	private void SetMoving(float direction)
	{
		this.SetFacing(direction);
		float num;
		if (this.wasInAggro)
		{
			num = this.RunSpeed;
			if (this.walkAudioSource)
			{
				this.walkAudioSource.Stop();
			}
			if (this.runAudioSource)
			{
				this.runAudioSource.Play();
			}
		}
		else
		{
			num = this.WalkSpeed;
			if (this.runAudioSource)
			{
				this.runAudioSource.Stop();
			}
			if (this.walkAudioSource)
			{
				this.walkAudioSource.Play();
			}
		}
		if (this.OnWalking != null)
		{
			this.OnWalking.Invoke(!this.wasInAggro);
		}
		if (this.OnRunning != null)
		{
			this.OnRunning.Invoke(this.wasInAggro);
		}
		this.body.SetVelocity(new float?(num * direction), null);
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x00083DBA File Offset: 0x00081FBA
	private bool IsInAggro()
	{
		return this.AggroRange && this.AggroRange.IsHeroInRange() && Time.timeAsDouble > this.nextAggroTime;
	}

	// Token: 0x06001C56 RID: 7254 RVA: 0x00083DE8 File Offset: 0x00081FE8
	private float SetFacing(float direction)
	{
		float num = 0f;
		direction = Mathf.Sign(direction);
		if (this.GetFacingDirection() != direction)
		{
			num = this.PlayTurnAnim();
			this.nextAggroTime = Time.timeAsDouble + (double)num + (double)this.TurnAggroCooldown.GetRandomValue();
		}
		else
		{
			this.animator.Play(this.WalkAnim);
		}
		if (this.queuedTurnFlip && this.FlipBeforeTurn)
		{
			this.queuedTurnFlip = false;
			this.FlipScale();
		}
		return num;
	}

	// Token: 0x06001C57 RID: 7255 RVA: 0x00083E5F File Offset: 0x0008205F
	private float PlayTurnAnim()
	{
		this.queuedTurnFlip = true;
		this.isTurnAnimPlaying = true;
		return this.animator.PlayAnimGetTime(this.TurnAnim);
	}

	// Token: 0x06001C58 RID: 7256 RVA: 0x00083E80 File Offset: 0x00082080
	private void FlipScale()
	{
		base.transform.SetScaleX(-base.transform.localScale.x);
	}

	// Token: 0x06001C59 RID: 7257 RVA: 0x00083E9E File Offset: 0x0008209E
	private float GetFacingDirection()
	{
		return Mathf.Sign(base.transform.localScale.x) * this.rightDirection;
	}

	// Token: 0x06001C5A RID: 7258 RVA: 0x00083EBC File Offset: 0x000820BC
	private bool IsRaysHittingWall(bool isDrawingGizmos = false)
	{
		float movingDirection = this.GetMovingDirection();
		Bounds bounds = this.box.bounds;
		Vector2 vector = bounds.max;
		Vector2 vector2 = bounds.min;
		Vector2 direction = (movingDirection > 0f) ? Vector2.right : Vector2.left;
		if (movingDirection < 0f)
		{
			vector.x = vector2.x;
		}
		else
		{
			vector2.x = vector.x;
		}
		vector.x -= 0.1f * movingDirection;
		vector2.x -= 0.1f * movingDirection;
		vector.y -= 0.5f;
		vector2.y += 0.5f;
		float length = (this.body ? Mathf.Max(this.WallDistance, this.body.linearVelocity.x * Time.fixedDeltaTime) : this.WallDistance) + 0.1f;
		if (isDrawingGizmos)
		{
			return false;
		}
		bool flag = global::Helper.IsRayHittingNoTriggers(vector, direction, length, 33024);
		bool flag2 = global::Helper.IsRayHittingNoTriggers(vector2, direction, length, 33024);
		return flag || flag2;
	}

	// Token: 0x06001C5B RID: 7259 RVA: 0x00083FDC File Offset: 0x000821DC
	private float GetMovingDirection()
	{
		if (!this.body || this.body.linearVelocity.x == 0f)
		{
			return this.GetFacingDirection();
		}
		return Mathf.Sign(this.body.linearVelocity.x);
	}

	// Token: 0x06001C5C RID: 7260 RVA: 0x0008402C File Offset: 0x0008222C
	private bool IsRaysHittingGroundFront(bool isDrawingGizmos = false)
	{
		float movingDirection = this.GetMovingDirection();
		Bounds bounds = this.box.bounds;
		Vector2 vector = bounds.min;
		Vector2 vector2 = bounds.max;
		Vector2 vector3 = bounds.center;
		if (movingDirection > 0f)
		{
			vector3.x = vector2.x + this.GroundAheadDistance;
		}
		else
		{
			vector3.x = vector.x - this.GroundAheadDistance;
		}
		float length = vector3.y - vector.y + 0.5f;
		return !isDrawingGizmos && global::Helper.IsRayHittingNoTriggers(vector3, Vector2.down, length, 33024);
	}

	// Token: 0x06001C5D RID: 7261 RVA: 0x000840D0 File Offset: 0x000822D0
	private bool IsRaysHittingGroundCentre(bool isDrawingGizmos = false)
	{
		this.GetMovingDirection();
		Bounds bounds = this.box.bounds;
		Vector2 vector = bounds.min;
		Vector2 vector2 = bounds.center;
		float num = vector2.y - vector.y + 0.5f;
		if (isDrawingGizmos)
		{
			Gizmos.color = (this.isMoving ? Color.yellow : Color.green);
			Gizmos.DrawLine(vector2, vector2 + Vector2.down * num);
			return false;
		}
		return global::Helper.IsRayHittingNoTriggers(vector2, Vector2.down, num, 33024);
	}

	// Token: 0x06001C5E RID: 7262 RVA: 0x0008416E File Offset: 0x0008236E
	public void ForceDirection(float direction)
	{
		if (direction == 0f)
		{
			return;
		}
		if (Mathf.Sign(this.GetMovingDirection()) != Mathf.Sign(direction))
		{
			this.forceTurn = true;
		}
	}

	// Token: 0x04001B53 RID: 6995
	private const float SKIN_WIDTH = 0.1f;

	// Token: 0x04001B54 RID: 6996
	private const float TOP_RAY_PADDING = 0.5f;

	// Token: 0x04001B55 RID: 6997
	private const float BOTTOM_RAY_PADDING = 0.5f;

	// Token: 0x04001B56 RID: 6998
	private const float DOWN_RAY_DISTANCE = 0.5f;

	// Token: 0x04001B57 RID: 6999
	private const int LAYERMASK = 33024;

	// Token: 0x04001B58 RID: 7000
	public bool StartActive = true;

	// Token: 0x04001B59 RID: 7001
	public string IdleAnim;

	// Token: 0x04001B5A RID: 7002
	public string TurnAnim;

	// Token: 0x04001B5B RID: 7003
	public float WallDistance = 1f;

	// Token: 0x04001B5C RID: 7004
	public float GroundAheadDistance = 0.5f;

	// Token: 0x04001B5D RID: 7005
	public float TurnCooldown;

	// Token: 0x04001B5E RID: 7006
	public string WalkAnim;

	// Token: 0x04001B5F RID: 7007
	public float WalkSpeed;

	// Token: 0x04001B60 RID: 7008
	public bool FlipBeforeTurn;

	// Token: 0x04001B61 RID: 7009
	[Space]
	public string StartleAnim;

	// Token: 0x04001B62 RID: 7010
	public string RunAnim;

	// Token: 0x04001B63 RID: 7011
	public float RunSpeed;

	// Token: 0x04001B64 RID: 7012
	[Space]
	public MinMaxFloat RestPauseTime;

	// Token: 0x04001B65 RID: 7013
	public MinMaxFloat RestCooldownTime;

	// Token: 0x04001B66 RID: 7014
	[SerializeField]
	private AudioSource walkAudioSource;

	// Token: 0x04001B67 RID: 7015
	[SerializeField]
	private AudioSource runAudioSource;

	// Token: 0x04001B68 RID: 7016
	[SerializeField]
	private float rightDirection = -1f;

	// Token: 0x04001B69 RID: 7017
	[Space]
	public AlertRange AggroRange;

	// Token: 0x04001B6A RID: 7018
	public MinMaxFloat TurnAggroCooldown = new MinMaxFloat(0.5f, 1.5f);

	// Token: 0x04001B6B RID: 7019
	[SerializeField]
	private AudioEventRandom aggroSound;

	// Token: 0x04001B6C RID: 7020
	[Space]
	public WalkerV2.UnityBoolEvent OnWalking;

	// Token: 0x04001B6D RID: 7021
	public WalkerV2.UnityBoolEvent OnRunning;

	// Token: 0x04001B6E RID: 7022
	private Coroutine walkRoutine;

	// Token: 0x04001B6F RID: 7023
	private bool isMoving;

	// Token: 0x04001B70 RID: 7024
	private bool queuedTurnFlip;

	// Token: 0x04001B71 RID: 7025
	private bool isTurnAnimPlaying;

	// Token: 0x04001B72 RID: 7026
	private float moveDirection;

	// Token: 0x04001B73 RID: 7027
	private bool wasInAggro;

	// Token: 0x04001B74 RID: 7028
	private bool forceTurn;

	// Token: 0x04001B75 RID: 7029
	private double nextTurnTime;

	// Token: 0x04001B76 RID: 7030
	private double nextAggroTime;

	// Token: 0x04001B77 RID: 7031
	private Transform hero;

	// Token: 0x04001B78 RID: 7032
	private tk2dSpriteAnimator animator;

	// Token: 0x04001B79 RID: 7033
	private BoxCollider2D box;

	// Token: 0x04001B7A RID: 7034
	private Rigidbody2D body;

	// Token: 0x020015F9 RID: 5625
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}
}
