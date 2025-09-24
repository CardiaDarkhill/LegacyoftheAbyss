using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ED4 RID: 3796
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Scales the GUI around a pivot point. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class ScaleGUI : FsmStateAction
	{
		// Token: 0x06006AF7 RID: 27383 RVA: 0x002158D0 File Offset: 0x00213AD0
		public override void Reset()
		{
			this.scaleX = 1f;
			this.scaleY = 1f;
			this.pivotX = 0.5f;
			this.pivotY = 0.5f;
			this.normalized = true;
			this.applyGlobally = false;
		}

		// Token: 0x06006AF8 RID: 27384 RVA: 0x0021592C File Offset: 0x00213B2C
		public override void OnGUI()
		{
			if (this.applied)
			{
				return;
			}
			Vector2 vector = new Vector2(this.scaleX.Value, this.scaleY.Value);
			if (object.Equals(vector.x, 0))
			{
				vector.x = 0.0001f;
			}
			if (object.Equals(vector.y, 0))
			{
				vector.x = 0.0001f;
			}
			Vector2 pivotPoint = new Vector2(this.pivotX.Value, this.pivotY.Value);
			if (this.normalized)
			{
				pivotPoint.x *= (float)Screen.width;
				pivotPoint.y *= (float)Screen.height;
			}
			GUIUtility.ScaleAroundPivot(vector, pivotPoint);
			if (this.applyGlobally)
			{
				PlayMakerGUI.GUIMatrix = GUI.matrix;
				this.applied = true;
			}
		}

		// Token: 0x06006AF9 RID: 27385 RVA: 0x00215A0E File Offset: 0x00213C0E
		public override void OnUpdate()
		{
			this.applied = false;
		}

		// Token: 0x04006A45 RID: 27205
		[RequiredField]
		[Tooltip("Scale in x (1 = 100%)")]
		public FsmFloat scaleX;

		// Token: 0x04006A46 RID: 27206
		[RequiredField]
		[Tooltip("Scale in y (1 = 100%)")]
		public FsmFloat scaleY;

		// Token: 0x04006A47 RID: 27207
		[RequiredField]
		[Tooltip("Scale around this x screen coordinate.")]
		public FsmFloat pivotX;

		// Token: 0x04006A48 RID: 27208
		[RequiredField]
		[Tooltip("Scale around this y screen coordinate.")]
		public FsmFloat pivotY;

		// Token: 0x04006A49 RID: 27209
		[Tooltip("Pivot point uses normalized coordinates (0-1). E.g. 0.5 is the center of the screen.")]
		public bool normalized;

		// Token: 0x04006A4A RID: 27210
		[Tooltip("Apply to all GUI actions in all FSMs.")]
		public bool applyGlobally;

		// Token: 0x04006A4B RID: 27211
		private bool applied;
	}
}
