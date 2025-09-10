using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F23 RID: 3875
	[ActionCategory("iTween")]
	[Tooltip("Resume an iTween action.")]
	public class iTweenResume : FsmStateAction
	{
		// Token: 0x06006C24 RID: 27684 RVA: 0x0021B71E File Offset: 0x0021991E
		public override void Reset()
		{
			this.iTweenType = iTweenFSMType.all;
			this.includeChildren = false;
			this.inScene = false;
		}

		// Token: 0x06006C25 RID: 27685 RVA: 0x0021B735 File Offset: 0x00219935
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoiTween();
			base.Finish();
		}

		// Token: 0x06006C26 RID: 27686 RVA: 0x0021B74C File Offset: 0x0021994C
		private void DoiTween()
		{
			if (this.iTweenType == iTweenFSMType.all)
			{
				iTween.Resume();
				return;
			}
			if (this.inScene)
			{
				iTween.Resume(Enum.GetName(typeof(iTweenFSMType), this.iTweenType));
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			iTween.Resume(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), this.iTweenType), this.includeChildren);
		}

		// Token: 0x04006BC6 RID: 27590
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006BC7 RID: 27591
		public iTweenFSMType iTweenType;

		// Token: 0x04006BC8 RID: 27592
		public bool includeChildren;

		// Token: 0x04006BC9 RID: 27593
		public bool inScene;
	}
}
