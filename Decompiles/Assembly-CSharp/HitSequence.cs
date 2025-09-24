using System;
using System.Collections;
using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000503 RID: 1283
public class HitSequence : MonoBehaviour
{
	// Token: 0x06002DE9 RID: 11753 RVA: 0x000C91C4 File Offset: 0x000C73C4
	private void OnDrawGizmos()
	{
		if (this.hitOrder == null)
		{
			return;
		}
		for (int i = 1; i < this.hitOrder.Length; i++)
		{
			HitRigidbody2D hitRigidbody2D = this.hitOrder[i];
			HitRigidbody2D hitRigidbody2D2 = this.hitOrder[i - 1];
			if (hitRigidbody2D && hitRigidbody2D2)
			{
				Gizmos.DrawLine(hitRigidbody2D.transform.position, hitRigidbody2D2.transform.position);
			}
		}
	}

	// Token: 0x06002DEA RID: 11754 RVA: 0x000C922C File Offset: 0x000C742C
	private void Awake()
	{
		for (int i = 0; i < this.hitOrder.Length; i++)
		{
			HitRigidbody2D hitRigidbody2D = this.hitOrder[i];
			int capturedIndex = i;
			hitRigidbody2D.WasHitBy += delegate(HitInstance hitInstance)
			{
				this.OnObjHit(capturedIndex, hitInstance);
			};
		}
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x000C927C File Offset: 0x000C747C
	private void OnObjHit(int index, HitInstance hitInstance)
	{
		if (this.isComplete)
		{
			return;
		}
		if (!hitInstance.IsFirstHit)
		{
			return;
		}
		if (index != this.nextHitIndex)
		{
			this.CancelSequence();
			return;
		}
		if (this.requireHitWith && hitInstance.RepresentingTool != this.requireHitWith)
		{
			this.CancelSequence();
			return;
		}
		if (this.requireSameHitSource && this.previousHitSource && hitInstance.Source != this.previousHitSource)
		{
			this.CancelSequence();
			return;
		}
		this.previousHitSource = hitInstance.Source;
		this.nextHitIndex++;
		if (this.nextHitIndex < this.hitOrder.Length)
		{
			return;
		}
		this.isComplete = true;
		base.StartCoroutine(this.CompleteDelayed());
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x000C9340 File Offset: 0x000C7540
	private void CancelSequence()
	{
		this.nextHitIndex = 0;
		this.previousHitSource = null;
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x000C9350 File Offset: 0x000C7550
	private IEnumerator CompleteDelayed()
	{
		yield return new WaitForSeconds(this.sequenceCompleteDelay);
		this.OnSequenceComplete.Invoke();
		this.completeSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		if (!string.IsNullOrEmpty(this.completeEvent))
		{
			EventRegister.SendEvent(this.completeEvent, null);
		}
		yield break;
	}

	// Token: 0x04002FFE RID: 12286
	[SerializeField]
	private HitRigidbody2D[] hitOrder;

	// Token: 0x04002FFF RID: 12287
	[SerializeField]
	private ToolItem requireHitWith;

	// Token: 0x04003000 RID: 12288
	[SerializeField]
	private bool requireSameHitSource;

	// Token: 0x04003001 RID: 12289
	[Space]
	[SerializeField]
	private float sequenceCompleteDelay;

	// Token: 0x04003002 RID: 12290
	[SerializeField]
	private AudioEvent completeSound;

	// Token: 0x04003003 RID: 12291
	[SerializeField]
	private string completeEvent;

	// Token: 0x04003004 RID: 12292
	[Space]
	public UnityEvent OnSequenceComplete;

	// Token: 0x04003005 RID: 12293
	private int nextHitIndex;

	// Token: 0x04003006 RID: 12294
	private GameObject previousHitSource;

	// Token: 0x04003007 RID: 12295
	private bool isComplete;
}
