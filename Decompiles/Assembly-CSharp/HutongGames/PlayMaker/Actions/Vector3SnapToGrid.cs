using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011AC RID: 4524
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Snap Vector3 coordinates to grid points.")]
	public class Vector3SnapToGrid : FsmStateAction
	{
		// Token: 0x060078E7 RID: 30951 RVA: 0x002492B5 File Offset: 0x002474B5
		public override void Reset()
		{
			this.vector3Variable = null;
			this.gridSize = new FsmFloat
			{
				Value = 1f
			};
		}

		// Token: 0x060078E8 RID: 30952 RVA: 0x002492D4 File Offset: 0x002474D4
		public override void OnEnter()
		{
			this.DoSnapToGrid();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078E9 RID: 30953 RVA: 0x002492EA File Offset: 0x002474EA
		public override void OnUpdate()
		{
			this.DoSnapToGrid();
		}

		// Token: 0x060078EA RID: 30954 RVA: 0x002492F4 File Offset: 0x002474F4
		private void DoSnapToGrid()
		{
			if (this.gridSize.Value < 0.001f)
			{
				return;
			}
			Vector3 vector = this.vector3Variable.Value;
			float value = this.gridSize.Value;
			vector /= value;
			vector.Set(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
			this.vector3Variable.Value = vector * value;
		}

		// Token: 0x04007947 RID: 31047
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector3 Variable to snap.")]
		public FsmVector3 vector3Variable;

		// Token: 0x04007948 RID: 31048
		[Tooltip("Grid Size.")]
		public FsmFloat gridSize;

		// Token: 0x04007949 RID: 31049
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
