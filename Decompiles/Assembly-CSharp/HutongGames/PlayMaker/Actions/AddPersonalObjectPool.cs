using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012DA RID: 4826
	public class AddPersonalObjectPool : FsmStateAction
	{
		// Token: 0x06007DDF RID: 32223 RVA: 0x0025778F File Offset: 0x0025598F
		public override void Reset()
		{
			this.Target = null;
			this.Prefab = null;
			this.Amount = null;
			this.SharePooledInScene = null;
			this.FinalCall = true;
		}

		// Token: 0x06007DE0 RID: 32224 RVA: 0x002577BC File Offset: 0x002559BC
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (this.SharePooledInScene.Value)
			{
				PersonalObjectPool.EnsurePooledInScene(safe, this.Prefab.Value, this.Amount.Value, this.FinalCall.Value, false, false);
			}
			else
			{
				safe.AddComponentIfNotPresent<PersonalObjectPool>().startupPool.Add(new StartupPool(this.Amount.Value, this.Prefab.Value, false, false));
			}
			base.Finish();
		}

		// Token: 0x04007DBC RID: 32188
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04007DBD RID: 32189
		[RequiredField]
		public FsmGameObject Prefab;

		// Token: 0x04007DBE RID: 32190
		[RequiredField]
		public FsmInt Amount;

		// Token: 0x04007DBF RID: 32191
		public FsmBool SharePooledInScene;

		// Token: 0x04007DC0 RID: 32192
		public FsmBool FinalCall;
	}
}
