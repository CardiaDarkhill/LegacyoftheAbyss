using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F4F RID: 3919
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Compares 2 Game Objects and sends Events based on the result.")]
	public class GameObjectCompare : FsmStateAction
	{
		// Token: 0x06006CF4 RID: 27892 RVA: 0x0021F05A File Offset: 0x0021D25A
		public override void Reset()
		{
			this.gameObjectVariable = null;
			this.compareTo = null;
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CF5 RID: 27893 RVA: 0x0021F086 File Offset: 0x0021D286
		public override void OnEnter()
		{
			this.DoGameObjectCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CF6 RID: 27894 RVA: 0x0021F09C File Offset: 0x0021D29C
		public override void OnUpdate()
		{
			this.DoGameObjectCompare();
		}

		// Token: 0x06006CF7 RID: 27895 RVA: 0x0021F0A4 File Offset: 0x0021D2A4
		private void DoGameObjectCompare()
		{
			bool flag = base.Fsm.GetOwnerDefaultTarget(this.gameObjectVariable) == this.compareTo.Value;
			this.storeResult.Value = flag;
			if (flag && this.equalEvent != null)
			{
				base.Fsm.Event(this.equalEvent);
				return;
			}
			if (!flag && this.notEqualEvent != null)
			{
				base.Fsm.Event(this.notEqualEvent);
			}
		}

		// Token: 0x04006CB4 RID: 27828
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Title("Game Object")]
		[Tooltip("A Game Object variable to compare.")]
		public FsmOwnerDefault gameObjectVariable;

		// Token: 0x04006CB5 RID: 27829
		[Tooltip("Compare the variable with this Game Object")]
		public FsmGameObject compareTo;

		// Token: 0x04006CB6 RID: 27830
		[Tooltip("Send this event if Game Objects are equal")]
		public FsmEvent equalEvent;

		// Token: 0x04006CB7 RID: 27831
		[Tooltip("Send this event if Game Objects are not equal")]
		public FsmEvent notEqualEvent;

		// Token: 0x04006CB8 RID: 27832
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of the check in a Bool Variable. (True if equal, false if not equal).")]
		public FsmBool storeResult;

		// Token: 0x04006CB9 RID: 27833
		[Tooltip("Repeat every frame. Useful if you're waiting for a true or false result.")]
		public bool everyFrame;
	}
}
