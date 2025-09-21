using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.SharedUtils;
using TeamCherry.Splines;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000516 RID: 1302
public class LiftControl : MonoBehaviour, HeroPlatformStick.IMoveHooks
{
	// Token: 0x06002E7E RID: 11902 RVA: 0x000CC5D4 File Offset: 0x000CA7D4
	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		if (this.stops != null)
		{
			foreach (LiftControl.LiftStop liftStop in this.stops)
			{
				Vector3 original = position;
				float? y = new float?(liftStop.PosY);
				Vector3 vector = original.Where(null, y, null);
				Gizmos.DrawWireSphere(vector, 0.2f);
				foreach (TempPressurePlate tempPressurePlate in liftStop.CallPlates)
				{
					if (tempPressurePlate)
					{
						Gizmos.DrawLine(vector, tempPressurePlate.transform.position);
					}
				}
			}
		}
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x000CC6AC File Offset: 0x000CA8AC
	private void OnValidate()
	{
		if (this.stops != null)
		{
			foreach (LiftControl.LiftStop liftStop in this.stops)
			{
				if (!(liftStop.CallPlate == null))
				{
					liftStop.CallPlates.Add(liftStop.CallPlate);
					liftStop.CallPlate = null;
				}
			}
		}
		if (this.supportMechanism != null)
		{
			this.supportMechanisms.Add(this.supportMechanism);
			this.supportMechanism = null;
		}
	}

	// Token: 0x06002E80 RID: 11904 RVA: 0x000CC728 File Offset: 0x000CA928
	private void Awake()
	{
		this.OnValidate();
		this.keepWorldPositions = base.GetComponentsInChildren<KeepWorldPosition>();
		bool flag = false;
		foreach (LiftControl.LiftStop liftStop in this.stops)
		{
			for (int j = liftStop.CallPlates.Count - 1; j >= 0; j--)
			{
				if (liftStop.CallPlates[j] == null)
				{
					liftStop.CallPlates.RemoveAt(j);
				}
			}
			if (liftStop.CallPlateUnlocks)
			{
				flag = true;
			}
		}
		this.currentStop = this.defaultStop;
		if (this.unlockReceptacle)
		{
			this.unlockReceptacle.Unlocked += this.Unlock;
			this.unlockReceptacle.StartedUnlocked += delegate()
			{
				this.SetUnlocked(true);
			};
		}
		else if (!flag)
		{
			this.isUnlocked = true;
		}
		else if (this.persistentUnlocked)
		{
			this.persistentUnlocked.OnGetSaveState += delegate(out bool value)
			{
				value = this.isUnlocked;
			};
			this.persistentUnlocked.OnSetSaveState += this.SetUnlocked;
		}
		if (this.persistentStopState)
		{
			this.persistentStopState.OnGetSaveState += delegate(out int value)
			{
				value = this.currentStop;
			};
			this.persistentStopState.OnSetSaveState += delegate(int value)
			{
				int num = this.stops.Length;
				if (num == 0)
				{
					return;
				}
				if (!this.overridingDefaultStop)
				{
					this.currentStop = Mathf.Clamp(value, 0, num - 1);
				}
				this.SetInitialPos();
			};
		}
		if (this.doorCloseTrigger)
		{
			this.doorCloseTrigger.OnTriggerEntered += delegate(Collider2D <p0>, GameObject <p1>)
			{
				this.MoveToNextStop();
			};
		}
		if (this.doorCloseButton)
		{
			SimpleButton simpleButton = this.doorCloseButton;
			simpleButton.DepressedChange = (Action<bool>)Delegate.Combine(simpleButton.DepressedChange, new Action<bool>(delegate(bool value)
			{
				if (value)
				{
					this.MoveToNextStop();
				}
			}));
		}
		if (this.buttonRaiseExitTrigger && this.doorCloseButton)
		{
			this.buttonRaiseExitTrigger.OnTriggerExited += delegate(Collider2D <p0>, GameObject <p1>)
			{
				if (this.isUnlocked)
				{
					this.doorCloseButton.SetLocked(false);
				}
			};
		}
		bool flag2 = !this.unlockReceptacle;
		for (int k = 0; k < this.stops.Length; k++)
		{
			LiftControl.LiftStop stop = this.stops[k];
			if (stop.CallPlateUnlocks && flag2)
			{
				foreach (TempPressurePlate tempPressurePlate in stop.CallPlates)
				{
					tempPressurePlate.Activated += this.Unlock;
				}
			}
			int stopIndex = k;
			Action <>9__10;
			foreach (TempPressurePlate tempPressurePlate2 in stop.CallPlates)
			{
				TempPressurePlate capturedPlate = tempPressurePlate2;
				tempPressurePlate2.PreActivated += delegate()
				{
					this.justPressedPlate = capturedPlate;
					if (this.justPressedPlate)
					{
						foreach (TempPressurePlate tempPressurePlate4 in this.stops[stopIndex].CallPlates)
						{
							if (tempPressurePlate4 != this.justPressedPlate)
							{
								tempPressurePlate4.ActivateSilent();
							}
						}
					}
				};
				TempPressurePlate tempPressurePlate3 = tempPressurePlate2;
				Action value2;
				if ((value2 = <>9__10) == null)
				{
					value2 = (<>9__10 = delegate()
					{
						if (stopIndex != this.currentStop && stop.CallTeleportPoint.IsEnabled)
						{
							this.transform.SetPositionY(stop.CallTeleportPoint.Value);
						}
						this.MoveToStop(stopIndex, false);
					});
				}
				tempPressurePlate3.Activated += value2;
			}
		}
		this.mechanismRotators = (from m in this.supportMechanisms
		where m
		select m).SelectMany((GameObject m) => m.GetComponentsInChildren<LoopRotator>()).ToArray<LoopRotator>();
	}

