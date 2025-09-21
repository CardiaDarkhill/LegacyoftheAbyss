using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F19 RID: 3865
	[ActionCategory("iTween")]
	[Tooltip("Rotates a GameObject to look at a supplied Transform or Vector3 over time.")]
	public class iTweenLookUpdate : FsmStateAction
	{
		// Token: 0x06006BF0 RID: 27632 RVA: 0x0021920A File Offset: 0x0021740A
		public override void Reset()
		{
			this.transformTarget = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorTarget = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.axis = iTweenFsmAction.AxisRestriction.none;
		}

		// Token: 0x06006BF1 RID: 27633 RVA: 0x00219248 File Offset: 0x00217448
		public override void OnEnter()
		{
			this.hash = new Hashtable();
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			if (this.transformTarget.IsNone)
			{
				this.hash.Add("looktarget", this.vectorTarget.IsNone ? Vector3.zero : this.vectorTarget.Value);
			}
			else if (this.vectorTarget.IsNone)
			{
				this.hash.Add("looktarget", this.transformTarget.Value.transform);
			}
			else
			{
				this.hash.Add("looktarget", this.transformTarget.Value.transform.position + this.vectorTarget.Value);
			}
			this.hash.Add("time", this.time.IsNone ? 1f : this.time.Value);
			this.hash.Add("axis", (this.axis == iTweenFsmAction.AxisRestriction.none) ? "" : Enum.GetName(typeof(iTweenFsmAction.AxisRestriction), this.axis));
			this.DoiTween();
		}

		// Token: 0x06006BF2 RID: 27634 RVA: 0x002193AB File Offset: 0x002175AB
		public override void OnExit()
		{
		}

		// Token: 0x06006BF3 RID: 27635 RVA: 0x002193B0 File Offset: 0x002175B0
		public override void OnUpdate()
		{
			this.hash.Remove("looktarget");
			if (this.transformTarget.IsNone)
			{
				this.hash.Add("looktarget", this.vectorTarget.IsNone ? Vector3.zero : this.vectorTarget.Value);
			}
			else if (this.vectorTarget.IsNone)
			{
				this.hash.Add("looktarget", this.transformTarget.Value.transform);
			}
			else
			{
				this.hash.Add("looktarget", this.transformTarget.Value.transform.position + this.vectorTarget.Value);
			}
			this.DoiTween();
		}

		// Token: 0x06006BF4 RID: 27636 RVA: 0x0021947F File Offset: 0x0021767F
		private void DoiTween()
		{
			iTween.LookUpdate(this.go, this.hash);
		}

		// Token: 0x04006B5A RID: 27482
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006B5B RID: 27483
		[Tooltip("Look at a transform position.")]
		public FsmGameObject transformTarget;

		// Token: 0x04006B5C RID: 27484
		[Tooltip("A target position the GameObject will look at. If Transform Target is defined this is used as a look offset.")]
		public FsmVector3 vectorTarget;

		// Token: 0x04006B5D RID: 27485
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x04006B5E RID: 27486
		[Tooltip("Restricts rotation to the supplied axis only. Just put there strinc like 'x' or 'xz'")]
		public iTweenFsmAction.AxisRestriction axis;

		// Token: 0x04006B5F RID: 27487
		private Hashtable hash;

		// Token: 0x04006B60 RID: 27488
		private GameObject go;
	}
}
