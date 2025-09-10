using System;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C78 RID: 3192
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class GetPlayerDataVector3 : FsmStateAction
	{
		// Token: 0x06006036 RID: 24630 RVA: 0x001E7688 File Offset: 0x001E5888
		public override void Reset()
		{
			this.vector3Name = null;
			this.storeValue = null;
		}

		// Token: 0x06006037 RID: 24631 RVA: 0x001E7698 File Offset: 0x001E5898
		public override void OnEnter()
		{
			if (VariableExtensions.VariableExists<Vector3, PlayerData>(this.vector3Name.Value))
			{
				this.storeValue.Value = PlayerData.instance.GetVariable(this.vector3Name.Value);
			}
			else
			{
				Debug.Log(string.Format("PlayerData vector3 {0} does not exist. (FSM: {1}, State: {2})", this.vector3Name.Value, base.Fsm.Name, base.State.Name), base.Owner);
			}
			base.Finish();
		}

		// Token: 0x04005D8E RID: 23950
		[RequiredField]
		public FsmString vector3Name;

		// Token: 0x04005D8F RID: 23951
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmVector3 storeValue;
	}
}
