using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9B RID: 2971
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activate or deactivate all children on a GameObject.")]
	public class ActivateAllChildren : FsmStateAction
	{
		// Token: 0x06005BE2 RID: 23522 RVA: 0x001CE7A0 File Offset: 0x001CC9A0
		public override void Reset()
		{
			this.gameObject = null;
			this.activate = true;
		}

		// Token: 0x06005BE3 RID: 23523 RVA: 0x001CE7B0 File Offset: 0x001CC9B0
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				foreach (object obj in value.transform)
				{
					((Transform)obj).gameObject.SetActive(this.activate);
				}
			}
			base.Finish();
		}

		// Token: 0x04005746 RID: 22342
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject gameObject;

		// Token: 0x04005747 RID: 22343
		public bool activate;
	}
}
