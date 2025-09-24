using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020002B1 RID: 689
public class CorpseItems : MonoBehaviour, BlackThreadState.IBlackThreadStateReceiver
{
	// Token: 0x06001878 RID: 6264 RVA: 0x0007063D File Offset: 0x0006E83D
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.genericPickupOffset, 0.1f);
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x0007065F File Offset: 0x0006E85F
	protected virtual void Awake()
	{
		this.Animator = base.GetComponent<tk2dSpriteAnimator>();
		this.HasAnimator = this.Animator;
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x00070680 File Offset: 0x0006E880
	protected virtual void Start()
	{
		this.pickupItems = this.GetPickupItems();
		List<CorpseItems.ItemPickupSingle> list = this.pickupItems;
		if (list != null && list.Count > 0)
		{
			this.pickupObject = Object.Instantiate<GenericPickup>(Gameplay.GenericPickupPrefab, base.transform);
			this.pickupObject.transform.localPosition = this.genericPickupOffset;
			this.pickupObject.SetActive(false, true);
			this.pickupObject.PickupAction += this.DoPickupItems;
			if (this.extraPickupEffectPrefab)
			{
				this.pickupEffect = Object.Instantiate<GameObject>(this.extraPickupEffectPrefab, base.transform);
				this.pickupEffect.transform.localPosition = this.genericPickupOffset;
				this.pickupEffect.SetActive(false);
			}
		}
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x00070748 File Offset: 0x0006E948
	public void DeactivatePickup()
	{
		if (this.pickupObject)
		{
			this.pickupObject.SetActive(false, false);
		}
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x00070764 File Offset: 0x0006E964
	public void ActivatePickup()
	{
		if (this.pickupObject)
		{
			this.pickupObject.SetActive(true, false);
		}
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x00070780 File Offset: 0x0006E980
	public void ClearPickupItems()
	{
		this.itemPickupGroups.Clear();
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x00070790 File Offset: 0x0006E990
	private List<CorpseItems.ItemPickupSingle> GetPickupItems()
	{
		List<CorpseItems.ItemPickupSingle> list = null;
		foreach (CorpseItems.ItemPickupGroup itemPickupGroup in this.itemPickupGroups)
		{
			if (Random.Range(0f, 1f) <= itemPickupGroup.TotalProbability && itemPickupGroup.Drops.Count > 0)
			{
				CorpseItems.ItemPickupProbability itemPickupProbability = (CorpseItems.ItemPickupProbability)Probability.GetRandomItemRootByProbability<CorpseItems.ItemPickupProbability, SavedItem>(itemPickupGroup.Drops.ToArray(), null);
				if (itemPickupProbability != null)
				{
					SavedItem item = itemPickupProbability.Item;
					if (item && item.CanGetMore())
					{
						int randomValue = itemPickupProbability.Amount.GetRandomValue(true);
						if (list == null)
						{
							list = new List<CorpseItems.ItemPickupSingle>(randomValue);
						}
						for (int i = 0; i < randomValue; i++)
						{
							list.Add(new CorpseItems.ItemPickupSingle
							{
								Item = item,
								PlayAnimOnPickup = itemPickupProbability.PlayAnimOnPickup
							});
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x00070894 File Offset: 0x0006EA94
	private bool DoPickupItems()
	{
		int count = this.pickupItems.Count;
		for (int i = count - 1; i >= 0; i--)
		{
			CorpseItems.ItemPickupSingle itemPickupSingle = this.pickupItems[i];
			if (itemPickupSingle.Item.TryGet(false, true))
			{
				this.pickupItems.RemoveAt(i);
				if (!string.IsNullOrEmpty(itemPickupSingle.PlayAnimOnPickup) && this.HasAnimator)
				{
					this.Animator.Play(itemPickupSingle.PlayAnimOnPickup);
				}
			}
		}
		if (this.pickupItems.Count < count && this.pickupEffect)
		{
			this.pickupEffect.SetActive(false);
			this.pickupEffect.SetActive(true);
		}
		if (this.pickupItems.Count != 0)
		{
			return false;
		}
		this.pickupObject.transform.SetParent(null, true);
		this.pickupObject = null;
		base.transform.Translate(0f, 0f, 0.005f, Space.Self);
		return true;
	}

	// Token: 0x1700027C RID: 636
	// (get) Token: 0x06001880 RID: 6272 RVA: 0x00070980 File Offset: 0x0006EB80
	protected bool IsBlackThreaded
	{
		get
		{
			return this.isBlackThreaded;
		}
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x00070988 File Offset: 0x0006EB88
	public float GetBlackThreadAmount()
	{
		return (float)(this.isBlackThreaded ? 1 : 0);
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x00070998 File Offset: 0x0006EB98
	public void SetIsBlackThreaded(bool isThreaded)
	{
		if (isThreaded)
		{
			this.isBlackThreaded = true;
			BlackThreadEffectRendererGroup component = base.GetComponent<BlackThreadEffectRendererGroup>();
			if (component != null)
			{
				component.SetBlackThreadAmount(1f);
				return;
			}
		}
		else
		{
			this.isBlackThreaded = false;
			BlackThreadEffectRendererGroup component2 = base.GetComponent<BlackThreadEffectRendererGroup>();
			if (component2 != null)
			{
				component2.OnRecycled();
			}
		}
	}

	// Token: 0x04001776 RID: 6006
	[SerializeField]
	private List<CorpseItems.ItemPickupGroup> itemPickupGroups;

	// Token: 0x04001777 RID: 6007
	[SerializeField]
	private Vector3 genericPickupOffset;

	// Token: 0x04001778 RID: 6008
	[SerializeField]
	private GameObject extraPickupEffectPrefab;

	// Token: 0x04001779 RID: 6009
	private List<CorpseItems.ItemPickupSingle> pickupItems;

	// Token: 0x0400177A RID: 6010
	private GenericPickup pickupObject;

	// Token: 0x0400177B RID: 6011
	private GameObject pickupEffect;

	// Token: 0x0400177C RID: 6012
	protected tk2dSpriteAnimator Animator;

	// Token: 0x0400177D RID: 6013
	protected bool HasAnimator;

	// Token: 0x0400177E RID: 6014
	private bool isBlackThreaded;

	// Token: 0x0200159A RID: 5530
	[Serializable]
	private class ItemPickupProbability : Probability.ProbabilityBase<SavedItem>
	{
		// Token: 0x17000DD3 RID: 3539
		// (get) Token: 0x06008793 RID: 34707 RVA: 0x00276DC2 File Offset: 0x00274FC2
		public override SavedItem Item
		{
			get
			{
				return this.item;
			}
		}

		// Token: 0x040087FB RID: 34811
		[SerializeField]
		private SavedItem item;

		// Token: 0x040087FC RID: 34812
		public MinMaxInt Amount = new MinMaxInt(1, 1);

		// Token: 0x040087FD RID: 34813
		public string PlayAnimOnPickup;
	}

	// Token: 0x0200159B RID: 5531
	[Serializable]
	private class ItemPickupGroup
	{
		// Token: 0x040087FE RID: 34814
		[Range(0f, 1f)]
		public float TotalProbability = 1f;

		// Token: 0x040087FF RID: 34815
		public List<CorpseItems.ItemPickupProbability> Drops;
	}

	// Token: 0x0200159C RID: 5532
	private struct ItemPickupSingle
	{
		// Token: 0x04008800 RID: 34816
		public SavedItem Item;

		// Token: 0x04008801 RID: 34817
		public string PlayAnimOnPickup;
	}
}
