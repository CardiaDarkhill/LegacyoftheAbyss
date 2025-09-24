using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EA7 RID: 3751
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Unparents all children from the Game Object.")]
	public class DetachChildren : FsmStateAction
	{
		// Token: 0x06006A49 RID: 27209 RVA: 0x0021378B File Offset: 0x0021198B
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006A4A RID: 27210 RVA: 0x00213794 File Offset: 0x00211994
		public override void OnEnter()
		{
			DetachChildren.DoDetachChildren(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x06006A4B RID: 27211 RVA: 0x002137B2 File Offset: 0x002119B2
		private static void DoDetachChildren(GameObject go)
		{
			if (go != null)
			{
				go.transform.DetachChildren();
			}
		}

		// Token: 0x040069A3 RID: 27043
		[RequiredField]
		[Tooltip("GameObject to unparent children from.")]
		public FsmOwnerDefault gameObject;
	}
}
