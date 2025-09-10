using System;
using TMProOld;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D58 RID: 3416
	[ActionCategory("TextMeshPro")]
	[Tooltip("Set TextMeshPro color.")]
	public class SetTextMeshProColor : FsmStateAction
	{
		// Token: 0x060063FE RID: 25598 RVA: 0x001F831B File Offset: 0x001F651B
		public override void Reset()
		{
			this.gameObject = null;
			this.color = null;
			this.everyFrame = false;
		}

		// Token: 0x060063FF RID: 25599 RVA: 0x001F8334 File Offset: 0x001F6534
		public override void OnEnter()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.gameObject != null)
			{
				this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				this.textMesh = this.go.GetComponent<TextMeshPro>();
				if (this.textMesh != null)
				{
					this.textMesh.color = this.color.Value;
				}
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006400 RID: 25600 RVA: 0x001F83BC File Offset: 0x001F65BC
		public override void OnUpdate()
		{
			if (this.gameObject != null)
			{
				this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				this.textMesh = this.go.GetComponent<TextMeshPro>();
				if (this.textMesh != null)
				{
					this.textMesh.color = this.color.Value;
				}
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x04006269 RID: 25193
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400626A RID: 25194
		[RequiredField]
		public FsmColor color;

		// Token: 0x0400626B RID: 25195
		public bool everyFrame;

		// Token: 0x0400626C RID: 25196
		private GameObject go;

		// Token: 0x0400626D RID: 25197
		private TextMeshPro textMesh;
	}
}
