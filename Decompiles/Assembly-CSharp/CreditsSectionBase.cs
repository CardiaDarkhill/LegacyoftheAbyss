using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000079 RID: 121
public abstract class CreditsSectionBase : MonoBehaviour
{
	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600036C RID: 876 RVA: 0x00011DC3 File Offset: 0x0000FFC3
	public float FadeUpDuration
	{
		get
		{
			return this.fadeUpDuration;
		}
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x0600036D RID: 877 RVA: 0x00011DCB File Offset: 0x0000FFCB
	public float FadeDownDuration
	{
		get
		{
			return this.fadeDownDuration;
		}
	}

	// Token: 0x0600036E RID: 878 RVA: 0x00011DD3 File Offset: 0x0000FFD3
	public Coroutine Show()
	{
		return base.StartCoroutine(this.ShowRoutine());
	}

	// Token: 0x0600036F RID: 879
	protected abstract IEnumerator ShowRoutine();

	// Token: 0x04000318 RID: 792
	[SerializeField]
	private float fadeUpDuration;

	// Token: 0x04000319 RID: 793
	[SerializeField]
	private float fadeDownDuration;
}
