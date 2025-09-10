using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F54 RID: 3924
	[ActionCategory(ActionCategory.Logic)]
	[ActionTarget(typeof(GameObject), "gameObject", false)]
	[Tooltip("Tests if a Game Object is visible to any camera.\nNOTE: In the editor this includes the Scene View camera!")]
	public class GameObjectIsVisible : ComponentAction<Renderer>
	{
		// Token: 0x06006D0C RID: 27916 RVA: 0x0021F3DB File Offset: 0x0021D5DB
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D0D RID: 27917 RVA: 0x0021F400 File Offset: 0x0021D600
		public override void OnEnter()
		{
			this.DoIsVisible();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D0E RID: 27918 RVA: 0x0021F416 File Offset: 0x0021D616
		public override void OnUpdate()
		{
			this.DoIsVisible();
		}

		// Token: 0x06006D0F RID: 27919 RVA: 0x0021F420 File Offset: 0x0021D620
		private void DoIsVisible()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				bool isVisible = base.renderer.isVisible;
				this.storeResult.Value = isVisible;
				base.Fsm.Event(isVisible ? this.trueEvent : this.falseEvent);
			}
		}

		// Token: 0x04006CCF RID: 27855
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject to test.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006CD0 RID: 27856
		[Tooltip("Event to send if the GameObject is visible.")]
		public FsmEvent trueEvent;

		// Token: 0x04006CD1 RID: 27857
		[Tooltip("Event to send if the GameObject is NOT visible.")]
		public FsmEvent falseEvent;

		// Token: 0x04006CD2 RID: 27858
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006CD3 RID: 27859
		[Tooltip("Perform this action every frame.")]
		public bool everyFrame;
	}
}
