using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F38 RID: 3896
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Flare effect used by a Light.")]
	public class SetLightFlare : ComponentAction<Light>
	{
		// Token: 0x06006C84 RID: 27780 RVA: 0x0021DF92 File Offset: 0x0021C192
		public override void Reset()
		{
			this.gameObject = null;
			this.lightFlare = null;
		}

		// Token: 0x06006C85 RID: 27781 RVA: 0x0021DFA2 File Offset: 0x0021C1A2
		public override void OnEnter()
		{
			this.DoSetLightRange();
			base.Finish();
		}

		// Token: 0x06006C86 RID: 27782 RVA: 0x0021DFB0 File Offset: 0x0021C1B0
		private void DoSetLightRange()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.flare = this.lightFlare;
			}
		}

		// Token: 0x04006C4B RID: 27723
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		[Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C4C RID: 27724
		[Tooltip("The flare to use.")]
		public Flare lightFlare;
	}
}