	// Token: 0x06002E81 RID: 11905 RVA: 0x000CCAC0 File Offset: 0x000CACC0
	private void Start()
	{
		GameManager instance = GameManager.instance;
		PlayerData instance2 = PlayerData.instance;
		string text = instance.GetEntryGateName();
		if (string.IsNullOrEmpty(text))
		{
			string sceneNameString = instance.GetSceneNameString();
			if (instance2.respawnScene == sceneNameString)
			{
				text = instance2.respawnMarkerName;
			}
		}
		this.overridingDefaultStop = false;
		for (int i = 0; i < this.stops.Length; i++)
		{
			foreach (string text2 in this.stops[i].EntryGatesStartAt)
			{
				if (!string.IsNullOrEmpty(text2) && !(text2 != text))
				{
					this.currentStop = i;
					this.overridingDefaultStop = true;
					break;
				}
			}
			if (this.overridingDefaultStop)
			{
				break;
			}
		}
		this.SetInitialPos();
		if (this.activeWhileMoving)
		{
			this.activeWhileMoving.SetActive(false);
		}
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x000CCB98 File Offset: 0x000CAD98
	private void SetInitialPos()
	{
		LiftControl.LiftStop liftStop = this.stops[this.currentStop];
		base.transform.SetPositionY(liftStop.PosY);
		this.isSilent = true;
		if (this.isUnlocked)
		{
			this.SetOpenDoors(liftStop.OpenDoorLeft, liftStop.OpenDoorRight, true, true);
			this.ResetPlates();
		}
		else if (this.unlockReceptacle)
		{
			if (this.unlockReceptacleIsInside)
			{
				this.SetOpenDoors(liftStop.OpenDoorLeft, liftStop.OpenDoorRight, true, true);
			}
			else
			{
				this.SetOpenDoors(false, false, true, true);
			}
			LiftControl.LiftStop[] array = this.stops;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (TempPressurePlate tempPressurePlate in array[i].CallPlates)
				{
					tempPressurePlate.ActivateSilent();
				}
			}
		}
		else
		{
			this.SetOpenDoors(false, false, true, true);
			foreach (LiftControl.LiftStop liftStop2 in this.stops)
			{
				if (!liftStop2.CallPlateUnlocks)
				{
					foreach (TempPressurePlate tempPressurePlate2 in liftStop2.CallPlates)
					{
						tempPressurePlate2.ActivateSilent();
					}
				}
			}
		}
		this.isSilent = false;
	}

