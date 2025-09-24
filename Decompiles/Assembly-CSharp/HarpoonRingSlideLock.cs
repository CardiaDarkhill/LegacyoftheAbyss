using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004F6 RID: 1270
public class HarpoonRingSlideLock : MonoBehaviour
{
	// Token: 0x06002D74 RID: 11636 RVA: 0x000C6604 File Offset: 0x000C4804
	private void OnDrawGizmosSelected()
	{
		if (!this.ring)
		{
			return;
		}
		if (this.ring.parent)
		{
			Gizmos.matrix = this.ring.parent.localToWorldMatrix;
		}
		Vector3 localPosition = this.ring.localPosition;
		Vector3 vector = localPosition + this.dropOffset;
		Gizmos.DrawWireSphere(vector, 0.1f);
		Gizmos.DrawLine(localPosition, vector);
	}

	// Token: 0x06002D75 RID: 11637 RVA: 0x000C6674 File Offset: 0x000C4874
	private void Awake()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.ringPrefab, this.ring);
		gameObject.transform.localPosition = new Vector3(0f, -0.46f, 0f);
		gameObject.transform.SetPositionZ(this.ringPrefab.transform.position.z);
		Transform transform = gameObject.transform.Find("Backing");
		if (transform)
		{
			transform.gameObject.SetActive(false);
		}
		this.initialRingPos = this.ring.localPosition;
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isComplete;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.isComplete = value;
				if (this.isComplete)
				{
					this.ring.localPosition = this.initialRingPos + this.dropOffset;
					UnlockablePropBase[] array = this.unlockables;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Opened();
					}
				}
			};
		}
	}

	// Token: 0x06002D76 RID: 11638 RVA: 0x000C6741 File Offset: 0x000C4941
	public void HeroOnRing()
	{
		if (this.isComplete || this.dropRoutine != null)
		{
			return;
		}
		this.dropRoutine = base.StartCoroutine(this.DropSequence());
	}

	// Token: 0x06002D77 RID: 11639 RVA: 0x000C6766 File Offset: 0x000C4966
	public void HeroOffRing()
	{
		if (this.isComplete)
		{
			return;
		}
		if (this.dropRoutine != null)
		{
			base.StopCoroutine(this.dropRoutine);
			this.dropRoutine = null;
		}
	}

	// Token: 0x06002D78 RID: 11640 RVA: 0x000C678C File Offset: 0x000C498C
	private IEnumerator DropSequence()
	{
		yield return new WaitForSeconds(this.dropDelay);
		this.BeforeDrop.Invoke();
		this.isComplete = true;
		Vector3 targetRingPos = this.initialRingPos + this.dropOffset;
		for (float elapsed = 0f; elapsed < this.dropDuration; elapsed += Time.deltaTime)
		{
			float t = this.dropCurve.Evaluate(elapsed / this.dropDuration);
			this.ring.localPosition = Vector3.Lerp(this.initialRingPos, targetRingPos, t);
			yield return null;
		}
		this.ring.localPosition = targetRingPos;
		this.dropImpactShake.DoShake(this, true);
		this.Dropped.Invoke();
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "RING DROP IMPACT", false);
		yield return new WaitForSeconds(this.unlockDelay);
		UnlockablePropBase[] array = this.unlockables;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Open();
		}
		this.dropRoutine = null;
		yield break;
	}

	// Token: 0x04002F26 RID: 12070
	[SerializeField]
	private GameObject ringPrefab;

	// Token: 0x04002F27 RID: 12071
	[SerializeField]
	private Transform ring;

	// Token: 0x04002F28 RID: 12072
	[SerializeField]
	private Vector2 dropOffset;

	// Token: 0x04002F29 RID: 12073
	[SerializeField]
	private float dropDelay;

	// Token: 0x04002F2A RID: 12074
	[SerializeField]
	private AnimationCurve dropCurve;

	// Token: 0x04002F2B RID: 12075
	[SerializeField]
	private float dropDuration;

	// Token: 0x04002F2C RID: 12076
	[SerializeField]
	private CameraShakeTarget dropImpactShake;

	// Token: 0x04002F2D RID: 12077
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04002F2E RID: 12078
	[SerializeField]
	private float unlockDelay;

	// Token: 0x04002F2F RID: 12079
	[SerializeField]
	private UnlockablePropBase[] unlockables;

	// Token: 0x04002F30 RID: 12080
	[Space]
	public UnityEvent BeforeDrop;

	// Token: 0x04002F31 RID: 12081
	public UnityEvent Dropped;

	// Token: 0x04002F32 RID: 12082
	private bool isComplete;

	// Token: 0x04002F33 RID: 12083
	private Vector3 initialRingPos;

	// Token: 0x04002F34 RID: 12084
	private Coroutine dropRoutine;
}
