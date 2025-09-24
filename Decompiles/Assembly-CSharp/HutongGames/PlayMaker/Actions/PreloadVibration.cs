using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200139C RID: 5020
	[ActionCategory("Hollow Knight")]
	public class PreloadVibration : FsmStateAction
	{
		// Token: 0x060080CB RID: 32971 RVA: 0x0025F1EC File Offset: 0x0025D3EC
		public override void Reset()
		{
			base.Reset();
			this.lowFidelityVibration = new FsmEnum
			{
				UseVariable = false
			};
			this.highFidelityVibration = new FsmObject
			{
				UseVariable = false
			};
		}

		// Token: 0x060080CC RID: 32972 RVA: 0x0025F218 File Offset: 0x0025D418
		public override void OnEnter()
		{
			base.OnEnter();
			base.Finish();
		}

		// Token: 0x04008001 RID: 32769
		[ObjectType(typeof(LowFidelityVibrations))]
		private FsmEnum lowFidelityVibration;

		// Token: 0x04008002 RID: 32770
		[ObjectType(typeof(TextAsset))]
		private FsmObject highFidelityVibration;
	}
}
