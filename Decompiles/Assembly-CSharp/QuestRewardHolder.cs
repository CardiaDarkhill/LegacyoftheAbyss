using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001CE RID: 462
public class QuestRewardHolder : MonoBehaviour
{
	// Token: 0x06001205 RID: 4613 RVA: 0x00053E71 File Offset: 0x00052071
	private void OnEnable()
	{
		this.UpdateState(true);
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x00053E7C File Offset: 0x0005207C
	private void UpdateState(bool isInstant)
	{
		this.pickup.PickupAction -= this.OnItemPickup;
		if (PlayerData.instance.GetVariable(this.pickupPdBool))
		{
			this.animator.Play(QuestRewardHolder._emptyAnim);
			this.pickup.SetActive(false, isInstant);
			this.disableOnPickup.SetAllActive(false);
			return;
		}
		if (this.quest.IsCompleted)
		{
			this.animator.Play(QuestRewardHolder._openAnim);
			this.pickup.SetActive(true, isInstant);
		}
		else
		{
			this.animator.Play(QuestRewardHolder._closedAnim);
			this.pickup.SetActive(false, isInstant);
		}
		this.pickup.PickupAction += this.OnItemPickup;
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x00053F3C File Offset: 0x0005213C
	public void Refresh()
	{
		this.UpdateState(false);
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x00053F48 File Offset: 0x00052148
	private bool OnItemPickup()
	{
		SavedItem rewardItem = this.quest.RewardItem;
		if (rewardItem && !rewardItem.TryGet(false, true))
		{
			return false;
		}
		this.animator.Play(QuestRewardHolder._emptyAnim);
		this.disableOnPickup.SetAllActive(false);
		PlayerData.instance.SetVariable(this.pickupPdBool, true);
		this.OnPickedUp.Invoke();
		return true;
	}

	// Token: 0x040010D6 RID: 4310
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private FullQuestBase quest;

	// Token: 0x040010D7 RID: 4311
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string pickupPdBool;

	// Token: 0x040010D8 RID: 4312
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GenericPickup pickup;

	// Token: 0x040010D9 RID: 4313
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Animator animator;

	// Token: 0x040010DA RID: 4314
	[SerializeField]
	private GameObject[] disableOnPickup;

	// Token: 0x040010DB RID: 4315
	[Space]
	public UnityEvent OnPickedUp;

	// Token: 0x040010DC RID: 4316
	private static readonly int _emptyAnim = Animator.StringToHash("Empty");

	// Token: 0x040010DD RID: 4317
	private static readonly int _openAnim = Animator.StringToHash("Open");

	// Token: 0x040010DE RID: 4318
	private static readonly int _closedAnim = Animator.StringToHash("Closed");
}
