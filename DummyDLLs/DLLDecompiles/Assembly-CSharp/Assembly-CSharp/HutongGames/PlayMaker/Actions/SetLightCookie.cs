using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F37 RID: 3895
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Texture projected by a Light.")]
	public class SetLightCookie : ComponentAction<Light>
	{
		// Token: 0x06006C80 RID: 27776 RVA: 0x0021DF2E File Offset: 0x0021C12E
		public override void Reset()
		{
			this.gameObject = null;
			this.lightCookie = null;
		}

		// Token: 0x06006C81 RID: 27777 RVA: 0x0021DF3E File Offset: 0x0021C13E
		public override void OnEnter()
		{
			this.DoSetLightCookie();
			base.Finish();
		}

		// Token: 0x06006C82 RID: 27778 RVA: 0x0021DF4C File Offset: 0x0021C14C
		private void DoSetLightCookie()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.cookie = this.lightCookie.Value;
			}
		}

		// Token: 0x04006C49 RID: 27721
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		[Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C4A RID: 27722
		[Tooltip("The texture to project.")]
		public FsmTexture lightCookie;
	}
}
