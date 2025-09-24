using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001305 RID: 4869
	public class SetEnviroRegionProperties : FsmStateAction
	{
		// Token: 0x06007E8E RID: 32398 RVA: 0x002594E4 File Offset: 0x002576E4
		public override void Reset()
		{
			this.Target = null;
			this.EnviroType = new FsmEnum
			{
				UseVariable = true
			};
			this.Priority = new FsmInt
			{
				UseVariable = true
			};
		}

		// Token: 0x06007E8F RID: 32399 RVA: 0x00259514 File Offset: 0x00257714
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				EnviroRegion component = safe.GetComponent<EnviroRegion>();
				if (component)
				{
					if (!this.EnviroType.IsNone)
					{
						component.EnvironmentType = (EnvironmentTypes)this.EnviroType.Value;
					}
					if (!this.Priority.IsNone)
					{
						component.Priority = this.Priority.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007E4C RID: 32332
		public FsmOwnerDefault Target;

		// Token: 0x04007E4D RID: 32333
		[ObjectType(typeof(EnvironmentTypes))]
		public FsmEnum EnviroType;

		// Token: 0x04007E4E RID: 32334
		public FsmInt Priority;
	}
}
