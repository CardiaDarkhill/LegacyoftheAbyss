using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E17 RID: 3607
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Syncs the value of an Animator Bool parameter to the current state. Sets the parameter to true when entering the state and false when exiting. For example, you can setup an animator with one animation per state with transition conditions based on the Bool parameter, then sync animator states with this FSM's states using this action.")]
	public class SyncAnimatorBoolToState : ComponentAction<Animator>
	{
		// Token: 0x17000BE5 RID: 3045
		// (get) Token: 0x060067C9 RID: 26569 RVA: 0x0020B080 File Offset: 0x00209280
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060067CA RID: 26570 RVA: 0x0020B088 File Offset: 0x00209288
		public override void Awake()
		{
			base.BlocksFinish = false;
		}

		// Token: 0x060067CB RID: 26571 RVA: 0x0020B091 File Offset: 0x00209291
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.parameter = null;
		}

		// Token: 0x060067CC RID: 26572 RVA: 0x0020B0A8 File Offset: 0x002092A8
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.paramID = Animator.StringToHash(this.parameter.Value);
			if (this.animator.isActiveAndEnabled)
			{
				this.animator.SetBool(this.paramID, true);
			}
		}

		// Token: 0x060067CD RID: 26573 RVA: 0x0020B104 File Offset: 0x00209304
		public override void OnExit()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			if (this.animator.isActiveAndEnabled)
			{
				this.animator.SetBool(this.paramID, false);
			}
		}

		// Token: 0x04006704 RID: 26372
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with the Animator component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006705 RID: 26373
		[RequiredField]
		[UIHint(UIHint.AnimatorBool)]
		[Tooltip("The bool parameter to sync. Set to true when the state is entered and false when the state exits.")]
		public FsmString parameter;

		// Token: 0x04006706 RID: 26374
		private int paramID;
	}
}
