using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011AA RID: 4522
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Multiplies a Vector3 variable by Time.deltaTime. Useful for frame rate independent motion.")]
	public class Vector3PerSecond : FsmStateAction
	{
		// Token: 0x060078E0 RID: 30944 RVA: 0x00249197 File Offset: 0x00247397
		public override void Reset()
		{
			this.vector3Variable = null;
			this.everyFrame = false;
		}

		// Token: 0x060078E1 RID: 30945 RVA: 0x002491A7 File Offset: 0x002473A7
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * Time.deltaTime;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078E2 RID: 30946 RVA: 0x002491D7 File Offset: 0x002473D7
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * Time.deltaTime;
		}

		// Token: 0x04007941 RID: 31041
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 variable to multiply by Time.deltaTime.")]
		public FsmVector3 vector3Variable;

		// Token: 0x04007942 RID: 31042
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
