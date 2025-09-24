using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB5 RID: 3509
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets properties on the last event that caused a state change. Use Set Event Properties to define these values when sending events")]
	public class GetEventProperties : FsmStateAction
	{
		// Token: 0x060065C6 RID: 26054 RVA: 0x00201C1D File Offset: 0x001FFE1D
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.datas = new FsmVar[1];
		}

		// Token: 0x060065C7 RID: 26055 RVA: 0x00201C38 File Offset: 0x001FFE38
		public override void OnEnter()
		{
			try
			{
				if (SetEventProperties.properties == null)
				{
					throw new ArgumentException("no properties");
				}
				for (int i = 0; i < this.keys.Length; i++)
				{
					if (SetEventProperties.properties.ContainsKey(this.keys[i].Value))
					{
						PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.datas[i], SetEventProperties.properties[this.keys[i].Value]);
					}
				}
			}
			catch (Exception ex)
			{
				string str = "no properties found ";
				Exception ex2 = ex;
				Debug.Log(str + ((ex2 != null) ? ex2.ToString() : null));
			}
			base.Finish();
		}

		// Token: 0x040064E7 RID: 25831
		[CompoundArray("Event Properties", "Key", "Data")]
		public FsmString[] keys;

		// Token: 0x040064E8 RID: 25832
		[UIHint(UIHint.Variable)]
		public FsmVar[] datas;
	}
}
