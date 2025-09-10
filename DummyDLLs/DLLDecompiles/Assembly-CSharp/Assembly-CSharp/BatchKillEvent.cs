using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000142 RID: 322
public sealed class BatchKillEvent : MonoBehaviour
{
	// Token: 0x060009F4 RID: 2548 RVA: 0x0002D138 File Offset: 0x0002B338
	private void Awake()
	{
		bool flag = this.eventSource;
		if (!flag)
		{
			this.eventSource = base.GetComponent<EventBase>();
			flag = this.eventSource;
		}
		if (flag)
		{
			this.eventSource.ReceivedEvent += this.OnReceivedEvent;
		}
		if (!this.removeNullOnEvent)
		{
			this.healthManagers.RemoveAll((HealthManager o) => o == null);
		}
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x0002D1B9 File Offset: 0x0002B3B9
	private void OnValidate()
	{
		if (this.eventSource == null)
		{
			this.eventSource = base.GetComponent<EventBase>();
		}
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x0002D1D8 File Offset: 0x0002B3D8
	[ContextMenu("Gather Health Managers")]
	private void GatherHealthManagers()
	{
		this.healthManagers.RemoveAll((HealthManager o) => o == null);
		this.healthManagers = this.healthManagers.Union(base.gameObject.GetComponentsInChildren<HealthManager>(true)).ToList<HealthManager>();
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x0002D234 File Offset: 0x0002B434
	private void OnReceivedEvent()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.removeNullOnEvent)
		{
			this.healthManagers.RemoveAll((HealthManager o) => o == null);
		}
		for (int i = this.healthManagers.Count - 1; i >= 0; i--)
		{
			HealthManager healthManager = this.healthManagers[i];
			if (!healthManager.isDead)
			{
				healthManager.Die(new float?((float)healthManager.GetAttackDirection()), AttackTypes.Generic, NailElements.None, null, false, 1f, true, false);
			}
		}
	}

	// Token: 0x04000980 RID: 2432
	[SerializeField]
	private EventBase eventSource;

	// Token: 0x04000981 RID: 2433
	[SerializeField]
	private List<HealthManager> healthManagers = new List<HealthManager>();

	// Token: 0x04000982 RID: 2434
	[SerializeField]
	private bool removeNullOnEvent = true;
}
