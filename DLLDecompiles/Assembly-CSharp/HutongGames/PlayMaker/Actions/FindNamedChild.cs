using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3A RID: 3130
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds a child on the selected object with the EXACT same name as the variable")]
	public class FindNamedChild : FsmStateAction
	{
		// Token: 0x06005F24 RID: 24356 RVA: 0x001E2BBB File Offset: 0x001E0DBB
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x06005F25 RID: 24357 RVA: 0x001E2BCB File Offset: 0x001E0DCB
		public override void OnEnter()
		{
			this.DoFindChild();
			base.Finish();
		}

		// Token: 0x06005F26 RID: 24358 RVA: 0x001E2BDC File Offset: 0x001E0DDC
		private void DoFindChild()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Transform transform = ownerDefaultTarget.transform.Find(this.storeResult.Name);
			this.storeResult.Value = ((transform != null) ? transform.gameObject : null);
		}

		// Token: 0x04005C18 RID: 23576
		[RequiredField]
		[Tooltip("The GameObject to search.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005C19 RID: 23577
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the child in a GameObject variable.")]
		public FsmGameObject storeResult;
	}
}
