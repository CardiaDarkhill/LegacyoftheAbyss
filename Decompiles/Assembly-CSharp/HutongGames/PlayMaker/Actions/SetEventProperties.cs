using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB6 RID: 3510
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets Event Data before sending an event. Get the Event Data, Get Event Properties action.")]
	public class SetEventProperties : FsmStateAction
	{
		// Token: 0x060065C9 RID: 26057 RVA: 0x00201CF0 File Offset: 0x001FFEF0
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.datas = new FsmVar[1];
		}

		// Token: 0x060065CA RID: 26058 RVA: 0x00201D0C File Offset: 0x001FFF0C
		public override void OnEnter()
		{
			SetEventProperties.properties = new Dictionary<string, object>();
			for (int i = 0; i < this.keys.Length; i++)
			{
				SetEventProperties.properties[this.keys[i].Value] = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.datas[i]);
			}
			base.Finish();
		}

		// Token: 0x040064E9 RID: 25833
		[CompoundArray("Event Properties", "Key", "Data")]
		public FsmString[] keys;

		// Token: 0x040064EA RID: 25834
		public FsmVar[] datas;

		// Token: 0x040064EB RID: 25835
		public static Dictionary<string, object> properties;
	}
}
