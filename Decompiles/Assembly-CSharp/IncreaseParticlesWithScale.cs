using System;
using UnityEngine;

// Token: 0x02000243 RID: 579
public class IncreaseParticlesWithScale : MonoBehaviour
{
	// Token: 0x06001527 RID: 5415 RVA: 0x0005FB68 File Offset: 0x0005DD68
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Color gray = Color.gray;
		float? a = new float?(0.5f);
		Gizmos.color = gray.Where(null, null, null, a);
		Gizmos.DrawLine(Vector3.left, Vector3.right);
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x0005FBCC File Offset: 0x0005DDCC
	private void Start()
	{
		Transform transform = base.transform;
		ParticleSystem component = base.GetComponent<ParticleSystem>();
		float x = transform.lossyScale.x;
		ParticleSystem.MainModule main = component.main;
		main.maxParticles = Mathf.CeilToInt((float)main.maxParticles * x);
		ParticleSystem.EmissionModule emissionModule;
		component.emission.rateOverTimeMultiplier = emissionModule.rateOverTimeMultiplier * x;
	}
}
