using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000762 RID: 1890
public class FlingChildrenOnStart : MonoBehaviour
{
	// Token: 0x0600434E RID: 17230 RVA: 0x00128518 File Offset: 0x00126718
	private void Start()
	{
		this.DoFling();
		this.hasStarted = true;
	}

	// Token: 0x0600434F RID: 17231 RVA: 0x00128527 File Offset: 0x00126727
	private void OnEnable()
	{
		if (this.hasStarted)
		{
			this.DoFling();
		}
	}

	// Token: 0x06004350 RID: 17232 RVA: 0x00128538 File Offset: 0x00126738
	public Transform GetParentParent()
	{
		if (this.checkedParentParent)
		{
			return this.parentParent;
		}
		if (!this.config.Parent)
		{
			this.config.Parent = base.gameObject;
		}
		if (!this.checkedParentParent)
		{
			this.checkedParentParent = true;
			this.parentParent = this.config.Parent.transform.parent;
		}
		return this.parentParent;
	}

	// Token: 0x06004351 RID: 17233 RVA: 0x001285A8 File Offset: 0x001267A8
	private void DoFling()
	{
		if (!this.config.Parent)
		{
			this.config.Parent = base.gameObject;
		}
		if (!this.checkedParentParent)
		{
			this.checkedParentParent = true;
			this.parentParent = this.config.Parent.transform.parent;
		}
		this.config.Parent.transform.SetParent(null, true);
		if (Math.Abs(this.randomiseZLocal.Start) > Mathf.Epsilon || Math.Abs(this.randomiseZLocal.End) > Mathf.Epsilon)
		{
			FlingUtils.FlingChildren(this.config, this.config.Parent.transform, Vector3.zero, new MinMaxFloat?(this.randomiseZLocal));
			return;
		}
		FlingUtils.FlingChildren(this.config, this.config.Parent.transform, Vector3.zero, null);
	}

	// Token: 0x0400450B RID: 17675
	[SerializeField]
	private FlingUtils.ChildrenConfig config = new FlingUtils.ChildrenConfig
	{
		SpeedMin = 15f,
		SpeedMax = 20f,
		AngleMin = 60f,
		AngleMax = 120f
	};

	// Token: 0x0400450C RID: 17676
	[SerializeField]
	private MinMaxFloat randomiseZLocal = new MinMaxFloat(0f, 0.001f);

	// Token: 0x0400450D RID: 17677
	private bool hasStarted;

	// Token: 0x0400450E RID: 17678
	private bool checkedParentParent;

	// Token: 0x0400450F RID: 17679
	private Transform parentParent;
}
