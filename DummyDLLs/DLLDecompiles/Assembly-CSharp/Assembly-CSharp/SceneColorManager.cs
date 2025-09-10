using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x02000270 RID: 624
[RequireComponent(typeof(ColorCorrectionCurves))]
public class SceneColorManager : MonoBehaviour
{
	// Token: 0x1700025B RID: 603
	// (get) Token: 0x0600163D RID: 5693 RVA: 0x00063DB8 File Offset: 0x00061FB8
	// (set) Token: 0x0600163E RID: 5694 RVA: 0x00063DC0 File Offset: 0x00061FC0
	public bool MarkerActive { get; private set; }

	// Token: 0x0600163F RID: 5695 RVA: 0x00063DC9 File Offset: 0x00061FC9
	public void SetFactor(float factor)
	{
		this.Factor = factor;
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x00063DD2 File Offset: 0x00061FD2
	public void SetSaturationA(float saturationA)
	{
		this.SaturationA = saturationA;
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x00063DDB File Offset: 0x00061FDB
	public void SetSaturationB(float saturationB)
	{
		this.SaturationB = saturationB;
	}

	// Token: 0x1700025C RID: 604
	// (get) Token: 0x06001642 RID: 5698 RVA: 0x00063DE4 File Offset: 0x00061FE4
	// (set) Token: 0x06001643 RID: 5699 RVA: 0x00063DEC File Offset: 0x00061FEC
	public bool StartBufferActive { get; private set; }

	// Token: 0x06001644 RID: 5700 RVA: 0x00063DF8 File Offset: 0x00061FF8
	public void GameInit()
	{
		this.curvesScript = base.GetComponent<ColorCorrectionCurves>();
		this.hasCurvesScript = (this.curvesScript != null);
		SceneColorManager._tempA = new List<Keyframe>(128);
		SceneColorManager._tempB = new List<Keyframe>(128);
		SceneColorManager._finalFramesList = new List<Keyframe>(128);
		this.gm = GameManager.instance;
		this.gm.UnloadingLevel += this.OnLevelUnload;
		this.UpdateSceneType();
		this.lastFactor = this.Factor;
		this.lastSaturationA = this.SaturationA;
		this.lastSaturationB = this.SaturationB;
		this.lastAmbientIntensityA = this.AmbientIntensityA;
		this.lastAmbientIntensityB = this.AmbientIntensityB;
		this.PairCurvesKeyframes();
	}

	// Token: 0x06001645 RID: 5701 RVA: 0x00063EBC File Offset: 0x000620BC
	public void SceneInit()
	{
		this.UpdateSceneType();
		if (!this.gameplayScene)
		{
			this.Factor = 0f;
			return;
		}
		this.StartBufferActive = true;
		this.MarkerActive = true;
		this.UpdateScript(true);
		HeroController hc = HeroController.instance;
		HeroController.HeroInPosition temp = null;
		temp = delegate(bool _)
		{
			this.FinishBufferPeriod();
			hc.heroInPositionDelayed -= temp;
		};
		hc.heroInPositionDelayed += temp;
	}

	// Token: 0x06001646 RID: 5702 RVA: 0x00063F3A File Offset: 0x0006213A
	private void Update()
	{
		if ((this.MarkerActive || this.StartBufferActive) && (float)Time.frameCount % 1f == 0f)
		{
			this.UpdateScript(false);
		}
	}

	// Token: 0x06001647 RID: 5703 RVA: 0x00063F66 File Offset: 0x00062166
	private void OnLevelUnload()
	{
		this.Factor = 0f;
		this.MarkerActive = false;
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x00063F7A File Offset: 0x0006217A
	private void OnDisable()
	{
		if (this.gm != null)
		{
			this.gm.UnloadingLevel -= this.OnLevelUnload;
		}
	}

	// Token: 0x06001649 RID: 5705 RVA: 0x00063FA1 File Offset: 0x000621A1
	public IEnumerator ForceRefresh()
	{
		this.UpdateScript(false);
		this.SetFactor(0.0002f);
		yield return new WaitForSeconds(0.1f);
		this.UpdateScript(false);
		yield break;
	}

	// Token: 0x0600164A RID: 5706 RVA: 0x00063FB0 File Offset: 0x000621B0
	private void FinishBufferPeriod()
	{
		this.UpdateScript(true);
		this.StartBufferActive = false;
	}

	// Token: 0x0600164B RID: 5707 RVA: 0x00063FC0 File Offset: 0x000621C0
	public void SetMarkerActive(bool active)
	{
		if (!this.StartBufferActive)
		{
			this.MarkerActive = active;
		}
	}

	// Token: 0x0600164C RID: 5708 RVA: 0x00063FD4 File Offset: 0x000621D4
	public void UpdateScript(bool forceUpdate = false)
	{
		if (!this.hasCurvesScript)
		{
			this.curvesScript = base.GetComponent<ColorCorrectionCurves>();
			this.hasCurvesScript = (this.curvesScript != null);
			if (!this.hasCurvesScript)
			{
				return;
			}
		}
		if (!this.PairedListsInitiated())
		{
			this.PairCurvesKeyframes();
		}
		if (this.changesInEditor)
		{
			this.PairCurvesKeyframes();
			this.UpdateScriptParameters();
			this.curvesScript.UpdateParameters();
			this.changesInEditor = false;
			return;
		}
		if (forceUpdate)
		{
			this.PairCurvesKeyframes();
			this.UpdateScriptParameters();
			this.curvesScript.UpdateParameters();
			return;
		}
		if (Math.Abs(this.Factor - this.lastFactor) > Mathf.Epsilon || Math.Abs(this.SaturationA - this.lastSaturationA) > Mathf.Epsilon || Math.Abs(this.SaturationB - this.lastSaturationB) > Mathf.Epsilon || Math.Abs(this.AmbientIntensityA - this.lastAmbientIntensityA) > Mathf.Epsilon || Math.Abs(this.AmbientIntensityB - this.lastAmbientIntensityB) > Mathf.Epsilon)
		{
			this.UpdateScriptParameters();
			this.curvesScript.UpdateParameters();
			this.lastFactor = this.Factor;
			this.lastSaturationA = this.SaturationA;
			this.lastSaturationB = this.SaturationB;
			this.lastAmbientIntensityA = this.AmbientIntensityA;
			this.lastAmbientIntensityB = this.AmbientIntensityB;
		}
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x0006412A File Offset: 0x0006232A
	private void EditorHasChanged()
	{
		this.changesInEditor = true;
		this.UpdateScript(false);
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x0006413C File Offset: 0x0006233C
	public static List<Keyframe[]> PairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
	{
		if (curveA.length == curveB.length)
		{
			return SceneColorManager.SimplePairKeyframes(curveA, curveB);
		}
		List<Keyframe[]> list = new List<Keyframe[]>();
		SceneColorManager._tempA.Clear();
		SceneColorManager._tempA.AddRange(curveA.keys);
		SceneColorManager._tempB.Clear();
		SceneColorManager._tempB.AddRange(curveB.keys);
		int i = 0;
		bool flag = false;
		Keyframe aKeyframe = SceneColorManager._tempA[i];
		Predicate<Keyframe> <>9__0;
		while (i < SceneColorManager._tempA.Count)
		{
			if (flag)
			{
				aKeyframe = SceneColorManager._tempA[i];
			}
			List<Keyframe> tempB = SceneColorManager._tempB;
			Predicate<Keyframe> match;
			if ((match = <>9__0) == null)
			{
				match = (<>9__0 = ((Keyframe bKeyframe) => Mathf.Abs(aKeyframe.time - bKeyframe.time) < 0.01f));
			}
			int num = tempB.FindIndex(match);
			if (num >= 0)
			{
				Keyframe[] item = new Keyframe[]
				{
					SceneColorManager._tempA[i],
					SceneColorManager._tempB[num]
				};
				list.Add(item);
				SceneColorManager._tempA.RemoveAt(i);
				SceneColorManager._tempB.RemoveAt(num);
				flag = false;
			}
			else
			{
				i++;
				flag = true;
			}
		}
		foreach (Keyframe keyframe in SceneColorManager._tempA)
		{
			Keyframe keyframe2 = SceneColorManager.CreatePair(keyframe, curveB);
			list.Add(new Keyframe[]
			{
				keyframe,
				keyframe2
			});
		}
		foreach (Keyframe keyframe3 in SceneColorManager._tempB)
		{
			Keyframe keyframe4 = SceneColorManager.CreatePair(keyframe3, curveA);
			list.Add(new Keyframe[]
			{
				keyframe4,
				keyframe3
			});
		}
		return list;
	}

	// Token: 0x0600164F RID: 5711 RVA: 0x00064334 File Offset: 0x00062534
	private static List<Keyframe[]> SimplePairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
	{
		if (curveA.length != curveB.length)
		{
			throw new UnityException("Simple Pair cannot work with curves with different number of Keyframes.");
		}
		List<Keyframe[]> list = new List<Keyframe[]>();
		Keyframe[] keys = curveA.keys;
		Keyframe[] keys2 = curveB.keys;
		for (int i = 0; i < curveA.length; i++)
		{
			list.Add(new Keyframe[]
			{
				keys[i],
				keys2[i]
			});
		}
		return list;
	}

	// Token: 0x06001650 RID: 5712 RVA: 0x000643A8 File Offset: 0x000625A8
	private static Keyframe CreatePair(Keyframe kf, AnimationCurve curve)
	{
		Keyframe result = new Keyframe
		{
			time = kf.time,
			value = curve.Evaluate(kf.time)
		};
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

	// Token: 0x06001651 RID: 5713 RVA: 0x0006446C File Offset: 0x0006266C
	private static AnimationCurve CreateCurveFromKeyframes(IList<Keyframe[]> keyframePairs, float factor)
	{
		SceneColorManager._finalFramesList.Clear();
		foreach (Keyframe[] array in keyframePairs)
		{
			SceneColorManager._finalFramesList.Add(SceneColorManager.AverageKeyframe(array[0], array[1], factor));
		}
		return new AnimationCurve(SceneColorManager._finalFramesList.ToArray());
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x000644E4 File Offset: 0x000626E4
	private static Keyframe AverageKeyframe(Keyframe a, Keyframe b, float factor)
	{
		return new Keyframe
		{
			time = a.time * (1f - factor) + b.time * factor,
			value = a.value * (1f - factor) + b.value * factor,
			inTangent = a.inTangent * (1f - factor) + b.inTangent * factor,
			outTangent = a.outTangent * (1f - factor) + b.outTangent * factor
		};
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x0006457C File Offset: 0x0006277C
	private void PairCurvesKeyframes()
	{
		this.redPairedKeyframes = SceneColorManager.PairKeyframes(this.RedA, this.RedB);
		this.greenPairedKeyframes = SceneColorManager.PairKeyframes(this.GreenA, this.GreenB);
		this.bluePairedKeyframes = SceneColorManager.PairKeyframes(this.BlueA, this.BlueB);
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x000645D0 File Offset: 0x000627D0
	private void UpdateScriptParameters()
	{
		if (!this.hasCurvesScript)
		{
			this.curvesScript = base.GetComponent<ColorCorrectionCurves>();
			this.hasCurvesScript = (this.curvesScript != null);
			if (!this.hasCurvesScript)
			{
				return;
			}
		}
		this.Factor = Mathf.Clamp01(this.Factor);
		this.SaturationA = Mathf.Clamp(this.SaturationA, 0f, 5f);
		this.SaturationB = Mathf.Clamp(this.SaturationB, 0f, 5f);
		this.curvesScript.saturation = Mathf.Lerp(this.SaturationA, this.SaturationB, this.Factor);
		this.curvesScript.redChannel = SceneColorManager.CreateCurveFromKeyframes(this.redPairedKeyframes, this.Factor);
		this.curvesScript.greenChannel = SceneColorManager.CreateCurveFromKeyframes(this.greenPairedKeyframes, this.Factor);
		this.curvesScript.blueChannel = SceneColorManager.CreateCurveFromKeyframes(this.bluePairedKeyframes, this.Factor);
		CustomSceneManager.SetLighting(Color.Lerp(this.AmbientColorA, this.AmbientColorB, this.Factor), Mathf.Lerp(this.AmbientIntensityA, this.AmbientIntensityB, this.Factor));
		if (this.gameplayScene)
		{
			if (this.hero == null)
			{
				this.hero = HeroController.instance;
			}
			this.hero.heroLight.BaseColor = Color.Lerp(this.HeroLightColorA, this.HeroLightColorB, this.Factor);
		}
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x00064744 File Offset: 0x00062944
	private bool PairedListsInitiated()
	{
		return this.redPairedKeyframes != null && this.greenPairedKeyframes != null && this.bluePairedKeyframes != null;
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x00064764 File Offset: 0x00062964
	private void UpdateSceneType()
	{
		if (this.gm == null)
		{
			this.gm = GameManager.instance;
		}
		if (this.gm.IsGameplayScene())
		{
			this.gameplayScene = true;
			if (this.hero == null)
			{
				this.hero = HeroController.instance;
				return;
			}
		}
		else
		{
			this.gameplayScene = false;
		}
	}

	// Token: 0x040014A9 RID: 5289
	public float Factor;

	// Token: 0x040014AA RID: 5290
	public float SaturationA = 1f;

	// Token: 0x040014AB RID: 5291
	public AnimationCurve RedA = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040014AC RID: 5292
	public AnimationCurve GreenA = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040014AD RID: 5293
	public AnimationCurve BlueA = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040014AE RID: 5294
	public Color AmbientColorA = Color.white;

	// Token: 0x040014AF RID: 5295
	public float AmbientIntensityA = 1f;

	// Token: 0x040014B0 RID: 5296
	public Color HeroLightColorA = Color.white;

	// Token: 0x040014B1 RID: 5297
	public float SaturationB = 1f;

	// Token: 0x040014B2 RID: 5298
	public AnimationCurve RedB = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040014B3 RID: 5299
	public AnimationCurve GreenB = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040014B4 RID: 5300
	public AnimationCurve BlueB = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040014B5 RID: 5301
	public Color AmbientColorB = Color.white;

	// Token: 0x040014B6 RID: 5302
	public float AmbientIntensityB = 1f;

	// Token: 0x040014B7 RID: 5303
	public Color HeroLightColorB = Color.white;

	// Token: 0x040014B8 RID: 5304
	private List<Keyframe[]> redPairedKeyframes;

	// Token: 0x040014B9 RID: 5305
	private List<Keyframe[]> greenPairedKeyframes;

	// Token: 0x040014BA RID: 5306
	private List<Keyframe[]> bluePairedKeyframes;

	// Token: 0x040014BB RID: 5307
	private bool hasCurvesScript;

	// Token: 0x040014BC RID: 5308
	private ColorCorrectionCurves curvesScript;

	// Token: 0x040014BD RID: 5309
	private const float PAIRING_DISTANCE = 0.01f;

	// Token: 0x040014BE RID: 5310
	private const float TANGENT_DISTANCE = 0.0012f;

	// Token: 0x040014BF RID: 5311
	private const float UPDATE_RATE = 1f;

	// Token: 0x040014C0 RID: 5312
	private bool gameplayScene;

	// Token: 0x040014C1 RID: 5313
	private HeroController hero;

	// Token: 0x040014C2 RID: 5314
	private GameManager gm;

	// Token: 0x040014C3 RID: 5315
	private static List<Keyframe> _tempA;

	// Token: 0x040014C4 RID: 5316
	private static List<Keyframe> _tempB;

	// Token: 0x040014C5 RID: 5317
	private static List<Keyframe> _finalFramesList;

	// Token: 0x040014C6 RID: 5318
	private bool changesInEditor = true;

	// Token: 0x040014C7 RID: 5319
	private float lastFactor;

	// Token: 0x040014C8 RID: 5320
	private float lastSaturationA;

	// Token: 0x040014C9 RID: 5321
	private float lastSaturationB;

	// Token: 0x040014CA RID: 5322
	private float lastAmbientIntensityA;

	// Token: 0x040014CB RID: 5323
	private float lastAmbientIntensityB;
}
