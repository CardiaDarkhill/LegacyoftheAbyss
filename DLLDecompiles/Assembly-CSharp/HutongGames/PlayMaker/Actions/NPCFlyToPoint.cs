using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001315 RID: 4885
	public class NPCFlyToPoint : FsmStateAction
	{
		// Token: 0x06007ECD RID: 32461 RVA: 0x00259D7D File Offset: 0x00257F7D
		public override void Reset()
		{
			this.Target = null;
			this.FlyToPos = null;
			this.ArrivedEvent = null;
		}

		// Token: 0x06007ECE RID: 32462 RVA: 0x00259D94 File Offset: 0x00257F94
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			this.flyComponent = safe.GetComponent<NPCFlyAround>();
			this.flyComponent.ArrivedAtPoint += this.OnArrived;
			this.flyComponent.StartFlyToPoint(this.FlyToPos.Value);
		}

		// Token: 0x06007ECF RID: 32463 RVA: 0x00259DF6 File Offset: 0x00257FF6
		public override void OnExit()
		{
			this.OnArrived();
		}

		// Token: 0x06007ED0 RID: 32464 RVA: 0x00259E00 File Offset: 0x00258000
		private void OnArrived()
		{
			if (!this.flyComponent)
			{
				return;
			}
			this.flyComponent.ArrivedAtPoint -= this.OnArrived;
			this.flyComponent = null;
			base.Fsm.Event(this.ArrivedEvent);
			base.Finish();
		}

		// Token: 0x04007E7C RID: 32380
		[CheckForComponent(typeof(NPCFlyAround))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E7D RID: 32381
		public FsmVector2 FlyToPos;

		// Token: 0x04007E7E RID: 32382
		public FsmEvent ArrivedEvent;

		// Token: 0x04007E7F RID: 32383
		private NPCFlyAround flyComponent;
	}
}
