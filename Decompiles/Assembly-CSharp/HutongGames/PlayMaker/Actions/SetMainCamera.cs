using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E51 RID: 3665
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Sets the Main Camera.")]
	public class SetMainCamera : FsmStateAction
	{
		// Token: 0x060068BF RID: 26815 RVA: 0x0020E1FE File Offset: 0x0020C3FE
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060068C0 RID: 26816 RVA: 0x0020E208 File Offset: 0x0020C408
		public override void OnEnter()
		{
			if (this.gameObject.Value != null)
			{
				if (Camera.main != null)
				{
					Camera.main.gameObject.tag = "Untagged";
				}
				this.gameObject.Value.tag = "MainCamera";
			}
			base.Finish();
		}

		// Token: 0x040067ED RID: 26605
		[RequiredField]
		[CheckForComponent(typeof(Camera))]
		[Tooltip("The GameObject to set as the main camera (should have a Camera component).")]
		public FsmGameObject gameObject;
	}
}
