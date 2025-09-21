using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200028B RID: 651
[RequireComponent(typeof(BoxCollider2D))]
public class WaterDisturber : MonoBehaviour
{
	// Token: 0x060016DD RID: 5853 RVA: 0x00066EB4 File Offset: 0x000650B4
	private void Start()
	{
		this.overlapCollider = base.GetComponent<BoxCollider2D>();
		Collider2D[] array = Physics2D.OverlapBoxAll(base.transform.position + this.overlapCollider.offset, this.overlapCollider.size, 0f);
		for (int i = 0; i < array.Length; i++)
		{
			WaterDetector component = array[i].GetComponent<WaterDetector>();
			if (component)
			{
				this.detectors.Add(component);
			}
		}
		this.overlapCollider.enabled = false;
		if (this.detectors.Count > 0)
		{
			base.StartCoroutine(this.Disturb());
		}
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x00066F55 File Offset: 0x00065155
	private IEnumerator Disturb()
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		foreach (WaterDetector waterDetector in this.detectors)
		{
			Vector3 position = waterDetector.transform.position;
			if (position.x < num)
			{
				num = position.x;
			}
			if (position.x > num2)
			{
				num2 = position.x;
			}
		}
		for (;;)
		{
			float force = Random.Range(this.minForce, this.maxForce);
			float seconds = Random.Range(this.minDelay, this.maxDelay);
			int index = Random.Range(0, this.detectors.Count);
			this.detectors[index].Splash(force);
			this.splashParticles.Fling(base.transform.position, 1f);
			yield return new WaitForSeconds(seconds);
		}
		yield break;
	}

	// Token: 0x0400155A RID: 5466
	public float minForce = 0.5f;

	// Token: 0x0400155B RID: 5467
	public float maxForce = 1f;

	// Token: 0x0400155C RID: 5468
	[Space]
	public float minDelay = 0.1f;

	// Token: 0x0400155D RID: 5469
	public float maxDelay = 0.25f;

	// Token: 0x0400155E RID: 5470
	[Space]
	public FlingGameObject splashParticles;

	// Token: 0x0400155F RID: 5471
	private BoxCollider2D overlapCollider;

	// Token: 0x04001560 RID: 5472
	private List<WaterDetector> detectors = new List<WaterDetector>();
}
