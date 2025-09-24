using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F73 RID: 3955
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds multiple float variables to float variable.")]
	public class FloatAddMultiple : FsmStateAction
	{
		// Token: 0x06006DA1 RID: 28065 RVA: 0x00220FEE File Offset: 0x0021F1EE
		public override void Reset()
		{
			this.floatVariables = null;
			this.addTo = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DA2 RID: 28066 RVA: 0x00221005 File Offset: 0x0021F205
		public override void OnEnter()
		{
			this.DoFloatAdd();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DA3 RID: 28067 RVA: 0x0022101B File Offset: 0x0021F21B
		public override void OnUpdate()
		{
			this.DoFloatAdd();
		}

		// Token: 0x06006DA4 RID: 28068 RVA: 0x00221024 File Offset: 0x0021F224
		private void DoFloatAdd()
		{
			for (int i = 0; i < this.floatVariables.Length; i++)
			{
				this.addTo.Value += this.floatVariables[i].Value;
			}
		}

		// Token: 0x04006D5E RID: 27998
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variables to add.")]
		public FsmFloat[] floatVariables;

		// Token: 0x04006D5F RID: 27999
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Add to this variable.")]
		public FsmFloat addTo;

		// Token: 0x04006D60 RID: 28000
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
