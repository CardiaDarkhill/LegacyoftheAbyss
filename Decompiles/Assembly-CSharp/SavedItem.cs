using System;
using UnityEngine;

// Token: 0x020001F1 RID: 497
public abstract class SavedItem : ScriptableObject
{
	// Token: 0x17000227 RID: 551
	// (get) Token: 0x06001328 RID: 4904 RVA: 0x00058154 File Offset: 0x00056354
	public virtual bool CanGetMultipleAtOnce
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x06001329 RID: 4905 RVA: 0x00058157 File Offset: 0x00056357
	public virtual bool IsUnique
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x0005815A File Offset: 0x0005635A
	public void Get(int amount, bool showPopup = true)
	{
		if (this.CanGetMultipleAtOnce)
		{
			this.GetMultiple(amount, showPopup);
			return;
		}
		this.Get(showPopup);
	}

	// Token: 0x0600132B RID: 4907
	public abstract void Get(bool showPopup = true);

	// Token: 0x0600132C RID: 4908
	public abstract bool CanGetMore();

	// Token: 0x0600132D RID: 4909 RVA: 0x00058174 File Offset: 0x00056374
	protected virtual void GetMultiple(int amount, bool showPopup)
	{
		for (int i = 0; i < amount; i++)
		{
			this.Get(showPopup);
		}
	}

	// Token: 0x0600132E RID: 4910 RVA: 0x00058194 File Offset: 0x00056394
	public bool TryGet(bool breakIfAtMax, bool showPopup = true)
	{
		CollectableItem collectableItem = this as CollectableItem;
		if (!(collectableItem != null) || !collectableItem.IsAtMax())
		{
			this.Get(showPopup);
			return true;
		}
		this.Get(showPopup);
		if (breakIfAtMax)
		{
			collectableItem.ConsumeItemResponse();
			collectableItem.InstantUseSounds.SpawnAndPlayOneShot(HeroController.instance.transform.position, null);
			return true;
		}
		return false;
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x000581F4 File Offset: 0x000563F4
	public virtual Sprite GetPopupIcon()
	{
		if (Application.isPlaying)
		{
			Debug.LogException(new NotImplementedException());
		}
		return null;
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x00058208 File Offset: 0x00056408
	public virtual string GetPopupName()
	{
		if (Application.isPlaying)
		{
			Debug.LogException(new NotImplementedException());
		}
		return null;
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x0005821C File Offset: 0x0005641C
	public virtual int GetSavedAmount()
	{
		if (Application.isPlaying)
		{
			Debug.LogException(new NotImplementedException());
		}
		return 0;
	}

	// Token: 0x06001332 RID: 4914 RVA: 0x00058230 File Offset: 0x00056430
	public virtual bool HasUpgradeIcon()
	{
		return false;
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x00058233 File Offset: 0x00056433
	public virtual bool GetTakesHeroControl()
	{
		return false;
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x00058236 File Offset: 0x00056436
	public virtual void SetupExtraDescription(GameObject obj)
	{
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x00058238 File Offset: 0x00056438
	public virtual void SetHasNew(bool hasPopup)
	{
		Debug.LogException(new NotImplementedException());
	}
}
