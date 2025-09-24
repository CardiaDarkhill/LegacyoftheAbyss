using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F3A RID: 3898
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Range of a Light.")]
	public class SetLightRange : ComponentAction<Light>
	{
		// Token: 0x06006C8D RID: 27789 RVA: 0x0021E076 File Offset: 0x0021C276
		public override void Reset()
		{
			this.gameObject = null;
			this.lightRange = 20f;
			this.everyFrame = false;
		}

		// Token: 0x06006C8E RID: 27790 RVA: 0x0021E096 File Offset: 0x0021C296
		public override void OnEnter()
		{
			this.DoSetLightRange();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006C8F RID: 27791 RVA: 0x0021E0AC File Offset: 0x0021C2AC
		public override void OnUpdate()
		{
			this.DoSetLightRange();
		}

		// Token: 0x06006C90 RID: 27792 RVA: 0x0021E0B4 File Offset: 0x0021C2B4
		private void DoSetLightRange()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.range = this.lightRange.Value;
			}
		}

		// Token: 0x04006C50 RID: 27728
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		[Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C51 RID: 27729
		[Tooltip("The range of the light.")]
		public FsmFloat lightRange;

		// Token: 0x04006C52 RID: 27730
		[Tooltip("Update every frame. Useful if the range is changing.")]
		public bool everyFrame;
	}
}
