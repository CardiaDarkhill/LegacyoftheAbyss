using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F75 RID: 3957
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Gets the shortest angle between two angles.")]
	public class FloatDeltaAngle : FsmStateAction
	{
		// Token: 0x06006DAB RID: 28075 RVA: 0x002210E2 File Offset: 0x0021F2E2
		public override void Reset()
		{
			this.fromAngle = null;
			this.toAngle = null;
			this.deltaAngle = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DAC RID: 28076 RVA: 0x00221100 File Offset: 0x0021F300
		public override void OnEnter()
		{
			this.DoDeltaAngle();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DAD RID: 28077 RVA: 0x00221116 File Offset: 0x0021F316
		public override void OnUpdate()
		{
			this.DoDeltaAngle();
		}

		// Token: 0x06006DAE RID: 28078 RVA: 0x0022111E File Offset: 0x0021F31E
		private void DoDeltaAngle()
		{
			this.deltaAngle.Value = Mathf.DeltaAngle(this.fromAngle.Value, this.toAngle.Value);
		}

		// Token: 0x04006D65 RID: 28005
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("First angle in degrees.")]
		public FsmFloat fromAngle;

		// Token: 0x04006D66 RID: 28006
		[RequiredField]
		[Tooltip("Second Angle in degrees.")]
		public FsmFloat toAngle;

		// Token: 0x04006D67 RID: 28007
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the shortest angle between the two angles. This takes account wrapping around 360.")]
		public FsmFloat deltaAngle;

		// Token: 0x04006D68 RID: 28008
		[Tooltip("Repeat every frame. Useful if the angles are changing.")]
		public bool everyFrame;
	}
}
