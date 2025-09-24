using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE5 RID: 3301
	[ActionCategory("Controls")]
	public class QueueJump : FsmStateAction
	{
		// Token: 0x06006226 RID: 25126 RVA: 0x001F0822 File Offset: 0x001EEA22
		public override void Reset()
		{
			this.queueBool = null;
		}

		// Token: 0x06006227 RID: 25127 RVA: 0x001F082B File Offset: 0x001EEA2B
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.queueBool.Value = false;
			this.CheckInput();
		}

		// Token: 0x06006228 RID: 25128 RVA: 0x001F085B File Offset: 0x001EEA5B
		public override void OnUpdate()
		{
			this.CheckInput();
		}

		// Token: 0x06006229 RID: 25129 RVA: 0x001F0864 File Offset: 0x001EEA64
		private void CheckInput()
		{
			if (!this.gm.isPaused)
			{
				if (this.inputHandler.inputActions.Jump.WasPressed)
				{
					this.queueBool.Value = true;
				}
				if (!this.inputHandler.inputActions.Jump.IsPressed)
				{
					this.queueBool.Value = false;
				}
			}
		}

		// Token: 0x04006043 RID: 24643
		[UIHint(UIHint.Variable)]
		public FsmBool queueBool;

		// Token: 0x04006044 RID: 24644
		private GameManager gm;

		// Token: 0x04006045 RID: 24645
		private InputHandler inputHandler;
	}
}
