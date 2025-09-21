using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001267 RID: 4711
	public class HeroAddExtraAirMoveVelocity : FsmStateAction
	{
		// Token: 0x06007C4D RID: 31821 RVA: 0x00252BFE File Offset: 0x00250DFE
		public override void Reset()
		{
			this.Velocity = null;
			this.Decay = 4f;
			this.CancelOnTurn = true;
			this.SkipApplyWhileMoving = true;
		}

		// Token: 0x06007C4E RID: 31822 RVA: 0x00252C30 File Offset: 0x00250E30
		public override void OnEnter()
		{
			if (this.Velocity.Value.magnitude <= Mathf.Epsilon)
			{
				base.Finish();
				return;
			}
			HeroController.instance.AddExtraAirMoveVelocity(new HeroController.DecayingVelocity
			{
				Velocity = this.Velocity.Value,
				Decay = this.Decay.Value,
				CancelOnTurn = this.CancelOnTurn.Value,
				SkipBehaviour = (this.SkipApplyWhileMoving.Value ? HeroController.DecayingVelocity.SkipBehaviours.WhileMoving : HeroController.DecayingVelocity.SkipBehaviours.None)
			});
			base.Finish();
		}

		// Token: 0x04007C5D RID: 31837
		public FsmVector2 Velocity;

		// Token: 0x04007C5E RID: 31838
		public FsmFloat Decay;

		// Token: 0x04007C5F RID: 31839
		public FsmBool CancelOnTurn;

		// Token: 0x04007C60 RID: 31840
		public FsmBool SkipApplyWhileMoving;
	}
}
