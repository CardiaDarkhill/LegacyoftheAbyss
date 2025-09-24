using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200103C RID: 4156
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Allow scenes to be activated. Use this after {{LoadSceneAsynch}} if you did not set the scene to activate after loading")]
	public class AllowSceneActivation : FsmStateAction
	{
		// Token: 0x060071E9 RID: 29161 RVA: 0x00230435 File Offset: 0x0022E635
		public override void Reset()
		{
			this.aSynchOperationHashCode = null;
			this.allowSceneActivation = true;
			this.progress = null;
			this.isDone = null;
			this.doneEvent = null;
			this.failureEvent = null;
		}

		// Token: 0x060071EA RID: 29162 RVA: 0x00230466 File Offset: 0x0022E666
		public override void OnEnter()
		{
			this.DoAllowSceneActivation();
		}

		// Token: 0x060071EB RID: 29163 RVA: 0x00230470 File Offset: 0x0022E670
		public override void OnUpdate()
		{
			if (!this.progress.IsNone)
			{
				this.progress.Value = LoadSceneAsynch.aSyncOperationLUT[this.aSynchOperationHashCode.Value].progress;
			}
			if (!this.isDone.IsNone)
			{
				this.isDone.Value = LoadSceneAsynch.aSyncOperationLUT[this.aSynchOperationHashCode.Value].isDone;
			}
			if (LoadSceneAsynch.aSyncOperationLUT[this.aSynchOperationHashCode.Value].isDone)
			{
				LoadSceneAsynch.aSyncOperationLUT.Remove(this.aSynchOperationHashCode.Value);
				base.Fsm.Event(this.doneEvent);
				base.Finish();
				return;
			}
		}

		// Token: 0x060071EC RID: 29164 RVA: 0x0023052C File Offset: 0x0022E72C
		private void DoAllowSceneActivation()
		{
			if (this.aSynchOperationHashCode.IsNone || this.allowSceneActivation.IsNone || LoadSceneAsynch.aSyncOperationLUT == null || !LoadSceneAsynch.aSyncOperationLUT.ContainsKey(this.aSynchOperationHashCode.Value))
			{
				base.Fsm.Event(this.failureEvent);
				base.Finish();
				return;
			}
			LoadSceneAsynch.aSyncOperationLUT[this.aSynchOperationHashCode.Value].allowSceneActivation = this.allowSceneActivation.Value;
		}

		// Token: 0x040071A5 RID: 29093
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The name of the new scene. It cannot be empty, null, or same as existing scenes.")]
		public FsmInt aSynchOperationHashCode;

		// Token: 0x040071A6 RID: 29094
		[Tooltip("Allow the scene to be activated")]
		public FsmBool allowSceneActivation;

		// Token: 0x040071A7 RID: 29095
		[ActionSection("Result")]
		[Tooltip("The loading's progress.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat progress;

		// Token: 0x040071A8 RID: 29096
		[Tooltip("True when loading is done")]
		[UIHint(UIHint.Variable)]
		public FsmBool isDone;

		// Token: 0x040071A9 RID: 29097
		[Tooltip("Event sent when scene loading is done")]
		public FsmEvent doneEvent;

		// Token: 0x040071AA RID: 29098
		[Tooltip("Event sent when action could not be performed. Check Log for more information")]
		public FsmEvent failureEvent;
	}
}
