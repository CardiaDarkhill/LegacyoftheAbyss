using System;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCB RID: 3275
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class PlayerDataBoolTest : FsmStateAction
	{
		// Token: 0x060061B3 RID: 25011 RVA: 0x001EF070 File Offset: 0x001ED270
		public override void Reset()
		{
			this.boolName = null;
			this.isTrue = null;
			this.isFalse = null;
		}

		// Token: 0x060061B4 RID: 25012 RVA: 0x001EF088 File Offset: 0x001ED288
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				Debug.Log("GameManager could not be found");
				return;
			}
			if (VariableExtensions.VariableExists<bool, PlayerData>(this.boolName.Value))
			{
				this.boolCheck = instance.GetPlayerDataBool(this.boolName.Value);
				base.Fsm.Event(this.boolCheck ? this.isTrue : this.isFalse);
			}
			base.Finish();
		}

		// Token: 0x04005FE5 RID: 24549
		[RequiredField]
		public FsmString boolName;

		// Token: 0x04005FE6 RID: 24550
		[Tooltip("Event to send if the Bool variable is True.")]
		public FsmEvent isTrue;

		// Token: 0x04005FE7 RID: 24551
		[Tooltip("Event to send if the Bool variable is False.")]
		public FsmEvent isFalse;

		// Token: 0x04005FE8 RID: 24552
		private bool boolCheck;
	}
}
