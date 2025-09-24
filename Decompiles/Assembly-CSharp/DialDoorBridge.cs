using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004CE RID: 1230
public class DialDoorBridge : MonoBehaviour
{
	// Token: 0x06002C3E RID: 11326 RVA: 0x000C1B24 File Offset: 0x000BFD24
	private void Awake()
	{
		this.SetInitialRotation(this.startHorizontal);
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isRotated;
			};
			this.persistent.OnSetSaveState += this.SetInitialRotation;
		}
		foreach (Lever lever in this.leversClockwise)
		{
			if (lever)
			{
				lever.OnHitDelayed.AddListener(delegate()
				{
					this.OnLeverHitDelayed(1f);
				});
			}
		}
		foreach (Lever lever2 in this.leversCounterclockwise)
		{
			if (lever2)
			{
				lever2.OnHitDelayed.AddListener(delegate()
				{
					this.OnLeverHitDelayed(-1f);
				});
			}
		}
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x000C1BE8 File Offset: 0x000BFDE8
	private void Start()
	{
		foreach (Animator animator in this.doors)
		{
			animator.Play(DialDoorBridge._openAnim, 0, 1f);
			animator.Update(0f);
			animator.enabled = false;
		}
	}

	// Token: 0x06002C40 RID: 11328 RVA: 0x000C1C30 File Offset: 0x000BFE30
	private void SetInitialRotation(bool value)
	{
		this.isRotated = value;
		if (this.activeHorizontal)
		{
			this.activeHorizontal.SetActive(this.isRotated);
		}
		if (this.activeVertical)
		{
			this.activeVertical.SetActive(!this.isRotated);
		}
		if (this.isRotated)
		{
			this.front.SetLocalRotation2D(-90f);
			this.back.SetLocalRotation2D(90f);
			return;
		}
		this.front.SetLocalRotation2D(0f);
		this.back.SetLocalRotation2D(0f);
	}

	// Token: 0x06002C41 RID: 11329 RVA: 0x000C1CCC File Offset: 0x000BFECC
	private void OnLeverHitDelayed(float rotateDirection)
	{
		if (this.rotateRoutine != null)
		{
			return;
		}
		this.isRotated = !this.isRotated;
		this.rotateRoutine = base.StartCoroutine(this.MoveRotate(rotateDirection));
	}

	// Token: 0x06002C42 RID: 11330 RVA: 0x000C1CF9 File Offset: 0x000BFEF9
	private IEnumerator MoveRotate(float direction)
	{
		foreach (Animator animator in this.doors)
		{
			animator.enabled = true;
			animator.Play(DialDoorBridge._closeAnim);
		}
		this.OnDoorsClose.Invoke();
		if (this.activeHorizontal)
		{
			this.activeHorizontal.SetActive(true);
		}
		if (this.activeVertical)
		{
			this.activeVertical.SetActive(true);
		}
		yield return new WaitForSeconds(this.moveDelay);
		this.OnStartMoving.Invoke();
		Animator[] array = this.doors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(DialDoorBridge._movingAnim);
		}
		Vector3 initialFrontRotation = this.front.localEulerAngles;
		Vector3 initialBackRotation = this.back.localEulerAngles;
		Vector3 targetFrontRotation = initialFrontRotation;
		targetFrontRotation.z += -90f * direction;
		Vector3 targetBackRotation = initialBackRotation;
		targetBackRotation.z += 90f * direction;
		this.moveRumble.DoShake(this, true);
		bool hasSlammed = false;
		float elapsed = 0f;
		while (elapsed < this.moveDuration)
		{
			float t = elapsed / this.moveDuration;
			float t2 = this.moveCurve.Evaluate(t);
			this.front.localEulerAngles = Vector3.LerpUnclamped(initialFrontRotation, targetFrontRotation, t2);
			this.back.localEulerAngles = Vector3.LerpUnclamped(initialBackRotation, targetBackRotation, t2);
			if (!hasSlammed && t >= this.endSlamPoint)
			{
				hasSlammed = true;
				this.EndSlam();
			}
			yield return null;
			elapsed += Time.deltaTime;
			if (this.cogController)
			{
				this.cogController.AnimateRotation += t * direction;
			}
		}
		this.front.localEulerAngles = targetFrontRotation;
		this.back.localEulerAngles = targetBackRotation;
		if (!hasSlammed)
		{
			this.EndSlam();
		}
		yield return new WaitForSeconds(this.doorOpenDelay);
		if (this.activeHorizontal)
		{
			this.activeHorizontal.SetActive(this.isRotated);
		}
		if (this.activeVertical)
		{
			this.activeVertical.SetActive(!this.isRotated);
		}
		array = this.doors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(DialDoorBridge._openAnim);
		}
		this.OnDoorsOpen.Invoke();
		this.rotateRoutine = null;
		yield break;
	}

	// Token: 0x06002C43 RID: 11331 RVA: 0x000C1D0F File Offset: 0x000BFF0F
	private void EndSlam()
	{
		this.moveRumble.CancelShake();
		this.endSlamShake.DoShake(this, true);
		this.OnEndSlam.Invoke();
	}

	// Token: 0x04002DA3 RID: 11683
	private static readonly int _openAnim = Animator.StringToHash("Open");

	// Token: 0x04002DA4 RID: 11684
	private static readonly int _closeAnim = Animator.StringToHash("Close");

	// Token: 0x04002DA5 RID: 11685
	private static readonly int _movingAnim = Animator.StringToHash("Moving");

	// Token: 0x04002DA6 RID: 11686
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04002DA7 RID: 11687
	[SerializeField]
	private Lever[] leversClockwise;

	// Token: 0x04002DA8 RID: 11688
	[SerializeField]
	private Lever[] leversCounterclockwise;

	// Token: 0x04002DA9 RID: 11689
	[SerializeField]
	private GameObject activeVertical;

	// Token: 0x04002DAA RID: 11690
	[SerializeField]
	private GameObject activeHorizontal;

	// Token: 0x04002DAB RID: 11691
	[SerializeField]
	private Animator[] doors;

	// Token: 0x04002DAC RID: 11692
	[SerializeField]
	private Transform front;

	// Token: 0x04002DAD RID: 11693
	[SerializeField]
	private Transform back;

	// Token: 0x04002DAE RID: 11694
	[SerializeField]
	private float moveDelay;

	// Token: 0x04002DAF RID: 11695
	[SerializeField]
	private float doorOpenDelay;

	// Token: 0x04002DB0 RID: 11696
	[SerializeField]
	private AnimationCurve moveCurve;

	// Token: 0x04002DB1 RID: 11697
	[SerializeField]
	private float moveDuration;

	// Token: 0x04002DB2 RID: 11698
	[SerializeField]
	private CameraShakeTarget moveRumble;

	// Token: 0x04002DB3 RID: 11699
	[SerializeField]
	[Range(0f, 1f)]
	private float endSlamPoint = 1f;

	// Token: 0x04002DB4 RID: 11700
	[SerializeField]
	private CameraShakeTarget endSlamShake;

	// Token: 0x04002DB5 RID: 11701
	[SerializeField]
	private CogRotationController cogController;

	// Token: 0x04002DB6 RID: 11702
	[SerializeField]
	private bool startHorizontal;

	// Token: 0x04002DB7 RID: 11703
	[Space]
	public UnityEvent OnDoorsClose;

	// Token: 0x04002DB8 RID: 11704
	public UnityEvent OnDoorsOpen;

	// Token: 0x04002DB9 RID: 11705
	[Space]
	public UnityEvent OnStartMoving;

	// Token: 0x04002DBA RID: 11706
	[Space]
	public UnityEvent OnEndSlam;

	// Token: 0x04002DBB RID: 11707
	private bool isRotated;

	// Token: 0x04002DBC RID: 11708
	private Coroutine rotateRoutine;
}
