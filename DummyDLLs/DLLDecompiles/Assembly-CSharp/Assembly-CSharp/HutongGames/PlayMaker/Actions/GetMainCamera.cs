using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E4C RID: 3660
	[ActionCategory(ActionCategory.Camera)]
	[ActionTarget(typeof(Camera), "storeGameObject", false)]
	[Tooltip("Gets the GameObject tagged MainCamera from the scene")]
	public class GetMainCamera : FsmStateAction
	{
		// Token: 0x060068A8 RID: 26792 RVA: 0x0020DE5D File Offset: 0x0020C05D
		public override void Reset()
		{
			this.storeGameObject = null;
		}

		// Token: 0x060068A9 RID: 26793 RVA: 0x0020DE66 File Offset: 0x0020C066
		public override void OnEnter()
		{
			this.storeGameObject.Value = ((Camera.main != null) ? Camera.main.gameObject : null);
			base.Finish();
		}

		// Token: 0x040067D8 RID: 26584
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Game Object tagged as MainCamera and in a Game Object Variable.")]
		public FsmGameObject storeGameObject;
	}
}
