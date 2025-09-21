using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001369 RID: 4969
	public class SetRemaskerExited : FsmStateAction
	{
		// Token: 0x06008021 RID: 32801 RVA: 0x0025D763 File Offset: 0x0025B963
		public override void Reset()
		{
			this.Target = null;
			this.WasDisabled = null;
			this.Recursive = null;
		}

		// Token: 0x06008022 RID: 32802 RVA: 0x0025D77C File Offset: 0x0025B97C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				if (this.Recursive.Value)
				{
					Remasker[] componentsInChildren = safe.GetComponentsInChildren<Remasker>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].Exited(this.WasDisabled.Value);
					}
				}
				else
				{
					Remasker component = safe.GetComponent<Remasker>();
					if (component)
					{
						component.Exited(this.WasDisabled.Value);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007F88 RID: 32648
		public FsmOwnerDefault Target;

		// Token: 0x04007F89 RID: 32649
		public FsmBool WasDisabled;

		// Token: 0x04007F8A RID: 32650
		public FsmBool Recursive;
	}
}
