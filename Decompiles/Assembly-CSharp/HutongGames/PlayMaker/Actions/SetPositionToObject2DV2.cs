using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D49 RID: 3401
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Position of a Game Object to another Game Object's position")]
	public class SetPositionToObject2DV2 : FsmStateAction
	{
		// Token: 0x060063BF RID: 25535 RVA: 0x001F7272 File Offset: 0x001F5472
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.xOffset = null;
			this.yOffset = null;
			this.active = true;
		}

		// Token: 0x060063C0 RID: 25536 RVA: 0x001F729C File Offset: 0x001F549C
		public override void OnEnter()
		{
			this.DoSetPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060063C1 RID: 25537 RVA: 0x001F72B2 File Offset: 0x001F54B2
		public override void OnUpdate()
		{
			if (this.everyFrame)
			{
				this.DoSetPosition();
			}
		}

		// Token: 0x060063C2 RID: 25538 RVA: 0x001F72C4 File Offset: 0x001F54C4
		private void DoSetPosition()
		{
			if (this.active.Value)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null || this.targetObject.IsNone || this.targetObject.Value == null)
				{
					return;
				}
				Vector3 position = this.targetObject.Value.transform.position;
				if (!this.xOffset.IsNone)
				{
					position = new Vector3(position.x + this.xOffset.Value, position.y, position.z);
				}
				if (!this.yOffset.IsNone)
				{
					position = new Vector3(position.x, position.y + this.yOffset.Value, position.z);
				}
				position = new Vector3(position.x, position.y, ownerDefaultTarget.transform.position.z);
				ownerDefaultTarget.transform.position = position;
			}
		}

		// Token: 0x04006211 RID: 25105
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006212 RID: 25106
		public FsmGameObject targetObject;

		// Token: 0x04006213 RID: 25107
		public FsmFloat xOffset;

		// Token: 0x04006214 RID: 25108
		public FsmFloat yOffset;

		// Token: 0x04006215 RID: 25109
		public FsmBool active;

		// Token: 0x04006216 RID: 25110
		public bool everyFrame;
	}
}
