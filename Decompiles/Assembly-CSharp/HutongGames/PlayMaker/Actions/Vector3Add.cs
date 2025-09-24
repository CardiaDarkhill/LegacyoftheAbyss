using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200119E RID: 4510
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Adds a value to Vector3 Variable.")]
	public class Vector3Add : FsmStateAction
	{
		// Token: 0x060078AA RID: 30890 RVA: 0x00248658 File Offset: 0x00246858
		public override void Reset()
		{
			this.vector3Variable = null;
			this.addVector = new FsmVector3
			{
				UseVariable = true
			};
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x060078AB RID: 30891 RVA: 0x00248681 File Offset: 0x00246881
		public override void OnEnter()
		{
			this.DoVector3Add();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078AC RID: 30892 RVA: 0x00248697 File Offset: 0x00246897
		public override void OnUpdate()
		{
			this.DoVector3Add();
		}

		// Token: 0x060078AD RID: 30893 RVA: 0x002486A0 File Offset: 0x002468A0
		private void DoVector3Add()
		{
			if (this.perSecond)
			{
				this.vector3Variable.Value = this.vector3Variable.Value + this.addVector.Value * Time.deltaTime;
				return;
			}
			this.vector3Variable.Value = this.vector3Variable.Value + this.addVector.Value;
		}

		// Token: 0x04007910 RID: 30992
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector3 variable to add to.")]
		public FsmVector3 vector3Variable;

		// Token: 0x04007911 RID: 30993
		[RequiredField]
		[Tooltip("Vector3 to add.")]
		public FsmVector3 addVector;

		// Token: 0x04007912 RID: 30994
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007913 RID: 30995
		[Tooltip("Add over one second (multiplies values by Time.deltaTime). Note: Needs Every Frame checked.")]
		public bool perSecond;
	}
}
