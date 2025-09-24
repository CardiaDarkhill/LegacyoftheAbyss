using System;
using UnityEngine;

// Token: 0x02000662 RID: 1634
public class FlamePin : MonoBehaviour
{
	// Token: 0x06003A3D RID: 14909 RVA: 0x000FF4AF File Offset: 0x000FD6AF
	private void Start()
	{
		this.pd = PlayerData.instance;
	}

	// Token: 0x06003A3E RID: 14910 RVA: 0x000FF4BC File Offset: 0x000FD6BC
	private void OnEnable()
	{
		base.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		if (this.pd == null)
		{
			this.pd = PlayerData.instance;
		}
		base.gameObject.GetComponent<SpriteRenderer>().enabled = true;
	}

	// Token: 0x04003CC8 RID: 15560
	private PlayerData pd;

	// Token: 0x04003CC9 RID: 15561
	public float level;
}
