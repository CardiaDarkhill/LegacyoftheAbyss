using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4E RID: 3406
	public class SetRotation2DLerp : FsmStateAction
	{
		// Token: 0x060063D4 RID: 25556 RVA: 0x001F77EF File Offset: 0x001F59EF
		public override void Reset()
		{
			this.Target = null;
			this.TargetRotationZ = null;
			this.LerpSpeed = null;
			this.Space = Space.Self;
		}

		// Token: 0x060063D5 RID: 25557 RVA: 0x001F7810 File Offset: 0x001F5A10
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			this.transform = safe.transform;
		}

		// Token: 0x060063D6 RID: 25558 RVA: 0x001F7848 File Offset: 0x001F5A48
		public override void OnUpdate()
		{
			Space space = this.Space;
			Quaternion quaternion;
			if (space != Space.World)
			{
				if (space != Space.Self)
				{
					throw new ArgumentOutOfRangeException();
				}
				quaternion = this.transform.localRotation;
			}
			else
			{
				quaternion = this.transform.rotation;
			}
			Quaternion quaternion2 = quaternion;
			Quaternion b = Quaternion.Euler(0f, 0f, this.TargetRotationZ.Value);
			quaternion2 = Quaternion.Lerp(quaternion2, b, this.LerpSpeed.Value * Time.deltaTime);
			space = this.Space;
			if (space == Space.World)
			{
				this.transform.rotation = quaternion2;
				return;
			}
			if (space != Space.Self)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.transform.localRotation = quaternion2;
		}

		// Token: 0x0400622C RID: 25132
		public FsmOwnerDefault Target;

		// Token: 0x0400622D RID: 25133
		public FsmFloat TargetRotationZ;

		// Token: 0x0400622E RID: 25134
		public FsmFloat LerpSpeed;

		// Token: 0x0400622F RID: 25135
		public Space Space;

		// Token: 0x04006230 RID: 25136
		private Transform transform;
	}
}
