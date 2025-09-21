using System;
using UnityEngine;

// Token: 0x02000600 RID: 1536
public class TriggerEnterSendMessage : MonoBehaviour
{
	// Token: 0x060036EF RID: 14063 RVA: 0x000F2474 File Offset: 0x000F0674
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("Acid trigger entered");
		collision.gameObject.SendMessage(this.message, this.options);
	}

	// Token: 0x040039B6 RID: 14774
	public string message = "Acid";

	// Token: 0x040039B7 RID: 14775
	public SendMessageOptions options = SendMessageOptions.DontRequireReceiver;
}
