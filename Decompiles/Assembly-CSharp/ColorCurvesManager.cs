using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x02000228 RID: 552
[RequireComponent(typeof(ColorCorrectionCurves))]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Dynamic Color Correction (Curves, Saturation)")]
public class ColorCurvesManager : MonoBehaviour
{
	// Token: 0x06001467 RID: 5223 RVA: 0x0005BCF8 File Offset: 0x00059EF8
	public void SetFactor(float factor)
	{
		this.Factor = factor;
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x0005BD01 File Offset: 0x00059F01
	public void SetSaturationA(float saturationA)
	{
		this.SaturationA = saturationA;
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x0005BD0A File Offset: 0x00059F0A
	public void SetSaturationB(float saturationB)
	{
		this.SaturationB = saturationB;
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x0005BD13 File Offset: 0x00059F13
	private void Start()
	{
		this.LastFactor = this.Factor;
		this.LastSaturationA = this.SaturationA;
		this.LastSaturationB = this.SaturationB;
		this.CurvesScript = base.GetComponent<ColorCorrectionCurves>();
		this.PairCurvesKeyframes();
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x0005BD4B File Offset: 0x00059F4B
	private void Update()
	{
		this.UpdateScript();
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x0005BD54 File Offset: 0x00059F54
	private void UpdateScript()
	{
		if (!this.PairedListsInitiated())
		{
			this.PairCurvesKeyframes();
		}
		if (this.ChangesInEditor)
		{
			this.PairCurvesKeyframes();
			this.UpdateScriptParameters();
			this.CurvesScript.UpdateParameters();
			this.ChangesInEditor = false;
			return;
		}
		if (this.Factor != this.LastFactor || this.SaturationA != this.LastSaturationA || this.SaturationB != this.LastSaturationB)
		{
			this.UpdateScriptParameters();
			this.CurvesScript.UpdateParameters();
			this.LastFactor = this.Factor;
			this.LastSaturationA = this.SaturationA;
			this.LastSaturationB = this.SaturationB;
		}
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x0005BDF5 File Offset: 0x00059FF5
	private void EditorHasChanged()
	{
		this.ChangesInEditor = true;
		this.UpdateScript();
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x0005BE04 File Offset: 0x0005A004
	public static List<Keyframe[]> PairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
	{
		if (curveA.length == curveB.length)
		{
			return ColorCurvesManager.SimplePairKeyframes(curveA, curveB);
		}
		List<Keyframe[]> list = new List<Keyframe[]>();
		List<Keyframe> list2 = new List<Keyframe>();
		List<Keyframe> list3 = new List<Keyframe>();
		list2.AddRange(curveA.keys);
		list3.AddRange(curveB.keys);
		int i = 0;
		while (i < list2.Count)
		{
			Keyframe aKeyframe = list2[i];
			int num = list3.FindIndex((Keyframe bKeyframe) => Mathf.Abs(aKeyframe.time - bKeyframe.time) < 0.01f);
			if (num >= 0)
			{
				Keyframe[] item = new Keyframe[]
				{
					list2[i],
					list3[num]
				};
				list.Add(item);
				list2.RemoveAt(i);
				list3.RemoveAt(num);
			}
			else
			{
				i++;
			}
		}
		foreach (Keyframe keyframe in list2)
		{
			Keyframe keyframe2 = ColorCurvesManager.CreatePair(keyframe, curveB);
			list.Add(new Keyframe[]
			{
				keyframe,
				keyframe2
			});
		}
		foreach (Keyframe keyframe3 in list3)
		{
			Keyframe keyframe4 = ColorCurvesManager.CreatePair(keyframe3, curveA);
			list.Add(new Keyframe[]
			{
				keyframe4,
				keyframe3
			});
		}
		return list;
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x0005BF94 File Offset: 0x0005A194
	private static List<Keyframe[]> SimplePairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
	{
		List<Keyframe[]> list = new List<Keyframe[]>();
		if (curveA.length != curveB.length)
		{
			throw new UnityException("Simple Pair cannot work with curves with different number of Keyframes.");
		}
		for (int i = 0; i < curveA.length; i++)
		{
			list.Add(new Keyframe[]
			{
				curveA.keys[i],
				curveB.keys[i]
			});
		}
		return list;
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x0005C004 File Offset: 0x0005A204
	private static Keyframe CreatePair(Keyframe kf, AnimationCurve curve)
	{
		Keyframe result = default(Keyframe);
		result.time = kf.time;
		result.value = curve.Evaluate(kf.time);
		if (kf.time >= 0.0012f)
		{
			float num = kf.time - 0.0012f;
			result.inTangent = (curve.Evaluate(num) - curve.Evaluate(kf.time)) / (num - kf.time);
		}
		if (kf.time + 0.0012f <= 1f)
		{
			float num2 = kf.time + 0.0012f;
			result.outTangent = (curve.Evaluate(num2) - curve.Evaluate(kf.time)) / (num2 - kf.time);
		}
		return result;
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x0005C0C8 File Offset: 0x0005A2C8
	public static AnimationCurve CreateCurveFromKeyframes(IList<Keyframe[]> keyframePairs, float factor)
	{
		Keyframe[] array = new Keyframe[keyframePairs.Count];
		for (int i = 0; i < keyframePairs.Count; i++)
		{
			Keyframe[] array2 = keyframePairs[i];
			array[i] = ColorCurvesManager.AverageKeyframe(array2[0], array2[1], factor);
		}
		return new AnimationCurve(array);
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x0005C11C File Offset: 0x0005A31C
	public static Keyframe AverageKeyframe(Keyframe a, Keyframe b, float factor)
	{
		return new Keyframe
		{
			time = a.time * (1f - factor) + b.time * factor,
			value = a.value * (1f - factor) + b.value * factor,
			inTangent = a.inTangent * (1f - factor) + b.inTangent * factor,
			outTangent = a.outTangent * (1f - factor) + b.outTangent * factor
		};
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x0005C1B4 File Offset: 0x0005A3B4
	private void PairCurvesKeyframes()
	{
		this.RedPairedKeyframes = ColorCurvesManager.PairKeyframes(this.RedA, this.RedB);
		this.GreenPairedKeyframes = ColorCurvesManager.PairKeyframes(this.GreenA, this.GreenB);
		this.BluePairedKeyframes = ColorCurvesManager.PairKeyframes(this.BlueA, this.BlueB);
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x0005C208 File Offset: 0x0005A408
	private void UpdateScriptParameters()
	{
		this.Factor = Mathf.Clamp01(this.Factor);
		this.SaturationA = Mathf.Clamp(this.SaturationA, 0f, 5f);
		this.SaturationB = Mathf.Clamp(this.SaturationB, 0f, 5f);
		this.CurvesScript.saturation = Mathf.Lerp(this.SaturationA, this.SaturationB, this.Factor);
		this.CurvesScript.redChannel = ColorCurvesManager.CreateCurveFromKeyframes(this.RedPairedKeyframes, this.Factor);
		this.CurvesScript.greenChannel = ColorCurvesManager.CreateCurveFromKeyframes(this.GreenPairedKeyframes, this.Factor);
		this.CurvesScript.blueChannel = ColorCurvesManager.CreateCurveFromKeyframes(this.BluePairedKeyframes, this.Factor);
	}

	// Token: 0x06001475 RID: 5237 RVA: 0x0005C2D2 File Offset: 0x0005A4D2
	private bool PairedListsInitiated()
	{
		return this.RedPairedKeyframes != null && this.GreenPairedKeyframes != null && this.BluePairedKeyframes != null && this.DepthRedPairedKeyframes != null && this.DepthGreenPairedKeyframes != null && this.DepthBluePairedKeyframes != null;
	}

	// Token: 0x040012AA RID: 4778
	public float Factor;

	// Token: 0x040012AB RID: 4779
	public float SaturationA = 1f;

	// Token: 0x040012AC RID: 4780
	public AnimationCurve RedA = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040012AD RID: 4781
	public AnimationCurve GreenA = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040012AE RID: 4782
	public AnimationCurve BlueA = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040012AF RID: 4783
	public Color AmbientColorA = Color.white;

	// Token: 0x040012B0 RID: 4784
	public float AmbientIntensityA = 1f;

	// Token: 0x040012B1 RID: 4785
	public Color HeroLightColorA = Color.white;

	// Token: 0x040012B2 RID: 4786
	public float SaturationB = 1f;

	// Token: 0x040012B3 RID: 4787
	public AnimationCurve RedB = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040012B4 RID: 4788
	public AnimationCurve GreenB = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040012B5 RID: 4789
	public AnimationCurve BlueB = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040012B6 RID: 4790
	public Color AmbientColorB = Color.white;

	// Token: 0x040012B7 RID: 4791
	public float AmbientIntensityB = 1f;

	// Token: 0x040012B8 RID: 4792
	public Color HeroLightColorB = Color.white;

	// Token: 0x040012B9 RID: 4793
	private List<Keyframe[]> RedPairedKeyframes;

	// Token: 0x040012BA RID: 4794
	private List<Keyframe[]> GreenPairedKeyframes;

	// Token: 0x040012BB RID: 4795
	private List<Keyframe[]> BluePairedKeyframes;

	// Token: 0x040012BC RID: 4796
	private List<Keyframe[]> DepthRedPairedKeyframes;

	// Token: 0x040012BD RID: 4797
	private List<Keyframe[]> DepthGreenPairedKeyframes;

	// Token: 0x040012BE RID: 4798
	private List<Keyframe[]> DepthBluePairedKeyframes;

	// Token: 0x040012BF RID: 4799
	private List<Keyframe[]> ZCurvePairedKeyframes;

	// Token: 0x040012C0 RID: 4800
	private ColorCorrectionCurves CurvesScript;

	// Token: 0x040012C1 RID: 4801
	private const float PAIRING_DISTANCE = 0.01f;

	// Token: 0x040012C2 RID: 4802
	private const float TANGENT_DISTANCE = 0.0012f;

	// Token: 0x040012C3 RID: 4803
	private bool ChangesInEditor = true;

	// Token: 0x040012C4 RID: 4804
	private float LastFactor;

	// Token: 0x040012C5 RID: 4805
	private float LastSaturationA;

	// Token: 0x040012C6 RID: 4806
	private float LastSaturationB;
}