	// Token: 0x06002E83 RID: 11907 RVA: 0x000CCCF8 File Offset: 0x000CAEF8
	private void SetOpenDoors(bool left, bool right, bool isInstant, bool vibrate = true)
	{
		if (isInstant)
		{
			if (this.doorLeft)
			{
				this.doorLeft.Play(left ? LiftControl._doorOpenAnim : LiftControl._doorCloseAnim, 0, 1f);
			}
			if (this.doorRight)
			{
				this.doorRight.Play(right ? LiftControl._doorOpenAnim : LiftControl._doorCloseAnim, 0, 1f);
				return;
			}
		}
		else
		{
			if (this.doorLeft)
			{
				this.doorLeft.Play(left ? LiftControl._doorOpenAnim : LiftControl._doorCloseAnim);
			}
			if (this.doorRight)
			{
				this.doorRight.Play(right ? LiftControl._doorOpenAnim : LiftControl._doorCloseAnim);
			}
			this.doorsOpenSound.SpawnAndPlayOneShot(base.transform.position, vibrate, null);
		}
	}

	// Token: 0x06002E84 RID: 11908 RVA: 0x000CCDD0 File Offset: 0x000CAFD0
	public void MoveToNextStop()
	{
		if (this.moveRoutine != null)
		{
			return;
		}
		int num = this.currentStop + 1;
		if (num >= this.stops.Length)
		{
			num = 0;
		}
		this.MoveToStop(num, true);
	}

