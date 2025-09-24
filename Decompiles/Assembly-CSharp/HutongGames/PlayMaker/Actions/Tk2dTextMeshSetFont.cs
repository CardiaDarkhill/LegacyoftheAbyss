using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8E RID: 2958
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the font of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetFont : FsmStateAction
	{
		// Token: 0x06005B96 RID: 23446 RVA: 0x001CD5EC File Offset: 0x001CB7EC
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B97 RID: 23447 RVA: 0x001CD621 File Offset: 0x001CB821
		public override void Reset()
		{
			this.gameObject = null;
			this.font = null;
			this.commit = true;
		}

		// Token: 0x06005B98 RID: 23448 RVA: 0x001CD63D File Offset: 0x001CB83D
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoSetFont();
			base.Finish();
		}

		// Token: 0x06005B99 RID: 23449 RVA: 0x001CD654 File Offset: 0x001CB854
		private void DoSetFont()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			GameObject value = this.font.Value;
			if (value == null)
			{
				return;
			}
			tk2dFont component = value.GetComponent<tk2dFont>();
			if (component == null)
			{
				return;
			}
			this._textMesh.font = component.data;
			this._textMesh.GetComponent<Renderer>().material = component.material;
			this._textMesh.Init(true);
		}

		// Token: 0x04005701 RID: 22273
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005702 RID: 22274
		[RequiredField]
		[Tooltip("The font gameObject")]
		[UIHint(UIHint.FsmGameObject)]
		[CheckForComponent(typeof(tk2dFont))]
		public FsmGameObject font;

		// Token: 0x04005703 RID: 22275
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmString)]
		public FsmBool commit;

		// Token: 0x04005704 RID: 22276
		private tk2dTextMesh _textMesh;
	}
}
