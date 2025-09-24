using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EAD RID: 3757
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the Child of a GameObject by Index.\nE.g., O to get the first child. HINT: Use this with an integer variable to iterate through children.")]
	public class GetChildNum : FsmStateAction
	{
		// Token: 0x06006A64 RID: 27236 RVA: 0x00213D3B File Offset: 0x00211F3B
		public override void Reset()
		{
			this.gameObject = null;
			this.childIndex = 0;
			this.store = null;
		}

		// Token: 0x06006A65 RID: 27237 RVA: 0x00213D57 File Offset: 0x00211F57
		public override void OnEnter()
		{
			this.store.Value = this.DoGetChildNum(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x06006A66 RID: 27238 RVA: 0x00213D84 File Offset: 0x00211F84
		private GameObject DoGetChildNum(GameObject go)
		{
			if (go == null || go.transform.childCount == 0 || this.childIndex.Value < 0)
			{
				return null;
			}
			return go.transform.GetChild(this.childIndex.Value % go.transform.childCount).gameObject;
		}

		// Token: 0x040069B7 RID: 27063
		[RequiredField]
		[Tooltip("The GameObject to search.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069B8 RID: 27064
		[RequiredField]
		[Tooltip("The index of the child to find (0 = first child).")]
		public FsmInt childIndex;

		// Token: 0x040069B9 RID: 27065
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the child in a GameObject variable.")]
		public FsmGameObject store;
	}
}
