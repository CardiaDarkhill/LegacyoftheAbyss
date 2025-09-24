using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001F8 RID: 504
public class StatusRecordComparisonResponse : MonoBehaviour
{
	// Token: 0x1700022A RID: 554
	// (get) Token: 0x0600134A RID: 4938 RVA: 0x000585FE File Offset: 0x000567FE
	// (set) Token: 0x0600134B RID: 4939 RVA: 0x00058606 File Offset: 0x00056806
	public string Key
	{
		get
		{
			return this.key;
		}
		set
		{
			this.key = value;
			this.OnEnable();
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x0600134C RID: 4940 RVA: 0x00058615 File Offset: 0x00056815
	// (set) Token: 0x0600134D RID: 4941 RVA: 0x0005861D File Offset: 0x0005681D
	public int CompareTo
	{
		get
		{
			return this.compareTo;
		}
		set
		{
			this.compareTo = value;
			this.OnEnable();
		}
	}

	// Token: 0x0600134E RID: 4942 RVA: 0x0005862C File Offset: 0x0005682C
	private void Start()
	{
		this.DoResponse();
		this.doOnEnable = true;
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x0005863B File Offset: 0x0005683B
	private void OnEnable()
	{
		if (!this.doOnEnable)
		{
			return;
		}
		this.DoResponse();
	}

	// Token: 0x06001350 RID: 4944 RVA: 0x0005864C File Offset: 0x0005684C
	private void DoResponse()
	{
		if (string.IsNullOrEmpty(this.key))
		{
			return;
		}
		if (GameManager.instance.GetStatusRecordInt(this.key) == this.compareTo)
		{
			this.OnIsEqual.Invoke();
			return;
		}
		this.OnIsNotEqual.Invoke();
	}

	// Token: 0x040011BA RID: 4538
	[SerializeField]
	private string key;

	// Token: 0x040011BB RID: 4539
	[SerializeField]
	private int compareTo;

	// Token: 0x040011BC RID: 4540
	[Space]
	public UnityEvent OnIsEqual;

	// Token: 0x040011BD RID: 4541
	public UnityEvent OnIsNotEqual;

	// Token: 0x040011BE RID: 4542
	private bool doOnEnable;
}
