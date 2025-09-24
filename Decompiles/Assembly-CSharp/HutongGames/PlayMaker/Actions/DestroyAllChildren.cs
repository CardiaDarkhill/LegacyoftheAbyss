using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C12 RID: 3090
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Destroy all children on a GameObject.")]
	public class DestroyAllChildren : FsmStateAction
	{
		// Token: 0x06005E36 RID: 24118 RVA: 0x001DAF5D File Offset: 0x001D915D
		public override void Reset()
		{
			this.gameObject = null;
			this.disable = new FsmBool(false);
		}

		// Token: 0x06005E37 RID: 24119 RVA: 0x001DAF78 File Offset: 0x001D9178
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				foreach (object obj in value.transform)
				{
					Transform transform = (Transform)obj;
					if (this.disable.Value)
					{
						transform.gameObject.SetActive(false);
					}
					else
					{
						Object.Destroy(transform.gameObject);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04005A82 RID: 23170
		[RequiredField]
		public FsmGameObject gameObject;

		// Token: 0x04005A83 RID: 23171
		public FsmBool disable;
	}
}
