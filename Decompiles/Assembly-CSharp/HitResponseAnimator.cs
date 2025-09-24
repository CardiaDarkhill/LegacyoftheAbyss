using System;
using UnityEngine;

// Token: 0x02000501 RID: 1281
public class HitResponseAnimator : MonoBehaviour
{
	// Token: 0x06002DD9 RID: 11737 RVA: 0x000C8C75 File Offset: 0x000C6E75
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<string>(ref this.hitAnimations, typeof(HitInstance.HitDirection));
	}

	// Token: 0x06002DDA RID: 11738 RVA: 0x000C8C8C File Offset: 0x000C6E8C
	private void Awake()
	{
		this.OnValidate();
		this.hitResponse.WasHitDirectional += this.OnWasHitDirectional;
	}

	// Token: 0x06002DDB RID: 11739 RVA: 0x000C8CAC File Offset: 0x000C6EAC
	private void OnWasHitDirectional(HitInstance.HitDirection hitDirection)
	{
		string text = this.hitAnimations[(int)hitDirection];
		if (string.IsNullOrEmpty(text) && (hitDirection == HitInstance.HitDirection.Up || hitDirection == HitInstance.HitDirection.Down))
		{
			ref Vector3 position = HeroController.instance.transform.position;
			Vector3 position2 = base.transform.position;
			text = ((position.x < position2.x) ? this.hitAnimations[1] : this.hitAnimations[0]);
		}
		this.animator.Play(text, this.playOnLayer, 0f);
	}

	// Token: 0x04002FE1 RID: 12257
	[SerializeField]
	private HitResponse hitResponse;

	// Token: 0x04002FE2 RID: 12258
	[SerializeField]
	private Animator animator;

	// Token: 0x04002FE3 RID: 12259
	[SerializeField]
	[ArrayForEnum(typeof(HitInstance.HitDirection))]
	private string[] hitAnimations;

	// Token: 0x04002FE4 RID: 12260
	[SerializeField]
	private int playOnLayer;
}
