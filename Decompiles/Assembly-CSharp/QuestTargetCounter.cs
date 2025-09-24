using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005A6 RID: 1446
public abstract class QuestTargetCounter : SavedItem, ICollectableUIMsgItem, IUIMsgPopupItem
{
	// Token: 0x140000A5 RID: 165
	// (add) Token: 0x060033F9 RID: 13305 RVA: 0x000E7D58 File Offset: 0x000E5F58
	// (remove) Token: 0x060033FA RID: 13306 RVA: 0x000E7D8C File Offset: 0x000E5F8C
	public static event Action<QuestTargetCounter> OnIncrement;

	// Token: 0x170005B7 RID: 1463
	// (get) Token: 0x060033FB RID: 13307 RVA: 0x000E7DBF File Offset: 0x000E5FBF
	protected virtual bool ShowCounterOnConsume
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060033FC RID: 13308 RVA: 0x000E7DC2 File Offset: 0x000E5FC2
	public void Increment()
	{
		Action<QuestTargetCounter> onIncrement = QuestTargetCounter.OnIncrement;
		if (onIncrement == null)
		{
			return;
		}
		onIncrement(this);
	}

	// Token: 0x060033FD RID: 13309 RVA: 0x000E7DD4 File Offset: 0x000E5FD4
	public static void ClearStatic()
	{
		QuestTargetCounter.OnIncrement = null;
	}

	// Token: 0x060033FE RID: 13310 RVA: 0x000E7DDC File Offset: 0x000E5FDC
	public virtual bool ShouldIncrementQuestCounter(QuestTargetCounter eventSender)
	{
		return eventSender == this;
	}

	// Token: 0x060033FF RID: 13311 RVA: 0x000E7DE5 File Offset: 0x000E5FE5
	public virtual int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return sourceCompletion.CompletedCount;
	}

	// Token: 0x170005B8 RID: 1464
	// (get) Token: 0x06003400 RID: 13312 RVA: 0x000E7DED File Offset: 0x000E5FED
	public virtual bool CanConsume
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06003401 RID: 13313 RVA: 0x000E7DF0 File Offset: 0x000E5FF0
	public virtual void Consume(int amount, bool showCounter)
	{
	}

	// Token: 0x06003402 RID: 13314 RVA: 0x000E7DF2 File Offset: 0x000E5FF2
	public override void Get(bool showPopup = true)
	{
		Debug.LogErrorFormat(this, "\"{0}\" has no \"Get()\" implementation.", new object[]
		{
			base.name
		});
	}

	// Token: 0x06003403 RID: 13315 RVA: 0x000E7E0E File Offset: 0x000E600E
	public virtual Sprite GetQuestCounterSprite(int index)
	{
		return this.GetPopupIcon();
	}

	// Token: 0x06003404 RID: 13316 RVA: 0x000E7E16 File Offset: 0x000E6016
	public Sprite GetUIMsgSprite()
	{
		return this.GetPopupIcon();
	}

	// Token: 0x06003405 RID: 13317 RVA: 0x000E7E1E File Offset: 0x000E601E
	public string GetUIMsgName()
	{
		return this.GetPopupName();
	}

	// Token: 0x06003406 RID: 13318 RVA: 0x000E7E26 File Offset: 0x000E6026
	public virtual float GetUIMsgIconScale()
	{
		return 1f;
	}

	// Token: 0x06003407 RID: 13319 RVA: 0x000E7E2D File Offset: 0x000E602D
	public Object GetRepresentingObject()
	{
		return this;
	}

	// Token: 0x06003408 RID: 13320 RVA: 0x000E7E30 File Offset: 0x000E6030
	public virtual IEnumerable<QuestTargetCounter> EnumerateSubTargets()
	{
		yield break;
	}
}
