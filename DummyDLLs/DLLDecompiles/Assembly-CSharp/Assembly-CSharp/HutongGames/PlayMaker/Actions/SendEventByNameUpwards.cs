using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0F RID: 3343
	public class SendEventByNameUpwards : FsmStateAction
	{
		// Token: 0x060062CD RID: 25293 RVA: 0x001F39D5 File Offset: 0x001F1BD5
		public override void Reset()
		{
			this.Target = null;
			this.EventName = null;
		}

		// Token: 0x060062CE RID: 25294 RVA: 0x001F39E8 File Offset: 0x001F1BE8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe && !string.IsNullOrEmpty(this.EventName.Value))
			{
				this.SendUpRecursive(safe.transform);
			}
			base.Finish();
		}

		// Token: 0x060062CF RID: 25295 RVA: 0x001F3A30 File Offset: 0x001F1C30
		private void SendUpRecursive(Transform transform)
		{
			FSMUtility.SendEventToGameObject(transform.gameObject, this.EventName.Value, false);
			if (transform.parent)
			{
				this.SendUpRecursive(transform.parent);
			}
			Rb2dFollowWithVelocity component = transform.GetComponent<Rb2dFollowWithVelocity>();
			if (component)
			{
				Transform target = component.Target;
				if (target)
				{
					this.SendUpRecursive(target);
				}
			}
		}

		// Token: 0x04006135 RID: 24885
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04006136 RID: 24886
		[RequiredField]
		public FsmString EventName;
	}
}
