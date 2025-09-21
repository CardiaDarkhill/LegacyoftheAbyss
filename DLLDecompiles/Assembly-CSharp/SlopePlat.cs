using System;
using UnityEngine;

// Token: 0x02000554 RID: 1364
public class SlopePlat : MonoBehaviour
{
	// Token: 0x060030D9 RID: 12505 RVA: 0x000D83D0 File Offset: 0x000D65D0
	private void Start()
	{
		this.hero = GameObject.FindWithTag("Player");
	}

	// Token: 0x060030DA RID: 12506 RVA: 0x000D83E4 File Offset: 0x000D65E4
	private void Update()
	{
		float x = this.hero.transform.position.x;
		if (x <= this.heroXLeft)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.platYLeft, base.transform.localPosition.z);
			return;
		}
		if (x >= this.heroXRight)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.platYRight, base.transform.localPosition.z);
			return;
		}
		float t = Mathf.InverseLerp(this.heroXLeft, this.heroXRight, x);
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, Mathf.Lerp(this.platYLeft, this.platYRight, t), base.transform.localPosition.z);
	}

	// Token: 0x04003412 RID: 13330
	public float heroXLeft;

	// Token: 0x04003413 RID: 13331
	public float heroXRight;

	// Token: 0x04003414 RID: 13332
	public float platYLeft;

	// Token: 0x04003415 RID: 13333
	public float platYRight;

	// Token: 0x04003416 RID: 13334
	private GameObject hero;
}
