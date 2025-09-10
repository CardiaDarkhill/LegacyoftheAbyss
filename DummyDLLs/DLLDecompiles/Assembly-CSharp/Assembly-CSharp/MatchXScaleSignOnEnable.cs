using System;
using UnityEngine;

// Token: 0x02000444 RID: 1092
public class MatchXScaleSignOnEnable : MonoBehaviour
{
	// Token: 0x06002676 RID: 9846 RVA: 0x000AE369 File Offset: 0x000AC569
	private void OnEnable()
	{
		this.targetSignPositive = (this.matchObject.localScale.x > 0f);
		if (this.reverseMatch)
		{
			this.targetSignPositive = !this.targetSignPositive;
		}
	}

	// Token: 0x06002677 RID: 9847 RVA: 0x000AE3A0 File Offset: 0x000AC5A0
	private void LateUpdate()
	{
		if ((base.transform.lossyScale.x < 0f && this.targetSignPositive) || (base.transform.lossyScale.x > 0f && !this.targetSignPositive))
		{
			base.transform.localScale = new Vector3(-base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x000AE42C File Offset: 0x000AC62C
	public void SetTargetSign(bool positive)
	{
		this.targetSignPositive = positive;
	}

	// Token: 0x040023C7 RID: 9159
	public Transform matchObject;

	// Token: 0x040023C8 RID: 9160
	public bool reverseMatch;

	// Token: 0x040023C9 RID: 9161
	private bool targetSignPositive;
}
