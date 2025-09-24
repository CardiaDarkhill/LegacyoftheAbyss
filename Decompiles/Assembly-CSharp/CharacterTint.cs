using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000D3 RID: 211
public class CharacterTint : MonoBehaviour
{
	// Token: 0x060006B4 RID: 1716 RVA: 0x0002216C File Offset: 0x0002036C
	private void Awake()
	{
		this.renderer = base.GetComponent<Renderer>();
		if (this.renderer)
		{
			this.block = new MaterialPropertyBlock();
			this.renderer.GetPropertyBlock(this.block);
			this.block.SetColor(CharacterTint._characterTintColorProp, this.baseColor);
			this.renderer.SetPropertyBlock(this.block);
		}
		this.UpdateTint(true);
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x000221DC File Offset: 0x000203DC
	public void AddInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		CharacterTint._regions.AddIfNotPresent(region);
		this.UpdateTint(forceImmediate);
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x000221F1 File Offset: 0x000203F1
	public void RemoveInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		CharacterTint._regions.Remove(region);
		this.UpdateTint(forceImmediate);
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x00022208 File Offset: 0x00020408
	private void UpdateTint(bool forceImmediate)
	{
		if (!this.renderer)
		{
			return;
		}
		SceneAppearanceRegion sceneAppearanceRegion = CharacterTint._regions.LastOrDefault<SceneAppearanceRegion>();
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		Color color;
		float fadeDuration;
		if (sceneAppearanceRegion != null)
		{
			color = sceneAppearanceRegion.CharacterTintColor;
			fadeDuration = sceneAppearanceRegion.FadeDuration;
			this.lastFadeTime = fadeDuration;
		}
		else
		{
			color = this.baseColor;
			fadeDuration = this.lastFadeTime;
		}
		this.renderer.GetPropertyBlock(this.block);
		if (forceImmediate)
		{
			this.block.SetColor(CharacterTint._characterTintColorProp, color);
			this.renderer.SetPropertyBlock(this.block);
			return;
		}
		Color startColor = this.block.GetColor(CharacterTint._characterTintColorProp);
		this.fadeRoutine = this.StartTimerRoutine(0f, fadeDuration, delegate(float time)
		{
			Color value = Color.Lerp(startColor, color, time);
			this.renderer.GetPropertyBlock(this.block);
			this.block.SetColor(CharacterTint._characterTintColorProp, value);
			this.renderer.SetPropertyBlock(this.block);
		}, null, null, false);
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x000222FC File Offset: 0x000204FC
	public static bool CanAdd(GameObject gameObject)
	{
		if (!gameObject)
		{
			return false;
		}
		Renderer component = gameObject.GetComponent<Renderer>();
		if (!component)
		{
			return false;
		}
		Material sharedMaterial = component.sharedMaterial;
		return sharedMaterial && sharedMaterial.IsKeywordEnabled("IS_CHARACTER");
	}

	// Token: 0x04000694 RID: 1684
	private static readonly List<SceneAppearanceRegion> _regions = new List<SceneAppearanceRegion>();

	// Token: 0x04000695 RID: 1685
	private readonly Color baseColor = Color.white;

	// Token: 0x04000696 RID: 1686
	private float lastFadeTime;

	// Token: 0x04000697 RID: 1687
	private Coroutine fadeRoutine;

	// Token: 0x04000698 RID: 1688
	private Renderer renderer;

	// Token: 0x04000699 RID: 1689
	private MaterialPropertyBlock block;

	// Token: 0x0400069A RID: 1690
	private static readonly int _characterTintColorProp = Shader.PropertyToID("_CharacterTintColor");
}
