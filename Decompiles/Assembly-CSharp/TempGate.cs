using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200056C RID: 1388
public class TempGate : MonoBehaviour
{
	// Token: 0x0600319A RID: 12698 RVA: 0x000DC3D8 File Offset: 0x000DA5D8
	private void OnDrawGizmos()
	{
		if (this.plate)
		{
			Gizmos.DrawLine(base.transform.position, this.plate.transform.position);
		}
	}

	// Token: 0x0600319B RID: 12699 RVA: 0x000DC407 File Offset: 0x000DA607
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x0600319C RID: 12700 RVA: 0x000DC418 File Offset: 0x000DA618
	private void Start()
	{
		if (this.plate)
		{
			this.plate.Activated += this.OnPlateActivated;
		}
		HeroController hc = HeroController.instance;
		if (hc.isHeroInPosition)
		{
			this.SetStartingState();
			return;
		}
		HeroController.HeroInPosition temp = null;
		temp = delegate(bool _)
		{
			this.SetStartingState();
			hc.heroInPosition -= temp;
		};
		hc.heroInPosition += temp;
	}

	// Token: 0x0600319D RID: 12701 RVA: 0x000DC4A0 File Offset: 0x000DA6A0
	private void SetStartingState()
	{
		if (!this.animator)
		{
			Debug.LogError("No animator found!", this);
			return;
		}
		bool flag = !string.IsNullOrEmpty(this.brokenPDBool) && PlayerData.instance.GetVariable(this.brokenPDBool);
		bool flag2 = flag || this.keepOpenRange.IsInside;
		string text = (!string.IsNullOrEmpty(this.openedAnim)) ? this.openedAnim : this.openAnim;
		string text2 = (!string.IsNullOrEmpty(this.closedAnim)) ? this.closedAnim : this.closeAnim;
		this.animator.Play(flag2 ? text : text2, 0, 1f);
		this.animator.Update(0f);
		IBeginStopper[] componentsInChildren = base.GetComponentsInChildren<IBeginStopper>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DoBeginStop();
		}
		if (this.readPersistent && !this.readPersistent.GetCurrentValue())
		{
			this.plate.ActivateSilent();
		}
		else if (flag2)
		{
			this.plate.ActivateSilent();
		}
		if (!flag)
		{
			this.updateRoutine = base.StartCoroutine(this.UpdateState(flag2));
		}
	}

	// Token: 0x0600319E RID: 12702 RVA: 0x000DC5C9 File Offset: 0x000DA7C9
	private void OnPlateActivated()
	{
		this.queueOpen = true;
	}

	// Token: 0x0600319F RID: 12703 RVA: 0x000DC5D2 File Offset: 0x000DA7D2
	private IEnumerator UpdateState(bool isOpen)
	{
		for (;;)
		{
			if (isOpen)
			{
				float closeDelayLeft = this.closeDelay + this.keepOpenMinTime;
				while (closeDelayLeft > 0f)
				{
					if (this.keepOpenRange.IsInside && (this.keepOpenMinTime <= 0f || closeDelayLeft < this.keepOpenMinTime))
					{
						closeDelayLeft = this.closeDelay;
					}
					else
					{
						closeDelayLeft -= Time.deltaTime;
					}
					yield return null;
				}
				this.animator.Play(this.closeAnim, 0, 0f);
				yield return null;
				yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
				isOpen = false;
				this.plate.Deactivate();
			}
			else
			{
				while (!this.queueOpen)
				{
					yield return null;
				}
				this.queueOpen = false;
				if (this.openDelay > 0f)
				{
					yield return new WaitForSeconds(this.openDelay);
				}
				this.animator.Play(this.openAnim, 0, 0f);
				yield return null;
				yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
				isOpen = true;
			}
		}
		yield break;
	}

	// Token: 0x060031A0 RID: 12704 RVA: 0x000DC5E8 File Offset: 0x000DA7E8
	public void SetBroken()
	{
		if (this.updateRoutine != null)
		{
			base.StopCoroutine(this.updateRoutine);
		}
		if (!string.IsNullOrEmpty(this.brokenPDBool))
		{
			PlayerData.instance.SetVariable(this.brokenPDBool, true);
		}
		this.animator.Play(this.openAnim, 0, 1f);
	}

	// Token: 0x04003503 RID: 13571
	[SerializeField]
	private string openAnim;

	// Token: 0x04003504 RID: 13572
	[SerializeField]
	private string openedAnim;

	// Token: 0x04003505 RID: 13573
	[SerializeField]
	private string closeAnim;

	// Token: 0x04003506 RID: 13574
	[SerializeField]
	private string closedAnim;

	// Token: 0x04003507 RID: 13575
	[SerializeField]
	private float openDelay;

	// Token: 0x04003508 RID: 13576
	[SerializeField]
	private float closeDelay;

	// Token: 0x04003509 RID: 13577
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private TempPressurePlate plate;

	// Token: 0x0400350A RID: 13578
	[SerializeField]
	private TrackTriggerObjects keepOpenRange;

	// Token: 0x0400350B RID: 13579
	[SerializeField]
	private float keepOpenMinTime;

	// Token: 0x0400350C RID: 13580
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string brokenPDBool;

	// Token: 0x0400350D RID: 13581
	[SerializeField]
	private PersistentBoolItem readPersistent;

	// Token: 0x0400350E RID: 13582
	private Animator animator;

	// Token: 0x0400350F RID: 13583
	private AudioSource source;

	// Token: 0x04003510 RID: 13584
	private bool queueOpen;

	// Token: 0x04003511 RID: 13585
	private Coroutine updateRoutine;
}
