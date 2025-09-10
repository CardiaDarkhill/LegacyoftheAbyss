using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010F0 RID: 4336
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the Arc sine. You can get the result in degrees, simply check on the RadToDeg conversion")]
	public class GetASine : FsmStateAction
	{
		// Token: 0x0600754B RID: 30027 RVA: 0x0023DBCF File Offset: 0x0023BDCF
		public override void Reset()
		{
			this.angle = null;
			this.RadToDeg = true;
			this.everyFrame = false;
			this.Value = null;
		}

		// Token: 0x0600754C RID: 30028 RVA: 0x0023DBF2 File Offset: 0x0023BDF2
		public override void OnEnter()
		{
			this.DoASine();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600754D RID: 30029 RVA: 0x0023DC08 File Offset: 0x0023BE08
		public override void OnUpdate()
		{
			this.DoASine();
		}

		// Token: 0x0600754E RID: 30030 RVA: 0x0023DC10 File Offset: 0x0023BE10
		private void DoASine()
		{
			float num = Mathf.Asin(this.Value.Value);
			if (this.RadToDeg.Value)
			{
				num *= 57.29578f;
			}
			this.angle.Value = num;
		}

		// Token: 0x040075BB RID: 30139
		[RequiredField]
		[Tooltip("The value of the sine")]
		public FsmFloat Value;

		// Token: 0x040075BC RID: 30140
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
		public FsmFloat angle;

		// Token: 0x040075BD RID: 30141
		[Tooltip("Check on if you want the angle expressed in degrees.")]
		public FsmBool RadToDeg;

		// Token: 0x040075BE RID: 30142
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
