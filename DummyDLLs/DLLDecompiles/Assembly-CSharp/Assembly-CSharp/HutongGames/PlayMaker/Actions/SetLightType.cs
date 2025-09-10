using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F3C RID: 3900
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Set Spot, Directional, or Point Light type.")]
	public class SetLightType : ComponentAction<Light>
	{
		// Token: 0x06006C97 RID: 27799 RVA: 0x0021E17E File Offset: 0x0021C37E
		public override void Reset()
		{
			this.gameObject = null;
			this.lightType = LightType.Point;
		}

		// Token: 0x06006C98 RID: 27800 RVA: 0x0021E198 File Offset: 0x0021C398
		public override void OnEnter()
		{
			this.DoSetLightType();
			base.Finish();
		}

		// Token: 0x06006C99 RID: 27801 RVA: 0x0021E1A8 File Offset: 0x0021C3A8
		private void DoSetLightType()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.type = (LightType)this.lightType.Value;
			}
		}

		// Token: 0x04006C56 RID: 27734
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		[Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C57 RID: 27735
		[ObjectType(typeof(LightType))]
		[Tooltip("Spot, directional, or point light.")]
		public FsmEnum lightType;
	}
}
