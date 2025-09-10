using System;
using UnityEngine;

// Token: 0x02000259 RID: 601
public class ParticleSystemAutoDisable : MonoBehaviour
{
	// Token: 0x060015AE RID: 5550 RVA: 0x00061F6C File Offset: 0x0006016C
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

	// Token: 0x060015AF RID: 5551 RVA: 0x00061FA5 File Offset: 0x000601A5
	private void OnEnable()
	{
		this.activated = false;
		if (this.disableOnSceneLoad)
		{
			GameManager.instance.NextSceneWillActivate += this.Recycle;
		}
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x00061FCC File Offset: 0x000601CC
	private void OnDisable()
	{
		if (this.disableOnSceneLoad && GameManager.UnsafeInstance)
		{
			GameManager.UnsafeInstance.NextSceneWillActivate -= this.Recycle;
		}
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x00061FF8 File Offset: 0x000601F8
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
			this.Recycle();
		}
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x00062044 File Offset: 0x00060244
	private void Recycle()
	{
		ParticleSystemAutoDisable.DisableBehaviours disableBehaviours = this.disableBehaviours;
		if (disableBehaviours == ParticleSystemAutoDisable.DisableBehaviours.Recycle)
		{
			base.gameObject.Recycle();
			return;
		}
		if (disableBehaviours != ParticleSystemAutoDisable.DisableBehaviours.Disable)
		{
			throw new NotImplementedException();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400144D RID: 5197
	[SerializeField]
	private ParticleSystemAutoDisable.DisableBehaviours disableBehaviours;

	// Token: 0x0400144E RID: 5198
	[SerializeField]
	private bool disableOnSceneLoad = true;

	// Token: 0x0400144F RID: 5199
	private ParticleSystem[] particleSystems;

	// Token: 0x04001450 RID: 5200
	private bool activated;

	// Token: 0x04001451 RID: 5201
	private bool subscribed;

	// Token: 0x02001550 RID: 5456
	private enum DisableBehaviours
	{
		// Token: 0x040086B3 RID: 34483
		Recycle,
		// Token: 0x040086B4 RID: 34484
		Disable
	}
}
