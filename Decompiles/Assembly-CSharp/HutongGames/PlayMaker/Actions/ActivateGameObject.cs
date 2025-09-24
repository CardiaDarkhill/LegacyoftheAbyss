using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E9C RID: 3740
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activates/deactivates a Game Object. Use this to hide/show areas, or enable/disable many Behaviours at once.")]
	public class ActivateGameObject : FsmStateAction
	{
		// Token: 0x06006A1C RID: 27164 RVA: 0x00212C43 File Offset: 0x00210E43
		public override void Reset()
		{
			this.gameObject = null;
			this.activate = true;
			this.recursive = false;
			this.resetOnExit = false;
			this.everyFrame = false;
		}

		// Token: 0x06006A1D RID: 27165 RVA: 0x00212C72 File Offset: 0x00210E72
		public override void OnEnter()
		{
			this.DoActivateGameObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A1E RID: 27166 RVA: 0x00212C88 File Offset: 0x00210E88
		public override void OnUpdate()
		{
			this.DoActivateGameObject();
		}

		// Token: 0x06006A1F RID: 27167 RVA: 0x00212C90 File Offset: 0x00210E90
		public override void OnExit()
		{
			if (this.activatedGameObject == null)
			{
				return;
			}
			if (this.resetOnExit)
			{
				if (this.recursive.Value)
				{
					this.SetActiveRecursively(this.activatedGameObject, !this.activate.Value);
					return;
				}
				this.activatedGameObject.SetActive(!this.activate.Value);
			}
		}

		// Token: 0x06006A20 RID: 27168 RVA: 0x00212CF8 File Offset: 0x00210EF8
		private void DoActivateGameObject()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.recursive.Value)
			{
				this.SetActiveRecursively(ownerDefaultTarget, this.activate.Value);
			}
			else
			{
				ownerDefaultTarget.SetActive(this.activate.Value);
			}
			this.activatedGameObject = ownerDefaultTarget;
		}

		// Token: 0x06006A21 RID: 27169 RVA: 0x00212D5C File Offset: 0x00210F5C
		public void SetActiveRecursively(GameObject go, bool state)
		{
			go.SetActive(state);
			foreach (object obj in go.transform)
			{
				Transform transform = (Transform)obj;
				this.SetActiveRecursively(transform.gameObject, state);
			}
		}

		// Token: 0x04006975 RID: 26997
		[RequiredField]
		[Tooltip("The GameObject to activate/deactivate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006976 RID: 26998
		[RequiredField]
		[Tooltip("Check to activate, uncheck to deactivate Game Object.")]
		public FsmBool activate;

		// Token: 0x04006977 RID: 26999
		[Tooltip("Recursively activate/deactivate all children.")]
		public FsmBool recursive;

		// Token: 0x04006978 RID: 27000
		[Tooltip("Reset the game objects when exiting this state. Useful if you want an object to be active only while this state is active.\nNote: Only applies to the last Game Object activated/deactivated (won't work if Game Object changes).")]
		public bool resetOnExit;

		// Token: 0x04006979 RID: 27001
		[Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
		public bool everyFrame;

		// Token: 0x0400697A RID: 27002
		private GameObject activatedGameObject;
	}
}
