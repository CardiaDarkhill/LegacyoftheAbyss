using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C72 RID: 3186
	[ActionCategory("Hollow Knight")]
	public class GetNamedParent : FsmStateAction
	{
		// Token: 0x06006022 RID: 24610 RVA: 0x001E72BF File Offset: 0x001E54BF
		public override void Reset()
		{
			this.gameObject = null;
			this.parentName = "";
			this.withTag = null;
			this.storeResult = null;
		}

		// Token: 0x06006023 RID: 24611 RVA: 0x001E72E8 File Offset: 0x001E54E8
		public override void OnEnter()
		{
			this.storeResult.Value = GetNamedParent.DoGetParentByName(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.parentName.Value, this.withTag.IsNone ? null : this.withTag.Value);
			base.Finish();
		}

		// Token: 0x06006024 RID: 24612 RVA: 0x001E7344 File Offset: 0x001E5544
		private static GameObject DoGetParentByName(GameObject root, string name, string tag)
		{
			if (root == null)
			{
				return null;
			}
			Transform parent = root.transform.parent;
			if (parent == null)
			{
				return null;
			}
			if (parent.name == name && (string.IsNullOrEmpty(tag) || parent.CompareTag(tag)))
			{
				return parent.gameObject;
			}
			return GetNamedParent.DoGetParentByName(parent.gameObject, name, tag);
		}

		// Token: 0x06006025 RID: 24613 RVA: 0x001E73A6 File Offset: 0x001E55A6
		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(this.parentName.Value) && string.IsNullOrEmpty(this.withTag.Value))
			{
				return "Specify Parent Name, Tag, or both.";
			}
			return null;
		}

		// Token: 0x04005D7C RID: 23932
		[RequiredField]
		[Tooltip("The GameObject to search.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D7D RID: 23933
		[Tooltip("The name of the parent to search for.")]
		public FsmString parentName;

		// Token: 0x04005D7E RID: 23934
		[UIHint(UIHint.Tag)]
		[Tooltip("The Tag to search for. If Parent Name is set, both name and Tag need to match.")]
		public FsmString withTag;

		// Token: 0x04005D7F RID: 23935
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a GameObject variable.")]
		public FsmGameObject storeResult;
	}
}
