using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FEC RID: 4076
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Rigid bodies 2D start sleeping when they come to rest. This action wakes up all rigid bodies 2D in the scene. E.g., if you Set Gravity 2D and want objects at rest to respond.")]
	public class WakeAllRigidBodies2d : FsmStateAction
	{
		// Token: 0x0600705F RID: 28767 RVA: 0x0022B8EE File Offset: 0x00229AEE
		public override void Reset()
		{
			this.everyFrame = false;
		}

		// Token: 0x06007060 RID: 28768 RVA: 0x0022B8F7 File Offset: 0x00229AF7
		public override void OnEnter()
		{
			this.DoWakeAll();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007061 RID: 28769 RVA: 0x0022B90D File Offset: 0x00229B0D
		public override void OnUpdate()
		{
			this.DoWakeAll();
		}

		// Token: 0x06007062 RID: 28770 RVA: 0x0022B918 File Offset: 0x00229B18
		private void DoWakeAll()
		{
			Rigidbody2D[] array = Object.FindObjectsOfType(typeof(Rigidbody2D)) as Rigidbody2D[];
			if (array != null)
			{
				Rigidbody2D[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].WakeUp();
				}
			}
		}

		// Token: 0x04007042 RID: 28738
		[Tooltip("Repeat every frame. Note: This would be very expensive!")]
		public bool everyFrame;
	}
}
