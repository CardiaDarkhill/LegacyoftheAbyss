using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE8 RID: 3048
	[ActionCategory(ActionCategory.StateMachine)]
	public class CheckFacingTarget : FsmStateAction
	{
		// Token: 0x06005D67 RID: 23911 RVA: 0x001D70E9 File Offset: 0x001D52E9
		public override void Reset()
		{
			this.facingObject = null;
			this.target = null;
			this.facingEvent = null;
			this.notFacingEvent = null;
			this.facingBool = null;
			this.notFacingBool = null;
			this.everyFrame = false;
		}

		// Token: 0x06005D68 RID: 23912 RVA: 0x001D711C File Offset: 0x001D531C
		public override void OnEnter()
		{
			this.self = base.Fsm.GetOwnerDefaultTarget(this.facingObject);
			this.targetTransform = this.target.Value.transform;
			this.CheckFacing();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005D69 RID: 23913 RVA: 0x001D716A File Offset: 0x001D536A
		public override void OnUpdate()
		{
			this.CheckFacing();
		}

		// Token: 0x06005D6A RID: 23914 RVA: 0x001D7174 File Offset: 0x001D5374
		private void CheckFacing()
		{
			if (this.targetTransform == null)
			{
				base.Finish();
				return;
			}
			if (this.self == null)
			{
				base.Finish();
				return;
			}
			bool flag = this.self.transform.position.x < this.targetTransform.position.x;
			bool flag2;
			if (this.spriteFacesRight)
			{
				flag2 = (this.self.transform.lossyScale.x >= 0f);
			}
			else
			{
				flag2 = (this.self.transform.lossyScale.x < 0f);
			}
			if ((flag2 && flag) || (!flag2 && !flag))
			{
				if (this.facingEvent != null)
				{
					FSMUtility.SendEventToGameObject(this.self, this.facingEvent, false);
				}
				this.facingBool.Value = true;
				return;
			}
			if (this.notFacingEvent != null)
			{
				FSMUtility.SendEventToGameObject(this.self, this.notFacingEvent, false);
			}
			this.facingBool.Value = false;
		}

		// Token: 0x04005984 RID: 22916
		[RequiredField]
		public FsmOwnerDefault facingObject;

		// Token: 0x04005985 RID: 22917
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005986 RID: 22918
		public bool spriteFacesRight;

		// Token: 0x04005987 RID: 22919
		public bool everyFrame;

		// Token: 0x04005988 RID: 22920
		public FsmEvent facingEvent;

		// Token: 0x04005989 RID: 22921
		public FsmEvent notFacingEvent;

		// Token: 0x0400598A RID: 22922
		[UIHint(UIHint.Variable)]
		public FsmBool facingBool;

		// Token: 0x0400598B RID: 22923
		[UIHint(UIHint.Variable)]
		public FsmBool notFacingBool;

		// Token: 0x0400598C RID: 22924
		private GameObject self;

		// Token: 0x0400598D RID: 22925
		private Transform targetTransform;
	}
}
