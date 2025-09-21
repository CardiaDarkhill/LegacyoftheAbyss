using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EE5 RID: 3813
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Begins a ScrollView. Use GUILayoutEndScrollView at the end of the block.")]
	public class GUILayoutBeginScrollView : GUILayoutAction
	{
		// Token: 0x06006B28 RID: 27432 RVA: 0x0021621E File Offset: 0x0021441E
		public override void Reset()
		{
			base.Reset();
			this.scrollPosition = null;
			this.horizontalScrollbar = null;
			this.verticalScrollbar = null;
			this.useCustomStyle = null;
			this.horizontalStyle = null;
			this.verticalStyle = null;
			this.backgroundStyle = null;
		}

		// Token: 0x06006B29 RID: 27433 RVA: 0x00216258 File Offset: 0x00214458
		public override void OnGUI()
		{
			if (this.useCustomStyle.Value)
			{
				this.scrollPosition.Value = GUILayout.BeginScrollView(this.scrollPosition.Value, this.horizontalScrollbar.Value, this.verticalScrollbar.Value, this.horizontalStyle.Value, this.verticalStyle.Value, this.backgroundStyle.Value, base.LayoutOptions);
				return;
			}
			this.scrollPosition.Value = GUILayout.BeginScrollView(this.scrollPosition.Value, this.horizontalScrollbar.Value, this.verticalScrollbar.Value, base.LayoutOptions);
		}

		// Token: 0x04006A74 RID: 27252
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Assign a Vector2 variable to store the scroll position of this view.")]
		public FsmVector2 scrollPosition;

		// Token: 0x04006A75 RID: 27253
		[Tooltip("Always show the horizontal scrollbars.")]
		public FsmBool horizontalScrollbar;

		// Token: 0x04006A76 RID: 27254
		[Tooltip("Always show the vertical scrollbars.")]
		public FsmBool verticalScrollbar;

		// Token: 0x04006A77 RID: 27255
		[Tooltip("Define custom styles below. NOTE: You have to define all the styles if you check this option.")]
		public FsmBool useCustomStyle;

		// Token: 0x04006A78 RID: 27256
		[Tooltip("Named style in the active GUISkin for the horizontal scrollbars.")]
		public FsmString horizontalStyle;

		// Token: 0x04006A79 RID: 27257
		[Tooltip("Named style in the active GUISkin for the vertical scrollbars.")]
		public FsmString verticalStyle;

		// Token: 0x04006A7A RID: 27258
		[Tooltip("Named style in the active GUISkin for the background.")]
		public FsmString backgroundStyle;
	}
}
