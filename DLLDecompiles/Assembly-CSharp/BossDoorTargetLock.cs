using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000379 RID: 889
[DefaultExecutionOrder(1)]
public class BossDoorTargetLock : MonoBehaviour
{
	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06001E5F RID: 7775 RVA: 0x0008BED1 File Offset: 0x0008A0D1
	// (set) Token: 0x06001E60 RID: 7776 RVA: 0x0008BEF2 File Offset: 0x0008A0F2
	private bool IsUnlocked
	{
		get
		{
			return !string.IsNullOrEmpty(this.playerData) && GameManager.instance.GetPlayerDataBool(this.playerData);
		}
		set
		{
			if (!string.IsNullOrEmpty(this.playerData))
			{
				GameManager.instance.SetPlayerDataBool(this.playerData, value);
				return;
			}
			Debug.LogError("Can't save an empty PlayerData bool!", this);
		}
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x0008BF1E File Offset: 0x0008A11E
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06001E62 RID: 7778 RVA: 0x0008BF2C File Offset: 0x0008A12C
	private void Start()
	{
		bool flag = true;
		BossDoorTargetLock.BossDoorTarget[] array = this.targets;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Evaluate())
			{
				flag = false;
			}
		}
		if (this.IsUnlocked)
		{
			if (this.animator)
			{
				this.animator.Play(this.unlockedAnimation);
				return;
			}
		}
		else if (flag && this.unlockTrigger)
		{
			TriggerEnterEvent.CollisionEvent temp = null;
			temp = delegate(Collider2D collider, GameObject sender)
			{
				this.StartCoroutine(this.UnlockSequence());
				this.unlockTrigger.OnTriggerEntered -= temp;
			};
			this.unlockTrigger.OnTriggerEntered += temp;
		}
	}

	// Token: 0x06001E63 RID: 7779 RVA: 0x0008BFCA File Offset: 0x0008A1CA
	private IEnumerator UnlockSequence()
	{
		if (this.animator)
		{
			this.animator.Play(this.unlockAnimation);
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		this.IsUnlocked = true;
		yield break;
	}

	// Token: 0x06001E64 RID: 7780 RVA: 0x0008BFD9 File Offset: 0x0008A1D9
	private void StartRoarLock()
	{
		HeroController.instance.StartRoarLock();
	}

	// Token: 0x06001E65 RID: 7781 RVA: 0x0008BFE5 File Offset: 0x0008A1E5
	private void StopRoarLock()
	{
		HeroController.instance.StopRoarLock();
	}

	// Token: 0x04001D60 RID: 7520
	public BossDoorTargetLock.BossDoorTarget[] targets;

	// Token: 0x04001D61 RID: 7521
	public string playerData = "finalBossDoorUnlocked";

	// Token: 0x04001D62 RID: 7522
	public TriggerEnterEvent unlockTrigger;

	// Token: 0x04001D63 RID: 7523
	public string unlockAnimation = "Unlock";

	// Token: 0x04001D64 RID: 7524
	public string unlockedAnimation = "Unlocked";

	// Token: 0x04001D65 RID: 7525
	private Animator animator;

	// Token: 0x02001625 RID: 5669
	[Serializable]
	public class BossDoorTarget
	{
		// Token: 0x06008913 RID: 35091 RVA: 0x0027BDC8 File Offset: 0x00279FC8
		public bool Evaluate()
		{
			if (this.door && this.indicator)
			{
				bool completed = this.door.CurrentCompletion.completed;
				this.indicator.SetActive(completed);
				return completed;
			}
			return false;
		}

		// Token: 0x040089D1 RID: 35281
		public BossSequenceDoor door;

		// Token: 0x040089D2 RID: 35282
		public GameObject indicator;
	}
}
