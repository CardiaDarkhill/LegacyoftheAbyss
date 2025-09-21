using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class Tk2dPlayRandomAnimationOnEnable : MonoBehaviour
{
	// Token: 0x06000661 RID: 1633 RVA: 0x00020B4D File Offset: 0x0001ED4D
	private void Reset()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x00020B5B File Offset: 0x0001ED5B
	private void OnEnable()
	{
		this.Play();
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00020B64 File Offset: 0x0001ED64
	public void Play()
	{
		string name = this.clipNames[Random.Range(0, this.clipNames.Length)];
		this.animator.Play(name);
		this.animator.PlayFromFrame(0);
	}

	// Token: 0x0400063B RID: 1595
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x0400063C RID: 1596
	[SerializeField]
	private string[] clipNames;
}
