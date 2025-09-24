using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EAB RID: 3755
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class GetChild : FsmStateAction
	{
		// Token: 0x06006A5B RID: 27227 RVA: 0x00213B4A File Offset: 0x00211D4A
		public override void Reset()
		{
			this.gameObject = null;
			this.childName = "";
			this.withTag = "Untagged";
			this.storeResult = null;
		}

		// Token: 0x06006A5C RID: 27228 RVA: 0x00213B7A File Offset: 0x00211D7A
		public override void OnEnter()
		{
			this.storeResult.Value = GetChild.DoGetChildByName(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.childName.Value, this.withTag.Value);
			base.Finish();
		}

		// Token: 0x06006A5D RID: 27229 RVA: 0x00213BBC File Offset: 0x00211DBC
		private static GameObject DoGetChildByName(GameObject root, string name, string tag)
		{
			if (root == null)
			{
				return null;
			}
			foreach (object obj in root.transform)
			{
				Transform transform = (Transform)obj;
				if (!string.IsNullOrEmpty(name))
				{
					if (transform.name == name)
					{
						if (string.IsNullOrEmpty(tag))
						{
							return transform.gameObject;
						}
						if (transform.tag.Equals(tag))
						{
							return transform.gameObject;
						}
					}
				}
				else if (!string.IsNullOrEmpty(tag) && transform.tag == tag)
				{
					return transform.gameObject;
				}
				GameObject gameObject = GetChild.DoGetChildByName(transform.gameObject, name, tag);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
			return null;
		}

		// Token: 0x06006A5E RID: 27230 RVA: 0x00213CA0 File Offset: 0x00211EA0
		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(this.childName.Value) && string.IsNullOrEmpty(this.withTag.Value))
			{
				return "Specify Child Name, Tag, or both.";
			}
			return null;
		}

		// Token: 0x040069B1 RID: 27057
		[RequiredField]
		[Tooltip("The GameObject to search.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069B2 RID: 27058
		[Tooltip("The name of the child to search for.")]
		public FsmString childName;

		// Token: 0x040069B3 RID: 27059
		[UIHint(UIHint.Tag)]
		[Tooltip("The Tag to search for. If Child Name is set, both name and Tag need to match.")]
		public FsmString withTag;

		// Token: 0x040069B4 RID: 27060
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a GameObject variable.")]
		public FsmGameObject storeResult;
	}
}
