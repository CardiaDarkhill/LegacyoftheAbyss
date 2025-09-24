using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200100A RID: 4106
	[ActionCategory(ActionCategory.Quaternion)]
	[Tooltip("Use a low pass filter to reduce the influence of sudden changes in a quaternion Variable.")]
	public class QuaternionLowPassFilter : QuaternionBaseAction
	{
		// Token: 0x060070EF RID: 28911 RVA: 0x0022C8EC File Offset: 0x0022AAEC
		public override void Reset()
		{
			this.quaternionVariable = null;
			this.filteringFactor = 0.1f;
			this.everyFrame = true;
			this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
		}

		// Token: 0x060070F0 RID: 28912 RVA: 0x0022C914 File Offset: 0x0022AB14
		public override void OnEnter()
		{
			this.filteredQuaternion = new Quaternion(this.quaternionVariable.Value.x, this.quaternionVariable.Value.y, this.quaternionVariable.Value.z, this.quaternionVariable.Value.w);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060070F1 RID: 28913 RVA: 0x0022C97A File Offset: 0x0022AB7A
		public override void OnUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.Update)
			{
				this.DoQuatLowPassFilter();
			}
		}

		// Token: 0x060070F2 RID: 28914 RVA: 0x0022C98A File Offset: 0x0022AB8A
		public override void OnLateUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.LateUpdate)
			{
				this.DoQuatLowPassFilter();
			}
		}

		// Token: 0x060070F3 RID: 28915 RVA: 0x0022C99B File Offset: 0x0022AB9B
		public override void OnFixedUpdate()
		{
			if (this.everyFrameOption == QuaternionBaseAction.everyFrameOptions.FixedUpdate)
			{
				this.DoQuatLowPassFilter();
			}
		}

		// Token: 0x060070F4 RID: 28916 RVA: 0x0022C9AC File Offset: 0x0022ABAC
		private void DoQuatLowPassFilter()
		{
			this.filteredQuaternion.x = this.quaternionVariable.Value.x * this.filteringFactor.Value + this.filteredQuaternion.x * (1f - this.filteringFactor.Value);
			this.filteredQuaternion.y = this.quaternionVariable.Value.y * this.filteringFactor.Value + this.filteredQuaternion.y * (1f - this.filteringFactor.Value);
			this.filteredQuaternion.z = this.quaternionVariable.Value.z * this.filteringFactor.Value + this.filteredQuaternion.z * (1f - this.filteringFactor.Value);
			this.filteredQuaternion.w = this.quaternionVariable.Value.w * this.filteringFactor.Value + this.filteredQuaternion.w * (1f - this.filteringFactor.Value);
			this.quaternionVariable.Value = new Quaternion(this.filteredQuaternion.x, this.filteredQuaternion.y, this.filteredQuaternion.z, this.filteredQuaternion.w);
		}

		// Token: 0x04007090 RID: 28816
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("quaternion Variable to filter. Should generally come from some constantly updated input")]
		public FsmQuaternion quaternionVariable;

		// Token: 0x04007091 RID: 28817
		[Tooltip("Determines how much influence new changes have. E.g., 0.1 keeps 10 percent of the unfiltered quaternion and 90 percent of the previously filtered value.")]
		public FsmFloat filteringFactor;

		// Token: 0x04007092 RID: 28818
		private Quaternion filteredQuaternion;
	}
}
