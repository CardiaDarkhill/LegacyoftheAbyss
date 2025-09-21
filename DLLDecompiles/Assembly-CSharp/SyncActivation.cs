using System;
using UnityEngine;

// Token: 0x020005CC RID: 1484
public class SyncActivation : MonoBehaviour
{
	// Token: 0x060034DC RID: 13532 RVA: 0x000EAA71 File Offset: 0x000E8C71
	private void OnEnable()
	{
		this.target.SetActive(!this.inverted);
	}

	// Token: 0x060034DD RID: 13533 RVA: 0x000EAA87 File Offset: 0x000E8C87
	private void OnDisable()
	{
		this.target.SetActive(this.inverted);
	}

	// Token: 0x04003850 RID: 14416
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject target;

	// Token: 0x04003851 RID: 14417
	[SerializeField]
	private bool inverted;
}
