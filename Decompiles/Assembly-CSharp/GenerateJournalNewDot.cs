using System;
using UnityEngine;

// Token: 0x02000375 RID: 885
public class GenerateJournalNewDot : MonoBehaviour
{
	// Token: 0x06001E4F RID: 7759 RVA: 0x0008BAE8 File Offset: 0x00089CE8
	private void Start()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.newDotObject);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = new Vector3(-0.65f, 0f, -0.0001f);
		gameObject.SetActive(false);
	}

	// Token: 0x04001D56 RID: 7510
	public GameObject newDotObject;
}
