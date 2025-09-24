using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F31 RID: 3889
	[ActionCategory("iTween")]
	[Tooltip("Stop an iTween action.")]
	public class iTweenStop : FsmStateAction
	{
		// Token: 0x06006C6B RID: 27755 RVA: 0x0021DAB6 File Offset: 0x0021BCB6
		public override void Reset()
		{
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.iTweenType = iTweenFSMType.all;
			this.includeChildren = false;
			this.inScene = false;
		}

		// Token: 0x06006C6C RID: 27756 RVA: 0x0021DADF File Offset: 0x0021BCDF
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoiTween();
			base.Finish();
		}

		// Token: 0x06006C6D RID: 27757 RVA: 0x0021DAF4 File Offset: 0x0021BCF4
		private void DoiTween()
		{
			if (!this.id.IsNone)
			{
				iTween.StopByName(this.id.Value);
				return;
			}
			if (this.iTweenType == iTweenFSMType.all)
			{
				iTween.Stop();
				return;
			}
			if (this.inScene)
			{
				iTween.Stop(Enum.GetName(typeof(iTweenFSMType), this.iTweenType));
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			iTween.Stop(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), this.iTweenType), this.includeChildren);
		}

		// Token: 0x04006C34 RID: 27700
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C35 RID: 27701
		public FsmString id;

		// Token: 0x04006C36 RID: 27702
		public iTweenFSMType iTweenType;

		// Token: 0x04006C37 RID: 27703
		public bool includeChildren;

		// Token: 0x04006C38 RID: 27704
		public bool inScene;
	}
}
