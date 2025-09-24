using System;
using System.Collections.Generic;
using TeamCherry.Splines;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200055D RID: 1373
public class SplineBladeTrap : CompoundSpline
{
	// Token: 0x06003110 RID: 12560 RVA: 0x000D9AEF File Offset: 0x000D7CEF
	private void OnValidate()
	{
		if (this.poolBlades < 1)
		{
			this.poolBlades = 1;
		}
	}

	// Token: 0x06003111 RID: 12561 RVA: 0x000D9B04 File Offset: 0x000D7D04
	private void Awake()
	{
		if (this.pressurePlate)
		{
			this.pressurePlate.OnPressed.AddListener(new UnityAction(this.TriggerBladeMove));
		}
		this.bladeTemplate.gameObject.SetActive(false);
		this.bladePool = new Queue<Rigidbody2D>(this.poolBlades);
		for (int i = 0; i < this.poolBlades; i++)
		{
			this.bladePool.Enqueue(Object.Instantiate<Rigidbody2D>(this.bladeTemplate, this.bladeTemplate.transform.parent));
		}
		if (this.bladeEndAudioObject)
		{
			this.bladeEndAudioSource = this.bladeEndAudioObject.GetComponent<AudioSource>();
		}
	}

	// Token: 0x06003112 RID: 12562 RVA: 0x000D9BB4 File Offset: 0x000D7DB4
	private void Update()
	{
		if (this.pressurePlateResetTimer > 0f)
		{
			this.pressurePlateResetTimer -= Time.deltaTime;
			if (this.pressurePlateResetTimer <= 0f && this.pressurePlate)
			{
				this.pressurePlate.SetBlocked(false);
			}
		}
		for (int i = 0; i < this.currentBlades.Count; i++)
		{
			SplineBladeTrap.BladeTracker bladeTracker = this.currentBlades[i];
			if (bladeTracker.EmergeDelayLeft > 0f)
			{
				bladeTracker.EmergeDelayLeft -= Time.deltaTime;
				if (bladeTracker.EmergeDelayLeft <= 0f)
				{
					bladeTracker.Blade.gameObject.SetActive(true);
					bladeTracker.CurrentDistance = 0f;
					this.UpdateBlade(bladeTracker);
					bladeTracker.Blade.transform.SetPosition2D(bladeTracker.Blade.position);
				}
				this.currentBlades[i] = bladeTracker;
			}
		}
	}

	// Token: 0x06003113 RID: 12563 RVA: 0x000D9CA8 File Offset: 0x000D7EA8
	private void FixedUpdate()
	{
		for (int i = this.currentBlades.Count - 1; i >= 0; i--)
		{
			SplineBladeTrap.BladeTracker bladeTracker = this.currentBlades[i];
			bladeTracker.CurrentDistance += this.bladeSpeed * Time.deltaTime;
			if (bladeTracker.CurrentDistance >= base.TotalDistance)
			{
				if (this.bladeEndAudioObject)
				{
					this.bladeEndAudioObject.transform.position = bladeTracker.Blade.gameObject.transform.position;
					this.bladeEndAudioSource.Play();
				}
				this.currentBlades.RemoveAt(i);
				bladeTracker.Blade.gameObject.SetActive(false);
				this.bladePool.Enqueue(bladeTracker.Blade);
			}
			else
			{
				this.UpdateBlade(bladeTracker);
				this.currentBlades[i] = bladeTracker;
			}
		}
	}

	// Token: 0x06003114 RID: 12564 RVA: 0x000D9D88 File Offset: 0x000D7F88
	public void TriggerBladeMove()
	{
		if (this.pressurePlateResetTimer > 0f)
		{
			return;
		}
		this.pressurePlateResetTimer = this.pressurePlateResetTime;
		if (this.pressurePlate)
		{
			this.pressurePlate.SetBlocked(true);
		}
		this.emergeAnticParticles.Play();
		Rigidbody2D blade = (this.bladePool.Count > 0) ? this.bladePool.Dequeue() : Object.Instantiate<Rigidbody2D>(this.bladeTemplate, this.bladeTemplate.transform.parent);
		this.currentBlades.Add(new SplineBladeTrap.BladeTracker
		{
			Blade = blade,
			EmergeDelayLeft = this.bladeEmergeDelay
		});
	}

	// Token: 0x06003115 RID: 12565 RVA: 0x000D9E34 File Offset: 0x000D8034
	private void UpdateBlade(SplineBladeTrap.BladeTracker bladeTracker)
	{
		Vector2 position = bladeTracker.Blade.position;
		Vector2 positionAlongSpline = base.GetPositionAlongSpline(bladeTracker.CurrentDistance);
		bladeTracker.Blade.position = positionAlongSpline;
		Vector3 to = positionAlongSpline - position;
		to.z = 0f;
		to.Normalize();
		float rotation = Vector3.SignedAngle(Vector3.right, to, Vector3.forward);
		bladeTracker.Blade.rotation = rotation;
	}

	// Token: 0x0400345A RID: 13402
	[SerializeField]
	[FormerlySerializedAs("blade")]
	private Rigidbody2D bladeTemplate;

	// Token: 0x0400345B RID: 13403
	[SerializeField]
	private int poolBlades;

	// Token: 0x0400345C RID: 13404
	[SerializeField]
	private float bladeEmergeDelay;

	// Token: 0x0400345D RID: 13405
	[SerializeField]
	private float bladeSpeed;

	// Token: 0x0400345E RID: 13406
	[Space]
	[SerializeField]
	private TrapPressurePlate pressurePlate;

	// Token: 0x0400345F RID: 13407
	[SerializeField]
	private float pressurePlateResetTime;

	// Token: 0x04003460 RID: 13408
	[Space]
	[SerializeField]
	private ParticleSystem emergeAnticParticles;

	// Token: 0x04003461 RID: 13409
	[SerializeField]
	private GameObject bladeEndAudioObject;

	// Token: 0x04003462 RID: 13410
	private Queue<Rigidbody2D> bladePool;

	// Token: 0x04003463 RID: 13411
	private List<SplineBladeTrap.BladeTracker> currentBlades = new List<SplineBladeTrap.BladeTracker>();

	// Token: 0x04003464 RID: 13412
	private float pressurePlateResetTimer;

	// Token: 0x04003465 RID: 13413
	private AudioSource bladeEndAudioSource;

	// Token: 0x02001860 RID: 6240
	private struct BladeTracker
	{
		// Token: 0x040091C5 RID: 37317
		public Rigidbody2D Blade;

		// Token: 0x040091C6 RID: 37318
		public float EmergeDelayLeft;

		// Token: 0x040091C7 RID: 37319
		public float CurrentDistance;
	}
}
