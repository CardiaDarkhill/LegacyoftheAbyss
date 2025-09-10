using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9C RID: 2972
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activate or deactivate all children on a GameObject.")]
	public class ActivateAllChildrenV2 : FsmStateAction
	{
		// Token: 0x06005BE5 RID: 23525 RVA: 0x001CE834 File Offset: 0x001CCA34
		public override void Reset()
		{
			this.gameObject = null;
			this.activate = true;
		}

		// Token: 0x06005BE6 RID: 23526 RVA: 0x001CE844 File Offset: 0x001CCA44
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

		// Token: 0x06005BE7 RID: 23527 RVA: 0x001CE8C0 File Offset: 0x001CCAC0
		public override void OnExit()
		{
			GameObject value = this.gameObject.Value;
			if (value != null && this.reverseOnExit)
			{
				foreach (object obj in value.transform)
				{
					((Transform)obj).gameObject.SetActive(!this.activate);
				}
			}
			base.Finish();
		}

		// Token: 0x04005748 RID: 22344
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject gameObject;

		// Token: 0x04005749 RID: 22345
		public bool activate;

		// Token: 0x0400574A RID: 22346
		public bool reverseOnExit;
	}
}
