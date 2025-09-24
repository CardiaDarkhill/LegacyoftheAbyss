using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ED3 RID: 3795
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Rotates the GUI around a pivot point. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class RotateGUI : FsmStateAction
	{
		// Token: 0x06006AF3 RID: 27379 RVA: 0x002157F7 File Offset: 0x002139F7
		public override void Reset()
		{
			this.angle = 0f;
			this.pivotX = 0.5f;
			this.pivotY = 0.5f;
			this.normalized = true;
			this.applyGlobally = false;
		}

		// Token: 0x06006AF4 RID: 27380 RVA: 0x00215838 File Offset: 0x00213A38
		public override void OnGUI()
		{
			if (this.applied)
			{
				return;
			}
			Vector2 pivotPoint = new Vector2(this.pivotX.Value, this.pivotY.Value);
			if (this.normalized)
			{
				pivotPoint.x *= (float)Screen.width;
				pivotPoint.y *= (float)Screen.height;
			}
			GUIUtility.RotateAroundPivot(this.angle.Value, pivotPoint);
			if (this.applyGlobally)
			{
				PlayMakerGUI.GUIMatrix = GUI.matrix;
				this.applied = true;
			}
		}

		// Token: 0x06006AF5 RID: 27381 RVA: 0x002158BF File Offset: 0x00213ABF
		public override void OnUpdate()
		{
			this.applied = false;
		}

		// Token: 0x04006A3F RID: 27199
		[RequiredField]
		[Tooltip("Angle to rotate in degrees.")]
		public FsmFloat angle;

		// Token: 0x04006A40 RID: 27200
		[RequiredField]
		[Tooltip("X coordinate of pivot.")]
		public FsmFloat pivotX;

		// Token: 0x04006A41 RID: 27201
		[RequiredField]
		[Tooltip("Y coordinate of pivot.")]
		public FsmFloat pivotY;

		// Token: 0x04006A42 RID: 27202
		[Tooltip("Use normalized screen coordinates (0-1).")]
		public bool normalized;

		// Token: 0x04006A43 RID: 27203
		[Tooltip("Apply to all GUI actions in all FSMs.")]
		public bool applyGlobally;

		// Token: 0x04006A44 RID: 27204
		private bool applied;
	}
}
