using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E8B RID: 3723
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets the number of Touches.")]
	public class GetTouchCount : FsmStateAction
	{
		// Token: 0x060069CA RID: 27082 RVA: 0x00211640 File Offset: 0x0020F840
		public override void Reset()
		{
			this.storeCount = null;
			this.everyFrame = false;
		}

		// Token: 0x060069CB RID: 27083 RVA: 0x00211650 File Offset: 0x0020F850
		public override void OnEnter()
		{
			this.DoGetTouchCount();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069CC RID: 27084 RVA: 0x00211666 File Offset: 0x0020F866
		public override void OnUpdate()
		{
			this.DoGetTouchCount();
		}

		// Token: 0x060069CD RID: 27085 RVA: 0x0021166E File Offset: 0x0020F86E
		private void DoGetTouchCount()
		{
			this.storeCount.Value = Input.touchCount;
		}

		// Token: 0x040068F8 RID: 26872
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the current number of touches in an Int Variable.")]
		public FsmInt storeCount;

		// Token: 0x040068F9 RID: 26873
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
