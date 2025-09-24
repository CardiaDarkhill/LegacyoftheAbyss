using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004EF RID: 1263
public class GlowResponseCoal : DebugDrawColliderRuntimeAdder
{
	// Token: 0x06002D3E RID: 11582 RVA: 0x000C588C File Offset: 0x000C3A8C
	private void OnValidate()
	{
		this.HandleUpgrade();
	}

	// Token: 0x06002D3F RID: 11583 RVA: 0x000C5894 File Offset: 0x000C3A94
	protected override void Awake()
	{
		base.Awake();
		this.HandleUpgrade();
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x000C58A2 File Offset: 0x000C3AA2
	private void HandleUpgrade()
	{
		if (this.fadeSprite)
		{
			this.FadeSprites.Add(this.fadeSprite);
			this.fadeSprite = null;
		}
	}

	// Token: 0x06002D41 RID: 11585 RVA: 0x000C58CC File Offset: 0x000C3ACC
	private void Start()
	{
		foreach (SpriteRenderer spriteRenderer in this.FadeSprites)
		{
			if (spriteRenderer)
			{
				Color color = spriteRenderer.color;
				color.a = 0f;
				spriteRenderer.color = color;
			}
		}
	}

	// Token: 0x06002D42 RID: 11586 RVA: 0x000C593C File Offset: 0x000C3B3C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (GlowResponseCoal.ShouldCollide(gameObject))
		{
			this.enteredObjects.Add(gameObject);
			if (!this.glowing)
			{
				this.StartGlow();
			}
		}
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x000C5974 File Offset: 0x000C3B74
	private static bool ShouldCollide(GameObject gameObject)
	{
		return gameObject.layer == 9 || gameObject.layer == 11 || gameObject.layer == 26 || gameObject.layer == 20 || gameObject.CompareTag("Geo") || gameObject.GetComponent<CoalBurn>() != null;
	}

	// Token: 0x06002D44 RID: 11588 RVA: 0x000C59C4 File Offset: 0x000C3BC4
	private void OnTriggerExit2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (this.enteredObjects.Remove(gameObject) && this.glowing && this.enteredObjects.Count <= 0)
		{
			this.EndGlow();
		}
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x000C5A04 File Offset: 0x000C3C04
	private void StartGlow()
	{
		this.glowing = true;
		if (this.particles)
		{
			this.particles.Play();
		}
		Vector3 position = base.transform.position;
		position.z = 0f;
		this.soundEffect.SpawnAndPlayOneShot(this.audioPlayerPrefab, position, null);
		this.FadeTo(this.fadeTo);
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x000C5A68 File Offset: 0x000C3C68
	private void EndGlow()
	{
		this.glowing = false;
		if (this.particles)
		{
			this.particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		this.FadeTo(0f);
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x000C5A98 File Offset: 0x000C3C98
	private void FadeTo(float alpha)
	{
		this.FadeSprites.RemoveAll((SpriteRenderer o) => o == null);
		if (this.FadeSprites.Count > 0)
		{
			if (this.fadeRoutine != null)
			{
				base.StopCoroutine(this.fadeRoutine);
			}
			this.fadeRoutine = base.StartCoroutine(this.Fade(alpha));
		}
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x000C5B05 File Offset: 0x000C3D05
	private IEnumerator Fade(float toAlpha, SpriteRenderer sprite)
	{
		float elapsed = 0f;
		Color initialColor = sprite ? sprite.color : Color.white;
		Color currentColor = initialColor;
		while (elapsed < this.fadeTime)
		{
			if (sprite)
			{
				currentColor.a = Mathf.Lerp(initialColor.a, toAlpha, elapsed / this.fadeTime);
				sprite.color = currentColor;
			}
			yield return null;
			elapsed += Time.deltaTime;
		}
		if (sprite)
		{
			currentColor.a = toAlpha;
			sprite.color = currentColor;
		}
		yield break;
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x000C5B22 File Offset: 0x000C3D22
	private IEnumerator Fade(float toAlpha)
	{
		float elapsed = 0f;
		this.fadeColors.Clear();
		this.fadeColors.Capacity = this.FadeSprites.Count;
		foreach (SpriteRenderer spriteRenderer in this.FadeSprites)
		{
			this.fadeColors.Add(spriteRenderer.color);
		}
		if (this.fadeTime > 0f)
		{
			float rate = 1f / this.fadeTime;
			while (elapsed < this.fadeTime)
			{
				for (int i = this.FadeSprites.Count - 1; i >= 0; i--)
				{
					SpriteRenderer spriteRenderer2 = this.FadeSprites[i];
					if (spriteRenderer2 == null)
					{
						this.FadeSprites.RemoveAt(i);
						this.fadeColors.RemoveAt(i);
					}
					else
					{
						Color color = this.fadeColors[i];
						color.a = Mathf.MoveTowards(color.a, toAlpha, rate * Time.deltaTime);
						spriteRenderer2.color = color;
						this.fadeColors[i] = color;
					}
				}
				yield return null;
				elapsed += Time.deltaTime;
			}
		}
		for (int j = this.FadeSprites.Count - 1; j >= 0; j--)
		{
			SpriteRenderer spriteRenderer3 = this.FadeSprites[j];
			if (spriteRenderer3 == null)
			{
				this.FadeSprites.RemoveAt(j);
				this.fadeColors.RemoveAt(j);
			}
			else
			{
				Color color2 = this.fadeColors[j];
				color2.a = toAlpha;
				spriteRenderer3.color = color2;
			}
		}
		this.fadeColors.Clear();
		yield break;
	}

	// Token: 0x06002D4A RID: 11594 RVA: 0x000C5B38 File Offset: 0x000C3D38
	public void FadeEnd()
	{
		this.FadeTo(0f);
		if (this.particles)
		{
			this.particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		CircleCollider2D component = base.GetComponent<CircleCollider2D>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	// Token: 0x06002D4B RID: 11595 RVA: 0x000C5B81 File Offset: 0x000C3D81
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region, false);
	}

	// Token: 0x04002EE4 RID: 12004
	[HideInInspector]
	[SerializeField]
	private SpriteRenderer fadeSprite;

	// Token: 0x04002EE5 RID: 12005
	public List<SpriteRenderer> FadeSprites = new List<SpriteRenderer>();

	// Token: 0x04002EE6 RID: 12006
	public float fadeTime = 0.5f;

	// Token: 0x04002EE7 RID: 12007
	public float fadeTo = 1f;

	// Token: 0x04002EE8 RID: 12008
	public ParticleSystem particles;

	// Token: 0x04002EE9 RID: 12009
	public AudioSource audioPlayerPrefab;

	// Token: 0x04002EEA RID: 12010
	public AudioEventRandom soundEffect;

	// Token: 0x04002EEB RID: 12011
	private bool glowing;

	// Token: 0x04002EEC RID: 12012
	private HashSet<GameObject> enteredObjects = new HashSet<GameObject>();

	// Token: 0x04002EED RID: 12013
	private Dictionary<SpriteRenderer, Coroutine> fadeRoutines = new Dictionary<SpriteRenderer, Coroutine>();

	// Token: 0x04002EEE RID: 12014
	private Coroutine fadeRoutine;

	// Token: 0x04002EEF RID: 12015
	private List<Color> fadeColors = new List<Color>();
}
