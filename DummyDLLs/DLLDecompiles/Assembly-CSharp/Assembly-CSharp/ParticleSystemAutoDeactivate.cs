using System;
using UnityEngine;

// Token: 0x02000258 RID: 600
public class ParticleSystemAutoDeactivate : MonoBehaviour
{
	// Token: 0x060015AB RID: 5547 RVA: 0x00061ED8 File Offset: 0x000600D8
	public void Start()
	{
		ParticleSystem component = base.GetComponent<ParticleSystem>();
		if (component)
		{
			this.particleSystems = new ParticleSystem[]
			{
				component
			};
			return;
		}
		this.particleSystems = base.GetComponentsInChildren<ParticleSystem>();
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x00061F14 File Offset: 0x00060114
	public void Update()
	{
		bool flag = false;
		ParticleSystem[] array = this.particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsAlive())
			{
				this.activated = true;
				flag = true;
				break;
			}
		}
		if (!flag && this.activated)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0400144A RID: 5194
	private ParticleSystem[] particleSystems;

	// Token: 0x0400144B RID: 5195
	private bool activated;

	// Token: 0x0400144C RID: 5196
	private bool subscribed;
}
