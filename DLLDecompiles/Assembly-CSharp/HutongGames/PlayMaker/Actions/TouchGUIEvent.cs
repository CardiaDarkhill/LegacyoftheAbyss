using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E92 RID: 3730
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends events when a GUI Texture or GUI Text is touched. Optionally filter by a fingerID.")]
	[Obsolete("GUIElement is part of the legacy UI system removed in 2019.3")]
	public class TouchGUIEvent : FsmStateAction
	{
		// Token: 0x060069EB RID: 27115 RVA: 0x00211F20 File Offset: 0x00210120
		public override void Reset()
		{
			this.gameObject = null;
			this.fingerId = new FsmInt
			{
				UseVariable = true
			};
			this.touchBegan = null;
			this.touchMoved = null;
			this.touchStationary = null;
			this.touchEnded = null;
			this.touchCanceled = null;
			this.storeFingerId = null;
			this.storeHitPoint = null;
			this.normalizeHitPoint = false;
			this.storeOffset = null;
			this.relativeTo = TouchGUIEvent.OffsetOptions.Center;
			this.normalizeOffset = true;
			this.everyFrame = true;
		}

		// Token: 0x060069EC RID: 27116 RVA: 0x00211FA4 File Offset: 0x002101A4
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x04006929 RID: 26921
		[RequiredField]
		[ActionSection("Obsolete. Use Unity UI instead.")]
		[Tooltip("The Game Object that owns the GUI Texture or GUI Text.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400692A RID: 26922
		[Tooltip("Only detect touches that match this fingerID, or set to None.")]
		public FsmInt fingerId;

		// Token: 0x0400692B RID: 26923
		[ActionSection("Events")]
		[Tooltip("Event to send on touch began.")]
		public FsmEvent touchBegan;

		// Token: 0x0400692C RID: 26924
		[Tooltip("Event to send on touch moved.")]
		public FsmEvent touchMoved;

		// Token: 0x0400692D RID: 26925
		[Tooltip("Event to send on stationary touch.")]
		public FsmEvent touchStationary;

		// Token: 0x0400692E RID: 26926
		[Tooltip("Event to send on touch ended.")]
		public FsmEvent touchEnded;

		// Token: 0x0400692F RID: 26927
		[Tooltip("Event to send on touch cancel.")]
		public FsmEvent touchCanceled;

		// Token: 0x04006930 RID: 26928
		[Tooltip("Event to send if not touching (finger down but not over the GUI element)")]
		public FsmEvent notTouching;

		// Token: 0x04006931 RID: 26929
		[ActionSection("Store Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the fingerId of the touch.")]
		public FsmInt storeFingerId;

		// Token: 0x04006932 RID: 26930
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen position where the GUI element was touched.")]
		public FsmVector3 storeHitPoint;

		// Token: 0x04006933 RID: 26931
		[Tooltip("Normalize the hit point screen coordinates (0-1).")]
		public FsmBool normalizeHitPoint;

		// Token: 0x04006934 RID: 26932
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the offset position of the hit.")]
		public FsmVector3 storeOffset;

		// Token: 0x04006935 RID: 26933
		[Tooltip("How to measure the offset.")]
		public TouchGUIEvent.OffsetOptions relativeTo;

		// Token: 0x04006936 RID: 26934
		[Tooltip("Normalize the offset.")]
		public FsmBool normalizeOffset;

		// Token: 0x04006937 RID: 26935
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04006938 RID: 26936
		private Vector3 touchStartPos;

		// Token: 0x02001BA8 RID: 7080
		public enum OffsetOptions
		{
			// Token: 0x04009E15 RID: 40469
			TopLeft,
			// Token: 0x04009E16 RID: 40470
			Center,
			// Token: 0x04009E17 RID: 40471
			TouchStart
		}
	}
}
