using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C61 RID: 3169
	[Tooltip("Stores the result as + or - a target distance depending on if on the right or left.")]
	public class GetClosestTargetDistanceX : FsmStateAction
	{
		// Token: 0x06005FD6 RID: 24534 RVA: 0x001E5F9B File Offset: 0x001E419B
		public override void Reset()
		{
			this.Target = null;
			this.FromGameObject = null;
			this.TargetDistanceX = null;
			this.StoreDistanceX = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005FD7 RID: 24535 RVA: 0x001E5FC0 File Offset: 0x001E41C0
		public override void OnEnter()
		{
			this.target = this.Target.GetSafe(this);
			this.from = this.FromGameObject.Value;
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FD8 RID: 24536 RVA: 0x001E5FF9 File Offset: 0x001E41F9
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06005FD9 RID: 24537 RVA: 0x001E6004 File Offset: 0x001E4204
		private void DoAction()
		{
			float num = Mathf.Abs(this.TargetDistanceX.Value);
			this.StoreDistanceX.Value = ((this.from.transform.position.x < this.target.transform.position.x) ? num : (-num));
			this.StorePositionX.Value = this.from.transform.position.x + this.StoreDistanceX.Value;
			if (this.StoreDistanceX.IsNone && this.StorePositionX.IsNone)
			{
				Debug.LogWarning("No values are being stored!", base.Owner);
			}
		}

		// Token: 0x04005D2A RID: 23850
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04005D2B RID: 23851
		private GameObject target;

		// Token: 0x04005D2C RID: 23852
		[RequiredField]
		public FsmGameObject FromGameObject;

		// Token: 0x04005D2D RID: 23853
		private GameObject from;

		// Token: 0x04005D2E RID: 23854
		[RequiredField]
		public FsmFloat TargetDistanceX;

		// Token: 0x04005D2F RID: 23855
		[Space]
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDistanceX;

		// Token: 0x04005D30 RID: 23856
		[UIHint(UIHint.Variable)]
		public FsmFloat StorePositionX;

		// Token: 0x04005D31 RID: 23857
		public bool EveryFrame;
	}
}
