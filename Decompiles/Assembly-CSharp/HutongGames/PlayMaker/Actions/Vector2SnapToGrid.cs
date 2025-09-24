using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001196 RID: 4502
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Snap Vector2 coordinates to grid points.")]
	public class Vector2SnapToGrid : FsmStateAction
	{
		// Token: 0x06007886 RID: 30854 RVA: 0x00248128 File Offset: 0x00246328
		public override void Reset()
		{
			this.vector2Variable = null;
			this.gridSize = new FsmFloat
			{
				Value = 1f
			};
		}

		// Token: 0x06007887 RID: 30855 RVA: 0x00248147 File Offset: 0x00246347
		public override void OnEnter()
		{
			this.DoSnapToGrid();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007888 RID: 30856 RVA: 0x0024815D File Offset: 0x0024635D
		public override void OnUpdate()
		{
			this.DoSnapToGrid();
		}

		// Token: 0x06007889 RID: 30857 RVA: 0x00248168 File Offset: 0x00246368
		private void DoSnapToGrid()
		{
			if (this.gridSize.Value < 0.001f)
			{
				return;
			}
			Vector2 vector = this.vector2Variable.Value;
			float value = this.gridSize.Value;
			vector /= value;
			vector.Set(Mathf.Round(vector.x), Mathf.Round(vector.y));
			this.vector2Variable.Value = vector * value;
		}

		// Token: 0x040078F4 RID: 30964
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector2 Variable to snap.")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078F5 RID: 30965
		[Tooltip("Grid Size.")]
		public FsmFloat gridSize;

		// Token: 0x040078F6 RID: 30966
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
