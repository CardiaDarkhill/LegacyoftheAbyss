using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001183 RID: 4483
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Get the XY channels of a Vector2 Variable and store them in Float Variables.")]
	public class GetVector2XY : FsmStateAction
	{
		// Token: 0x06007832 RID: 30770 RVA: 0x00247205 File Offset: 0x00245405
		public override void Reset()
		{
			this.vector2Variable = null;
			this.storeX = null;
			this.storeY = null;
			this.everyFrame = false;
		}

		// Token: 0x06007833 RID: 30771 RVA: 0x00247223 File Offset: 0x00245423
		public override void OnEnter()
		{
			this.DoGetVector2XYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007834 RID: 30772 RVA: 0x00247239 File Offset: 0x00245439
		public override void OnUpdate()
		{
			this.DoGetVector2XYZ();
		}

		// Token: 0x06007835 RID: 30773 RVA: 0x00247244 File Offset: 0x00245444
		private void DoGetVector2XYZ()
		{
			if (this.vector2Variable == null)
			{
				return;
			}
			if (this.storeX != null)
			{
				this.storeX.Value = this.vector2Variable.Value.x;
			}
			if (this.storeY != null)
			{
				this.storeY.Value = this.vector2Variable.Value.y;
			}
		}

		// Token: 0x040078A8 RID: 30888
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector2 source")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078A9 RID: 30889
		[UIHint(UIHint.Variable)]
		[Tooltip("The x component")]
		public FsmFloat storeX;

		// Token: 0x040078AA RID: 30890
		[UIHint(UIHint.Variable)]
		[Tooltip("The y component")]
		public FsmFloat storeY;

		// Token: 0x040078AB RID: 30891
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
