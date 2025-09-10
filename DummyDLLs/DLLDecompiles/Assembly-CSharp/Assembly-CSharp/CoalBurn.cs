using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004B5 RID: 1205
public class CoalBurn : MonoBehaviour
{
	// Token: 0x17000510 RID: 1296
	// (get) Token: 0x06002B7B RID: 11131 RVA: 0x000BEAD0 File Offset: 0x000BCCD0
	// (set) Token: 0x06002B7C RID: 11132 RVA: 0x000BEB09 File Offset: 0x000BCD09
	private Color SpriteColour
	{
		get
		{
			if (this.tk2dSprite)
			{
				return this.tk2dSprite.color;
			}
			if (this.spriteRenderer)
			{
				return this.spriteRenderer.color;
			}
			return Color.clear;
		}
		set
		{
			if (this.tk2dSprite)
			{
				this.tk2dSprite.color = value;
				return;
			}
			if (this.spriteRenderer)
			{
				this.spriteRenderer.color = value;
			}
		}
	}

	// Token: 0x06002B7D RID: 11133 RVA: 0x000BEB40 File Offset: 0x000BCD40
	private void Awake()
	{
		this.tk2dSprite = base.GetComponent<tk2dSprite>();
		if (!this.tk2dSprite)
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		}
		this.initialSpriteColor = this.SpriteColour;
		if (this.burningEffectsParent)
		{
			this.burningParticles = this.burningEffectsParent.GetComponentsInChildren<ParticleSystem>(true);
			foreach (ParticleSystem particleSystem in this.burningParticles)
			{
				particleSystem.main.playOnAwake = false;
				particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			this.burningEffectsParent.SetActive(true);
		}
		else
		{
			this.burningParticles = Array.Empty<ParticleSystem>();
		}
		if (this.burnUpEffectsParent)
		{
			this.burnUpParticles = this.burnUpEffectsParent.GetComponentsInChildren<ParticleSystem>(true);
			foreach (ParticleSystem particleSystem2 in this.burnUpParticles)
			{
				particleSystem2.main.playOnAwake = false;
				particleSystem2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			this.burnUpEffectsParent.SetActive(true);
		}
		else
		{
			this.burnUpParticles = Array.Empty<ParticleSystem>();
		}
		this.body = base.GetComponent<Rigidbody2D>();
		this.collider = base.GetComponent<Collider2D>();
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x000BEC64 File Offset: 0x000BCE64
	private void OnDisable()
	{
		if (this.burnRoutine != null)
		{
			base.StopCoroutine(this.burnRoutine);
			this.burnRoutine = null;
		}
		this.StopBurning();
		this.SpriteColour = this.initialSpriteColor;
		this.hasBurnedUp = false;
		this.enteredColliders.Clear();
		this.inCoalRegion = false;
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x000BECB8 File Offset: 0x000BCEB8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.hasBurnedUp)
		{
			return;
		}
		if (!this.IsCoalRegion(collision))
		{
			return;
		}
		this.enteredColliders.Add(collision);
		this.inCoalRegion = true;
		if (this.burnRoutine == null)
		{
			this.burnRoutine = base.StartCoroutine(this.BurnMonitor());
		}
	}

	// Token: 0x06002B80 RID: 11136 RVA: 0x000BED06 File Offset: 0x000BCF06
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.hasBurnedUp)
		{
			return;
		}
		if (!this.inCoalRegion)
		{
			return;
		}
		if (!this.IsCoalRegion(collision))
		{
			return;
		}
		this.enteredColliders.Remove(collision);
		if (this.enteredColliders.Count == 0)
		{
			this.inCoalRegion = false;
		}
	}

	// Token: 0x06002B81 RID: 11137 RVA: 0x000BED48 File Offset: 0x000BCF48
	private bool IsCoalRegion(Collider2D collision)
	{
		if (collision.CompareTag("Geo"))
		{
			return false;
		}
		GameObject gameObject = collision.gameObject;
		return gameObject.GetComponent<GlowResponseCoal>() || (gameObject.layer == 17 && gameObject.CompareTag("Coal"));
	}

	// Token: 0x06002B82 RID: 11138 RVA: 0x000BED91 File Offset: 0x000BCF91
	private IEnumerator BurnMonitor()
	{
		for (;;)
		{
			IL_52:
			if (this.inCoalRegion)
			{
				float elapsed;
				for (elapsed = 0f; elapsed < this.burnDelay; elapsed += Time.deltaTime)
				{
					if (!this.inCoalRegion)
					{
						goto IL_52;
					}
					yield return null;
				}
				elapsed = 0f;
				while (this.inCoalRegion)
				{
					this.StartBurning();
					this.SetBurnTime(elapsed);
					while (elapsed < this.burnTime)
					{
						if (!this.inCoalRegion)
						{
							this.StopBurning();
							break;
						}
						yield return null;
						elapsed += Time.deltaTime;
						this.SetBurnTime(elapsed);
					}
					if (elapsed >= this.burnTime)
					{
						goto Block_5;
					}
					this.SetBurnTime(elapsed);
					while (elapsed > 0f && !this.inCoalRegion)
					{
						yield return null;
						elapsed -= Time.deltaTime;
						this.SetBurnTime(elapsed);
					}
				}
				this.StopBurning();
			}
			else
			{
				yield return null;
			}
		}
		Block_5:
		this.BurnUp();
		for (float elapsed = 0f; elapsed < 0.5f; elapsed += Time.deltaTime)
		{
			this.SpriteColour = Color.Lerp(this.burnColor, Color.clear, elapsed / 0.5f);
			yield return null;
		}
		this.SpriteColour = Color.clear;
		for (;;)
		{
			bool flag = false;
			ParticleSystem[] array = this.burnUpParticles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
			yield return null;
		}
		this.burnRoutine = null;
		yield break;
		yield break;
	}

	// Token: 0x06002B83 RID: 11139 RVA: 0x000BEDA0 File Offset: 0x000BCFA0
	private void StartBurning()
	{
		this.playingParticle = true;
		ParticleSystem[] array = this.burningParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(true);
		}
	}

	// Token: 0x06002B84 RID: 11140 RVA: 0x000BEDD4 File Offset: 0x000BCFD4
	private void StopBurning()
	{
		if (!this.playingParticle)
		{
			return;
		}
		this.playingParticle = false;
		ParticleSystem[] array = this.burningParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	// Token: 0x06002B85 RID: 11141 RVA: 0x000BEE10 File Offset: 0x000BD010
	private void BurnUp()
	{
		this.StopBurning();
		this.hasBurnedUp = true;
		ParticleSystem[] array = this.burnUpParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(true);
		}
		this.collider.enabled = false;
		this.body.linearVelocity = Vector2.zero;
		this.body.isKinematic = true;
		this.onBurnedUp.Invoke();
		IBurnable component = base.gameObject.GetComponent<IBurnable>();
		if (component != null)
		{
			component.BurnUp();
		}
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x000BEE90 File Offset: 0x000BD090
	private void SetBurnTime(float time)
	{
		if (time < 0f)
		{
			time = 0f;
		}
		else if (time > this.burnTime)
		{
			time = this.burnTime;
		}
		float t = time / this.burnTime;
		this.SpriteColour = Color.Lerp(this.initialSpriteColor, this.burnColor, t);
	}

	// Token: 0x04002CC2 RID: 11458
	[SerializeField]
	private GameObject burningEffectsParent;

	// Token: 0x04002CC3 RID: 11459
	[SerializeField]
	private GameObject burnUpEffectsParent;

	// Token: 0x04002CC4 RID: 11460
	[SerializeField]
	private float burnDelay = 2f;

	// Token: 0x04002CC5 RID: 11461
	[SerializeField]
	private float burnTime = 1.5f;

	// Token: 0x04002CC6 RID: 11462
	[SerializeField]
	private float unburnTime = 0.5f;

	// Token: 0x04002CC7 RID: 11463
	[SerializeField]
	private Color burnColor = Color.black;

	// Token: 0x04002CC8 RID: 11464
	[Space]
	[SerializeField]
	private UnityEvent onBurnedUp;

	// Token: 0x04002CC9 RID: 11465
	private bool inCoalRegion;

	// Token: 0x04002CCA RID: 11466
	private bool hasBurnedUp;

	// Token: 0x04002CCB RID: 11467
	private Coroutine burnRoutine;

	// Token: 0x04002CCC RID: 11468
	private Color initialSpriteColor;

	// Token: 0x04002CCD RID: 11469
	private ParticleSystem[] burningParticles;

	// Token: 0x04002CCE RID: 11470
	private ParticleSystem[] burnUpParticles;

	// Token: 0x04002CCF RID: 11471
	private tk2dSprite tk2dSprite;

	// Token: 0x04002CD0 RID: 11472
	private SpriteRenderer spriteRenderer;

	// Token: 0x04002CD1 RID: 11473
	private Rigidbody2D body;

	// Token: 0x04002CD2 RID: 11474
	private Collider2D collider;

	// Token: 0x04002CD3 RID: 11475
	private bool playingParticle;

	// Token: 0x04002CD4 RID: 11476
	private HashSet<Collider2D> enteredColliders = new HashSet<Collider2D>();
}
