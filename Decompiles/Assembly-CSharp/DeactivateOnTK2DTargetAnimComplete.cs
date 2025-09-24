using System;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class DeactivateOnTK2DTargetAnimComplete : MonoBehaviour
{
	// Token: 0x0600038E RID: 910 RVA: 0x0001254B File Offset: 0x0001074B
	private void OnEnable()
	{
		this.startedPlayingTargetClip = false;
	}

	// Token: 0x0600038F RID: 911 RVA: 0x00012554 File Offset: 0x00010754
	private void Update()
	{
		if (!this.startedPlayingTargetClip)
		{
			if (this.animator.IsPlaying(this.targetClipName))
			{
				this.startedPlayingTargetClip = true;
				return;
			}
		}
		else if (!this.animator.IsPlaying(this.targetClipName))
		{
			this.disableGameObject.SetActive(false);
		}
	}

	// Token: 0x0400033A RID: 826
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x0400033B RID: 827
	[SerializeField]
	private GameObject disableGameObject;

	// Token: 0x0400033C RID: 828
	[SerializeField]
	private string targetClipName;

	// Token: 0x0400033D RID: 829
	private bool startedPlayingTargetClip;
}
