using System;
using TMProOld;
using UnityEngine;

// Token: 0x02000058 RID: 88
public sealed class AddAnimatorToSubMesh : MonoBehaviour
{
	// Token: 0x06000246 RID: 582 RVA: 0x0000DFC8 File Offset: 0x0000C1C8
	private void Start()
	{
		TMP_SubMesh[] componentsInChildren = base.GetComponentsInChildren<TMP_SubMesh>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.AddComponentIfNotPresent<Animator>().runtimeAnimatorController = this.controller;
		}
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0000E004 File Offset: 0x0000C204
	private void OnTransformChildrenChanged()
	{
		TMP_SubMesh[] componentsInChildren = base.GetComponentsInChildren<TMP_SubMesh>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.AddComponentIfNotPresent<Animator>().runtimeAnimatorController = this.controller;
		}
	}

	// Token: 0x040001F2 RID: 498
	[SerializeField]
	private RuntimeAnimatorController controller;
}
