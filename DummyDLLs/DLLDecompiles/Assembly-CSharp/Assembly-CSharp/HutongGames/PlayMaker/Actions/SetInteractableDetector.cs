using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001366 RID: 4966
	public sealed class SetInteractableDetector : FSMUtility.GetComponentFsmStateAction<InteractableBase>
	{
		// Token: 0x17000C40 RID: 3136
		// (get) Token: 0x06008012 RID: 32786 RVA: 0x0025D54A File Offset: 0x0025B74A
		protected override bool LogMissingComponent
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06008013 RID: 32787 RVA: 0x0025D54D File Offset: 0x0025B74D
		public override void Reset()
		{
			base.Reset();
			this.detector = null;
			this.allowNullDetector = null;
			this.previousDetector = null;
		}

		// Token: 0x06008014 RID: 32788 RVA: 0x0025D56C File Offset: 0x0025B76C
		protected override void DoAction(InteractableBase component)
		{
			TriggerEnterEvent safe = this.detector.GetSafe<TriggerEnterEvent>();
			if (!this.allowNullDetector.Value && safe == null)
			{
				return;
			}
			if (!this.previousDetector.IsNone && component.EnterDetector != null && component.EnterDetector != safe)
			{
				this.previousDetector.Value = component.EnterDetector.gameObject;
			}
			component.EnterDetector = safe;
		}

		// Token: 0x04007F7F RID: 32639
		public FsmGameObject detector;

		// Token: 0x04007F80 RID: 32640
		public FsmBool allowNullDetector;

		// Token: 0x04007F81 RID: 32641
		[Tooltip("Optional storage of previous detector")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject previousDetector;

		// Token: 0x02001BFC RID: 7164
		public enum DetectorType
		{
			// Token: 0x04009FC3 RID: 40899
			Enter,
			// Token: 0x04009FC4 RID: 40900
			Exit
		}
	}
}
