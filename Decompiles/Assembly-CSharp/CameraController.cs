using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x0200015C RID: 348
public class CameraController : MonoBehaviour
{
	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06000A89 RID: 2697 RVA: 0x0002F2A0 File Offset: 0x0002D4A0
	// (remove) Token: 0x06000A8A RID: 2698 RVA: 0x0002F2D8 File Offset: 0x0002D4D8
	public event Action PositionedAtHero;

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000A8B RID: 2699 RVA: 0x0002F30D File Offset: 0x0002D50D
	public float StartLockedTimer
	{
		get
		{
			return this.startLockedTimer;
		}
	}

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000A8C RID: 2700 RVA: 0x0002F315 File Offset: 0x0002D515
	// (set) Token: 0x06000A8D RID: 2701 RVA: 0x0002F31D File Offset: 0x0002D51D
	public bool AllowExitingSceneBounds { get; private set; }

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x06000A8E RID: 2702 RVA: 0x0002F326 File Offset: 0x0002D526
	public bool HasBeenPositionedAtHero
	{
		get
		{
			return this.gm != null && this.lastInPositionScene == this.gm.sceneName;
		}
	}

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x06000A8F RID: 2703 RVA: 0x0002F34E File Offset: 0x0002D54E
	// (set) Token: 0x06000A90 RID: 2704 RVA: 0x0002F355 File Offset: 0x0002D555
	public static bool IsPositioningCamera { get; private set; }

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000A91 RID: 2705 RVA: 0x0002F35D File Offset: 0x0002D55D
	public CameraLockArea CurrentLockArea
	{
		get
		{
			return this.currentLockArea;
		}
	}

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000A92 RID: 2706 RVA: 0x0002F365 File Offset: 0x0002D565
	// (set) Token: 0x06000A93 RID: 2707 RVA: 0x0002F36D File Offset: 0x0002D56D
	public bool IsBloomForced
	{
		get
		{
			return this.isBloomForced;
		}
		set
		{
			this.isBloomForced = value;
			this.ApplyEffectConfiguration();
		}
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x0002F37C File Offset: 0x0002D57C
	public void GameInit()
	{
		this.gm = GameManager.instance;
		this.cam = base.GetComponent<Camera>();
		this.cameraParent = base.transform.parent.transform;
		this.ApplyEffectConfiguration();
		this.gm.UnloadingLevel += this.OnLevelUnload;
		this.gm.OnFinishedEnteringScene += this.OnFinishedEnteringScene;
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x0002F3EA File Offset: 0x0002D5EA
	private void OnFinishedEnteringScene()
	{
		this.startLockedTimer = 0f;
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x0002F3F8 File Offset: 0x0002D5F8
	public void SceneInit()
	{
		this.ResetStartTimer();
		this.velocity = Vector3.zero;
		if (this.gm.IsGameplayScene())
		{
			this.isGameplayScene = true;
			if (this.hero_ctrl == null)
			{
				this.hero_ctrl = HeroController.instance;
				this.hero_ctrl.heroInPosition += this.PositionToHero;
			}
			this.lockZoneList = new List<CameraLockArea>();
			this.GetTilemapInfo();
			this.xLockMin = 0f;
			this.xLockMax = this.xLimit;
			this.yLockMin = 0f;
			this.yLockMax = this.yLimit;
			this.AllowExitingSceneBounds = false;
			this.dampTimeX = this.dampTime;
			this.dampTimeY = this.dampTime;
			this.maxVelocityCurrent = this.maxVelocity;
		}
		else
		{
			this.isGameplayScene = false;
		}
		this.ApplyEffectConfiguration();
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x0002F4D8 File Offset: 0x0002D6D8
	public void ApplyEffectConfiguration()
	{
		bool flag = this.gm.IsGameplayScene();
		MapZone currentMapZoneEnum = this.gm.GetCurrentMapZoneEnum();
		string sceneNameString = this.gm.GetSceneNameString();
		bool flag2 = Platform.Current.GraphicsTier > Platform.GraphicsTiers.Low;
		FastNoise component = base.GetComponent<FastNoise>();
		component.Init();
		component.enabled = false;
		base.GetComponent<NewCameraNoise>().enabled = (flag && flag2 && ConfigManager.IsNoiseEffectEnabled);
		BloomOptimized component2 = base.GetComponent<BloomOptimized>();
		component2.enabled = (flag2 || !flag || this.isBloomForced || currentMapZoneEnum == MapZone.MEMORY || (currentMapZoneEnum == MapZone.CRADLE && sceneNameString != "Tube_Hub") || currentMapZoneEnum == MapZone.CLOVER || CameraController._forceBloomScenes.Contains(sceneNameString));
		component2.BlurIterations = (flag2 ? component2.InitialIterations : 1);
		base.GetComponent<BrightnessEffect>().enabled = (flag && flag2);
		ColorCorrectionCurves component3 = base.GetComponent<ColorCorrectionCurves>();
		component3.enabled = true;
		component3.IsBloomActive = component2.enabled;
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x0002F5C4 File Offset: 0x0002D7C4
	private void LateUpdate()
	{
		if (Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		Vector3 position = base.transform.position;
		float x = position.x;
		float y = position.y;
		float z = position.z;
		Vector3 position2 = this.cameraParent.position;
		float x2 = position2.x;
		float y2 = position2.y;
		Vector3 position3 = this.camTarget.transform.position;
		if (this.isGameplayScene && this.mode != CameraController.CameraMode.FROZEN)
		{
			if (this.hero_ctrl.cState.lookingUp || this.hero_ctrl.cState.lookingUpRing)
			{
				if (this.camTarget.mode != CameraTarget.TargetMode.FREE)
				{
					this.dampTimeYSlowTimer = this.lookSlowTime;
					this.lookOffset = this.hero_ctrl.transform.position.y - position3.y + 6f;
				}
			}
			else if (this.hero_ctrl.cState.lookingDown || this.hero_ctrl.cState.lookingDownRing)
			{
				if (this.camTarget.mode != CameraTarget.TargetMode.FREE)
				{
					this.dampTimeYSlowTimer = this.lookSlowTime;
					this.lookOffset = this.hero_ctrl.transform.position.y - position3.y - 6f;
				}
			}
			else
			{
				this.lookOffset = 0f;
			}
			this.UpdateTargetDestinationDelta();
			Vector3 vector = this.cam.WorldToViewportPoint(position3);
			Vector3 vector2 = new Vector3(this.targetDeltaX, this.targetDeltaY, 0f) - this.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, vector.z));
			this.destination = new Vector3(x + vector2.x, y + vector2.y, z);
			if (this.mode == CameraController.CameraMode.LOCKED && this.currentLockArea != null)
			{
				if (this.lookOffset > Mathf.Epsilon)
				{
					if (this.currentLockArea.preventLookUp && this.destination.y > this.currentLockArea.lookYMax)
					{
						if (position.y > this.currentLockArea.lookYMax)
						{
							this.destination = new Vector3(this.destination.x, this.destination.y - this.lookOffset, this.destination.z);
						}
						else
						{
							this.destination = new Vector3(this.destination.x, this.currentLockArea.lookYMax, this.destination.z);
						}
					}
				}
				else if (this.lookOffset < -Mathf.Epsilon && this.currentLockArea.preventLookDown && this.destination.y < this.currentLockArea.lookYMin)
				{
					if (position.y < this.currentLockArea.lookYMin)
					{
						this.destination = new Vector3(this.destination.x, this.destination.y - this.lookOffset, this.destination.z);
					}
					else
					{
						this.destination = new Vector3(this.destination.x, this.currentLockArea.lookYMin, this.destination.z);
					}
				}
			}
			if (this.mode == CameraController.CameraMode.FOLLOWING || this.mode == CameraController.CameraMode.LOCKED)
			{
				this.destination = this.KeepWithinSceneBounds(this.destination);
			}
			position.x = Vector3.SmoothDamp(position, new Vector3(this.destination.x, y, z), ref this.velocityX, this.dampTimeX).x;
			if (this.dampTimeYSlowTimer > 0f)
			{
				position.y = Vector3.SmoothDamp(position, new Vector3(x, this.destination.y, z), ref this.velocityY, 0.15f).y;
				this.dampTimeYSlowTimer -= Time.deltaTime;
			}
			else if (this.isRising)
			{
				this.dampTimeRising -= Time.deltaTime / 10f;
				if (this.dampTimeRising < 0.03f)
				{
					this.dampTimeRising = 0.03f;
				}
				position.y = Vector3.SmoothDamp(position, new Vector3(x, this.destination.y, z), ref this.velocityY, this.dampTimeRising).y;
			}
			else if (this.isFalling)
			{
				this.dampTimeFalling -= Time.deltaTime / 10f;
				if (this.dampTimeFalling < 0.03f)
				{
					this.dampTimeFalling = 0.03f;
				}
				position.y = Vector3.SmoothDamp(position, new Vector3(x, this.destination.y, z), ref this.velocityY, this.dampTimeFalling).y;
			}
			else
			{
				position.y = Vector3.SmoothDamp(position, new Vector3(x, this.destination.y, z), ref this.velocityY, this.dampTimeY).y;
				if (this.dampTimeFalling != this.dampTimeY)
				{
					this.dampTimeFalling = this.dampTimeY;
				}
			}
			base.transform.SetPosition2D(position);
			x = position.x;
			y = position.y;
			if (this.velocity.magnitude > this.maxVelocityCurrent)
			{
				this.velocity = this.velocity.normalized * this.maxVelocityCurrent;
			}
		}
		if (this.isGameplayScene)
		{
			if (!this.AllowExitingSceneBounds)
			{
				if (x + x2 < 14.6f)
				{
					position.x = 14.6f;
				}
				if (position.x + x2 > this.xLimit)
				{
					position.x = this.xLimit;
				}
				if (y + y2 < 8.3f)
				{
					position.y = 8.3f;
				}
				if (position.y + y2 > this.yLimit)
				{
					position.y = this.yLimit;
				}
			}
			base.transform.SetPosition2D(position);
			if (this.startLockedTimer > 0f)
			{
				this.startLockedTimer -= Time.deltaTime;
				if (this.startLockedTimer <= 0f)
				{
					this.instantLockedArea.Clear();
				}
			}
		}
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x0002FBEE File Offset: 0x0002DDEE
	private void OnDisable()
	{
		if (this.hero_ctrl != null)
		{
			this.hero_ctrl.heroInPosition -= this.PositionToHero;
		}
		if (CameraController.lastPositioner == this)
		{
			CameraController.IsPositioningCamera = false;
		}
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x0002FC28 File Offset: 0x0002DE28
	public void FreezeInPlace(bool freezeTarget = false)
	{
		this.SetMode(CameraController.CameraMode.FROZEN);
		if (freezeTarget)
		{
			this.camTarget.FreezeInPlace();
		}
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x0002FC3F File Offset: 0x0002DE3F
	public void StopFreeze(bool stopFreezeTarget = false)
	{
		this.SetMode(CameraController.CameraMode.FOLLOWING);
		if (stopFreezeTarget)
		{
			this.camTarget.EndFreeMode();
		}
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x0002FC58 File Offset: 0x0002DE58
	public void FadeOut(CameraFadeType type)
	{
		this.SetMode(CameraController.CameraMode.FROZEN);
		switch (type)
		{
		case CameraFadeType.LEVEL_TRANSITION:
			this.fadeFSM.SendEventSafe("SCENE FADE OUT");
			return;
		case CameraFadeType.HERO_DEATH:
			this.fadeFSM.SendEventSafe("DEATH RESPAWN");
			return;
		case CameraFadeType.HERO_HAZARD_DEATH:
			this.fadeFSM.SendEventSafe("HAZARD FADE");
			return;
		case CameraFadeType.JUST_FADE:
			this.fadeFSM.SendEventSafe("SCENE FADE OUT");
			return;
		case CameraFadeType.START_FADE:
			this.fadeFSM.SendEventSafe("START FADE");
			return;
		case CameraFadeType.TO_MENU:
			this.fadeFSM.SendEventSafe("SCENE FADE TO MENU");
			return;
		default:
			return;
		}
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x0002FCF0 File Offset: 0x0002DEF0
	public void FadeSceneIn()
	{
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x0002FCF4 File Offset: 0x0002DEF4
	public void LockToArea(CameraLockArea lockArea)
	{
		if (!this.lockZoneList.Contains(lockArea) || lockArea == this.currentLockArea)
		{
			if (this.verboseMode)
			{
				Debug.LogFormat("LockZone Activated: {0} at startLockedTimer {1} ({2}s)", new object[]
				{
					lockArea.name,
					this.startLockedTimer,
					Time.timeSinceLevelLoad
				});
			}
			if (lockArea != this.currentLockArea)
			{
				this.lockZoneList.Add(lockArea);
			}
			if (this.currentLockArea != null && this.currentLockArea.priority > lockArea.priority)
			{
				return;
			}
			this.currentLockArea = lockArea;
			if (this.mode != CameraController.CameraMode.FROZEN)
			{
				this.SetMode(CameraController.CameraMode.LOCKED);
			}
			this.xLockMin = ((lockArea.cameraXMin < 14.6f) ? 14.6f : lockArea.cameraXMin);
			if (lockArea.cameraXMax < 0f || lockArea.cameraXMax > this.xLimit)
			{
				this.xLockMax = this.xLimit;
			}
			else
			{
				this.xLockMax = lockArea.cameraXMax;
			}
			this.yLockMin = ((lockArea.cameraYMin < 8.3f) ? 8.3f : lockArea.cameraYMin);
			if (lockArea.cameraYMax < 0f || lockArea.cameraYMax > this.yLimit)
			{
				this.yLockMax = this.yLimit;
			}
			else
			{
				this.yLockMax = lockArea.cameraYMax;
			}
			if (this.startLockedTimer > 0f && (this.hero_ctrl.transitionState != HeroTransitionState.WAITING_TO_TRANSITION || this.instantLockedArea.Contains(lockArea)))
			{
				Vector3 position = this.hero_ctrl.transform.position;
				position.x += this.camTarget.xOffset;
				this.camTarget.transform.SetPosition2D(this.KeepWithinLockBounds(position));
				this.camTarget.destination = this.camTarget.transform.position;
				this.camTarget.EnterLockZoneInstant(this.xLockMin, this.xLockMax, this.yLockMin, this.yLockMax);
				base.transform.SetPosition2D(this.KeepWithinLockBounds(position));
				this.destination = base.transform.position;
				this.instantLockedArea.Add(lockArea);
				lockArea.OnDestroyEvent += this.OnLockAreaDestroyed;
				return;
			}
			this.camTarget.EnterLockZone(this.xLockMin, this.xLockMax, this.yLockMin, this.yLockMax);
		}
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x0002FF74 File Offset: 0x0002E174
	public void ReleaseLock(CameraLockArea lockarea)
	{
		if (this == null)
		{
			return;
		}
		this.lockZoneList.Remove(lockarea);
		if (this.verboseMode)
		{
			Debug.Log("LockZone Released " + lockarea.name);
		}
		if (lockarea == this.currentLockArea)
		{
			if (this.lockZoneList.Count > 0)
			{
				int num = int.MinValue;
				for (int i = this.lockZoneList.Count - 1; i >= 0; i--)
				{
					CameraLockArea cameraLockArea = this.lockZoneList[i];
					if (cameraLockArea.priority > num)
					{
						num = cameraLockArea.priority;
						this.currentLockArea = cameraLockArea;
					}
				}
				this.xLockMin = this.currentLockArea.cameraXMin;
				this.xLockMax = this.currentLockArea.cameraXMax;
				this.yLockMin = this.currentLockArea.cameraYMin;
				this.yLockMax = this.currentLockArea.cameraYMax;
				this.xLockMin = ((this.currentLockArea.cameraXMin < 14.6f) ? 14.6f : this.currentLockArea.cameraXMin);
				if (this.currentLockArea.cameraXMax < 0f || this.currentLockArea.cameraXMax > this.xLimit)
				{
					this.xLockMax = this.xLimit;
				}
				else
				{
					this.xLockMax = this.currentLockArea.cameraXMax;
				}
				this.yLockMin = ((this.currentLockArea.cameraYMin < 8.3f) ? 8.3f : this.currentLockArea.cameraYMin);
				if (this.currentLockArea.cameraYMax < 0f || this.currentLockArea.cameraYMax > this.yLimit)
				{
					this.yLockMax = this.yLimit;
				}
				else
				{
					this.yLockMax = this.currentLockArea.cameraYMax;
				}
				this.camTarget.enteredFromLockZone = true;
				this.camTarget.EnterLockZone(this.xLockMin, this.xLockMax, this.yLockMin, this.yLockMax);
				if (this.startLockedTimer > 0f && this.hero_ctrl.transitionState != HeroTransitionState.WAITING_TO_TRANSITION)
				{
					Vector3 position = this.hero_ctrl.transform.position;
					position.x += this.camTarget.xOffset;
					this.camTarget.transform.SetPosition2D(this.KeepWithinLockBounds(position));
					this.camTarget.destination = this.camTarget.transform.position;
					this.camTarget.EnterLockZoneInstant(this.xLockMin, this.xLockMax, this.yLockMin, this.yLockMax);
					base.transform.SetPosition2D(this.KeepWithinLockBounds(position));
					this.destination = base.transform.position;
					return;
				}
			}
			else
			{
				this.lastLockPosition = base.transform.position;
				if (this.camTarget != null)
				{
					this.camTarget.enteredFromLockZone = false;
					this.camTarget.ExitLockZone();
				}
				this.currentLockArea = null;
				if (!this.hero_ctrl.cState.hazardDeath && !this.hero_ctrl.cState.dead && this.gm.GameState != GameState.EXITING_LEVEL && this.mode != CameraController.CameraMode.FROZEN)
				{
					this.SetMode(CameraController.CameraMode.FOLLOWING);
				}
				if (this.startLockedTimer > 0f)
				{
					Vector3 position2 = this.hero_ctrl.transform.position;
					position2.x += this.camTarget.xOffset;
					this.camTarget.transform.SetPosition2D(this.KeepWithinLockBounds(position2));
					this.camTarget.destination = this.camTarget.transform.position;
					this.camTarget.EnterLockZoneInstant(this.xLockMin, this.xLockMax, this.yLockMin, this.yLockMax);
					base.transform.SetPosition2D(this.KeepWithinLockBounds(position2));
					this.destination = base.transform.position;
					return;
				}
			}
		}
		else if (this.verboseMode)
		{
			Debug.Log("LockZone was not the current lock when removed.");
		}
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x00030384 File Offset: 0x0002E584
	public void ResetStartTimer()
	{
		this.startLockedTimer = 0.65f;
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x00030394 File Offset: 0x0002E594
	public void SnapTo(float x, float y)
	{
		Transform transform = this.camTarget.transform;
		transform.position = new Vector3(x, y, transform.position.z);
		Transform transform2 = base.transform;
		transform2.position = new Vector3(x, y, transform2.position.z);
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x000303E4 File Offset: 0x0002E5E4
	public void SnapToY(float y)
	{
		Transform transform = this.camTarget.transform;
		Vector3 position = transform.position;
		position = new Vector3(position.x, y, position.z);
		transform.position = position;
		Transform transform2 = base.transform;
		Vector3 position2 = transform2.position;
		position2 = new Vector3(position2.x, y, position2.z);
		transform2.position = position2;
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x00030444 File Offset: 0x0002E644
	public void SnapTargetToY(float y)
	{
		Transform transform = this.camTarget.transform;
		Vector3 position = transform.position;
		position = new Vector3(position.x, y, position.z);
		transform.position = position;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x0003047D File Offset: 0x0002E67D
	private void OnLockAreaDestroyed(CameraLockArea lockArea)
	{
		this.instantLockedArea.Remove(lockArea);
		lockArea.OnDestroyEvent -= this.OnLockAreaDestroyed;
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x0003049E File Offset: 0x0002E69E
	private void UpdateTargetDestinationDelta()
	{
		this.targetDeltaX = this.camTarget.transform.position.x;
		this.targetDeltaY = this.camTarget.transform.position.y + this.lookOffset;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x000304DD File Offset: 0x0002E6DD
	public void SetMode(CameraController.CameraMode newMode)
	{
		if (newMode != this.mode)
		{
			if (newMode == CameraController.CameraMode.PREVIOUS)
			{
				this.mode = this.prevMode;
				return;
			}
			this.prevMode = this.mode;
			this.mode = newMode;
		}
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x0003050C File Offset: 0x0002E70C
	public Vector3 KeepWithinSceneBounds(Vector3 targetDest)
	{
		Vector3 vector = targetDest;
		bool flag = false;
		bool flag2 = false;
		if (!this.AllowExitingSceneBounds)
		{
			if (vector.x < 14.6f)
			{
				vector = new Vector3(14.6f, vector.y, vector.z);
				flag = true;
				flag2 = true;
			}
			if (vector.x > this.xLimit)
			{
				vector = new Vector3(this.xLimit, vector.y, vector.z);
				flag = true;
				flag2 = true;
			}
			if (vector.y < 8.3f)
			{
				vector = new Vector3(vector.x, 8.3f, vector.z);
				flag = true;
			}
			if (vector.y > this.yLimit)
			{
				vector = new Vector3(vector.x, this.yLimit, vector.z);
				flag = true;
			}
		}
		this.atSceneBounds = flag;
		this.atHorizontalSceneBounds = flag2;
		return vector;
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x000305E0 File Offset: 0x0002E7E0
	private Vector2 KeepWithinSceneBounds(Vector2 targetDest)
	{
		bool flag = false;
		if (targetDest.x < 14.6f)
		{
			targetDest = new Vector2(14.6f, targetDest.y);
			flag = true;
		}
		if (targetDest.x > this.xLimit)
		{
			targetDest = new Vector2(this.xLimit, targetDest.y);
			flag = true;
		}
		if (targetDest.y < 8.3f)
		{
			targetDest = new Vector2(targetDest.x, 8.3f);
			flag = true;
		}
		if (targetDest.y > this.yLimit)
		{
			targetDest = new Vector2(targetDest.x, this.yLimit);
			flag = true;
		}
		this.atSceneBounds = flag;
		return targetDest;
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x00030680 File Offset: 0x0002E880
	private bool IsAtSceneBounds(Vector2 targetDest)
	{
		bool result = false;
		if (targetDest.x <= 14.6f)
		{
			result = true;
		}
		if (targetDest.x >= this.xLimit)
		{
			result = true;
		}
		if (targetDest.y <= 8.3f)
		{
			result = true;
		}
		if (targetDest.y >= this.yLimit)
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x000306D0 File Offset: 0x0002E8D0
	private bool IsAtHorizontalSceneBounds(Vector2 targetDest, out bool leftSide)
	{
		bool result = false;
		leftSide = false;
		if (targetDest.x <= 14.6f)
		{
			result = true;
			leftSide = true;
		}
		if (targetDest.x >= this.xLimit)
		{
			result = true;
			leftSide = false;
		}
		return result;
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00030708 File Offset: 0x0002E908
	private bool IsTouchingSides(float x)
	{
		bool result = false;
		if (x <= 14.6f)
		{
			result = true;
		}
		if (x >= this.xLimit)
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x00030730 File Offset: 0x0002E930
	public Vector2 KeepWithinLockBounds(Vector2 targetDest)
	{
		float x = targetDest.x;
		float y = targetDest.y;
		if (x < this.xLockMin)
		{
			x = this.xLockMin;
		}
		if (x > this.xLockMax)
		{
			x = this.xLockMax;
		}
		if (y < this.yLockMin)
		{
			y = this.yLockMin;
		}
		if (y > this.yLockMax)
		{
			y = this.yLockMax;
		}
		return new Vector2(x, y);
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x00030794 File Offset: 0x0002E994
	private void GetTilemapInfo()
	{
		this.tilemap = this.gm.tilemap;
		if (this.tilemap)
		{
			this.sceneWidth = (float)this.tilemap.width;
			this.sceneHeight = (float)this.tilemap.height;
			this.xLimit = this.sceneWidth - 14.6f;
			this.yLimit = this.sceneHeight - 8.3f;
		}
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x00030807 File Offset: 0x0002EA07
	public void SetAllowExitingSceneBounds(bool value)
	{
		this.AllowExitingSceneBounds = value;
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00030810 File Offset: 0x0002EA10
	public void ResetPositionedAtHero()
	{
		this.lastInPositionScene = string.Empty;
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x0003081D File Offset: 0x0002EA1D
	public void PositionToHero(bool forceDirect)
	{
		if (this.positionToHeroCoroutine != null)
		{
			base.StopCoroutine(this.positionToHeroCoroutine);
		}
		CameraController.lastPositioner = this;
		CameraController.IsPositioningCamera = true;
		this.positionToHeroCoroutine = base.StartCoroutine(this.DoPositionToHero(forceDirect));
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x00030852 File Offset: 0x0002EA52
	public void PositionToHeroInstant(bool forceDirect)
	{
		CameraController.lastPositioner = this;
		this.DoPositionToHeroInstant(forceDirect);
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x00030861 File Offset: 0x0002EA61
	private IEnumerator DoPositionToHero(bool forceDirect)
	{
		yield return new WaitForFixedUpdate();
		this.GetTilemapInfo();
		this.camTarget.PositionToStart();
		this.UpdateTargetDestinationDelta();
		CameraController.CameraMode previousMode = this.mode;
		this.SetMode(CameraController.CameraMode.FROZEN);
		Vector3 newPosition = this.KeepWithinSceneBounds(this.camTarget.transform.position);
		if (this.verboseMode)
		{
			Debug.LogFormat("CC - STR: NewPosition: {0} TargetDelta: ({1}, {2}) CT-XOffset: {3} HeroPos: {4} CT-Pos: {5}", new object[]
			{
				newPosition,
				this.targetDeltaX,
				this.targetDeltaY,
				this.camTarget.xOffset,
				this.hero_ctrl.transform.position,
				this.camTarget.transform.position
			});
		}
		if (forceDirect)
		{
			if (this.verboseMode)
			{
				Debug.Log("====> TEST 1a - ForceDirect Positioning Mode");
			}
			base.transform.SetPosition2D(newPosition);
		}
		else
		{
			if (this.verboseMode)
			{
				Debug.Log("====> TEST 1b - Normal Positioning Mode");
			}
			bool flag2;
			bool flag = this.IsAtHorizontalSceneBounds(newPosition, out flag2);
			if (this.currentLockArea != null)
			{
				if (this.verboseMode)
				{
					Debug.Log("====> TEST 3 - Lock Zone Active");
				}
				base.transform.SetPosition2D(this.KeepWithinLockBounds(newPosition));
			}
			else
			{
				if (this.verboseMode)
				{
					Debug.Log("====> TEST 4 - No Lock Zone");
				}
				base.transform.SetPosition2D(newPosition);
			}
			if (flag)
			{
				if (this.verboseMode)
				{
					Debug.Log("====> TEST 2 - At Horizontal Scene Bounds");
				}
				if ((flag2 && !this.hero_ctrl.cState.facingRight) || (!flag2 && this.hero_ctrl.cState.facingRight))
				{
					if (this.verboseMode)
					{
						Debug.Log("====> TEST 2a - Hero Facing Bounds");
					}
					base.transform.SetPosition2D(newPosition);
				}
				else
				{
					if (this.verboseMode)
					{
						Debug.Log("====> TEST 2b - Hero Facing Inwards");
					}
					if (this.IsTouchingSides(this.targetDeltaX))
					{
						if (this.verboseMode)
						{
							Debug.Log("Xoffset still touching sides");
						}
						base.transform.SetPosition2D(newPosition);
					}
					else
					{
						if (this.verboseMode)
						{
							Debug.LogFormat("Not Touching Sides with Xoffset CT: {0} Hero: {1}", new object[]
							{
								this.camTarget.transform.position,
								this.hero_ctrl.transform.position
							});
						}
						if (this.hero_ctrl.cState.facingRight)
						{
							base.transform.SetPosition2D(this.hero_ctrl.transform.position.x + 1f, newPosition.y);
						}
						else
						{
							base.transform.SetPosition2D(this.hero_ctrl.transform.position.x - 1f, newPosition.y);
						}
					}
				}
			}
		}
		this.destination = base.transform.position;
		this.velocity = Vector3.zero;
		this.velocityX = Vector3.zero;
		this.velocityY = Vector3.zero;
		yield return new WaitForSeconds(0.1f);
		if (previousMode == CameraController.CameraMode.FROZEN)
		{
			this.SetMode(CameraController.CameraMode.FOLLOWING);
			if (this.currentLockArea != null)
			{
				this.LockToArea(this.currentLockArea);
			}
		}
		else if (previousMode == CameraController.CameraMode.LOCKED)
		{
			if (this.currentLockArea != null)
			{
				this.SetMode(previousMode);
			}
			else
			{
				this.SetMode(CameraController.CameraMode.FOLLOWING);
			}
		}
		else
		{
			this.SetMode(previousMode);
		}
		if (this.verboseMode)
		{
			Debug.LogFormat("CC - PositionToHero FIN: - TargetDelta: ({0}, {1}) Destination: {2} CT-XOffset: {3} NewPosition: {4} CamTargetPos: {5} HeroPos: {6}", new object[]
			{
				this.targetDeltaX,
				this.targetDeltaY,
				this.destination,
				this.camTarget.xOffset,
				newPosition,
				this.camTarget.transform.position,
				this.hero_ctrl.transform.position
			});
		}
		if (this.gm != null)
		{
			this.lastInPositionScene = this.gm.sceneName;
		}
		CameraController.IsPositioningCamera = false;
		if (this.PositionedAtHero != null)
		{
			this.PositionedAtHero();
		}
		this.positionToHeroCoroutine = null;
		yield break;
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x00030878 File Offset: 0x0002EA78
	private void DoPositionToHeroInstant(bool forceDirect)
	{
		this.GetTilemapInfo();
		this.camTarget.PositionToStart();
		this.UpdateTargetDestinationDelta();
		CameraController.CameraMode cameraMode = this.mode;
		this.SetMode(CameraController.CameraMode.FROZEN);
		Vector3 position = this.camTarget.transform.position;
		this.camTarget.Update();
		Vector3 vector = this.KeepWithinSceneBounds(position);
		if (this.verboseMode)
		{
			Debug.LogFormat("CC - STR: NewPosition: {0} TargetDelta: ({1}, {2}) CT-XOffset: {3} HeroPos: {4} CT-Pos: {5}", new object[]
			{
				vector,
				this.targetDeltaX,
				this.targetDeltaY,
				this.camTarget.xOffset,
				this.hero_ctrl.transform.position,
				this.camTarget.transform.position
			});
		}
		if (forceDirect)
		{
			if (this.verboseMode)
			{
				Debug.Log("====> TEST 1a - ForceDirect Positioning Mode");
			}
			base.transform.SetPosition2D(vector);
		}
		else
		{
			if (this.verboseMode)
			{
				Debug.Log("====> TEST 1b - Normal Positioning Mode");
			}
			bool flag2;
			bool flag = this.IsAtHorizontalSceneBounds(vector, out flag2);
			if (this.currentLockArea != null)
			{
				if (this.verboseMode)
				{
					Debug.Log("====> TEST 3 - Lock Zone Active");
				}
				base.transform.SetPosition2D(this.KeepWithinLockBounds(vector));
			}
			else
			{
				if (this.verboseMode)
				{
					Debug.Log("====> TEST 4 - No Lock Zone");
				}
				base.transform.SetPosition2D(vector);
			}
			if (flag)
			{
				if (this.verboseMode)
				{
					Debug.Log("====> TEST 2 - At Horizontal Scene Bounds");
				}
				if ((flag2 && !this.hero_ctrl.cState.facingRight) || (!flag2 && this.hero_ctrl.cState.facingRight))
				{
					if (this.verboseMode)
					{
						Debug.Log("====> TEST 2a - Hero Facing Bounds");
					}
					base.transform.SetPosition2D(vector);
				}
				else
				{
					if (this.verboseMode)
					{
						Debug.Log("====> TEST 2b - Hero Facing Inwards");
					}
					if (this.IsTouchingSides(this.targetDeltaX))
					{
						if (this.verboseMode)
						{
							Debug.Log("Xoffset still touching sides");
						}
						base.transform.SetPosition2D(vector);
					}
					else
					{
						if (this.verboseMode)
						{
							Debug.LogFormat("Not Touching Sides with Xoffset CT: {0} Hero: {1}", new object[]
							{
								this.camTarget.transform.position,
								this.hero_ctrl.transform.position
							});
						}
						if (this.hero_ctrl.cState.facingRight)
						{
							base.transform.SetPosition2D(this.hero_ctrl.transform.position.x + 1f, vector.y);
						}
						else
						{
							base.transform.SetPosition2D(this.hero_ctrl.transform.position.x - 1f, vector.y);
						}
					}
				}
			}
		}
		this.destination = base.transform.position;
		this.velocity = Vector3.zero;
		this.velocityX = Vector3.zero;
		this.velocityY = Vector3.zero;
		if (cameraMode == CameraController.CameraMode.FROZEN)
		{
			this.SetMode(CameraController.CameraMode.FOLLOWING);
			if (this.currentLockArea != null)
			{
				this.LockToArea(this.currentLockArea);
			}
		}
		else if (cameraMode == CameraController.CameraMode.LOCKED)
		{
			if (this.currentLockArea != null)
			{
				this.SetMode(cameraMode);
			}
			else
			{
				this.SetMode(CameraController.CameraMode.FOLLOWING);
			}
		}
		else
		{
			this.SetMode(cameraMode);
		}
		if (this.verboseMode)
		{
			Debug.LogFormat("CC - PositionToHero FIN: - TargetDelta: ({0}, {1}) Destination: {2} CT-XOffset: {3} NewPosition: {4} CamTargetPos: {5} HeroPos: {6}", new object[]
			{
				this.targetDeltaX,
				this.targetDeltaY,
				this.destination,
				this.camTarget.xOffset,
				vector,
				this.camTarget.transform.position,
				this.hero_ctrl.transform.position
			});
		}
		if (this.gm != null)
		{
			this.lastInPositionScene = this.gm.sceneName;
		}
		if (!CameraController.IsPositioningCamera && this.PositionedAtHero != null)
		{
			this.PositionedAtHero();
		}
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00030CA7 File Offset: 0x0002EEA7
	private IEnumerator FadeInFailSafe()
	{
		yield return new WaitForSeconds(5f);
		if (this.fadeFSM.ActiveStateName != "Normal" && this.fadeFSM.ActiveStateName != "FadingOut")
		{
			Debug.LogFormat("Failsafe fade in activated. State: {0} Scene: {1}", new object[]
			{
				this.fadeFSM.ActiveStateName,
				this.gm.sceneName
			});
			this.fadeFSM.Fsm.Event("FADE SCENE IN");
		}
		yield break;
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00030CB6 File Offset: 0x0002EEB6
	private void StopFailSafe()
	{
		if (this.fadeInFailSafeCo != null)
		{
			base.StopCoroutine(this.fadeInFailSafeCo);
		}
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x00030CCC File Offset: 0x0002EECC
	private void OnLevelUnload()
	{
		AudioGroupManager.ClearAudioGroups();
		if (this == null)
		{
			return;
		}
		if (this.verboseMode)
		{
			Debug.Log("Removing cam locks. (" + this.lockZoneList.Count.ToString() + " total)");
		}
		while (this.lockZoneList.Count > 0)
		{
			this.ReleaseLock(this.lockZoneList[0]);
		}
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x00030D3C File Offset: 0x0002EF3C
	private void OnDestroy()
	{
		if (this.gm != null)
		{
			this.gm.UnloadingLevel -= this.OnLevelUnload;
			this.gm.OnFinishedEnteringScene -= this.OnFinishedEnteringScene;
		}
		if (CameraController.lastPositioner == this)
		{
			CameraController.lastPositioner = null;
		}
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00030D98 File Offset: 0x0002EF98
	public void ScreenFlash(Color colour)
	{
		this.screenFlash.gameObject.SetActive(true);
		this.screenFlash.SetColor(colour);
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00030DB7 File Offset: 0x0002EFB7
	public void ScreenFlashLifeblood()
	{
		this.ScreenFlash(this.flash_lifeblood);
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00030DC5 File Offset: 0x0002EFC5
	public void ScreenFlashPoison()
	{
		this.ScreenFlash(this.flash_poison);
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00030DD3 File Offset: 0x0002EFD3
	public void ScreenFlashBomb()
	{
		this.ScreenFlash(this.flash_bomb);
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00030DE1 File Offset: 0x0002EFE1
	public void ScreenFlashTrobbio()
	{
		this.ScreenFlash(this.flash_trobbio);
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x00030DEF File Offset: 0x0002EFEF
	public void ScreenFlashFrostStart()
	{
		this.ScreenFlash(this.flash_frostStart);
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x00030DFD File Offset: 0x0002EFFD
	public void ScreenFlashFrostDamage()
	{
		this.ScreenFlash(this.flash_frostDamage);
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x00030E0B File Offset: 0x0002F00B
	public void ScreenFlashPerfectDash()
	{
		this.ScreenFlash(this.flash_perfectDash);
	}

	// Token: 0x04000A01 RID: 2561
	private bool verboseMode;

	// Token: 0x04000A02 RID: 2562
	public CameraController.CameraMode mode;

	// Token: 0x04000A03 RID: 2563
	private CameraController.CameraMode prevMode;

	// Token: 0x04000A04 RID: 2564
	public bool atSceneBounds;

	// Token: 0x04000A05 RID: 2565
	public bool atHorizontalSceneBounds;

	// Token: 0x04000A06 RID: 2566
	private bool isGameplayScene;

	// Token: 0x04000A07 RID: 2567
	public Vector3 lastFramePosition;

	// Token: 0x04000A08 RID: 2568
	public Vector2 lastLockPosition;

	// Token: 0x04000A09 RID: 2569
	private Coroutine fadeInFailSafeCo;

	// Token: 0x04000A0A RID: 2570
	[Header("Inspector Variables")]
	public float dampTime;

	// Token: 0x04000A0B RID: 2571
	public float dampTimeX;

	// Token: 0x04000A0C RID: 2572
	public float dampTimeY;

	// Token: 0x04000A0D RID: 2573
	public float dampTimeYSlow = 0.2f;

	// Token: 0x04000A0E RID: 2574
	public float dampTimeYSlowTimer;

	// Token: 0x04000A0F RID: 2575
	public float lookSlowTime = 0.35f;

	// Token: 0x04000A10 RID: 2576
	public bool isFalling;

	// Token: 0x04000A11 RID: 2577
	public bool isRising;

	// Token: 0x04000A12 RID: 2578
	public float dampTimeFalling;

	// Token: 0x04000A13 RID: 2579
	public float dampTimeRising;

	// Token: 0x04000A14 RID: 2580
	public float heroBotYLimit;

	// Token: 0x04000A15 RID: 2581
	private float panTime;

	// Token: 0x04000A16 RID: 2582
	private float currentPanTime;

	// Token: 0x04000A17 RID: 2583
	private Vector3 velocity;

	// Token: 0x04000A18 RID: 2584
	private Vector3 velocityX;

	// Token: 0x04000A19 RID: 2585
	private Vector3 velocityY;

	// Token: 0x04000A1A RID: 2586
	public float fallOffset;

	// Token: 0x04000A1B RID: 2587
	public float fallOffset_multiplier;

	// Token: 0x04000A1C RID: 2588
	public Vector3 destination;

	// Token: 0x04000A1D RID: 2589
	public float maxVelocity;

	// Token: 0x04000A1E RID: 2590
	public float maxVelocityFalling;

	// Token: 0x04000A1F RID: 2591
	private float maxVelocityCurrent;

	// Token: 0x04000A20 RID: 2592
	private float horizontalOffset;

	// Token: 0x04000A21 RID: 2593
	public float lookOffset;

	// Token: 0x04000A22 RID: 2594
	private float startLockedTimer;

	// Token: 0x04000A23 RID: 2595
	private float targetDeltaX;

	// Token: 0x04000A24 RID: 2596
	private float targetDeltaY;

	// Token: 0x04000A25 RID: 2597
	[HideInInspector]
	public Vector2 panToTarget;

	// Token: 0x04000A26 RID: 2598
	public float sceneWidth;

	// Token: 0x04000A27 RID: 2599
	public float sceneHeight;

	// Token: 0x04000A28 RID: 2600
	public float xLimit;

	// Token: 0x04000A29 RID: 2601
	public float yLimit;

	// Token: 0x04000A2A RID: 2602
	private CameraLockArea currentLockArea;

	// Token: 0x04000A2B RID: 2603
	private Vector3 panStartPos;

	// Token: 0x04000A2C RID: 2604
	private Vector3 panEndPos;

	// Token: 0x04000A2D RID: 2605
	[SerializeField]
	private PlayMakerFSM fadeFSM;

	// Token: 0x04000A2E RID: 2606
	public Camera cam;

	// Token: 0x04000A2F RID: 2607
	private HeroController hero_ctrl;

	// Token: 0x04000A30 RID: 2608
	private GameManager gm;

	// Token: 0x04000A31 RID: 2609
	public tk2dTileMap tilemap;

	// Token: 0x04000A32 RID: 2610
	public CameraTarget camTarget;

	// Token: 0x04000A33 RID: 2611
	private Transform cameraParent;

	// Token: 0x04000A34 RID: 2612
	public List<CameraLockArea> lockZoneList;

	// Token: 0x04000A35 RID: 2613
	public float xLockMin;

	// Token: 0x04000A36 RID: 2614
	public float xLockMax;

	// Token: 0x04000A37 RID: 2615
	public float yLockMin;

	// Token: 0x04000A38 RID: 2616
	public float yLockMax;

	// Token: 0x04000A3A RID: 2618
	public SimpleFadeOut screenFlash;

	// Token: 0x04000A3B RID: 2619
	public Color flash_lifeblood;

	// Token: 0x04000A3C RID: 2620
	public Color flash_poison;

	// Token: 0x04000A3D RID: 2621
	public Color flash_bomb;

	// Token: 0x04000A3E RID: 2622
	public Color flash_trobbio;

	// Token: 0x04000A3F RID: 2623
	public Color flash_frostStart;

	// Token: 0x04000A40 RID: 2624
	public Color flash_frostDamage;

	// Token: 0x04000A41 RID: 2625
	public Color flash_perfectDash;

	// Token: 0x04000A42 RID: 2626
	private bool isBloomForced;

	// Token: 0x04000A43 RID: 2627
	private HashSet<CameraLockArea> instantLockedArea = new HashSet<CameraLockArea>();

	// Token: 0x04000A44 RID: 2628
	private Coroutine positionToHeroCoroutine;

	// Token: 0x04000A45 RID: 2629
	private string lastInPositionScene;

	// Token: 0x04000A46 RID: 2630
	private static CameraController lastPositioner;

	// Token: 0x04000A48 RID: 2632
	private static readonly List<string> _forceBloomScenes = new List<string>
	{
		"Weave_10",
		"Slab_10b",
		"Shellwood_11b",
		"Coral_Tower_01",
		"Ant_Queen",
		"Clover_01",
		"Clover_20"
	};

	// Token: 0x0200148F RID: 5263
	public enum CameraMode
	{
		// Token: 0x040083AB RID: 33707
		FROZEN,
		// Token: 0x040083AC RID: 33708
		FOLLOWING,
		// Token: 0x040083AD RID: 33709
		LOCKED,
		// Token: 0x040083AE RID: 33710
		PANNING,
		// Token: 0x040083AF RID: 33711
		FADEOUT,
		// Token: 0x040083B0 RID: 33712
		FADEIN,
		// Token: 0x040083B1 RID: 33713
		PREVIOUS
	}
}
