using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200105C RID: 4188
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Enables/Disables a Behaviour on a GameObject. Optionally reset the Behaviour on exiting the state. E.g., if you want the Behaviour to be active only while this state is active.")]
	public class EnableBehaviour : FsmStateAction
	{
		// Token: 0x06007288 RID: 29320 RVA: 0x002332B5 File Offset: 0x002314B5
		public override void Reset()
		{
			this.gameObject = null;
			this.behaviour = null;
			this.component = null;
			this.enable = true;
			this.resetOnExit = true;
		}

		// Token: 0x06007289 RID: 29321 RVA: 0x002332E4 File Offset: 0x002314E4
		public override void OnEnter()
		{
			this.DoEnableBehaviour(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x0600728A RID: 29322 RVA: 0x00233304 File Offset: 0x00231504
		private void DoEnableBehaviour(GameObject go)
		{
			bool flag = this.component != null;
			if (!flag && go == null)
			{
				return;
			}
			if (flag)
			{
				this.componentTarget = (this.component as Behaviour);
			}
			else
			{
				this.componentTarget = (go.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as Behaviour);
			}
			if (this.componentTarget == null)
			{
				base.LogWarning(" " + go.name + " missing behaviour: " + this.behaviour.Value);
				return;
			}
			this.componentTarget.enabled = this.enable.Value;
		}

		// Token: 0x0600728B RID: 29323 RVA: 0x002333AD File Offset: 0x002315AD
		public override void OnExit()
		{
			if (this.componentTarget == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.componentTarget.enabled = !this.enable.Value;
			}
		}

		// Token: 0x0600728C RID: 29324 RVA: 0x002333E4 File Offset: 0x002315E4
		public override string ErrorCheck()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null || this.component != null || this.behaviour.IsNone || string.IsNullOrEmpty(this.behaviour.Value))
			{
				return null;
			}
			if (!(ownerDefaultTarget.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as Behaviour != null))
			{
				return "Behaviour missing";
			}
			return null;
		}

		// Token: 0x04007283 RID: 29315
		[RequiredField]
		[Tooltip("The GameObject that owns the Behaviour.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007284 RID: 29316
		[UIHint(UIHint.Behaviour)]
		[Tooltip("The name of the Behaviour to enable/disable.")]
		public FsmString behaviour;

		// Token: 0x04007285 RID: 29317
		[Tooltip("Optionally drag a component directly into this field (behavior name will be ignored).")]
		public Component component;

		// Token: 0x04007286 RID: 29318
		[RequiredField]
		[Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enable;

		// Token: 0x04007287 RID: 29319
		[Tooltip("Reset the enabled state of the Behaviour when leaving this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007288 RID: 29320
		private Behaviour componentTarget;
	}
}
