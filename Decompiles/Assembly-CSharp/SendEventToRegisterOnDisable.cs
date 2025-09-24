using System;
using UnityEngine;

// Token: 0x020005B8 RID: 1464
public class SendEventToRegisterOnDisable : MonoBehaviour
{
	// Token: 0x06003480 RID: 13440 RVA: 0x000E95FE File Offset: 0x000E77FE
	private void OnDisable()
	{
		if (string.IsNullOrEmpty(this.sendEvent))
		{
			return;
		}
		EventRegister.SendEvent(this.sendEvent, null);
	}

	// Token: 0x040037F5 RID: 14325
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private string sendEvent;
}
