using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001063 RID: 4195
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the Scale of a Game Object sends an event based on positivity or negativity of x/y value")]
	public class SendEventByScale : FsmStateAction
	{
		// Token: 0x060072A9 RID: 29353 RVA: 0x002349D2 File Offset: 0x00232BD2
		public override void Reset()
		{
			this.xScale = true;
			this.gameObject = null;
			this.space = Space.World;
		}

		// Token: 0x060072AA RID: 29354 RVA: 0x002349EC File Offset: 0x00232BEC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = (this.space == Space.World) ? ownerDefaultTarget.transform.lossyScale : ownerDefaultTarget.transform.localScale;
			float num;
			if (this.xScale)
			{
				num = vector.x;
			}
			else
			{
				num = vector.y;
			}
			if (num > 0f)
			{
				base.Fsm.Event(this.eventTarget, this.positiveEvent);
			}
			else
			{
				base.Fsm.Event(this.eventTarget, this.negativeEvent);
			}
			base.Finish();
		}

		// Token: 0x040072A9 RID: 29353
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072AA RID: 29354
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x040072AB RID: 29355
		[Tooltip("If false, check Y scale")]
		public bool xScale;

		// Token: 0x040072AC RID: 29356
		public FsmEvent positiveEvent;

		// Token: 0x040072AD RID: 29357
		public FsmEvent negativeEvent;

		// Token: 0x040072AE RID: 29358
		public Space space;
	}
}
