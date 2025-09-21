using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004E6 RID: 1254
[ExecuteInEditMode]
public class ForceUniformScaling : MonoBehaviour
{
	// Token: 0x06002CFC RID: 11516 RVA: 0x000C48FA File Offset: 0x000C2AFA
	[ContextMenu("Refresh")]
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			base.StartCoroutine(this.ReScaleWait());
			return;
		}
		this.DoReScale();
	}

	// Token: 0x06002CFD RID: 11517 RVA: 0x000C4917 File Offset: 0x000C2B17
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06002CFE RID: 11518 RVA: 0x000C491F File Offset: 0x000C2B1F
	private IEnumerator ReScaleWait()
	{
		yield return this.waitForEndOfFrame;
		this.DoReScale();
		yield break;
	}

	// Token: 0x06002CFF RID: 11519 RVA: 0x000C4930 File Offset: 0x000C2B30
	private void DoReScale()
	{
		Transform transform = base.transform;
		Vector3 localScale = transform.localScale;
		Vector3 lossyScale = transform.lossyScale;
		if (Math.Abs(lossyScale.x - lossyScale.y) <= Mathf.Epsilon)
		{
			return;
		}
		float num = Mathf.Abs(lossyScale.x);
		float num2 = Mathf.Abs(lossyScale.y);
		float num3 = num / num2;
		float num4 = num2 / num;
		if (num3 > num4)
		{
			localScale.y *= num3;
		}
		else
		{
			localScale.x *= num4;
		}
		transform.localScale = localScale;
	}

	// Token: 0x04002EA6 RID: 11942
	private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
}
