using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C49 RID: 3145
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds a value to a Float Variable.")]
	public class FloatAddV2 : FsmStateAction
	{
		// Token: 0x06005F62 RID: 24418 RVA: 0x001E4D15 File Offset: 0x001E2F15
		public override void Reset()
		{
			this.floatVariable = null;
			this.add = null;
			this.everyFrame = false;
			this.perSecond = false;
			this.fixedUpdate = false;
			this.activeBool = false;
		}

		// Token: 0x06005F63 RID: 24419 RVA: 0x001E4D46 File Offset: 0x001E2F46
		public override void Awake()
		{
			if (this.fixedUpdate)
			{
				base.Fsm.HandleFixedUpdate = true;
			}
		}

		// Token: 0x06005F64 RID: 24420 RVA: 0x001E4D5C File Offset: 0x001E2F5C
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005F65 RID: 24421 RVA: 0x001E4D6A File Offset: 0x001E2F6A
		public override void OnEnter()
		{
			this.DoFloatAdd();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F66 RID: 24422 RVA: 0x001E4D80 File Offset: 0x001E2F80
		public override void OnUpdate()
		{
			if (!this.fixedUpdate)
			{
				this.DoFloatAdd();
			}
		}

		// Token: 0x06005F67 RID: 24423 RVA: 0x001E4D90 File Offset: 0x001E2F90
		public override void OnFixedUpdate()
		{
			if (this.fixedUpdate)
			{
				this.DoFloatAdd();
			}
		}

		// Token: 0x06005F68 RID: 24424 RVA: 0x001E4DA0 File Offset: 0x001E2FA0
		private void DoFloatAdd()
		{
			if (this.activeBool.IsNone || this.activeBool.Value)
			{
				if (!this.perSecond)
				{
					this.floatVariable.Value += this.add.Value;
					return;
				}
				this.floatVariable.Value += this.add.Value * Time.deltaTime;
			}
		}

		// Token: 0x04005CC2 RID: 23746
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to add to.")]
		public FsmFloat floatVariable;

		// Token: 0x04005CC3 RID: 23747
		[RequiredField]
		[Tooltip("Amount to add.")]
		public FsmFloat add;

		// Token: 0x04005CC4 RID: 23748
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04005CC5 RID: 23749
		[Tooltip("Used with Every Frame. Adds the value over one second to make the operation frame rate independent.")]
		public bool perSecond;

		// Token: 0x04005CC6 RID: 23750
		public bool fixedUpdate;

		// Token: 0x04005CC7 RID: 23751
		[UIHint(UIHint.Variable)]
		public FsmBool activeBool;
	}
}
