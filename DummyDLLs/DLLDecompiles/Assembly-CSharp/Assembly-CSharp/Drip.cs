using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004D3 RID: 1235
public class Drip : MonoBehaviour
{
	// Token: 0x06002C6C RID: 11372 RVA: 0x000C27D7 File Offset: 0x000C09D7
	private void Awake()
	{
		this.dripAnimator = this.dripSprite.GetComponent<Animator>();
	}

	// Token: 0x06002C6D RID: 11373 RVA: 0x000C27EA File Offset: 0x000C09EA
	private void Start()
	{
		base.StartCoroutine(this.DripRoutine());
	}

	// Token: 0x06002C6E RID: 11374 RVA: 0x000C27F9 File Offset: 0x000C09F9
	private IEnumerator DripRoutine()
	{
		for (;;)
		{
			float seconds = Random.Range(this.minWaitTime, this.maxWaitTime);
			yield return new WaitForSeconds(seconds);
			this.idleSprite.SetActive(false);
			this.dripSprite.SetActive(true);
			base.StartCoroutine(this.DropDrip());
			yield return new WaitForSeconds(this.dripAnimator.GetCurrentAnimatorStateInfo(0).length);
			this.idleSprite.SetActive(true);
			this.dripSprite.SetActive(false);
		}
		yield break;
	}

	// Token: 0x06002C6F RID: 11375 RVA: 0x000C2808 File Offset: 0x000C0A08
	private IEnumerator DropDrip()
	{
		yield return new WaitForSeconds(this.dripDelay);
		this.dripPrefab.Spawn(this.dripSpawnPoint.position).transform.SetPositionZ(0.003f);
		yield break;
	}

	// Token: 0x04002E05 RID: 11781
	public float minWaitTime = 1f;

	// Token: 0x04002E06 RID: 11782
	public float maxWaitTime = 7f;

	// Token: 0x04002E07 RID: 11783
	public GameObject idleSprite;

	// Token: 0x04002E08 RID: 11784
	public GameObject dripSprite;

	// Token: 0x04002E09 RID: 11785
	private Animator dripAnimator;

	// Token: 0x04002E0A RID: 11786
	public Transform dripSpawnPoint;

	// Token: 0x04002E0B RID: 11787
	public float dripDelay = 0.6f;

	// Token: 0x04002E0C RID: 11788
	public GameObject dripPrefab;
}
