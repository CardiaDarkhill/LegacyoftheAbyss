using System;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class GeneratePortraitFrame : MonoBehaviour
{
	// Token: 0x0600006F RID: 111 RVA: 0x000040E0 File Offset: 0x000022E0
	private void Start()
	{
		GameObject gameObject = base.transform.parent.gameObject;
		GameObject gameObject2 = Object.Instantiate<GameObject>(this.frameObject);
		gameObject2.transform.parent = gameObject.transform;
		gameObject2.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z - 0.0001f);
		gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject2.SetActive(false);
	}

	// Token: 0x0400005A RID: 90
	public GameObject frameObject;
}
