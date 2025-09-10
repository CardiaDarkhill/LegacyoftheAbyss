using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012E8 RID: 4840
	public class SetInteractableLabel : FsmStateAction
	{
		// Token: 0x06007E32 RID: 32306 RVA: 0x002586ED File Offset: 0x002568ED
		public override void Reset()
		{
			this.Target = null;
			this.Label = null;
			this.UseChildren = null;
		}

		// Token: 0x06007E33 RID: 32307 RVA: 0x00258704 File Offset: 0x00256904
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
						component.InteractLabel = (InteractableBase.PromptLabels)this.Label.Value;
					}
				}
				else
				{
					InteractableBase[] componentsInChildren = safe.GetComponentsInChildren<InteractableBase>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].InteractLabel = (InteractableBase.PromptLabels)this.Label.Value;
					}
				}
				base.Finish();
			}
		}

		// Token: 0x04007E08 RID: 32264
		[RequiredField]
		[CheckForComponent(typeof(InteractableBase))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E09 RID: 32265
		[ObjectType(typeof(InteractableBase.PromptLabels))]
		public FsmEnum Label;

		// Token: 0x04007E0A RID: 32266
		public FsmBool UseChildren;
	}
}
