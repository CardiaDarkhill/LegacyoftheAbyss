using System;
using UnityEngine;

// Token: 0x020003FA RID: 1018
public class KeepWorldScalePositive : MonoBehaviour, IUpdateBatchableUpdate
{
	// Token: 0x17000398 RID: 920
	// (get) Token: 0x060022B4 RID: 8884 RVA: 0x0009F7E5 File Offset: 0x0009D9E5
	public bool ShouldUpdate
	{
		get
		{
			return this.everyFrame;
		}
	}

	// Token: 0x060022B5 RID: 8885 RVA: 0x0009F7ED File Offset: 0x0009D9ED
	private void OnEnable()
	{
		if (this.everyFrame)
		{
			this.updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
			this.updateBatcher.Add(this);
			return;
		}
		this.UpdateScale();
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x0009F81A File Offset: 0x0009DA1A
	private void OnDisable()
	{
		if (this.updateBatcher != null)
		{
			this.updateBatcher.Remove(this);
			this.updateBatcher = null;
		}
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x0009F83E File Offset: 0x0009DA3E
	public void BatchedUpdate()
	{
		this.UpdateScale();
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x0009F848 File Offset: 0x0009DA48
	private void UpdateScale()
	{
		Transform transform = base.transform;
		Vector3 localScale = transform.localScale;
		if (this.x && base.transform.lossyScale.x < 0f)
		{
			localScale.x = -localScale.x;
		}
		if (this.y && base.transform.lossyScale.y < 0f)
		{
			localScale.y = -localScale.y;
		}
		transform.localScale = localScale;
	}

	// Token: 0x0400218A RID: 8586
	[SerializeField]
	private bool x = true;

	// Token: 0x0400218B RID: 8587
	[SerializeField]
	private bool y = true;

	// Token: 0x0400218C RID: 8588
	[SerializeField]
	private bool everyFrame = true;

	// Token: 0x0400218D RID: 8589
	private UpdateBatcher updateBatcher;
}
