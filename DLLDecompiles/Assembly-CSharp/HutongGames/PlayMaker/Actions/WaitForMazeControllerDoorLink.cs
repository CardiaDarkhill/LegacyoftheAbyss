using System;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013A5 RID: 5029
	public class WaitForMazeControllerDoorLink : FsmStateAction
	{
		// Token: 0x060080FA RID: 33018 RVA: 0x0025FF3E File Offset: 0x0025E13E
		public override void Reset()
		{
			this.StoreCorrectDoor = null;
			this.DoorsLinkedEvent = null;
		}

		// Token: 0x060080FB RID: 33019 RVA: 0x0025FF50 File Offset: 0x0025E150
		public override void OnEnter()
		{
			this.mazeController = MazeController.NewestInstance;
			if (this.mazeController == null)
			{
				return;
			}
			if (this.mazeController.IsDoorLinkComplete)
			{
				this.OnDoorsLinked();
				return;
			}
			this.mazeController.DoorsLinked += this.OnDoorsLinked;
		}

		// Token: 0x060080FC RID: 33020 RVA: 0x0025FFA2 File Offset: 0x0025E1A2
		public override void OnExit()
		{
			if (this.mazeController)
			{
				this.mazeController.DoorsLinked -= this.OnDoorsLinked;
				this.mazeController = null;
			}
		}

		// Token: 0x060080FD RID: 33021 RVA: 0x0025FFD0 File Offset: 0x0025E1D0
		private void OnDoorsLinked()
		{
			TransitionPoint transitionPoint = this.mazeController.EnumerateCorrectDoors().FirstOrDefault<TransitionPoint>();
			this.StoreCorrectDoor.Value = ((transitionPoint != null) ? transitionPoint.gameObject : null);
			this.mazeController.DoorsLinked -= this.OnDoorsLinked;
			this.mazeController = null;
			base.Fsm.Event(this.DoorsLinkedEvent);
			base.Finish();
		}

		// Token: 0x0400803C RID: 32828
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreCorrectDoor;

		// Token: 0x0400803D RID: 32829
		public FsmEvent DoorsLinkedEvent;

		// Token: 0x0400803E RID: 32830
		private MazeController mazeController;
	}
}
