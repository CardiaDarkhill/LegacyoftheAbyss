using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011A6 RID: 4518
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Use a low pass filter to reduce the influence of sudden changes in a Vector3 Variable. Useful when working with Get Device Acceleration to isolate gravity.")]
	public class Vector3LowPassFilter : FsmStateAction
	{
		// Token: 0x060078CF RID: 30927 RVA: 0x00248D0A File Offset: 0x00246F0A
		public override void Reset()
		{
			this.vector3Variable = null;
			this.filteringFactor = 0.1f;
		}

		// Token: 0x060078D0 RID: 30928 RVA: 0x00248D23 File Offset: 0x00246F23
		public override void OnEnter()
		{
			this.filteredVector = new Vector3(this.vector3Variable.Value.x, this.vector3Variable.Value.y, this.vector3Variable.Value.z);
		}

		// Token: 0x060078D1 RID: 30929 RVA: 0x00248D60 File Offset: 0x00246F60
		public override void OnUpdate()
		{
			this.filteredVector.x = this.vector3Variable.Value.x * this.filteringFactor.Value + this.filteredVector.x * (1f - this.filteringFactor.Value);
			this.filteredVector.y = this.vector3Variable.Value.y * this.filteringFactor.Value + this.filteredVector.y * (1f - this.filteringFactor.Value);
			this.filteredVector.z = this.vector3Variable.Value.z * this.filteringFactor.Value + this.filteredVector.z * (1f - this.filteringFactor.Value);
			this.vector3Variable.Value = new Vector3(this.filteredVector.x, this.filteredVector.y, this.filteredVector.z);
		}

		// Token: 0x04007933 RID: 31027
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector3 Variable to filter. Should generally come from some constantly updated input, e.g., acceleration.")]
		public FsmVector3 vector3Variable;

		// Token: 0x04007934 RID: 31028
		[Tooltip("Determines how much influence new changes have. E.g., 0.1 keeps 10 percent of the unfiltered vector and 90 percent of the previously filtered value.")]
		public FsmFloat filteringFactor;

		// Token: 0x04007935 RID: 31029
		private Vector3 filteredVector;
	}
}
