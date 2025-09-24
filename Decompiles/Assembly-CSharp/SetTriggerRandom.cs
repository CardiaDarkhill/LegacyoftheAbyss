using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000AC RID: 172
public class SetTriggerRandom : MonoBehaviour
{
	// Token: 0x0600051A RID: 1306 RVA: 0x0001A710 File Offset: 0x00018910
	private void Awake()
	{
		if (this.animators == null || this.animators.Length == 0)
		{
			Animator component = base.GetComponent<Animator>();
			this.animators = new Animator[]
			{
				component
			};
		}
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x0001A745 File Offset: 0x00018945
	private void OnEnable()
	{
		this.routine = base.StartCoroutine(this.TriggerRoutine());
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x0001A759 File Offset: 0x00018959
	private void OnDisable()
	{
		if (this.routine != null)
		{
			base.StopCoroutine(this.routine);
		}
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0001A76F File Offset: 0x0001896F
	private IEnumerator TriggerRoutine()
	{
		for (;;)
		{
			float seconds = Random.Range(this.minInterval, this.maxInterval);
			yield return new WaitForSeconds(seconds);
			bool flag = false;
			foreach (SetTriggerRandom.TriggerAllowRegion triggerAllowRegion in this.triggerAllowRegions)
			{
				if (triggerAllowRegion.Trigger.IsInside && !triggerAllowRegion.RequireActive.activeInHierarchy)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (Animator animator in this.animators)
				{
					if (animator && animator.isActiveAndEnabled)
					{
						animator.SetTrigger(this.trigger);
					}
				}
				if (this.onTriggerSet != null)
				{
					this.onTriggerSet.Invoke();
				}
			}
		}
		yield break;
	}

	// Token: 0x040004F5 RID: 1269
	[SerializeField]
	private string trigger = "Shine";

	// Token: 0x040004F6 RID: 1270
	[SerializeField]
	private float minInterval = 0.5f;

	// Token: 0x040004F7 RID: 1271
	[SerializeField]
	private float maxInterval = 1.5f;

	// Token: 0x040004F8 RID: 1272
	[SerializeField]
	private Animator[] animators;

	// Token: 0x040004F9 RID: 1273
	[Space]
	[SerializeField]
	private UnityEvent onTriggerSet;

	// Token: 0x040004FA RID: 1274
	[Space]
	[SerializeField]
	private SetTriggerRandom.TriggerAllowRegion[] triggerAllowRegions;

	// Token: 0x040004FB RID: 1275
	private Coroutine routine;

	// Token: 0x0200141A RID: 5146
	[Serializable]
	private class TriggerAllowRegion
	{
		// Token: 0x040081EA RID: 33258
		public TrackTriggerObjects Trigger;

		// Token: 0x040081EB RID: 33259
		public GameObject RequireActive;
	}
}
