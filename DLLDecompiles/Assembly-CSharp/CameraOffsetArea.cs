using System;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class CameraOffsetArea : MonoBehaviour
{
	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000AF4 RID: 2804 RVA: 0x00031B21 File Offset: 0x0002FD21
	public Vector2 Offset
	{
		get
		{
			return this.offset;
		}
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00031B29 File Offset: 0x0002FD29
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!CameraLockArea.IsInApplicableGameState() || !collision.CompareTag("Player"))
		{
			return;
		}
		GameCameras.instance.cameraTarget.AddOffsetArea(this);
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x00031B50 File Offset: 0x0002FD50
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!collision.CompareTag("Player"))
		{
			return;
		}
		GameCameras.instance.cameraTarget.RemoveOffsetArea(this);
	}

	// Token: 0x04000A7D RID: 2685
	[SerializeField]
	private Vector2 offset;
}
