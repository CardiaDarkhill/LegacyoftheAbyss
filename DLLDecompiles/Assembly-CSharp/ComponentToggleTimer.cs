using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004C0 RID: 1216
public class ComponentToggleTimer : MonoBehaviour
{
	// Token: 0x06002BE8 RID: 11240 RVA: 0x000C07A9 File Offset: 0x000BE9A9
	private void OnEnable()
	{
		this.component.enabled = this.initialState;
		base.StartCoroutine(this.Routine());
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x000C07C9 File Offset: 0x000BE9C9
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06002BEA RID: 11242 RVA: 0x000C07D1 File Offset: 0x000BE9D1
	private IEnumerator Routine()
	{
		WaitForSeconds wait = new WaitForSeconds(this.betweenDelay);
		WaitForFixedUpdate frameWait = this.isPhysics ? new WaitForFixedUpdate() : null;
		int countLeft = this.totalCount;
		while (countLeft > 0 || this.totalCount <= 0)
		{
			yield return wait;
			this.component.enabled = !this.initialState;
			yield return frameWait;
			this.component.enabled = this.initialState;
			int num = countLeft;
			countLeft = num - 1;
		}
		yield break;
	}

	// Token: 0x04002D49 RID: 11593
	[SerializeField]
	private Behaviour component;

	// Token: 0x04002D4A RID: 11594
	[SerializeField]
	private bool initialState;

	// Token: 0x04002D4B RID: 11595
	[SerializeField]
	private bool isPhysics;

	// Token: 0x04002D4C RID: 11596
	[SerializeField]
	private float betweenDelay;

	// Token: 0x04002D4D RID: 11597
	[SerializeField]
	private int totalCount;
}
