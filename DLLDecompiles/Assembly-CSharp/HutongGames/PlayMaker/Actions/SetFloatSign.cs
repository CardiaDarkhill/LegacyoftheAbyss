using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F5D RID: 3933
	[ActionCategory(ActionCategory.Math)]
	public class SetFloatSign : FsmStateAction
	{
		// Token: 0x06006D3B RID: 27963 RVA: 0x0021FBC2 File Offset: 0x0021DDC2
		public override void Reset()
		{
			this.floatValue = null;
			this.setPositive = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D3C RID: 27964 RVA: 0x0021FBD9 File Offset: 0x0021DDD9
		public override void OnEnter()
		{
			this.DoSignTest();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D3D RID: 27965 RVA: 0x0021FBEF File Offset: 0x0021DDEF
		public override void OnUpdate()
		{
			this.DoSignTest();
		}

		// Token: 0x06006D3E RID: 27966 RVA: 0x0021FBF8 File Offset: 0x0021DDF8
		private void DoSignTest()
		{
			if (this.floatValue == null)
			{
				return;
			}
			if (this.setPositive.Value)
			{
				if (this.floatValue.Value < 0f)
				{
					this.floatValue.Value *= -1f;
					return;
				}
			}
			else if (this.floatValue.Value > 0f)
			{
				this.floatValue.Value *= -1f;
			}
		}

		// Token: 0x04006CFE RID: 27902
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatValue;

		// Token: 0x04006CFF RID: 27903
		public FsmBool setPositive;

		// Token: 0x04006D00 RID: 27904
		[Tooltip("Repeat every frame. Useful if the variable is changing and you're waiting for a particular result.")]
		public bool everyFrame;
	}
}
