using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200039C RID: 924
public class BossStatueExtraFlashEffect : MonoBehaviour
{
	// Token: 0x06001F31 RID: 7985 RVA: 0x0008EA94 File Offset: 0x0008CC94
	private void Start()
	{
		BossStatue componentInParent = base.GetComponentInParent<BossStatue>();
		if (componentInParent && !componentInParent.DreamStatueState.hasBeenSeen && !componentInParent.isAlwaysUnlockedDream)
		{
			this.toggle.SetActive(false);
			if (componentInParent.DreamStatueState.isUnlocked)
			{
				if (componentInParent.StatueState.isUnlocked && !componentInParent.StatueState.hasBeenSeen && this.mainEffect)
				{
					this.mainEffect.OnFlashBegin += delegate()
					{
						base.Invoke("DoAppear", this.flashSequenceDelay);
					};
				}
				else if (this.triggerUnlockEvent)
				{
					TriggerEnterEvent.CollisionEvent temp = null;
					temp = (TriggerEnterEvent.CollisionEvent)Delegate.Combine(temp, new TriggerEnterEvent.CollisionEvent(delegate(Collider2D collision, GameObject sender)
					{
						this.DoAppear();
						this.triggerUnlockEvent.OnTriggerEntered -= temp;
					}));
					this.triggerUnlockEvent.OnTriggerEntered += temp;
				}
			}
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x0008EB8A File Offset: 0x0008CD8A
	private void DoAppear()
	{
		base.gameObject.SetActive(true);
		base.StartCoroutine(this.AppearRoutine(this.toggle));
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x0008EBAB File Offset: 0x0008CDAB
	private IEnumerator AppearRoutine(GameObject toggle)
	{
		yield return new WaitForSeconds(this.toggleEnableTime);
		toggle.SetActive(true);
		yield break;
	}

	// Token: 0x04001E20 RID: 7712
	public BossStatueFlashEffect mainEffect;

	// Token: 0x04001E21 RID: 7713
	public float flashSequenceDelay = 2f;

	// Token: 0x04001E22 RID: 7714
	public TriggerEnterEvent triggerUnlockEvent;

	// Token: 0x04001E23 RID: 7715
	public float toggleEnableTime = 1.35f;

	// Token: 0x04001E24 RID: 7716
	public GameObject toggle;
}
