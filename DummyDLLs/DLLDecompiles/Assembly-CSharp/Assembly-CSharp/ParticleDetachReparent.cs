using System;
using UnityEngine;

// Token: 0x02000529 RID: 1321
public sealed class ParticleDetachReparent : MonoBehaviour
{
	// Token: 0x06002F79 RID: 12153 RVA: 0x000D10F4 File Offset: 0x000CF2F4
	private void Awake()
	{
		if (this.particleSystem == null)
		{
			this.particleSystem = base.GetComponent<ParticleSystem>();
			if (this.particleSystem == null)
			{
				base.enabled = false;
				return;
			}
		}
		this.originalParent = base.transform.parent;
		this.hasParent = (this.originalParent != null);
		if (this.originalParent == null)
		{
			base.enabled = false;
			return;
		}
		DestroyCallback.AddCallback(this.originalParent.gameObject, delegate
		{
			this.parentDestroyed = true;
			this.hasParent = false;
		});
	}

	// Token: 0x06002F7A RID: 12154 RVA: 0x000D1186 File Offset: 0x000CF386
	private void OnValidate()
	{
		if (this.particleSystem == null)
		{
			this.particleSystem = base.GetComponent<ParticleSystem>();
		}
	}

	// Token: 0x06002F7B RID: 12155 RVA: 0x000D11A4 File Offset: 0x000CF3A4
	private void OnEnable()
	{
		if (base.transform.parent != null && !this.detached)
		{
			this.originalParent = base.transform.parent;
			this.localPosition = base.transform.localPosition;
			this.localRotation = base.transform.localRotation;
			this.localScale = base.transform.localScale;
			base.transform.parent = null;
			this.detached = true;
			this.stopTriggered = false;
		}
	}

	// Token: 0x06002F7C RID: 12156 RVA: 0x000D122C File Offset: 0x000CF42C
	private void OnDisable()
	{
		if (this.detached)
		{
			this.detached = false;
			if (this.originalParent)
			{
				base.transform.parent = this.originalParent;
				base.transform.localPosition = this.localPosition;
				base.transform.localRotation = this.localRotation;
				base.transform.localScale = this.localScale;
			}
		}
	}

	// Token: 0x06002F7D RID: 12157 RVA: 0x000D129C File Offset: 0x000CF49C
	private void LateUpdate()
	{
		if (!this.detached)
		{
			return;
		}
		if (this.hasParent && this.originalParent.gameObject.activeInHierarchy)
		{
			base.transform.position = this.originalParent.TransformPoint(this.localPosition);
			base.transform.rotation = this.originalParent.rotation * this.localRotation;
			return;
		}
		if (!this.stopTriggered)
		{
			this.particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			this.stopTriggered = true;
		}
		if (!this.particleSystem.IsAlive(true))
		{
			this.detached = false;
			if (this.hasParent)
			{
				base.transform.parent = this.originalParent;
				base.transform.localPosition = this.localPosition;
				base.transform.localRotation = this.localRotation;
				base.transform.localScale = this.localScale;
				return;
			}
			if (this.parentDestroyed)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x04003239 RID: 12857
	[SerializeField]
	private ParticleSystem particleSystem;

	// Token: 0x0400323A RID: 12858
	private Transform originalParent;

	// Token: 0x0400323B RID: 12859
	private Vector3 localPosition;

	// Token: 0x0400323C RID: 12860
	private Quaternion localRotation;

	// Token: 0x0400323D RID: 12861
	private Vector3 localScale;

	// Token: 0x0400323E RID: 12862
	private bool detached;

	// Token: 0x0400323F RID: 12863
	private bool stopTriggered;

	// Token: 0x04003240 RID: 12864
	private bool hasParent;

	// Token: 0x04003241 RID: 12865
	private bool parentDestroyed;
}
