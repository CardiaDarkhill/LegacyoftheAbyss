using System;
using UnityEngine;

// Token: 0x0200044A RID: 1098
public class ParticleSystemAutoDestroy : MonoBehaviour
{
	// Token: 0x060026A0 RID: 9888 RVA: 0x000AECD0 File Offset: 0x000ACED0
	public void Start()
	{
		this.ps = base.GetComponent<ParticleSystem>();
	}

	// Token: 0x060026A1 RID: 9889 RVA: 0x000AECE0 File Offset: 0x000ACEE0
	public void Update()
	{
		if (this.ps)
		{
			if (this.ps.IsAlive())
			{
				this.activated = true;
			}
			if (!this.ps.IsAlive() && this.activated)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x040023FF RID: 9215
	private ParticleSystem ps;

	// Token: 0x04002400 RID: 9216
	private bool activated;
}
