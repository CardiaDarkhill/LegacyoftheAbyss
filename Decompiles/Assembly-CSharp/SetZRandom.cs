using System;
using UnityEngine;

// Token: 0x020005C4 RID: 1476
public class SetZRandom : MonoBehaviour
{
	// Token: 0x060034AC RID: 13484 RVA: 0x000E9DE4 File Offset: 0x000E7FE4
	private void Awake()
	{
		Transform transform = base.transform;
		this.initialZ = (this.localSpace ? transform.localPosition.z : transform.position.z);
	}

	// Token: 0x060034AD RID: 13485 RVA: 0x000E9E1E File Offset: 0x000E801E
	private void Start()
	{
		this.DoSetZ();
	}

	// Token: 0x060034AE RID: 13486 RVA: 0x000E9E26 File Offset: 0x000E8026
	private void OnEnable()
	{
		this.DoSetZ();
	}

	// Token: 0x060034AF RID: 13487 RVA: 0x000E9E30 File Offset: 0x000E8030
	private void DoSetZ()
	{
		float num = Random.Range(this.zMin, this.zMax);
		if (this.relativeToInitial)
		{
			num += this.initialZ;
		}
		if (this.localSpace)
		{
			base.transform.SetLocalPositionZ(num);
			return;
		}
		base.transform.SetPositionZ(num);
	}

	// Token: 0x0400381B RID: 14363
	[SerializeField]
	private float zMin;

	// Token: 0x0400381C RID: 14364
	[SerializeField]
	private float zMax;

	// Token: 0x0400381D RID: 14365
	[SerializeField]
	private bool localSpace;

	// Token: 0x0400381E RID: 14366
	[SerializeField]
	private bool relativeToInitial;

	// Token: 0x0400381F RID: 14367
	private float initialZ;
}
