using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000225 RID: 549
public class CharacterLightDust : MonoBehaviour
{
	// Token: 0x06001457 RID: 5207 RVA: 0x0005B6A4 File Offset: 0x000598A4
	private void Awake()
	{
		if (this.fgSprite && this.bgSprite)
		{
			this.spriteRenderers = new SpriteRenderer[]
			{
				this.fgSprite,
				this.bgSprite
			};
		}
		else if (this.fgSprite)
		{
			this.spriteRenderers = new SpriteRenderer[]
			{
				this.fgSprite
			};
		}
		else if (this.bgSprite)
		{
			this.spriteRenderers = new SpriteRenderer[]
			{
				this.bgSprite
			};
		}
		else
		{
			this.spriteRenderers = Array.Empty<SpriteRenderer>();
		}
		if (this.fgSprite)
		{
			this.fgMaterial = this.fgSprite.sharedMaterial;
		}
		if (this.bgSprite)
		{
			this.bgMaterial = this.bgSprite.sharedMaterial;
		}
		this.baseColors = new Color[this.spriteRenderers.Length];
		for (int i = 0; i < this.spriteRenderers.Length; i++)
		{
			SpriteRenderer spriteRenderer = this.spriteRenderers[i];
			this.baseColors[i] = spriteRenderer.color;
		}
		this.UpdateColor(true);
		this.gm = GameManager.instance;
		this.gm.NextSceneWillActivate += this.RevertMaterials;
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x0005B7E4 File Offset: 0x000599E4
	private void OnDestroy()
	{
		if (!this.gm)
		{
			return;
		}
		this.gm.NextSceneWillActivate -= this.RevertMaterials;
		this.gm = null;
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x0005B812 File Offset: 0x00059A12
	public void AddInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		this.insideRegions.AddIfNotPresent(region);
		this.UpdateColor(forceImmediate);
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x0005B828 File Offset: 0x00059A28
	public void RemoveInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		this.insideRegions.Remove(region);
		this.UpdateColor(forceImmediate);
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x0005B840 File Offset: 0x00059A40
	private void UpdateColor(bool forceImmediate)
	{
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		Color targetColor;
		bool flag;
		if (this.insideRegions.Count == 0)
		{
			if (forceImmediate)
			{
				this.RevertMaterials();
				this.SetColor(this.defaultColor);
				this.fadeDuration = 0f;
				return;
			}
			targetColor = this.defaultColor;
			flag = true;
		}
		else
		{
			SceneAppearanceRegion sceneAppearanceRegion = this.insideRegions.Last<SceneAppearanceRegion>();
			targetColor = sceneAppearanceRegion.CharacterLightDustColor;
			this.fadeDuration = sceneAppearanceRegion.FadeDuration;
			SceneAppearanceRegion.DustMaterials characterLightDustMaterials = sceneAppearanceRegion.CharacterLightDustMaterials;
			if (this.fgSprite)
			{
				this.fgSprite.sharedMaterial = characterLightDustMaterials.Foreground;
			}
			if (this.bgSprite)
			{
				this.bgSprite.sharedMaterial = characterLightDustMaterials.Background;
			}
			flag = false;
		}
		Color startColor = this.currentColor;
		if (this.fadeDuration > 0f && base.isActiveAndEnabled)
		{
			this.fadeRoutine = this.StartTimerRoutine(0f, this.fadeDuration, delegate(float time)
			{
				this.SetColor(Color.Lerp(startColor, targetColor, time));
			}, null, flag ? new Action(this.RevertMaterials) : null, false);
			return;
		}
		this.SetColor(targetColor);
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x0005B97A File Offset: 0x00059B7A
	private void RevertMaterials()
	{
		if (this.fgSprite)
		{
			this.fgSprite.sharedMaterial = this.fgMaterial;
		}
		if (this.bgSprite)
		{
			this.bgSprite.sharedMaterial = this.bgMaterial;
		}
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x0005B9B8 File Offset: 0x00059BB8
	private void SetColor(Color color)
	{
		this.currentColor = color;
		for (int i = 0; i < this.spriteRenderers.Length; i++)
		{
			SpriteRenderer spriteRenderer = this.spriteRenderers[i];
			if (spriteRenderer)
			{
				Color color2 = this.baseColors[i].MultiplyElements(color);
				spriteRenderer.color = color2;
				spriteRenderer.enabled = (color2.a > 0.001f);
			}
		}
	}

	// Token: 0x04001293 RID: 4755
	[SerializeField]
	private SpriteRenderer fgSprite;

	// Token: 0x04001294 RID: 4756
	[SerializeField]
	private SpriteRenderer bgSprite;

	// Token: 0x04001295 RID: 4757
	[SerializeField]
	private Color defaultColor = new Color(1f, 1f, 1f, 0f);

	// Token: 0x04001296 RID: 4758
	private SpriteRenderer[] spriteRenderers;

	// Token: 0x04001297 RID: 4759
	private Color[] baseColors;

	// Token: 0x04001298 RID: 4760
	private Coroutine fadeRoutine;

	// Token: 0x04001299 RID: 4761
	private float fadeDuration;

	// Token: 0x0400129A RID: 4762
	private Color currentColor;

	// Token: 0x0400129B RID: 4763
	private readonly List<SceneAppearanceRegion> insideRegions = new List<SceneAppearanceRegion>();

	// Token: 0x0400129C RID: 4764
	private GameManager gm;

	// Token: 0x0400129D RID: 4765
	private Material fgMaterial;

	// Token: 0x0400129E RID: 4766
	private Material bgMaterial;
}
