using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010A6 RID: 4262
	[Tooltip("Base class for actions that want to run a sub FSM.")]
	public abstract class RunFSMAction : FsmStateAction
	{
		// Token: 0x060073BD RID: 29629 RVA: 0x00238144 File Offset: 0x00236344
		public override void Reset()
		{
			this.runFsm = null;
		}

		// Token: 0x060073BE RID: 29630 RVA: 0x0023814D File Offset: 0x0023634D
		public override bool Event(FsmEvent fsmEvent)
		{
			if (this.runFsm != null && (fsmEvent.IsGlobal || fsmEvent.IsSystemEvent))
			{
				this.runFsm.Event(fsmEvent);
			}
			return false;
		}

		// Token: 0x060073BF RID: 29631 RVA: 0x00238174 File Offset: 0x00236374
		public override void OnEnter()
		{
			if (this.runFsm == null)
			{
				base.Finish();
				return;
			}
			this.runFsm.OnEnable();
			if (!this.runFsm.Started)
			{
				this.runFsm.Start();
			}
			this.CheckIfFinished();
		}

		// Token: 0x060073C0 RID: 29632 RVA: 0x002381AE File Offset: 0x002363AE
		public override void OnUpdate()
		{
			if (this.runFsm != null)
			{
				this.runFsm.Update();
				this.CheckIfFinished();
				return;
			}
			base.Finish();
		}

		// Token: 0x060073C1 RID: 29633 RVA: 0x002381D0 File Offset: 0x002363D0
		public override void OnFixedUpdate()
		{
			if (this.runFsm != null)
			{
				this.runFsm.FixedUpdate();
				this.CheckIfFinished();
				return;
			}
			base.Finish();
		}

		// Token: 0x060073C2 RID: 29634 RVA: 0x002381F2 File Offset: 0x002363F2
		public override void OnLateUpdate()
		{
			if (this.runFsm != null)
			{
				this.runFsm.LateUpdate();
				this.CheckIfFinished();
				return;
			}
			base.Finish();
		}

		// Token: 0x060073C3 RID: 29635 RVA: 0x00238214 File Offset: 0x00236414
		public override void DoTriggerEnter(Collider other)
		{
			if (this.runFsm.HandleTriggerEnter)
			{
				this.runFsm.OnTriggerEnter(other);
			}
		}

		// Token: 0x060073C4 RID: 29636 RVA: 0x0023822F File Offset: 0x0023642F
		public override void DoTriggerStay(Collider other)
		{
			if (this.runFsm.HandleTriggerStay)
			{
				this.runFsm.OnTriggerStay(other);
			}
		}

		// Token: 0x060073C5 RID: 29637 RVA: 0x0023824A File Offset: 0x0023644A
		public override void DoTriggerExit(Collider other)
		{
			if (this.runFsm.HandleTriggerExit)
			{
				this.runFsm.OnTriggerExit(other);
			}
		}

		// Token: 0x060073C6 RID: 29638 RVA: 0x00238265 File Offset: 0x00236465
		public override void DoCollisionEnter(Collision collisionInfo)
		{
			if (this.runFsm.HandleCollisionEnter)
			{
				this.runFsm.OnCollisionEnter(collisionInfo);
			}
		}

		// Token: 0x060073C7 RID: 29639 RVA: 0x00238280 File Offset: 0x00236480
		public override void DoCollisionStay(Collision collisionInfo)
		{
			if (this.runFsm.HandleCollisionStay)
			{
				this.runFsm.OnCollisionStay(collisionInfo);
			}
		}

		// Token: 0x060073C8 RID: 29640 RVA: 0x0023829B File Offset: 0x0023649B
		public override void DoCollisionExit(Collision collisionInfo)
		{
			if (this.runFsm.HandleCollisionExit)
			{
				this.runFsm.OnCollisionExit(collisionInfo);
			}
		}

		// Token: 0x060073C9 RID: 29641 RVA: 0x002382B6 File Offset: 0x002364B6
		public override void DoParticleCollision(GameObject other)
		{
			if (this.runFsm.HandleParticleCollision)
			{
				this.runFsm.OnParticleCollision(other);
			}
		}

		// Token: 0x060073CA RID: 29642 RVA: 0x002382D1 File Offset: 0x002364D1
		public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
		{
			if (this.runFsm.HandleControllerColliderHit)
			{
				this.runFsm.OnControllerColliderHit(collisionInfo);
			}
		}

		// Token: 0x060073CB RID: 29643 RVA: 0x002382EC File Offset: 0x002364EC
		public override void DoTriggerEnter2D(Collider2D other)
		{
			if (this.runFsm.HandleTriggerEnter2D)
			{
				this.runFsm.OnTriggerEnter2D(other);
			}
		}

		// Token: 0x060073CC RID: 29644 RVA: 0x00238307 File Offset: 0x00236507
		public override void DoTriggerStay2D(Collider2D other)
		{
			if (this.runFsm.HandleTriggerStay2D)
			{
				this.runFsm.OnTriggerStay2D(other);
			}
		}

		// Token: 0x060073CD RID: 29645 RVA: 0x00238322 File Offset: 0x00236522
		public override void DoTriggerExit2D(Collider2D other)
		{
			if (this.runFsm.HandleTriggerExit2D)
			{
				this.runFsm.OnTriggerExit2D(other);
			}
		}

		// Token: 0x060073CE RID: 29646 RVA: 0x0023833D File Offset: 0x0023653D
		public override void DoCollisionEnter2D(Collision2D collisionInfo)
		{
			if (this.runFsm.HandleCollisionEnter2D)
			{
				this.runFsm.OnCollisionEnter2D(collisionInfo);
			}
		}

		// Token: 0x060073CF RID: 29647 RVA: 0x00238358 File Offset: 0x00236558
		public override void DoCollisionStay2D(Collision2D collisionInfo)
		{
			if (this.runFsm.HandleCollisionStay2D)
			{
				this.runFsm.OnCollisionStay2D(collisionInfo);
			}
		}

		// Token: 0x060073D0 RID: 29648 RVA: 0x00238373 File Offset: 0x00236573
		public override void DoCollisionExit2D(Collision2D collisionInfo)
		{
			if (this.runFsm.HandleCollisionExit2D)
			{
				this.runFsm.OnCollisionExit2D(collisionInfo);
			}
		}

		// Token: 0x060073D1 RID: 29649 RVA: 0x0023838E File Offset: 0x0023658E
		public override void OnGUI()
		{
			if (this.runFsm != null && this.runFsm.HandleOnGUI)
			{
				this.runFsm.OnGUI();
			}
		}

		// Token: 0x060073D2 RID: 29650 RVA: 0x002383B0 File Offset: 0x002365B0
		public override void OnExit()
		{
			if (this.runFsm != null)
			{
				this.runFsm.Stop();
			}
		}

		// Token: 0x060073D3 RID: 29651 RVA: 0x002383C5 File Offset: 0x002365C5
		protected virtual void CheckIfFinished()
		{
			if (this.runFsm == null || this.runFsm.Finished)
			{
				base.Finish();
			}
		}

		// Token: 0x040073E2 RID: 29666
		protected Fsm runFsm;
	}
}
