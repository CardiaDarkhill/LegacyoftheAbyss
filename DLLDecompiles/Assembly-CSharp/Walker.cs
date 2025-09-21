using System;
using UnityEngine;

// Token: 0x0200031F RID: 799
public class Walker : MonoBehaviour
{
	// Token: 0x06001C17 RID: 7191 RVA: 0x00082D78 File Offset: 0x00080F78
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.audioSource = base.GetComponent<AudioSource>();
		if (this.animatedChild != null)
		{
			this.animator_child = this.animatedChild.GetComponent<tk2dSpriteAnimator>();
		}
		this.GetCollider();
	}

	// Token: 0x06001C18 RID: 7192 RVA: 0x00082DCE File Offset: 0x00080FCE
	private void GetCollider()
	{
		if (this.bodyCollider)
		{
			return;
		}
		this.bodyCollider = base.GetComponent<Collider2D>();
	}

	// Token: 0x06001C19 RID: 7193 RVA: 0x00082DEC File Offset: 0x00080FEC
	protected void Start()
	{
		this.mainCamera = GameCameras.instance.mainCamera;
		this.hero = HeroController.instance;
		if (this.currentFacing == 0)
		{
			this.currentFacing = this.GetFacingFromScale();
		}
		if (this.state == Walker.States.NotReady)
		{
			this.turnCooldownRemaining = -Mathf.Epsilon;
			this.BeginWaitingForConditions();
		}
	}

	// Token: 0x06001C1A RID: 7194 RVA: 0x00082E42 File Offset: 0x00081042
	private int GetFacingFromScale()
	{
		if (base.transform.localScale.x * this.rightScale < 0f)
		{
			return -1;
		}
		return 1;
	}

	// Token: 0x06001C1B RID: 7195 RVA: 0x00082E68 File Offset: 0x00081068
	private void Update()
	{
		this.turnCooldownRemaining -= Time.deltaTime;
		this.aggroEdgeTurnCooldownRemaining -= Time.deltaTime;
		this.GetCollider();
		switch (this.state)
		{
		case Walker.States.WaitingForConditions:
			this.UpdateWaitingForConditions();
			break;
		case Walker.States.Stopped:
			this.UpdateStopping();
			break;
		case Walker.States.Walking:
			this.UpdateWalking();
			break;
		case Walker.States.Turning:
			this.UpdateTurning();
			break;
		}
		if (this.aggroEdgeTurnCooldown > 0f && this.state == Walker.States.Turning && this.alertRange.IsHeroInRange() && (!this.lineOfSightDetector || this.lineOfSightDetector.CanSeeHero))
		{
			this.aggroEdgeTurnCooldownRemaining = this.aggroEdgeTurnCooldown;
		}
	}

	// Token: 0x06001C1C RID: 7196 RVA: 0x00082F28 File Offset: 0x00081128
	public void StartMoving()
	{
		if (this.state == Walker.States.Stopped || this.state == Walker.States.WaitingForConditions)
		{
			this.startInactive = false;
			int facing;
			if (this.currentFacing == 0)
			{
				facing = ((Random.Range(0, 2) == 0) ? -1 : 1);
			}
			else
			{
				facing = (this.currentFacing = this.GetFacingFromScale());
			}
			this.BeginWalkingOrTurning(facing);
		}
		this.Update();
	}

	// Token: 0x06001C1D RID: 7197 RVA: 0x00082F83 File Offset: 0x00081183
	public void CancelTurn()
	{
		if (this.state == Walker.States.Turning)
		{
			this.BeginWalking(this.currentFacing);
		}
	}

	// Token: 0x06001C1E RID: 7198 RVA: 0x00082F9C File Offset: 0x0008119C
	public void Go(int facing)
	{
		this.turnCooldownRemaining = -Mathf.Epsilon;
		if (this.state == Walker.States.Stopped || this.state == Walker.States.Walking)
		{
			this.BeginWalkingOrTurning(facing);
		}
		else if (this.state == Walker.States.Turning && this.currentFacing == facing)
		{
			this.CancelTurn();
		}
		this.Update();
	}

	// Token: 0x06001C1F RID: 7199 RVA: 0x00082FEE File Offset: 0x000811EE
	public void RecieveGoMessage(int facing)
	{
		if (this.state != Walker.States.Stopped || this.stopReason != Walker.StopReasons.Controlled)
		{
			this.Go(facing);
		}
	}

	// Token: 0x06001C20 RID: 7200 RVA: 0x00083009 File Offset: 0x00081209
	public void Stop(Walker.StopReasons reason)
	{
		this.BeginStopped(reason);
	}

	// Token: 0x06001C21 RID: 7201 RVA: 0x00083012 File Offset: 0x00081212
	public void ChangeFacing(int facing)
	{
		if (this.state == Walker.States.Turning)
		{
			this.turningFacing = facing;
			this.currentFacing = -facing;
			return;
		}
		this.currentFacing = facing;
	}

	// Token: 0x06001C22 RID: 7202 RVA: 0x00083034 File Offset: 0x00081234
	private void BeginWaitingForConditions()
	{
		this.state = Walker.States.WaitingForConditions;
		this.didFulfilCameraDistanceCondition = false;
		this.didFulfilHeroXCondition = false;
		this.UpdateWaitingForConditions();
	}

	// Token: 0x06001C23 RID: 7203 RVA: 0x00083054 File Offset: 0x00081254
	private void UpdateWaitingForConditions()
	{
		if (!this.didFulfilCameraDistanceCondition && (this.mainCamera.transform.position - base.transform.position).sqrMagnitude < 3600f)
		{
			this.didFulfilCameraDistanceCondition = true;
		}
		if (this.didFulfilCameraDistanceCondition && !this.didFulfilHeroXCondition && this.hero != null && Mathf.Abs(this.hero.transform.GetPositionX() - this.waitHeroX) < 1f)
		{
			this.didFulfilHeroXCondition = true;
		}
		if (this.didFulfilCameraDistanceCondition && (!this.waitForHeroX || this.didFulfilHeroXCondition) && !this.startInactive && !this.ambush)
		{
			this.BeginStopped(Walker.StopReasons.Bored);
			this.StartMoving();
		}
	}

	// Token: 0x06001C24 RID: 7204 RVA: 0x0008311C File Offset: 0x0008131C
	private void BeginStopped(Walker.StopReasons reason)
	{
		this.state = Walker.States.Stopped;
		this.stopReason = reason;
		if (this.audioSource)
		{
			this.audioSource.Stop();
		}
		if (reason == Walker.StopReasons.Bored)
		{
			tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName(this.idleClip);
			if (clipByName != null && !this.animator.IsPlaying(clipByName))
			{
				this.animator.Play(clipByName);
			}
			if (!string.IsNullOrEmpty(this.childAnimSuffix) && this.animator_child)
			{
				tk2dSpriteAnimationClip clipByName2 = this.animator_child.GetClipByName(this.idleClip + this.childAnimSuffix);
				if (clipByName2 != null && !this.animator_child.IsPlaying(clipByName2))
				{
					this.animator_child.Play(clipByName2);
				}
			}
			this.body.linearVelocity = Vector2.Scale(this.body.linearVelocity, new Vector2(0f, 1f));
			if (this.pauses)
			{
				this.pauseTimeRemaining = Random.Range(this.pauseTimeMin, this.pauseTimeMax);
				return;
			}
			this.EndStopping();
		}
	}

	// Token: 0x06001C25 RID: 7205 RVA: 0x00083228 File Offset: 0x00081428
	private void UpdateStopping()
	{
		if (this.stopReason == Walker.StopReasons.Bored)
		{
			this.pauseTimeRemaining -= Time.deltaTime;
			if (this.pauseTimeRemaining <= 0f)
			{
				this.EndStopping();
			}
		}
	}

	// Token: 0x06001C26 RID: 7206 RVA: 0x00083258 File Offset: 0x00081458
	private void EndStopping()
	{
		if (this.currentFacing == 0)
		{
			this.BeginWalkingOrTurning((Random.Range(0, 2) == 0) ? 1 : -1);
			return;
		}
		if (Random.Range(0, 100) < this.turnAfterIdlePercentage)
		{
			this.BeginTurning(-this.currentFacing);
			return;
		}
		this.BeginWalking(this.currentFacing);
	}

	// Token: 0x06001C27 RID: 7207 RVA: 0x000832AB File Offset: 0x000814AB
	private void BeginWalkingOrTurning(int facing)
	{
		if (this.currentFacing == facing)
		{
			this.BeginWalking(facing);
			return;
		}
		this.BeginTurning(facing);
	}

	// Token: 0x06001C28 RID: 7208 RVA: 0x000832C8 File Offset: 0x000814C8
	private void BeginWalking(int facing)
	{
		this.state = Walker.States.Walking;
		this.animator.Play(this.walkClip);
		if (!string.IsNullOrEmpty(this.childAnimSuffix) && this.animator_child)
		{
			this.animator_child.Play(this.walkClip + this.childAnimSuffix);
		}
		this.UpdateScale(facing);
		this.walkTimeRemaining = Random.Range(this.pauseWaitMin, this.pauseWaitMax);
		if (this.audioSource)
		{
			this.audioSource.Play();
		}
		this.body.linearVelocity = new Vector2((facing > 0) ? this.walkSpeedR : this.walkSpeedL, this.body.linearVelocity.y);
	}

	// Token: 0x06001C29 RID: 7209 RVA: 0x0008338C File Offset: 0x0008158C
	private void UpdateWalking()
	{
		bool flag = this.turnCooldownRemaining <= 0f;
		bool flag2 = this.aggroEdgeTurnCooldownRemaining <= 0f;
		if (flag)
		{
			Sweep sweep = new Sweep(this.bodyCollider, 1 - this.currentFacing, 3, 0.1f, 0.01f);
			if (sweep.Check(this.bodyCollider.bounds.extents.x + 0.5f, 33024, false))
			{
				this.BeginTurning(-this.currentFacing);
				return;
			}
			if (!this.preventTurningToFaceHero && flag2 && (this.hero != null && this.hero.transform.GetPositionX() > base.transform.GetPositionX() != this.currentFacing > 0) && ((this.lineOfSightDetector != null && this.lineOfSightDetector.CanSeeHero) || this.lineOfSightDetector == null) && this.alertRange != null && this.alertRange.IsHeroInRange())
			{
				this.BeginTurning(-this.currentFacing);
				return;
			}
			if (!this.ignoreHoles)
			{
				Sweep sweep2 = new Sweep(this.bodyCollider, 3, 3, 0.1f, 0.01f);
				float num;
				if (!sweep2.Check(0.25f, 33024, out num, false, new Vector2((this.bodyCollider.bounds.extents.x + 0.5f + this.edgeXAdjuster) * (float)this.currentFacing, 0f)))
				{
					this.BeginTurning(-this.currentFacing);
					return;
				}
			}
		}
		if (!flag2)
		{
			this.walkTimeRemaining = 0f;
		}
		else if (this.pauses && (((!(this.lineOfSightDetector != null) || !this.lineOfSightDetector.CanSeeHero) && !(this.lineOfSightDetector == null)) || !(this.alertRange != null) || !this.alertRange.IsHeroInRange()))
		{
			this.walkTimeRemaining -= Time.deltaTime;
			if (this.walkTimeRemaining <= 0f)
			{
				this.BeginStopped(Walker.StopReasons.Bored);
				return;
			}
		}
		this.body.linearVelocity = new Vector2((this.currentFacing > 0) ? this.walkSpeedR : this.walkSpeedL, this.body.linearVelocity.y);
	}

	// Token: 0x06001C2A RID: 7210 RVA: 0x000835F0 File Offset: 0x000817F0
	private void BeginTurning(int facing)
	{
		Sweep sweep = new Sweep(this.bodyCollider, 3, 3, 0.1f, 0.01f);
		if (!sweep.Check(0.25f, 33024, false))
		{
			return;
		}
		this.state = Walker.States.Turning;
		this.turningFacing = facing;
		if (this.preventTurn)
		{
			this.EndTurning();
			return;
		}
		this.turnCooldownRemaining = this.turnPause;
		this.body.linearVelocity = Vector2.Scale(this.body.linearVelocity, new Vector2((float)(this.turnStopMovement ? 0 : -1), 1f));
		if (this.turnBeforeAnimation)
		{
			this.currentFacing = this.turningFacing;
			this.UpdateScale(this.currentFacing);
		}
		this.animator.Play(this.turnClip);
		if (!string.IsNullOrEmpty(this.childAnimSuffix) && this.animator_child)
		{
			this.animator_child.Play(this.turnClip + this.childAnimSuffix);
		}
		FSMUtility.SendEventToGameObject(base.gameObject, (facing > 0) ? "TURN RIGHT" : "TURN LEFT", false);
	}

	// Token: 0x06001C2B RID: 7211 RVA: 0x0008370C File Offset: 0x0008190C
	private void UpdateTurning()
	{
		this.body.linearVelocity = Vector2.Scale(this.body.linearVelocity, new Vector2((float)(this.turnStopMovement ? 0 : 1), 1f));
		if (!this.animator.Playing)
		{
			this.EndTurning();
		}
	}

	// Token: 0x06001C2C RID: 7212 RVA: 0x0008375E File Offset: 0x0008195E
	private void EndTurning()
	{
		this.currentFacing = this.turningFacing;
		this.BeginWalking(this.currentFacing);
	}

	// Token: 0x06001C2D RID: 7213 RVA: 0x00083778 File Offset: 0x00081978
	public void ClearTurnCooldown()
	{
		this.turnCooldownRemaining = -Mathf.Epsilon;
	}

	// Token: 0x06001C2E RID: 7214 RVA: 0x00083786 File Offset: 0x00081986
	private void UpdateScale(int facing)
	{
		if (!this.preventScaleChange)
		{
			base.transform.SetScaleX((float)facing * this.rightScale);
		}
	}

	// Token: 0x04001B14 RID: 6932
	private const int LAYERMASK = 33024;

	// Token: 0x04001B15 RID: 6933
	[Header("Structure")]
	[SerializeField]
	private LineOfSightDetector lineOfSightDetector;

	// Token: 0x04001B16 RID: 6934
	[SerializeField]
	private AlertRange alertRange;

	// Token: 0x04001B17 RID: 6935
	private Rigidbody2D body;

	// Token: 0x04001B18 RID: 6936
	private Collider2D bodyCollider;

	// Token: 0x04001B19 RID: 6937
	private tk2dSpriteAnimator animator;

	// Token: 0x04001B1A RID: 6938
	private tk2dSpriteAnimator animator_child;

	// Token: 0x04001B1B RID: 6939
	private AudioSource audioSource;

	// Token: 0x04001B1C RID: 6940
	private Camera mainCamera;

	// Token: 0x04001B1D RID: 6941
	private HeroController hero;

	// Token: 0x04001B1E RID: 6942
	private const float CameraDistanceForActivation = 60f;

	// Token: 0x04001B1F RID: 6943
	private const float WaitHeroXThreshold = 1f;

	// Token: 0x04001B20 RID: 6944
	[Header("Configuration")]
	[SerializeField]
	private bool ambush;

	// Token: 0x04001B21 RID: 6945
	[SerializeField]
	private string idleClip;

	// Token: 0x04001B22 RID: 6946
	[SerializeField]
	private string turnClip;

	// Token: 0x04001B23 RID: 6947
	[SerializeField]
	private string walkClip;

	// Token: 0x04001B24 RID: 6948
	[SerializeField]
	private float edgeXAdjuster;

	// Token: 0x04001B25 RID: 6949
	[SerializeField]
	private bool preventScaleChange;

	// Token: 0x04001B26 RID: 6950
	[SerializeField]
	private bool preventTurn;

	// Token: 0x04001B27 RID: 6951
	[SerializeField]
	private float pauseTimeMin;

	// Token: 0x04001B28 RID: 6952
	[SerializeField]
	private float pauseTimeMax;

	// Token: 0x04001B29 RID: 6953
	[SerializeField]
	private float pauseWaitMin;

	// Token: 0x04001B2A RID: 6954
	[SerializeField]
	private float pauseWaitMax;

	// Token: 0x04001B2B RID: 6955
	[SerializeField]
	private bool pauses;

	// Token: 0x04001B2C RID: 6956
	[SerializeField]
	private float rightScale;

	// Token: 0x04001B2D RID: 6957
	[SerializeField]
	public bool startInactive;

	// Token: 0x04001B2E RID: 6958
	[SerializeField]
	private int turnAfterIdlePercentage;

	// Token: 0x04001B2F RID: 6959
	[SerializeField]
	private float turnPause;

	// Token: 0x04001B30 RID: 6960
	[SerializeField]
	private bool waitForHeroX;

	// Token: 0x04001B31 RID: 6961
	[SerializeField]
	private float waitHeroX;

	// Token: 0x04001B32 RID: 6962
	[SerializeField]
	public float walkSpeedL;

	// Token: 0x04001B33 RID: 6963
	[SerializeField]
	public float walkSpeedR;

	// Token: 0x04001B34 RID: 6964
	[SerializeField]
	public bool ignoreHoles;

	// Token: 0x04001B35 RID: 6965
	[SerializeField]
	private bool preventTurningToFaceHero;

	// Token: 0x04001B36 RID: 6966
	[SerializeField]
	private bool turnBeforeAnimation;

	// Token: 0x04001B37 RID: 6967
	[SerializeField]
	private bool turnStopMovement = true;

	// Token: 0x04001B38 RID: 6968
	[SerializeField]
	private GameObject animatedChild;

	// Token: 0x04001B39 RID: 6969
	[SerializeField]
	private string childAnimSuffix;

	// Token: 0x04001B3A RID: 6970
	[SerializeField]
	private float aggroEdgeTurnCooldown;

	// Token: 0x04001B3B RID: 6971
	private Walker.States state;

	// Token: 0x04001B3C RID: 6972
	private bool didFulfilCameraDistanceCondition;

	// Token: 0x04001B3D RID: 6973
	private bool didFulfilHeroXCondition;

	// Token: 0x04001B3E RID: 6974
	private int currentFacing;

	// Token: 0x04001B3F RID: 6975
	private int turningFacing;

	// Token: 0x04001B40 RID: 6976
	private float walkTimeRemaining;

	// Token: 0x04001B41 RID: 6977
	private float pauseTimeRemaining;

	// Token: 0x04001B42 RID: 6978
	private float turnCooldownRemaining;

	// Token: 0x04001B43 RID: 6979
	private float aggroEdgeTurnCooldownRemaining;

	// Token: 0x04001B44 RID: 6980
	private Walker.StopReasons stopReason;

	// Token: 0x04001B45 RID: 6981
	private int logState;

	// Token: 0x020015F7 RID: 5623
	private enum States
	{
		// Token: 0x04008946 RID: 35142
		NotReady,
		// Token: 0x04008947 RID: 35143
		WaitingForConditions,
		// Token: 0x04008948 RID: 35144
		Stopped,
		// Token: 0x04008949 RID: 35145
		Walking,
		// Token: 0x0400894A RID: 35146
		Turning
	}

	// Token: 0x020015F8 RID: 5624
	public enum StopReasons
	{
		// Token: 0x0400894C RID: 35148
		Bored,
		// Token: 0x0400894D RID: 35149
		Controlled
	}
}
