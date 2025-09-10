using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007C7 RID: 1991
public class PersistentIntItemResponder : MonoBehaviour
{
	// Token: 0x0600461B RID: 17947 RVA: 0x00130AC6 File Offset: 0x0012ECC6
	private void Awake()
	{
		if (!this.persistent)
		{
			return;
		}
		this.persistent.OnSetSaveState += delegate(int value)
		{
			if (value.Test(this.test, this.testValue))
			{
				this.OnSuccess.Invoke();
				return;
			}
			this.OnFailure.Invoke();
		};
	}

	// Token: 0x040046A6 RID: 18086
	[SerializeField]
	private PersistentIntItem persistent;

	// Token: 0x040046A7 RID: 18087
	[SerializeField]
	private Extensions.IntTest test;

	// Token: 0x040046A8 RID: 18088
	[SerializeField]
	private int testValue;

	// Token: 0x040046A9 RID: 18089
	[Space]
	public UnityEvent OnSuccess;

	// Token: 0x040046AA RID: 18090
	public UnityEvent OnFailure;
}
