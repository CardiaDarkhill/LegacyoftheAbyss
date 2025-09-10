using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000791 RID: 1937
public static class TimeUtility
{
	// Token: 0x06004492 RID: 17554 RVA: 0x0012C592 File Offset: 0x0012A792
	public static IEnumerator Wait(float duration)
	{
		for (float timer = 0f; timer < duration; timer += Time.deltaTime)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06004493 RID: 17555 RVA: 0x0012C5A1 File Offset: 0x0012A7A1
	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
		yield break;
	}
}
