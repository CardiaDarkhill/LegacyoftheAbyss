using System;
using System.Text;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200124A RID: 4682
	public class SetHeroCStateDelay : FsmStateAction
	{
		// Token: 0x06007BC9 RID: 31689 RVA: 0x0025070F File Offset: 0x0024E90F
		public override void Reset()
		{
			this.VariableName = null;
			this.Value = null;
			this.Delay = 0f;
			this.SetOppositeOnExit = false;
			this.timer = 0f;
		}

		// Token: 0x06007BCA RID: 31690 RVA: 0x0025073C File Offset: 0x0024E93C
		public override void OnEnter()
		{
			this.timer = 0f;
		}

		// Token: 0x06007BCB RID: 31691 RVA: 0x00250749 File Offset: 0x0024E949
		public override void OnExit()
		{
			if (this.SetOppositeOnExit)
			{
				HeroController.instance.SetCState(this.VariableName.Value, !this.Value.Value);
			}
		}

		// Token: 0x06007BCC RID: 31692 RVA: 0x00250776 File Offset: 0x0024E976
		public override void OnUpdate()
		{
			if (this.timer <= this.Delay)
			{
				this.timer += Time.deltaTime;
				return;
			}
			this.DoAction();
			base.Finish();
		}

		// Token: 0x06007BCD RID: 31693 RVA: 0x002507A5 File Offset: 0x0024E9A5
		private void DoAction()
		{
			if (!this.VariableName.IsNone)
			{
				HeroController.instance.SetCState(this.VariableName.Value, this.Value.Value);
			}
		}

		// Token: 0x06007BCE RID: 31694 RVA: 0x002507D4 File Offset: 0x0024E9D4
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

		// Token: 0x04007BF9 RID: 31737
		public FsmString VariableName;

		// Token: 0x04007BFA RID: 31738
		public FsmBool Value;

		// Token: 0x04007BFB RID: 31739
		public float Delay;

		// Token: 0x04007BFC RID: 31740
		public bool SetOppositeOnExit;

		// Token: 0x04007BFD RID: 31741
		private float timer;
	}
}
