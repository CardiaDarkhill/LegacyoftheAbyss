using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E24 RID: 3620
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Loop through all items in an {{Array}} and run an FSM Template on each item.\\nNOTE: This is an advanced/experimental action, still in beta. Please report any issues you run into.")]
	public class ArrayForEach : RunFSMAction
	{
		// Token: 0x060067FB RID: 26619 RVA: 0x0020B844 File Offset: 0x00209A44
		public override void Reset()
		{
			this.array = null;
			this.fsmTemplateControl = new FsmTemplateControl(FsmTemplateControl.TargetType.FsmTemplate);
			this.runFsm = null;
		}

		// Token: 0x060067FC RID: 26620 RVA: 0x0020B860 File Offset: 0x00209A60
		public override void Awake()
		{
			if (this.array != null && this.fsmTemplateControl.fsmTemplate != null && Application.isPlaying)
			{
				this.runFsm = base.Fsm.CreateSubFsm(this.fsmTemplateControl);
			}
		}

		// Token: 0x060067FD RID: 26621 RVA: 0x0020B89B File Offset: 0x00209A9B
		public override void OnEnter()
		{
			if (this.array == null || this.runFsm == null)
			{
				base.Finish();
				return;
			}
			this.currentIndex = 0;
			this.StartFsm();
		}

		// Token: 0x060067FE RID: 26622 RVA: 0x0020B8C1 File Offset: 0x00209AC1
		public override void OnUpdate()
		{
			this.runFsm.Update();
			if (!this.runFsm.Finished)
			{
				return;
			}
			this.StartNextFsm();
		}

		// Token: 0x060067FF RID: 26623 RVA: 0x0020B8E2 File Offset: 0x00209AE2
		public override void OnFixedUpdate()
		{
			this.runFsm.FixedUpdate();
			if (!this.runFsm.Finished)
			{
				return;
			}
			this.StartNextFsm();
		}

		// Token: 0x06006800 RID: 26624 RVA: 0x0020B903 File Offset: 0x00209B03
		public override void OnLateUpdate()
		{
			this.runFsm.LateUpdate();
			if (!this.runFsm.Finished)
			{
				return;
			}
			this.StartNextFsm();
		}

		// Token: 0x06006801 RID: 26625 RVA: 0x0020B924 File Offset: 0x00209B24
		private void StartNextFsm()
		{
			this.currentIndex++;
			this.StartFsm();
		}

		// Token: 0x06006802 RID: 26626 RVA: 0x0020B93C File Offset: 0x00209B3C
		private void StartFsm()
		{
			while (this.currentIndex < this.array.Length)
			{
				this.DoStartFsm();
				if (!this.runFsm.Finished)
				{
					return;
				}
				this.currentIndex++;
			}
			base.Fsm.Event(this.finishEvent);
			base.Finish();
		}

		// Token: 0x06006803 RID: 26627 RVA: 0x0020B998 File Offset: 0x00209B98
		private void DoStartFsm()
		{
			this.storeItem.SetValue(this.array.Values[this.currentIndex]);
			this.fsmTemplateControl.UpdateValues();
			this.fsmTemplateControl.ApplyOverrides(this.runFsm);
			this.runFsm.OnEnable();
			if (!this.runFsm.Started)
			{
				this.runFsm.Start();
			}
		}

		// Token: 0x06006804 RID: 26628 RVA: 0x0020BA01 File Offset: 0x00209C01
		protected override void CheckIfFinished()
		{
		}

		// Token: 0x0400672C RID: 26412
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Array to iterate through.")]
		public FsmArray array;

		// Token: 0x0400672D RID: 26413
		[HideTypeFilter]
		[MatchElementType("array")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the item in a variable")]
		public FsmVar storeItem;

		// Token: 0x0400672E RID: 26414
		[ActionSection("Run FSM")]
		[Tooltip("The Template to run on each item in the array.<ul><li>The Template should expose a variable in the Inspector.</li><li>Use this Input variable to input the stored item.</li><li>The Template should use {{Finish FSM}} when finished.</li></ul>")]
		public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl(FsmTemplateControl.TargetType.FsmTemplate);

		// Token: 0x0400672F RID: 26415
		[Tooltip("Event to send after iterating through all items in the Array.")]
		public FsmEvent finishEvent;

		// Token: 0x04006730 RID: 26416
		private int currentIndex;
	}
}
