using System;
using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200025E RID: 606
public class PoisonPouchEventCaller : MonoBehaviour
{
	// Token: 0x060015C8 RID: 5576 RVA: 0x00062512 File Offset: 0x00060712
	private void OnEnable()
	{
		if (Gameplay.PoisonPouchTool.IsEquipped)
		{
			this.OnPoisonEquipped.Invoke();
			return;
		}
		this.OnPoisonNotEquipped.Invoke();
	}

	// Token: 0x04001462 RID: 5218
	public UnityEvent OnPoisonEquipped;

	// Token: 0x04001463 RID: 5219
	public UnityEvent OnPoisonNotEquipped;
}
