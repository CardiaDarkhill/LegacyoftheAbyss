using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000060 RID: 96
public class AnimationRunner : MonoBehaviour
{
	// Token: 0x0600026F RID: 623 RVA: 0x0000E7E2 File Offset: 0x0000C9E2
	private void OnEnable()
	{
		this.PlayAnim(this.idleState, false);
	}

	// Token: 0x06000270 RID: 624 RVA: 0x0000E7F1 File Offset: 0x0000C9F1
	public void Play()
	{
		this.PlayAnim(this.playState, true);
	}

	// Token: 0x06000271 RID: 625 RVA: 0x0000E800 File Offset: 0x0000CA00
	private void PlayAnim(string stateName, bool invokeEvent)
	{
		if (this.animator && base.gameObject.activeInHierarchy)
		{
			if (this.invokeRoutine != null)
			{
				base.StopCoroutine(this.invokeRoutine);
				this.invokeRoutine = null;
				this.animationFinished.Invoke();
			}
			this.animator.Play(stateName);
			if (invokeEvent)
			{
				this.invokeRoutine = base.StartCoroutine(this.InvokeAnimEndEvent());
			}
		}
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0000E86E File Offset: 0x0000CA6E
	private IEnumerator InvokeAnimEndEvent()
	{
		AnimatorStateInfo beforeState = this.animator.GetCurrentAnimatorStateInfo(0);
		yield return null;
		AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		if (beforeState.GetHashCode() != currentAnimatorStateInfo.GetHashCode())
		{
			yield return new WaitForSeconds(currentAnimatorStateInfo.length + this.endDelay);
			this.animationFinished.Invoke();
		}
		this.invokeRoutine = null;
		yield break;
	}

	// Token: 0x04000218 RID: 536
	[SerializeField]
	private Animator animator;

	// Token: 0x04000219 RID: 537
	[SerializeField]
	private string idleState = "Idle";

	// Token: 0x0400021A RID: 538
	[SerializeField]
	private string playState;

	// Token: 0x0400021B RID: 539
	[SerializeField]
	private float endDelay;

	// Token: 0x0400021C RID: 540
	[SerializeField]
	[Space]
	private UnityEvent animationFinished;

	// Token: 0x0400021D RID: 541
	private Coroutine invokeRoutine;
}
