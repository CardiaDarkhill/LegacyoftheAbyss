using System;
using UnityEngine;

// Token: 0x02000539 RID: 1337
[RequireComponent(typeof(RelativeJoint2D))]
public class RelativeJointAutoConfigure : MonoBehaviour
{
	// Token: 0x06003008 RID: 12296 RVA: 0x000D3BE2 File Offset: 0x000D1DE2
	private void Awake()
	{
		this.joint = base.GetComponent<RelativeJoint2D>();
		this.joint.autoConfigureOffset = true;
	}

	// Token: 0x06003009 RID: 12297 RVA: 0x000D3BFC File Offset: 0x000D1DFC
	private void Start()
	{
		this.joint.autoConfigureOffset = false;
	}

	// Token: 0x040032EC RID: 13036
	private RelativeJoint2D joint;
}
