using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D68 RID: 3432
	public class SineMovement : FsmStateAction
	{
		// Token: 0x0600644E RID: 25678 RVA: 0x001F98B3 File Offset: 0x001F7AB3
		public override void Reset()
		{
			this.Target = null;
			this.Offset = null;
			this.Space = Space.Self;
			this.Speed = null;
		}

		// Token: 0x0600644F RID: 25679 RVA: 0x001F98D4 File Offset: 0x001F7AD4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.transform = safe.transform;
				if (this.Space == Space.Self)
				{
					this.initialPosition = this.transform.localPosition;
				}
				else
				{
					this.initialPosition = this.transform.position;
				}
			}
			if (!this.transform)
			{
				base.Finish();
			}
		}

		// Token: 0x06006450 RID: 25680 RVA: 0x001F9944 File Offset: 0x001F7B44
		public override void OnUpdate()
		{
			Vector3 b = this.Offset.Value * Mathf.Sin(base.State.StateTime * this.Speed.Value);
			if (this.Space == Space.Self)
			{
				this.transform.localPosition = this.initialPosition + b;
				return;
			}
			this.transform.position = this.initialPosition + b;
		}

		// Token: 0x040062C9 RID: 25289
		public FsmOwnerDefault Target;

		// Token: 0x040062CA RID: 25290
		public FsmVector3 Offset;

		// Token: 0x040062CB RID: 25291
		public Space Space;

		// Token: 0x040062CC RID: 25292
		public FsmFloat Speed;

		// Token: 0x040062CD RID: 25293
		private Transform transform;

		// Token: 0x040062CE RID: 25294
		private Vector3 initialPosition;
	}
}
