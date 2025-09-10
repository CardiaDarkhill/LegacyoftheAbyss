using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001275 RID: 4725
	[ActionCategory("Particle System")]
	[Tooltip("Waits for particle system to stop playing")]
	public sealed class LerpObjectPosition : FsmStateAction
	{
		// Token: 0x06007C7D RID: 31869 RVA: 0x002535EC File Offset: 0x002517EC
		public override void Reset()
		{
			this.fromPosition = null;
			this.toPosition = null;
			this.target = null;
			this.duration = null;
			this.resetOnExit = null;
			this.setFinalPositionOnExit = null;
		}

		// Token: 0x06007C7E RID: 31870 RVA: 0x00253618 File Offset: 0x00251818
		public override void OnEnter()
		{
			this.fromGO = this.fromPosition.GetSafe(this);
			this.toGO = this.toPosition.GetSafe(this);
			this.targetGO = this.target.GetSafe(this);
			bool flag = this.fromGO != null;
			bool flag2 = this.toGO != null;
			bool flag3 = this.targetGO != null;
			this.useLocal = this.useLocalPosition.Value;
			if (this.duration.Value <= 0f && (flag2 && flag3))
			{
				if (this.useLocal)
				{
					this.targetGO.transform.localPosition = this.toGO.transform.localPosition;
					return;
				}
				this.targetGO.transform.position = this.toGO.transform.position;
				return;
			}
			else
			{
				if (flag3)
				{
					if (this.useLocal)
					{
						this.fromPositionValue = this.targetGO.transform.localPosition;
					}
					else
					{
						this.fromPositionValue = this.targetGO.transform.position;
					}
				}
				if (flag)
				{
					if (this.useLocal)
					{
						this.fromPositionValue = this.fromGO.transform.localPosition;
					}
					else
					{
						this.fromPositionValue = this.fromGO.transform.position;
					}
				}
				if (!flag2 || !flag3)
				{
					base.Finish();
					return;
				}
				this.t = 0f;
				this.inverDuration = 1f / this.duration.Value;
				return;
			}
		}

		// Token: 0x06007C7F RID: 31871 RVA: 0x00253798 File Offset: 0x00251998
		public override void OnExit()
		{
			if (this.setFinalPositionOnExit.Value && this.toGO != null)
			{
				if (this.targetGO != null)
				{
					if (this.useLocal)
					{
						this.targetGO.transform.localPosition = this.toGO.transform.localPosition;
						return;
					}
					this.targetGO.transform.position = this.toGO.transform.position;
					return;
				}
			}
			else if (this.resetOnExit.Value && this.targetGO != null)
			{
				if (this.useLocal)
				{
					this.targetGO.transform.localPosition = this.fromPositionValue;
					return;
				}
				this.targetGO.transform.position = this.fromPositionValue;
			}
		}

		// Token: 0x06007C80 RID: 31872 RVA: 0x0025386C File Offset: 0x00251A6C
		public override void OnUpdate()
		{
			while (this.t < 1f)
			{
				this.t += this.inverDuration * Time.deltaTime;
				if (this.useLocal)
				{
					this.targetGO.transform.localPosition = Vector3.Lerp(this.fromPositionValue, this.toGO.transform.localPosition, Mathf.Clamp01(this.t));
				}
				else
				{
					this.targetGO.transform.position = Vector3.Lerp(this.fromPositionValue, this.toGO.transform.position, Mathf.Clamp01(this.t));
				}
				if (this.t >= 1f)
				{
					base.Finish();
				}
			}
		}

		// Token: 0x04007C86 RID: 31878
		[Tooltip("Will use target's current position if not set.")]
		public FsmOwnerDefault fromPosition;

		// Token: 0x04007C87 RID: 31879
		[RequiredField]
		public FsmOwnerDefault toPosition;

		// Token: 0x04007C88 RID: 31880
		[RequiredField]
		public FsmOwnerDefault target;

		// Token: 0x04007C89 RID: 31881
		public FsmFloat duration;

		// Token: 0x04007C8A RID: 31882
		public FsmBool resetOnExit;

		// Token: 0x04007C8B RID: 31883
		public FsmBool setFinalPositionOnExit;

		// Token: 0x04007C8C RID: 31884
		public FsmBool useLocalPosition;

		// Token: 0x04007C8D RID: 31885
		private GameObject fromGO;

		// Token: 0x04007C8E RID: 31886
		private GameObject toGO;

		// Token: 0x04007C8F RID: 31887
		private GameObject targetGO;

		// Token: 0x04007C90 RID: 31888
		private Vector3 fromPositionValue;

		// Token: 0x04007C91 RID: 31889
		private float t;

		// Token: 0x04007C92 RID: 31890
		private float inverDuration;

		// Token: 0x04007C93 RID: 31891
		private bool useLocal;
	}
}
