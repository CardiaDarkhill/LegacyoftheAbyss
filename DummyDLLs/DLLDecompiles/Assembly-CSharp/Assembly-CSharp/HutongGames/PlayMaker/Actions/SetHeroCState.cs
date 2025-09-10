using System;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001249 RID: 4681
	public class SetHeroCState : FsmStateAction
	{
		// Token: 0x06007BC2 RID: 31682 RVA: 0x00250617 File Offset: 0x0024E817
		public override void Reset()
		{
			this.VariableName = null;
			this.Value = null;
			this.EveryFrame = false;
			this.SetOppositeOnExit = false;
		}

		// Token: 0x06007BC3 RID: 31683 RVA: 0x00250635 File Offset: 0x0024E835
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007BC4 RID: 31684 RVA: 0x0025064B File Offset: 0x0024E84B
		public override void OnExit()
		{
			if (this.SetOppositeOnExit)
			{
				HeroController.instance.SetCState(this.VariableName.Value, !this.Value.Value);
			}
		}

		// Token: 0x06007BC5 RID: 31685 RVA: 0x00250678 File Offset: 0x0024E878
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007BC6 RID: 31686 RVA: 0x00250680 File Offset: 0x0024E880
		private void DoAction()
		{
			if (!this.VariableName.IsNone)
			{
				HeroController.instance.SetCState(this.VariableName.Value, this.Value.Value);
			}
		}

		// Token: 0x06007BC7 RID: 31687 RVA: 0x002506B0 File Offset: 0x0024E8B0
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

		// Token: 0x04007BF5 RID: 31733
		public FsmString VariableName;

		// Token: 0x04007BF6 RID: 31734
		public FsmBool Value;

		// Token: 0x04007BF7 RID: 31735
		public bool EveryFrame;

		// Token: 0x04007BF8 RID: 31736
		public bool SetOppositeOnExit;
	}
}
