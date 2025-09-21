using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class CameraTarget : MonoBehaviour
{
	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000B21 RID: 2849 RVA: 0x000323C1 File Offset: 0x000305C1
	// (set) Token: 0x06000B22 RID: 2850 RVA: 0x000323C9 File Offset: 0x000305C9
	public bool IsFreeModeManual { get; private set; }

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000B23 RID: 2851 RVA: 0x000323D2 File Offset: 0x000305D2
	private bool IsIgnoringXOffset
	{
		get
		{
			if (!this.ignoreXOffset)
			{
				PlayerData playerData = this.hero_ctrl.playerData;
				return playerData != null && playerData.atBench;
			}
			return true;
		}
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x000323F4 File Offset: 0x000305F4
	public void GameInit()
	{
		this.gm = GameManager.instance;
		if (this.cameraCtrl == null)
		{
			this.cameraCtrl = base.transform.parent.GetComponent<CameraController>();
		}
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x00032428 File Offset: 0x00030628
	public void SceneInit()
	{
		if (this.gm == null)
		{
			this.gm = GameManager.instance;
		}
		if (this.gm.IsGameplayScene())
		{
			this.isGameplayScene = true;
			this.hero_ctrl = HeroController.instance;
			this.heroTransform = this.hero_ctrl.transform;
			this.mode = CameraTarget.TargetMode.FOLLOW_HERO;
			this.IsFreeModeManual = false;
			this.xLockMin = 0f;
			this.xLockMax = this.cameraCtrl.xLimit;
			this.yLockMin = 0f;
			this.yLockMax = this.cameraCtrl.yLimit;
			this.StopCamRising();
			return;
		}
		this.isGameplayScene = false;
		this.mode = CameraTarget.TargetMode.FREE;
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x000324DC File Offset: 0x000306DC
	public void Update()
	{
		if (this.hero_ctrl == null || !this.isGameplayScene)
		{
			this.mode = CameraTarget.TargetMode.FREE;
			return;
		}
		if (Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		if (this.isGameplayScene)
		{
			float x = base.transform.position.x;
			float y = base.transform.position.y;
			float z = base.transform.position.z;
			float x2 = this.heroTransform.position.x;
			float num = this.heroTransform.position.y;
			Vector2 cameraOffset = this.GetCameraOffset();
			num += cameraOffset.y;
			if (this.mode == CameraTarget.TargetMode.FOLLOW_HERO || this.mode == CameraTarget.TargetMode.LOCK_ZONE)
			{
				this.SetDampTime();
				this.destination = this.heroTransform.position;
				if (this.IsIgnoringXOffset)
				{
					this.xOffset = 0f;
				}
				else if (this.hero_ctrl.cState.transitioning)
				{
					this.xOffset = (this.hero_ctrl.cState.facingRight ? 1f : -1f);
				}
				else if (this.hero_ctrl.cState.facingRight)
				{
					if (this.xOffset < this.xLookAhead)
					{
						this.xOffset += Time.deltaTime * 6f;
					}
				}
				else if (this.xOffset > -this.xLookAhead)
				{
					this.xOffset -= Time.deltaTime * 6f;
				}
				if (this.xOffset > 1f)
				{
					this.xOffset = 1f;
				}
				if (this.xOffset < -1f)
				{
					this.xOffset = -1f;
				}
				if (this.hero_ctrl.cState.dashing && (this.hero_ctrl.current_velocity.x > 5f || this.hero_ctrl.current_velocity.x < -5f))
				{
					if (this.hero_ctrl.cState.facingRight)
					{
						this.dashOffset = this.dashLookAhead;
					}
					else
					{
						this.dashOffset = -this.dashLookAhead;
					}
				}
				else if (this.sprinting)
				{
					if (this.hero_ctrl.cState.facingRight)
					{
						this.dashOffset = this.sprintLookAhead;
					}
					else
					{
						this.dashOffset = -this.sprintLookAhead;
					}
				}
				else if (this.sliding)
				{
					if (this.hero_ctrl.cState.facingRight)
					{
						this.dashOffset = this.slidingLookAhead;
					}
					else
					{
						this.dashOffset = -this.slidingLookAhead;
					}
				}
				else if (this.harpooning)
				{
					if (this.hero_ctrl.cState.facingRight)
					{
						this.dashOffset = this.harpoonLookAhead;
					}
					else
					{
						this.dashOffset = -this.harpoonLookAhead;
					}
				}
				else
				{
					this.dashOffset = 0f;
				}
				if (this.mode == CameraTarget.TargetMode.FREE)
				{
					this.dashOffset = 0f;
				}
				if (this.umbrellaFloating)
				{
					this.umbrellaFloatTimer += Time.deltaTime;
				}
				else
				{
					this.umbrellaFloatTimer = 0f;
				}
				if (this.ridingUpdraft)
				{
					this.wallSprintOffset = this.updraftLookahead;
				}
				else if (this.superJumping)
				{
					this.cameraCtrl.isRising = true;
					this.wallSprintOffset = this.superJumpLookahead;
					if (y + this.wallSprintOffset > this.superJumpDestinationY)
					{
						this.wallSprintOffset = this.superJumpDestinationY - y;
					}
				}
				else if (this.camRising)
				{
					this.wallSprintOffset = this.risingLookAhead;
					if (y + this.wallSprintOffset > this.risingDestinationY)
					{
						this.wallSprintOffset = this.risingDestinationY - y;
					}
				}
				else if (this.sliding)
				{
					this.wallSprintOffset = this.slidingLookAheadVertical;
				}
				else if (this.hero_ctrl.cState.falling && this.hero_ctrl.current_velocity.y < -22f)
				{
					this.cameraCtrl.isFalling = true;
					this.wallSprintOffset -= Time.deltaTime * 4f;
					if (this.wallSprintOffset < -1f)
					{
						this.wallSprintOffset = -1f;
					}
				}
				else if (this.umbrellaFloatTimer > 1.25f)
				{
					this.wallSprintOffset -= Time.deltaTime * 1f;
					if (this.wallSprintOffset < -1.5f)
					{
						this.wallSprintOffset = -1.5f;
					}
				}
				else
				{
					if (this.cameraCtrl.isFalling)
					{
						this.cameraCtrl.isFalling = false;
					}
					if (this.cameraCtrl.isRising)
					{
						this.cameraCtrl.isRising = false;
					}
					this.wallSprintOffset = 0f;
				}
				if (this.mode == CameraTarget.TargetMode.FREE)
				{
					this.wallSprintOffset = 0f;
				}
				this.destination = new Vector2(this.destination.x + this.xOffset + this.dashOffset, this.destination.y + this.wallSprintOffset);
				float num2;
				float num3;
				float num4;
				float num5;
				if (this.mode == CameraTarget.TargetMode.FOLLOW_HERO)
				{
					num2 = 0f;
					num3 = 9999f;
					num4 = 0f;
					num5 = 9999f;
				}
				else
				{
					num2 = this.xLockMin;
					num3 = this.xLockMax;
					num4 = this.yLockMin;
					num5 = this.yLockMax;
				}
				if (this.destination.x < num2)
				{
					this.destination.x = num2;
				}
				if (this.destination.x > num3)
				{
					this.destination.x = num3;
				}
				if (this.destination.y < num4)
				{
					this.destination.y = num4;
				}
				if (this.destination.y > num5)
				{
					this.destination.y = num5;
				}
				Vector3 vector = this.heroTransform.position - this.heroPosition_prev;
				if (this.hero_ctrl.cState.transitioning)
				{
					base.transform.position = this.destination;
				}
				else if (this.detachTimer <= 0f && !this.detachedFromHero)
				{
					if (vector.x < -0.001f && base.transform.position.x > this.destination.x)
					{
						base.transform.SetPositionX(base.transform.position.x + vector.x);
						if (base.transform.position.x < this.destination.x)
						{
							base.transform.SetPositionX(this.destination.x);
						}
					}
					if (vector.x > 0.001f && base.transform.position.x < this.destination.x)
					{
						base.transform.SetPositionX(base.transform.position.x + vector.x);
						if (base.transform.position.x > this.destination.x)
						{
							base.transform.SetPositionX(this.destination.x);
						}
					}
					if (vector.y < -0.001f && base.transform.position.y > this.destination.y)
					{
						base.transform.SetPositionY(base.transform.position.y + vector.y);
						if (base.transform.position.y < this.destination.y)
						{
							base.transform.SetPositionY(this.destination.y);
						}
					}
					if (vector.y > 0.001f && base.transform.position.y < this.destination.y)
					{
						base.transform.SetPositionY(base.transform.position.y + vector.y);
						if (base.transform.position.y > this.destination.y)
						{
							base.transform.SetPositionY(this.destination.y);
						}
					}
				}
				this.heroPosition_prev = this.heroTransform.position;
				base.transform.position = new Vector3(Vector3.SmoothDamp(base.transform.position, new Vector3(this.destination.x, y, z), ref this.velocityX, this.dampTimeX).x, Vector3.SmoothDamp(base.transform.position, new Vector3(x, this.destination.y, z), ref this.velocityY, this.dampTimeY).y, z);
			}
			this.heroPrevPosition = this.heroTransform.position;
			if (!this.cameraCtrl.AllowExitingSceneBounds)
			{
				if (base.transform.position.x < 14.6f)
				{
					base.transform.SetPositionX(14.6f);
				}
				if (base.transform.position.x > this.cameraCtrl.xLimit)
				{
					base.transform.SetPositionX(this.cameraCtrl.xLimit);
				}
				if (base.transform.position.y < 8.3f)
				{
					base.transform.SetPositionY(8.3f);
				}
				if (base.transform.position.y > this.cameraCtrl.yLimit)
				{
					base.transform.SetPositionY(this.cameraCtrl.yLimit);
				}
				if (base.transform.position.x + this.xOffset < 14.6f)
				{
					this.xOffset = 14.6f - base.transform.position.x - 0.001f;
				}
				if (base.transform.position.x + this.xOffset > this.cameraCtrl.xLimit)
				{
					this.xOffset = this.cameraCtrl.xLimit - base.transform.position.x + 0.001f;
				}
			}
		}
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x00032EDC File Offset: 0x000310DC
	public void EnterLockZone(float xLockMin_var, float xLockMax_var, float yLockMin_var, float yLockMax_var)
	{
		if (this == null)
		{
			return;
		}
		if (this.IsFreeModeManual)
		{
			return;
		}
		float num = this.xLockMin;
		float num2 = this.xLockMax;
		float num3 = this.yLockMin;
		float num4 = this.yLockMax;
		this.xLockMin = xLockMin_var;
		this.xLockMax = xLockMax_var;
		this.yLockMin = yLockMin_var;
		this.yLockMax = yLockMax_var;
		this.mode = CameraTarget.TargetMode.LOCK_ZONE;
		Vector3 position = base.transform.position;
		Vector3 position2 = base.transform.position;
		float x = this.heroTransform.position.x;
		float y = this.heroTransform.position.y;
		float num5 = x + this.xOffset + this.dashOffset;
		if (num5 < this.xLockMin)
		{
			num5 = this.xLockMin;
		}
		if (num5 > this.xLockMax)
		{
			num5 = this.xLockMax;
		}
		float num6 = x + this.xOffset + this.dashOffset;
		if (num6 < num)
		{
			num6 = num;
		}
		if (num6 > num2)
		{
			num6 = num2;
		}
		if (num5 != num6)
		{
			if (num6 - num5 < -9f || num6 - num5 > 9f)
			{
				this.dampTimeX = this.dampTimeSlower;
			}
			else
			{
				this.dampTimeX = this.dampTimeSlow;
			}
			this.slowTimer = this.slowTime;
		}
		float num7 = y + this.wallSprintOffset;
		if (num7 < this.yLockMin)
		{
			num7 = this.yLockMin;
		}
		if (num7 > this.yLockMax)
		{
			num7 = this.yLockMax;
		}
		float num8 = y + this.wallSprintOffset;
		if (num8 < num3)
		{
			num8 = num3;
		}
		if (num8 > num4)
		{
			num8 = num4;
		}
		if (num7 != num8)
		{
			if (num8 - num7 < -9f || num8 - num7 > 9f)
			{
				this.dampTimeY = this.dampTimeSlower;
			}
			else
			{
				this.dampTimeY = this.dampTimeSlow;
			}
			this.slowTimer = this.slowTime;
		}
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x000330A0 File Offset: 0x000312A0
	public void EnterLockZoneInstant(float xLockMin_var, float xLockMax_var, float yLockMin_var, float yLockMax_var)
	{
		if (this.IsFreeModeManual)
		{
			return;
		}
		this.xLockMin = xLockMin_var;
		this.xLockMax = xLockMax_var;
		this.yLockMin = yLockMin_var;
		this.yLockMax = yLockMax_var;
		this.mode = CameraTarget.TargetMode.LOCK_ZONE;
		if (base.transform.position.x < this.xLockMin)
		{
			base.transform.SetPositionX(this.xLockMin);
		}
		if (base.transform.position.x > this.xLockMax)
		{
			base.transform.SetPositionX(this.xLockMax);
		}
		if (base.transform.position.y < this.yLockMin)
		{
			base.transform.SetPositionY(this.yLockMin);
		}
		if (base.transform.position.y > this.yLockMax)
		{
			base.transform.SetPositionY(this.yLockMax);
		}
		this.stickToHeroX = true;
		this.stickToHeroY = true;
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0003318C File Offset: 0x0003138C
	public void ExitLockZone()
	{
		if (this.hero_ctrl == null)
		{
			return;
		}
		if (this.mode == CameraTarget.TargetMode.FREE)
		{
			return;
		}
		if (this.hero_ctrl.cState.hazardDeath || this.hero_ctrl.cState.dead || (this.hero_ctrl.transitionState != HeroTransitionState.WAITING_TO_TRANSITION && this.hero_ctrl.transitionState != HeroTransitionState.WAITING_TO_ENTER_LEVEL))
		{
			this.mode = CameraTarget.TargetMode.FREE;
		}
		else
		{
			this.mode = CameraTarget.TargetMode.FOLLOW_HERO;
		}
		float num = this.xLockMin;
		float num2 = this.xLockMax;
		float num3 = this.yLockMin;
		float num4 = this.yLockMax;
		Vector3 position = base.transform.position;
		Vector3 position2 = base.transform.position;
		Vector3 position3 = base.transform.position;
		float x = this.heroTransform.position.x;
		float y = this.heroTransform.position.y;
		this.xLockMin = 0f;
		this.xLockMax = this.cameraCtrl.xLimit;
		this.yLockMin = 0f;
		this.yLockMax = this.cameraCtrl.yLimit;
		float num5 = x + this.xOffset + this.dashOffset;
		if (num5 < this.xLockMin)
		{
			num5 = this.xLockMin;
		}
		if (num5 > this.xLockMax)
		{
			num5 = this.xLockMax;
		}
		float num6 = x + this.xOffset + this.dashOffset;
		if (num6 < num)
		{
			num6 = num;
		}
		if (num6 > num2)
		{
			num6 = num2;
		}
		if (num5 != num6)
		{
			if (num6 - num5 < -9f || num6 - num5 > 9f)
			{
				this.dampTimeX = this.dampTimeSlower;
			}
			else
			{
				this.dampTimeX = this.dampTimeSlow;
			}
			this.slowTimer = this.slowTime;
		}
		float num7 = y + this.wallSprintOffset;
		if (num7 < this.yLockMin)
		{
			num7 = this.yLockMin;
		}
		if (num7 > this.yLockMax)
		{
			num7 = this.yLockMax;
		}
		float num8 = y + this.wallSprintOffset;
		if (num8 < num3)
		{
			num8 = num3;
		}
		if (num8 > num4)
		{
			num8 = num4;
		}
		if (num7 != num8)
		{
			if (num8 - num7 < -9f || num8 - num7 > 9f)
			{
				this.dampTimeY = this.dampTimeSlower;
			}
			else
			{
				this.dampTimeY = this.dampTimeSlow;
			}
			this.slowTimer = this.slowTime;
		}
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x000333C4 File Offset: 0x000315C4
	private void SetDampTime()
	{
		if (this.slowTimer > 0f)
		{
			this.slowTimer -= Time.deltaTime;
		}
		else
		{
			if (this.dampTimeX > this.dampTimeNormal)
			{
				this.dampTimeX -= 0.4f * Time.deltaTime;
			}
			else if (this.dampTimeX < this.dampTimeNormal)
			{
				this.dampTimeX = this.dampTimeNormal;
			}
			if (this.dampTimeY > this.dampTimeNormal)
			{
				this.dampTimeY -= 0.4f * Time.deltaTime;
			}
			else if (this.dampTimeY < this.dampTimeNormal)
			{
				this.dampTimeY = this.dampTimeNormal;
			}
		}
		if (this.detachTimer > 0f)
		{
			this.detachTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x00033498 File Offset: 0x00031698
	public void SetSuperDash(bool active)
	{
		this.superDashing = active;
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x000334A1 File Offset: 0x000316A1
	public void SetSprint(bool active)
	{
		this.sprinting = active;
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x000334AA File Offset: 0x000316AA
	public void SetSliding(bool active)
	{
		this.sliding = active;
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x000334B3 File Offset: 0x000316B3
	public void SetHarpooning(bool active)
	{
		this.harpooning = active;
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x000334BC File Offset: 0x000316BC
	public void SetUpdraft(bool active)
	{
		this.ridingUpdraft = active;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x000334C5 File Offset: 0x000316C5
	public void SetSuperJump(bool active, float destinationY)
	{
		this.superJumping = active;
		this.superJumpDestinationY = destinationY;
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x000334D5 File Offset: 0x000316D5
	public void StartCamRising(float lookAhead, float destinationY)
	{
		this.camRising = true;
		this.risingLookAhead = lookAhead;
		this.risingDestinationY = destinationY;
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x000334EC File Offset: 0x000316EC
	public void StopCamRising()
	{
		this.camRising = false;
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x000334F5 File Offset: 0x000316F5
	public void SetWallSprint(bool active)
	{
		this.wallSprinting = active;
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x000334FE File Offset: 0x000316FE
	public void FreezeInPlace()
	{
		this.mode = CameraTarget.TargetMode.FREE;
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x00033507 File Offset: 0x00031707
	public void StartFreeMode(bool useXOffset)
	{
		this.mode = CameraTarget.TargetMode.FREE;
		this.IsFreeModeManual = true;
		this.ignoreXOffset = !useXOffset;
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00033521 File Offset: 0x00031721
	public void SetIgnoringXOffset(bool value)
	{
		this.ignoreXOffset = value;
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0003352A File Offset: 0x0003172A
	public void EndFreeMode()
	{
		this.mode = CameraTarget.TargetMode.FOLLOW_HERO;
		this.IsFreeModeManual = false;
		if (this.cameraCtrl.CurrentLockArea)
		{
			this.cameraCtrl.LockToArea(this.cameraCtrl.CurrentLockArea);
		}
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x00033562 File Offset: 0x00031762
	public void StartLockMode()
	{
		this.mode = CameraTarget.TargetMode.LOCK_ZONE;
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0003356B File Offset: 0x0003176B
	public void SetUmbrellaFloating(bool isFloating)
	{
		this.umbrellaFloating = isFloating;
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00033574 File Offset: 0x00031774
	public void EndFallStick()
	{
		this.fallStick = false;
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0003357D File Offset: 0x0003177D
	public void ShortDamp()
	{
		this.dampTimeX = this.dampTimeSlow;
		this.dampTimeY = this.dampTimeSlow;
		this.slowTimer = this.slowTimeShort;
		this.detachTimer = this.detachTimeShort;
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x000335AF File Offset: 0x000317AF
	public void ShortDetach()
	{
		this.detachTimer = this.detachTimeShort;
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x000335BD File Offset: 0x000317BD
	public void ShorterDetach()
	{
		this.detachTimer = 0.1f;
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x000335CA File Offset: 0x000317CA
	public void SetDetachedFromHero(bool detach)
	{
		this.detachedFromHero = detach;
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x000335D4 File Offset: 0x000317D4
	public void PositionToStart()
	{
		this.EndFallStick();
		float x = base.transform.position.x;
		Vector3 position = base.transform.position;
		float num = this.heroTransform.position.x;
		float num2 = this.heroTransform.position.y;
		Vector2 cameraOffset = this.GetCameraOffset();
		num += cameraOffset.x;
		num2 += cameraOffset.y;
		this.velocityX = Vector3.zero;
		this.velocityY = Vector3.zero;
		this.destination = this.heroTransform.position;
		if (this.IsIgnoringXOffset)
		{
			this.xOffset = 0f;
		}
		else
		{
			this.xOffset = (this.hero_ctrl.cState.facingRight ? 1f : -1f);
		}
		if (this.mode == CameraTarget.TargetMode.LOCK_ZONE)
		{
			if (num < this.xLockMin && this.hero_ctrl.cState.facingRight)
			{
				this.xOffset = num - x + 1f;
			}
			if (num > this.xLockMax && !this.hero_ctrl.cState.facingRight)
			{
				this.xOffset = num - x - 1f;
			}
			if (x + this.xOffset > this.xLockMax)
			{
				this.xOffset = this.xLockMax - x;
			}
			if (x + this.xOffset < this.xLockMin)
			{
				this.xOffset = this.xLockMin - x;
			}
		}
		if (this.xOffset < -this.xLookAhead)
		{
			this.xOffset = -this.xLookAhead;
		}
		if (this.xOffset > this.xLookAhead)
		{
			this.xOffset = this.xLookAhead;
		}
		this.destination.x = this.destination.x + this.xOffset;
		if (this.verboseMode)
		{
			Debug.LogFormat("CT PTS - xOffset: {0} HeroPos: {1}, {2}", new object[]
			{
				this.xOffset,
				num,
				num2
			});
		}
		if (this.mode == CameraTarget.TargetMode.FOLLOW_HERO)
		{
			if (this.verboseMode)
			{
				Debug.LogFormat("CT PTS - Follow Hero - CT Pos: {0}", new object[]
				{
					base.transform.position
				});
			}
			base.transform.position = this.cameraCtrl.KeepWithinSceneBounds(this.destination);
		}
		else if (this.mode == CameraTarget.TargetMode.LOCK_ZONE)
		{
			if (this.destination.x < this.xLockMin)
			{
				this.destination.x = this.xLockMin;
			}
			if (this.destination.x > this.xLockMax)
			{
				this.destination.x = this.xLockMax;
			}
			if (this.destination.y < this.yLockMin)
			{
				this.destination.y = this.yLockMin;
			}
			if (this.destination.y > this.yLockMax)
			{
				this.destination.y = this.yLockMax;
			}
			base.transform.position = this.destination;
			if (this.verboseMode)
			{
				Debug.LogFormat("CT PTS - Lock Zone - CT Pos: {0}", new object[]
				{
					base.transform.position
				});
			}
		}
		if (this.verboseMode)
		{
			Debug.LogFormat("CT - PTS: HeroPos: {0} Mode: {1} Dest: {2}", new object[]
			{
				this.heroTransform.position,
				this.mode,
				this.destination
			});
		}
		this.heroPrevPosition = this.heroTransform.position;
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x00033944 File Offset: 0x00031B44
	public void AddOffsetArea(CameraOffsetArea offsetArea)
	{
		int num = this.cameraOffsetAreas.IndexOf(offsetArea);
		if (num >= 0)
		{
			this.cameraOffsetAreas.RemoveAt(num);
		}
		this.cameraOffsetAreas.Add(offsetArea);
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0003397A File Offset: 0x00031B7A
	public void RemoveOffsetArea(CameraOffsetArea offsetArea)
	{
		this.cameraOffsetAreas.Remove(offsetArea);
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x00033989 File Offset: 0x00031B89
	public Vector2 GetCameraOffset()
	{
		if (this.cameraOffsetAreas.Count == 0)
		{
			return Vector2.zero;
		}
		return this.cameraOffsetAreas[this.cameraOffsetAreas.Count - 1].Offset;
	}

	// Token: 0x04000A9B RID: 2715
	private bool verboseMode;

	// Token: 0x04000A9C RID: 2716
	[HideInInspector]
	public GameManager gm;

	// Token: 0x04000A9D RID: 2717
	[HideInInspector]
	public HeroController hero_ctrl;

	// Token: 0x04000A9E RID: 2718
	private Transform heroTransform;

	// Token: 0x04000A9F RID: 2719
	public CameraController cameraCtrl;

	// Token: 0x04000AA0 RID: 2720
	public CameraTarget.TargetMode mode;

	// Token: 0x04000AA2 RID: 2722
	private bool ignoreXOffset;

	// Token: 0x04000AA3 RID: 2723
	public Vector3 destination;

	// Token: 0x04000AA4 RID: 2724
	private Vector3 velocityX;

	// Token: 0x04000AA5 RID: 2725
	private Vector3 velocityY;

	// Token: 0x04000AA6 RID: 2726
	public Vector3 heroPosition_prev;

	// Token: 0x04000AA7 RID: 2727
	public float xOffset;

	// Token: 0x04000AA8 RID: 2728
	public float dashOffset;

	// Token: 0x04000AA9 RID: 2729
	public float wallSprintOffset;

	// Token: 0x04000AAA RID: 2730
	public float fallOffset;

	// Token: 0x04000AAB RID: 2731
	public float fallOffset_multiplier;

	// Token: 0x04000AAC RID: 2732
	public float fallStickOffset;

	// Token: 0x04000AAD RID: 2733
	public float xLockMin;

	// Token: 0x04000AAE RID: 2734
	public float xLockMax;

	// Token: 0x04000AAF RID: 2735
	public float yLockMin;

	// Token: 0x04000AB0 RID: 2736
	public float yLockMax;

	// Token: 0x04000AB1 RID: 2737
	public bool enteredLeft;

	// Token: 0x04000AB2 RID: 2738
	public bool enteredRight;

	// Token: 0x04000AB3 RID: 2739
	public bool enteredTop;

	// Token: 0x04000AB4 RID: 2740
	public bool enteredBot;

	// Token: 0x04000AB5 RID: 2741
	public bool exitedLeft;

	// Token: 0x04000AB6 RID: 2742
	public bool exitedRight;

	// Token: 0x04000AB7 RID: 2743
	public bool exitedTop;

	// Token: 0x04000AB8 RID: 2744
	public bool exitedBot;

	// Token: 0x04000AB9 RID: 2745
	public bool superDashing;

	// Token: 0x04000ABA RID: 2746
	public bool sprinting;

	// Token: 0x04000ABB RID: 2747
	public bool harpooning;

	// Token: 0x04000ABC RID: 2748
	public bool sliding;

	// Token: 0x04000ABD RID: 2749
	public bool ridingUpdraft;

	// Token: 0x04000ABE RID: 2750
	public bool wallSprinting;

	// Token: 0x04000ABF RID: 2751
	public bool superJumping;

	// Token: 0x04000AC0 RID: 2752
	public bool camRising;

	// Token: 0x04000AC1 RID: 2753
	public bool umbrellaFloating;

	// Token: 0x04000AC2 RID: 2754
	public float umbrellaFloatTimer;

	// Token: 0x04000AC3 RID: 2755
	public float superJumpDestinationY;

	// Token: 0x04000AC4 RID: 2756
	public float risingDestinationY;

	// Token: 0x04000AC5 RID: 2757
	public float slowTime = 0.5f;

	// Token: 0x04000AC6 RID: 2758
	public float slowTimeShort = 0.25f;

	// Token: 0x04000AC7 RID: 2759
	public float detachTimeShort = 0.25f;

	// Token: 0x04000AC8 RID: 2760
	public float dampTimeNormal;

	// Token: 0x04000AC9 RID: 2761
	public float dampTimeSlow;

	// Token: 0x04000ACA RID: 2762
	public float dampTimeSlower;

	// Token: 0x04000ACB RID: 2763
	public float xLookAhead;

	// Token: 0x04000ACC RID: 2764
	public float runLookAhead;

	// Token: 0x04000ACD RID: 2765
	public float dashLookAhead;

	// Token: 0x04000ACE RID: 2766
	public float superDashLookAhead;

	// Token: 0x04000ACF RID: 2767
	public float sprintLookAhead;

	// Token: 0x04000AD0 RID: 2768
	public float harpoonLookAhead;

	// Token: 0x04000AD1 RID: 2769
	public float slidingLookAhead;

	// Token: 0x04000AD2 RID: 2770
	public float slidingLookAheadVertical;

	// Token: 0x04000AD3 RID: 2771
	public float updraftLookahead = 12f;

	// Token: 0x04000AD4 RID: 2772
	public float superJumpLookahead = 12f;

	// Token: 0x04000AD5 RID: 2773
	public float risingLookAhead;

	// Token: 0x04000AD6 RID: 2774
	public float wallSprintLookAhead;

	// Token: 0x04000AD7 RID: 2775
	private Vector3 heroPrevPosition;

	// Token: 0x04000AD8 RID: 2776
	private float previousTargetX;

	// Token: 0x04000AD9 RID: 2777
	private float dampTime;

	// Token: 0x04000ADA RID: 2778
	public float dampTimeX;

	// Token: 0x04000ADB RID: 2779
	public float dampTimeY;

	// Token: 0x04000ADC RID: 2780
	private float slowTimer;

	// Token: 0x04000ADD RID: 2781
	private float detachTimer;

	// Token: 0x04000ADE RID: 2782
	private float snapDistance = 0.15f;

	// Token: 0x04000ADF RID: 2783
	public float fallCatcher;

	// Token: 0x04000AE0 RID: 2784
	public bool detachedFromHero;

	// Token: 0x04000AE1 RID: 2785
	public bool stickToHeroX;

	// Token: 0x04000AE2 RID: 2786
	public bool stickToHeroY;

	// Token: 0x04000AE3 RID: 2787
	public bool enteredFromLockZone;

	// Token: 0x04000AE4 RID: 2788
	public bool fallStick;

	// Token: 0x04000AE5 RID: 2789
	private bool isGameplayScene;

	// Token: 0x04000AE6 RID: 2790
	private List<CameraOffsetArea> cameraOffsetAreas = new List<CameraOffsetArea>();

	// Token: 0x04000AE7 RID: 2791
	private Vector3 previousPosition;

	// Token: 0x04000AE8 RID: 2792
	private Vector3 previousHeroPosition;

	// Token: 0x02001498 RID: 5272
	public enum TargetMode
	{
		// Token: 0x040083D0 RID: 33744
		FOLLOW_HERO,
		// Token: 0x040083D1 RID: 33745
		LOCK_ZONE,
		// Token: 0x040083D2 RID: 33746
		BOSS,
		// Token: 0x040083D3 RID: 33747
		FREE
	}
}
