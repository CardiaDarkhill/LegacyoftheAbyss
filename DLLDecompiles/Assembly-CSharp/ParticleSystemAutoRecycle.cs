using System;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class ParticleSystemAutoRecycle : MonoBehaviour
{
	// Token: 0x060015B4 RID: 5556 RVA: 0x0006208E File Offset: 0x0006028E
	public void Start()
	{
		this.ps = base.GetComponents<ParticleSystem>();
		if (this.ps.Length == 0)
		{
			this.ps = base.GetComponentsInChildren<ParticleSystem>(true);
		}
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x000620B2 File Offset: 0x000602B2
	private void OnDisable()
	{
		this.activated = false;
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x000620BC File Offset: 0x000602BC
	public void Update()
	{
		if (this.ps.Length == 0)
		{
			return;
		}
		ParticleSystem[] array;
		if (!this.activated)
		{
			array = this.ps;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive())
				{
					this.activated = true;
					return;
				}
			}
			return;
		}
		bool flag = false;
		array = this.ps;
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
			this.RecycleSelf();
		}
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x00062131 File Offset: 0x00060331
	public void RecycleSelf()
	{
		this.Recycle<ParticleSystemAutoRecycle>();
	}

	// Token: 0x04001452 RID: 5202
	private ParticleSystem[] ps;

	// Token: 0x04001453 RID: 5203
	private bool activated;
}
