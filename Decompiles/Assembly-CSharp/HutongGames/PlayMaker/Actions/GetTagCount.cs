using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EB9 RID: 3769
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the number of Game Objects in the scene with the specified Tag.")]
	public class GetTagCount : FsmStateAction
	{
		// Token: 0x06006A99 RID: 27289 RVA: 0x002145A6 File Offset: 0x002127A6
		public override void Reset()
		{
			this.tag = "Untagged";
			this.storeResult = null;
		}

		// Token: 0x06006A9A RID: 27290 RVA: 0x002145C0 File Offset: 0x002127C0
		public override void OnEnter()
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(this.tag.Value);
			if (this.storeResult != null)
			{
				this.storeResult.Value = ((array != null) ? array.Length : 0);
			}
			base.Finish();
		}

		// Token: 0x040069E3 RID: 27107
		[UIHint(UIHint.Tag)]
		[Tooltip("The Tag to search for.")]
		public FsmString tag;

		// Token: 0x040069E4 RID: 27108
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the number found in an int variable.")]
		public FsmInt storeResult;
	}
}
