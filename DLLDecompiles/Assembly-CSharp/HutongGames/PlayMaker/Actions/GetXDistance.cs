using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EC4 RID: 3780
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable. X axis only.")]
	public class GetXDistance : FsmStateAction
	{
		// Token: 0x06006AC5 RID: 27333 RVA: 0x00214D1C File Offset: 0x00212F1C
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x06006AC6 RID: 27334 RVA: 0x00214D3A File Offset: 0x00212F3A
		public override void OnEnter()
		{
			this.DoGetDistance();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006AC7 RID: 27335 RVA: 0x00214D50 File Offset: 0x00212F50
		public override void OnUpdate()
		{
			this.DoGetDistance();
		}

		// Token: 0x06006AC8 RID: 27336 RVA: 0x00214D58 File Offset: 0x00212F58
		private void DoGetDistance()
		{
			GameObject gameObject = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value;
			if (gameObject == null || this.target.Value == null || this.storeResult == null)
			{
				return;
			}
			float num = gameObject.transform.position.x - this.target.Value.transform.position.x;
			if (num < 0f)
			{
				num *= -1f;
			}
			this.storeResult.Value = num;
		}

		// Token: 0x04006A06 RID: 27142
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006A07 RID: 27143
		[RequiredField]
		public FsmGameObject target;

		// Token: 0x04006A08 RID: 27144
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		// Token: 0x04006A09 RID: 27145
		public bool everyFrame;
	}
}
