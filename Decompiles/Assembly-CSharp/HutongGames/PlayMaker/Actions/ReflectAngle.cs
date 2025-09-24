using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF8 RID: 3320
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Reflect the selected angle horizontally or vertically.")]
	public class ReflectAngle : FsmStateAction
	{
		// Token: 0x06006272 RID: 25202 RVA: 0x001F2597 File Offset: 0x001F0797
		public override void Reset()
		{
			this.angle = null;
			this.reflectHorizontally = false;
			this.reflectVertically = false;
			this.disallowNegative = false;
			this.storeResult = null;
		}

		// Token: 0x06006273 RID: 25203 RVA: 0x001F25BC File Offset: 0x001F07BC
		public override void OnEnter()
		{
			this.DoReflectAngle();
			base.Finish();
		}

		// Token: 0x06006274 RID: 25204 RVA: 0x001F25CC File Offset: 0x001F07CC
		private void DoReflectAngle()
		{
			float num = this.angle.Value;
			if (this.reflectHorizontally)
			{
				num = 180f - num;
			}
			if (this.reflectVertically)
			{
				num = -num;
			}
			while (num > 360f)
			{
				num -= 360f;
			}
			while (num < -360f)
			{
				num += 360f;
			}
			if (this.disallowNegative)
			{
				while (num < 0f)
				{
					num += 360f;
				}
			}
			this.storeResult.Value = num;
		}

		// Token: 0x040060D9 RID: 24793
		[RequiredField]
		[Tooltip("The angle to reflect. Must be expressed in degrees, ")]
		public FsmFloat angle;

		// Token: 0x040060DA RID: 24794
		public bool reflectHorizontally;

		// Token: 0x040060DB RID: 24795
		public bool reflectVertically;

		// Token: 0x040060DC RID: 24796
		public bool disallowNegative;

		// Token: 0x040060DD RID: 24797
		[Tooltip("Float to store the reflected angle in.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;
	}
}
