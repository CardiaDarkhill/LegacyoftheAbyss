using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001318 RID: 4888
	public class DoSurfaceWaterSplash : FsmStateAction
	{
		// Token: 0x06007EDB RID: 32475 RVA: 0x00259F92 File Offset: 0x00258192
		public override void Reset()
		{
			this.Splasher = null;
			this.SplashSurface = null;
			this.SplashType = null;
		}

		// Token: 0x06007EDC RID: 32476 RVA: 0x00259FAC File Offset: 0x002581AC
		public override void OnEnter()
		{
			GameObject safe = this.Splasher.GetSafe(this);
			if (safe && this.SplashSurface.Value)
			{
				SurfaceWaterRegion componentInParent = this.SplashSurface.Value.GetComponentInParent<SurfaceWaterRegion>();
				if (componentInParent)
				{
					DoSurfaceWaterSplash.SplashTypes splashTypes = (DoSurfaceWaterSplash.SplashTypes)this.SplashType.Value;
					if (splashTypes != DoSurfaceWaterSplash.SplashTypes.In)
					{
						if (splashTypes == DoSurfaceWaterSplash.SplashTypes.Out)
						{
							componentInParent.DoSplashOut(safe.transform, Vector2.zero);
						}
					}
					else
					{
						componentInParent.DoSplashIn(safe.transform, false);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007E85 RID: 32389
		public FsmOwnerDefault Splasher;

		// Token: 0x04007E86 RID: 32390
		public FsmGameObject SplashSurface;

		// Token: 0x04007E87 RID: 32391
		[ObjectType(typeof(DoSurfaceWaterSplash.SplashTypes))]
		public FsmEnum SplashType;

		// Token: 0x02001BF2 RID: 7154
		public enum SplashTypes
		{
			// Token: 0x04009FA9 RID: 40873
			In,
			// Token: 0x04009FAA RID: 40874
			Out
		}
	}
}
