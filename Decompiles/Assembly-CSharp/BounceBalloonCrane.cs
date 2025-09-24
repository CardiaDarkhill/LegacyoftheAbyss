using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000499 RID: 1177
public class BounceBalloonCrane : UnlockablePropBase
{
	// Token: 0x06002A91 RID: 10897 RVA: 0x000B8894 File Offset: 0x000B6A94
	private void OnDrawGizmosSelected()
	{
		if (this.hangChain && this.hangChain.parent)
		{
			Gizmos.DrawWireSphere(this.hangChain.parent.TransformPoint(this.hangChainRetractedPos), 0.1f);
		}
	}

	// Token: 0x06002A92 RID: 10898 RVA: 0x000B88E5 File Offset: 0x000B6AE5
	private void Awake()
	{
		if (this.gears == null)
		{
			this.gears = Array.Empty<LoopRotator>();
		}
		this.anchors = base.GetComponentsInChildren<BounceBalloonCraneAnchor>();
	}

	// Token: 0x06002A93 RID: 10899 RVA: 0x000B8908 File Offset: 0x000B6B08
	private void Start()
	{
		if (this.startEffectsParent)
		{
			this.startEffectsParent.SetActive(false);
		}
		if (this.endEffectsParent)
		{
			this.endEffectsParent.SetActive(false);
		}
		if (!this.hangChain)
		{
			return;
		}
		this.hangChainEndPos = this.hangChain.localPosition;
		this.hangChain.SetLocalPosition2D(this.hangChainRetractedPos);
		BounceBalloonCraneAnchor[] array = this.anchors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetInactive();
		}
	}

	// Token: 0x06002A94 RID: 10900 RVA: 0x000B899C File Offset: 0x000B6B9C
	public override void Open()
	{
		if (!this.hangChain)
		{
			return;
		}
		this.StopMoveRoutines();
		this.hangChain.SetLocalPosition2D(this.hangChainRetractedPos);
		BounceBalloonCraneAnchor[] array = this.anchors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetInactive();
		}
		if (this.moveRoutines == null)
		{
			this.moveRoutines = new Stack<Coroutine>(4);
		}
		this.moveRoutines.Push(base.StartCoroutine(this.Move()));
	}

	// Token: 0x06002A95 RID: 10901 RVA: 0x000B8A18 File Offset: 0x000B6C18
	public override void Opened()
	{
		if (!this.hangChain)
		{
			return;
		}
		this.StopMoveRoutines();
		this.hangChain.SetLocalPosition2D(this.hangChainEndPos);
		LoopRotator[] array = this.gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		BounceBalloonCraneAnchor[] array2 = this.anchors;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetActive(true);
		}
	}

	// Token: 0x06002A96 RID: 10902 RVA: 0x000B8A86 File Offset: 0x000B6C86
	private void StopMoveRoutines()
	{
		if (this.moveRoutines == null)
		{
			return;
		}
		while (this.moveRoutines.Count > 0)
		{
			base.StopCoroutine(this.moveRoutines.Pop());
		}
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x000B8AB0 File Offset: 0x000B6CB0
	private IEnumerator Move()
	{
		LoopRotator[] array = this.gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StartRotation();
		}
		if (this.chainLoopSource)
		{
			this.chainLoopSource.Play();
		}
		if (this.startEffectsParent)
		{
			this.startEffectsParent.SetActive(false);
			this.startEffectsParent.SetActive(true);
		}
		if (this.beforeMoveDuration > 0f)
		{
			Vector2 initialPos = this.hangChain.localPosition;
			Vector2 targetPos = initialPos + this.beforeMoveOffset;
			yield return this.moveRoutines.PushReturn(this.StartTimerRoutine(0f, this.beforeMoveDuration, delegate(float time)
			{
				this.hangChain.SetLocalPosition2D(Vector2.LerpUnclamped(initialPos, targetPos, this.beforeMoveCurve.Evaluate(time)));
			}, null, null, false));
		}
		if (this.hangChainMoveSpeed > 0f)
		{
			Vector2 initialPos = this.hangChain.localPosition;
			float duration = (this.hangChainEndPos - this.hangChainRetractedPos).magnitude / this.hangChainMoveSpeed;
			yield return this.moveRoutines.PushReturn(this.StartTimerRoutine(0f, duration, delegate(float time)
			{
				float t = this.hangChainMoveCurve.Evaluate(time);
				this.hangChain.SetLocalPosition2D(Vector2.LerpUnclamped(initialPos, this.hangChainEndPos, t));
			}, null, null, false));
		}
		array = this.gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopRotation();
		}
		this.endCameraShake.DoShake(this, true);
		if (this.chainLoopSource)
		{
			this.chainLoopSource.Stop();
			if (this.chainEndSound)
			{
				this.chainLoopSource.PlayOneShot(this.chainEndSound);
			}
		}
		if (this.endEffectsParent)
		{
			this.endEffectsParent.SetActive(false);
			this.endEffectsParent.SetActive(true);
		}
		if (this.afterMoveDuration > 0f)
		{
			Vector2 initialPos = this.hangChain.localPosition;
			Vector2 targetPos = initialPos + this.afterMoveOffset;
			yield return this.moveRoutines.PushReturn(this.StartTimerRoutine(0f, this.afterMoveDuration, delegate(float time)
			{
				this.hangChain.SetLocalPosition2D(Vector2.LerpUnclamped(initialPos, targetPos, this.afterMoveCurve.Evaluate(time)));
			}, null, null, false));
		}
		if (this.balloonInflateWait > 0f)
		{
			yield return new WaitForSeconds(this.balloonInflateWait);
		}
		BounceBalloonCraneAnchor[] array2 = this.anchors;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetActive(false);
		}
		yield break;
	}

	// Token: 0x04002B3D RID: 11069
	[SerializeField]
	private Transform hangChain;

	// Token: 0x04002B3E RID: 11070
	[SerializeField]
	private Vector2 hangChainRetractedPos;

	// Token: 0x04002B3F RID: 11071
	[Space]
	[SerializeField]
	private Vector2 beforeMoveOffset;

	// Token: 0x04002B40 RID: 11072
	[SerializeField]
	private AnimationCurve beforeMoveCurve;

	// Token: 0x04002B41 RID: 11073
	[SerializeField]
	private float beforeMoveDuration;

	// Token: 0x04002B42 RID: 11074
	[Space]
	[SerializeField]
	private float hangChainMoveSpeed;

	// Token: 0x04002B43 RID: 11075
	[SerializeField]
	private AnimationCurve hangChainMoveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002B44 RID: 11076
	[Space]
	[SerializeField]
	private Vector2 afterMoveOffset;

	// Token: 0x04002B45 RID: 11077
	[SerializeField]
	private AnimationCurve afterMoveCurve;

	// Token: 0x04002B46 RID: 11078
	[SerializeField]
	private float afterMoveDuration;

	// Token: 0x04002B47 RID: 11079
	[Space]
	[SerializeField]
	private CameraShakeTarget endCameraShake;

	// Token: 0x04002B48 RID: 11080
	[Space]
	[SerializeField]
	private LoopRotator[] gears;

	// Token: 0x04002B49 RID: 11081
	[SerializeField]
	private GameObject startEffectsParent;

	// Token: 0x04002B4A RID: 11082
	[SerializeField]
	private GameObject endEffectsParent;

	// Token: 0x04002B4B RID: 11083
	[Space]
	[SerializeField]
	private float balloonInflateWait;

	// Token: 0x04002B4C RID: 11084
	[Space]
	[SerializeField]
	private AudioSource chainLoopSource;

	// Token: 0x04002B4D RID: 11085
	[SerializeField]
	private AudioClip chainEndSound;

	// Token: 0x04002B4E RID: 11086
	private Vector2 hangChainEndPos;

	// Token: 0x04002B4F RID: 11087
	private Stack<Coroutine> moveRoutines;

	// Token: 0x04002B50 RID: 11088
	private BounceBalloonCraneAnchor[] anchors;
}
