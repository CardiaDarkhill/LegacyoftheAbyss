using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E4E RID: 3662
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Sets the Background Color used by the Camera.")]
	public class SetBackgroundColor : ComponentAction<Camera>
	{
		// Token: 0x060068B0 RID: 26800 RVA: 0x0020E05E File Offset: 0x0020C25E
		public override void Reset()
		{
			this.gameObject = null;
			this.backgroundColor = Color.black;
			this.everyFrame = false;
		}

		// Token: 0x060068B1 RID: 26801 RVA: 0x0020E07E File Offset: 0x0020C27E
		public override void OnEnter()
		{
			this.DoSetBackgroundColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068B2 RID: 26802 RVA: 0x0020E094 File Offset: 0x0020C294
		public override void OnUpdate()
		{
			this.DoSetBackgroundColor();
		}

		// Token: 0x060068B3 RID: 26803 RVA: 0x0020E09C File Offset: 0x0020C29C
		private void DoSetBackgroundColor()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.camera.backgroundColor = this.backgroundColor.Value;
			}
		}

		// Token: 0x040067E3 RID: 26595
		[RequiredField]
		[CheckForComponent(typeof(Camera))]
		[Tooltip("The game object that owns the Camera component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067E4 RID: 26596
		[RequiredField]
		[Tooltip("The background color.")]
		public FsmColor backgroundColor;

		// Token: 0x040067E5 RID: 26597
		[Tooltip("Repeat every frame. Useful if the color is animated.")]
		public bool everyFrame;
	}
}
