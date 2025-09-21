using System;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class RotationController : MonoBehaviour
{
	// Token: 0x06000068 RID: 104 RVA: 0x00004000 File Offset: 0x00002200
	private void Update()
	{
		if (Input.GetAxis("Horizontal") != 0f)
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.y += Input.GetAxis("Horizontal") * this.speed;
			base.transform.eulerAngles = eulerAngles;
		}
	}

	// Token: 0x04000058 RID: 88
	public float speed = 1f;
}
