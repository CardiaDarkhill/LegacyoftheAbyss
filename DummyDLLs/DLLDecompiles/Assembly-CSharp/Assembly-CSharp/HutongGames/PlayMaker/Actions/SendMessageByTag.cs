using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D13 RID: 3347
	public class SendMessageByTag : FsmStateAction
	{
		// Token: 0x060062DC RID: 25308 RVA: 0x001F3D0C File Offset: 0x001F1F0C
		public override void OnEnter()
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(this.tag.Value);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SendMessage(this.message.Value);
			}
		}

		// Token: 0x060062DD RID: 25309 RVA: 0x001F3D4B File Offset: 0x001F1F4B
		public override void OnExit()
		{
		}

		// Token: 0x04006143 RID: 24899
		public FsmString tag;

		// Token: 0x04006144 RID: 24900
		public FsmString message;
	}
}
