using System;
using UnityEngine;

// Token: 0x020005C7 RID: 1479
public class SpawnStagMenu : MonoBehaviour
{
	// Token: 0x060034C1 RID: 13505 RVA: 0x000EA2DC File Offset: 0x000E84DC
	private void Start()
	{
		if (HeroController.instance)
		{
			HeroController.HeroInPosition temp = null;
			temp = delegate(bool <p0>)
			{
				this.SendEvent();
				HeroController.instance.heroInPosition -= temp;
			};
			HeroController.instance.heroInPosition += temp;
			return;
		}
		this.SendEvent();
	}

	// Token: 0x060034C2 RID: 13506 RVA: 0x000EA332 File Offset: 0x000E8532
	private void SendEvent()
	{
		if (GameCameras.instance)
		{
			this.fsm = GameCameras.instance.openStagFSM;
		}
		if (this.fsm)
		{
			this.fsm.SendEvent("SPAWN");
		}
	}

	// Token: 0x060034C3 RID: 13507 RVA: 0x000EA36D File Offset: 0x000E856D
	private void OnDestroy()
	{
		if (this.fsm)
		{
			this.fsm.SendEvent("DESPAWN");
		}
	}

	// Token: 0x04003833 RID: 14387
	private PlayMakerFSM fsm;
}
