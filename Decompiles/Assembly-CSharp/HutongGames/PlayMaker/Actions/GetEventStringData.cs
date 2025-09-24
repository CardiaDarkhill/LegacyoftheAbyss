using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001087 RID: 4231
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the String data from the last Event.")]
	public class GetEventStringData : FsmStateAction
	{
		// Token: 0x06007332 RID: 29490 RVA: 0x002365B4 File Offset: 0x002347B4
		public override void Reset()
		{
			this.getStringData = null;
		}

		// Token: 0x06007333 RID: 29491 RVA: 0x002365BD File Offset: 0x002347BD
		public override void OnEnter()
		{
			this.getStringData.Value = Fsm.EventData.StringData;
			base.Finish();
		}

		// Token: 0x0400733D RID: 29501
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the string data in a variable.")]
		public FsmString getStringData;
	}
}
