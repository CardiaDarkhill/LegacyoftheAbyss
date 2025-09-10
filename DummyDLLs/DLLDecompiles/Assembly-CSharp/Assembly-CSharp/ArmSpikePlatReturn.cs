using System;
using UnityEngine;

// Token: 0x02000490 RID: 1168
public class ArmSpikePlatReturn : ArmSpikePlat, IBinarySwitchable
{
	// Token: 0x06002A24 RID: 10788 RVA: 0x000B6A88 File Offset: 0x000B4C88
	private void Update()
	{
		if (this.returnDelayLeft > 0f)
		{
			this.returnDelayLeft -= Time.deltaTime;
			if (this.returnDelayLeft <= 0f)
			{
				base.DoRotate(this.landStartDelay, -base.PreviousDirection, false);
			}
		}
	}

	// Token: 0x06002A25 RID: 10789 RVA: 0x000B6AD5 File Offset: 0x000B4CD5
	protected override void OnActivate()
	{
		base.OnActivate();
		this.returnDelayLeft = 0f;
	}

	// Token: 0x06002A26 RID: 10790 RVA: 0x000B6AE8 File Offset: 0x000B4CE8
	protected override void OnEnd()
	{
		base.OnEnd();
		if (this.willReturn)
		{
			this.willReturn = false;
		}
		else
		{
			this.returnDelayLeft = this.returnDelay;
			this.willReturn = true;
		}
		if (this.queueChangeState)
		{
			this.queueChangeState = false;
			this.ChangeDefaultState();
		}
	}

	// Token: 0x06002A27 RID: 10791 RVA: 0x000B6B34 File Offset: 0x000B4D34
	public void SwitchBinaryState(bool value)
	{
		bool flag = this.binaryState;
		this.binaryState = value;
		if (this.binaryState == flag)
		{
			return;
		}
		if (this.returnDelayLeft > 0f)
		{
			this.returnDelayLeft = 0f;
			this.willReturn = false;
			return;
		}
		if (base.IsRotating)
		{
			this.queueChangeState = true;
			return;
		}
		this.ChangeDefaultState();
	}

	// Token: 0x06002A28 RID: 10792 RVA: 0x000B6B8F File Offset: 0x000B4D8F
	private void ChangeDefaultState()
	{
		this.willReturn = true;
		base.DoRotate(0f, base.PreviousDirection, false);
	}

	// Token: 0x04002A98 RID: 10904
	[Header("Return")]
	[SerializeField]
	private float returnDelay;

	// Token: 0x04002A99 RID: 10905
	private float returnDelayLeft;

	// Token: 0x04002A9A RID: 10906
	private bool willReturn;

	// Token: 0x04002A9B RID: 10907
	private bool binaryState;

	// Token: 0x04002A9C RID: 10908
	private bool queueChangeState;
}
