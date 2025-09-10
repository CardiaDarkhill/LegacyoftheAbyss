using System;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public sealed class ResetAnimatorOnEnable : MonoBehaviour
{
	// Token: 0x06000512 RID: 1298 RVA: 0x0001A5E9 File Offset: 0x000187E9
	private void Awake()
	{
		if (this.animator == null)
		{
			this.animator = base.GetComponent<Animator>();
			if (this.animator == null)
			{
				base.enabled = false;
			}
		}
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x0001A61A File Offset: 0x0001881A
	private void OnValidate()
	{
		if (!this.animator)
		{
			this.animator = base.GetComponent<Animator>();
		}
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0001A638 File Offset: 0x00018838
	private void OnEnable()
	{
		if (this.animator != null && !string.IsNullOrEmpty(this.animationStateName))
		{
			this.animator.Play(this.animationStateName, -1, 0f);
			this.animator.Update(0f);
		}
	}

	// Token: 0x040004F0 RID: 1264
	[SerializeField]
	private string animationStateName = "YourStartState";

	// Token: 0x040004F1 RID: 1265
	[SerializeField]
	private Animator animator;
}
