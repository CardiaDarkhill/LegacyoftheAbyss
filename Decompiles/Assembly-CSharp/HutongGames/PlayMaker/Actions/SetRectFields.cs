using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001013 RID: 4115
	[ActionCategory(ActionCategory.Rect)]
	[Tooltip("Sets the individual fields of a Rect Variable. To leave any field unchanged, set variable to 'None'.")]
	public class SetRectFields : FsmStateAction
	{
		// Token: 0x0600711E RID: 28958 RVA: 0x0022D24C File Offset: 0x0022B44C
		public override void Reset()
		{
			this.rectVariable = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.width = new FsmFloat
			{
				UseVariable = true
			};
			this.height = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x0600711F RID: 28959 RVA: 0x0022D2AF File Offset: 0x0022B4AF
		public override void OnEnter()
		{
			this.DoSetRectFields();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007120 RID: 28960 RVA: 0x0022D2C5 File Offset: 0x0022B4C5
		public override void OnUpdate()
		{
			this.DoSetRectFields();
		}

		// Token: 0x06007121 RID: 28961 RVA: 0x0022D2D0 File Offset: 0x0022B4D0
		private void DoSetRectFields()
		{
			if (this.rectVariable.IsNone)
			{
				return;
			}
			Rect value = this.rectVariable.Value;
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			if (!this.width.IsNone)
			{
				value.width = this.width.Value;
			}
			if (!this.height.IsNone)
			{
				value.height = this.height.Value;
			}
			this.rectVariable.Value = value;
		}

		// Token: 0x040070B8 RID: 28856
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Rect Variable to set.")]
		public FsmRect rectVariable;

		// Token: 0x040070B9 RID: 28857
		[Tooltip("Set X value.")]
		public FsmFloat x;

		// Token: 0x040070BA RID: 28858
		[Tooltip("Set Y value.")]
		public FsmFloat y;

		// Token: 0x040070BB RID: 28859
		[Tooltip("Set Width.")]
		public FsmFloat width;

		// Token: 0x040070BC RID: 28860
		[Tooltip("Set Height.")]
		public FsmFloat height;

		// Token: 0x040070BD RID: 28861
		[Tooltip("Repeat every frame. Useful if the fields are animated.")]
		public bool everyFrame;
	}
}
