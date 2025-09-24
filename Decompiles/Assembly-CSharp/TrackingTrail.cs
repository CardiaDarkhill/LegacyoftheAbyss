using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.Splines;
using UnityEngine;

// Token: 0x0200057A RID: 1402
[ExecuteInEditMode]
public class TrackingTrail : MonoBehaviour
{
	// Token: 0x0600322D RID: 12845 RVA: 0x000DF939 File Offset: 0x000DDB39
	private void OnEnable()
	{
		TrackingTrail._activeTrails.AddIfNotPresent(this);
	}

	// Token: 0x0600322E RID: 12846 RVA: 0x000DF948 File Offset: 0x000DDB48
	private void Start()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.spline.VertexColor = Color.clear;
		this.spline.UpdateMesh(true);
		if (this.silhouettesParent)
		{
			int pointCount = this.spline.GetPointCount();
			Transform transform = this.spline.transform;
			SpriteRenderer[] componentsInChildren = this.silhouettesParent.GetComponentsInChildren<SpriteRenderer>();
			this.silhouetteDict = new Dictionary<int, SpriteRenderer>(componentsInChildren.Length);
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				Vector2 a = spriteRenderer.transform.position;
				float num = float.MaxValue;
				int num2 = -1;
				for (int j = 0; j < pointCount; j++)
				{
					SplineBase.Point point = this.spline.GetPoint(j);
					Vector3 v = transform.TransformPoint(point.Position);
					float num3 = Vector2.Distance(a, v);
					if (num3 <= num)
					{
						num = num3;
						num2 = j;
					}
				}
				if (num2 >= 0)
				{
					this.silhouetteDict[num2] = spriteRenderer;
				}
				spriteRenderer.color = Color.clear;
			}
		}
		this.UpdateShaderPositions();
		GameObject gameObject = base.gameObject;
		string name = gameObject.name;
		string name2 = gameObject.scene.name;
		foreach (TrackingTrail.CrossSceneFade crossSceneFade in TrackingTrail._crossSceneFades)
		{
			if (!(crossSceneFade.SceneName != name2) && !(crossSceneFade.Id != name))
			{
				this.previousCrossSceneFade = crossSceneFade;
				break;
			}
		}
		foreach (TrackingTrail.CrossSceneFade crossSceneFade2 in TrackingTrail._crossSceneFades)
		{
			if (!(crossSceneFade2.SceneName != this.continueInScene.SceneName) && !(crossSceneFade2.Id != this.continueInScene.Id))
			{
				this.nextCrossSceneFade = crossSceneFade2;
				break;
			}
		}
		if (this.previousCrossSceneFade != null || this.nextCrossSceneFade != null)
		{
			this.StartTrailInternal(false);
		}
	}

	// Token: 0x0600322F RID: 12847 RVA: 0x000DFB84 File Offset: 0x000DDD84
	private void OnDisable()
	{
		TrackingTrail._activeTrails.Remove(this);
	}

	// Token: 0x06003230 RID: 12848 RVA: 0x000DFB94 File Offset: 0x000DDD94
	private void UpdateShaderPositions()
	{
		if (!this.silhouettesParent)
		{
			return;
		}
		Renderer component = this.spline.GetComponent<Renderer>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		component.GetPropertyBlock(materialPropertyBlock);
		int num = this.silhouettesParent.childCount;
		if (num > 10)
		{
			num = 10;
		}
		materialPropertyBlock.SetFloat(TrackingTrail._cutoutsCountProp, (float)num);
		Vector4[] array = new Vector4[num];
		for (int i = 0; i < num; i++)
		{
			Transform child = this.silhouettesParent.GetChild(i);
			if (child.gameObject.activeInHierarchy)
			{
				Vector3 position = child.position;
				array[i] = position;
			}
		}
		materialPropertyBlock.SetVectorArray(TrackingTrail._cutoutsProp, array);
		component.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x06003231 RID: 12849 RVA: 0x000DFC48 File Offset: 0x000DDE48
	public static void ClearStatic()
	{
		TrackingTrail._crossSceneFades.Clear();
	}

	// Token: 0x06003232 RID: 12850 RVA: 0x000DFC54 File Offset: 0x000DDE54
	public void StartTrail(GameObject starter)
	{
		this.queuedEnd = false;
		TrackingTrail._crossSceneFades.Clear();
		TrackingTrail.FadeDownAll();
		List<TrackingTrail.FadeTracker> list = this.runningFades;
		this.refading = (list != null && list.Count > 0);
		this.StartTrailInternal(true);
	}

	// Token: 0x06003233 RID: 12851 RVA: 0x000DFC9C File Offset: 0x000DDE9C
	public static void FadeDownAll()
	{
		foreach (TrackingTrail trackingTrail in TrackingTrail._activeTrails)
		{
			trackingTrail.refading = false;
			List<TrackingTrail.FadeTracker> list = trackingTrail.runningFades;
			if (list != null && list.Count > 0)
			{
				trackingTrail.queuedEnd = true;
			}
		}
	}

	// Token: 0x06003234 RID: 12852 RVA: 0x000DFD08 File Offset: 0x000DDF08
	private void StartTrailInternal(bool endCrossSceneFade)
	{
		base.StartCoroutine(this.RunFades(endCrossSceneFade));
	}

	// Token: 0x06003235 RID: 12853 RVA: 0x000DFD18 File Offset: 0x000DDF18
	private IEnumerator RunFades(bool endCrossSceneFade)
	{
		if (this.runningFades != null)
		{
			while (this.runningFades.Count > 0)
			{
				yield return null;
			}
		}
		this.refading = false;
		if (endCrossSceneFade)
		{
			this.EndCrossSceneFade();
		}
		int pointCount = this.spline.GetPointCount();
		if (this.runningFades == null)
		{
			this.runningFades = new List<TrackingTrail.FadeTracker>(pointCount);
		}
		else
		{
			this.runningFades.Clear();
		}
		TrackingTrail.CrossSceneFade crossSceneFade = this.previousCrossSceneFade;
		int num = (crossSceneFade != null) ? crossSceneFade.LastIndex : 0;
		TrackingTrail.CrossSceneFade crossSceneFade2 = this.previousCrossSceneFade;
		int num2 = (crossSceneFade2 != null) ? (crossSceneFade2.SceneIndex + 1) : 0;
		float num3 = this.crossSceneAddDelay * (float)num2;
		for (int i = 0; i < pointCount; i++)
		{
			int num4 = i + num;
			this.runningFades.Add(new TrackingTrail.FadeTracker
			{
				State = TrackingTrail.FadeStates.Off,
				TimeLeftInState = this.fadeUpTravelDelay * (float)num4 + num3
			});
		}
		if (this.nextCrossSceneFade == null && !string.IsNullOrEmpty(this.continueInScene.SceneName) && !string.IsNullOrEmpty(this.continueInScene.SceneName))
		{
			this.nextCrossSceneFade = new TrackingTrail.CrossSceneFade
			{
				SceneName = this.continueInScene.SceneName,
				Id = this.continueInScene.Id,
				LastIndex = pointCount - 1,
				SceneIndex = num2
			};
			TrackingTrail._crossSceneFades.Add(this.nextCrossSceneFade);
		}
		float num5;
		if (this.previousCrossSceneFade != null)
		{
			num5 = this.previousCrossSceneFade.TimeElapsed;
		}
		else if (this.nextCrossSceneFade != null)
		{
			num5 = this.nextCrossSceneFade.TimeElapsed;
		}
		else
		{
			num5 = 0f;
		}
		if (num5 > Mathf.Epsilon)
		{
			for (float num6 = 0f; num6 <= num5; num6 += 0.016666668f)
			{
				for (int j = 0; j < pointCount; j++)
				{
					Color color;
					this.TickTracker(0.016666668f, j, out color);
				}
			}
			if (this.nextCrossSceneFade != null)
			{
				this.nextCrossSceneFade.TimeElapsed = num5;
			}
			if (this.previousCrossSceneFade != null)
			{
				this.previousCrossSceneFade.TimeElapsed = num5;
			}
		}
		this.spline.VertexColor = Color.white;
		for (;;)
		{
			if (this.queuedEnd)
			{
				this.queuedEnd = false;
				foreach (TrackingTrail.FadeTracker fadeTracker in this.runningFades)
				{
					if (fadeTracker.State < TrackingTrail.FadeStates.FadeDown)
					{
						fadeTracker.State = TrackingTrail.FadeStates.On;
						fadeTracker.TimeLeftInState = 0f;
					}
				}
			}
			bool flag = false;
			for (int k = 0; k < pointCount; k++)
			{
				Color color2;
				if (this.TickTracker(Time.deltaTime, k, out color2))
				{
					flag = true;
					this.spline.SetPointColor(k, color2);
				}
			}
			if (!flag)
			{
				break;
			}
			this.spline.UpdateMesh(true);
			foreach (TrackingTrail.CrossSceneFade crossSceneFade3 in TrackingTrail._crossSceneFades)
			{
				crossSceneFade3.TimeElapsed += Time.deltaTime;
			}
			yield return null;
		}
		this.runningFades.Clear();
		this.EndCrossSceneFade();
		yield break;
	}

	// Token: 0x06003236 RID: 12854 RVA: 0x000DFD30 File Offset: 0x000DDF30
	private void EndCrossSceneFade()
	{
		if (this.nextCrossSceneFade != null)
		{
			TrackingTrail._crossSceneFades.Remove(this.nextCrossSceneFade);
			this.nextCrossSceneFade = null;
		}
		if (this.previousCrossSceneFade != null)
		{
			TrackingTrail._crossSceneFades.Remove(this.previousCrossSceneFade);
			this.previousCrossSceneFade = null;
		}
	}

	// Token: 0x06003237 RID: 12855 RVA: 0x000DFD80 File Offset: 0x000DDF80
	private bool TickTracker(float deltaTime, int trackerIndex, out Color pointColor)
	{
		TrackingTrail.FadeTracker fadeTracker = this.runningFades[trackerIndex];
		SpriteRenderer spriteRenderer;
		this.silhouetteDict.TryGetValue(trackerIndex, out spriteRenderer);
		Color color = new Color(1f, 1f, 1f, 0f);
		Color color2 = new Color(1f, 1f, 1f, 1f);
		switch (fadeTracker.State)
		{
		case TrackingTrail.FadeStates.Off:
			pointColor = color;
			fadeTracker.TimeLeftInState -= deltaTime;
			if (fadeTracker.TimeLeftInState > 0f)
			{
				return true;
			}
			fadeTracker.State = TrackingTrail.FadeStates.FadeUp;
			fadeTracker.TimeLeftInState = this.fadeUpDuration;
			return true;
		case TrackingTrail.FadeStates.FadeUp:
		{
			float num = this.refading ? this.reFadeDuration : this.fadeUpDuration;
			fadeTracker.TimeLeftInState -= deltaTime;
			float t = fadeTracker.TimeLeftInState / num;
			pointColor = Color.Lerp(color2, color, t);
			if (spriteRenderer != null)
			{
				spriteRenderer.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), t);
			}
			if (fadeTracker.TimeLeftInState > 0f)
			{
				return true;
			}
			fadeTracker.State = TrackingTrail.FadeStates.On;
			fadeTracker.TimeLeftInState = this.stayUpTime;
			return true;
		}
		case TrackingTrail.FadeStates.On:
			fadeTracker.TimeLeftInState -= deltaTime;
			pointColor = color2;
			if (spriteRenderer != null)
			{
				spriteRenderer.color = Color.white;
			}
			if (!this.refading && fadeTracker.TimeLeftInState > 0f)
			{
				return true;
			}
			fadeTracker.State = TrackingTrail.FadeStates.FadeDown;
			fadeTracker.TimeLeftInState = (this.refading ? this.reFadeDuration : this.fadeDownDuration);
			return true;
		case TrackingTrail.FadeStates.FadeDown:
		{
			float num2 = this.refading ? this.reFadeDuration : this.fadeDownDuration;
			fadeTracker.TimeLeftInState -= deltaTime;
			float t2 = fadeTracker.TimeLeftInState / num2;
			pointColor = Color.Lerp(color, color2, t2);
			if (spriteRenderer != null)
			{
				spriteRenderer.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, t2);
			}
			if (fadeTracker.TimeLeftInState > 0f)
			{
				return true;
			}
			fadeTracker.State = TrackingTrail.FadeStates.Ended;
			return true;
		}
		case TrackingTrail.FadeStates.Ended:
			pointColor = color;
			this.refading = false;
			return false;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x040035CC RID: 13772
	[SerializeField]
	private HermiteSplineFullPath spline;

	// Token: 0x040035CD RID: 13773
	[SerializeField]
	private float fadeUpTravelDelay;

	// Token: 0x040035CE RID: 13774
	[SerializeField]
	private float crossSceneAddDelay;

	// Token: 0x040035CF RID: 13775
	[SerializeField]
	private float fadeUpDuration;

	// Token: 0x040035D0 RID: 13776
	[SerializeField]
	private float stayUpTime;

	// Token: 0x040035D1 RID: 13777
	[SerializeField]
	private float fadeDownDuration;

	// Token: 0x040035D2 RID: 13778
	[SerializeField]
	private float reFadeDuration;

	// Token: 0x040035D3 RID: 13779
	[Space]
	[SerializeField]
	private Transform silhouettesParent;

	// Token: 0x040035D4 RID: 13780
	[Space]
	[SerializeField]
	private TrackingTrail.CrossSceneFadeSetup continueInScene;

	// Token: 0x040035D5 RID: 13781
	private Dictionary<int, SpriteRenderer> silhouetteDict;

	// Token: 0x040035D6 RID: 13782
	private List<TrackingTrail.FadeTracker> runningFades;

	// Token: 0x040035D7 RID: 13783
	private bool queuedEnd;

	// Token: 0x040035D8 RID: 13784
	private bool refading;

	// Token: 0x040035D9 RID: 13785
	private TrackingTrail.CrossSceneFade previousCrossSceneFade;

	// Token: 0x040035DA RID: 13786
	private TrackingTrail.CrossSceneFade nextCrossSceneFade;

	// Token: 0x040035DB RID: 13787
	private static readonly int _cutoutsCountProp = Shader.PropertyToID("_CutoutsCount");

	// Token: 0x040035DC RID: 13788
	private static readonly int _cutoutsProp = Shader.PropertyToID("_Cutouts");

	// Token: 0x040035DD RID: 13789
	private static readonly List<TrackingTrail.CrossSceneFade> _crossSceneFades = new List<TrackingTrail.CrossSceneFade>();

	// Token: 0x040035DE RID: 13790
	private static readonly List<TrackingTrail> _activeTrails = new List<TrackingTrail>();

	// Token: 0x040035DF RID: 13791
	private static readonly int TINT_COLOR = Shader.PropertyToID("_TintColor");

	// Token: 0x040035E0 RID: 13792
	private static readonly int COLOR = Shader.PropertyToID("_Color");

	// Token: 0x040035E1 RID: 13793
	private static readonly int SECONDARY_COLOR = Shader.PropertyToID("_SecondaryColor");

	// Token: 0x040035E2 RID: 13794
	private const int SHADER_ARRAY_SIZE = 10;

	// Token: 0x02001883 RID: 6275
	private enum FadeStates
	{
		// Token: 0x04009253 RID: 37459
		Off,
		// Token: 0x04009254 RID: 37460
		FadeUp,
		// Token: 0x04009255 RID: 37461
		On,
		// Token: 0x04009256 RID: 37462
		FadeDown,
		// Token: 0x04009257 RID: 37463
		Ended
	}

	// Token: 0x02001884 RID: 6276
	private class FadeTracker
	{
		// Token: 0x04009258 RID: 37464
		public TrackingTrail.FadeStates State;

		// Token: 0x04009259 RID: 37465
		public float TimeLeftInState;
	}

	// Token: 0x02001885 RID: 6277
	[Serializable]
	private struct CrossSceneFadeSetup
	{
		// Token: 0x0400925A RID: 37466
		public string SceneName;

		// Token: 0x0400925B RID: 37467
		public string Id;
	}

	// Token: 0x02001886 RID: 6278
	private class CrossSceneFade
	{
		// Token: 0x0400925C RID: 37468
		public string SceneName;

		// Token: 0x0400925D RID: 37469
		public string Id;

		// Token: 0x0400925E RID: 37470
		public int LastIndex;

		// Token: 0x0400925F RID: 37471
		public float TimeElapsed;

		// Token: 0x04009260 RID: 37472
		public int SceneIndex;
	}
}
