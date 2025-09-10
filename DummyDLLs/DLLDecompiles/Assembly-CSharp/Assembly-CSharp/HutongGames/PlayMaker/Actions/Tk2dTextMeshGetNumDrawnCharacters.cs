using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B85 RID: 2949
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the number of drawn characters of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetNumDrawnCharacters : FsmStateAction
	{
		// Token: 0x06005B64 RID: 23396 RVA: 0x001CCBBC File Offset: 0x001CADBC
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B65 RID: 23397 RVA: 0x001CCBF1 File Offset: 0x001CADF1
		public override void Reset()
		{
			this.gameObject = null;
			this.numDrawnCharacters = null;
			this.everyframe = false;
		}

		// Token: 0x06005B66 RID: 23398 RVA: 0x001CCC08 File Offset: 0x001CAE08
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetNumDrawnCharacters();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B67 RID: 23399 RVA: 0x001CCC24 File Offset: 0x001CAE24
		public override void OnUpdate()
		{
			this.DoGetNumDrawnCharacters();
		}

		// Token: 0x06005B68 RID: 23400 RVA: 0x001CCC2C File Offset: 0x001CAE2C
		private void DoGetNumDrawnCharacters()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component");
				return;
			}
			this.numDrawnCharacters.Value = this._textMesh.NumDrawnCharacters();
		}

		// Token: 0x040056D3 RID: 22227
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056D4 RID: 22228
		[RequiredField]
		[Tooltip("The number of drawn characters")]
		[UIHint(UIHint.Variable)]
		public FsmInt numDrawnCharacters;

		// Token: 0x040056D5 RID: 22229
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056D6 RID: 22230
		private tk2dTextMesh _textMesh;
	}
}
