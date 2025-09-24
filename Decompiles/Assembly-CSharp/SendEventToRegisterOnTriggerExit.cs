using System;
using UnityEngine;

// Token: 0x020005BB RID: 1467
public class SendEventToRegisterOnTriggerExit : MonoBehaviour
{
	// Token: 0x06003489 RID: 13449 RVA: 0x000E9715 File Offset: 0x000E7915
	private void OnTriggerExit2D(Collider2D collision)
	{
		EventRegister.SendEvent(this.sendEvent, null);
	}

	// Token: 0x040037FA RID: 14330
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private string sendEvent;
}
