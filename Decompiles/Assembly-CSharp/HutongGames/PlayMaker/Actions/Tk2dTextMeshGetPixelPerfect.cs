using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B86 RID: 2950
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the pixelPerfect flag of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetPixelPerfect : FsmStateAction
	{
		// Token: 0x06005B6A RID: 23402 RVA: 0x001CCC66 File Offset: 0x001CAE66
		public override void Reset()
		{
			this.gameObject = null;
			this.pixelPerfect = null;
		}

		// Token: 0x06005B6B RID: 23403 RVA: 0x001CCC76 File Offset: 0x001CAE76
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x040056D7 RID: 22231
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056D8 RID: 22232
		[RequiredField]
		[Tooltip("(Deprecated in 2D Toolkit 2.0) Is the text pixelPerfect")]
		[UIHint(UIHint.Variable)]
		public FsmBool pixelPerfect;
	}
}
