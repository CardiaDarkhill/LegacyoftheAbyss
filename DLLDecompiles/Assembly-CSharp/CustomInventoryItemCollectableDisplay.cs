using System;
using UnityEngine;

// Token: 0x0200067C RID: 1660
public class CustomInventoryItemCollectableDisplay : MonoBehaviour
{
	// Token: 0x170006B4 RID: 1716
	// (get) Token: 0x06003B60 RID: 15200 RVA: 0x00105440 File Offset: 0x00103640
	// (set) Token: 0x06003B61 RID: 15201 RVA: 0x00105448 File Offset: 0x00103648
	public InventoryItemSelectable Owner { get; set; }

	// Token: 0x140000C6 RID: 198
	// (add) Token: 0x06003B62 RID: 15202 RVA: 0x00105454 File Offset: 0x00103654
	// (remove) Token: 0x06003B63 RID: 15203 RVA: 0x0010548C File Offset: 0x0010368C
	public event Action<CustomInventoryItemCollectableDisplay> OnDestroyed;

	// Token: 0x170006B5 RID: 1717
	// (get) Token: 0x06003B64 RID: 15204 RVA: 0x001054C1 File Offset: 0x001036C1
	public float JitterMagnitudeMultiplier
	{
		get
		{
			return this.jitterMagnitudeMultiplier;
		}
	}

	// Token: 0x06003B65 RID: 15205 RVA: 0x001054C9 File Offset: 0x001036C9
	protected virtual void OnEnable()
	{
		this.isObjActive = true;
		if (this.isPaneActive && !this.activated)
		{
			this.OnActivate();
			this.activated = true;
		}
	}

	// Token: 0x06003B66 RID: 15206 RVA: 0x001054EF File Offset: 0x001036EF
	protected virtual void OnDisable()
	{
		this.isObjActive = false;
		if (this.isPaneActive && this.activated)
		{
			this.OnDeactivate();
			this.activated = false;
		}
	}

	// Token: 0x06003B67 RID: 15207 RVA: 0x00105515 File Offset: 0x00103715
	protected virtual void OnDestroy()
	{
		Action<CustomInventoryItemCollectableDisplay> onDestroyed = this.OnDestroyed;
		if (onDestroyed != null)
		{
			onDestroyed(this);
		}
		this.OnDestroyed = null;
	}

	// Token: 0x06003B68 RID: 15208 RVA: 0x00105530 File Offset: 0x00103730
	public virtual void OnPaneStart()
	{
		this.isPaneActive = true;
		if (this.isObjActive && !this.activated)
		{
			this.OnActivate();
			this.activated = true;
		}
	}

	// Token: 0x06003B69 RID: 15209 RVA: 0x00105556 File Offset: 0x00103756
	public virtual void OnPaneEnd()
	{
		this.isPaneActive = false;
		if (this.isObjActive && this.activated)
		{
			this.OnDeactivate();
			this.activated = false;
		}
	}

	// Token: 0x06003B6A RID: 15210 RVA: 0x0010557C File Offset: 0x0010377C
	protected virtual void OnActivate()
	{
	}

	// Token: 0x06003B6B RID: 15211 RVA: 0x0010557E File Offset: 0x0010377E
	protected virtual void OnDeactivate()
	{
	}

	// Token: 0x06003B6C RID: 15212 RVA: 0x00105580 File Offset: 0x00103780
	public virtual void OnPrePaneEnd()
	{
	}

	// Token: 0x06003B6D RID: 15213 RVA: 0x00105582 File Offset: 0x00103782
	public virtual void OnSelect()
	{
	}

	// Token: 0x06003B6E RID: 15214 RVA: 0x00105584 File Offset: 0x00103784
	public virtual void OnDeselect()
	{
	}

	// Token: 0x06003B6F RID: 15215 RVA: 0x00105586 File Offset: 0x00103786
	public virtual void OnConsumeStart()
	{
	}

	// Token: 0x06003B70 RID: 15216 RVA: 0x00105588 File Offset: 0x00103788
	public virtual void OnConsumeEnd()
	{
	}

	// Token: 0x06003B71 RID: 15217 RVA: 0x0010558A File Offset: 0x0010378A
	public virtual void OnConsumeComplete()
	{
	}

	// Token: 0x06003B72 RID: 15218 RVA: 0x0010558C File Offset: 0x0010378C
	public virtual void OnConsumeBlocked()
	{
	}

	// Token: 0x04003DA9 RID: 15785
	[SerializeField]
	private float jitterMagnitudeMultiplier = 1f;

	// Token: 0x04003DAA RID: 15786
	private bool isPaneActive = true;

	// Token: 0x04003DAB RID: 15787
	private bool isObjActive;

	// Token: 0x04003DAC RID: 15788
	private bool activated;
}
