using System;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000C9 RID: 201
public class tk2dAnimationTriggerResponder : MonoBehaviour
{
	// Token: 0x0600065B RID: 1627 RVA: 0x000209BC File Offset: 0x0001EBBC
	private bool? IsFsmEventValid(string eventName)
	{
		if (!this.fsmTarget)
		{
			return null;
		}
		return new bool?(this.fsmTarget.FsmEvents.Any((FsmEvent e) => e.Name.Equals(eventName)));
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x00020A0E File Offset: 0x0001EC0E
	private void OnEnable()
	{
		if (this.animator)
		{
			tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
			tk2dSpriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(tk2dSpriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered));
		}
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00020A44 File Offset: 0x0001EC44
	private void OnDisable()
	{
		if (this.animator)
		{
			tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
			tk2dSpriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(tk2dSpriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered));
		}
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00020A7C File Offset: 0x0001EC7C
	public void ReSubEvents()
	{
		if (this.animator)
		{
			tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
			tk2dSpriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(tk2dSpriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered));
			tk2dSpriteAnimator tk2dSpriteAnimator2 = this.animator;
			tk2dSpriteAnimator2.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(tk2dSpriteAnimator2.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered));
		}
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00020AE4 File Offset: 0x0001ECE4
	private void OnAnimationEventTriggered(tk2dSpriteAnimator _, tk2dSpriteAnimationClip clip, int frame)
	{
		if (!string.IsNullOrEmpty(this.eventInfo) && clip != null && !clip.frames[frame].eventInfo.Equals(this.eventInfo))
		{
			return;
		}
		if (this.fsmTarget)
		{
			this.fsmTarget.SendEvent(this.fsmEvent);
		}
		this.EventTriggered.Invoke();
	}

	// Token: 0x04000636 RID: 1590
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x04000637 RID: 1591
	[SerializeField]
	private string eventInfo;

	// Token: 0x04000638 RID: 1592
	[Space]
	[SerializeField]
	private PlayMakerFSM fsmTarget;

	// Token: 0x04000639 RID: 1593
	[ModifiableProperty]
	[Conditional("fsmTarget", true, false, false)]
	[InspectorValidation("IsFsmEventValid")]
	[SerializeField]
	private string fsmEvent;

	// Token: 0x0400063A RID: 1594
	[Space]
	public UnityEvent EventTriggered;
}
