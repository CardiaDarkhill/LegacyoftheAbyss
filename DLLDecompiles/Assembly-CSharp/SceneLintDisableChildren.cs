using System;
using UnityEngine;

// Token: 0x02000367 RID: 871
public class SceneLintDisableChildren : MonoBehaviour, ISceneLintUpgrader
{
	// Token: 0x06001E02 RID: 7682 RVA: 0x0008AAD4 File Offset: 0x00088CD4
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		bool flag = false;
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.activeSelf)
			{
				transform.gameObject.SetActive(false);
				flag = true;
			}
		}
		if (!flag)
		{
			return null;
		}
		return "Deactivated child GameObjects so they start disabled";
	}
}
