using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EE2 RID: 3810
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Begin a GUILayout area that follows the specified game object. Useful for overlays (e.g., playerName). NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	public class GUILayoutBeginAreaFollowObject : FsmStateAction
	{
		// Token: 0x06006B1E RID: 27422 RVA: 0x00215F68 File Offset: 0x00214168
		public override void Reset()
		{
			this.gameObject = null;
			this.offsetLeft = 0f;
			this.offsetTop = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
			this.style = "";
		}

		// Token: 0x06006B1F RID: 27423 RVA: 0x00215FD8 File Offset: 0x002141D8
		public override void OnGUI()
		{
			GameObject value = this.gameObject.Value;
			if (value == null || Camera.main == null)
			{
				GUILayoutBeginAreaFollowObject.DummyBeginArea();
				return;
			}
			Vector3 position = value.transform.position;
			if (Camera.main.transform.InverseTransformPoint(position).z < 0f)
			{
				GUILayoutBeginAreaFollowObject.DummyBeginArea();
				return;
			}
			Vector2 vector = Camera.main.WorldToScreenPoint(position);
			float x = vector.x + (this.normalized.Value ? (this.offsetLeft.Value * (float)Screen.width) : this.offsetLeft.Value);
			float y = vector.y + (this.normalized.Value ? (this.offsetTop.Value * (float)Screen.width) : this.offsetTop.Value);
			Rect screenRect = new Rect(x, y, this.width.Value, this.height.Value);
			if (this.normalized.Value)
			{
				screenRect.width *= (float)Screen.width;
				screenRect.height *= (float)Screen.height;
			}
			screenRect.y = (float)Screen.height - screenRect.y;
			GUILayout.BeginArea(screenRect, this.style.Value);
		}

		// Token: 0x06006B20 RID: 27424 RVA: 0x00216130 File Offset: 0x00214330
		private static void DummyBeginArea()
		{
			GUILayout.BeginArea(default(Rect));
		}

		// Token: 0x04006A69 RID: 27241
		[RequiredField]
		[Tooltip("The GameObject to follow.")]
		public FsmGameObject gameObject;

		// Token: 0x04006A6A RID: 27242
		[RequiredField]
		[Tooltip("Left screen offset.")]
		public FsmFloat offsetLeft;

		// Token: 0x04006A6B RID: 27243
		[RequiredField]
		[Tooltip("Screen offset up.")]
		public FsmFloat offsetTop;

		// Token: 0x04006A6C RID: 27244
		[RequiredField]
		[Tooltip("Width of area.")]
		public FsmFloat width;

		// Token: 0x04006A6D RID: 27245
		[RequiredField]
		[Tooltip("Height of area.")]
		public FsmFloat height;

		// Token: 0x04006A6E RID: 27246
		[Tooltip("Use normalized screen coordinates (0-1).")]
		public FsmBool normalized;

		// Token: 0x04006A6F RID: 27247
		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;
	}
}
