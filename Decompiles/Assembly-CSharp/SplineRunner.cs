using System;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020000B6 RID: 182
public class SplineRunner : MonoBehaviour
{
	// Token: 0x0600054E RID: 1358 RVA: 0x0001AE75 File Offset: 0x00019075
	[UsedImplicitly]
	private bool IsEventTargetEventValid(string fsmEvent)
	{
		return this.eventTarget.IsEventValid(fsmEvent);
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0001AE84 File Offset: 0x00019084
	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(this.targetPos, 0.15f);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(this.adjustedTargetPos, 0.2f);
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0001AED8 File Offset: 0x000190D8
	private void OnDrawGizmosSelected()
	{
		if (!this.raceController)
		{
			return;
		}
		Vector2 positionAlongSpline = this.raceController.GetPositionAlongSpline(this.splineDistance + this.maxSpeedHeroDistance);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(positionAlongSpline, 0.2f);
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x0001AF24 File Offset: 0x00019124
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.box = base.GetComponent<BoxCollider2D>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.animator.AnimationEventTriggeredEvent += this.OnAnimationEventTriggered;
		this.animator.AnimationCompletedEvent += this.OnAnimationCompleted;
		if (this.dashBurst)
		{
			this.dashBurst.SetActive(false);
		}
		if (this.dashBurstDown)
		{
			this.dashBurstDown.SetActive(false);
		}
		this.hero = HeroController.instance.transform;
		if (this.bonkEffect)
		{
			this.bonkEffect.IsActive = false;
			this.bonkEffect.HitInDirection += this.OnBonked;
		}
		if (this.audioSource)
		{
			this.audioSource.loop = true;
			this.audioSource.playOnAwake = false;
			this.audioSource.Stop();
		}
		this.SwitchFromPhysics();
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x0001B02C File Offset: 0x0001922C
	private void FixedUpdate()
	{
		if (!this.isFollowingPath)
		{
			return;
		}
		if (this.animator.IsPlaying("Bonk"))
		{
			this.runnerVelocity *= this.bonkDeceleration * Time.deltaTime;
			this.body.linearVelocity = this.runnerVelocity;
		}
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x0001B084 File Offset: 0x00019284
	private void Update()
	{
		this.UpdateState();
		if (this.currentMoveState != this.prevMoveState || this.justStartedRunning)
		{
			if (this.currentMoveState == SplineRunner.MoveStates.Running)
			{
				if (this.runEffectsPrefab && !this.spawnedRunEffects)
				{
					this.spawnedRunEffects = this.runEffectsPrefab.Spawn(base.transform);
					this.spawnedRunEffects.StartEffect(false, false);
				}
			}
			else if (this.spawnedRunEffects)
			{
				this.spawnedRunEffects.Stop();
				this.spawnedRunEffects = null;
			}
		}
		if (this.queuedJumpEffect)
		{
			this.queuedJumpEffect = false;
			if (this.jumpEffectsPrefab)
			{
				this.jumpEffectsPrefab.Spawn(base.transform.TransformPoint(this.jumpEffectOffset)).Play(base.gameObject, this.runnerVelocity, this.jumpEffectOffset);
			}
		}
		this.prevMoveState = this.currentMoveState;
		this.justStartedRunning = false;
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0001B178 File Offset: 0x00019378
	public void SetRaceController(SprintRaceController newRaceController)
	{
		if (this.raceController)
		{
			this.raceController.RaceCompleted -= this.OnRaceCompleted;
			this.raceController.RaceDisqualified -= this.OnRaceDisqualified;
		}
		this.raceController = newRaceController;
		if (this.raceController)
		{
			this.raceController.RaceCompleted += this.OnRaceCompleted;
			this.raceController.RaceDisqualified += this.OnRaceDisqualified;
		}
		if (this.isFollowingPath)
		{
			this.FinishedRunning();
		}
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0001B210 File Offset: 0x00019410
	private void OnRaceCompleted(bool didHeroWin)
	{
		if (!didHeroWin)
		{
			this.FinishedRunning();
		}
		if (!this.eventTarget)
		{
			return;
		}
		string text = didHeroWin ? this.heroWinEvent : this.runnerWinEvent;
		if (!string.IsNullOrEmpty(text))
		{
			this.eventTarget.SendEvent(text);
		}
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0001B25A File Offset: 0x0001945A
	private void OnRaceDisqualified()
	{
		if (!this.eventTarget)
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.disqualifiedEvent))
		{
			this.eventTarget.SendEvent(this.disqualifiedEvent);
		}
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001B288 File Offset: 0x00019488
	private void UpdateState()
	{
		if (!this.isFollowingPath || this.isInCustomAnim)
		{
			return;
		}
		this.UpdateTarget();
		if (this.dashTimeLeft > 0f)
		{
			this.dashTimeLeft -= Time.deltaTime;
			if (this.dashTimeLeft <= 0f)
			{
				this.FastForwardToCurrentPoint(false);
				this.nextDashTime = Time.timeAsDouble + (double)this.dashCooldown.GetRandomValue();
				this.StopLoop();
				if (this.isDashingDown)
				{
					int num;
					int num2;
					float num3;
					this.raceController.GetRaceInfo(out num, out num2, out num3);
					float num4 = num3 * this.speedMultiplierRange.Start;
					if (this.currentSpeed > num4)
					{
						Debug.Log(string.Format("Dashing finished! Reducing speed from {0} to {1}", this.currentSpeed, num4));
						this.currentSpeed = num4;
					}
				}
			}
			else
			{
				if (this.isDashingDown)
				{
					Transform transform = base.transform;
					Vector3 position = transform.position;
					position.y -= this.currentSpeed * Time.deltaTime;
					transform.position = position;
					return;
				}
				Transform transform2 = base.transform;
				Vector3 position2 = transform2.position;
				position2.x += this.currentSpeed * Mathf.Sign(transform2.lossyScale.x) * (float)(this.spriteFacesRight ? 1 : -1) * Time.deltaTime;
				transform2.position = position2;
				return;
			}
		}
		this.UpdateRunner();
		if (!this.isFollowingPath || this.isInCustomAnim)
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.pitch = Time.timeScale;
		}
		SplineRunner.MoveStates moveStates = this.currentMoveState;
		if (moveStates != SplineRunner.MoveStates.Running)
		{
			if (moveStates != SplineRunner.MoveStates.Flying)
			{
				return;
			}
			if (!this.animator.IsPlaying("Fly Turn"))
			{
				string name = (this.runnerVelocity.y > 0f) ? "Fly Up" : "Fly Down";
				if (!this.animator.IsPlaying(name))
				{
					this.animator.Play(name);
				}
			}
		}
		else if (this.prevMoveState != SplineRunner.MoveStates.Running)
		{
			this.animator.Play("Sprint Start");
			this.PlayLoop(this.runAudioLoop);
			if (this.voiceAudioSource)
			{
				this.voiceAudioSource.clip = this.voiceRunTable.SelectClip(false);
				this.voiceAudioSource.volume = this.voiceRunTable.SelectVolume();
				this.voiceAudioSource.pitch = this.voiceRunTable.SelectPitch();
				this.voiceAudioSource.Play();
				return;
			}
		}
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0001B50C File Offset: 0x0001970C
	private void UpdateTarget()
	{
		if (this.isTargetAtEnd)
		{
			return;
		}
		Vector2 b = base.transform.position;
		this.targetPos = this.raceController.GetPositionAlongSpline(this.splineDistance);
		while ((this.targetPos - b).magnitude < this.targetDistance)
		{
			this.splineDistance += 0.1f;
			this.targetPos = this.raceController.GetPositionAlongSpline(this.splineDistance);
			if (this.splineDistance >= this.raceController.TotalPathDistance)
			{
				this.isTargetAtEnd = true;
				break;
			}
		}
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001B5AC File Offset: 0x000197AC
	private void UpdateRunner()
	{
		int num;
		int num2;
		float num3;
		this.raceController.GetRaceInfo(out num, out num2, out num3);
		bool flag = num2 > num || (num <= num2 && this.raceController.GetDistanceAlongSpline(this.hero.position, false) - this.splineDistance > this.maxSpeedHeroDistance);
		if (flag)
		{
			if (this.CanDashForward())
			{
				this.DashForward();
				return;
			}
			this.currentTargetSpeed = num3 * this.speedMultiplierRange.End;
		}
		else
		{
			this.speedChangeDelayLeft -= Time.deltaTime;
			if (this.speedChangeDelayLeft <= 0f)
			{
				this.speedChangeDelayLeft = this.speedChangeDelay.GetRandomValue();
				this.currentTargetSpeed = num3 * this.speedMultiplierRange.GetRandomValue();
			}
		}
		this.currentSpeed = Mathf.Lerp(this.currentSpeed, this.currentTargetSpeed, Time.deltaTime * this.speedLerpSpeed);
		float num4 = this.animator.IsPlaying("Sprint Start") ? this.runAnimStartSpeedMultiplier : 1f;
		this.TrySnapToGround(this.targetPos, out this.adjustedTargetPos, true);
		Transform transform = base.transform;
		Vector2 vector = transform.position;
		Vector2 vector2 = this.adjustedTargetPos - vector;
		Vector2 a = vector2;
		if (this.currentMoveState == SplineRunner.MoveStates.Running)
		{
			a.y = 0f;
		}
		a = ((a.magnitude > 0f) ? a.normalized : Vector2.zero);
		this.runnerVelocity = a * (this.currentSpeed * num4);
		if (!this.isTargetAtEnd || !this.ShouldTurn(this.runnerVelocity.x))
		{
			vector += this.runnerVelocity * Time.deltaTime;
			Vector2 position = vector;
			Vector2 vector3;
			float num5;
			if (Vector2.Dot(vector2.normalized, Vector2.up) > 0.5f)
			{
				this.StartFlying(true);
			}
			else if (this.TrySnapToGround(vector, out vector3, false, out num5))
			{
				if (this.wasGroundSnapped)
				{
					if (vector3.y + 0.2f < this.previousGroundSnappedY)
					{
						this.wasGroundSnapped = false;
					}
					else if (vector3.y - 0.2f > this.previousGroundSnappedY)
					{
						if (this.animator.IsPlaying("Sprint"))
						{
							this.animator.PlayFromFrame("Sprint Start", 0);
						}
						else if (this.animator.IsPlaying("Dash Forward") || this.animator.IsPlaying("Dash Forward Mantle"))
						{
							this.animator.PlayFromFrame("Dash Forward Mantle", 0);
						}
					}
				}
				if (num5 < 0.1f)
				{
					if (!this.wasGroundSnapped)
					{
						this.PlayOneShot(this.landClip);
					}
					this.currentMoveState = SplineRunner.MoveStates.Running;
					this.wasGroundSnapped = true;
					this.groundSnapT = 1f;
					this.groundSnapLerpMultiplier = 1f;
					position = vector3;
				}
				else
				{
					if (!this.wasGroundSnapped)
					{
						this.wasGroundSnapped = true;
						this.groundSnapLerpMultiplier = 1f;
						this.groundSnapT = 0f;
					}
					this.groundSnapLerpMultiplier += Time.deltaTime * Mathf.Abs(Physics2D.gravity.y);
					this.groundSnapT += Time.deltaTime * this.groundSnapLerpMultiplier;
					if (this.groundSnapT > 1f)
					{
						this.groundSnapT = 1f;
					}
					position = Vector2.Lerp(vector, vector3, this.groundSnapT);
				}
				this.previousGroundSnappedY = vector3.y;
			}
			else
			{
				this.StartFlying(vector2.y > 0f);
			}
			if (this.ShouldTurn(this.runnerVelocity.x))
			{
				if (this.currentMoveState == SplineRunner.MoveStates.Flying)
				{
					this.animator.Play("Fly Turn");
				}
				else
				{
					this.animator.Play("Turn");
					this.isInCustomAnim = true;
					this.StopLoop();
				}
			}
			transform.SetPosition2D(position);
			return;
		}
		bool flag2;
		this.raceController.ReportRunnerLapCompleted(out flag2);
		if (flag2)
		{
			this.FinishedRunning();
			return;
		}
		this.StartLoop();
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0001B9A8 File Offset: 0x00019BA8
	private void StartFlying(bool doLaunch)
	{
		if (doLaunch && this.currentMoveState != SplineRunner.MoveStates.Flying)
		{
			this.dashTimeLeft = 0f;
			this.animator.Play("Antic Skid");
			this.isInCustomAnim = true;
			this.StopLoop();
			this.PlayOneShot(this.jumpAnticClip);
			if (this.voiceAudioSource)
			{
				this.voiceAudioSource.Stop();
				this.voiceAudioSource.loop = false;
				this.voiceAudioSource.clip = this.voiceEffortTable.SelectClip(false);
				this.voiceAudioSource.volume = this.voiceEffortTable.SelectVolume();
				this.voiceAudioSource.pitch = this.voiceEffortTable.SelectPitch();
				this.voiceAudioSource.Play();
			}
		}
		else if (!this.isInCustomAnim)
		{
			this.PlayLoop(this.flyAudioLoop);
		}
		this.currentMoveState = SplineRunner.MoveStates.Flying;
		this.wasGroundSnapped = false;
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0001BA94 File Offset: 0x00019C94
	private void FinishedRunning()
	{
		if (this.spawnedRunEffects)
		{
			this.spawnedRunEffects.Stop();
			this.spawnedRunEffects = null;
		}
		this.isFollowingPath = false;
		if (this.bonkEffect)
		{
			this.bonkEffect.IsActive = false;
		}
		if (this.voiceAudioSource)
		{
			this.voiceAudioSource.Stop();
			this.voiceAudioSource.clip = null;
		}
		this.StopLoop();
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0001BB0A File Offset: 0x00019D0A
	private void StartLoop()
	{
		this.splineDistance = 0f;
		this.isTargetAtEnd = false;
		this.FastForwardToCurrentPoint(true);
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0001BB28 File Offset: 0x00019D28
	private void FastForwardToCurrentPoint(bool useShouldTurn = false)
	{
		if (!useShouldTurn)
		{
			this.splineDistance = this.raceController.GetDistanceAlongSpline(base.transform.position, true);
			return;
		}
		float num = 0f;
		bool flag = false;
		while (this.ShouldTurn(this.GetDirectionToSplineDistance(num)))
		{
			num += 0.1f;
			if (num >= this.raceController.TotalPathDistance)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.splineDistance = num;
		}
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x0001BB98 File Offset: 0x00019D98
	private float GetDirectionToSplineDistance(float distance)
	{
		Vector2 b = base.transform.position;
		return Mathf.Sign((this.raceController.GetPositionAlongSpline(distance) - b).x);
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x0001BBD4 File Offset: 0x00019DD4
	private bool ShouldTurn(float moveDir)
	{
		float num = base.transform.localScale.x * (float)(this.spriteFacesRight ? 1 : -1);
		return (num < 0f && moveDir > 0f) || (num > 0f && moveDir < 0f);
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x0001BC24 File Offset: 0x00019E24
	private bool TrySnapToGround(Vector2 initialPos, out Vector2 newPos, bool isTarget)
	{
		float num;
		return this.TrySnapToGround(initialPos, out newPos, isTarget, out num);
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0001BC3C File Offset: 0x00019E3C
	private bool TrySnapToGround(Vector2 initialPos, out Vector2 newPos, bool isTarget, out float groundDistance)
	{
		Vector2 vector = initialPos + this.box.offset;
		Vector2 vector2 = this.box.size * 0.5f;
		Vector2 vector3 = vector - vector2;
		float num = vector.y - vector3.y;
		float distance = num + this.groundDistanceThreshold;
		RaycastHit2D hit = global::Helper.Raycast2D(vector, Vector2.down, distance, 256);
		bool result;
		if (hit)
		{
			newPos = hit.point;
			newPos.y += vector2.y - this.box.offset.y;
			result = true;
			groundDistance = vector3.y - hit.point.y;
		}
		else
		{
			newPos = initialPos;
			if (isTarget)
			{
				newPos.y += num;
			}
			result = false;
			groundDistance = float.MaxValue;
		}
		return result;
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0001BD18 File Offset: 0x00019F18
	private void OnAnimationCompleted(tk2dSpriteAnimator spriteAnimator, tk2dSpriteAnimationClip clip)
	{
		this.isInCustomAnim = false;
		if (!this.isFollowingPath)
		{
			return;
		}
		string name = clip.name;
		if (!(name == "Fly Turn"))
		{
			if (name == "Sprint Start")
			{
				spriteAnimator.Play("Sprint");
				return;
			}
			if (name == "Turn")
			{
				spriteAnimator.Play("Sprint");
				spriteAnimator.transform.FlipLocalScale(true, false, false);
				return;
			}
			if (name == "Antic Skid")
			{
				this.queuedJumpEffect = true;
				this.PlayLoop(this.flyAudioLoop);
				this.PlayOneShot(this.jumpClip);
				return;
			}
			if (!(name == "Bonk"))
			{
				return;
			}
			spriteAnimator.Play("Sprint Start");
			this.PlayLoop(this.runAudioLoop);
			if (this.voiceAudioSource)
			{
				this.voiceAudioSource.loop = true;
				this.voiceAudioSource.clip = this.voiceRunTable.SelectClip(false);
				this.voiceAudioSource.volume = this.voiceRunTable.SelectVolume();
				this.voiceAudioSource.pitch = this.voiceRunTable.SelectPitch();
				this.voiceAudioSource.Play();
			}
			this.SwitchFromPhysics();
			return;
		}
		else
		{
			spriteAnimator.transform.FlipLocalScale(true, false, false);
			if (this.runnerVelocity.y > 0f)
			{
				spriteAnimator.Play("Fly Up");
				return;
			}
			if (this.CanDashDown())
			{
				this.DashDown();
				return;
			}
			spriteAnimator.Play("Fly Down");
			return;
		}
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x0001BE94 File Offset: 0x0001A094
	private void OnAnimationEventTriggered(tk2dSpriteAnimator spriteAnimator, tk2dSpriteAnimationClip clip, int frame)
	{
		if (!this.queuedDashEffect)
		{
			return;
		}
		string name = clip.name;
		if (!(name == "Dash Forward") && !(name == "Dash Down"))
		{
			return;
		}
		this.queuedDashEffect = false;
		this.isInCustomAnim = false;
		if (!this.isDashingDown && this.dashEffectPrefab)
		{
			Transform transform = base.transform;
			Vector3 localScale = transform.localScale;
			DashEffect dashEffect = this.dashEffectPrefab.Spawn(transform.position);
			dashEffect.transform.localScale = new Vector3(localScale.x * -1f, localScale.y, localScale.z);
			dashEffect.Play(base.gameObject);
		}
		GameObject gameObject = this.isDashingDown ? this.dashBurstDown : this.dashBurst;
		if (gameObject)
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
			gameObject.SetActive(true);
		}
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x0001BF89 File Offset: 0x0001A189
	private void PlayOneShot(AudioClip clip)
	{
		if (!this.audioSource || !clip)
		{
			return;
		}
		this.audioSource.PlayOneShot(clip);
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x0001BFB0 File Offset: 0x0001A1B0
	private void PlayLoop(AudioClip clip)
	{
		if (!this.audioSource)
		{
			return;
		}
		if (this.audioSource.clip == clip)
		{
			return;
		}
		this.audioSource.pitch = 1f;
		this.audioSource.clip = clip;
		this.audioSource.Play();
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0001C006 File Offset: 0x0001A206
	private void StopLoop()
	{
		if (!this.audioSource)
		{
			return;
		}
		this.audioSource.Stop();
		this.audioSource.pitch = 1f;
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x0001C031 File Offset: 0x0001A231
	[ContextMenu("TEST", true)]
	public bool CanTest()
	{
		return Application.isPlaying;
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x0001C038 File Offset: 0x0001A238
	[ContextMenu("TEST")]
	public void StartRunning()
	{
		this.PositionAtStart();
		this.isFollowingPath = true;
		this.justStartedRunning = true;
		int num;
		int num2;
		float num3;
		this.raceController.GetRaceInfo(out num, out num2, out num3);
		this.currentSpeed = (this.currentTargetSpeed = num3 * this.speedMultiplierRange.End);
		this.speedChangeDelayLeft = this.speedChangeDelay.GetRandomValue();
		if (this.CanDashForward())
		{
			this.DashForward();
		}
		this.raceController.StartTracking();
		if (this.bonkEffect)
		{
			this.bonkEffect.IsActive = true;
		}
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x0001C0CC File Offset: 0x0001A2CC
	public void PositionAtStart()
	{
		this.ResetRace();
		this.raceController.BeginInRace();
		Vector2 positionAlongSpline = this.raceController.GetPositionAlongSpline(0f);
		this.TrySnapToGround(positionAlongSpline, out positionAlongSpline, false);
		base.transform.SetPosition2D(positionAlongSpline);
		float moveDir = this.raceController.GetPositionAlongSpline(0.1f).x - positionAlongSpline.x;
		if (this.ShouldTurn(moveDir))
		{
			base.transform.FlipLocalScale(true, false, false);
		}
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x0001C148 File Offset: 0x0001A348
	public void ResetRace()
	{
		this.FinishedRunning();
		this.isInCustomAnim = false;
		this.splineDistance = 0f;
		this.isTargetAtEnd = false;
		this.dashTimeLeft = 0f;
		this.nextDashTime = 0.0;
		if (this.raceController != null)
		{
			this.raceController.StopTracking();
			this.raceController.EndInRace();
		}
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x0001C1B4 File Offset: 0x0001A3B4
	private bool CanDashForward()
	{
		if (this.currentMoveState != SplineRunner.MoveStates.Running)
		{
			return false;
		}
		if (this.dashTimeLeft > 0f || Time.timeAsDouble < this.nextDashTime)
		{
			return false;
		}
		float num = this.dashSpeed * this.dashDuration;
		Transform transform = base.transform;
		float num2 = transform.localScale.x * (float)(this.spriteFacesRight ? 1 : -1);
		Vector2 initialPos = transform.position;
		initialPos.x += num * num2;
		return this.TrySnapToGround(initialPos, out initialPos, false);
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0001C238 File Offset: 0x0001A438
	private void DashForward()
	{
		this.animator.PlayFromFrame("Dash Forward", 0);
		this.isDashingDown = false;
		this.dashTimeLeft = this.dashDuration;
		this.currentSpeed = this.dashSpeed;
		this.currentMoveState = SplineRunner.MoveStates.Dashing;
		this.DashShared();
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x0001C278 File Offset: 0x0001A478
	private bool CanDashDown()
	{
		if (this.currentMoveState != SplineRunner.MoveStates.Flying)
		{
			return false;
		}
		if (this.isDashingDown && (this.dashTimeLeft > 0f || Time.timeAsDouble < this.nextDashTime))
		{
			return false;
		}
		Vector2 b = base.transform.position;
		float num = this.splineDistance;
		Vector2 positionAlongSpline;
		Vector2 vector;
		do
		{
			num += 0.1f;
			positionAlongSpline = this.raceController.GetPositionAlongSpline(num);
			if (num >= this.raceController.TotalPathDistance)
			{
				break;
			}
			vector = positionAlongSpline - b;
		}
		while (vector.y <= Mathf.Epsilon && vector.magnitude < this.dashDownCheckDistance);
		Vector2 vector2 = positionAlongSpline - b;
		if (vector2.y > Mathf.Epsilon)
		{
			return false;
		}
		float num2;
		for (num2 = vector2.DirectionToAngle(); num2 < 0f; num2 += 360f)
		{
		}
		while (num2 >= 360f)
		{
			num2 -= 360f;
		}
		return this.dashDownAngleRange.IsInRange(num2);
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x0001C36C File Offset: 0x0001A56C
	private void DashDown()
	{
		this.animator.PlayFromFrame("Dash Down", 0);
		this.isInCustomAnim = true;
		this.isDashingDown = true;
		this.dashTimeLeft = this.raceController.DashDownDuration;
		this.currentSpeed = this.dashDownSpeed;
		this.DashShared();
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x0001C3BB File Offset: 0x0001A5BB
	private void DashShared()
	{
		this.queuedDashEffect = true;
		this.StopLoop();
		this.PlayLoop(this.dashAudioLoop);
		this.dashAudio.SpawnAndPlayOneShot(base.transform.position, null);
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0001C3F0 File Offset: 0x0001A5F0
	private void OnBonked(GameObject source, HitInstance.HitDirection direction)
	{
		this.animator.Play("Bonk");
		this.StopLoop();
		this.isInCustomAnim = true;
		if (this.voiceHitTable)
		{
			this.voiceAudioSource.Stop();
			this.voiceHitTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		this.SwitchToPhysics();
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x0001C450 File Offset: 0x0001A650
	private void SwitchToPhysics()
	{
		this.body.constraints = RigidbodyConstraints2D.FreezeRotation;
		this.body.linearVelocity = this.runnerVelocity;
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0001C46F File Offset: 0x0001A66F
	private void SwitchFromPhysics()
	{
		this.body.constraints = RigidbodyConstraints2D.FreezeAll;
		this.body.linearVelocity = Vector2.zero;
	}

	// Token: 0x0400052B RID: 1323
	private const string FLY_UP_ANIM = "Fly Up";

	// Token: 0x0400052C RID: 1324
	private const string FLY_DOWN_ANIM = "Fly Down";

	// Token: 0x0400052D RID: 1325
	private const string FLY_TURN_ANIM = "Fly Turn";

	// Token: 0x0400052E RID: 1326
	private const string FLY_ANTIC_ANIM = "Antic Skid";

	// Token: 0x0400052F RID: 1327
	private const string RUN_START_ANIM = "Sprint Start";

	// Token: 0x04000530 RID: 1328
	private const string RUN_ANIM = "Sprint";

	// Token: 0x04000531 RID: 1329
	private const string RUN_TURN_ANIM = "Turn";

	// Token: 0x04000532 RID: 1330
	private const string DASH_ANIM = "Dash Forward";

	// Token: 0x04000533 RID: 1331
	private const string DASH_MANTLE_ANIM = "Dash Forward Mantle";

	// Token: 0x04000534 RID: 1332
	private const string BONK_ANIM = "Bonk";

	// Token: 0x04000535 RID: 1333
	private const string DASH_DOWN_ANIM = "Dash Down";

	// Token: 0x04000536 RID: 1334
	[SerializeField]
	private MinMaxFloat speedMultiplierRange;

	// Token: 0x04000537 RID: 1335
	[SerializeField]
	private MinMaxFloat speedChangeDelay;

	// Token: 0x04000538 RID: 1336
	[SerializeField]
	private float speedLerpSpeed;

	// Token: 0x04000539 RID: 1337
	[SerializeField]
	private float maxSpeedHeroDistance;

	// Token: 0x0400053A RID: 1338
	[Space]
	[SerializeField]
	private float dashSpeed;

	// Token: 0x0400053B RID: 1339
	[SerializeField]
	private float dashDuration;

	// Token: 0x0400053C RID: 1340
	[SerializeField]
	private MinMaxFloat dashCooldown;

	// Token: 0x0400053D RID: 1341
	[SerializeField]
	private float dashDownCheckDistance;

	// Token: 0x0400053E RID: 1342
	[SerializeField]
	private MinMaxFloat dashDownAngleRange;

	// Token: 0x0400053F RID: 1343
	[SerializeField]
	private float dashDownSpeed;

	// Token: 0x04000540 RID: 1344
	[SerializeField]
	private float runAnimStartSpeedMultiplier;

	// Token: 0x04000541 RID: 1345
	[Space]
	[SerializeField]
	private bool spriteFacesRight;

	// Token: 0x04000542 RID: 1346
	[SerializeField]
	private float groundDistanceThreshold;

	// Token: 0x04000543 RID: 1347
	[SerializeField]
	private float targetDistance;

	// Token: 0x04000544 RID: 1348
	[SerializeField]
	private RunEffects runEffectsPrefab;

	// Token: 0x04000545 RID: 1349
	[SerializeField]
	private DashEffect dashEffectPrefab;

	// Token: 0x04000546 RID: 1350
	[SerializeField]
	private GameObject dashBurst;

	// Token: 0x04000547 RID: 1351
	[SerializeField]
	private GameObject dashBurstDown;

	// Token: 0x04000548 RID: 1352
	[SerializeField]
	private JumpEffects jumpEffectsPrefab;

	// Token: 0x04000549 RID: 1353
	[SerializeField]
	private Vector3 jumpEffectOffset;

	// Token: 0x0400054A RID: 1354
	[Space]
	[SerializeField]
	private HitResponseBase bonkEffect;

	// Token: 0x0400054B RID: 1355
	[SerializeField]
	private float bonkDeceleration;

	// Token: 0x0400054C RID: 1356
	[Space]
	[SerializeField]
	private PlayMakerFSM eventTarget;

	// Token: 0x0400054D RID: 1357
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsEventTargetEventValid")]
	private string runnerWinEvent;

	// Token: 0x0400054E RID: 1358
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsEventTargetEventValid")]
	private string heroWinEvent;

	// Token: 0x0400054F RID: 1359
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsEventTargetEventValid")]
	private string disqualifiedEvent;

	// Token: 0x04000550 RID: 1360
	[Space]
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000551 RID: 1361
	[SerializeField]
	private AudioClip runAudioLoop;

	// Token: 0x04000552 RID: 1362
	[SerializeField]
	private AudioEvent dashAudio;

	// Token: 0x04000553 RID: 1363
	[SerializeField]
	private AudioClip dashAudioLoop;

	// Token: 0x04000554 RID: 1364
	[SerializeField]
	private AudioClip flyAudioLoop;

	// Token: 0x04000555 RID: 1365
	[SerializeField]
	private AudioClip jumpAnticClip;

	// Token: 0x04000556 RID: 1366
	[SerializeField]
	private AudioClip jumpClip;

	// Token: 0x04000557 RID: 1367
	[SerializeField]
	private AudioClip landClip;

	// Token: 0x04000558 RID: 1368
	[Space]
	[SerializeField]
	private AudioSource voiceAudioSource;

	// Token: 0x04000559 RID: 1369
	[SerializeField]
	private RandomAudioClipTable voiceRunTable;

	// Token: 0x0400055A RID: 1370
	[SerializeField]
	private RandomAudioClipTable voiceEffortTable;

	// Token: 0x0400055B RID: 1371
	[SerializeField]
	private RandomAudioClipTable voiceHitTable;

	// Token: 0x0400055C RID: 1372
	private Vector2 targetPos;

	// Token: 0x0400055D RID: 1373
	private Vector2 adjustedTargetPos;

	// Token: 0x0400055E RID: 1374
	private Vector2 runnerVelocity;

	// Token: 0x0400055F RID: 1375
	private bool isFollowingPath;

	// Token: 0x04000560 RID: 1376
	private bool isInCustomAnim;

	// Token: 0x04000561 RID: 1377
	private float splineDistance;

	// Token: 0x04000562 RID: 1378
	private bool isTargetAtEnd;

	// Token: 0x04000563 RID: 1379
	private bool queuedJumpEffect;

	// Token: 0x04000564 RID: 1380
	private bool queuedDashEffect;

	// Token: 0x04000565 RID: 1381
	private float currentSpeed;

	// Token: 0x04000566 RID: 1382
	private float currentTargetSpeed;

	// Token: 0x04000567 RID: 1383
	private float speedChangeDelayLeft;

	// Token: 0x04000568 RID: 1384
	private float dashTimeLeft;

	// Token: 0x04000569 RID: 1385
	private double nextDashTime;

	// Token: 0x0400056A RID: 1386
	private bool isDashingDown;

	// Token: 0x0400056B RID: 1387
	private SplineRunner.MoveStates currentMoveState;

	// Token: 0x0400056C RID: 1388
	private bool justStartedRunning;

	// Token: 0x0400056D RID: 1389
	private SplineRunner.MoveStates prevMoveState;

	// Token: 0x0400056E RID: 1390
	private bool wasGroundSnapped;

	// Token: 0x0400056F RID: 1391
	private float groundSnapLerpMultiplier;

	// Token: 0x04000570 RID: 1392
	private float groundSnapT;

	// Token: 0x04000571 RID: 1393
	private float previousGroundSnappedY;

	// Token: 0x04000572 RID: 1394
	private SprintRaceController raceController;

	// Token: 0x04000573 RID: 1395
	private Rigidbody2D body;

	// Token: 0x04000574 RID: 1396
	private BoxCollider2D box;

	// Token: 0x04000575 RID: 1397
	private tk2dSpriteAnimator animator;

	// Token: 0x04000576 RID: 1398
	private RunEffects spawnedRunEffects;

	// Token: 0x04000577 RID: 1399
	private Transform hero;

	// Token: 0x0200141F RID: 5151
	private enum MoveStates
	{
		// Token: 0x04008201 RID: 33281
		Running,
		// Token: 0x04008202 RID: 33282
		Flying,
		// Token: 0x04008203 RID: 33283
		Dashing
	}
}
