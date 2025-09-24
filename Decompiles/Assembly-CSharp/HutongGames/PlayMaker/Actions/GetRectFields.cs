using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001010 RID: 4112
	[ActionCategory(ActionCategory.Rect)]
	[Tooltip("Get the individual fields of a Rect Variable and store them in Float Variables.")]
	public class GetRectFields : FsmStateAction
	{
		// Token: 0x0600710D RID: 28941 RVA: 0x0022CDD8 File Offset: 0x0022AFD8
		public override void Reset()
		{
			this.rectVariable = null;
			this.storeX = null;
			this.storeY = null;
			this.storeWidth = null;
			this.storeHeight = null;
			this.storeMin = null;
			this.storeMax = null;
			this.storeSize = null;
			this.storeCenter = null;
			this.everyFrame = false;
		}

		// Token: 0x0600710E RID: 28942 RVA: 0x0022CE2B File Offset: 0x0022B02B
		public override void OnEnter()
		{
			this.DoGetRectFields();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600710F RID: 28943 RVA: 0x0022CE41 File Offset: 0x0022B041
		public override void OnUpdate()
		{
			this.DoGetRectFields();
		}

		// Token: 0x06007110 RID: 28944 RVA: 0x0022CE4C File Offset: 0x0022B04C
		private void DoGetRectFields()
		{
			if (this.rectVariable.IsNone)
			{
				return;
			}
			this.storeX.Value = this.rectVariable.Value.x;
			this.storeY.Value = this.rectVariable.Value.y;
			this.storeWidth.Value = this.rectVariable.Value.width;
			this.storeHeight.Value = this.rectVariable.Value.height;
			this.storeMin.Value = this.rectVariable.Value.min;
			this.storeMax.Value = this.rectVariable.Value.max;
			this.storeSize.Value = this.rectVariable.Value.size;
			this.storeCenter.Value = this.rectVariable.Value.center;
		}

		// Token: 0x040070A0 RID: 28832
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Rect Variable.")]
		public FsmRect rectVariable;

		// Token: 0x040070A1 RID: 28833
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X value in a Float Variable.")]
		public FsmFloat storeX;

		// Token: 0x040070A2 RID: 28834
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X value in a Float Variable.")]
		public FsmFloat storeY;

		// Token: 0x040070A3 RID: 28835
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Width in a Float Variable.")]
		public FsmFloat storeWidth;

		// Token: 0x040070A4 RID: 28836
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Height in a Float Variable.")]
		public FsmFloat storeHeight;

		// Token: 0x040070A5 RID: 28837
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Min position in a Vector2 Variable.")]
		public FsmVector2 storeMin;

		// Token: 0x040070A6 RID: 28838
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Max position in a Vector2 Variable.")]
		public FsmVector2 storeMax;

		// Token: 0x040070A7 RID: 28839
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Size in a Vector2 Variable.")]
		public FsmVector2 storeSize;

		// Token: 0x040070A8 RID: 28840
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Center in a Vector2 Variable.")]
		public FsmVector2 storeCenter;

		// Token: 0x040070A9 RID: 28841
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
