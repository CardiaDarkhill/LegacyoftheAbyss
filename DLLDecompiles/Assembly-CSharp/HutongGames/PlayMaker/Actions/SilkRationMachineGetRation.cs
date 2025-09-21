using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001317 RID: 4887
	public class SilkRationMachineGetRation : FsmStateAction
	{
		// Token: 0x06007ED5 RID: 32469 RVA: 0x00259EBD File Offset: 0x002580BD
		public override void Reset()
		{
			this.Target = null;
			this.DidDropRation = null;
		}

		// Token: 0x06007ED6 RID: 32470 RVA: 0x00259ED0 File Offset: 0x002580D0
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.rationMachine = safe.GetComponent<SilkRationMachine>();
				if (this.rationMachine)
				{
					this.rationMachine.RationDropped += this.OnRationDropped;
					bool value = this.rationMachine.TryDropRation();
					if (!this.DidDropRation.IsNone)
					{
						this.DidDropRation.Value = value;
					}
				}
			}
		}

		// Token: 0x06007ED7 RID: 32471 RVA: 0x00259F47 File Offset: 0x00258147
		public override void OnExit()
		{
			this.UnsubscribeEvents();
		}

		// Token: 0x06007ED8 RID: 32472 RVA: 0x00259F4F File Offset: 0x0025814F
		private void OnRationDropped()
		{
			this.UnsubscribeEvents();
			base.Finish();
		}

		// Token: 0x06007ED9 RID: 32473 RVA: 0x00259F5D File Offset: 0x0025815D
		private void UnsubscribeEvents()
		{
			if (this.rationMachine)
			{
				this.rationMachine.RationDropped -= this.OnRationDropped;
				this.rationMachine = null;
			}
		}

		// Token: 0x04007E82 RID: 32386
		[CheckForComponent(typeof(SilkRationMachine))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E83 RID: 32387
		[UIHint(UIHint.Variable)]
		public FsmBool DidDropRation;

		// Token: 0x04007E84 RID: 32388
		private SilkRationMachine rationMachine;
	}
}
