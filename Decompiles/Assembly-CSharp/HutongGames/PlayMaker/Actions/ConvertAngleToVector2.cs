using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFF RID: 3071
	[ActionCategory(ActionCategory.Vector2)]
	public class ConvertAngleToVector2 : FsmStateAction
	{
		// Token: 0x06005DDF RID: 24031 RVA: 0x001D9A71 File Offset: 0x001D7C71
		public override void Reset()
		{
			this.storeVector = null;
			this.angle = null;
			this.everyFrame = false;
		}

		// Token: 0x06005DE0 RID: 24032 RVA: 0x001D9A88 File Offset: 0x001D7C88
		public override void OnEnter()
		{
			this.DoCalculate();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DE1 RID: 24033 RVA: 0x001D9A9E File Offset: 0x001D7C9E
		public override void OnUpdate()
		{
			this.DoCalculate();
		}

		// Token: 0x06005DE2 RID: 24034 RVA: 0x001D9AA8 File Offset: 0x001D7CA8
		private void DoCalculate()
		{
			float num;
			for (num = this.angle.Value; num > 360f; num -= 360f)
			{
			}
			while (num < 360f)
			{
				num += 360f;
			}
			float f = 0.017453292f * num;
			this.storeVector.Value = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		}

		// Token: 0x04005A39 RID: 23097
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat angle;

		// Token: 0x04005A3A RID: 23098
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmVector2 storeVector;

		// Token: 0x04005A3B RID: 23099
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
