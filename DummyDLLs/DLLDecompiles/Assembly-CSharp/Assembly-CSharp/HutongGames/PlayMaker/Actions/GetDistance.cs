using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EAE RID: 3758
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable.")]
	public class GetDistance : FsmStateAction
	{
		// Token: 0x06006A68 RID: 27240 RVA: 0x00213DE6 File Offset: 0x00211FE6
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x06006A69 RID: 27241 RVA: 0x00213E04 File Offset: 0x00212004
		public override void OnEnter()
		{
			this.DoGetDistance();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A6A RID: 27242 RVA: 0x00213E1A File Offset: 0x0021201A
		public override void OnUpdate()
		{
			this.DoGetDistance();
		}

		// Token: 0x06006A6B RID: 27243 RVA: 0x00213E24 File Offset: 0x00212024
		private void DoGetDistance()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null || this.target.Value == null || this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = Vector3.Distance(ownerDefaultTarget.transform.position, this.target.Value.transform.position);
		}

		// Token: 0x040069BA RID: 27066
		[RequiredField]
		[Tooltip("Measure distance from this GameObject.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069BB RID: 27067
		[RequiredField]
		[Tooltip("Target GameObject.")]
		public FsmGameObject target;

		// Token: 0x040069BC RID: 27068
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance in a float variable.")]
		public FsmFloat storeResult;

		// Token: 0x040069BD RID: 27069
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
