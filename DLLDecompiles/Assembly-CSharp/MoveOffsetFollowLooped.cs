using System;
using UnityEngine;

// Token: 0x0200051D RID: 1309
public class MoveOffsetFollowLooped : MonoBehaviour
{
	// Token: 0x06002F1E RID: 12062 RVA: 0x000CFEAC File Offset: 0x000CE0AC
	private void Start()
	{
		if (this.followLocal)
		{
			this.previousFollowPos = this.followLocal.localPosition;
		}
	}

	// Token: 0x06002F1F RID: 12063 RVA: 0x000CFED4 File Offset: 0x000CE0D4
	private void Update()
	{
		if (!this.followLocal)
		{
			return;
		}
		Vector2 a = this.followLocal.localPosition;
		Vector2 b = a - this.previousFollowPos;
		Vector2 vector = base.transform.localPosition + b;
		if (vector.x > this.maxPos.x)
		{
			vector.x = this.minPos.x;
		}
		if (vector.y > this.maxPos.y)
		{
			vector.y = this.minPos.y;
		}
		if (vector.x < this.minPos.x)
		{
			vector.x = this.maxPos.x;
		}
		if (vector.y < this.minPos.y)
		{
			vector.y = this.maxPos.y;
		}
		base.transform.SetLocalPosition2D(vector);
		this.previousFollowPos = a;
	}

	// Token: 0x040031D3 RID: 12755
	[SerializeField]
	private Transform followLocal;

	// Token: 0x040031D4 RID: 12756
	[SerializeField]
	private Vector2 minPos;

	// Token: 0x040031D5 RID: 12757
	[SerializeField]
	private Vector2 maxPos;

	// Token: 0x040031D6 RID: 12758
	private Vector2 previousFollowPos;
}
