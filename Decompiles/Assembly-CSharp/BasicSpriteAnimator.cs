using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000069 RID: 105
[RequireComponent(typeof(SpriteRenderer))]
public class BasicSpriteAnimator : MonoBehaviour
{
	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060002A4 RID: 676 RVA: 0x0000F06A File Offset: 0x0000D26A
	private int FrameCount
	{
		get
		{
			Sprite[] array = this.preFrames;
			int num = (array != null) ? array.Length : 0;
			Sprite[] array2 = this.frames;
			return num + ((array2 != null) ? array2.Length : 0);
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060002A5 RID: 677 RVA: 0x0000F08B File Offset: 0x0000D28B
	public float Length
	{
		get
		{
			return (float)this.FrameCount / this.fps;
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0000F09B File Offset: 0x0000D29B
	private void OnValidate()
	{
		this.maxTime = (float)this.FrameCount * (1f / this.fps);
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x0000F0B7 File Offset: 0x0000D2B7
	private void Awake()
	{
		this.OnValidate();
		this.rend = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x0000F0CB File Offset: 0x0000D2CB
	private void OnEnable()
	{
		this.isVisible = this.rend.isVisible;
		if (this.playOnEnable)
		{
			this.Play();
		}
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0000F0EC File Offset: 0x0000D2EC
	private void OnDisable()
	{
		this.StopAnimRoutine();
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0000F0F4 File Offset: 0x0000D2F4
	private void OnBecameVisible()
	{
		this.isVisible = true;
	}

	// Token: 0x060002AB RID: 683 RVA: 0x0000F0FD File Offset: 0x0000D2FD
	private void OnBecameInvisible()
	{
		this.isVisible = false;
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0000F106 File Offset: 0x0000D306
	public void Play()
	{
		this.PlayInternal(false);
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0000F10F File Offset: 0x0000D30F
	public void PlayRandom()
	{
		this.PlayInternal(true);
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0000F118 File Offset: 0x0000D318
	private void PlayInternal(bool forceStartRandom)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		this.StopAnimRoutine();
		if (this.FrameCount > 1)
		{
			this.animRoutine = base.StartCoroutine(this.Animate(forceStartRandom));
		}
		this.OnAnimStart.Invoke();
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0000F150 File Offset: 0x0000D350
	public void QueueStop()
	{
		this.queuedStop = true;
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x0000F159 File Offset: 0x0000D359
	public void StopImmediately()
	{
		this.StopAnimRoutine();
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0000F161 File Offset: 0x0000D361
	private void StopAnimRoutine()
	{
		if (this.animRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.animRoutine);
		this.animRoutine = null;
		this.AnimEnded();
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0000F185 File Offset: 0x0000D385
	private IEnumerator Animate(bool forceStartRandom)
	{
		this.queuedStop = false;
		float elapsedTime = 0f;
		bool hasStartedLoop = false;
		float randomValue = this.startDelay.GetRandomValue();
		if (randomValue > 0f)
		{
			this.rend.sprite = ((this.preFrames.Length != 0) ? this.preFrames[0] : this.frames[0]);
			yield return new WaitForSeconds(randomValue);
		}
		WaitUntil wait = this.visibilityCulling ? new WaitUntil(() => this.isVisible) : null;
		for (;;)
		{
			int num = Mathf.FloorToInt((float)this.FrameCount * (elapsedTime / this.maxTime));
			Sprite sprite;
			if (num < this.preFrames.Length)
			{
				sprite = this.preFrames[num];
			}
			else
			{
				if (this.frames.Length == 0)
				{
					goto IL_219;
				}
				if (!hasStartedLoop && (this.startRandom || forceStartRandom))
				{
					elapsedTime = Random.Range((float)this.preFrames.Length / this.fps, this.maxTime);
				}
				hasStartedLoop = true;
				num -= this.preFrames.Length;
				num %= this.frames.Length;
				sprite = this.frames[num];
			}
			if (this.rend.enabled)
			{
				this.rend.sprite = sprite;
			}
			double waitStartTime = Time.timeAsDouble;
			yield return wait;
			float num2 = (float)(Time.timeAsDouble - waitStartTime);
			elapsedTime += num2;
			if (elapsedTime >= this.maxTime)
			{
				if (this.queuedStop)
				{
					break;
				}
				if (!this.looping)
				{
					goto IL_219;
				}
				elapsedTime %= this.maxTime;
				float num3 = (float)this.preFrames.Length / this.fps;
				elapsedTime += num3;
			}
		}
		this.queuedStop = false;
		IL_219:
		this.AnimEnded();
		yield break;
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x0000F19B File Offset: 0x0000D39B
	private void AnimEnded()
	{
		this.animRoutine = null;
		this.OnAnimEnd.Invoke();
	}

	// Token: 0x04000241 RID: 577
	[SerializeField]
	private float fps = 30f;

	// Token: 0x04000242 RID: 578
	[Space]
	[SerializeField]
	private MinMaxFloat startDelay;

	// Token: 0x04000243 RID: 579
	[SerializeField]
	private Sprite[] preFrames;

	// Token: 0x04000244 RID: 580
	[SerializeField]
	private Sprite[] frames;

	// Token: 0x04000245 RID: 581
	[SerializeField]
	private bool startRandom = true;

	// Token: 0x04000246 RID: 582
	[SerializeField]
	private bool looping = true;

	// Token: 0x04000247 RID: 583
	[SerializeField]
	private bool playOnEnable = true;

	// Token: 0x04000248 RID: 584
	[SerializeField]
	private bool visibilityCulling;

	// Token: 0x04000249 RID: 585
	[Space]
	public UnityEvent OnAnimStart;

	// Token: 0x0400024A RID: 586
	public UnityEvent OnAnimEnd;

	// Token: 0x0400024B RID: 587
	private SpriteRenderer rend;

	// Token: 0x0400024C RID: 588
	private Coroutine animRoutine;

	// Token: 0x0400024D RID: 589
	private float maxTime;

	// Token: 0x0400024E RID: 590
	private bool queuedStop;

	// Token: 0x0400024F RID: 591
	private bool isVisible;
}
