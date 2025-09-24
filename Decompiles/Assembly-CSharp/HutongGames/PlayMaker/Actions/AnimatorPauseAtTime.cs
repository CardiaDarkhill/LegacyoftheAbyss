using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAE RID: 2990
	public class AnimatorPauseAtTime : FsmStateAction
	{
		// Token: 0x06005C45 RID: 23621 RVA: 0x001D11BB File Offset: 0x001CF3BB
		public override void Reset()
		{
			this.Target = null;
			this.Anim = null;
			this.Time = null;
		}

		// Token: 0x06005C46 RID: 23622 RVA: 0x001D11D4 File Offset: 0x001CF3D4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			Animator animator = safe ? safe.GetComponent<Animator>() : null;
			if (animator != null)
			{
				animator.Play(this.Anim.Name, 0, this.Time.Value);
				animator.enabled = false;
				animator.Update(0f);
			}
			base.Finish();
		}

		// Token: 0x040057BD RID: 22461
		public FsmOwnerDefault Target;

		// Token: 0x040057BE RID: 22462
		public FsmString Anim;

		// Token: 0x040057BF RID: 22463
		public FsmFloat Time;
	}
}
