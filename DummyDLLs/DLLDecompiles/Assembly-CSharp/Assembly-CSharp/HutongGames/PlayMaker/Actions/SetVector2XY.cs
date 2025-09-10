using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001186 RID: 4486
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Sets the XY channels of a Vector2 Variable. To leave any channel unchanged, set variable to 'None'.")]
	public class SetVector2XY : FsmStateAction
	{
		// Token: 0x0600783F RID: 30783 RVA: 0x002473C2 File Offset: 0x002455C2
		public override void Reset()
		{
			this.vector2Variable = null;
			this.vector2Value = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06007840 RID: 30784 RVA: 0x002473FD File Offset: 0x002455FD
		public override void OnEnter()
		{
			this.DoSetVector2XYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007841 RID: 30785 RVA: 0x00247413 File Offset: 0x00245613
		public override void OnUpdate()
		{
			this.DoSetVector2XYZ();
		}

		// Token: 0x06007842 RID: 30786 RVA: 0x0024741C File Offset: 0x0024561C
		private void DoSetVector2XYZ()
		{
			if (this.vector2Variable == null)
			{
				return;
			}
			Vector2 value = this.vector2Variable.Value;
			if (!this.vector2Value.IsNone)
			{
				value = this.vector2Value.Value;
			}
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			this.vector2Variable.Value = value;
		}

		// Token: 0x040078B2 RID: 30898
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector2 target")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078B3 RID: 30899
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector2 source")]
		public FsmVector2 vector2Value;

		// Token: 0x040078B4 RID: 30900
		[Tooltip("The x component. Override vector2Value if set")]
		public FsmFloat x;

		// Token: 0x040078B5 RID: 30901
		[Tooltip("The y component.Override vector2Value if set")]
		public FsmFloat y;

		// Token: 0x040078B6 RID: 30902
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
