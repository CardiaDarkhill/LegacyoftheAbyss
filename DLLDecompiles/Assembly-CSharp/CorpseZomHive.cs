using System;
using UnityEngine;

// Token: 0x020002B8 RID: 696
public class CorpseZomHive : CorpseChunker
{
	// Token: 0x060018AA RID: 6314 RVA: 0x000710FC File Offset: 0x0006F2FC
	protected override void LandEffects()
	{
		base.LandEffects();
		GameObject gameObject = GameObject.FindWithTag("Extra Tag");
		if (gameObject)
		{
			for (int i = 0; i < 3; i++)
			{
				int index = Random.Range(0, gameObject.transform.childCount);
				Transform child = gameObject.transform.GetChild(index);
				if (child)
				{
					child.SetParent(null);
					child.position = base.transform.position;
					FSMUtility.SendEventToGameObject(child.gameObject, "SPAWN", false);
					FlingUtils.FlingObject(new FlingUtils.SelfConfig
					{
						Object = child.gameObject,
						SpeedMin = 5f,
						SpeedMax = 10f,
						AngleMin = 0f,
						AngleMax = 180f
					}, base.transform, Vector3.zero);
				}
			}
		}
	}
}
