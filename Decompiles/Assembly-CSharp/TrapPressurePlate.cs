using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200057F RID: 1407
public class TrapPressurePlate : MonoBehaviour
{
	// Token: 0x0600326D RID: 12909 RVA: 0x000E085C File Offset: 0x000DEA5C
	private void Awake()
	{
		if (this.tracker)
		{
			this.tracker.InsideStateChanged += this.OnInsideStateChanged;
		}
	}

	// Token: 0x0600326E RID: 12910 RVA: 0x000E0882 File Offset: 0x000DEA82
	private void OnInsideStateChanged(bool isInside)
	{
		if (!this.plate)
		{
			return;
		}
		if (isInside)
		{
			this.canReturn = false;
			if (this.dropRoutine == null)
			{
				this.dropRoutine = base.StartCoroutine(this.Drop());
				return;
			}
		}
		else
		{
			this.canReturn = true;
		}
	}

	// Token: 0x0600326F RID: 12911 RVA: 0x000E08BE File Offset: 0x000DEABE
	private IEnumerator Drop()
	{
		float platePos = this.plate.transform.localPosition.y;
		float bottomPlatePos = platePos + this.dropOffset;
		float elapsed;
		for (elapsed = 0f; elapsed < this.dropTime; elapsed += Time.deltaTime)
		{
			this.plate.SetLocalPositionY(Mathf.Lerp(platePos, bottomPlatePos, elapsed / this.dropTime));
			yield return null;
		}
		this.plate.SetLocalPositionY(bottomPlatePos);
		this.dropShake.DoShake(this, true);
		this.dropAudio.SpawnAndPlayOneShot(base.transform.position, null);
		this.OnPressed.Invoke();
		elapsed = 0f;
		while (elapsed < this.raiseDelay)
		{
			yield return null;
			if (this.canReturn && !this.isBlocked)
			{
				elapsed += Time.deltaTime;
			}
			else
			{
				elapsed = 0f;
			}
		}
		this.riseAudio.SpawnAndPlayOneShot(base.transform.position, null);
		for (elapsed = 0f; elapsed < this.raiseTime; elapsed += Time.deltaTime)
		{
			this.plate.SetLocalPositionY(Mathf.Lerp(bottomPlatePos, platePos, elapsed / this.raiseTime));
			yield return null;
		}
		this.plate.SetLocalPositionY(platePos);
		this.dropRoutine = null;
		yield break;
	}

	// Token: 0x06003270 RID: 12912 RVA: 0x000E08CD File Offset: 0x000DEACD
	public void SetBlocked(bool value)
	{
		this.isBlocked = value;
	}

	// Token: 0x04003623 RID: 13859
	[SerializeField]
	private TrackTriggerObjects tracker;

	// Token: 0x04003624 RID: 13860
	[SerializeField]
	private Transform plate;

	// Token: 0x04003625 RID: 13861
	[SerializeField]
	private float dropOffset;

	// Token: 0x04003626 RID: 13862
	[SerializeField]
	private float dropTime;

	// Token: 0x04003627 RID: 13863
	[SerializeField]
	private float raiseDelay;

	// Token: 0x04003628 RID: 13864
	[SerializeField]
	private float raiseTime;

	// Token: 0x04003629 RID: 13865
	[SerializeField]
	private CameraShakeTarget dropShake;

	// Token: 0x0400362A RID: 13866
	[SerializeField]
	private AudioEvent dropAudio;

	// Token: 0x0400362B RID: 13867
	[SerializeField]
	private AudioEvent riseAudio;

	// Token: 0x0400362C RID: 13868
	[Space]
	public UnityEvent OnPressed;

	// Token: 0x0400362D RID: 13869
	private Coroutine dropRoutine;

	// Token: 0x0400362E RID: 13870
	private bool canReturn;

	// Token: 0x0400362F RID: 13871
	private bool isBlocked;
}
