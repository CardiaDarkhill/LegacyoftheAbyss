using System;
using UnityEngine;

// Token: 0x020007A1 RID: 1953
[CreateAssetMenu(fileName = "GamepadVibration", menuName = "Hollow Knight/Gamepad Vibration", order = 164)]
public class GamepadVibration : ScriptableObject
{
	// Token: 0x170007B5 RID: 1973
	// (get) Token: 0x060044E2 RID: 17634 RVA: 0x0012D355 File Offset: 0x0012B555
	public AnimationCurve SmallMotor
	{
		get
		{
			return this.smallMotor;
		}
	}

	// Token: 0x170007B6 RID: 1974
	// (get) Token: 0x060044E3 RID: 17635 RVA: 0x0012D35D File Offset: 0x0012B55D
	public AnimationCurve LargeMotor
	{
		get
		{
			return this.largeMotor;
		}
	}

	// Token: 0x170007B7 RID: 1975
	// (get) Token: 0x060044E4 RID: 17636 RVA: 0x0012D365 File Offset: 0x0012B565
	public float PlaybackRate
	{
		get
		{
			return this.playbackRate;
		}
	}

	// Token: 0x060044E5 RID: 17637 RVA: 0x0012D370 File Offset: 0x0012B570
	protected void Reset()
	{
		this.smallMotor = AnimationCurve.Constant(0f, 1f, 1f);
		this.largeMotor = AnimationCurve.Constant(0f, 1f, 1f);
		this.playbackRate = 1f;
	}

	// Token: 0x060044E6 RID: 17638 RVA: 0x0012D3BC File Offset: 0x0012B5BC
	public float GetDuration()
	{
		return Mathf.Max(GamepadVibration.GetDuration(this.smallMotor), GamepadVibration.GetDuration(this.largeMotor));
	}

	// Token: 0x060044E7 RID: 17639 RVA: 0x0012D3DC File Offset: 0x0012B5DC
	private static float GetDuration(AnimationCurve animationCurve)
	{
		if (animationCurve.length == 0)
		{
			return 0f;
		}
		return animationCurve[animationCurve.length - 1].time;
	}

	// Token: 0x040045C6 RID: 17862
	[SerializeField]
	private AnimationCurve smallMotor;

	// Token: 0x040045C7 RID: 17863
	[SerializeField]
	private AnimationCurve largeMotor;

	// Token: 0x040045C8 RID: 17864
	[SerializeField]
	[Range(0.01f, 5f)]
	private float playbackRate;
}
