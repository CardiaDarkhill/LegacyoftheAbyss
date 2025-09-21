using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000257 RID: 599
public class ParticleEffectsScaleToCollider : MonoBehaviour
{
	// Token: 0x060015A2 RID: 5538 RVA: 0x00061B6D File Offset: 0x0005FD6D
	private void OnValidate()
	{
		if (this.lerpEmission != null && this.particles != null && this.particles.Length != 0)
		{
			this.particles = null;
		}
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x00061B95 File Offset: 0x0005FD95
	private void Awake()
	{
		if (!this.lerpEmission)
		{
			this.particleInfos = new Dictionary<ParticleSystem, ParticleEffectsScaleToCollider.ParticleSystemInfo>();
		}
		this.initialScale = base.transform.localScale;
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x00061BC0 File Offset: 0x0005FDC0
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		if (!this.lerpEmission)
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				this.particleInfos[particleSystem] = new ParticleEffectsScaleToCollider.ParticleSystemInfo
				{
					EmissionRate = emission.rateOverTimeMultiplier
				};
			}
		}
		if (this.useParent)
		{
			Transform parent = base.transform.parent;
			if (parent != null)
			{
				ParticleEffectsScaleTarget component = parent.GetComponent<ParticleEffectsScaleTarget>();
				if (component != null)
				{
					this.SetScaleToCollider(component.Target);
					return;
				}
				this.SetScaleToGameObject(base.transform.parent.gameObject);
			}
		}
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x00061C7D File Offset: 0x0005FE7D
	private void Start()
	{
		this.hasStarted = true;
		this.OnEnable();
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x00061C8C File Offset: 0x0005FE8C
	private void OnDisable()
	{
		if (!this.lerpEmission)
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				ParticleEffectsScaleToCollider.ParticleSystemInfo particleSystemInfo = this.particleInfos[particleSystem];
				particleSystem.emission.rateOverTimeMultiplier = particleSystemInfo.EmissionRate;
			}
		}
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x00061CE4 File Offset: 0x0005FEE4
	public void SetScaleToGameObject(GameObject obj)
	{
		Collider2D component = obj.GetComponent<Collider2D>();
		if (component)
		{
			this.SetScaleToCollider(component);
			return;
		}
		Debug.LogError("Could not find collider", this);
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x00061D14 File Offset: 0x0005FF14
	public void SetScaleToCollider(Collider2D col)
	{
		base.transform.localScale = this.initialScale;
		Vector2 vector;
		if (col.isActiveAndEnabled)
		{
			vector = col.bounds.size;
		}
		else
		{
			Rigidbody2D attachedRigidbody = col.attachedRigidbody;
			bool flag = false;
			Bounds bounds = default(Bounds);
			if (attachedRigidbody)
			{
				ParticleEffectsScaleToCollider.colliders.Clear();
				int attachedColliders = attachedRigidbody.GetAttachedColliders(ParticleEffectsScaleToCollider.colliders);
				for (int i = 0; i < attachedColliders; i++)
				{
					Collider2D collider2D = ParticleEffectsScaleToCollider.colliders[i];
					if (collider2D.enabled && !collider2D.isTrigger)
					{
						if (!flag)
						{
							bounds = collider2D.bounds;
							flag = true;
						}
						else
						{
							bounds.Encapsulate(collider2D.bounds);
						}
					}
				}
				ParticleEffectsScaleToCollider.colliders.Clear();
			}
			if (flag)
			{
				vector = bounds.size;
			}
			else
			{
				vector = this.referenceColliderSize;
			}
		}
		float area = this.referenceColliderSize.GetArea();
		float num = vector.GetArea() / area;
		Vector2 original = vector.DivideElements(this.referenceColliderSize);
		Vector3 self = base.transform.InverseTransformVector(this.initialScale.MultiplyElements(original.ToVector3(1f)));
		base.transform.localScale = self.Abs();
		if (this.lerpEmission)
		{
			this.lerpEmission.TotalMultiplier = num;
			return;
		}
		foreach (ParticleSystem particleSystem in this.particles)
		{
			ParticleEffectsScaleToCollider.ParticleSystemInfo particleSystemInfo = this.particleInfos[particleSystem];
			particleSystem.emission.rateOverTimeMultiplier = particleSystemInfo.EmissionRate * num;
		}
	}

	// Token: 0x04001442 RID: 5186
	[SerializeField]
	private ParticleSystem[] particles;

	// Token: 0x04001443 RID: 5187
	[SerializeField]
	private ParticleEffectsLerpEmission lerpEmission;

	// Token: 0x04001444 RID: 5188
	[SerializeField]
	private Vector2 referenceColliderSize = Vector2.one;

	// Token: 0x04001445 RID: 5189
	[SerializeField]
	private bool useParent;

	// Token: 0x04001446 RID: 5190
	private bool hasStarted;

	// Token: 0x04001447 RID: 5191
	private Vector3 initialScale;

	// Token: 0x04001448 RID: 5192
	private Dictionary<ParticleSystem, ParticleEffectsScaleToCollider.ParticleSystemInfo> particleInfos;

	// Token: 0x04001449 RID: 5193
	private static List<Collider2D> colliders = new List<Collider2D>();

	// Token: 0x0200154F RID: 5455
	[Serializable]
	private struct ParticleSystemInfo
	{
		// Token: 0x040086B1 RID: 34481
		public float EmissionRate;
	}
}
