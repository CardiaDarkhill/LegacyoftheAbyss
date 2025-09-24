using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9D RID: 3229
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForDashReleased : FsmStateAction
	{
		// Token: 0x060060E6 RID: 24806 RVA: 0x001EB3C5 File Offset: 0x001E95C5
		public override void Reset()
		{
			this.storeDashWasReleased = null;
		}

		// Token: 0x060060E7 RID: 24807 RVA: 0x001EB3CE File Offset: 0x001E95CE
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.storeDashWasReleased.Value = false;
			this.timer = 0f;
		}

		// Token: 0x060060E8 RID: 24808 RVA: 0x001EB403 File Offset: 0x001E9603
		public override void OnUpdate()
		{
			if (this.timer >= this.timeBeforeActive)
			{
				this.CheckInput();
				return;
			}
			this.timer += Time.deltaTime;
		}

		// Token: 0x060060E9 RID: 24809 RVA: 0x001EB42C File Offset: 0x001E962C
		private void CheckInput()
		{
			if (!this.gm.isPaused && !this.inputHandler.inputActions.Dash.IsPressed)
			{
				this.storeDashWasReleased.Value = true;
			}
		}

		// Token: 0x04005E8A RID: 24202
		public FsmBool storeDashWasReleased;

		// Token: 0x04005E8B RID: 24203
		public float timeBeforeActive;

		// Token: 0x04005E8C RID: 24204
		private float timer;

		// Token: 0x04005E8D RID: 24205
		private GameManager gm;

		// Token: 0x04005E8E RID: 24206
		private InputHandler inputHandler;
	}
}
