using System;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001261 RID: 4705
	public class GetHeroCStateEvent : FsmStateAction
	{
		// Token: 0x06007C35 RID: 31797 RVA: 0x0025242B File Offset: 0x0025062B
		public override void Reset()
		{
			this.VariableName = null;
			this.StoreValue = null;
			this.EveryFrame = false;
			this.TrueEvent = null;
			this.FalseEvent = null;
		}

		// Token: 0x06007C36 RID: 31798 RVA: 0x00252450 File Offset: 0x00250650
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007C37 RID: 31799 RVA: 0x00252466 File Offset: 0x00250666
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007C38 RID: 31800 RVA: 0x00252470 File Offset: 0x00250670
		private void DoAction()
		{
			if (this.VariableName.IsNone)
			{
				base.Finish();
				return;
			}
			bool cstate = HeroController.instance.GetCState(this.VariableName.Value);
			if (!this.StoreValue.IsNone)
			{
				this.StoreValue.Value = cstate;
			}
			if (cstate)
			{
				base.Fsm.Event(this.TrueEvent);
				return;
			}
			base.Fsm.Event(this.FalseEvent);
		}

		// Token: 0x06007C39 RID: 31801 RVA: 0x002524E8 File Offset: 0x002506E8
		public override string ErrorCheck()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (string.IsNullOrEmpty(this.VariableName.Value))
			{
				stringBuilder.AppendLine("State name must be specified!");
			}
			else if (!HeroController.CStateExists(this.VariableName.Value))
			{
				stringBuilder.AppendLine("State could not be found in HeroControllerStates");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04007C46 RID: 31814
		public FsmString VariableName;

		// Token: 0x04007C47 RID: 31815
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		// Token: 0x04007C48 RID: 31816
		public FsmEvent TrueEvent;

		// Token: 0x04007C49 RID: 31817
		public FsmEvent FalseEvent;

		// Token: 0x04007C4A RID: 31818
		public bool EveryFrame;
	}
}
