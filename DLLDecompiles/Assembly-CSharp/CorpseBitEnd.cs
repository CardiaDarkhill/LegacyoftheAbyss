using System;
using UnityEngine;

// Token: 0x020002AA RID: 682
public class CorpseBitEnd : MonoBehaviour
{
	// Token: 0x0600185F RID: 6239 RVA: 0x00070228 File Offset: 0x0006E428
	private void Update()
	{
		if (this.timer <= 0f && !this.stopped)
		{
			Rigidbody2D component = base.GetComponent<Rigidbody2D>();
			if (component)
			{
				component.isKinematic = true;
			}
			component.linearVelocity = new Vector2(0f, 0f);
			component.angularVelocity = 0f;
			base.GetComponent<ObjectBounce>().StopBounce();
			base.GetComponent<PolygonCollider2D>().enabled = false;
			this.stopped = true;
			return;
		}
		this.timer -= Time.deltaTime;
	}

	// Token: 0x04001766 RID: 5990
	public float timer;

	// Token: 0x04001767 RID: 5991
	private bool stopped;
}
