using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5B RID: 3419
	[ActionCategory("Trail Renderer")]
	[Tooltip("Set trail renderer parameters")]
	public class SetTrailRenderer : FsmStateAction
	{
		// Token: 0x06006408 RID: 25608 RVA: 0x001F8531 File Offset: 0x001F6731
		public override void Reset()
		{
			this.gameObject = null;
			this.startWidth = new FsmFloat
			{
				UseVariable = true
			};
			this.endWidth = new FsmFloat
			{
				UseVariable = true
			};
			this.time = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06006409 RID: 25609 RVA: 0x001F8570 File Offset: 0x001F6770
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				this.trail = ownerDefaultTarget.GetComponent<TrailRenderer>();
				if (this.trail == null)
				{
					base.Finish();
				}
				this.DoSetTrail();
				if (!this.everyFrame)
				{
					base.Finish();
					return;
				}
			}
			else
			{
				base.Finish();
			}
		}

		// Token: 0x0600640A RID: 25610 RVA: 0x001F85D2 File Offset: 0x001F67D2
		public override void OnUpdate()
		{
			this.DoSetTrail();
		}

		// Token: 0x0600640B RID: 25611 RVA: 0x001F85DC File Offset: 0x001F67DC
		private void DoSetTrail()
		{
			if (!this.startWidth.IsNone)
			{
				this.trail.startWidth = this.startWidth.Value;
			}
			if (!this.endWidth.IsNone)
			{
				this.trail.endWidth = this.endWidth.Value;
			}
			if (!this.time.IsNone)
			{
				this.trail.time = this.time.Value;
			}
		}

		// Token: 0x04006275 RID: 25205
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006276 RID: 25206
		public FsmFloat startWidth;

		// Token: 0x04006277 RID: 25207
		public FsmFloat endWidth;

		// Token: 0x04006278 RID: 25208
		public FsmFloat time;

		// Token: 0x04006279 RID: 25209
		public bool everyFrame;

		// Token: 0x0400627A RID: 25210
		private TrailRenderer trail;
	}
}
