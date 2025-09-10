using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000497 RID: 1175
public class BlackThreadImpacter : MonoBehaviour
{
	// Token: 0x06002A74 RID: 10868 RVA: 0x000B7FC0 File Offset: 0x000B61C0
	private void Awake()
	{
		this.silk_strand_impact = base.transform.Find("silk_strand_impact").gameObject;
		this.black_thread_strand = base.transform.Find("black_thread_strand").gameObject;
		this.appearRange = base.transform.Find("Appear Range").gameObject;
		this.persistent = base.GetComponent<PersistentBoolItem>();
		if (this.persistent != null)
		{
			this.persistent.OnGetSaveState += delegate(out bool val)
			{
				val = this.activated;
			};
			this.persistent.OnSetSaveState += delegate(bool val)
			{
				this.activated = val;
				if (this.activated)
				{
					this.SetAlreadyActivated();
				}
			};
		}
		if (this.activated)
		{
			this.silk_strand_impact.SetActive(false);
			this.black_thread_strand.SetActive(false);
			return;
		}
		this.silk_strand_impact.SetActive(false);
		this.black_thread_strand.SetActive(true);
		this.appearRange.SetActive(false);
	}

	// Token: 0x06002A75 RID: 10869 RVA: 0x000B80AB File Offset: 0x000B62AB
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.activated)
		{
			this.appearRange.SetActive(false);
			base.StartCoroutine(this.DoAppear());
			this.activated = true;
		}
	}

	// Token: 0x06002A76 RID: 10870 RVA: 0x000B80D5 File Offset: 0x000B62D5
	public IEnumerator DoAppear()
	{
		yield return new WaitForSeconds(Random.Range(0.1f, 0.6f));
		this.silk_strand_impact.SetActive(true);
		yield return new WaitForSeconds(0.59f);
		this.silk_strand_impact.SetActive(false);
		this.black_thread_strand.SetActive(true);
		yield break;
	}

	// Token: 0x06002A77 RID: 10871 RVA: 0x000B80E4 File Offset: 0x000B62E4
	private void SetAlreadyActivated()
	{
	}

	// Token: 0x04002B14 RID: 11028
	private bool activated;

	// Token: 0x04002B15 RID: 11029
	private GameObject silk_strand_impact;

	// Token: 0x04002B16 RID: 11030
	private GameObject black_thread_strand;

	// Token: 0x04002B17 RID: 11031
	private GameObject appearRange;

	// Token: 0x04002B18 RID: 11032
	private PersistentBoolItem persistent;
}
