using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ECD RID: 3789
	[ActionCategory(ActionCategory.GUIElement)]
	[Tooltip("Performs a Hit Test on a Game Object with a GUITexture or GUIText component.")]
	[Obsolete("GUIElement is part of the legacy UI system removed in 2019.3")]
	public class GUIElementHitTest : FsmStateAction
	{
		// Token: 0x06006AE3 RID: 27363 RVA: 0x0021549C File Offset: 0x0021369C
		public override void Reset()
		{
			this.gameObject = null;
			this.camera = null;
			this.screenPoint = new FsmVector3
			{
				UseVariable = true
			};
			this.screenX = new FsmFloat
			{
				UseVariable = true
			};
			this.screenY = new FsmFloat
			{
				UseVariable = true
			};
			this.normalized = true;
			this.hitEvent = null;
			this.everyFrame = true;
		}

		// Token: 0x06006AE4 RID: 27364 RVA: 0x0021550C File Offset: 0x0021370C
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x04006A2A RID: 27178
		[RequiredField]
		[ActionSection("Obsolete. Use Unity UI instead.")]
		[Tooltip("The GameObject that has a GUITexture or GUIText component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006A2B RID: 27179
		[Tooltip("Specify camera or use MainCamera as default.")]
		public Camera camera;

		// Token: 0x04006A2C RID: 27180
		[Tooltip("A vector position on screen. Usually stored by actions like GetTouchInfo, or World To Screen Point.")]
		public FsmVector3 screenPoint;

		// Token: 0x04006A2D RID: 27181
		[Tooltip("Specify screen X coordinate.")]
		public FsmFloat screenX;

		// Token: 0x04006A2E RID: 27182
		[Tooltip("Specify screen Y coordinate.")]
		public FsmFloat screenY;

		// Token: 0x04006A2F RID: 27183
		[Tooltip("Whether the specified screen coordinates are normalized (0-1).")]
		public FsmBool normalized;

		// Token: 0x04006A30 RID: 27184
		[Tooltip("Event to send if the Hit Test is true.")]
		public FsmEvent hitEvent;

		// Token: 0x04006A31 RID: 27185
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of the Hit Test in a bool variable (true/false).")]
		public FsmBool storeResult;

		// Token: 0x04006A32 RID: 27186
		[Tooltip("Repeat every frame. Useful if you want to wait for the hit test to return true.")]
		public FsmBool everyFrame;

		// Token: 0x04006A33 RID: 27187
		private GameObject gameObjectCached;
	}
}
