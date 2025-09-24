using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200107D RID: 4221
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Enables/Disables an FSM component on a GameObject.")]
	public class EnableFSM : FsmStateAction
	{
		// Token: 0x0600730F RID: 29455 RVA: 0x00235E66 File Offset: 0x00234066
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.enable = true;
			this.resetOnExit = true;
		}

		// Token: 0x06007310 RID: 29456 RVA: 0x00235E97 File Offset: 0x00234097
		public override void OnEnter()
		{
			this.DoEnableFSM();
			base.Finish();
		}

		// Token: 0x06007311 RID: 29457 RVA: 0x00235EA8 File Offset: 0x002340A8
		private void DoEnableFSM()
		{
			GameObject gameObject = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value;
			if (gameObject == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.fsmName.Value))
			{
				foreach (PlayMakerFSM playMakerFSM in gameObject.GetComponents<PlayMakerFSM>())
				{
					if (playMakerFSM.FsmName == this.fsmName.Value)
					{
						this.fsmComponent = playMakerFSM;
						break;
					}
				}
			}
			else
			{
				this.fsmComponent = gameObject.GetComponent<PlayMakerFSM>();
			}
			if (this.fsmComponent == null)
			{
				base.LogWarning("Missing FsmComponent!");
				return;
			}
			this.fsmComponent.enabled = this.enable.Value;
		}

		// Token: 0x06007312 RID: 29458 RVA: 0x00235F6F File Offset: 0x0023416F
		public override void OnExit()
		{
			if (this.fsmComponent == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.fsmComponent.enabled = !this.enable.Value;
			}
		}

		// Token: 0x04007315 RID: 29461
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007316 RID: 29462
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on GameObject. Useful if you have more than one FSM on a GameObject. Leave blank to use the first FSM on the game object.")]
		public FsmString fsmName;

		// Token: 0x04007317 RID: 29463
		[Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enable;

		// Token: 0x04007318 RID: 29464
		[Tooltip("Reset the initial enabled state when exiting the state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007319 RID: 29465
		private PlayMakerFSM fsmComponent;
	}
}
