using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000349 RID: 841
public class DropRecycle : MonoBehaviour
{
	// Token: 0x06001D39 RID: 7481 RVA: 0x000873B1 File Offset: 0x000855B1
	private void Awake()
	{
		this.colliders = base.GetComponentsInChildren<Collider2D>(true);
		this.enabledStates = new bool[this.colliders.Length];
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x000873D3 File Offset: 0x000855D3
	private void OnEnable()
	{
		if (this.hasStarted && !this.waitForCall)
		{
			this.StartDrop();
		}
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x000873EB File Offset: 0x000855EB
	private void Start()
	{
		if (!this.waitForCall)
		{
			this.StartDrop();
		}
		this.hasStarted = true;
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x00087402 File Offset: 0x00085602
	private void OnDisable()
	{
		if (this.dropRoutine != null)
		{
			base.StopCoroutine(this.dropRoutine);
			this.dropRoutine = null;
		}
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x00087420 File Offset: 0x00085620
	public static void AddInactive(GameObject gameObject)
	{
		DropRecycle dropRecycle = gameObject.GetComponent<DropRecycle>();
		if (!dropRecycle)
		{
			dropRecycle = gameObject.AddComponent<DropRecycle>();
		}
		dropRecycle.waitForCall = true;
		Collider2D component = gameObject.GetComponent<Collider2D>();
		if (component)
		{
			dropRecycle.dropDuration = 0f;
			dropRecycle.dropDistance = component.bounds.size.y + 0.5f;
			return;
		}
		dropRecycle.dropDuration = 2f;
		dropRecycle.dropDistance = 0f;
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x0008749A File Offset: 0x0008569A
	public void StartDrop()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.dropRoutine == null)
		{
			this.dropRoutine = base.StartCoroutine(this.DropTimer());
		}
	}

	// Token: 0x06001D3F RID: 7487 RVA: 0x000874BF File Offset: 0x000856BF
	private IEnumerator DropTimer()
	{
		for (int i = 0; i < this.colliders.Length; i++)
		{
			this.enabledStates[i] = this.colliders[i].enabled;
		}
		if (this.waitForBodySleep)
		{
			float elapsed = 0f;
			while (elapsed < this.dropDelay)
			{
				yield return null;
				if (this.waitForBodySleep.IsSleeping())
				{
					elapsed += Time.deltaTime;
				}
				else
				{
					elapsed = 0f;
				}
			}
		}
		else
		{
			yield return new WaitForSeconds(this.dropDelay);
		}
		Collider2D[] array = this.colliders;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].enabled = false;
		}
		float dropTimeLeft = this.dropDuration;
		float startY = base.transform.position.y;
		do
		{
			yield return null;
			if (this.dropDuration > 0f)
			{
				dropTimeLeft -= Time.deltaTime;
				if (dropTimeLeft <= 0f)
				{
					break;
				}
			}
		}
		while (this.dropDistance <= 0f || startY - base.transform.position.y < this.dropDistance);
		for (int k = 0; k < this.colliders.Length; k++)
		{
			this.colliders[k].enabled = this.enabledStates[k];
		}
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x04001C7C RID: 7292
	[SerializeField]
	private float dropDelay;

	// Token: 0x04001C7D RID: 7293
	[SerializeField]
	private float dropDuration;

	// Token: 0x04001C7E RID: 7294
	[SerializeField]
	private float dropDistance;

	// Token: 0x04001C7F RID: 7295
	[SerializeField]
	private Rigidbody2D waitForBodySleep;

	// Token: 0x04001C80 RID: 7296
	[SerializeField]
	private bool waitForCall;

	// Token: 0x04001C81 RID: 7297
	private bool hasStarted;

	// Token: 0x04001C82 RID: 7298
	private Coroutine dropRoutine;

	// Token: 0x04001C83 RID: 7299
	private Collider2D[] colliders;

	// Token: 0x04001C84 RID: 7300
	private bool[] enabledStates;
}
