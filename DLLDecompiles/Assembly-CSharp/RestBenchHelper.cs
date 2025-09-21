using System;
using UnityEngine;

// Token: 0x02000365 RID: 869
public sealed class RestBenchHelper : MonoBehaviour
{
	// Token: 0x06001DFA RID: 7674 RVA: 0x0008A920 File Offset: 0x00088B20
	private void OnDisable()
	{
		if (this.heroOnBench)
		{
			this.heroOnBench = false;
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				PlayerData.instance.atBench = false;
				instance.AffectedByGravity(true);
				InteractEvents component = base.gameObject.GetComponent<InteractEvents>();
				if (component)
				{
					component.EndInteraction();
				}
				instance.RegainControl();
				instance.StartAnimationControlToIdle();
			}
		}
	}

	// Token: 0x06001DFB RID: 7675 RVA: 0x0008A983 File Offset: 0x00088B83
	public void SetOnBench(bool onBench)
	{
		this.heroOnBench = onBench;
	}

	// Token: 0x04001D1D RID: 7453
	private bool heroOnBench;
}
