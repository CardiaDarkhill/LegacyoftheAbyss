using System;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class TipOverObject : MonoBehaviour
{
	// Token: 0x06000649 RID: 1609 RVA: 0x000205D4 File Offset: 0x0001E7D4
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		Transform transform = base.transform.Find("Sprite");
		if (transform != null && transform.transform.localPosition.x < 0f)
		{
			this.fallRight = true;
		}
		this.cameraReceiver.ReceivedEvent += this.ShakeEvent;
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x0002063C File Offset: 0x0001E83C
	private void ShakeEvent()
	{
		if (!this.didFall && this.active && (float)Random.Range(1, 100) <= this.tipChance)
		{
			this.DoTipOver();
		}
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x00020665 File Offset: 0x0001E865
	public void DoTipOver()
	{
		if (!this.didFall)
		{
			if (this.fallRight)
			{
				this.animator.Play("Tip Over Right");
			}
			else
			{
				this.animator.Play("Tip Over Left");
			}
			this.didFall = true;
		}
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x000206A0 File Offset: 0x0001E8A0
	public void SetActive(bool set_active)
	{
		this.active = set_active;
	}

	// Token: 0x04000622 RID: 1570
	public float tipChance;

	// Token: 0x04000623 RID: 1571
	public EventBase cameraReceiver;

	// Token: 0x04000624 RID: 1572
	public bool active = true;

	// Token: 0x04000625 RID: 1573
	private bool fallRight;

	// Token: 0x04000626 RID: 1574
	private bool didFall;

	// Token: 0x04000627 RID: 1575
	private Animator animator;
}
