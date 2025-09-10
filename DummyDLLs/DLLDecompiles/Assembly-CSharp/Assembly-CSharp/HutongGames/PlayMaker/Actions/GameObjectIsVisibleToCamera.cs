using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F55 RID: 3925
	[ActionCategory(ActionCategory.Logic)]
	[ActionTarget(typeof(GameObject), "gameObject", false)]
	[Tooltip("Tests if a Game Object is visible to a specific camera. Note, using bounds is a little more expensive than using the center point.")]
	public class GameObjectIsVisibleToCamera : ComponentAction<Renderer, Camera>
	{
		// Token: 0x17000BFC RID: 3068
		// (get) Token: 0x06006D11 RID: 27921 RVA: 0x0021F484 File Offset: 0x0021D684
		private Camera cameraComponent
		{
			get
			{
				return this.cachedComponent2;
			}
		}

		// Token: 0x06006D12 RID: 27922 RVA: 0x0021F48C File Offset: 0x0021D68C
		public override void Reset()
		{
			this.gameObject = null;
			this.camera = null;
			this.useBounds = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D13 RID: 27923 RVA: 0x0021F4BF File Offset: 0x0021D6BF
		public override void OnEnter()
		{
			this.DoIsVisible();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D14 RID: 27924 RVA: 0x0021F4D5 File Offset: 0x0021D6D5
		public override void OnUpdate()
		{
			this.DoIsVisible();
		}

		// Token: 0x06006D15 RID: 27925 RVA: 0x0021F4E0 File Offset: 0x0021D6E0
		private void DoIsVisible()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget, this.camera.Value))
			{
				bool flag = ActionHelpers.IsVisible(ownerDefaultTarget, this.cameraComponent, this.useBounds.Value);
				this.storeResult.Value = flag;
				base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
			}
		}

		// Token: 0x04006CD4 RID: 27860
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject to test.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006CD5 RID: 27861
		[Tooltip("The GameObject with the Camera component.")]
		public FsmGameObject camera;

		// Token: 0x04006CD6 RID: 27862
		[Tooltip("Use the bounds of the GameObject. Otherwise uses just the center point.")]
		public FsmBool useBounds;

		// Token: 0x04006CD7 RID: 27863
		[Tooltip("Event to send if the GameObject is visible.")]
		public FsmEvent trueEvent;

		// Token: 0x04006CD8 RID: 27864
		[Tooltip("Event to send if the GameObject is NOT visible.")]
		public FsmEvent falseEvent;

		// Token: 0x04006CD9 RID: 27865
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006CDA RID: 27866
		[Tooltip("Perform this action every frame.")]
		public bool everyFrame;
	}
}