	// Token: 0x06002E85 RID: 11909 RVA: 0x000CCE04 File Offset: 0x000CB004
	public void StopMoving()
	{
		if (this.moveRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.moveRoutine);
		this.moveRoutine = null;
		LoopRotator[] array = this.mechanismRotators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopRotation();
		}
		if (this.movingLoop)
		{
			this.movingLoop.Stop();
		}
		if (this.movingVibrationPlayer)
		{
			this.movingVibrationPlayer.Stop();
		}
		if (this.activeWhileMoving)
		{
			this.activeWhileMoving.SetActive(false);
		}
	}

	// Token: 0x06002E86 RID: 11910 RVA: 0x000CCE94 File Offset: 0x000CB094
	public void MoveToStop(int stopIndex, bool isControllingCamera)
	{
		if (!this.isUnlocked)
		{
			return;
		}
		if (this.moveRoutine != null)
		{
			if (stopIndex == this.currentStop)
			{
				return;
			}
			base.StopCoroutine(this.moveRoutine);
		}
		if (stopIndex == this.currentStop)
		{
			this.ResetPlates();
			LiftControl.LiftStop liftStop = this.stops[this.currentStop];
			this.SetOpenDoors(liftStop.OpenDoorLeft, liftStop.OpenDoorRight, false, true);
			return;
		}
		this.currentStop = stopIndex;
		this.moveRoutine = base.StartCoroutine(this.MoveRoutine(isControllingCamera));
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x000CCF14 File Offset: 0x000CB114
	private IEnumerator MoveRoutine(bool isControllingCamera)
	{
		LiftControl.LiftStop stop = this.stops[this.currentStop];
		bool vibrate = isControllingCamera;
		this.SetOpenDoors(false, false, false, vibrate);
		this.activateSound.SpawnAndPlayOneShot(base.transform.position, null);
		this.startMoveCamShake.DoShake(this, true);
		if (this.activeWhileMoving)
		{
			this.activeWhileMoving.SetActive(true);
		}
		this.OnStartedMoving.Invoke();
		if (this.doorCloseButton)
		{
			this.doorCloseButton.SetLocked(true);
		}
		yield return new WaitForSeconds(this.moveDelay);
		if (this.bobPlat)
		{
			this.bobPlat.enabled = false;
		}
		if (this.movingLoop)
		{
			this.movingLoop.Play();
		}
		if (vibrate && this.movingVibrationPlayer)
		{
			this.movingVibrationPlayer.Play();
		}
		Vector2 startPos = base.transform.position;
		Vector2 original = startPos;
		float? y = new float?(stop.PosY);
		Vector2 targetPos = original.Where(null, y);
		float num = Vector2.Distance(targetPos, startPos);
		float duration = num / this.moveSpeed;
		LoopRotator[] array;
		float currentChainSpeed;
		if (targetPos.y > startPos.y)
		{
			array = this.mechanismRotators;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StartRotationReversed();
			}
			currentChainSpeed = -this.chainSplineOffsetSpeed;
		}
		else
		{
			array = this.mechanismRotators;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StartRotation();
			}
			currentChainSpeed = this.chainSplineOffsetSpeed;
		}
		CameraController camCtrl = GameCameras.instance.cameraController;
		float camOffset = 0f;
		if (isControllingCamera)
		{
			if (this.controlCamera)
			{
				camCtrl.SetMode(CameraController.CameraMode.PANNING);
				camOffset = camCtrl.camTarget.transform.position.y - startPos.y;
			}
			else
			{
				isControllingCamera = false;
			}
		}
		bool hasResetPlates = this.plateResetDistance <= Mathf.Epsilon;
		float elapsed = 0f;
		float unscaledElapsed = 0f;
		while (elapsed < duration)
		{
			Vector2 vector = Vector2.Lerp(startPos, targetPos, elapsed / duration);
			base.transform.SetPosition2D(vector);
			this.chainSplineOffset += currentChainSpeed * Time.deltaTime;
			this.PositionUpdated();
			if (!hasResetPlates && Vector2.Distance(startPos, vector) >= this.plateResetDistance)
			{
				hasResetPlates = true;
				this.ResetPlates();
			}
			if (isControllingCamera)
			{
				camCtrl.SnapTargetToY(this.clampCameraY.GetClampedBetween(vector.y + camOffset));
			}
			yield return null;
			float num2 = Time.deltaTime;
			if (this.speedCurveDuration > 0f)
			{
				float num3 = unscaledElapsed / this.speedCurveDuration;
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				unscaledElapsed += Time.deltaTime;
				num2 = Time.deltaTime * this.speedCurve.Evaluate(num3);
			}
			elapsed += num2;
		}
		if (isControllingCamera)
		{
			camCtrl.camTarget.PositionToStart();
			camCtrl.camTarget.destination.y = this.clampCameraY.GetClampedBetween(targetPos.y + camOffset);
			camCtrl.camTarget.transform.position = camCtrl.KeepWithinSceneBounds(camCtrl.camTarget.destination);
			camCtrl.SetMode(CameraController.CameraMode.PREVIOUS);
		}
		base.transform.SetPosition2D(targetPos);
		this.PositionUpdated();
		array = this.mechanismRotators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopRotation();
		}
		if (this.bobPlat)
		{
			this.bobPlat.enabled = true;
			if (this.bobSilent)
			{
				this.bobPlat.DoBobSilent();
			}
			else
			{
				this.bobPlat.DoBob(vibrate);
			}
		}
		if (this.movingLoop)
		{
			this.movingLoop.Stop();
		}
		if (this.movingVibrationPlayer)
		{
			this.movingVibrationPlayer.Stop();
		}
		this.arriveSound.SpawnAndPlayOneShot(base.transform.position, null);
		if (this.activeWhileMoving)
		{
			this.activeWhileMoving.SetActive(false);
		}
		this.OnStoppedMoving.Invoke();
		yield return new WaitForSeconds(this.endDelay);
		this.SetOpenDoors(stop.OpenDoorLeft, stop.OpenDoorRight, false, vibrate);
		if (this.doorCloseButton && (!this.buttonRaiseExitTrigger || this.justPressedPlate))
		{
			this.doorCloseButton.SetLocked(false);
		}
		this.moveRoutine = null;
		this.justPressedPlate = null;
		this.ResetPlates();
		yield break;
	}

	// Token: 0x06002E88 RID: 11912 RVA: 0x000CCF2C File Offset: 0x000CB12C
	private void PositionUpdated()
	{
		KeepWorldPosition[] array = this.keepWorldPositions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ForceUpdate();
		}
		foreach (SplineBase splineBase in this.chainSplines)
		{
			if (splineBase)
			{
				splineBase.TextureOffset = this.chainSplineOffset;
				splineBase.UpdateSpline();
			}
		}
	}

	// Token: 0x06002E89 RID: 11913 RVA: 0x000CCF89 File Offset: 0x000CB189
	public void SetUnlocked(bool value)
	{
		this.isUnlocked = value;
		if (this.isUnlocked)
		{
			this.OnUnlocked.Invoke();
		}
		this.SetInitialPos();
	}

	// Token: 0x06002E8A RID: 11914 RVA: 0x000CCFAC File Offset: 0x000CB1AC
	public void Unlock()
	{
		this.isUnlocked = true;
		this.OnUnlock.Invoke();
		bool vibrate = true;
		if (!this.bobSilent)
		{
			HeroController instance = HeroController.instance;
			if (instance)
			{
				Vector3 vector = base.transform.position - instance.transform.position;
				if (Mathf.Abs(vector.x) > 5f || Mathf.Abs(vector.y) > 5f)
				{
					vibrate = false;
				}
			}
		}
		if (this.bobPlat)
		{
			if (this.bobSilent)
			{
				this.bobPlat.DoBobSilent();
			}
			else
			{
				this.bobPlat.DoBob(vibrate);
			}
		}
		if (this.doorCloseButton && this.doorCloseButton.IsDepressed)
		{
			this.MoveToNextStop();
			return;
		}
		if (this.unlockReceptacle && !this.unlockReceptacleIsInside)
		{
			this.MoveToStop(this.currentStop, false);
		}
	}

	// Token: 0x06002E8B RID: 11915 RVA: 0x000CD098 File Offset: 0x000CB298
	private void ResetPlates()
	{
		bool flag = this.moveRoutine != null;
		int i = 0;
		while (i < this.stops.Length)
		{
			LiftControl.LiftStop liftStop = this.stops[i];
			if (this.currentStop == i && (!flag || this.justPressedPlate))
			{
				using (List<TempPressurePlate>.Enumerator enumerator = liftStop.CallPlates.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TempPressurePlate tempPressurePlate = enumerator.Current;
						tempPressurePlate.ActivateSilent();
					}
					goto IL_AE;
				}
				goto IL_66;
			}
			goto IL_66;
			IL_AE:
			i++;
			continue;
			IL_66:
			foreach (TempPressurePlate tempPressurePlate2 in liftStop.CallPlates)
			{
				if (this.isSilent)
				{
					tempPressurePlate2.DeactivateSilent();
				}
				else
				{
					tempPressurePlate2.Deactivate();
				}
			}
			goto IL_AE;
		}
	}

	// Token: 0x06002E8C RID: 11916 RVA: 0x000CD184 File Offset: 0x000CB384
	public void AddMoveHooks(Action onStartMove, Action onStopMove)
	{
		this.OnStartedMoving.AddListener(delegate()
		{
			onStartMove();
		});
		this.OnStoppedMoving.AddListener(delegate()
		{
			onStopMove();
		});
	}

	// Token: 0x040030FA RID: 12538
	private static readonly int _doorOpenAnim = Animator.StringToHash("Open");

	// Token: 0x040030FB RID: 12539
	private static readonly int _doorCloseAnim = Animator.StringToHash("Close");

	// Token: 0x040030FC RID: 12540
	[Header("Structure")]
	[SerializeField]
	private Animator doorLeft;

	// Token: 0x040030FD RID: 12541
	[SerializeField]
	private Animator doorRight;

	// Token: 0x040030FE RID: 12542
	[Space]
	[SerializeField]
	private PersistentBoolItem persistentUnlocked;

	// Token: 0x040030FF RID: 12543
	[SerializeField]
	private PersistentIntItem persistentStopState;

	// Token: 0x04003100 RID: 12544
	[SerializeField]
	private ItemReceptacle unlockReceptacle;

	// Token: 0x04003101 RID: 12545
	[SerializeField]
	private bool unlockReceptacleIsInside;

	// Token: 0x04003102 RID: 12546
	[Space]
	[SerializeField]
	private TriggerEnterEvent doorCloseTrigger;

	// Token: 0x04003103 RID: 12547
	[SerializeField]
	private SimpleButton doorCloseButton;

	// Token: 0x04003104 RID: 12548
	[SerializeField]
	private TriggerEnterEvent buttonRaiseExitTrigger;

	// Token: 0x04003105 RID: 12549
	[Space]
	[SerializeField]
	private CameraShakeTarget startMoveCamShake;

	// Token: 0x04003106 RID: 12550
	[SerializeField]
	private float moveDelay = 1f;

	// Token: 0x04003107 RID: 12551
	[SerializeField]
	private float moveSpeed = 10f;

	// Token: 0x04003108 RID: 12552
	[SerializeField]
	private AnimationCurve speedCurve = AnimationCurve.Constant(1f, 1f, 1f);

	// Token: 0x04003109 RID: 12553
	[SerializeField]
	private float speedCurveDuration;

	// Token: 0x0400310A RID: 12554
	[SerializeField]
	private bool controlCamera;

	// Token: 0x0400310B RID: 12555
	[SerializeField]
	private MinMaxFloat clampCameraY;

	// Token: 0x0400310C RID: 12556
	[SerializeField]
	private float endDelay = 1f;

	// Token: 0x0400310D RID: 12557
	[SerializeField]
	private float plateResetDistance;

	// Token: 0x0400310E RID: 12558
	[SerializeField]
	private LiftPlatform bobPlat;

	// Token: 0x0400310F RID: 12559
	[SerializeField]
	private bool bobSilent;

	// Token: 0x04003110 RID: 12560
	[SerializeField]
	private AudioEvent activateSound;

	// Token: 0x04003111 RID: 12561
	[SerializeField]
	private AudioEvent arriveSound;

	// Token: 0x04003112 RID: 12562
	[SerializeField]
	private AudioEvent doorsOpenSound;

	// Token: 0x04003113 RID: 12563
	[SerializeField]
	private AudioSource movingLoop;

	// Token: 0x04003114 RID: 12564
	[SerializeField]
	private VibrationPlayer movingVibrationPlayer;

	// Token: 0x04003115 RID: 12565
	[Space]
	public UnityEvent OnUnlock;

	// Token: 0x04003116 RID: 12566
	public UnityEvent OnUnlocked;

	// Token: 0x04003117 RID: 12567
	[Header("Scene Config")]
	[SerializeField]
	private LiftControl.LiftStop[] stops;

	// Token: 0x04003118 RID: 12568
	[SerializeField]
	private int defaultStop;

	// Token: 0x04003119 RID: 12569
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private GameObject supportMechanism;

	// Token: 0x0400311A RID: 12570
	[SerializeField]
	private List<GameObject> supportMechanisms;

	// Token: 0x0400311B RID: 12571
	[SerializeField]
	private SplineBase[] chainSplines;

	// Token: 0x0400311C RID: 12572
	[SerializeField]
	private float chainSplineOffsetSpeed;

	// Token: 0x0400311D RID: 12573
	[SerializeField]
	private GameObject activeWhileMoving;

	// Token: 0x0400311E RID: 12574
	[Space]
	public UnityEvent OnStartedMoving;

	// Token: 0x0400311F RID: 12575
	public UnityEvent OnStoppedMoving;

	// Token: 0x04003120 RID: 12576
	private bool isUnlocked;

	// Token: 0x04003121 RID: 12577
	private int currentStop;

	// Token: 0x04003122 RID: 12578
	private bool overridingDefaultStop;

	// Token: 0x04003123 RID: 12579
	private TempPressurePlate justPressedPlate;

	// Token: 0x04003124 RID: 12580
	private LoopRotator[] mechanismRotators;

	// Token: 0x04003125 RID: 12581
	private KeepWorldPosition[] keepWorldPositions;

	// Token: 0x04003126 RID: 12582
	private float chainSplineOffset;

	// Token: 0x04003127 RID: 12583
	private Coroutine moveRoutine;

	// Token: 0x04003128 RID: 12584
	private VibrationEmission moveLoopEmission;

	// Token: 0x04003129 RID: 12585
	private bool isSilent;

	// Token: 0x02001812 RID: 6162
	[Serializable]
	private class LiftStop
	{
		// Token: 0x0400908E RID: 37006
		public float PosY;

		// Token: 0x0400908F RID: 37007
		public bool OpenDoorLeft;

		// Token: 0x04009090 RID: 37008
		public bool OpenDoorRight;

		// Token: 0x04009091 RID: 37009
		[Space]
		public List<TempPressurePlate> CallPlates;

		// Token: 0x04009092 RID: 37010
		public bool CallPlateUnlocks;

		// Token: 0x04009093 RID: 37011
		[HideInInspector]
		[Obsolete]
		public TempPressurePlate CallPlate;

		// Token: 0x04009094 RID: 37012
		public string[] EntryGatesStartAt;

		// Token: 0x04009095 RID: 37013
		public OverrideFloat CallTeleportPoint;
	}
}
