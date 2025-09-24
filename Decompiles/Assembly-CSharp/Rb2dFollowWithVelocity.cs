using System;
using UnityEngine;

// Token: 0x0200026B RID: 619
public class Rb2dFollowWithVelocity : MonoBehaviour
{
	// Token: 0x1700025A RID: 602
	// (get) Token: 0x06001620 RID: 5664 RVA: 0x000631C1 File Offset: 0x000613C1
	public Transform Target
	{
		get
		{
			return this.target;
		}
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x000631C9 File Offset: 0x000613C9
	private void Reset()
	{
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x000631D7 File Offset: 0x000613D7
	private void Awake()
	{
		if (this.deparent && base.transform.parent)
		{
			base.transform.SetParent(null, true);
		}
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x00063200 File Offset: 0x00061400
	private void Start()
	{
		this.initialTargetPos = this.target.position;
		this.initialOffset = this.body.position - this.initialTargetPos;
		this.joint = this.body.GetComponent<TargetJoint2D>();
		if (this.joint)
		{
			this.joint.autoConfigureTarget = false;
		}
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x0006326C File Offset: 0x0006146C
	private void FixedUpdate()
	{
		Vector2 a;
		if (this.followLerpX > 0.99f && this.followLerpY > 0.99f)
		{
			a = this.target.position;
		}
		else
		{
			Vector3 position = this.target.position;
			a.x = Mathf.Lerp(this.initialTargetPos.x, position.x, this.followLerpX);
			a.y = Mathf.Lerp(this.initialTargetPos.y, position.y, this.followLerpY);
		}
		Vector2 a2 = a + this.initialOffset;
		if (this.joint)
		{
			this.joint.target = a2;
			return;
		}
		Vector2 linearVelocity = (a2 - this.body.position) / Time.deltaTime;
		this.body.linearVelocity = linearVelocity;
	}

	// Token: 0x04001488 RID: 5256
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x04001489 RID: 5257
	[SerializeField]
	private Transform target;

	// Token: 0x0400148A RID: 5258
	[SerializeField]
	private bool deparent;

	// Token: 0x0400148B RID: 5259
	[Space]
	[SerializeField]
	[Range(0f, 1f)]
	private float followLerpX = 1f;

	// Token: 0x0400148C RID: 5260
	[SerializeField]
	[Range(0f, 1f)]
	private float followLerpY = 1f;

	// Token: 0x0400148D RID: 5261
	private Vector2 initialTargetPos;

	// Token: 0x0400148E RID: 5262
	private Vector2 initialOffset;

	// Token: 0x0400148F RID: 5263
	private TargetJoint2D joint;
}
