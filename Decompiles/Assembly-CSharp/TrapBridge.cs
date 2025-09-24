using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200057B RID: 1403
public abstract class TrapBridge : MonoBehaviour
{
	// Token: 0x0600323A RID: 12858 RVA: 0x000E0054 File Offset: 0x000DE254
	public void Open()
	{
		if (this.openBehaviourRoutine != null)
		{
			this.openHoldTimeLeft = this.openHoldDuration;
			return;
		}
		this.openBehaviourRoutine = base.StartCoroutine(this.OpenBehaviour());
	}

	// Token: 0x0600323B RID: 12859 RVA: 0x000E007D File Offset: 0x000DE27D
	private IEnumerator OpenBehaviour()
	{
		if (this.anticDelay > 0f)
		{
			yield return new WaitForSeconds(this.anticDelay);
		}
		this.onOpenAntic.Invoke();
		if (this.openDelay > 0f)
		{
			yield return new WaitForSeconds(this.openDelay);
		}
		this.onOpen.Invoke();
		this.openSound.SpawnAndPlayOneShot(base.transform.position, null);
		yield return base.StartCoroutine(this.DoOpenAnim());
		this.onOpened.Invoke();
		if (this.insideTracker)
		{
			this.openHoldTimeLeft = this.openHoldDuration;
			while (this.openHoldTimeLeft > 0f)
			{
				if (this.insideTracker.IsInside)
				{
					this.openHoldTimeLeft = this.openHoldDuration;
				}
				else
				{
					this.openHoldTimeLeft -= Time.deltaTime;
				}
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(this.openHoldDuration);
		}
		this.onClose.Invoke();
		this.closeSound.SpawnAndPlayOneShot(base.transform.position, null);
		yield return base.StartCoroutine(this.DoCloseAnim());
		this.openBehaviourRoutine = null;
		yield break;
	}

	// Token: 0x0600323C RID: 12860
	protected abstract IEnumerator DoOpenAnim();

	// Token: 0x0600323D RID: 12861
	protected abstract IEnumerator DoCloseAnim();

	// Token: 0x040035E3 RID: 13795
	[SerializeField]
	private TrackTriggerObjects insideTracker;

	// Token: 0x040035E4 RID: 13796
	[SerializeField]
	private float anticDelay;

	// Token: 0x040035E5 RID: 13797
	[SerializeField]
	private float openDelay;

	// Token: 0x040035E6 RID: 13798
	[SerializeField]
	private float openHoldDuration;

	// Token: 0x040035E7 RID: 13799
	[SerializeField]
	private AudioEvent openSound;

	// Token: 0x040035E8 RID: 13800
	[SerializeField]
	private AudioEvent closeSound;

	// Token: 0x040035E9 RID: 13801
	[Space]
	[SerializeField]
	protected UnityEvent onOpenAntic;

	// Token: 0x040035EA RID: 13802
	[SerializeField]
	protected UnityEvent onOpen;

	// Token: 0x040035EB RID: 13803
	[SerializeField]
	protected UnityEvent onOpened;

	// Token: 0x040035EC RID: 13804
	[SerializeField]
	protected UnityEvent onClose;

	// Token: 0x040035ED RID: 13805
	private Coroutine openBehaviourRoutine;

	// Token: 0x040035EE RID: 13806
	private float openHoldTimeLeft;
}
