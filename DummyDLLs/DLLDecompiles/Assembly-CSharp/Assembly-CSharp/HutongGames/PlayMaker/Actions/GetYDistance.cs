using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EC5 RID: 3781
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable. Y axis only.")]
	public class GetYDistance : FsmStateAction
	{
		// Token: 0x06006ACA RID: 27338 RVA: 0x00214E00 File Offset: 0x00213000
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.storeResult = null;
			this.everyFrame = true;
			this.allowNegatives = false;
		}

		// Token: 0x06006ACB RID: 27339 RVA: 0x00214E25 File Offset: 0x00213025
		public override void OnEnter()
		{
			this.DoGetDistance();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006ACC RID: 27340 RVA: 0x00214E3B File Offset: 0x0021303B
		public override void OnUpdate()
		{
			this.DoGetDistance();
		}

		// Token: 0x06006ACD RID: 27341 RVA: 0x00214E44 File Offset: 0x00213044
		private void DoGetDistance()
		{
			GameObject gameObject = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value;
			if (gameObject == null || this.target.Value == null || this.storeResult == null)
			{
				return;
			}
			float num = gameObject.transform.position.y - this.target.Value.transform.position.y;
			if (!this.allowNegatives && num < 0f)
			{
				num *= -1f;
			}
			this.storeResult.Value = num;
		}

		// Token: 0x04006A0A RID: 27146
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006A0B RID: 27147
		[RequiredField]
		public FsmGameObject target;

		// Token: 0x04006A0C RID: 27148
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		// Token: 0x04006A0D RID: 27149
		public bool everyFrame;

		// Token: 0x04006A0E RID: 27150
		public bool allowNegatives;
	}
}
