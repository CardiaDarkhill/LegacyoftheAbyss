using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012E5 RID: 4837
	public class ActivateInteractible : FsmStateAction
	{
		// Token: 0x06007E25 RID: 32293 RVA: 0x002584D6 File Offset: 0x002566D6
		public bool IsActivate()
		{
			return this.Activate.Value;
		}

		// Token: 0x06007E26 RID: 32294 RVA: 0x002584E3 File Offset: 0x002566E3
		public override void Reset()
		{
			this.Target = null;
			this.Activate = null;
			this.AllowQueueing = null;
			this.UseChildren = null;
		}

		// Token: 0x06007E27 RID: 32295 RVA: 0x00258504 File Offset: 0x00256704
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				if (!this.UseChildren.Value)
				{
					InteractableBase component = safe.GetComponent<InteractableBase>();
					if (component)
					{
						this.DoInteractableAction(component);
					}
				}
				else
				{
					foreach (InteractableBase interactable in safe.GetComponentsInChildren<InteractableBase>())
					{
						this.DoInteractableAction(interactable);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x06007E28 RID: 32296 RVA: 0x00258573 File Offset: 0x00256773
		private void DoInteractableAction(InteractableBase interactable)
		{
			if (this.Activate.Value)
			{
				interactable.Activate();
				return;
			}
			interactable.Deactivate(this.AllowQueueing.Value);
		}

		// Token: 0x04007DFC RID: 32252
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04007DFD RID: 32253
		[RequiredField]
		public FsmBool Activate;

		// Token: 0x04007DFE RID: 32254
		[HideIf("IsActivate")]
		public FsmBool AllowQueueing;

		// Token: 0x04007DFF RID: 32255
		public FsmBool UseChildren;
	}
}
