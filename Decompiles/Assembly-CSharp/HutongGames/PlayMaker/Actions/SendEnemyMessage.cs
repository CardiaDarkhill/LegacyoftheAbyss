using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0D RID: 3341
	[ActionCategory("Hollow Knight")]
	[Tooltip("Hook for enemy messages for scripts. Translates messages into appropriate method calls.")]
	public class SendEnemyMessage : FsmStateAction
	{
		// Token: 0x060062C6 RID: 25286 RVA: 0x001F38CD File Offset: 0x001F1ACD
		public override void Reset()
		{
			this.Target = new FsmGameObject
			{
				UseVariable = true
			};
			this.EventString = new FsmString
			{
				UseVariable = true
			};
		}

		// Token: 0x060062C7 RID: 25287 RVA: 0x001F38F4 File Offset: 0x001F1AF4
		public override void OnEnter()
		{
			GameObject value = this.Target.Value;
			string value2 = this.EventString.Value;
			if (value != null && !string.IsNullOrEmpty(value2))
			{
				if (!(value2 == "GO LEFT"))
				{
					if (value2 == "GO RIGHT")
					{
						SendEnemyMessage.SendWalkerGoInDirection(value, 1);
					}
				}
				else
				{
					SendEnemyMessage.SendWalkerGoInDirection(value, -1);
				}
			}
			base.Finish();
		}

		// Token: 0x060062C8 RID: 25288 RVA: 0x001F395C File Offset: 0x001F1B5C
		private static void SendWalkerGoInDirection(GameObject target, int facing)
		{
			Walker component = target.GetComponent<Walker>();
			if (component != null)
			{
				component.RecieveGoMessage(facing);
			}
			WalkerV2 component2 = target.GetComponent<WalkerV2>();
			if (component2)
			{
				component2.ForceDirection((float)facing);
			}
		}

		// Token: 0x04006131 RID: 24881
		public FsmGameObject Target;

		// Token: 0x04006132 RID: 24882
		public FsmString EventString;
	}
}
