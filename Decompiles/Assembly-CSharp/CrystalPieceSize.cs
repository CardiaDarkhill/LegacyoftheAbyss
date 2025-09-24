using System;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class CrystalPieceSize : MonoBehaviour
{
	// Token: 0x0600147C RID: 5244 RVA: 0x0005C5EC File Offset: 0x0005A7EC
	private void OnEnable()
	{
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, Random.Range(-0.01f, 0.01f));
		float num;
		if (Random.Range(0, 100) < 75)
		{
			num = Random.Range(0.65f, 0.85f);
		}
		else
		{
			num = Random.Range(0.9f, 1.2f);
		}
		base.transform.localScale = new Vector3(num, num, num);
	}
}
