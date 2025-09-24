using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000269 RID: 617
public class RandomRotationDelay : MonoBehaviour
{
	// Token: 0x06001618 RID: 5656 RVA: 0x000630AA File Offset: 0x000612AA
	private void Start()
	{
		base.StartCoroutine(this.RandomRotate());
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x000630B9 File Offset: 0x000612B9
	private void OnEnable()
	{
		base.StartCoroutine(this.RandomRotate());
	}

	// Token: 0x0600161A RID: 5658 RVA: 0x000630C8 File Offset: 0x000612C8
	private IEnumerator RandomRotate()
	{
		yield return new WaitForSeconds(this.delay);
		Transform transform = base.transform;
		Vector3 localEulerAngles = transform.localEulerAngles;
		localEulerAngles.z = Random.Range(0f, 360f);
		transform.localEulerAngles = localEulerAngles;
		yield break;
	}

	// Token: 0x04001483 RID: 5251
	public float delay = 0.25f;
}
