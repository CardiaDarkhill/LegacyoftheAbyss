using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200048F RID: 1167
public class ArmSpikePlat : MonoBehaviour
{
	// Token: 0x170004FC RID: 1276
	// (get) Token: 0x06002A12 RID: 10770 RVA: 0x000B6846 File Offset: 0x000B4A46
	// (set) Token: 0x06002A13 RID: 10771 RVA: 0x000B684E File Offset: 0x000B4A4E
	private protected float PreviousDirection { protected get; private set; }

	// Token: 0x170004FD RID: 1277
	// (get) Token: 0x06002A14 RID: 10772 RVA: 0x000B6857 File Offset: 0x000B4A57
	public bool IsRotating
	{
		get
		{
			return this.rotateRoutine != null;
		}
	}

	// Token: 0x06002A15 RID: 10773 RVA: 0x000B6862 File Offset: 0x000B4A62
	private void Awake()
	{
		this.PreviousDirection = 1f;
	}

	// Token: 0x06002A16 RID: 10774 RVA: 0x000B686F File Offset: 0x000B4A6F
	private void OnEnable()
	{
		this.UpdatePlatActive();
		this.SetPlatMoving(false);
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x000B687E File Offset: 0x000B4A7E
	private void OnDisable()
	{
		if (this.rotateRoutine != null)
		{
			base.StopCoroutine(this.rotateRoutine);
			this.rotateRoutine = null;
		}
	}

	// Token: 0x06002A18 RID: 10776 RVA: 0x000B689B File Offset: 0x000B4A9B
	public void DoLandRotate()
	{
		if (this.IsPlatformLeft())
		{
			this.DoRotate(this.landStartDelay, 1f, true);
			return;
		}
		this.DoRotate(this.landStartDelay, -1f, true);
	}

	// Token: 0x06002A19 RID: 10777 RVA: 0x000B68CA File Offset: 0x000B4ACA
	private bool IsPlatformLeft()
	{
		return this.platform.position.x < this.pivot.position.x;
	}

	// Token: 0x06002A1A RID: 10778 RVA: 0x000B68F0 File Offset: 0x000B4AF0
	protected void DoRotate(float startDelay, float direction, bool doLandEffect = true)
	{
		if (this.rotateRoutine != null)
		{
			return;
		}
		if (doLandEffect && this.strikePrefab)
		{
			this.strikePrefab.Spawn(this.platform.position);
		}
		this.onActivate.Invoke();
		this.OnActivate();
		this.rotateRoutine = base.StartCoroutine(this.RotationSequence(startDelay, direction));
	}

	// Token: 0x06002A1B RID: 10779 RVA: 0x000B6952 File Offset: 0x000B4B52
	private IEnumerator RotationSequence(float startDelay, float rotateDirection)
	{
		if (this.platformJitter)
		{
			this.platformJitter.StartJitter();
		}
		yield return new WaitForSeconds(startDelay);
		if (this.platformJitter)
		{
			this.platformJitter.StopJitter();
		}
		this.SetPlatActive(false);
		this.SetPlatMoving(true);
		this.PreviousDirection = rotateDirection;
		float initialAngle = this.pivot.localEulerAngles.z;
		float targetAngle = initialAngle + rotateDirection * 180f;
		for (float elapsed = 0f; elapsed < this.rotateDuration; elapsed += Time.deltaTime)
		{
			this.pivot.SetRotation2D(Mathf.Lerp(initialAngle, targetAngle, elapsed / this.rotateDuration));
			yield return null;
		}
		this.pivot.SetRotation2D(targetAngle);
		this.UpdatePlatActive();
		this.SetPlatMoving(false);
		this.rotateRoutine = null;
		this.onEnd.Invoke();
		this.OnEnd();
		yield break;
	}

	// Token: 0x06002A1C RID: 10780 RVA: 0x000B696F File Offset: 0x000B4B6F
	private void UpdatePlatActive()
	{
		this.SetPlatActive(this.IsPlatformUpright());
	}

	// Token: 0x06002A1D RID: 10781 RVA: 0x000B6980 File Offset: 0x000B4B80
	private bool IsPlatformUpright()
	{
		bool flag = this.platform.eulerAngles.z.IsWithinTolerance(5f, 0f);
		if (base.transform.lossyScale.y < 0f)
		{
			flag = !flag;
		}
		return flag;
	}

	// Token: 0x06002A1E RID: 10782 RVA: 0x000B69CA File Offset: 0x000B4BCA
	private void SetPlatActive(bool value)
	{
		this.activePlatParts.SetAllActive(value);
		this.inactivePlatParts.SetAllActive(!value);
	}

	// Token: 0x06002A1F RID: 10783 RVA: 0x000B69E7 File Offset: 0x000B4BE7
	private void SetPlatMoving(bool isMoving)
	{
		this.staticActive.SetAllActive(!isMoving);
	}

	// Token: 0x06002A20 RID: 10784 RVA: 0x000B69F8 File Offset: 0x000B4BF8
	public void DoHitRotate(HitInstance.HitDirection hitDirection)
	{
		bool flag = this.IsPlatformLeft();
		this.IsPlatformUpright();
		switch (hitDirection)
		{
		case HitInstance.HitDirection.Left:
		case HitInstance.HitDirection.Right:
			break;
		case HitInstance.HitDirection.Up:
			if (flag)
			{
				this.DoRotate(this.hitStartDelay, -1f, true);
				return;
			}
			this.DoRotate(this.hitStartDelay, 1f, true);
			return;
		case HitInstance.HitDirection.Down:
			if (flag)
			{
				this.DoRotate(this.hitStartDelay, 1f, true);
				return;
			}
			this.DoRotate(this.hitStartDelay, -1f, true);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002A21 RID: 10785 RVA: 0x000B6A7B File Offset: 0x000B4C7B
	protected virtual void OnActivate()
	{
	}

	// Token: 0x06002A22 RID: 10786 RVA: 0x000B6A7D File Offset: 0x000B4C7D
	protected virtual void OnEnd()
	{
	}

	// Token: 0x04002A8A RID: 10890
	[SerializeField]
	private Transform pivot;

	// Token: 0x04002A8B RID: 10891
	[SerializeField]
	private Transform platform;

	// Token: 0x04002A8C RID: 10892
	[SerializeField]
	private JitterSelf platformJitter;

	// Token: 0x04002A8D RID: 10893
	[SerializeField]
	private GameObject[] activePlatParts;

	// Token: 0x04002A8E RID: 10894
	[SerializeField]
	private GameObject[] inactivePlatParts;

	// Token: 0x04002A8F RID: 10895
	[SerializeField]
	private GameObject[] staticActive;

	// Token: 0x04002A90 RID: 10896
	[SerializeField]
	private GameObject strikePrefab;

	// Token: 0x04002A91 RID: 10897
	[Space]
	[SerializeField]
	protected float hitStartDelay;

	// Token: 0x04002A92 RID: 10898
	[SerializeField]
	protected float landStartDelay;

	// Token: 0x04002A93 RID: 10899
	[SerializeField]
	private float rotateDuration;

	// Token: 0x04002A94 RID: 10900
	[Space]
	[SerializeField]
	private UnityEvent onActivate;

	// Token: 0x04002A95 RID: 10901
	[SerializeField]
	private UnityEvent onEnd;

	// Token: 0x04002A96 RID: 10902
	private Coroutine rotateRoutine;
}
