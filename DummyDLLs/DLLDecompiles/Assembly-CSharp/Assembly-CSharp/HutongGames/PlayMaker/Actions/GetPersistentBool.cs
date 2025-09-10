using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013A6 RID: 5030
	public class GetPersistentBool : FsmStateAction
	{
		// Token: 0x060080FF RID: 33023 RVA: 0x00260048 File Offset: 0x0025E248
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.storeValue = null;
		}

		// Token: 0x06008100 RID: 33024 RVA: 0x0026005C File Offset: 0x0025E25C
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				PersistentBoolItem component = gameObject.GetComponent<PersistentBoolItem>();
				this.storeValue.Value = component.ItemData.Value;
			}
			base.Finish();
		}

		// Token: 0x0400803F RID: 32831
		public FsmOwnerDefault target;

		// Token: 0x04008040 RID: 32832
		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;
	}
}
