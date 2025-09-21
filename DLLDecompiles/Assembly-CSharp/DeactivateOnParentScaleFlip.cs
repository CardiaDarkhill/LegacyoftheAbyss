using System;
using UnityEngine;

// Token: 0x0200007F RID: 127
public class DeactivateOnParentScaleFlip : MonoBehaviour
{
	// Token: 0x06000388 RID: 904 RVA: 0x0001245C File Offset: 0x0001065C
	private void Awake()
	{
		if (!this.parent)
		{
			this.hc = base.GetComponentInParent<HeroController>(true);
			if (this.hc)
			{
				this.hc.FlippedSprite += this.Disable;
			}
		}
	}

	// Token: 0x06000389 RID: 905 RVA: 0x0001249C File Offset: 0x0001069C
	private void OnEnable()
	{
		if (this.parent)
		{
			this.initialXSign = Mathf.Sign(this.parent.transform.localScale.x);
		}
	}

	// Token: 0x0600038A RID: 906 RVA: 0x000124CB File Offset: 0x000106CB
	private void OnDestroy()
	{
		if (this.hc)
		{
			this.hc.FlippedSprite -= this.Disable;
			this.hc = null;
		}
	}

	// Token: 0x0600038B RID: 907 RVA: 0x000124F8 File Offset: 0x000106F8
	private void LateUpdate()
	{
		if (!this.parent)
		{
			return;
		}
		if (!Mathf.Approximately(Mathf.Sign(this.parent.transform.localScale.x), this.initialXSign))
		{
			this.Disable();
		}
	}

	// Token: 0x0600038C RID: 908 RVA: 0x00012535 File Offset: 0x00010735
	private void Disable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000337 RID: 823
	[SerializeField]
	private Transform parent;

	// Token: 0x04000338 RID: 824
	private float initialXSign;

	// Token: 0x04000339 RID: 825
	private HeroController hc;
}
