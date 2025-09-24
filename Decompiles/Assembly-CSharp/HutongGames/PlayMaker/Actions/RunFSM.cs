using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010A5 RID: 4261
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Creates an FSM at runtime from a saved {{Template}}. The FSM is only active while the state is active. This lets you nest FSMs inside states.\nThis is a very powerful action! It allows you to create a library of FSM Templates that can be re-used in your project. You can edit the template in one place and the changes are reflected everywhere.\nNOTE: You can also specify a template in the {{FSM Inspector}}.")]
	public class RunFSM : RunFSMAction
	{
		// Token: 0x060073B4 RID: 29620 RVA: 0x00237EE3 File Offset: 0x002360E3
		public override void Reset()
		{
			this.fsmTemplateControl = new FsmTemplateControl(FsmTemplateControl.TargetType.FsmTemplate);
			this.runFsm = null;
			this.everyFrame = false;
		}

		// Token: 0x060073B5 RID: 29621 RVA: 0x00237F00 File Offset: 0x00236100
		public override void Awake()
		{
			base.HandlesOnEvent = true;
			this.fsmTemplateControl.Init();
			if (this.fsmTemplateControl.fsmTemplate != null && Application.isPlaying)
			{
				this.runFsm = base.Fsm.CreateSubFsm(this.fsmTemplateControl);
			}
		}

		// Token: 0x060073B6 RID: 29622 RVA: 0x00237F50 File Offset: 0x00236150
		public override void OnEnter()
		{
			if (this.runFsm == null)
			{
				base.Finish();
				return;
			}
			this.fsmTemplateControl.UpdateValues();
			this.fsmTemplateControl.ApplyOverrides(this.runFsm);
			this.runFsm.OnEnable();
			Fsm runFsm = this.runFsm;
			runFsm.OnOutputEvent = (Action<FsmEvent>)Delegate.Combine(runFsm.OnOutputEvent, new Action<FsmEvent>(this.OnOutputEvent));
			if (!this.runFsm.Started)
			{
				this.runFsm.Start();
			}
			this.fsmTemplateControl.UpdateOutput(base.Fsm);
			this.CheckIfFinished();
		}

		// Token: 0x060073B7 RID: 29623 RVA: 0x00237FE9 File Offset: 0x002361E9
		public override void OnExit()
		{
			if (this.runFsm == null)
			{
				return;
			}
			Fsm runFsm = this.runFsm;
			runFsm.OnOutputEvent = (Action<FsmEvent>)Delegate.Remove(runFsm.OnOutputEvent, new Action<FsmEvent>(this.OnOutputEvent));
		}

		// Token: 0x060073B8 RID: 29624 RVA: 0x0023801C File Offset: 0x0023621C
		private void OnOutputEvent(FsmEvent fsmEvent)
		{
			this.fsmTemplateControl.UpdateOutput(base.Fsm);
			FsmEvent fsmEvent2 = this.fsmTemplateControl.MapEvent(fsmEvent);
			if (fsmEvent2 == null)
			{
				return;
			}
			base.Fsm.Event(fsmEvent2);
		}

		// Token: 0x060073B9 RID: 29625 RVA: 0x00238058 File Offset: 0x00236258
		public override void OnUpdate()
		{
			if (this.restart)
			{
				this.OnEnter();
				this.restart = false;
				return;
			}
			if (this.runFsm != null)
			{
				this.runFsm.Update();
				this.fsmTemplateControl.UpdateOutput(base.Fsm);
				this.CheckIfFinished();
				return;
			}
			base.Finish();
		}

		// Token: 0x060073BA RID: 29626 RVA: 0x002380AC File Offset: 0x002362AC
		public override void OnLateUpdate()
		{
			if (this.runFsm != null)
			{
				this.runFsm.LateUpdate();
				this.fsmTemplateControl.UpdateOutput(base.Fsm);
				this.CheckIfFinished();
				return;
			}
			base.Finish();
		}

		// Token: 0x060073BB RID: 29627 RVA: 0x002380E0 File Offset: 0x002362E0
		protected override void CheckIfFinished()
		{
			if (this.runFsm == null)
			{
				base.Finish();
				return;
			}
			if (this.runFsm.Finished)
			{
				if (!this.everyFrame)
				{
					base.Finish();
					base.Fsm.Event(this.finishEvent);
					return;
				}
				this.restart = true;
			}
		}

		// Token: 0x040073DE RID: 29662
		[Tooltip("The Template to use. You can drag and drop, use the Unity object browser, or the categorized popup browser to select a template.")]
		public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl(FsmTemplateControl.TargetType.FsmTemplate);

		// Token: 0x040073DF RID: 29663
		[Tooltip("Event to send when the FSM has finished (usually because it ran a {{Finish FSM}} action).")]
		public FsmEvent finishEvent;

		// Token: 0x040073E0 RID: 29664
		[ActionSection("")]
		[Tooltip("Repeat every frame. Waits for the sub Fsm to finish before calling it again.")]
		public bool everyFrame;

		// Token: 0x040073E1 RID: 29665
		private bool restart;
	}
}
