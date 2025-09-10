using System;
using UnityEngine;

// Token: 0x02000357 RID: 855
public class InstantiateOnAwake : MonoBehaviour
{
	// Token: 0x06001D96 RID: 7574 RVA: 0x0008888C File Offset: 0x00086A8C
	private void Awake()
	{
		this.template.SetActive(false);
		for (int i = 0; i < this.count; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.template, this.template.transform.parent);
			gameObject.transform.localPosition = this.template.transform.localPosition + this.itemOffset * (float)i;
			if (!string.IsNullOrEmpty(this.fsmCounterName))
			{
				gameObject.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt(this.fsmCounterName).Value = this.fsmCounterOffset + i;
			}
			gameObject.SetActive(true);
		}
	}

	// Token: 0x04001CCA RID: 7370
	[SerializeField]
	private GameObject template;

	// Token: 0x04001CCB RID: 7371
	[SerializeField]
	private int count;

	// Token: 0x04001CCC RID: 7372
	[SerializeField]
	private Vector2 itemOffset;

	// Token: 0x04001CCD RID: 7373
	[SerializeField]
	private string fsmCounterName;

	// Token: 0x04001CCE RID: 7374
	[SerializeField]
	private int fsmCounterOffset;
}
