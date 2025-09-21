using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D48 RID: 3400
	public class SetPositionToObject2DLateUpdate : FsmStateAction
	{
		// Token: 0x060063B9 RID: 25529 RVA: 0x001F713A File Offset: 0x001F533A
		public override void Reset()
		{
			this.GameObject = null;
			this.TargetObject = null;
			this.XOffset = null;
			this.YOffset = null;
		}

		// Token: 0x060063BA RID: 25530 RVA: 0x001F7158 File Offset: 0x001F5358
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		// Token: 0x060063BB RID: 25531 RVA: 0x001F7166 File Offset: 0x001F5366
		public override void OnEnter()
		{
			this.DoSetPosition();
		}

		// Token: 0x060063BC RID: 25532 RVA: 0x001F716E File Offset: 0x001F536E
		public override void OnLateUpdate()
		{
			this.DoSetPosition();
		}

		// Token: 0x060063BD RID: 25533 RVA: 0x001F7178 File Offset: 0x001F5378
		private void DoSetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.GameObject);
			if (ownerDefaultTarget == null || this.TargetObject.IsNone || this.TargetObject.Value == null)
			{
				return;
			}
			Vector3 position = this.TargetObject.Value.transform.position;
			if (!this.XOffset.IsNone)
			{
				position = new Vector3(position.x + this.XOffset.Value, position.y, position.z);
			}
			if (!this.YOffset.IsNone)
			{
				position = new Vector3(position.x, position.y + this.YOffset.Value, position.z);
			}
			position = new Vector3(position.x, position.y, ownerDefaultTarget.transform.position.z);
			ownerDefaultTarget.transform.position = position;
		}

		// Token: 0x0400620D RID: 25101
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault GameObject;

		// Token: 0x0400620E RID: 25102
		public FsmGameObject TargetObject;

		// Token: 0x0400620F RID: 25103
		public FsmFloat XOffset;

		// Token: 0x04006210 RID: 25104
		public FsmFloat YOffset;
	}
}
