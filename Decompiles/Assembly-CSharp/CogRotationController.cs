using System;
using UnityEngine;

// Token: 0x02000074 RID: 116
[ExecuteInEditMode]
public class CogRotationController : MonoBehaviour
{
	// Token: 0x17000038 RID: 56
	// (get) Token: 0x0600034D RID: 845 RVA: 0x0001183D File Offset: 0x0000FA3D
	// (set) Token: 0x0600034E RID: 846 RVA: 0x00011845 File Offset: 0x0000FA45
	public float RotationMultiplier
	{
		get
		{
			return this.rotationMultiplier;
		}
		set
		{
			this.rotationMultiplier = value;
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x0600034F RID: 847 RVA: 0x0001184E File Offset: 0x0000FA4E
	// (set) Token: 0x06000350 RID: 848 RVA: 0x00011856 File Offset: 0x0000FA56
	public float AnimateRotation
	{
		get
		{
			return this.animateRotation;
		}
		set
		{
			this.animateRotation = value;
		}
	}

	// Token: 0x06000351 RID: 849 RVA: 0x00011860 File Offset: 0x0000FA60
	private void OnEnable()
	{
		this.capturedAnimateRotation = 0f;
		this.externalRotation = 0f;
		this.oldAnimateRotation = 0f;
		this.speedAnimation = 0f;
		this.queueUpdateRotation = true;
		this.nextUpdateTime = 0.0;
		this.ApplyRotation();
	}

	// Token: 0x06000352 RID: 850 RVA: 0x000118B8 File Offset: 0x0000FAB8
	private void LateUpdate()
	{
		if (Math.Abs(this.animateRotation - this.oldAnimateRotation) > 0.0001f)
		{
			this.oldAnimateRotation = this.animateRotation;
			this.queueUpdateRotation = true;
		}
		if (Math.Abs(this.rotationSpeed) > Mathf.Epsilon)
		{
			this.speedAnimation += this.rotationSpeed * Time.deltaTime * this.rotationMultiplier;
			this.queueUpdateRotation = true;
		}
		if (this.queueUpdateRotation)
		{
			this.ApplyRotation();
		}
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00011938 File Offset: 0x0000FB38
	public void ApplyRotation(float rotation)
	{
		this.externalRotation = rotation;
		this.queueUpdateRotation = true;
		this.ApplyRotation();
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00011950 File Offset: 0x0000FB50
	private void ApplyRotation()
	{
		float num;
		if (this.scaleFpsLimit)
		{
			num = this.fpsLimit * this.rotationMultiplier;
		}
		else
		{
			num = this.fpsLimit;
		}
		if (num > 0f)
		{
			if (Time.timeAsDouble < this.nextUpdateTime)
			{
				return;
			}
			this.nextUpdateTime = Time.timeAsDouble + (double)(1f / num);
		}
		float num2 = this.externalRotation + this.animateRotation + this.capturedAnimateRotation + this.speedAnimation;
		foreach (CogRotationController.Cog cog in this.cogs)
		{
			cog.Transform.SetLocalRotation2D(num2 * cog.RotationSpeed);
		}
		this.queueUpdateRotation = false;
	}

	// Token: 0x06000355 RID: 853 RVA: 0x000119F8 File Offset: 0x0000FBF8
	public void ResetNextUpdateTime()
	{
		this.nextUpdateTime = 0.0;
	}

	// Token: 0x06000356 RID: 854 RVA: 0x00011A09 File Offset: 0x0000FC09
	public void CaptureAnimateRotation()
	{
		this.capturedAnimateRotation += this.animateRotation;
		this.animateRotation = 0f;
		this.oldAnimateRotation = 0f;
		this.ApplyRotation();
	}

	// Token: 0x06000357 RID: 855 RVA: 0x00011A3A File Offset: 0x0000FC3A
	public void SetRotationSpeed(float value)
	{
		this.rotationSpeed = value;
	}

	// Token: 0x040002FA RID: 762
	[SerializeField]
	private CogRotationController.Cog[] cogs;

	// Token: 0x040002FB RID: 763
	[Space]
	[SerializeField]
	private float rotationMultiplier = 1f;

	// Token: 0x040002FC RID: 764
	[SerializeField]
	private float fpsLimit;

	// Token: 0x040002FD RID: 765
	[SerializeField]
	private bool scaleFpsLimit;

	// Token: 0x040002FE RID: 766
	[Space]
	[SerializeField]
	private float animateRotation;

	// Token: 0x040002FF RID: 767
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04000300 RID: 768
	private float capturedAnimateRotation;

	// Token: 0x04000301 RID: 769
	private float externalRotation;

	// Token: 0x04000302 RID: 770
	private float oldAnimateRotation;

	// Token: 0x04000303 RID: 771
	private float speedAnimation;

	// Token: 0x04000304 RID: 772
	private bool queueUpdateRotation;

	// Token: 0x04000305 RID: 773
	private double nextUpdateTime;

	// Token: 0x020013F0 RID: 5104
	[Serializable]
	private class Cog
	{
		// Token: 0x04008143 RID: 33091
		public Transform Transform;

		// Token: 0x04008144 RID: 33092
		public float RotationSpeed;
	}
}
