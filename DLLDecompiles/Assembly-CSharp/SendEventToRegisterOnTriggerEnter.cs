using System;
using UnityEngine;

// Token: 0x020005BA RID: 1466
public class SendEventToRegisterOnTriggerEnter : MonoBehaviour
{
	// Token: 0x06003487 RID: 13447 RVA: 0x000E96C4 File Offset: 0x000E78C4
	private void OnTriggerEnter2D(Collider2D collision)
	{
		int layer = collision.gameObject.layer;
		int num = 1 << layer;
		if ((this.allowLayers.value & num) != num)
		{
			return;
		}
		EventRegister.SendEvent(this.sendEvent, null);
	}

	// Token: 0x040037F8 RID: 14328
	[SerializeField]
	private LayerMask allowLayers = -1;

	// Token: 0x040037F9 RID: 14329
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private string sendEvent;
}
