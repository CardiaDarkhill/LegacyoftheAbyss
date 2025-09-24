using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EDB RID: 3803
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Controls the appearance of Mouse Cursor.")]
	public class SetMouseCursor : FsmStateAction
	{
		// Token: 0x06006B0E RID: 27406 RVA: 0x00215BE8 File Offset: 0x00213DE8
		public override void Reset()
		{
			this.cursorTexture = null;
			this.hideCursor = false;
			this.lockCursor = false;
			this.everyFrame = false;
		}

		// Token: 0x06006B0F RID: 27407 RVA: 0x00215C10 File Offset: 0x00213E10
		public override void OnEnter()
		{
			PlayMakerGUI.LockCursor = this.lockCursor.Value;
			PlayMakerGUI.HideCursor = this.hideCursor.Value;
			PlayMakerGUI.MouseCursor = this.cursorTexture.Value;
			this.UpdateCursorState();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B10 RID: 27408 RVA: 0x00215C61 File Offset: 0x00213E61
		private void UpdateCursorState()
		{
			Cursor.lockState = (this.lockCursor.Value ? CursorLockMode.Locked : CursorLockMode.None);
			Cursor.visible = !this.hideCursor.Value;
		}

		// Token: 0x06006B11 RID: 27409 RVA: 0x00215C8C File Offset: 0x00213E8C
		public override void OnUpdate()
		{
			this.UpdateCursorState();
		}

		// Token: 0x06006B12 RID: 27410 RVA: 0x00215C94 File Offset: 0x00213E94
		public override void OnGUI()
		{
			if (PlayMakerGUI.Exists)
			{
				return;
			}
			Texture value = this.cursorTexture.Value;
			if (value != null)
			{
				Vector3 mousePosition = ActionHelpers.GetMousePosition();
				GUI.DrawTexture(new Rect(mousePosition.x - (float)value.width * 0.5f, (float)Screen.height - mousePosition.y - (float)value.height * 0.5f, (float)value.width, (float)value.height), value);
			}
		}

		// Token: 0x04006A57 RID: 27223
		[Tooltip("The texture to use for the cursor.")]
		public FsmTexture cursorTexture;

		// Token: 0x04006A58 RID: 27224
		[Tooltip("Hide the cursor.")]
		public FsmBool hideCursor;

		// Token: 0x04006A59 RID: 27225
		[Tooltip("Lock the cursor to the center of the screen. Useful in first person controllers.")]
		public FsmBool lockCursor;

		// Token: 0x04006A5A RID: 27226
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
