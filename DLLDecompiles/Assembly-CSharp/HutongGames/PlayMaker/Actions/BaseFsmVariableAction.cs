using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001079 RID: 4217
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	public abstract class BaseFsmVariableAction : FsmStateAction
	{
		// Token: 0x060072FD RID: 29437 RVA: 0x00235937 File Offset: 0x00233B37
		public override void Reset()
		{
			this.fsmNotFound = null;
			this.variableNotFound = null;
		}

		// Token: 0x060072FE RID: 29438 RVA: 0x00235948 File Offset: 0x00233B48
		protected bool UpdateCache(GameObject go, string fsmName)
		{
			if (go == null)
			{
				return false;
			}
			if (this.fsm == null || this.cachedGameObject != go || this.cachedFsmName != fsmName)
			{
				this.fsm = ActionHelpers.GetGameObjectFsm(go, fsmName);
				this.cachedGameObject = go;
				this.cachedFsmName = fsmName;
				if (this.fsm == null)
				{
					base.LogWarning("Could not find FSM: " + fsmName);
					base.Fsm.Event(this.fsmNotFound);
				}
			}
			return true;
		}

		// Token: 0x060072FF RID: 29439 RVA: 0x002359D6 File Offset: 0x00233BD6
		protected void DoVariableNotFound(string variableName)
		{
			base.LogWarning("Could not find variable: " + variableName);
			base.Fsm.Event(this.variableNotFound);
		}

		// Token: 0x04007300 RID: 29440
		[ActionSection("Events")]
		[Tooltip("The event to send if the FSM is not found.")]
		public FsmEvent fsmNotFound;

		// Token: 0x04007301 RID: 29441
		[Tooltip("The event to send if the Variable is not found.")]
		public FsmEvent variableNotFound;

		// Token: 0x04007302 RID: 29442
		private GameObject cachedGameObject;

		// Token: 0x04007303 RID: 29443
		private string cachedFsmName;

		// Token: 0x04007304 RID: 29444
		protected PlayMakerFSM fsm;
	}
}
