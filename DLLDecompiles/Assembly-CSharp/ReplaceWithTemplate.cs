using System;
using UnityEngine;

// Token: 0x0200053A RID: 1338
public class ReplaceWithTemplate : MonoBehaviour
{
	// Token: 0x0600300B RID: 12299 RVA: 0x000D3C14 File Offset: 0x000D1E14
	public void Awake()
	{
		if (!this.template)
		{
			return;
		}
		Transform transform = base.transform;
		Transform parent = transform.parent;
		Vector3 localPosition = transform.localPosition;
		Vector3 localScale = transform.localScale;
		Quaternion localRotation = transform.localRotation;
		Transform transform2 = Object.Instantiate<GameObject>(this.template, parent).transform;
		transform2.localPosition = localPosition;
		transform2.localScale = localScale;
		transform2.localRotation = localRotation;
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x040032ED RID: 13037
	[SerializeField]
	private GameObject template;
}
