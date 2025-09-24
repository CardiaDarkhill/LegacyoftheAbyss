using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000061 RID: 97
[RequireComponent(typeof(Animator))]
public class AnimatorControlSequence : MonoBehaviour
{
	// Token: 0x06000274 RID: 628 RVA: 0x0000E890 File Offset: 0x0000CA90
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0000E89E File Offset: 0x0000CA9E
	private IEnumerator Start()
	{
		if (this.alreadyPlayed)
		{
			yield break;
		}
		this.animator.enabled = true;
		this.animator.Play(this.stateName, 0, 0f);
		yield return null;
		this.animator.enabled = false;
		yield break;
	}

	// Token: 0x06000276 RID: 630 RVA: 0x0000E8AD File Offset: 0x0000CAAD
	[ContextMenu("Play From Start", true)]
	[ContextMenu("Play From End", true)]
	private bool CanPlay()
	{
		return Application.isPlaying;
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0000E8B4 File Offset: 0x0000CAB4
	[ContextMenu("Play From Start")]
	public void PlayAnimatorFromStart()
	{
		this.animator.enabled = true;
		this.animator.Play(this.stateName, 0, 0f);
		this.alreadyPlayed = true;
	}

	// Token: 0x06000278 RID: 632 RVA: 0x0000E8E0 File Offset: 0x0000CAE0
	[ContextMenu("Play From End")]
	public void PlayAnimatorFromEnd()
	{
		this.animator.enabled = true;
		this.animator.Play(this.stateName, 0, 1f);
		this.alreadyPlayed = true;
	}

	// Token: 0x0400021E RID: 542
	[SerializeField]
	private string stateName;

	// Token: 0x0400021F RID: 543
	private Animator animator;

	// Token: 0x04000220 RID: 544
	private bool alreadyPlayed;
}
