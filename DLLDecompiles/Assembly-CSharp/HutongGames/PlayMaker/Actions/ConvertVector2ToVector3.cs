using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E73 RID: 3699
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Store a Vector2 XY components into a Vector3 XY component. The Vector3 z component is also accessible for convenience")]
	public class ConvertVector2ToVector3 : FsmStateAction
	{
		// Token: 0x06006971 RID: 26993 RVA: 0x002109E5 File Offset: 0x0020EBE5
		public override void Reset()
		{
			this.vector2 = null;
			this.vector3 = null;
			this.everyFrame = false;
		}

		// Token: 0x06006972 RID: 26994 RVA: 0x002109FC File Offset: 0x0020EBFC
		public override void OnEnter()
		{
			this.vector3.Value = new Vector3(this.vector2.Value.x, this.vector2.Value.y, this.zValue.Value);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006973 RID: 26995 RVA: 0x00210A52 File Offset: 0x0020EC52
		public override void OnUpdate()
		{
			this.vector3.Value = new Vector3(this.vector2.Value.x, this.vector2.Value.y, this.zValue.Value);
		}

		// Token: 0x040068AD RID: 26797
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector2")]
		public FsmVector2 vector2;

		// Token: 0x040068AE RID: 26798
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector3")]
		public FsmVector3 vector3;

		// Token: 0x040068AF RID: 26799
		[Tooltip("The vector3 z value")]
		public FsmFloat zValue;

		// Token: 0x040068B0 RID: 26800
		[Tooltip("Repeat every frame. Useful if the Vector2 value is changing.")]
		public bool everyFrame;
	}
}
