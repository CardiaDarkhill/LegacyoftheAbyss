using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F01 RID: 3841
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets the value of the specified Input Axis and stores it in a Float Variable. See {{Unity Input Manager}} docs.")]
	[SeeAlso("Unity Input Manager")]
	public class GetAxis : FsmStateAction
	{
		// Token: 0x06006B7F RID: 27519 RVA: 0x002174EA File Offset: 0x002156EA
		public override void Reset()
		{
			this.axisName = "";
			this.multiplier = 1f;
			this.invert = null;
			this.store = null;
			this.everyFrame = true;
		}

		// Token: 0x06006B80 RID: 27520 RVA: 0x00217521 File Offset: 0x00215721
		public override void OnEnter()
		{
			this.DoGetAxis();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B81 RID: 27521 RVA: 0x00217537 File Offset: 0x00215737
		public override void OnUpdate()
		{
			this.DoGetAxis();
		}

		// Token: 0x06006B82 RID: 27522 RVA: 0x00217540 File Offset: 0x00215740
		private void DoGetAxis()
		{
			if (FsmString.IsNullOrEmpty(this.axisName))
			{
				return;
			}
			float num = Input.GetAxis(this.axisName.Value);
			if (!this.multiplier.IsNone)
			{
				num *= this.multiplier.Value;
			}
			if (this.invert.Value)
			{
				num *= -1f;
			}
			this.store.Value = num;
		}

		// Token: 0x04006AD1 RID: 27345
		[RequiredField]
		[Tooltip("The name of the axis. Set in the Unity Input Manager.")]
		public FsmString axisName;

		// Token: 0x04006AD2 RID: 27346
		[Tooltip("Normally axis values are in the range -1 to 1. Use the multiplier to make this range bigger. E.g., A multiplier of 100 returns values from -100 to 100.")]
		public FsmFloat multiplier;

		// Token: 0x04006AD3 RID: 27347
		[Tooltip("Invert the value of for the axis. E.g., -1 becomes 1, and 1 becomes -1.")]
		public FsmBool invert;

		// Token: 0x04006AD4 RID: 27348
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a float variable.")]
		public FsmFloat store;

		// Token: 0x04006AD5 RID: 27349
		[Tooltip("Get the axis value every frame. This should be true most of the time, but there might be times when you only want to get the value once.")]
		public bool everyFrame;
	}
}
