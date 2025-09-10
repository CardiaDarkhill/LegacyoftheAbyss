using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200118F RID: 4495
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Moves a Vector2 towards a Target. Optionally sends an event when successful.")]
	public class Vector2MoveTowards : FsmStateAction
	{
		// Token: 0x06007868 RID: 30824 RVA: 0x00247B25 File Offset: 0x00245D25
		public override void Reset()
		{
			this.source = null;
			this.target = null;
			this.maxSpeed = 10f;
			this.finishDistance = 1f;
			this.finishEvent = null;
		}

		// Token: 0x06007869 RID: 30825 RVA: 0x00247B5C File Offset: 0x00245D5C
		public override void OnUpdate()
		{
			this.DoMoveTowards();
		}

		// Token: 0x0600786A RID: 30826 RVA: 0x00247B64 File Offset: 0x00245D64
		private void DoMoveTowards()
		{
			this.source.Value = Vector2.MoveTowards(this.source.Value, this.target.Value, this.maxSpeed.Value * Time.deltaTime);
			if ((this.source.Value - this.target.Value).magnitude < this.finishDistance.Value)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
			}
		}

		// Token: 0x040078D9 RID: 30937
		[RequiredField]
		[Tooltip("The Vector2 to Move")]
		public FsmVector2 source;

		// Token: 0x040078DA RID: 30938
		[Tooltip("A target Vector2 to move towards.")]
		public FsmVector2 target;

		// Token: 0x040078DB RID: 30939
		[HasFloatSlider(0f, 20f)]
		[Tooltip("The maximum movement speed. HINT: You can make this a variable to change it over time.")]
		public FsmFloat maxSpeed;

		// Token: 0x040078DC RID: 30940
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Distance at which the move is considered finished, and the Finish Event is sent.")]
		public FsmFloat finishDistance;

		// Token: 0x040078DD RID: 30941
		[Tooltip("Event to send when the Finish Distance is reached.")]
		public FsmEvent finishEvent;
	}
}
