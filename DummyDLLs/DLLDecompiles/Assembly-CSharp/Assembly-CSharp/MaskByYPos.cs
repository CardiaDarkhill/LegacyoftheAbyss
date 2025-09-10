using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200051A RID: 1306
public class MaskByYPos : MonoBehaviour
{
	// Token: 0x06002F04 RID: 12036 RVA: 0x000CF9D8 File Offset: 0x000CDBD8
	private void OnDrawGizmosSelected()
	{
		if (this.maskIfAboveY)
		{
			Vector3 position = base.transform.position;
			float? y = new float?(this.aboveYPos);
			Gizmos.DrawWireSphere(position.Where(null, y, null), 0.3f);
		}
		if (this.maskIfBelowY)
		{
			Vector3 position2 = base.transform.position;
			float? y = new float?(this.belowYPos);
			Gizmos.DrawWireSphere(position2.Where(null, y, null), 0.3f);
		}
	}

	// Token: 0x06002F05 RID: 12037 RVA: 0x000CFA69 File Offset: 0x000CDC69
	private void OnValidate()
	{
		if (!Mathf.Approximately(this.yPos, 0f))
		{
			this.aboveYPos = this.yPos;
			this.belowYPos = this.yPos;
			this.yPos = 0f;
		}
	}

	// Token: 0x06002F06 RID: 12038 RVA: 0x000CFAA0 File Offset: 0x000CDCA0
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06002F07 RID: 12039 RVA: 0x000CFAA8 File Offset: 0x000CDCA8
	private void Reset()
	{
		this.fadeGroup = base.GetComponent<NestedFadeGroupBase>();
		if (!this.fadeGroup)
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		}
	}

	// Token: 0x06002F08 RID: 12040 RVA: 0x000CFAD0 File Offset: 0x000CDCD0
	private void OnEnable()
	{
		if (this.spriteRenderer == null && !this.fadeGroup)
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		}
		this.hasSpriteRenderer = this.spriteRenderer;
		this.hasFadeGroup = this.fadeGroup;
		this.DoCheck(true);
	}

	// Token: 0x06002F09 RID: 12041 RVA: 0x000CFB2D File Offset: 0x000CDD2D
	private void LateUpdate()
	{
		this.DoCheck(false);
	}

	// Token: 0x06002F0A RID: 12042 RVA: 0x000CFB38 File Offset: 0x000CDD38
	private void DoCheck(bool force)
	{
		float y = base.transform.position.y;
		bool flag = (!this.maskIfAboveY || y <= this.aboveYPos) && (!this.maskIfBelowY || y >= this.belowYPos);
		if (!flag)
		{
			if (this.wasActive || force)
			{
				if (this.hasSpriteRenderer)
				{
					this.spriteRenderer.enabled = false;
				}
				if (this.hasFadeGroup)
				{
					this.fadeGroup.AlphaSelf = 0f;
				}
			}
		}
		else if (!this.wasActive || force)
		{
			if (this.hasSpriteRenderer)
			{
				this.spriteRenderer.enabled = true;
			}
			if (this.hasFadeGroup)
			{
				this.fadeGroup.AlphaSelf = 1f;
			}
		}
		this.wasActive = flag;
	}

	// Token: 0x040031A8 RID: 12712
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x040031A9 RID: 12713
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x040031AA RID: 12714
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private float yPos;

	// Token: 0x040031AB RID: 12715
	[SerializeField]
	private bool maskIfAboveY;

	// Token: 0x040031AC RID: 12716
	[SerializeField]
	[ModifiableProperty]
	[Conditional("maskIfAboveY", true, false, false)]
	private float aboveYPos;

	// Token: 0x040031AD RID: 12717
	[SerializeField]
	private bool maskIfBelowY;

	// Token: 0x040031AE RID: 12718
	[SerializeField]
	[ModifiableProperty]
	[Conditional("maskIfBelowY", true, false, false)]
	private float belowYPos;

	// Token: 0x040031AF RID: 12719
	private bool wasActive;

	// Token: 0x040031B0 RID: 12720
	private bool hasSpriteRenderer;

	// Token: 0x040031B1 RID: 12721
	private bool hasFadeGroup;
}
