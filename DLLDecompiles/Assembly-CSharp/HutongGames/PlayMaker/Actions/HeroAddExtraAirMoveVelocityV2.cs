using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001268 RID: 4712
	public class HeroAddExtraAirMoveVelocityV2 : FsmStateAction
	{
		// Token: 0x06007C50 RID: 31824 RVA: 0x00252CCD File Offset: 0x00250ECD
		public override void Reset()
		{
			this.Velocity = null;
			this.Decay = 4f;
			this.CancelOnTurn = true;
			this.SkipBehaviour = HeroController.DecayingVelocity.SkipBehaviours.WhileMoving;
		}

		// Token: 0x06007C51 RID: 31825 RVA: 0x00252D04 File Offset: 0x00250F04
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
				SkipBehaviour = (HeroController.DecayingVelocity.SkipBehaviours)this.SkipBehaviour.Value
			});
			base.Finish();
		}

		// Token: 0x04007C61 RID: 31841
		public FsmVector2 Velocity;

		// Token: 0x04007C62 RID: 31842
		public FsmFloat Decay;

		// Token: 0x04007C63 RID: 31843
		public FsmBool CancelOnTurn;

		// Token: 0x04007C64 RID: 31844
		[ObjectType(typeof(HeroController.DecayingVelocity.SkipBehaviours))]
		public FsmEnum SkipBehaviour;
	}
}
