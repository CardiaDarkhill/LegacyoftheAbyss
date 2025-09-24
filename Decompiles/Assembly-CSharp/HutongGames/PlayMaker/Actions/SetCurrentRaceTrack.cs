using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001272 RID: 4722
	public class SetCurrentRaceTrack : FsmStateAction
	{
		// Token: 0x06007C72 RID: 31858 RVA: 0x002531F3 File Offset: 0x002513F3
		public override void Reset()
		{
			this.Runner = null;
			this.Track = null;
			this.GetReward = null;
		}

		// Token: 0x06007C73 RID: 31859 RVA: 0x0025320C File Offset: 0x0025140C
		public override void OnEnter()
		{
			SplineRunner component = this.Runner.GetSafe(this).GetComponent<SplineRunner>();
			SprintRaceController sprintRaceController;
			if (this.Track.Value)
			{
				sprintRaceController = this.Track.Value.GetComponent<SprintRaceController>();
				sprintRaceController.transform.parent.gameObject.SetActive(true);
			}
			else
			{
				sprintRaceController = null;
			}
			component.SetRaceController(sprintRaceController);
			this.GetReward.Value = ((sprintRaceController != null) ? sprintRaceController.Reward : null);
			base.Finish();
		}

		// Token: 0x04007C77 RID: 31863
		[CheckForComponent(typeof(SplineRunner))]
		public FsmOwnerDefault Runner;

		// Token: 0x04007C78 RID: 31864
		[CheckForComponent(typeof(SprintRaceController))]
		public FsmGameObject Track;

		// Token: 0x04007C79 RID: 31865
		[Space]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(SavedItem))]
		public FsmObject GetReward;
	}
}
