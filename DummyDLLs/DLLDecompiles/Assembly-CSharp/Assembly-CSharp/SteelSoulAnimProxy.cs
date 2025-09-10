using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x020000BF RID: 191
[RequireComponent(typeof(tk2dSpriteAnimator))]
public class SteelSoulAnimProxy : MonoBehaviour, IHeroAnimationController
{
	// Token: 0x06000611 RID: 1553 RVA: 0x0001F18C File Offset: 0x0001D38C
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x0001F19C File Offset: 0x0001D39C
	public tk2dSpriteAnimationClip GetClip(string clipName)
	{
		if (PlayerData.instance.permadeathMode == PermadeathModes.On && this.steelSoulAnims)
		{
			tk2dSpriteAnimationClip clipByName = this.steelSoulAnims.GetClipByName(clipName);
			if (clipByName != null)
			{
				return clipByName;
			}
		}
		tk2dSpriteAnimationClip clipByName2 = this.animator.Library.GetClipByName(clipName);
		if (clipByName2 != null)
		{
			return clipByName2;
		}
		return null;
	}

	// Token: 0x040005DE RID: 1502
	[SerializeField]
	private tk2dSpriteAnimation steelSoulAnims;

	// Token: 0x040005DF RID: 1503
	private tk2dSpriteAnimator animator;
}
