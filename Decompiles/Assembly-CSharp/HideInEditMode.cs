using System;
using UnityEngine;

// Token: 0x0200076D RID: 1901
[ExecuteInEditMode]
public class HideInEditMode : MonoBehaviour, ISceneLintUpgrader
{
	// Token: 0x060043DD RID: 17373 RVA: 0x0012A0A1 File Offset: 0x001282A1
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		if (!base.gameObject.activeSelf)
		{
			return "HideInEditMode object is disabled - is this intended? (" + base.gameObject.name + ")";
		}
		return null;
	}

	// Token: 0x0400452E RID: 17710
	[SerializeField]
	private bool isVisible;

	// Token: 0x0400452F RID: 17711
	private bool isInCameraRender;

	// Token: 0x04004530 RID: 17712
	private bool subscribed;
}
