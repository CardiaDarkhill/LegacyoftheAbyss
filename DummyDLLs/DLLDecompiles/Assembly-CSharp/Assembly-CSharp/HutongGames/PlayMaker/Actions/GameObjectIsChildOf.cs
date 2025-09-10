using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F52 RID: 3922
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if a game object is a child of another game object and stores the result in a bool variable.\nE.g., Uses this to check if a collision object is the child of another object.")]
	public class GameObjectIsChildOf : FsmStateAction
	{
		// Token: 0x06006D03 RID: 27907 RVA: 0x0021F290 File Offset: 0x0021D490
		public override void Reset()
		{
			this.gameObject = null;
			this.isChildOf = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006D04 RID: 27908 RVA: 0x0021F2B5 File Offset: 0x0021D4B5
		public override void OnEnter()
		{
			this.DoIsChildOf(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x06006D05 RID: 27909 RVA: 0x0021F2D4 File Offset: 0x0021D4D4
		private void DoIsChildOf(GameObject go)
		{
			if (go == null || this.isChildOf == null)
			{
				return;
			}
			bool flag = go.transform.IsChildOf(this.isChildOf.Value.transform);
			this.storeResult.Value = flag;
			base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x04006CC5 RID: 27845
		[RequiredField]
		[Tooltip("GameObject to test.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006CC6 RID: 27846
		[RequiredField]
		[Tooltip("Is it a child of this GameObject?")]
		public FsmGameObject isChildOf;

		// Token: 0x04006CC7 RID: 27847
		[Tooltip("Event to send if GameObject is a child.")]
		public FsmEvent trueEvent;

		// Token: 0x04006CC8 RID: 27848
		[Tooltip("Event to send if GameObject is NOT a child.")]
		public FsmEvent falseEvent;

		// Token: 0x04006CC9 RID: 27849
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store result in a bool variable")]
		public FsmBool storeResult;
	}
}
