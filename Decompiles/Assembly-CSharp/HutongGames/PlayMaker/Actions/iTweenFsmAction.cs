using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F16 RID: 3862
	[Tooltip("iTween base action - don't use!")]
	public abstract class iTweenFsmAction : FsmStateAction
	{
		// Token: 0x06006BE0 RID: 27616 RVA: 0x002189C8 File Offset: 0x00216BC8
		public override void Reset()
		{
			this.startEvent = null;
			this.finishEvent = null;
			this.realTime = new FsmBool
			{
				Value = false
			};
			this.stopOnExit = new FsmBool
			{
				Value = true
			};
			this.loopDontFinish = new FsmBool
			{
				Value = true
			};
			this.itweenType = "";
		}

		// Token: 0x06006BE1 RID: 27617 RVA: 0x00218A24 File Offset: 0x00216C24
		protected void OnEnteriTween(FsmOwnerDefault anOwner)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(anOwner);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.itweenEvents = ownerDefaultTarget.AddComponent<iTweenFSMEvents>();
			this.itweenEvents.itweenFSMAction = this;
			iTweenFSMEvents.itweenIDCount++;
			this.itweenID = iTweenFSMEvents.itweenIDCount;
			this.itweenEvents.itweenID = iTweenFSMEvents.itweenIDCount;
			this.itweenEvents.donotfinish = (!this.loopDontFinish.IsNone && this.loopDontFinish.Value);
			iTweenFSMEvents iTweenFSMEvents = this.itweenEvents;
			iTweenFSMEvents.onDestroy = (Action<iTweenFSMEvents>)Delegate.Combine(iTweenFSMEvents.onDestroy, new Action<iTweenFSMEvents>(this.<OnEnteriTween>g__OnDestroyEvents|10_0));
		}

		// Token: 0x06006BE2 RID: 27618 RVA: 0x00218AD4 File Offset: 0x00216CD4
		protected void IsLoop(bool aValue)
		{
			if (this.itweenEvents != null)
			{
				this.itweenEvents.islooping = aValue;
			}
		}

		// Token: 0x06006BE3 RID: 27619 RVA: 0x00218AF0 File Offset: 0x00216CF0
		protected void OnExitiTween(FsmOwnerDefault anOwner)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(anOwner);
			if (this.itweenEvents == null && this.itweenEvents != null)
			{
				this.itweenEvents = null;
			}
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.itweenEvents)
			{
				Object.Destroy(this.itweenEvents);
				this.itweenEvents = null;
			}
			if (this.stopOnExit.IsNone)
			{
				iTween.Stop(ownerDefaultTarget, this.itweenType);
				return;
			}
			if (this.stopOnExit.Value)
			{
				iTween.Stop(ownerDefaultTarget, this.itweenType);
			}
		}

		// Token: 0x06006BE5 RID: 27621 RVA: 0x00218B9D File Offset: 0x00216D9D
		[CompilerGenerated]
		private void <OnEnteriTween>g__OnDestroyEvents|10_0(iTweenFSMEvents events)
		{
			events.onDestroy = (Action<iTweenFSMEvents>)Delegate.Remove(events.onDestroy, new Action<iTweenFSMEvents>(this.<OnEnteriTween>g__OnDestroyEvents|10_0));
			if (this.itweenEvents == events)
			{
				this.itweenEvents = null;
			}
		}

		// Token: 0x04006B3E RID: 27454
		[ActionSection("Events")]
		public FsmEvent startEvent;

		// Token: 0x04006B3F RID: 27455
		public FsmEvent finishEvent;

		// Token: 0x04006B40 RID: 27456
		[Tooltip("Setting this to true will allow the animation to continue independent of the current time which is helpful for animating menus after a game has been paused by setting Time.timeScale=0.")]
		public FsmBool realTime;

		// Token: 0x04006B41 RID: 27457
		public FsmBool stopOnExit;

		// Token: 0x04006B42 RID: 27458
		public FsmBool loopDontFinish;

		// Token: 0x04006B43 RID: 27459
		internal iTweenFSMEvents itweenEvents;

		// Token: 0x04006B44 RID: 27460
		protected string itweenType = "";

		// Token: 0x04006B45 RID: 27461
		protected int itweenID = -1;

		// Token: 0x02001BAD RID: 7085
		public enum AxisRestriction
		{
			// Token: 0x04009E29 RID: 40489
			none,
			// Token: 0x04009E2A RID: 40490
			x,
			// Token: 0x04009E2B RID: 40491
			y,
			// Token: 0x04009E2C RID: 40492
			z,
			// Token: 0x04009E2D RID: 40493
			xy,
			// Token: 0x04009E2E RID: 40494
			xz,
			// Token: 0x04009E2F RID: 40495
			yz
		}
	}
}
