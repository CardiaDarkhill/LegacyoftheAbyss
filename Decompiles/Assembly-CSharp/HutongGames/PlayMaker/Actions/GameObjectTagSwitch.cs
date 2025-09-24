using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F56 RID: 3926
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends an Event based on a Game Object's Tag.")]
	public class GameObjectTagSwitch : FsmStateAction
	{
		// Token: 0x06006D17 RID: 27927 RVA: 0x0021F55B File Offset: 0x0021D75B
		public override void Reset()
		{
			this.gameObject = null;
			this.compareTo = new FsmString[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x06006D18 RID: 27928 RVA: 0x0021F583 File Offset: 0x0021D783
		public override void OnEnter()
		{
			this.DoTagSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D19 RID: 27929 RVA: 0x0021F599 File Offset: 0x0021D799
		public override void OnUpdate()
		{
			this.DoTagSwitch();
		}

		// Token: 0x06006D1A RID: 27930 RVA: 0x0021F5A4 File Offset: 0x0021D7A4
		private void DoTagSwitch()
		{
			GameObject value = this.gameObject.Value;
			if (value == null)
			{
				return;
			}
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (value.tag == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04006CDB RID: 27867
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The GameObject to test.")]
		public FsmGameObject gameObject;

		// Token: 0x04006CDC RID: 27868
		[CompoundArray("Tag Switches", "Compare Tag", "Send Event")]
		[UIHint(UIHint.Tag)]
		[Tooltip("Compare GameObject's Tag.")]
		public FsmString[] compareTo;

		// Token: 0x04006CDD RID: 27869
		[Tooltip("Send this event if Tag matches.")]
		public FsmEvent[] sendEvent;

		// Token: 0x04006CDE RID: 27870
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
