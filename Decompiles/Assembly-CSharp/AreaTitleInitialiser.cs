using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000609 RID: 1545
public class AreaTitleInitialiser : MonoBehaviour
{
	// Token: 0x0600372D RID: 14125 RVA: 0x000F3840 File Offset: 0x000F1A40
	private IEnumerator Start()
	{
		if (this.areaTitle == null)
		{
			yield break;
		}
		GameObject areaTitleGameObject = this.areaTitle.gameObject;
		while (!this.areaTitle.Initialised)
		{
			if (!areaTitleGameObject.activeSelf)
			{
				areaTitleGameObject.SetActive(true);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04003A02 RID: 14850
	[SerializeField]
	private AreaTitle areaTitle;
}
