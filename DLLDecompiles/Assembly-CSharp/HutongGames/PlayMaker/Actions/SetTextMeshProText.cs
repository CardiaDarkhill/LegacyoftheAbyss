using System;
using TMProOld;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D59 RID: 3417
	[ActionCategory("TextMeshPro")]
	[Tooltip("Set TextMeshPro color.")]
	public class SetTextMeshProText : FsmStateAction
	{
		// Token: 0x06006402 RID: 25602 RVA: 0x001F8433 File Offset: 0x001F6633
		public override void Reset()
		{
			this.gameObject = null;
			this.textString = null;
		}

		// Token: 0x06006403 RID: 25603 RVA: 0x001F8444 File Offset: 0x001F6644
		public override void OnEnter()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this.textMesh = this.go.GetComponent<TextMeshPro>();
				if (this.textMesh != null)
				{
					this.textMesh.text = this.textString.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x0400626E RID: 25198
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400626F RID: 25199
		[RequiredField]
		public FsmString textString;

		// Token: 0x04006270 RID: 25200
		private GameObject go;

		// Token: 0x04006271 RID: 25201
		private TextMeshPro textMesh;
	}
}
