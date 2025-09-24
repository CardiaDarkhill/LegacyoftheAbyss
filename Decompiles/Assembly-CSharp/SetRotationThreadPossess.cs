using System;
using UnityEngine;

// Token: 0x0200054C RID: 1356
public class SetRotationThreadPossess : MonoBehaviour
{
	// Token: 0x06003074 RID: 12404 RVA: 0x000D6328 File Offset: 0x000D4528
	private void OnEnable()
	{
		if (this.hasStarted)
		{
			this.DoSetRotation();
		}
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x000D6338 File Offset: 0x000D4538
	private void Start()
	{
		this.hasStarted = true;
		this.DoSetRotation();
	}

	// Token: 0x06003076 RID: 12406 RVA: 0x000D6347 File Offset: 0x000D4547
	public void Update()
	{
		this.DoSetRotation();
	}

	// Token: 0x06003077 RID: 12407 RVA: 0x000D6350 File Offset: 0x000D4550
	private void DoSetRotation()
	{
		float angleToSilkThread = GameManager.instance.sm.AngleToSilkThread;
		base.transform.SetRotation2D(angleToSilkThread + this.angleOffset);
	}

	// Token: 0x04003373 RID: 13171
	[SerializeField]
	private float angleOffset;

	// Token: 0x04003374 RID: 13172
	private bool hasStarted;
}
