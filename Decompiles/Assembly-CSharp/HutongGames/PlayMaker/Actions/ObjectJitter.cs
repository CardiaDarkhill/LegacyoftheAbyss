using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC6 RID: 3270
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Jitter an object around using its Transform.")]
	public class ObjectJitter : FsmStateAction
	{
		// Token: 0x06006196 RID: 24982 RVA: 0x001EE80C File Offset: 0x001ECA0C
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
			this.allowMovement = null;
			this.limitFps = null;
		}

		// Token: 0x06006197 RID: 24983 RVA: 0x001EE864 File Offset: 0x001ECA64
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006198 RID: 24984 RVA: 0x001EE874 File Offset: 0x001ECA74
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.startX = ownerDefaultTarget.transform.position.x;
			this.startY = ownerDefaultTarget.transform.position.y;
			this.startZ = ownerDefaultTarget.transform.position.z;
			if (this.x.Value == 0f && this.y.Value == 0f)
			{
				base.Finish();
			}
		}

		// Token: 0x06006199 RID: 24985 RVA: 0x001EE909 File Offset: 0x001ECB09
		public override void OnFixedUpdate()
		{
			this.DoTranslate();
		}

		// Token: 0x0600619A RID: 24986 RVA: 0x001EE914 File Offset: 0x001ECB14
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
			if (this.allowMovement.Value)
			{
				ownerDefaultTarget.transform.Translate(Random.Range(-this.x.Value, this.x.Value), Random.Range(-this.y.Value, this.y.Value), Random.Range(-this.z.Value, this.z.Value));
				return;
			}
			Vector3 position = new Vector3(this.startX + Random.Range(-this.x.Value, this.x.Value), this.startY + Random.Range(-this.y.Value, this.y.Value), this.startZ + Random.Range(-this.z.Value, this.z.Value));
			ownerDefaultTarget.transform.position = position;
		}

		// Token: 0x0600619B RID: 24987 RVA: 0x001EEA64 File Offset: 0x001ECC64
		public override void OnExit()
		{
			if (!this.allowMovement.Value)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				ownerDefaultTarget.transform.position = new Vector3(this.startX, this.startY, this.startZ);
			}
		}

		// Token: 0x04005FC0 RID: 24512
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005FC1 RID: 24513
		[Tooltip("Jitter along x axis.")]
		public FsmFloat x;

		// Token: 0x04005FC2 RID: 24514
		[Tooltip("Jitter along y axis.")]
		public FsmFloat y;

		// Token: 0x04005FC3 RID: 24515
		[Tooltip("Jitter along z axis.")]
		public FsmFloat z;

		// Token: 0x04005FC4 RID: 24516
		[Tooltip("If true, don't jitter around start pos")]
		public FsmBool allowMovement;

		// Token: 0x04005FC5 RID: 24517
		public FsmFloat limitFps;

		// Token: 0x04005FC6 RID: 24518
		private float startX;

		// Token: 0x04005FC7 RID: 24519
		private float startY;

		// Token: 0x04005FC8 RID: 24520
		private float startZ;

		// Token: 0x04005FC9 RID: 24521
		private double nextUpdateTime;
	}
}
