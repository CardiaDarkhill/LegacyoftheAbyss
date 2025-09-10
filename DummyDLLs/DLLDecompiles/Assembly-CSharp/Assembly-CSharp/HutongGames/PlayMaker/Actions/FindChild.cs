using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EA8 RID: 3752
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name.\nNote, you can specify a path to the child, e.g., LeftShoulder/Arm/Hand/Finger. If you need to specify a tag, use {{GetChild}}.")]
	public class FindChild : FsmStateAction
	{
		// Token: 0x06006A4D RID: 27213 RVA: 0x002137D0 File Offset: 0x002119D0
		public override void Reset()
		{
			this.gameObject = null;
			this.childName = "";
			this.storeResult = null;
		}

		// Token: 0x06006A4E RID: 27214 RVA: 0x002137F0 File Offset: 0x002119F0
		public override void OnEnter()
		{
			this.DoFindChild();
			base.Finish();
		}

		// Token: 0x06006A4F RID: 27215 RVA: 0x00213800 File Offset: 0x00211A00
		private void DoFindChild()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Transform transform = ownerDefaultTarget.transform.Find(this.childName.Value);
			this.storeResult.Value = ((transform != null) ? transform.gameObject : null);
		}

		// Token: 0x040069A4 RID: 27044
		[RequiredField]
		[Tooltip("The GameObject to search.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069A5 RID: 27045
		[RequiredField]
		[Tooltip("The name of the child. Note, you can specify a path to the child, e.g., LeftShoulder/Arm/Hand/Finger")]
		public FsmString childName;

		// Token: 0x040069A6 RID: 27046
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the child in a GameObject variable.")]
		public FsmGameObject storeResult;
	}
}
