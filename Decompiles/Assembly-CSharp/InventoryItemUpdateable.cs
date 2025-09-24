using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public abstract class InventoryItemUpdateable : InventoryItemSelectableDirectional
{
	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060011B3 RID: 4531
	// (set) Token: 0x060011B4 RID: 4532
	protected abstract bool IsSeen { get; set; }

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x060011B5 RID: 4533 RVA: 0x00052BA8 File Offset: 0x00050DA8
	// (set) Token: 0x060011B6 RID: 4534 RVA: 0x00052C18 File Offset: 0x00050E18
	private bool IsSeenAll
	{
		get
		{
			if (this.isSeenProviders == null)
			{
				return this.IsSeen;
			}
			using (List<InventoryItemUpdateable.IIsSeenProvider>.Enumerator enumerator = this.isSeenProviders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.IsSeen)
					{
						return false;
					}
				}
			}
			return this.IsSeen;
		}
		set
		{
			this.IsSeen = value;
			if (this.isSeenProviders == null)
			{
				return;
			}
			foreach (InventoryItemUpdateable.IIsSeenProvider isSeenProvider in this.isSeenProviders)
			{
				isSeenProvider.IsSeen = value;
			}
		}
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x00052C7C File Offset: 0x00050E7C
	protected override void Awake()
	{
		base.Awake();
		if (this.newDot)
		{
			this.newDotInitialScale = this.newDot.transform.localScale;
		}
		this.Pane = base.GetComponentInParent<InventoryPane>();
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x00052CB4 File Offset: 0x00050EB4
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.Pane)
		{
			this.Pane.OnPaneStart += this.ResetIsScaling;
			this.Pane.OnPostPaneStart += this.UpdateDisplay;
		}
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x00052D04 File Offset: 0x00050F04
	protected override void OnDisable()
	{
		base.OnDisable();
		if (this.Pane)
		{
			this.Pane.OnPaneStart -= this.ResetIsScaling;
			this.Pane.OnPostPaneStart -= this.UpdateDisplay;
		}
		if (this.isScaling)
		{
			this.isScaling = false;
			this.newDot.SetActive(false);
		}
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x00052D6E File Offset: 0x00050F6E
	protected virtual void OnDestroy()
	{
		if (this.Pane)
		{
			this.Pane = null;
		}
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x00052D84 File Offset: 0x00050F84
	private void ResetIsScaling()
	{
		this.isScaling = false;
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x00052D90 File Offset: 0x00050F90
	protected override void UpdateDisplay()
	{
		base.UpdateDisplay();
		if (!this.newDot)
		{
			return;
		}
		if (this.isScaling)
		{
			return;
		}
		if (!this.IsSeenAll)
		{
			this.isNew = true;
		}
		this.newDot.SetActive(this.isNew);
		this.newDot.transform.localScale = this.newDotInitialScale;
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x00052DF0 File Offset: 0x00050FF0
	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		if (!this.isNew)
		{
			return;
		}
		this.isNew = false;
		if (this.newDot)
		{
			this.newDot.transform.ScaleTo(this, Vector3.zero, UI.NewDotScaleTime, UI.NewDotScaleDelay, false, true, null);
			this.IsSeenAll = true;
			this.isScaling = true;
			return;
		}
		try
		{
			this.IsSeenAll = true;
		}
		catch (NotImplementedException)
		{
		}
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x00052E70 File Offset: 0x00051070
	public void RegisterIsSeenProvider(InventoryItemUpdateable.IIsSeenProvider provider)
	{
		if (this.isSeenProviders == null)
		{
			this.isSeenProviders = new List<InventoryItemUpdateable.IIsSeenProvider>();
		}
		this.isSeenProviders.Add(provider);
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x00052E91 File Offset: 0x00051091
	public void DeregisterIsSeenProvider(InventoryItemUpdateable.IIsSeenProvider provider)
	{
		List<InventoryItemUpdateable.IIsSeenProvider> list = this.isSeenProviders;
		if (list == null)
		{
			return;
		}
		list.Remove(provider);
	}

	// Token: 0x040010A0 RID: 4256
	[Space]
	[SerializeField]
	private GameObject newDot;

	// Token: 0x040010A1 RID: 4257
	private Vector3 newDotInitialScale;

	// Token: 0x040010A2 RID: 4258
	private bool isNew;

	// Token: 0x040010A3 RID: 4259
	private bool isScaling;

	// Token: 0x040010A4 RID: 4260
	private List<InventoryItemUpdateable.IIsSeenProvider> isSeenProviders;

	// Token: 0x040010A5 RID: 4261
	protected InventoryPane Pane;

	// Token: 0x02001509 RID: 5385
	public interface IIsSeenProvider
	{
		// Token: 0x17000D4A RID: 3402
		// (get) Token: 0x06008586 RID: 34182
		// (set) Token: 0x06008587 RID: 34183
		bool IsSeen { get; set; }
	}
}
