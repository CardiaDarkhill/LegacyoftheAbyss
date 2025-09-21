using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EAC RID: 3756
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the number of children that a GameObject has.")]
	public class GetChildCount : FsmStateAction
	{
		// Token: 0x06006A60 RID: 27232 RVA: 0x00213CD5 File Offset: 0x00211ED5
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x06006A61 RID: 27233 RVA: 0x00213CE5 File Offset: 0x00211EE5
		public override void OnEnter()
		{
			this.DoGetChildCount();
			base.Finish();
		}

		// Token: 0x06006A62 RID: 27234 RVA: 0x00213CF4 File Offset: 0x00211EF4
		private void DoGetChildCount()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.childCount;
		}

		// Token: 0x040069B5 RID: 27061
		[RequiredField]
		[Tooltip("The GameObject to test.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069B6 RID: 27062
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the number of children in an int variable.")]
		public FsmInt storeResult;
	}
}
