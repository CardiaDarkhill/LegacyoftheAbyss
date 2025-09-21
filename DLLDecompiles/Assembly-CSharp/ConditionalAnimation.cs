using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000076 RID: 118
public abstract class ConditionalAnimation : MonoBehaviour
{
	// Token: 0x06000362 RID: 866
	public abstract bool CanPlayAnimation();

	// Token: 0x06000363 RID: 867
	public abstract void PlayAnimation();

	// Token: 0x06000364 RID: 868
	public abstract IEnumerator PlayAndWait();
}
