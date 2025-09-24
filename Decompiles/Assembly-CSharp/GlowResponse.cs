using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020004ED RID: 1261
public class GlowResponse : DebugDrawColliderRuntimeAdder
{
	// Token: 0x06002D2E RID: 11566 RVA: 0x000C541C File Offset: 0x000C361C
	private void OnValidate()
	{
		if (this.fadeSprite)
		{
			this.FadeSprites.Add(this.fadeSprite);
			this.fadeSprite = null;
		}
	}

	// Token: 0x06002D2F RID: 11567 RVA: 0x000C5443 File Offset: 0x000C3643
	protected override void Awake()
	{
		base.Awake();
		this.OnValidate();
	}

	// Token: 0x06002D30 RID: 11568 RVA: 0x000C5454 File Offset: 0x000C3654
	private void Start()
	{
		this.currentAlpha = (float)(this.invert ? 1 : 0);
		foreach (SpriteRenderer spriteRenderer in this.FadeSprites)
		{
			if (spriteRenderer)
			{
				Color color = spriteRenderer.color;
				color.a = this.currentAlpha;
				spriteRenderer.color = color;
			}
		}
		if (this.fadeGroup)
		{
			this.fadeGroup.AlphaSelf = this.currentAlpha;
		}
		if (this.fadeGroupStable)
		{
			this.fadeGroupStable.AlphaSelf = this.currentAlpha;
		}
	}

	// Token: 0x06002D31 RID: 11569 RVA: 0x000C5514 File Offset: 0x000C3714
	private void OnDisable()
	{
		this.insideObjects.Clear();
	}

	// Token: 0x06002D32 RID: 11570 RVA: 0x000C5524 File Offset: 0x000C3724
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.insideObjects.Add(collision.gameObject) || this.insideObjects.Count != 1)
		{
			return;
		}
		if (this.particles)
		{
			this.particles.Play();
		}
		Vector3 position = base.transform.position;
		position.z = 0f;
		if (this.randomAudioClipTable)
		{
			this.randomAudioClipTable.SpawnAndPlayOneShot(this.audioPlayerPrefab, position, false, 1f, null);
		}
		else
		{
			this.soundEffect.SpawnAndPlayOneShot(this.audioPlayerPrefab, position, null);
		}
		this.FadeTo(this.invert ? 0f : this.fadeTo);
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x000C55DC File Offset: 0x000C37DC
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!this.insideObjects.Remove(collision.gameObject) || this.insideObjects.Count != 0)
		{
			return;
		}
		if (this.particles)
		{
			this.particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		this.FadeTo(this.invert ? this.fadeTo : 0f);
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x000C563F File Offset: 0x000C383F
	private void FadeTo(float alpha)
	{
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		if (base.gameObject.activeInHierarchy)
		{
			this.fadeRoutine = base.StartCoroutine(this.Fade(alpha));
		}
	}

	// Token: 0x06002D35 RID: 11573 RVA: 0x000C5675 File Offset: 0x000C3875
	private IEnumerator Fade(float toAlpha)
	{
		float initialAlpha = this.currentAlpha;
		for (float elapsed = 0f; elapsed < this.fadeTime; elapsed += Time.deltaTime)
		{
			this.currentAlpha = Mathf.Lerp(initialAlpha, toAlpha, elapsed / this.fadeTime);
			this.UpdateGlow();
			yield return null;
		}
		this.currentAlpha = toAlpha;
		this.UpdateGlow();
		if (this.useDefaultPulse && this.currentAlpha > Mathf.Epsilon)
		{
			this.fadeRoutine = base.StartCoroutine(this.UpdateGlowLoop());
		}
		yield break;
	}

	// Token: 0x06002D36 RID: 11574 RVA: 0x000C568B File Offset: 0x000C388B
	private IEnumerator UpdateGlowLoop()
	{
		for (;;)
		{
			this.UpdateGlow();
			yield return null;
		}
		yield break;
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x000C569C File Offset: 0x000C389C
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

	// Token: 0x06002D38 RID: 11576 RVA: 0x000C56E8 File Offset: 0x000C38E8
	private void UpdateGlow()
	{
		float num;
		if (this.useDefaultPulse)
		{
			AnimationCurve glowResponsePulseCurve = Effects.GlowResponsePulseCurve;
			float glowResponsePulseDuration = Effects.GlowResponsePulseDuration;
			float glowResponsePulseFrameRate = Effects.GlowResponsePulseFrameRate;
			float time = Mathf.Floor(Time.time * glowResponsePulseFrameRate + 0.5f) / glowResponsePulseFrameRate % glowResponsePulseDuration / glowResponsePulseDuration;
			num = glowResponsePulseCurve.Evaluate(time);
		}
		else
		{
			num = 1f;
		}
		foreach (SpriteRenderer spriteRenderer in this.FadeSprites)
		{
			Color color = spriteRenderer.color;
			color.a = this.currentAlpha * num;
			spriteRenderer.color = color;
		}
		if (this.fadeGroup)
		{
			this.fadeGroup.AlphaSelf = this.currentAlpha * num;
		}
		if (this.fadeGroupStable)
		{
			this.fadeGroupStable.AlphaSelf = this.currentAlpha;
		}
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x000C57D4 File Offset: 0x000C39D4
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region, false);
	}

	// Token: 0x04002ED4 RID: 11988
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private SpriteRenderer fadeSprite;

	// Token: 0x04002ED5 RID: 11989
	[SerializeField]
	private List<SpriteRenderer> FadeSprites = new List<SpriteRenderer>();

	// Token: 0x04002ED6 RID: 11990
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x04002ED7 RID: 11991
	[SerializeField]
	private NestedFadeGroupBase fadeGroupStable;

	// Token: 0x04002ED8 RID: 11992
	[SerializeField]
	private float fadeTime = 0.5f;

	// Token: 0x04002ED9 RID: 11993
	[SerializeField]
	private float fadeTo = 1f;

	// Token: 0x04002EDA RID: 11994
	[SerializeField]
	private bool invert;

	// Token: 0x04002EDB RID: 11995
	[SerializeField]
	private bool useDefaultPulse = true;

	// Token: 0x04002EDC RID: 11996
	[SerializeField]
	private ParticleSystem particles;

	// Token: 0x04002EDD RID: 11997
	private float currentAlpha;

	// Token: 0x04002EDE RID: 11998
	public AudioSource audioPlayerPrefab;

	// Token: 0x04002EDF RID: 11999
	public AudioEventRandom soundEffect;

	// Token: 0x04002EE0 RID: 12000
	public RandomAudioClipTable randomAudioClipTable;

	// Token: 0x04002EE1 RID: 12001
	private Coroutine fadeRoutine;

	// Token: 0x04002EE2 RID: 12002
	private readonly HashSet<GameObject> insideObjects = new HashSet<GameObject>();
}
