using System;
using UnityEngine;

// Token: 0x020003F0 RID: 1008
public class SpecialQuestItemVariant : MonoBehaviour
{
	// Token: 0x0600226D RID: 8813 RVA: 0x0009E83E File Offset: 0x0009CA3E
	private void OnDisable()
	{
		this.isInactive = false;
	}

	// Token: 0x0600226E RID: 8814 RVA: 0x0009E848 File Offset: 0x0009CA48
	private void Start()
	{
		if (this.isInactive)
		{
			return;
		}
		if (Random.Range(0f, 1f) > this.probability)
		{
			return;
		}
		if (!this.activeQuest.IsAccepted)
		{
			return;
		}
		if (this.activeQuest.IsCompleted)
		{
			return;
		}
		if (this.activeQuest.CanComplete)
		{
			return;
		}
		if (this.spriteFlash)
		{
			this.spriteFlash.flashWhiteLong();
			this.spriteFlash.FlashingWhiteLong();
		}
		if (this.itemPickupPrefab)
		{
			CollectableItemPickup itemPick = Object.Instantiate<CollectableItemPickup>(this.itemPickupPrefab, base.transform);
			Transform itemPickTrans = itemPick.transform;
			itemPickTrans.localPosition = new Vector3(0f, 0f, -0.0001f);
			itemPickTrans.SetRotation2D(0f);
			itemPick.SetItem(this.activeQuest, false);
			itemPick.OnPickup.AddListener(delegate()
			{
				itemPickTrans.SetParent(null, true);
				if (this.spriteFlash)
				{
					this.spriteFlash.gameObject.SetActive(false);
				}
			});
			if (this.poleTopLand)
			{
				this.poleTopLand.OnStick.AddListener(delegate()
				{
					itemPick.PickupAnim = CollectableItemPickup.PickupAnimations.Stand;
				});
			}
		}
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x0009E98E File Offset: 0x0009CB8E
	public void SetInactive()
	{
		this.isInactive = true;
	}

	// Token: 0x0400213C RID: 8508
	[SerializeField]
	private FullQuestBase activeQuest;

	// Token: 0x0400213D RID: 8509
	[SerializeField]
	[Range(0f, 1f)]
	private float probability = 1f;

	// Token: 0x0400213E RID: 8510
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private CollectableItemPickup itemPickupPrefab;

	// Token: 0x0400213F RID: 8511
	[SerializeField]
	private SpriteFlash spriteFlash;

	// Token: 0x04002140 RID: 8512
	[SerializeField]
	private BreakablePoleTopLand poleTopLand;

	// Token: 0x04002141 RID: 8513
	private bool isInactive;
}
