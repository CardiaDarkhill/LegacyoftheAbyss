using System;
using UnityEngine;

// Token: 0x020000C1 RID: 193
[RequireComponent(typeof(Animator))]
public class StopAnimatorsAtPoint : MonoBehaviour
{
	// Token: 0x06000617 RID: 1559 RVA: 0x0001F23D File Offset: 0x0001D43D
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x0001F24C File Offset: 0x0001D44C
	private void Start()
	{
		if (this.stopEvent)
		{
			this.stopEvent.ReceivedEvent += delegate()
			{
				this.shouldStop = true;
				this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			};
		}
		if (this.stopInstantEvent)
		{
			this.stopInstantEvent.ReceivedEvent += delegate()
			{
				this.animator.enabled = false;
				Vector3 localPosition = base.transform.localPosition;
				localPosition.y = this.stopInstantHeight;
				base.transform.localPosition = localPosition;
			};
		}
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x0001F2A1 File Offset: 0x0001D4A1
	public void SetCanStop()
	{
		this.canStop = true;
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x0001F2AA File Offset: 0x0001D4AA
	public void SetCannotStop()
	{
		this.canStop = false;
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x0001F2B3 File Offset: 0x0001D4B3
	private void Update()
	{
		if (this.shouldStop && this.canStop && this.animator.enabled)
		{
			this.animator.enabled = false;
			this.canStop = false;
			this.shouldStop = false;
		}
	}

	// Token: 0x040005E0 RID: 1504
	public EventRegister stopEvent;

	// Token: 0x040005E1 RID: 1505
	public EventRegister stopInstantEvent;

	// Token: 0x040005E2 RID: 1506
	private bool canStop;

	// Token: 0x040005E3 RID: 1507
	private bool shouldStop;

	// Token: 0x040005E4 RID: 1508
	public float stopInstantHeight = 1.75f;

	// Token: 0x040005E5 RID: 1509
	private Animator animator;
}
