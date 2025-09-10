using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000515 RID: 1301
public class LifebloodPustuleSubWither : MonoBehaviour
{
	// Token: 0x06002E77 RID: 11895 RVA: 0x000CC4AA File Offset: 0x000CA6AA
	private void Reset()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06002E78 RID: 11896 RVA: 0x000CC4B8 File Offset: 0x000CA6B8
	private void Awake()
	{
		if (this.witherStartEffects)
		{
			this.witherStartEffects.SetActive(false);
			this.particles = this.witherStartEffects.GetComponentsInChildren<ParticleSystem>(true);
		}
	}

	// Token: 0x06002E79 RID: 11897 RVA: 0x000CC4E8 File Offset: 0x000CA6E8
	public void BeginWither(Transform fromTrans)
	{
		if (this.isWithered)
		{
			return;
		}
		this.isWithered = true;
		Vector2 a = base.transform.position;
		Vector2 b = fromTrans.position;
		float num = Vector2.Distance(a, b);
		base.StartCoroutine(this.Wither(num * 0.01f));
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x000CC53C File Offset: 0x000CA73C
	public void StartWithered()
	{
		if (this.isWithered)
		{
			return;
		}
		this.isWithered = true;
		if (this.witherVariant)
		{
			this.spriteRenderer.sprite = this.witherVariant;
		}
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		this.spriteRenderer.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat(LifebloodPustuleSubWither._desaturateProp, 0f);
		this.spriteRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x000CC5A5 File Offset: 0x000CA7A5
	private IEnumerator Wither(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (this.witherVariant)
		{
			this.spriteRenderer.sprite = this.witherVariant;
		}
		Color color = this.spriteRenderer.color;
		if (this.witherStartEffects)
		{
			ParticleSystem[] array = this.particles;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.MainModule main = array[i].main;
				ParticleSystem.MinMaxGradient startColor = main.startColor;
				ParticleSystemGradientMode mode = startColor.mode;
				if (mode != ParticleSystemGradientMode.Color)
				{
					if (mode == ParticleSystemGradientMode.TwoColors)
					{
						startColor.colorMin *= color;
						startColor.colorMax *= color;
					}
				}
				else
				{
					startColor.color *= color;
				}
				main.startColor = startColor;
			}
			this.witherStartEffects.SetActive(true);
		}
		MaterialPropertyBlock block = new MaterialPropertyBlock();
		this.spriteRenderer.GetPropertyBlock(block);
		for (float elapsed = 0f; elapsed < 1.5f; elapsed += Time.deltaTime)
		{
			block.SetFloat(LifebloodPustuleSubWither._desaturateProp, Mathf.Lerp(1f, 0f, elapsed / 1.5f));
			this.spriteRenderer.SetPropertyBlock(block);
			yield return null;
		}
		block.SetFloat(LifebloodPustuleSubWither._desaturateProp, 0f);
		this.spriteRenderer.SetPropertyBlock(block);
		yield break;
	}

	// Token: 0x040030F2 RID: 12530
	private const float DESATURATE_DURATION = 1.5f;

	// Token: 0x040030F3 RID: 12531
	private const float DISTANCE_DELAY = 0.01f;

	// Token: 0x040030F4 RID: 12532
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x040030F5 RID: 12533
	[SerializeField]
	private Sprite witherVariant;

	// Token: 0x040030F6 RID: 12534
	[SerializeField]
	private GameObject witherStartEffects;

	// Token: 0x040030F7 RID: 12535
	private bool isWithered;

	// Token: 0x040030F8 RID: 12536
	private ParticleSystem[] particles;

	// Token: 0x040030F9 RID: 12537
	private static readonly int _desaturateProp = Shader.PropertyToID("_SaturationLerp");
}
