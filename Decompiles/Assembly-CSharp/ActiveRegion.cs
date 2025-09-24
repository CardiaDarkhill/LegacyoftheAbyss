using System;
using UnityEngine;

// Token: 0x02000340 RID: 832
[RequireComponent(typeof(BoxCollider2D))]
public class ActiveRegion : MonoBehaviour
{
	// Token: 0x06001CF9 RID: 7417 RVA: 0x00086918 File Offset: 0x00084B18
	private void OnTriggerEnter2D(Collider2D col)
	{
		FSMActivator component = col.GetComponent<FSMActivator>();
		if (component)
		{
			if (this.staggeredActivation)
			{
				base.StartCoroutine(component.ActivateStaggered());
				return;
			}
			component.Activate();
		}
	}

	// Token: 0x04001C54 RID: 7252
	private bool verboseMode;

	// Token: 0x04001C55 RID: 7253
	[Tooltip("Activate FSMs immediately or over multiple frames.")]
	[HideInInspector]
	public bool staggeredActivation = true;

	// Token: 0x04001C56 RID: 7254
	private BoxCollider2D activeRegion;
}
