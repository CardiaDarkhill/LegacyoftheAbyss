using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F1F RID: 3871
	[ActionCategory("iTween")]
	[Tooltip("Pause an iTween action.")]
	public class iTweenPause : FsmStateAction
	{
		// Token: 0x06006C11 RID: 27665 RVA: 0x0021AF57 File Offset: 0x00219157
		public override void Reset()
		{
			this.iTweenType = iTweenFSMType.all;
			this.includeChildren = false;
			this.inScene = false;
		}

		// Token: 0x06006C12 RID: 27666 RVA: 0x0021AF6E File Offset: 0x0021916E
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoiTween();
			base.Finish();
		}

		// Token: 0x06006C13 RID: 27667 RVA: 0x0021AF84 File Offset: 0x00219184
		private void DoiTween()
		{
			if (this.iTweenType == iTweenFSMType.all)
			{
				iTween.Pause();
				return;
			}
			if (this.inScene)
			{
				iTween.Pause(Enum.GetName(typeof(iTweenFSMType), this.iTweenType));
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			iTween.Pause(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), this.iTweenType), this.includeChildren);
		}

		// Token: 0x04006BAD RID: 27565
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006BAE RID: 27566
		public iTweenFSMType iTweenType;

		// Token: 0x04006BAF RID: 27567
		public bool includeChildren;

		// Token: 0x04006BB0 RID: 27568
		public bool inScene;
	}
}
