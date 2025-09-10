using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class AnimatorPlayDelaySequence : MonoBehaviour
{
	// Token: 0x0600028A RID: 650 RVA: 0x0000ECAE File Offset: 0x0000CEAE
	private void Start()
	{
		this.SetAtEnd();
	}

	// Token: 0x0600028B RID: 651 RVA: 0x0000ECB8 File Offset: 0x0000CEB8
	private void SetAtEnd()
	{
		if (string.IsNullOrEmpty(this.startAtEndAnim))
		{
			return;
		}
		AnimatorPlayDelaySequence.AnimatorGroup[] array = this.animatorGroups;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (Animator animator in array[i].Animators)
			{
				if (animator)
				{
					animator.Play(this.startAtEndAnim, 0, 1f);
				}
			}
		}
	}

	// Token: 0x0600028C RID: 652 RVA: 0x0000ED20 File Offset: 0x0000CF20
	public void Play(string animName)
	{
		if (this.playRoutine != null)
		{
			base.StopCoroutine(this.playRoutine);
		}
		this.playRoutine = base.StartCoroutine(this.Play(animName, false));
	}

	// Token: 0x0600028D RID: 653 RVA: 0x0000ED4A File Offset: 0x0000CF4A
	public void PlayReversed(string animName)
	{
		if (this.playRoutine != null)
		{
			base.StopCoroutine(this.playRoutine);
		}
		this.playRoutine = base.StartCoroutine(this.Play(animName, true));
	}

	// Token: 0x0600028E RID: 654 RVA: 0x0000ED74 File Offset: 0x0000CF74
	private IEnumerator Play(string animName, bool isReversed)
	{
		yield return new WaitForSeconds(isReversed ? this.startDelayReverse : this.startDelayForward);
		bool doneFirst = false;
		int i = isReversed ? (this.animatorGroups.Length - 1) : 0;
		while (isReversed ? (i >= 0) : (i < this.animatorGroups.Length))
		{
			if (doneFirst)
			{
				yield return new WaitForSeconds(this.delayBetweenGroups);
			}
			else
			{
				doneFirst = true;
			}
			foreach (Animator animator in this.animatorGroups[i].Animators)
			{
				if (animator)
				{
					animator.Play(animName);
				}
			}
			i += (isReversed ? -1 : 1);
		}
		yield break;
	}

	// Token: 0x0400022D RID: 557
	[SerializeField]
	private AnimatorPlayDelaySequence.AnimatorGroup[] animatorGroups;

	// Token: 0x0400022E RID: 558
	[SerializeField]
	private float startDelayForward;

	// Token: 0x0400022F RID: 559
	[SerializeField]
	private float startDelayReverse;

	// Token: 0x04000230 RID: 560
	[SerializeField]
	private float delayBetweenGroups;

	// Token: 0x04000231 RID: 561
	[Space]
	[SerializeField]
	private string startAtEndAnim;

	// Token: 0x04000232 RID: 562
	private Coroutine playRoutine;

	// Token: 0x020013DE RID: 5086
	[Serializable]
	private class AnimatorGroup
	{
		// Token: 0x040080F3 RID: 33011
		public Animator[] Animators;
	}
}
