using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001193 RID: 4499
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Multiplies a Vector2 variable by Time.deltaTime. Useful for frame rate independent motion.")]
	public class Vector2PerSecond : FsmStateAction
	{
		// Token: 0x06007879 RID: 30841 RVA: 0x00247EAE File Offset: 0x002460AE
		public override void Reset()
		{
			this.vector2Variable = null;
			this.everyFrame = true;
		}

		// Token: 0x0600787A RID: 30842 RVA: 0x00247EBE File Offset: 0x002460BE
		public override void OnEnter()
		{
			this.vector2Variable.Value = this.vector2Variable.Value * Time.deltaTime;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600787B RID: 30843 RVA: 0x00247EEE File Offset: 0x002460EE
		public override void OnUpdate()
		{
			this.vector2Variable.Value = this.vector2Variable.Value * Time.deltaTime;
		}

		// Token: 0x040078E9 RID: 30953
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector2")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078EA RID: 30954
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
