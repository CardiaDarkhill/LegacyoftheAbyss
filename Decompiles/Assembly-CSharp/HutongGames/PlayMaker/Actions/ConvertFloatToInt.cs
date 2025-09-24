using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E6C RID: 3692
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Float value to an Integer value.")]
	public class ConvertFloatToInt : FsmStateAction
	{
		// Token: 0x0600694E RID: 26958 RVA: 0x00210559 File Offset: 0x0020E759
		public override void Reset()
		{
			this.floatVariable = null;
			this.intVariable = null;
			this.rounding = ConvertFloatToInt.FloatRounding.Nearest;
			this.everyFrame = false;
		}

		// Token: 0x0600694F RID: 26959 RVA: 0x00210577 File Offset: 0x0020E777
		public override void OnEnter()
		{
			this.DoConvertFloatToInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006950 RID: 26960 RVA: 0x0021058D File Offset: 0x0020E78D
		public override void OnUpdate()
		{
			this.DoConvertFloatToInt();
		}

		// Token: 0x06006951 RID: 26961 RVA: 0x00210598 File Offset: 0x0020E798
		private void DoConvertFloatToInt()
		{
			switch (this.rounding)
			{
			case ConvertFloatToInt.FloatRounding.RoundDown:
				this.intVariable.Value = Mathf.FloorToInt(this.floatVariable.Value);
				return;
			case ConvertFloatToInt.FloatRounding.RoundUp:
				this.intVariable.Value = Mathf.CeilToInt(this.floatVariable.Value);
				return;
			case ConvertFloatToInt.FloatRounding.Nearest:
				this.intVariable.Value = Mathf.RoundToInt(this.floatVariable.Value);
				return;
			default:
				return;
			}
		}

		// Token: 0x04006894 RID: 26772
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to convert to an integer.")]
		public FsmFloat floatVariable;

		// Token: 0x04006895 RID: 26773
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in an Integer variable.")]
		public FsmInt intVariable;

		// Token: 0x04006896 RID: 26774
		[Tooltip("Whether to round up or down.")]
		public ConvertFloatToInt.FloatRounding rounding;

		// Token: 0x04006897 RID: 26775
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x02001BA3 RID: 7075
		public enum FloatRounding
		{
			// Token: 0x04009E02 RID: 40450
			RoundDown,
			// Token: 0x04009E03 RID: 40451
			RoundUp,
			// Token: 0x04009E04 RID: 40452
			Nearest
		}
	}
}
