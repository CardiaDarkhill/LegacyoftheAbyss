using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC7 RID: 3271
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Jitter an object around using its Transform.")]
	public class ObjectJitterLocal : FsmStateAction
	{
		// Token: 0x0600619D RID: 24989 RVA: 0x001EEAC4 File Offset: 0x001ECCC4
		public override void Reset()
		{
			this.gameObject = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.resetPosOnStateExit = false;
			this.limitFps = null;
		}

		// Token: 0x0600619E RID: 24990 RVA: 0x001EEB1C File Offset: 0x001ECD1C
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600619F RID: 24991 RVA: 0x001EEB2C File Offset: 0x001ECD2C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.startX = ownerDefaultTarget.transform.localPosition.x;
			this.startY = ownerDefaultTarget.transform.localPosition.y;
			this.startZ = ownerDefaultTarget.transform.localPosition.z;
		}

		// Token: 0x060061A0 RID: 24992 RVA: 0x001EEB98 File Offset: 0x001ECD98
		public override void OnExit()
		{
			if (this.resetPosOnStateExit)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				ownerDefaultTarget.transform.localPosition = new Vector3(this.startX, this.startY, this.startZ);
			}
		}

		// Token: 0x060061A1 RID: 24993 RVA: 0x001EEBEB File Offset: 0x001ECDEB
		public override void OnFixedUpdate()
		{
			this.DoTranslate();
		}

		// Token: 0x060061A2 RID: 24994 RVA: 0x001EEBF4 File Offset: 0x001ECDF4
		private void DoTranslate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.limitFps.Value > 0f)
			{
				double timeAsDouble = Time.timeAsDouble;
				if (timeAsDouble < this.nextUpdateTime)
				{
					return;
				}
				this.nextUpdateTime = timeAsDouble + (double)(1f / this.limitFps.Value);
			}
			Vector3 localPosition = new Vector3(this.startX + Random.Range(-this.x.Value, this.x.Value), this.startY + Random.Range(-this.y.Value, this.y.Value), this.startZ + Random.Range(-this.z.Value, this.z.Value));
			ownerDefaultTarget.transform.localPosition = localPosition;
		}

		// Token: 0x04005FCA RID: 24522
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005FCB RID: 24523
		[Tooltip("Jitter along x axis.")]
		public FsmFloat x;

		// Token: 0x04005FCC RID: 24524
		[Tooltip("Jitter along y axis.")]
		public FsmFloat y;

		// Token: 0x04005FCD RID: 24525
		[Tooltip("Jitter along z axis.")]
		public FsmFloat z;

		// Token: 0x04005FCE RID: 24526
		public bool resetPosOnStateExit;

		// Token: 0x04005FCF RID: 24527
		public FsmFloat limitFps;

		// Token: 0x04005FD0 RID: 24528
		private float startX;

		// Token: 0x04005FD1 RID: 24529
		private float startY;

		// Token: 0x04005FD2 RID: 24530
		private float startZ;

		// Token: 0x04005FD3 RID: 24531
		private double nextUpdateTime;
	}
}
