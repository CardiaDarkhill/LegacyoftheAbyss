using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012C2 RID: 4802
	public class DoPassBlackThreadState : FsmStateAction
	{
		// Token: 0x06007D90 RID: 32144 RVA: 0x00256B23 File Offset: 0x00254D23
		public override void Reset()
		{
			this.Source = null;
			this.Target = null;
		}

		// Token: 0x06007D91 RID: 32145 RVA: 0x00256B34 File Offset: 0x00254D34
		public override void OnEnter()
		{
			PassBlackThreadState component = this.Source.GetSafe(this).GetComponent<PassBlackThreadState>();
			GameObject value = this.Target.Value;
			if (value)
			{
				value.GetComponent<BlackThreadState>().PassState(component);
			}
			base.Finish();
		}

		// Token: 0x04007D7D RID: 32125
		[CheckForComponent(typeof(PassBlackThreadState))]
		public FsmOwnerDefault Source;

		// Token: 0x04007D7E RID: 32126
		public FsmGameObject Target;
	}
}
