using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013AA RID: 5034
	public class DestroyPersistentBoolIfPdBool : FsmStateAction
	{
		// Token: 0x0600810C RID: 33036 RVA: 0x002602CE File Offset: 0x0025E4CE
		public override void Reset()
		{
			this.Target = new FsmOwnerDefault();
			this.PdBoolName = null;
		}

		// Token: 0x0600810D RID: 33037 RVA: 0x002602E4 File Offset: 0x0025E4E4
		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(this.PdBoolName.Value))
			{
				base.Finish();
				return;
			}
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				PersistentBoolItem component = safe.GetComponent<PersistentBoolItem>();
				if (component)
				{
					Object.Destroy(component);
				}
			}
			base.Finish();
		}

		// Token: 0x0400804A RID: 32842
		public FsmOwnerDefault Target;

		// Token: 0x0400804B RID: 32843
		[UIHint(UIHint.Variable)]
		public FsmString PdBoolName;
	}
}
