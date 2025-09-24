using System;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class SetAngleToVelocity : MonoBehaviour
{
	// Token: 0x06000516 RID: 1302 RVA: 0x0001A69C File Offset: 0x0001889C
	private void Update()
	{
		Vector2 linearVelocity = this.rb.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f + this.angleOffset;
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x040004F2 RID: 1266
	public Rigidbody2D rb;

	// Token: 0x040004F3 RID: 1267
	public float angleOffset;
}
