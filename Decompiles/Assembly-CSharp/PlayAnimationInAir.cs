using System;
using UnityEngine;

// Token: 0x0200009C RID: 156
public class PlayAnimationInAir : MonoBehaviour
{
	// Token: 0x060004DF RID: 1247 RVA: 0x00019992 File Offset: 0x00017B92
	private void OnEnable()
	{
		if (this.animator)
		{
			this.animator.Play(this.airAnim);
		}
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x000199B2 File Offset: 0x00017BB2
	private void OnCollisionEnter2D(Collision2D collision)
	{
		this.currentCollisions++;
		if (this.currentCollisions == 1 && this.animator)
		{
			this.animator.Play(this.groundAnim);
		}
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x000199E9 File Offset: 0x00017BE9
	private void OnCollisionExit2D(Collision2D collision)
	{
		this.currentCollisions--;
		if (this.currentCollisions == 0 && this.animator)
		{
			this.animator.Play(this.airAnim);
		}
	}

	// Token: 0x040004B4 RID: 1204
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x040004B5 RID: 1205
	[SerializeField]
	private string airAnim;

	// Token: 0x040004B6 RID: 1206
	[SerializeField]
	private string groundAnim;

	// Token: 0x040004B7 RID: 1207
	private int currentCollisions;
}
