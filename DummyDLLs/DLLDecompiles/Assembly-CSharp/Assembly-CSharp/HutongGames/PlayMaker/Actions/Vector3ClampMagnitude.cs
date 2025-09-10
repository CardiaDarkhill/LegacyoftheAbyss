using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011A0 RID: 4512
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Clamps the Magnitude of Vector3 Variable.")]
	public class Vector3ClampMagnitude : FsmStateAction
	{
		// Token: 0x060078B4 RID: 30900 RVA: 0x00248802 File Offset: 0x00246A02
		public override void Reset()
		{
			this.vector3Variable = null;
			this.maxLength = null;
			this.everyFrame = false;
		}

		// Token: 0x060078B5 RID: 30901 RVA: 0x00248819 File Offset: 0x00246A19
		public override void OnEnter()
		{
			this.DoVector3ClampMagnitude();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078B6 RID: 30902 RVA: 0x0024882F File Offset: 0x00246A2F
		public override void OnUpdate()
		{
			this.DoVector3ClampMagnitude();
		}

		// Token: 0x060078B7 RID: 30903 RVA: 0x00248837 File Offset: 0x00246A37
		private void DoVector3ClampMagnitude()
		{
			this.vector3Variable.Value = Vector3.ClampMagnitude(this.vector3Variable.Value, this.maxLength.Value);
		}

		// Token: 0x0400791A RID: 31002
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 variable to clamp.")]
		public FsmVector3 vector3Variable;

		// Token: 0x0400791B RID: 31003
		[RequiredField]
		[Tooltip("Clamp to this max length.")]
		public FsmFloat maxLength;

		// Token: 0x0400791C RID: 31004
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
