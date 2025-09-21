using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001370 RID: 4976
	public class TryOpenQuickMap : FsmStateAction
	{
		// Token: 0x0600803A RID: 32826 RVA: 0x0025DC28 File Offset: 0x0025BE28
		public override void Reset()
		{
			this.Target = null;
			this.StoreDisplayName = null;
			this.HasMapEvent = null;
			this.NoMapEvent = null;
			this.StoreValue = null;
		}

		// Token: 0x0600803B RID: 32827 RVA: 0x0025DC50 File Offset: 0x0025BE50
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				GameMap component = safe.GetComponent<GameMap>();
				if (component)
				{
					string value;
					bool flag = component.TryOpenQuickMap(out value);
					this.StoreDisplayName.Value = value;
					this.StoreValue.Value = flag;
					base.Fsm.Event(flag ? this.HasMapEvent : this.NoMapEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x04007F9E RID: 32670
		public FsmOwnerDefault Target;

		// Token: 0x04007F9F RID: 32671
		[UIHint(UIHint.Variable)]
		public FsmString StoreDisplayName;

		// Token: 0x04007FA0 RID: 32672
		public FsmEvent HasMapEvent;

		// Token: 0x04007FA1 RID: 32673
		public FsmEvent NoMapEvent;

		// Token: 0x04007FA2 RID: 32674
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;
	}
}
