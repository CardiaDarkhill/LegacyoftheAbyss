using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200009E RID: 158
[RequireComponent(typeof(Animator))]
public class PlayFromRandomFrameMecanim : MonoBehaviour
{
	// Token: 0x060004E7 RID: 1255 RVA: 0x00019AC8 File Offset: 0x00017CC8
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x00019AD6 File Offset: 0x00017CD6
	private void Start()
	{
		if (!this.onEnable)
		{
			this.DoPlay();
		}
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x00019AE6 File Offset: 0x00017CE6
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.DoPlay();
		}
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x00019AF6 File Offset: 0x00017CF6
	private void DoPlay()
	{
		base.StartCoroutine(this.DelayStart());
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00019B05 File Offset: 0x00017D05
	private IEnumerator DelayStart()
	{
		yield return null;
		if (string.IsNullOrEmpty(this.stateToPlay))
		{
			int shortNameHash = this.animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
			this.animator.Play(shortNameHash, this.layerToPlay, Random.Range(0f, 1f));
		}
		else
		{
			this.animator.Play(this.stateToPlay, this.layerToPlay, Random.Range(0f, 1f));
		}
		yield break;
	}

	// Token: 0x040004BC RID: 1212
	public string stateToPlay;

	// Token: 0x040004BD RID: 1213
	public int layerToPlay;

	// Token: 0x040004BE RID: 1214
	public bool onEnable;

	// Token: 0x040004BF RID: 1215
	private Animator animator;
}
