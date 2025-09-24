using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012DB RID: 4827
	public class RestBenchHelperState : FsmStateAction
	{
		// Token: 0x06007DE2 RID: 32226 RVA: 0x00257849 File Offset: 0x00255A49
		public override void Reset()
		{
			this.Target = null;
			this.HeroOnBench = null;
		}

		// Token: 0x06007DE3 RID: 32227 RVA: 0x0025785C File Offset: 0x00255A5C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				RestBenchHelper restBenchHelper = safe.AddComponentIfNotPresent<RestBenchHelper>();
				if (restBenchHelper != null)
				{
					restBenchHelper.SetOnBench(this.HeroOnBench.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007DC1 RID: 32193
		[RequiredField]
		[CheckForComponent(typeof(RestBenchHelper))]
		public FsmOwnerDefault Target;

		// Token: 0x04007DC2 RID: 32194
		public FsmBool HeroOnBench;
	}
}
