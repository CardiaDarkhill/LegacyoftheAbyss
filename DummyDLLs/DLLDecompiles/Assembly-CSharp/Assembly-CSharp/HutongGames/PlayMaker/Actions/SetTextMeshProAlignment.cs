using System;
using TMProOld;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D57 RID: 3415
	[ActionCategory("TextMeshPro")]
	[Tooltip("Set TextMeshPro color.")]
	public class SetTextMeshProAlignment : FsmStateAction
	{
		// Token: 0x060063FB RID: 25595 RVA: 0x001F8170 File Offset: 0x001F6370
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060063FC RID: 25596 RVA: 0x001F817C File Offset: 0x001F637C
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (this.go != null)
				{
					this.textMesh = this.go.GetComponent<TextMeshPro>();
					if (this.textMesh != null)
					{
						if (this.topLeft.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.TopLeft;
						}
						if (this.topRight.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.TopRight;
						}
						if (this.topCentre.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.Top;
						}
						if (this.topJustified.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.TopJustified;
						}
						if (this.centreLeft.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.Left;
						}
						if (this.centreRight.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.Right;
						}
						if (this.centreCentre.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.Center;
						}
						if (this.centreJustified.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.Justified;
						}
						if (this.bottomLeft.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.BottomLeft;
						}
						if (this.bottomRight.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.BottomRight;
						}
						if (this.bottomCentre.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.Bottom;
						}
						if (this.bottomJustified.Value)
						{
							this.textMesh.alignment = TextAlignmentOptions.BottomJustified;
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0400625A RID: 25178
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400625B RID: 25179
		[RequiredField]
		public FsmBool topLeft;

		// Token: 0x0400625C RID: 25180
		public FsmBool topRight;

		// Token: 0x0400625D RID: 25181
		public FsmBool topCentre;

		// Token: 0x0400625E RID: 25182
		public FsmBool topJustified;

		// Token: 0x0400625F RID: 25183
		public FsmBool centreLeft;

		// Token: 0x04006260 RID: 25184
		public FsmBool centreRight;

		// Token: 0x04006261 RID: 25185
		public FsmBool centreCentre;

		// Token: 0x04006262 RID: 25186
		public FsmBool centreJustified;

		// Token: 0x04006263 RID: 25187
		public FsmBool bottomLeft;

		// Token: 0x04006264 RID: 25188
		public FsmBool bottomRight;

		// Token: 0x04006265 RID: 25189
		public FsmBool bottomCentre;

		// Token: 0x04006266 RID: 25190
		public FsmBool bottomJustified;

		// Token: 0x04006267 RID: 25191
		private GameObject go;

		// Token: 0x04006268 RID: 25192
		private TextMeshPro textMesh;
	}
}
