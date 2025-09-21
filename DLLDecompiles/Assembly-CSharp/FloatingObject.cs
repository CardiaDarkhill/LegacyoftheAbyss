using System;
using UnityEngine;

// Token: 0x020004E0 RID: 1248
public class FloatingObject : MonoBehaviour
{
	// Token: 0x06002CCF RID: 11471 RVA: 0x000C4000 File Offset: 0x000C2200
	private void Start()
	{
		this.initialPos = base.transform.localPosition;
	}

	// Token: 0x06002CD0 RID: 11472 RVA: 0x000C4014 File Offset: 0x000C2214
	private void Update()
	{
		base.transform.localPosition = this.initialPos + new Vector3(this.variance.x * Mathf.Sin(Time.time * this.speedX + this.initialPos.z), this.variance.y * Mathf.Sin(Time.time * this.speedY + this.initialPos.z), 0f);
	}

	// Token: 0x04002E73 RID: 11891
	public Vector2 variance = Vector2.one;

	// Token: 0x04002E74 RID: 11892
	public float speedX = 0.5f;

	// Token: 0x04002E75 RID: 11893
	public float speedY = 1f;

	// Token: 0x04002E76 RID: 11894
	private Vector3 initialPos;
}
