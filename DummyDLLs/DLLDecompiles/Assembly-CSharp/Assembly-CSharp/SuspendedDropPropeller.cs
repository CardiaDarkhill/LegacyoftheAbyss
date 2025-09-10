using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000567 RID: 1383
public class SuspendedDropPropeller : SuspendedPlatformBase
{
	// Token: 0x0600317A RID: 12666 RVA: 0x000DBCFC File Offset: 0x000D9EFC
	protected override void Awake()
	{
		base.Awake();
		if (this.inactivePropeller)
		{
			this.inactivePropeller.SetActive(true);
		}
		if (this.activePropeller)
		{
			this.activePropeller.SetActive(false);
		}
		this.dropTarget.gameObject.SetActive(false);
	}

	// Token: 0x0600317B RID: 12667 RVA: 0x000DBD52 File Offset: 0x000D9F52
	public override void CutDown()
	{
		base.CutDown();
		this.StopDrop();
		this.dropRoutine = base.StartCoroutine(this.DropDown());
	}

	// Token: 0x0600317C RID: 12668 RVA: 0x000DBD74 File Offset: 0x000D9F74
	protected override void OnStartActivated()
	{
		base.OnStartActivated();
		this.StopDrop();
		this.platDrop.SetPosition2D(this.dropTarget.position);
		if (this.inactivePropeller)
		{
			this.inactivePropeller.SetActive(false);
		}
		if (this.activePropeller)
		{
			this.activePropeller.SetActive(true);
		}
		if (this.updraft)
		{
			this.updraft.SendEvent("ACTIVATE");
		}
		UnityEvent onStartedActivated = this.OnStartedActivated;
		if (onStartedActivated == null)
		{
			return;
		}
		onStartedActivated.Invoke();
	}

	// Token: 0x0600317D RID: 12669 RVA: 0x000DBE07 File Offset: 0x000DA007
	private void StopDrop()
	{
		if (this.dropRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.dropRoutine);
		this.dropRoutine = null;
	}

	// Token: 0x0600317E RID: 12670 RVA: 0x000DBE25 File Offset: 0x000DA025
	private IEnumerator DropDown()
	{
		UnityEvent onDropStart = this.OnDropStart;
		if (onDropStart != null)
		{
			onDropStart.Invoke();
		}
		Vector2 endPos = this.dropTarget.position;
		float speed = 0f;
		while (this.platDrop.position.y > endPos.y)
		{
			speed += this.gravity * Time.deltaTime;
			if (speed > this.maxSpeed)
			{
				speed = this.maxSpeed;
			}
			Vector3 position = this.platDrop.position;
			position.y -= speed * Time.deltaTime;
			this.platDrop.SetPosition2D(position);
			yield return null;
		}
		this.platDrop.SetPosition2D(endPos);
		foreach (object obj in this.dropTarget)
		{
			Transform transform = (Transform)obj;
			transform.SetParent(null, true);
			transform.gameObject.SetActive(true);
		}
		this.impactShake.DoShake(this, true);
		UnityEvent onDropImpact = this.OnDropImpact;
		if (onDropImpact != null)
		{
			onDropImpact.Invoke();
		}
		yield return new WaitForSeconds(this.propellerStartDelay);
		if (this.inactivePropeller)
		{
			this.inactivePropeller.SetActive(false);
		}
		if (this.activePropeller)
		{
			this.activePropeller.SetActive(true);
		}
		if (this.updraft)
		{
			this.updraft.SendEvent("ACTIVATE");
		}
		yield break;
	}

	// Token: 0x040034CE RID: 13518
	[SerializeField]
	private float gravity;

	// Token: 0x040034CF RID: 13519
	[SerializeField]
	private float maxSpeed;

	// Token: 0x040034D0 RID: 13520
	[SerializeField]
	private Transform platDrop;

	// Token: 0x040034D1 RID: 13521
	[SerializeField]
	private Transform dropTarget;

	// Token: 0x040034D2 RID: 13522
	[SerializeField]
	private PlayMakerFSM updraft;

	// Token: 0x040034D3 RID: 13523
	[SerializeField]
	private float propellerStartDelay;

	// Token: 0x040034D4 RID: 13524
	[SerializeField]
	private GameObject inactivePropeller;

	// Token: 0x040034D5 RID: 13525
	[SerializeField]
	private GameObject activePropeller;

	// Token: 0x040034D6 RID: 13526
	[SerializeField]
	private CameraShakeTarget impactShake;

	// Token: 0x040034D7 RID: 13527
	[Space]
	public UnityEvent OnDropStart;

	// Token: 0x040034D8 RID: 13528
	public UnityEvent OnDropImpact;

	// Token: 0x040034D9 RID: 13529
	public UnityEvent OnStartedActivated;

	// Token: 0x040034DA RID: 13530
	private Coroutine dropRoutine;
}
