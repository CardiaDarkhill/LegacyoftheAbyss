using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001044 RID: 4164
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene isValid flag. A scene may be invalid if, for example, you tried to open a scene that does not exist. In this case, the scene returned from EditorSceneManager.OpenScene would return False for IsValid. ")]
	public class GetSceneIsValid : GetSceneActionBase
	{
		// Token: 0x0600720D RID: 29197 RVA: 0x00230AED File Offset: 0x0022ECED
		public override void Reset()
		{
			base.Reset();
			this.isValid = null;
		}

		// Token: 0x0600720E RID: 29198 RVA: 0x00230AFC File Offset: 0x0022ECFC
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoGetSceneIsValid();
			base.Finish();
		}

		// Token: 0x0600720F RID: 29199 RVA: 0x00230B10 File Offset: 0x0022ED10
		private void DoGetSceneIsValid()
		{
			if (!this._sceneFound)
			{
				return;
			}
			if (!this.isValid.IsNone)
			{
				this.isValid.Value = this._scene.IsValid();
			}
			if (this._scene.IsValid())
			{
				base.Fsm.Event(this.isValidEvent);
			}
			else
			{
				base.Fsm.Event(this.isNotValidEvent);
			}
			base.Fsm.Event(this.sceneFoundEvent);
		}

		// Token: 0x040071C9 RID: 29129
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("true if the scene is loaded.")]
		public FsmBool isValid;

		// Token: 0x040071CA RID: 29130
		[Tooltip("Event sent if the scene is valid.")]
		public FsmEvent isValidEvent;

		// Token: 0x040071CB RID: 29131
		[Tooltip("Event sent if the scene is not valid.")]
		public FsmEvent isNotValidEvent;
	}
}
