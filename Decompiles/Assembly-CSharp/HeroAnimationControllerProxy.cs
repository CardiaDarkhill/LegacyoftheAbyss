using System;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class HeroAnimationControllerProxy : MonoBehaviour, IHeroAnimationController
{
	// Token: 0x06000424 RID: 1060 RVA: 0x0001631E File Offset: 0x0001451E
	public tk2dSpriteAnimationClip GetClip(string clipName)
	{
		if (HeroController.instance == null)
		{
			return null;
		}
		return HeroController.instance.GetComponent<HeroAnimationController>().GetClip(clipName);
	}
}
