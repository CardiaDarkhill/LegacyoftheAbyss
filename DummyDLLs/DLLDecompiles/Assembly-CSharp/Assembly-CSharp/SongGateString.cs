using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200027E RID: 638
public class SongGateString : MonoBehaviour
{
	// Token: 0x06001694 RID: 5780 RVA: 0x000659CE File Offset: 0x00063BCE
	private void Start()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.distance = Mathf.Abs(base.transform.position.x - this.doorMark.position.x);
	}

	// Token: 0x06001695 RID: 5781 RVA: 0x00065A08 File Offset: 0x00063C08
	public void StrumStart()
	{
		float delay = 2f - this.distance * 0.35f;
		this.StartTimerRoutine(delay, "Strum", 0f);
	}

	// Token: 0x06001696 RID: 5782 RVA: 0x00065A3C File Offset: 0x00063C3C
	public void StrumEnd()
	{
		float delay = Random.Range(0.1f, 0.25f);
		this.StartTimerRoutine(delay, "Idle", 0f);
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x00065A6A File Offset: 0x00063C6A
	private void StartTimerRoutine(float delay, string anim, float stopDelay)
	{
		if (this.timerRoutine != null)
		{
			base.StopCoroutine(this.timerRoutine);
		}
		this.timerRoutine = base.StartCoroutine(this.PlayAnimDelayed(delay, anim, stopDelay));
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x00065A95 File Offset: 0x00063C95
	private IEnumerator PlayAnimDelayed(float delay, string anim, float stopDelay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		this.animator.Play(anim);
		if (stopDelay <= 0f)
		{
			yield break;
		}
		yield return new WaitForSeconds(stopDelay);
		this.animator.Play("Idle");
		yield break;
	}

	// Token: 0x06001699 RID: 5785 RVA: 0x00065AB9 File Offset: 0x00063CB9
	public void QuickStrum()
	{
		this.StartTimerRoutine(0f, "Strum", 0.5f + Random.Range(0f, 0.15f));
	}

	// Token: 0x04001515 RID: 5397
	[SerializeField]
	private Transform doorMark;

	// Token: 0x04001516 RID: 5398
	private tk2dSpriteAnimator animator;

	// Token: 0x04001517 RID: 5399
	private float distance;

	// Token: 0x04001518 RID: 5400
	private Coroutine timerRoutine;
}
