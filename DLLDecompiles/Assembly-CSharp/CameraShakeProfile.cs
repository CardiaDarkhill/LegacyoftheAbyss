using System;
using UnityEngine;

// Token: 0x02000156 RID: 342
[CreateAssetMenu(menuName = "Camera/Camera Shake Profile")]
public class CameraShakeProfile : ScriptableObject, ICameraShake, ICameraShakeVibration
{
	// Token: 0x170000DC RID: 220
	// (get) Token: 0x06000A5E RID: 2654 RVA: 0x0002EC30 File Offset: 0x0002CE30
	public float Magnitude
	{
		get
		{
			return this.magnitude;
		}
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x06000A5F RID: 2655 RVA: 0x0002EC38 File Offset: 0x0002CE38
	public int FreezeFrames
	{
		get
		{
			if (this.ShowSwayFields())
			{
				return 0;
			}
			return this.freezeFrames;
		}
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x0002EC4A File Offset: 0x0002CE4A
	private bool ShowDecayField()
	{
		return this.type == CameraShakeProfile.CameraShakeTypes.Shake;
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x0002EC55 File Offset: 0x0002CE55
	private bool ShowDurationField()
	{
		return this.type != CameraShakeProfile.CameraShakeTypes.Rumble;
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x0002EC63 File Offset: 0x0002CE63
	private bool ShowSwayFields()
	{
		return this.type == CameraShakeProfile.CameraShakeTypes.Sway;
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x06000A63 RID: 2659 RVA: 0x0002EC6E File Offset: 0x0002CE6E
	public bool CanFinish
	{
		get
		{
			return this.type == CameraShakeProfile.CameraShakeTypes.Shake || (this.type == CameraShakeProfile.CameraShakeTypes.Sway && this.endAfterDuration);
		}
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000A64 RID: 2660 RVA: 0x0002EC8B File Offset: 0x0002CE8B
	public bool LimitUpdates
	{
		get
		{
			return this.type == CameraShakeProfile.CameraShakeTypes.Sway && !this.useRandomShake;
		}
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000A65 RID: 2661 RVA: 0x0002ECA1 File Offset: 0x0002CEA1
	public CameraShakeWorldForceIntensities WorldForceOnStart
	{
		get
		{
			return this.worldForceOnStart;
		}
	}

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x06000A66 RID: 2662 RVA: 0x0002ECA9 File Offset: 0x0002CEA9
	public ICameraShakeVibration CameraShakeVibration
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x0002ECAC File Offset: 0x0002CEAC
	public VibrationEmission PlayVibration(bool isRealtime)
	{
		VibrationEmission result;
		if (this.type == CameraShakeProfile.CameraShakeTypes.Rumble)
		{
			result = VibrationManager.PlayVibrationClipOneShot(this.vibration, null, true, "", isRealtime);
		}
		else
		{
			result = VibrationManager.PlayVibrationClipOneShot(this.vibration, null, false, "", isRealtime);
		}
		return result;
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x0002ED01 File Offset: 0x0002CF01
	public float GetVibrationStrength(float timeElapsed)
	{
		if (this.decayVibrations && this.type == CameraShakeProfile.CameraShakeTypes.Shake)
		{
			return this.decay.Evaluate(timeElapsed);
		}
		return 1f;
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x0002ED28 File Offset: 0x0002CF28
	public Vector2 GetOffset(float elapsedTime)
	{
		if (this.duration <= 0f)
		{
			return Vector2.zero;
		}
		float time = elapsedTime / this.duration;
		if (Time.frameCount != this.randomOffsetFrame)
		{
			this.randomOffsetFrame = Time.frameCount;
			this.randomOffsetThisFrame = Random.insideUnitCircle;
		}
		switch (this.type)
		{
		case CameraShakeProfile.CameraShakeTypes.Shake:
		{
			float num = this.decay.Evaluate(time);
			return this.randomOffsetThisFrame * (this.magnitude * num);
		}
		case CameraShakeProfile.CameraShakeTypes.Rumble:
			return this.randomOffsetThisFrame * this.magnitude;
		case CameraShakeProfile.CameraShakeTypes.Sway:
		{
			if (!this.useRandomShake)
			{
				return new Vector2(this.swayX.Evaluate(time), this.swayY.Evaluate(time)) * this.magnitude;
			}
			Vector2 vector = this.randomOffsetThisFrame * this.magnitude;
			return new Vector2(vector.x * this.swayX.Evaluate(time), vector.y * this.swayY.Evaluate(time));
		}
		default:
			return Vector2.zero;
		}
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x0002EE3C File Offset: 0x0002D03C
	public bool IsDone(float elapsedTime)
	{
		switch (this.type)
		{
		case CameraShakeProfile.CameraShakeTypes.Shake:
			return elapsedTime >= this.duration;
		case CameraShakeProfile.CameraShakeTypes.Rumble:
			return false;
		case CameraShakeProfile.CameraShakeTypes.Sway:
			return this.endAfterDuration && elapsedTime >= this.duration;
		default:
			return true;
		}
	}

	// Token: 0x040009DC RID: 2524
	[SerializeField]
	private CameraShakeProfile.CameraShakeTypes type;

	// Token: 0x040009DD RID: 2525
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowDecayField", true, true, true)]
	private AnimationCurve decay = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x040009DE RID: 2526
	[SerializeField]
	private bool decayVibrations;

	// Token: 0x040009DF RID: 2527
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowDurationField", true, true, true)]
	private float duration = 1f;

	// Token: 0x040009E0 RID: 2528
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowDurationField", true, true, true)]
	private bool endAfterDuration;

	// Token: 0x040009E1 RID: 2529
	[SerializeField]
	private float magnitude = 1f;

	// Token: 0x040009E2 RID: 2530
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowSwayFields", false, true, true)]
	private int freezeFrames = 1;

	// Token: 0x040009E3 RID: 2531
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowSwayFields", true, true, true)]
	private AnimationCurve swayX;

	// Token: 0x040009E4 RID: 2532
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowSwayFields", true, true, true)]
	private AnimationCurve swayY;

	// Token: 0x040009E5 RID: 2533
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowSwayFields", true, true, true)]
	private bool useRandomShake;

	// Token: 0x040009E6 RID: 2534
	[Space]
	[SerializeField]
	private CameraShakeWorldForceIntensities worldForceOnStart;

	// Token: 0x040009E7 RID: 2535
	[Space]
	[SerializeField]
	private VibrationData vibration;

	// Token: 0x040009E8 RID: 2536
	private Vector2 randomOffsetThisFrame;

	// Token: 0x040009E9 RID: 2537
	private int randomOffsetFrame;

	// Token: 0x0200148D RID: 5261
	private enum CameraShakeTypes
	{
		// Token: 0x040083A2 RID: 33698
		Shake,
		// Token: 0x040083A3 RID: 33699
		Rumble,
		// Token: 0x040083A4 RID: 33700
		Sway
	}
}
