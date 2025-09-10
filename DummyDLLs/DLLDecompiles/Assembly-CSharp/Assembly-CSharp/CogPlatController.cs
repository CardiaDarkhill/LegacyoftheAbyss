using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004B8 RID: 1208
public class CogPlatController : MonoBehaviour
{
	// Token: 0x06002B97 RID: 11159 RVA: 0x000BF330 File Offset: 0x000BD530
	private void Start()
	{
		if (this.mainTicker)
		{
			this.mainTicker.ReceivedEvent += this.TickMain;
		}
		if (this.armTicker)
		{
			this.armTicker.ReceivedEvent += this.TickArms;
		}
	}

	// Token: 0x06002B98 RID: 11160 RVA: 0x000BF388 File Offset: 0x000BD588
	private void OnDestroy()
	{
		if (this.mainTicker)
		{
			this.mainTicker.ReceivedEvent -= this.TickMain;
		}
		if (this.armTicker)
		{
			this.armTicker.ReceivedEvent -= this.TickArms;
		}
	}

	// Token: 0x06002B99 RID: 11161 RVA: 0x000BF3E0 File Offset: 0x000BD5E0
	private void Update()
	{
		if (this.doRotateArms)
		{
			float num = Mathf.Min(this.armRotationDuration, this.armTicker.TickDelay);
			float num2 = this.rotateTimeElapsed / num;
			if (num2 >= 1f)
			{
				this.doRotateArms = false;
				this.EndRotation();
			}
			else
			{
				this.UpdateRotation(num2);
			}
			this.rotateTimeElapsed += Time.deltaTime;
		}
	}

	// Token: 0x06002B9A RID: 11162 RVA: 0x000BF44B File Offset: 0x000BD64B
	private void TickMain()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		this.OnTick.Invoke();
	}

	// Token: 0x06002B9B RID: 11163 RVA: 0x000BF461 File Offset: 0x000BD661
	private void TickArms()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		this.OnArmTick.Invoke();
		this.doRotateArms = true;
		this.rotateTimeElapsed = 0f;
		this.StartRotation();
	}

	// Token: 0x06002B9C RID: 11164 RVA: 0x000BF490 File Offset: 0x000BD690
	private void StartRotation()
	{
		if (this.isRotating)
		{
			this.EndRotation();
		}
		this.isRotating = true;
		CogPlatArm[] array = this.arms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StartRotation();
		}
	}

	// Token: 0x06002B9D RID: 11165 RVA: 0x000BF4D0 File Offset: 0x000BD6D0
	private void UpdateRotation(float time)
	{
		CogPlatArm[] array = this.arms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateRotation(time);
		}
	}

	// Token: 0x06002B9E RID: 11166 RVA: 0x000BF4FC File Offset: 0x000BD6FC
	private void EndRotation()
	{
		this.OnArmRotationEnd.Invoke();
		CogPlatArm[] array = this.arms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].EndRotation();
		}
		this.isRotating = false;
	}

	// Token: 0x04002CE9 RID: 11497
	[Header("Structure")]
	[SerializeField]
	private CogPlatArm[] arms;

	// Token: 0x04002CEA RID: 11498
	[Header("Parameters")]
	[SerializeField]
	private TimedTicker mainTicker;

	// Token: 0x04002CEB RID: 11499
	[SerializeField]
	private TimedTicker armTicker;

	// Token: 0x04002CEC RID: 11500
	[SerializeField]
	private float armRotationDuration;

	// Token: 0x04002CED RID: 11501
	[Space]
	public UnityEvent OnTick;

	// Token: 0x04002CEE RID: 11502
	public UnityEvent OnArmTick;

	// Token: 0x04002CEF RID: 11503
	public UnityEvent OnArmRotationEnd;

	// Token: 0x04002CF0 RID: 11504
	private bool doRotateArms;

	// Token: 0x04002CF1 RID: 11505
	private float rotateTimeElapsed;

	// Token: 0x04002CF2 RID: 11506
	private bool isRotating;
}
