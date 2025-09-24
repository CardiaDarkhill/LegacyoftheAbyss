using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FBC RID: 4028
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Adds a 2d torque (rotational force) to a Game Object.")]
	public class AddTorque2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F24 RID: 28452 RVA: 0x00225AA6 File Offset: 0x00223CA6
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006F25 RID: 28453 RVA: 0x00225AB4 File Offset: 0x00223CB4
		public override void Reset()
		{
			this.gameObject = null;
			this.torque = null;
			this.everyFrame = false;
		}

		// Token: 0x06006F26 RID: 28454 RVA: 0x00225ACB File Offset: 0x00223CCB
		public override void OnEnter()
		{
			this.DoAddTorque();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F27 RID: 28455 RVA: 0x00225AE1 File Offset: 0x00223CE1
		public override void OnFixedUpdate()
		{
			this.DoAddTorque();
		}

		// Token: 0x06006F28 RID: 28456 RVA: 0x00225AEC File Offset: 0x00223CEC
		private void DoAddTorque()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.AddTorque(this.torque.Value, this.forceMode);
		}

		// Token: 0x04006EC7 RID: 28359
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to add torque to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006EC8 RID: 28360
		[Tooltip("Option for applying the force")]
		public ForceMode2D forceMode;

		// Token: 0x04006EC9 RID: 28361
		[Tooltip("Torque")]
		public FsmFloat torque;

		// Token: 0x04006ECA RID: 28362
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
