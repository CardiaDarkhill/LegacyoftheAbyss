using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F5B RID: 3931
	[ActionCategory(ActionCategory.Logic)]
	public class PlayerdataIntCompare : FsmStateAction
	{
		// Token: 0x06006D30 RID: 27952 RVA: 0x0021F91D File Offset: 0x0021DB1D
		public override void Reset()
		{
			this.playerdataInt = null;
			this.compareTo = 0;
			this.equal = null;
			this.lessThan = null;
			this.greaterThan = null;
		}

		// Token: 0x06006D31 RID: 27953 RVA: 0x0021F947 File Offset: 0x0021DB47
		public override void OnEnter()
		{
			this.DoIntCompare();
			base.Finish();
		}

		// Token: 0x06006D32 RID: 27954 RVA: 0x0021F958 File Offset: 0x0021DB58
		private void DoIntCompare()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				return;
			}
			int @int = instance.playerData.GetInt(this.playerdataInt.Value);
			if (@int == this.compareTo.Value)
			{
				base.Fsm.Event(this.equal);
				return;
			}
			if (@int < this.compareTo.Value)
			{
				base.Fsm.Event(this.lessThan);
				return;
			}
			if (@int > this.compareTo.Value)
			{
				base.Fsm.Event(this.greaterThan);
			}
		}

		// Token: 0x06006D33 RID: 27955 RVA: 0x0021F9EB File Offset: 0x0021DBEB
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThan) && FsmEvent.IsNullOrEmpty(this.greaterThan))
			{
				return "Action sends no events!";
			}
			return "";
		}

		// Token: 0x04006CF3 RID: 27891
		[RequiredField]
		public FsmString playerdataInt;

		// Token: 0x04006CF4 RID: 27892
		[RequiredField]
		public FsmInt compareTo;

		// Token: 0x04006CF5 RID: 27893
		[Tooltip("Event sent if Int 1 equals Int 2")]
		public FsmEvent equal;

		// Token: 0x04006CF6 RID: 27894
		[Tooltip("Event sent if Int 1 is less than Int 2")]
		public FsmEvent lessThan;

		// Token: 0x04006CF7 RID: 27895
		[Tooltip("Event sent if Int 1 is greater than Int 2")]
		public FsmEvent greaterThan;
	}
}
