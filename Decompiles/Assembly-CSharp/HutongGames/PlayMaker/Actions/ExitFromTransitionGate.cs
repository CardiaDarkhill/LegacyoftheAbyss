using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2A RID: 3114
	public class ExitFromTransitionGate : FsmStateAction
	{
		// Token: 0x06005EC6 RID: 24262 RVA: 0x001DFF55 File Offset: 0x001DE155
		public override void Reset()
		{
			this.Gate = null;
		}

		// Token: 0x06005EC7 RID: 24263 RVA: 0x001DFF60 File Offset: 0x001DE160
		public override void OnEnter()
		{
			TransitionPoint transitionPoint = this.Gate.Value as TransitionPoint;
			if (!transitionPoint)
			{
				base.Finish();
				return;
			}
			HeroController instance = HeroController.instance;
			base.StartCoroutine(instance.EnterScene(transitionPoint, 0f, true, new Action(base.Finish), false));
		}

		// Token: 0x04005B69 RID: 23401
		[ObjectType(typeof(TransitionPoint))]
		public FsmObject Gate;
	}
}
