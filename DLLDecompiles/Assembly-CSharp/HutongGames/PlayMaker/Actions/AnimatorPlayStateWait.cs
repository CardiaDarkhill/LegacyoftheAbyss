using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAF RID: 2991
	[Tooltip("Plays an animator state and waits until it's completion. Will never finish if state or animator can not be found.")]
	public class AnimatorPlayStateWait : FsmStateAction
	{
		// Token: 0x06005C48 RID: 23624 RVA: 0x001D1246 File Offset: 0x001CF446
		public override void Reset()
		{
			this.target = null;
			this.stateName = null;
			this.finishEvent = null;
		}

		// Token: 0x06005C49 RID: 23625 RVA: 0x001D1260 File Offset: 0x001CF460
		public override void OnEnter()
		{
			this.hasWaited = false;
			this.resumeTime = null;
			GameObject safe = this.target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			this.animator = safe.GetComponent<Animator>();
			if (this.animator && !string.IsNullOrWhiteSpace(this.stateName.Value))
			{
				this.animator.Play(this.stateName.Value, 0, 0f);
			}
		}

		// Token: 0x06005C4A RID: 23626 RVA: 0x001D12E0 File Offset: 0x001CF4E0
		public override void OnUpdate()
		{
			if (this.hasWaited)
			{
				if (this.animator && this.resumeTime == null)
				{
					AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
					this.resumeTime = new double?((double)currentAnimatorStateInfo.length + Time.timeAsDouble);
					this.stateHash = currentAnimatorStateInfo.shortNameHash;
				}
				if (this.resumeTime != null)
				{
					double timeAsDouble = Time.timeAsDouble;
					double? num = this.resumeTime;
					if (timeAsDouble >= num.GetValueOrDefault() & num != null)
					{
						goto IL_A3;
					}
				}
				if (this.animator.GetCurrentAnimatorStateInfo(0).shortNameHash == this.stateHash)
				{
					return;
				}
				IL_A3:
				base.Fsm.Event(this.finishEvent);
				base.Finish();
				return;
			}
			this.hasWaited = true;
		}

		// Token: 0x040057C0 RID: 22464
		public FsmOwnerDefault target;

		// Token: 0x040057C1 RID: 22465
		public FsmString stateName;

		// Token: 0x040057C2 RID: 22466
		public FsmEvent finishEvent;

		// Token: 0x040057C3 RID: 22467
		private Animator animator;

		// Token: 0x040057C4 RID: 22468
		private bool hasWaited;

		// Token: 0x040057C5 RID: 22469
		private double? resumeTime;

		// Token: 0x040057C6 RID: 22470
		private int stateHash;
	}
}
