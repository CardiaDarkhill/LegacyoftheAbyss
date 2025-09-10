using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000B9 RID: 185
public class SpriteFadeMaterial : MonoBehaviour
{
	// Token: 0x06000596 RID: 1430 RVA: 0x0001D34B File Offset: 0x0001B54B
	private void Awake()
	{
		this.sprites = base.GetComponentsInChildren<SpriteRenderer>();
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x0001D35C File Offset: 0x0001B55C
	private void Start()
	{
		SpriteRenderer[] array = this.sprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sharedMaterial = this.initialMaterial;
		}
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x0001D38C File Offset: 0x0001B58C
	public void FadeBack()
	{
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		if (this.onFadeEnd != null)
		{
			this.onFadeEnd();
			this.onFadeEnd = null;
		}
		this.fadeRoutine = base.StartCoroutine(this.FadeBackRoutine());
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x0001D3D9 File Offset: 0x0001B5D9
	private IEnumerator FadeBackRoutine()
	{
		SpriteRenderer[] newSprites = new SpriteRenderer[this.sprites.Length];
		for (int i = 0; i < newSprites.Length; i++)
		{
			newSprites[i] = Object.Instantiate<SpriteRenderer>(this.sprites[i], this.sprites[i].transform.parent);
			newSprites[i].transform.Translate(new Vector3(0f, 0f, -0.001f), Space.World);
			newSprites[i].gameObject.name = this.sprites[i].gameObject.name;
			newSprites[i].sharedMaterial = this.initialMaterial;
			newSprites[i].color = Color.clear;
		}
		this.onFadeEnd = delegate()
		{
			SpriteRenderer[] newSprites2 = newSprites;
			for (int k = 0; k < newSprites2.Length; k++)
			{
				newSprites2[k].color = Color.white;
			}
			for (int l = 0; l < this.sprites.Length; l++)
			{
				Object.DestroyImmediate(this.sprites[l].gameObject);
			}
			Animator component = this.GetComponent<Animator>();
			if (component)
			{
				component.Rebind();
			}
			this.sprites = newSprites;
		};
		for (float elapsed = 0f; elapsed <= this.fadeBackDuration; elapsed += Time.deltaTime)
		{
			SpriteRenderer[] newSprites3 = newSprites;
			for (int j = 0; j < newSprites3.Length; j++)
			{
				newSprites3[j].color = Color.Lerp(Color.clear, Color.white, elapsed / this.fadeBackDuration);
			}
			yield return null;
		}
		if (this.onFadeEnd != null)
		{
			this.onFadeEnd();
			this.onFadeEnd = null;
		}
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x040005AB RID: 1451
	public Material initialMaterial;

	// Token: 0x040005AC RID: 1452
	public float fadeBackDuration = 1f;

	// Token: 0x040005AD RID: 1453
	private SpriteRenderer[] sprites;

	// Token: 0x040005AE RID: 1454
	private Coroutine fadeRoutine;

	// Token: 0x040005AF RID: 1455
	private Action onFadeEnd;
}
