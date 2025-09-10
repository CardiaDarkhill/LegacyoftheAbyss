using System;
using UnityEngine;

// Token: 0x020002C4 RID: 708
public class CycloneDust : MonoBehaviour
{
	// Token: 0x060018F5 RID: 6389 RVA: 0x0007244C File Offset: 0x0007064C
	private void Start()
	{
		this.parent = base.transform.parent;
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x0007245F File Offset: 0x0007065F
	private void OnEnable()
	{
		this.playing = false;
		this.particle.Stop();
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x00072474 File Offset: 0x00070674
	private void Update()
	{
		if (this.parent.position.y < this.dustY)
		{
			if (!this.playing)
			{
				this.particle.Play();
				this.playing = true;
				return;
			}
		}
		else if (this.playing)
		{
			this.particle.Stop();
			this.playing = false;
		}
	}

	// Token: 0x040017E6 RID: 6118
	public float dustY;

	// Token: 0x040017E7 RID: 6119
	public ParticleSystem particle;

	// Token: 0x040017E8 RID: 6120
	private Transform parent;

	// Token: 0x040017E9 RID: 6121
	private bool playing;
}
