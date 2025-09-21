using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EBF RID: 3775
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets a Game Object's Name.")]
	public class SetName : FsmStateAction
	{
		// Token: 0x06006AB3 RID: 27315 RVA: 0x00214971 File Offset: 0x00212B71
		public override void Reset()
		{
			this.gameObject = null;
			this.name = null;
		}

		// Token: 0x06006AB4 RID: 27316 RVA: 0x00214981 File Offset: 0x00212B81
		public override void OnEnter()
		{
			this.DoSetLayer();
			base.Finish();
		}

		// Token: 0x06006AB5 RID: 27317 RVA: 0x00214990 File Offset: 0x00212B90
		private void DoSetLayer()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			ownerDefaultTarget.name = this.name.Value;
		}

		// Token: 0x040069F8 RID: 27128
		[RequiredField]
		[Tooltip("The Game Object to name.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069F9 RID: 27129
		[RequiredField]
		[Tooltip("The new name.")]
		public FsmString name;
	}
}
