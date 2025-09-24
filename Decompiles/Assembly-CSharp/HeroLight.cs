using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020003B8 RID: 952
public class HeroLight : MonoBehaviour
{
	// Token: 0x17000353 RID: 851
	// (get) Token: 0x06001FF6 RID: 8182 RVA: 0x0009173D File Offset: 0x0008F93D
	// (set) Token: 0x06001FF7 RID: 8183 RVA: 0x00091745 File Offset: 0x0008F945
	public Color BaseColor
	{
		get
		{
			return this.baseColor;
		}
		set
		{
			this.baseColor = value;
			this.SetColor();
		}
	}

	// Token: 0x17000354 RID: 852
	// (get) Token: 0x06001FF8 RID: 8184 RVA: 0x00091754 File Offset: 0x0008F954
	// (set) Token: 0x06001FF9 RID: 8185 RVA: 0x00091790 File Offset: 0x0008F990
	public Color MaterialColor
	{
		get
		{
			if (!this.spriteRenderer || !this.spriteRenderer.material)
			{
				return Color.white;
			}
			return this.spriteRenderer.material.GetColor(HeroLight._color);
		}
		set
		{
			if (this.spriteRenderer && this.spriteRenderer.material)
			{
				this.spriteRenderer.material.SetColor(HeroLight._color, value);
			}
		}
	}

	// Token: 0x17000355 RID: 853
	// (get) Token: 0x06001FFA RID: 8186 RVA: 0x000917C7 File Offset: 0x0008F9C7
	// (set) Token: 0x06001FFB RID: 8187 RVA: 0x000917CF File Offset: 0x0008F9CF
	public float Alpha
	{
		get
		{
			return this.alpha;
		}
		set
		{
			this.alpha = Mathf.Max(value, 0.001f);
			this.SetColor();
		}
	}

	// Token: 0x06001FFC RID: 8188 RVA: 0x000917E8 File Offset: 0x0008F9E8
	private void Awake()
	{
		this.Alpha = 1f;
		if (this.spriteRenderer)
		{
			this.baseColor = this.spriteRenderer.color;
			if (float.IsInfinity(this.baseColor.a))
			{
				this.baseColor.a = 1f;
			}
			else if (float.IsNaN(this.baseColor.a) || this.baseColor.a < 0.001f)
			{
				this.baseColor.a = 0.001f;
			}
			this.ApplyColor(this.baseColor);
		}
	}

