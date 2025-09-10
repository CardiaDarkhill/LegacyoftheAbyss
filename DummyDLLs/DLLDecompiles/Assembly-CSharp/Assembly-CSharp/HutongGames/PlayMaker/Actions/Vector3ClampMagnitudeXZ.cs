using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011A1 RID: 4513
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Clamps the magnitude of Vector3 variable on the XZ Plane.")]
	public class Vector3ClampMagnitudeXZ : FsmStateAction
	{
		// Token: 0x060078B9 RID: 30905 RVA: 0x00248867 File Offset: 0x00246A67
		public override void Reset()
		{
			this.vector3Variable = null;
			this.maxLength = null;
			this.everyFrame = false;
		}

		// Token: 0x060078BA RID: 30906 RVA: 0x0024887E File Offset: 0x00246A7E
		public override void OnEnter()
		{
			this.DoVector3ClampMagnitudeXZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078BB RID: 30907 RVA: 0x00248894 File Offset: 0x00246A94
		public override void OnUpdate()
		{
			this.DoVector3ClampMagnitudeXZ();
		}

		// Token: 0x060078BC RID: 30908 RVA: 0x0024889C File Offset: 0x00246A9C
		private void DoVector3ClampMagnitudeXZ()
		{
			Vector2 vector = Vector2.ClampMagnitude(new Vector2(this.vector3Variable.Value.x, this.vector3Variable.Value.z), this.maxLength.Value);
			this.vector3Variable.Value = new Vector3(vector.x, this.vector3Variable.Value.y, vector.y);
		}

		// Token: 0x0400791D RID: 31005
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 variable to clamp.")]
		public FsmVector3 vector3Variable;

		// Token: 0x0400791E RID: 31006
		[RequiredField]
		[Tooltip("Clamp to this max length.")]
		public FsmFloat maxLength;

		// Token: 0x0400791F RID: 31007
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
