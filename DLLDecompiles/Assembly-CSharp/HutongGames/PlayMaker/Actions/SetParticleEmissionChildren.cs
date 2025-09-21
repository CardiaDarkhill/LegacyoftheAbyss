using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D37 RID: 3383
	public class SetParticleEmissionChildren : FsmStateAction
	{
		// Token: 0x06006373 RID: 25459 RVA: 0x001F6409 File Offset: 0x001F4609
		public override void Reset()
		{
			this.Target = null;
			this.SetEmission = null;
			this.EveryFrame = false;
		}

		// Token: 0x06006374 RID: 25460 RVA: 0x001F6420 File Offset: 0x001F4620
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe == null)
			{
				base.Finish();
				return;
			}
			this.childSystems = safe.GetComponentsInChildren<ParticleSystem>(true);
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006375 RID: 25461 RVA: 0x001F646B File Offset: 0x001F466B
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06006376 RID: 25462 RVA: 0x001F6474 File Offset: 0x001F4674
		private void DoAction()
		{
			ParticleSystem[] array = this.childSystems;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].emission.enabled = this.SetEmission.Value;
			}
		}

		// Token: 0x06006377 RID: 25463 RVA: 0x001F64B4 File Offset: 0x001F46B4
		public override void OnExit()
		{
			if (this.stopOnExit)
			{
				ParticleSystem[] array = this.childSystems;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].emission.enabled = false;
				}
			}
			base.OnExit();
		}

		// Token: 0x040061CC RID: 25036
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x040061CD RID: 25037
		public FsmBool SetEmission;

		// Token: 0x040061CE RID: 25038
		public bool EveryFrame;

		// Token: 0x040061CF RID: 25039
		public bool stopOnExit;

		// Token: 0x040061D0 RID: 25040
		private ParticleSystem[] childSystems;
	}
}
