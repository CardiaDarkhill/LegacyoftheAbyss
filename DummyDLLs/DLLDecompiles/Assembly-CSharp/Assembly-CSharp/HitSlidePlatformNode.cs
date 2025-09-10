using System;
using UnityEngine;

// Token: 0x02000505 RID: 1285
public class HitSlidePlatformNode : MonoBehaviour
{
	// Token: 0x1700052D RID: 1325
	// (get) Token: 0x06002DFC RID: 11772 RVA: 0x000C9B01 File Offset: 0x000C7D01
	public bool IsEndNode
	{
		get
		{
			return this.isEndNode;
		}
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x000C9B09 File Offset: 0x000C7D09
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<HitSlidePlatformNode>(ref this.connectedNodes, typeof(HitInstance.HitDirection));
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x000C9B20 File Offset: 0x000C7D20
	private void OnDrawGizmos()
	{
		Vector2 v = base.transform.position;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(v, 0.15f);
		foreach (HitSlidePlatformNode hitSlidePlatformNode in this.connectedNodes)
		{
			if (hitSlidePlatformNode)
			{
				Vector2 v2 = hitSlidePlatformNode.transform.position;
				Gizmos.color = Color.white;
				Gizmos.DrawLine(v, v2);
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(v, 0.3f);
			}
		}
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x000C9BC1 File Offset: 0x000C7DC1
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x000C9BC9 File Offset: 0x000C7DC9
	public HitSlidePlatformNode GetConnectedNode(HitInstance.HitDirection hitDirection)
	{
		return this.connectedNodes[(int)hitDirection];
	}

	// Token: 0x0400302D RID: 12333
	[SerializeField]
	[ArrayForEnum(typeof(HitInstance.HitDirection))]
	private HitSlidePlatformNode[] connectedNodes;

	// Token: 0x0400302E RID: 12334
	[Space]
	[SerializeField]
	private bool isEndNode;
}
