using System;
using UnityEngine;

// Token: 0x02000096 RID: 150
public class LoopRotator : MonoBehaviour
{
	// Token: 0x060004AF RID: 1199 RVA: 0x00019258 File Offset: 0x00017458
	private void OnValidate()
	{
		if (this.acceleration < 0f)
		{
			this.acceleration = 0f;
		}
		if (this.deceleration < 0f)
		{
			this.deceleration = 0f;
		}
		if (this.fpsLimit < 0f)
		{
			this.fpsLimit = 0f;
		}
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x000192AD File Offset: 0x000174AD
	private void Start()
	{
		this.currentSpeed = (this.isRotating ? this.rotationSpeed : 0f);
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x000192CA File Offset: 0x000174CA
	private void OnEnable()
	{
		if (this.startOnEnable)
		{
			this.StartRotation();
		}
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x000192DC File Offset: 0x000174DC
	private void Update()
	{
		if (Mathf.Abs(this.currentSpeed) <= 0.01f && !this.isRotating)
		{
			return;
		}
		float num;
		if (this.fpsLimit > 0f)
		{
			if (Time.timeAsDouble < this.nextUpdateTime)
			{
				return;
			}
			num = 1f / this.fpsLimit;
			this.nextUpdateTime = Time.timeAsDouble + (double)num;
		}
		else
		{
			num = Time.deltaTime;
		}
		float b = this.isReversed ? (-this.rotationSpeed) : this.rotationSpeed;
		if (this.isRotating)
		{
			if (this.acceleration > 0f)
			{
				this.currentSpeed = Mathf.Lerp(this.currentSpeed, b, this.acceleration * num);
			}
			else
			{
				this.currentSpeed = this.rotationSpeed;
			}
		}
		else if (this.deceleration > 0f)
		{
			this.currentSpeed = Mathf.Lerp(this.currentSpeed, 0f, this.deceleration * num);
		}
		else
		{
			this.currentSpeed = 0f;
		}
		Transform transform = base.transform;
		Vector3 localEulerAngles = transform.localEulerAngles;
		localEulerAngles.z += this.currentSpeed * num;
		transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x000193F7 File Offset: 0x000175F7
	[ContextMenu("Test Rotation", true)]
	[ContextMenu("Stop Rotation", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x000193FE File Offset: 0x000175FE
	[ContextMenu("Test Rotation")]
	public void StartRotation()
	{
		this.isRotating = true;
		this.isReversed = false;
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0001940E File Offset: 0x0001760E
	public void StartRotationReversed()
	{
		this.isRotating = true;
		this.isReversed = this.allowReversed;
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x00019423 File Offset: 0x00017623
	[ContextMenu("Stop Rotation")]
	public void StopRotation()
	{
		this.isRotating = false;
	}

	// Token: 0x0400047B RID: 1147
	[SerializeField]
	private float rotationSpeed = 10f;

	// Token: 0x0400047C RID: 1148
	[SerializeField]
	private bool allowReversed;

	// Token: 0x0400047D RID: 1149
	[SerializeField]
	private float acceleration = 10f;

	// Token: 0x0400047E RID: 1150
	[SerializeField]
	private float deceleration = 10f;

	// Token: 0x0400047F RID: 1151
	[SerializeField]
	private float fpsLimit;

	// Token: 0x04000480 RID: 1152
	[SerializeField]
	private bool startOnEnable;

	// Token: 0x04000481 RID: 1153
	private float currentSpeed;

	// Token: 0x04000482 RID: 1154
	private bool isRotating;

	// Token: 0x04000483 RID: 1155
	private bool isReversed;

	// Token: 0x04000484 RID: 1156
	private double nextUpdateTime;
}
