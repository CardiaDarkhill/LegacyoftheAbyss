using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x02000227 RID: 551
public class ColorCorrectionCurvesAnimator : MonoBehaviour
{
	// Token: 0x06001462 RID: 5218 RVA: 0x0005BB7C File Offset: 0x00059D7C
	private void Awake()
	{
		this.redPairedKeyframes = ColorCurvesManager.PairKeyframes(this.startState.RedChannel, this.endState.RedChannel);
		this.greenPairedKeyframes = ColorCurvesManager.PairKeyframes(this.startState.GreenChannel, this.endState.GreenChannel);
		this.bluePairedKeyframes = ColorCurvesManager.PairKeyframes(this.startState.BlueChannel, this.endState.BlueChannel);
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x0005BBEC File Offset: 0x00059DEC
	public void DoPlay()
	{
		if (this.routine != null)
		{
			base.StopCoroutine(this.routine);
		}
		this.routine = this.StartTimerRoutine(0f, this.getDurationFromCinematic ? this.getDurationFromCinematic.Duration : this.duration, delegate(float t)
		{
			this.curves.saturation = Mathf.Lerp(this.startState.Saturation, this.endState.Saturation, t);
			this.curves.redChannel = ColorCurvesManager.CreateCurveFromKeyframes(this.redPairedKeyframes, t);
			this.curves.greenChannel = ColorCurvesManager.CreateCurveFromKeyframes(this.greenPairedKeyframes, t);
			this.curves.blueChannel = ColorCurvesManager.CreateCurveFromKeyframes(this.bluePairedKeyframes, t);
			this.curves.UpdateParameters();
			this.curves.UpdateMaterial();
		}, null, delegate
		{
			this.routine = null;
		}, false);
	}

	// Token: 0x040012A1 RID: 4769
	[SerializeField]
	private ColorCorrectionCurves curves;

	// Token: 0x040012A2 RID: 4770
	[SerializeField]
	private ColorCorrectionCurvesAnimator.State startState;

	// Token: 0x040012A3 RID: 4771
	[SerializeField]
	private ColorCorrectionCurvesAnimator.State endState;

	// Token: 0x040012A4 RID: 4772
	[SerializeField]
	[Conditional("getDurationFromCinematic", false, false, false)]
	private float duration;

	// Token: 0x040012A5 RID: 4773
	[SerializeField]
	private CinematicPlayer getDurationFromCinematic;

	// Token: 0x040012A6 RID: 4774
	private Coroutine routine;

	// Token: 0x040012A7 RID: 4775
	private List<Keyframe[]> redPairedKeyframes;

	// Token: 0x040012A8 RID: 4776
	private List<Keyframe[]> greenPairedKeyframes;

	// Token: 0x040012A9 RID: 4777
	private List<Keyframe[]> bluePairedKeyframes;

	// Token: 0x02001541 RID: 5441
	[Serializable]
	private class State
	{
		// Token: 0x0400867F RID: 34431
		public float Saturation = 1f;

		// Token: 0x04008680 RID: 34432
		public AnimationCurve RedChannel = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});

		// Token: 0x04008681 RID: 34433
		public AnimationCurve GreenChannel = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});

		// Token: 0x04008682 RID: 34434
		public AnimationCurve BlueChannel = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});
	}
}
