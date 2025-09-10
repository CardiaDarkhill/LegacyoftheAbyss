using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E99 RID: 3737
	[ActionCategory(ActionCategory.GameObject)]
	public class FindChildRecursive : FsmStateAction
	{
		// Token: 0x06006A0F RID: 27151 RVA: 0x002129DF File Offset: 0x00210BDF
		public override void Reset()
		{
			this.gameObject = null;
			this.childName = "";
			this.storeResult = null;
		}

		// Token: 0x06006A10 RID: 27152 RVA: 0x00212A00 File Offset: 0x00210C00
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Transform transform = this.DoFindChildRecursive(ownerDefaultTarget.transform, this.childName.Value);
			this.storeResult.Value = ((transform != null) ? transform.gameObject : null);
			base.Finish();
		}

		// Token: 0x06006A11 RID: 27153 RVA: 0x00212A64 File Offset: 0x00210C64
		private Transform DoFindChildRecursive(Transform parent, string childName)
		{
			int childCount = parent.childCount;
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				if (transform.gameObject.name == childName)
				{
					return transform;
				}
			}
			foreach (object obj2 in parent)
			{
				Transform parent2 = (Transform)obj2;
				Transform transform2 = this.DoFindChildRecursive(parent2, childName);
				if (transform2)
				{
					return transform2;
				}
			}
			return null;
		}

		// Token: 0x04006968 RID: 26984
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006969 RID: 26985
		[RequiredField]
		public FsmString childName;

		// Token: 0x0400696A RID: 26986
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;
	}
}
