using System;
using UnityEngine;

// Token: 0x020005B7 RID: 1463
public class SendEventToRegisterMethod : MonoBehaviour
{
	// Token: 0x0600347E RID: 13438 RVA: 0x000E95E8 File Offset: 0x000E77E8
	public void DoSend()
	{
		EventRegister.SendEvent(this.sendEvent, null);
	}

	// Token: 0x040037F4 RID: 14324
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private string sendEvent;
}
