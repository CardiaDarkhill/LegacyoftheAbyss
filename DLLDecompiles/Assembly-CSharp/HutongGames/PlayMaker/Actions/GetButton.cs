using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F03 RID: 3843
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets the pressed state of the specified Button and stores it in a Bool Variable. See Unity Input Manager docs.")]
	public class GetButton : FsmStateAction
	{
		// Token: 0x06006B87 RID: 27527 RVA: 0x002177D6 File Offset: 0x002159D6
		public override void Reset()
		{
			this.buttonName = "Fire1";
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x06006B88 RID: 27528 RVA: 0x002177F6 File Offset: 0x002159F6
		public override void OnEnter()
		{
			this.DoGetButton();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B89 RID: 27529 RVA: 0x0021780C File Offset: 0x00215A0C
		public override void OnUpdate()
		{
			this.DoGetButton();
		}

		// Token: 0x06006B8A RID: 27530 RVA: 0x00217814 File Offset: 0x00215A14
		private void DoGetButton()
		{
			this.storeResult.Value = Input.GetButton(this.buttonName.Value);
		}

		// Token: 0x04006ADD RID: 27357
		[RequiredField]
		[Tooltip("The name of the button. Set in the Unity Input Manager.")]
		public FsmString buttonName;

		// Token: 0x04006ADE RID: 27358
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the button state in a Bool Variable.")]
		public FsmBool storeResult;

		// Token: 0x04006ADF RID: 27359
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
