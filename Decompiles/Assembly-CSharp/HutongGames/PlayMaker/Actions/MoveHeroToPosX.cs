using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200124C RID: 4684
	public class MoveHeroToPosX : FsmStateAction
	{
		// Token: 0x06007BD3 RID: 31699 RVA: 0x002508CC File Offset: 0x0024EACC
		public bool IsUsingTarget()
		{
			return !this.PositionTarget.IsNone;
		}

		// Token: 0x06007BD4 RID: 31700 RVA: 0x002508DC File Offset: 0x0024EADC
		public override void Reset()
		{
			this.Hero = null;
			this.PositionTarget = new FsmGameObject
			{
				UseVariable = true
			};
			this.PositionX = null;
		}

		// Token: 0x06007BD5 RID: 31701 RVA: 0x00250900 File Offset: 0x0024EB00
		public override void OnEnter()
		{
			GameObject safe = this.Hero.GetSafe(this);
			if (safe)
			{
				HeroController component = safe.GetComponent<HeroController>();
				if (component)
				{
					float targetX = this.PositionX.Value;
					if (this.PositionTarget.Value)
					{
						targetX = this.PositionTarget.Value.transform.position.x;
					}
					this.moveRoutine = base.StartCoroutine(component.MoveToPositionX(targetX, new Action(this.End)));
					return;
				}
			}
			base.Finish();
		}

		// Token: 0x06007BD6 RID: 31702 RVA: 0x00250990 File Offset: 0x0024EB90
		private void End()
		{
			base.Fsm.Event(this.FinishEvent);
			base.Finish();
		}

		// Token: 0x06007BD7 RID: 31703 RVA: 0x002509A9 File Offset: 0x0024EBA9
		public override void OnExit()
		{
			if (this.moveRoutine != null)
			{
				base.StopCoroutine(this.moveRoutine);
				this.moveRoutine = null;
			}
		}

		// Token: 0x04007C02 RID: 31746
		public FsmOwnerDefault Hero;

		// Token: 0x04007C03 RID: 31747
		public FsmGameObject PositionTarget;

		// Token: 0x04007C04 RID: 31748
		[HideIf("IsUsingTarget")]
		public FsmFloat PositionX;

		// Token: 0x04007C05 RID: 31749
		public FsmEvent FinishEvent;

		// Token: 0x04007C06 RID: 31750
		private Coroutine moveRoutine;
	}
}
