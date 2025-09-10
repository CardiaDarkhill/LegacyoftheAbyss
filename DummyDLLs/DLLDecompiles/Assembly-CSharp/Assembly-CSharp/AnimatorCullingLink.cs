using System;
using UnityEngine;

// Token: 0x02000062 RID: 98
[RequireComponent(typeof(Animator))]
public class AnimatorCullingLink : MonoBehaviour
{
	// Token: 0x0600027A RID: 634 RVA: 0x0000E914 File Offset: 0x0000CB14
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
	}

	// Token: 0x0600027B RID: 635 RVA: 0x0000E930 File Offset: 0x0000CB30
	private void Update()
	{
		bool flag = false;
		Renderer[] array = this.targets;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isVisible)
			{
				flag = true;
			}
		}
		bool flag2 = flag;
		bool? flag3 = this.wasVisible;
		if (!(flag2 == flag3.GetValueOrDefault() & flag3 != null))
		{
			this.wasVisible = new bool?(flag);
			this.animator.enabled = flag;
		}
	}

	// Token: 0x04000221 RID: 545
	[SerializeField]
	private Renderer[] targets;

	// Token: 0x04000222 RID: 546
	private bool? wasVisible;

	// Token: 0x04000223 RID: 547
	private Animator animator;
}
