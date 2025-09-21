using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EB5 RID: 3765
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Random Child of a Game Object.")]
	public class GetRandomChild : FsmStateAction
	{
		// Token: 0x06006A87 RID: 27271 RVA: 0x0021438D File Offset: 0x0021258D
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x06006A88 RID: 27272 RVA: 0x0021439D File Offset: 0x0021259D
		public override void OnEnter()
		{
			this.DoGetRandomChild();
			base.Finish();
		}

		// Token: 0x06006A89 RID: 27273 RVA: 0x002143AC File Offset: 0x002125AC
		private void DoGetRandomChild()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			int childCount = ownerDefaultTarget.transform.childCount;
			if (childCount == 0)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.GetChild(Random.Range(0, childCount)).gameObject;
		}

		// Token: 0x040069D9 RID: 27097
		[RequiredField]
		[Tooltip("The parent Game Object.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069DA RID: 27098
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the random child in a Game Object Variable.")]
		public FsmGameObject storeResult;
	}
}
