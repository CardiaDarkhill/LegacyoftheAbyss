using System;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public class JournalEntryItem : InventoryItemUpdateable, InventoryItemListManager<JournalEntryItem, EnemyJournalRecord>.IMoveNextButton
{
	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06001B1F RID: 6943 RVA: 0x0007E172 File Offset: 0x0007C372
	// (set) Token: 0x06001B20 RID: 6944 RVA: 0x0007E17A File Offset: 0x0007C37A
	public EnemyJournalRecord Record { get; private set; }

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06001B21 RID: 6945 RVA: 0x0007E183 File Offset: 0x0007C383
	public override string DisplayName
	{
		get
		{
			if (!this.Record.IsVisible)
			{
				return null;
			}
			return this.Record.DisplayName;
		}
	}

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x06001B22 RID: 6946 RVA: 0x0007E1A4 File Offset: 0x0007C3A4
	public override string Description
	{
		get
		{
			if (!this.Record.IsVisible)
			{
				return null;
			}
			return this.Record.Description;
		}
	}

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x06001B23 RID: 6947 RVA: 0x0007E1C5 File Offset: 0x0007C3C5
	// (set) Token: 0x06001B24 RID: 6948 RVA: 0x0007E1E1 File Offset: 0x0007C3E1
	protected override bool IsSeen
	{
		get
		{
			return !this.Record.IsVisible || this.Record.SeenInJournal;
		}
		set
		{
			if (!this.Record.IsVisible)
			{
				return;
			}
			if (value)
			{
				EnemyJournalManager.SetJournalSeen(this.Record);
				return;
			}
			throw new NotImplementedException();
		}
	}

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x06001B25 RID: 6949 RVA: 0x0007E205 File Offset: 0x0007C405
	public bool WillSubmitMoveNext
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001B26 RID: 6950 RVA: 0x0007E208 File Offset: 0x0007C408
	public void Setup(EnemyJournalRecord record)
	{
		this.Record = record;
		base.gameObject.name = "Journal Entry (" + record.name + ")";
		bool flag = !record.IsVisible;
		if (this.emptyIcon)
		{
			this.emptyIcon.SetActive(flag);
		}
		if (this.iconSprite)
		{
			if (this.initialSprite == null)
			{
				this.initialSprite = this.iconSprite.sprite;
			}
			if (flag)
			{
				this.iconSprite.gameObject.SetActive(false);
			}
			else
			{
				this.iconSprite.gameObject.SetActive(true);
				this.iconSprite.sprite = (record.IconSprite ? record.IconSprite : this.initialSprite);
			}
		}
		bool flag2 = !flag && record.KillCount >= record.KillsRequired;
		if (this.completeFrame)
		{
			this.completeFrame.SetActive(!flag && flag2);
		}
		if (this.standardFrame)
		{
			this.standardFrame.SetActive(!flag && !flag2);
		}
		this.UpdateDisplay();
	}

	// Token: 0x04001A14 RID: 6676
	[Space]
	[SerializeField]
	private GameObject emptyIcon;

	// Token: 0x04001A15 RID: 6677
	[SerializeField]
	private GameObject standardFrame;

	// Token: 0x04001A16 RID: 6678
	[SerializeField]
	private GameObject completeFrame;

	// Token: 0x04001A17 RID: 6679
	[SerializeField]
	private SpriteRenderer iconSprite;

	// Token: 0x04001A18 RID: 6680
	private Sprite initialSprite;
}
