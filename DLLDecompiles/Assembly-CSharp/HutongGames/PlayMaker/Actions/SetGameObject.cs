using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EBD RID: 3773
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the value of a Game Object Variable.")]
	public class SetGameObject : FsmStateAction
	{
		// Token: 0x06006AAB RID: 27307 RVA: 0x002148B9 File Offset: 0x00212AB9
		public override void Reset()
		{
			this.variable = null;
			this.gameObject = null;
			this.everyFrame = false;
		}

		// Token: 0x06006AAC RID: 27308 RVA: 0x002148D0 File Offset: 0x00212AD0
		public override void OnEnter()
		{
			this.variable.Value = this.gameObject.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006AAD RID: 27309 RVA: 0x002148F6 File Offset: 0x00212AF6
		public override void OnUpdate()
		{
			this.variable.Value = this.gameObject.Value;
		}

		// Token: 0x040069F3 RID: 27123
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The GameObject Variable to set.")]
		public FsmGameObject variable;

		// Token: 0x040069F4 RID: 27124
		[Tooltip("Set the variable value. NOTE: leave empty to set to null.")]
		public FsmGameObject gameObject;

		// Token: 0x040069F5 RID: 27125
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
