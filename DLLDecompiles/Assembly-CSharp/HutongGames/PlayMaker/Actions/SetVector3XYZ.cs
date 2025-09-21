using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200119D RID: 4509
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Sets the XYZ channels of a Vector3 Variable. To leave any channel unchanged, set variable to 'None'.")]
	public class SetVector3XYZ : FsmStateAction
	{
		// Token: 0x060078A5 RID: 30885 RVA: 0x00248534 File Offset: 0x00246734
		public override void Reset()
		{
			this.vector3Variable = null;
			this.vector3Value = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x060078A6 RID: 30886 RVA: 0x0024858C File Offset: 0x0024678C
		public override void OnEnter()
		{
			this.DoSetVector3XYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078A7 RID: 30887 RVA: 0x002485A2 File Offset: 0x002467A2
		public override void OnUpdate()
		{
			this.DoSetVector3XYZ();
		}

		// Token: 0x060078A8 RID: 30888 RVA: 0x002485AC File Offset: 0x002467AC
		private void DoSetVector3XYZ()
		{
			if (this.vector3Variable == null)
			{
				return;
			}
			Vector3 value = this.vector3Variable.Value;
			if (!this.vector3Value.IsNone)
			{
				value = this.vector3Value.Value;
			}
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				value.z = this.z.Value;
			}
			this.vector3Variable.Value = value;
		}

		// Token: 0x0400790A RID: 30986
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector3 variable to set.")]
		public FsmVector3 vector3Variable;

		// Token: 0x0400790B RID: 30987
		[Tooltip("Set using another Vector3 variable and/or individual channels below.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 vector3Value;

		// Token: 0x0400790C RID: 30988
		[Tooltip("Set X channel.")]
		public FsmFloat x;

		// Token: 0x0400790D RID: 30989
		[Tooltip("Set Y channel.")]
		public FsmFloat y;

		// Token: 0x0400790E RID: 30990
		[Tooltip("Set Z channel.")]
		public FsmFloat z;

		// Token: 0x0400790F RID: 30991
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
