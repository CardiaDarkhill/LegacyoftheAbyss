using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F3F RID: 3903
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if any of the given Bool Variables are True.")]
	public class BoolAnyTrue : FsmStateAction
	{
		// Token: 0x06006CA5 RID: 27813 RVA: 0x0021E31F File Offset: 0x0021C51F
		public override void Reset()
		{
			this.boolVariables = null;
			this.sendEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CA6 RID: 27814 RVA: 0x0021E33D File Offset: 0x0021C53D
		public override void OnEnter()
		{
			this.DoAnyTrue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CA7 RID: 27815 RVA: 0x0021E353 File Offset: 0x0021C553
		public override void OnUpdate()
		{
			this.DoAnyTrue();
		}

		// Token: 0x06006CA8 RID: 27816 RVA: 0x0021E35C File Offset: 0x0021C55C
		private void DoAnyTrue()
		{
			if (this.boolVariables.Length == 0)
			{
				return;
			}
			bool value = false;
			FsmBool[] array = this.boolVariables;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Value)
				{
					base.Fsm.Event(this.sendEvent);
					value = true;
					break;
				}
			}
			this.storeResult.Value = value;
		}

		// Token: 0x04006C5F RID: 27743
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variables to check.")]
		public FsmBool[] boolVariables;

		// Token: 0x04006C60 RID: 27744
		[Tooltip("Event to send if any of the Bool variables are True.")]
		public FsmEvent sendEvent;

		// Token: 0x04006C61 RID: 27745
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006C62 RID: 27746
		[Tooltip("Repeat every frame while the state is active. Useful if you're waiting for any to be true.")]
		public bool everyFrame;
	}
}
