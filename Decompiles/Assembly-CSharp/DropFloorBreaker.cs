using System;
using System.Collections;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020004D5 RID: 1237
public class DropFloorBreaker : MonoBehaviour
{
	// Token: 0x06002C7C RID: 11388 RVA: 0x000C2A10 File Offset: 0x000C0C10
	private void OnDrawGizmosSelected()
	{
		if (this.breakThresholds == null)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		foreach (DropFloorBreaker.BreakThreshold breakThreshold in this.breakThresholds)
		{
			Gizmos.DrawWireSphere(new Vector3(0f, breakThreshold.YPos, 0f), 0.2f);
		}
	}

	// Token: 0x06002C7D RID: 11389 RVA: 0x000C2A74 File Offset: 0x000C0C74
	private void Awake()
	{
		if (this.dropBody)
		{
			this.dropBody.isKinematic = true;
			this.dropBody.linearVelocity = Vector2.zero;
			this.dropBody.angularVelocity = 0f;
		}
		if (this.activeInIdle)
		{
			this.activeInIdle.SetActive(true);
		}
		if (this.activeInFall)
		{
			this.activeInFall.SetActive(false);
		}
		foreach (DropFloorBreaker.BreakThreshold breakThreshold in this.breakThresholds)
		{
			if (breakThreshold.Break)
			{
				breakThreshold.Break.SetActive(true);
			}
			if (breakThreshold.BreakEffects)
			{
				breakThreshold.BreakEffects.SetActive(false);
			}
		}
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x000C2B3C File Offset: 0x000C0D3C
	public void Break()
	{
		if (this.dropRoutine != null)
		{
			return;
		}
		if (this.dropBody)
		{
			this.dropBody.isKinematic = false;
		}
		if (this.activeInIdle)
		{
			this.activeInIdle.SetActive(false);
		}
		if (this.activeInFall)
		{
			this.activeInFall.SetActive(true);
		}
		this.dropRoutine = base.StartCoroutine(this.DropRoutine());
		if (this.droppedCamlock)
		{
			this.droppedCamlock.SetActive(true);
		}
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x000C2BC8 File Offset: 0x000C0DC8
	public void Broken()
	{
		if (this.dropRoutine != null)
		{
			base.StopCoroutine(this.dropRoutine);
		}
		if (this.dropBody)
		{
			this.dropBody.gameObject.SetActive(false);
		}
		if (this.activeInIdle)
		{
			this.activeInIdle.SetActive(false);
		}
		if (this.activeInFall)
		{
			this.activeInFall.SetActive(false);
		}
		foreach (DropFloorBreaker.BreakThreshold breakThreshold in this.breakThresholds)
		{
			if (breakThreshold.Break)
			{
				breakThreshold.Break.SetActive(false);
			}
			if (breakThreshold.BreakEffects)
			{
				breakThreshold.BreakEffects.SetActive(false);
			}
		}
		if (this.droppedCamlock)
		{
			this.droppedCamlock.SetActive(true);
		}
	}

	// Token: 0x06002C80 RID: 11392 RVA: 0x000C2CA1 File Offset: 0x000C0EA1
	private IEnumerator DropRoutine()
	{
		DropFloorBreaker.BreakThreshold[] orderedBreakThresholds = (from a in this.breakThresholds
		orderby a.YPos
		select a).Reverse<DropFloorBreaker.BreakThreshold>().ToArray<DropFloorBreaker.BreakThreshold>();
		Transform trans = base.transform;
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		int currentIndex = 0;
		while (currentIndex < orderedBreakThresholds.Length)
		{
			Vector2 position = this.dropBody.position;
			ref Vector3 ptr = trans.InverseTransformPoint(position);
			DropFloorBreaker.BreakThreshold breakThreshold = orderedBreakThresholds[currentIndex];
			if (ptr.y < breakThreshold.YPos)
			{
				if (breakThreshold.Break)
				{
					breakThreshold.Break.SetActive(false);
				}
				if (breakThreshold.BreakEffects)
				{
					breakThreshold.BreakEffects.SetActive(true);
				}
				int num = currentIndex;
				currentIndex = num + 1;
			}
			yield return wait;
		}
		float endYPosVal = this.endYPos.IsEnabled ? this.endYPos.Value : -20f;
		for (;;)
		{
			Vector2 position2 = this.dropBody.position;
			if (trans.InverseTransformPoint(position2).y < endYPosVal)
			{
				break;
			}
			yield return wait;
		}
		if (this.activeInFall)
		{
			this.activeInFall.SetActive(false);
		}
		yield break;
	}

	// Token: 0x04002E16 RID: 11798
	[SerializeField]
	private Rigidbody2D dropBody;

	// Token: 0x04002E17 RID: 11799
	[SerializeField]
	private GameObject activeInIdle;

	// Token: 0x04002E18 RID: 11800
	[SerializeField]
	private GameObject activeInFall;

	// Token: 0x04002E19 RID: 11801
	[SerializeField]
	private GameObject droppedCamlock;

	// Token: 0x04002E1A RID: 11802
	[SerializeField]
	private DropFloorBreaker.BreakThreshold[] breakThresholds;

	// Token: 0x04002E1B RID: 11803
	[SerializeField]
	private OverrideFloat endYPos;

	// Token: 0x04002E1C RID: 11804
	private Coroutine dropRoutine;

	// Token: 0x020017DE RID: 6110
	[Serializable]
	private struct BreakThreshold
	{
		// Token: 0x04008FB6 RID: 36790
		public float YPos;

		// Token: 0x04008FB7 RID: 36791
		public GameObject Break;

		// Token: 0x04008FB8 RID: 36792
		public GameObject BreakEffects;
	}
}
