using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010E5 RID: 4325
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets Random Rotation for a Game Object. Uncheck an axis to keep the current rotation around that axis.")]
	public class SetRandomRotation : FsmStateAction
	{
		// Token: 0x06007509 RID: 29961 RVA: 0x0023C979 File Offset: 0x0023AB79
		public override void Reset()
		{
			this.gameObject = null;
			this.x = true;
			this.y = true;
			this.z = true;
		}

		// Token: 0x0600750A RID: 29962 RVA: 0x0023C9A6 File Offset: 0x0023ABA6
		public override void OnEnter()
		{
			this.DoRandomRotation();
			base.Finish();
		}

		// Token: 0x0600750B RID: 29963 RVA: 0x0023C9B4 File Offset: 0x0023ABB4
		private void DoRandomRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 localEulerAngles = ownerDefaultTarget.transform.localEulerAngles;
			float num = localEulerAngles.x;
			float num2 = localEulerAngles.y;
			float num3 = localEulerAngles.z;
			if (this.x.Value)
			{
				num = (float)Random.Range(0, 360);
			}
			if (this.y.Value)
			{
				num2 = (float)Random.Range(0, 360);
			}
			if (this.z.Value)
			{
				num3 = (float)Random.Range(0, 360);
			}
			ownerDefaultTarget.transform.localEulerAngles = new Vector3(num, num2, num3);
		}

		// Token: 0x04007561 RID: 30049
		[RequiredField]
		[Tooltip("The Game Object to randomly rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007562 RID: 30050
		[Tooltip("Use X axis.")]
		public FsmBool x;

		// Token: 0x04007563 RID: 30051
		[Tooltip("Use Y axis.")]
		public FsmBool y;

		// Token: 0x04007564 RID: 30052
		[Tooltip("Use Z axis.")]
		public FsmBool z;
	}
}