	// Token: 0x06001FFD RID: 8189 RVA: 0x00091882 File Offset: 0x0008FA82
	public void AddInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		this.insideRegions.AddIfNotPresent(region);
		this.UpdateColor(forceImmediate);
	}

	// Token: 0x06001FFE RID: 8190 RVA: 0x00091898 File Offset: 0x0008FA98
	public void RemoveInside(SceneAppearanceRegion region)
	{
		this.insideRegions.Remove(region);
		this.UpdateColor(false);
	}

	// Token: 0x06001FFF RID: 8191 RVA: 0x000918B0 File Offset: 0x0008FAB0
	public void UpdateColor(bool forceImmediate)
	{
		if (!this.spriteRenderer)
		{
			return;
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		float targetT;
		Color targetLerpColor;
		Action action;
		if (this.insideRegions.Count == 0)
		{
			if (forceImmediate)
			{
				this.colorLerpT = 0f;
				this.lerpColor = this.baseColor;
				this.SetColor();
				this.fadeDuration = 0f;
				return;
			}
			targetT = 0f;
			targetLerpColor = this.lerpColor;
			action = delegate()
			{
				this.lerpColor = this.baseColor;
			};
		}
		else
		{
			SceneAppearanceRegion sceneAppearanceRegion = this.insideRegions.Last<SceneAppearanceRegion>();
			targetLerpColor = this.GetTargetColor(sceneAppearanceRegion);
			targetT = 1f;
			this.fadeDuration = sceneAppearanceRegion.FadeDuration;
			action = null;
		}
		float startT = this.colorLerpT;
		Color startColor = this.lerpColor;
		if (this.fadeDuration > 0f && !forceImmediate && base.isActiveAndEnabled)
		{
			this.fadeRoutine = this.StartTimerRoutine(0f, this.fadeDuration, delegate(float time)
			{
				this.colorLerpT = Mathf.Lerp(startT, targetT, time);
				this.lerpColor = Color.Lerp(startColor, targetLerpColor, time);
				this.SetColor();
			}, null, action, false);
			return;
		}
		this.colorLerpT = 1f;
		this.lerpColor = targetLerpColor;
		if (action != null)
		{
			action();
		}
		this.SetColor();
	}

	// Token: 0x06002000 RID: 8192 RVA: 0x000919FC File Offset: 0x0008FBFC
	private void SetColor()
	{
		Color color = Color.Lerp(this.baseColor, this.lerpColor, this.colorLerpT);
		color.a *= this.Alpha;
		this.ApplyColor(color);
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x00091A3C File Offset: 0x0008FC3C
	public void Detach()
	{
		if (this.isDetached)
		{
			return;
		}
		this.isDetached = true;
		if (this.lerpRoutine != null)
		{
			base.StopCoroutine(this.lerpRoutine);
			this.lerpRoutine = null;
			this.transform.SetLocalPosition2D(this.initialLocalPosition);
			this.vignette.SetLocalPosition2D(this.initialLocalPosition);
		}
		this.initialLocalPosition = this.transform.localPosition;
		this.initialParent = this.transform.parent;
		HeroLight.DeParent(this.transform);
		HeroLight.DeParent(this.vignette);
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x00091AD4 File Offset: 0x0008FCD4
	private static void DeParent(Transform transform)
	{
		transform.SetParent(null, true);
		Vector3 localScale = transform.localScale;
		localScale.x = Mathf.Abs(localScale.x);
		transform.localScale = localScale;
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x00091B0C File Offset: 0x0008FD0C
	private static void ReParent(Transform transform, Transform parent)
	{
		transform.SetParent(parent);
		Vector3 localScale = transform.localScale;
		localScale.x = Mathf.Abs(localScale.x);
		transform.localScale = localScale;
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x00091B40 File Offset: 0x0008FD40
	public void Reattach()
	{
		if (!this.isDetached)
		{
			return;
		}
		this.isDetached = false;
		HeroLight.ReParent(this.transform, this.initialParent);
		HeroLight.ReParent(this.vignette, this.initialParent);
		this.lerpStartPos = this.transform.localPosition;
		this.lerpRoutine = this.StartTimerRoutine(0f, this.lerpTime, delegate(float time)
		{
			Vector2 position = Vector2.Lerp(this.lerpStartPos, this.initialLocalPosition, time);
			this.transform.SetLocalPosition2D(position);
			this.vignette.SetLocalPosition2D(position);
		}, null, delegate
		{
			this.lerpRoutine = null;
		}, false);
	}

	// Token: 0x06002005 RID: 8197 RVA: 0x00091BC6 File Offset: 0x0008FDC6
	private Color GetTargetColor(SceneAppearanceRegion region)
	{
		return region.HeroLightColor;
	}

	// Token: 0x06002006 RID: 8198 RVA: 0x00091BD0 File Offset: 0x0008FDD0
	private void ApplyColor(Color color)
	{
		this.spriteRenderer.color = color;
		if (this.heroLightDonut)
		{
			Color color2 = this.heroLightDonut.color;
			color2.a = this.heroLightDonutAlphaCurve.Evaluate(color.a);
			this.heroLightDonut.color = color2;
		}
	}

	// Token: 0x04001EFA RID: 7930
	[SerializeField]
	protected SpriteRenderer spriteRenderer;

	// Token: 0x04001EFB RID: 7931
	[SerializeField]
	private new Transform transform;

	// Token: 0x04001EFC RID: 7932
	[SerializeField]
	private Transform vignette;

	// Token: 0x04001EFD RID: 7933
	[SerializeField]
	private float lerpTime = 0.2f;

	// Token: 0x04001EFE RID: 7934
	[SerializeField]
	private SpriteRenderer heroLightDonut;

	// Token: 0x04001EFF RID: 7935
	[SerializeField]
	private AnimationCurve heroLightDonutAlphaCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04001F00 RID: 7936
	private bool isDetached;

	// Token: 0x04001F01 RID: 7937
	private Vector2 initialLocalPosition;

	// Token: 0x04001F02 RID: 7938
	private Vector2 lerpStartPos;

	// Token: 0x04001F03 RID: 7939
	private Transform initialParent;

	// Token: 0x04001F04 RID: 7940
	private Coroutine lerpRoutine;

	// Token: 0x04001F05 RID: 7941
	private Color baseColor;

	// Token: 0x04001F06 RID: 7942
	private Color lerpColor;

	// Token: 0x04001F07 RID: 7943
	private float colorLerpT;

	// Token: 0x04001F08 RID: 7944
	private Coroutine fadeRoutine;

	// Token: 0x04001F09 RID: 7945
	private float fadeDuration;

	// Token: 0x04001F0A RID: 7946
	private readonly List<SceneAppearanceRegion> insideRegions = new List<SceneAppearanceRegion>();

	// Token: 0x04001F0B RID: 7947
	private static readonly int _color = Shader.PropertyToID("_Color");

	// Token: 0x04001F0C RID: 7948
	private float alpha;
}
