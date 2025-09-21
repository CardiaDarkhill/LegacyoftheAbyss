using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E53 RID: 3667
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Checks the height clearance for a CharacterController, or, in other words, if a CharacterController can be set to a height without collisions. Often used while crouching to check if the controller has room to stand up.")]
	public class ControllerCheckHeight : ComponentAction<CharacterController>
	{
		// Token: 0x17000BE6 RID: 3046
		// (get) Token: 0x060068C9 RID: 26825 RVA: 0x0020E500 File Offset: 0x0020C700
		private CharacterController controller
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060068CA RID: 26826 RVA: 0x0020E508 File Offset: 0x0020C708
		public override void Reset()
		{
			this.gameObject = null;
			this.repeatInterval = new FsmInt
			{
				Value = 1
			};
			this.checkHeight = null;
			this.layerMask = null;
			this.didPass = null;
			this.clearEvent = null;
			this.blockedEvent = null;
		}

		// Token: 0x060068CB RID: 26827 RVA: 0x0020E546 File Offset: 0x0020C746
		public override void OnEnter()
		{
			if (!base.UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			this.DoCheck();
			if (this.repeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x060068CC RID: 26828 RVA: 0x0020E581 File Offset: 0x0020C781
		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoCheck();
			}
		}

		// Token: 0x060068CD RID: 26829 RVA: 0x0020E59F File Offset: 0x0020C79F
		private void DoCheck()
		{
			this.repeat = this.repeatInterval.Value;
			this.DoCapsuleOverlap();
			base.Fsm.Event(this.didPass.Value ? this.clearEvent : this.blockedEvent);
		}

		// Token: 0x060068CE RID: 26830 RVA: 0x0020E5E0 File Offset: 0x0020C7E0
		private void DoCapsuleOverlap()
		{
			float radius = this.controller.radius;
			Vector3 point = base.cachedTransform.TransformPoint(-(Vector3.up * (this.controller.height / 2f - radius)));
			Vector3 point2 = base.cachedTransform.TransformPoint(Vector3.up * (this.checkHeight.Value - this.controller.height * 0.5f - radius));
			int num = Physics.OverlapCapsuleNonAlloc(point, point2, radius, this.colliders, this.layerMask.Value);
			this.didPass.Value = (num == 0);
		}

		// Token: 0x040067FA RID: 26618
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067FB RID: 26619
		[UIHint(UIHint.LayerMask)]
		[Tooltip("Layers to check collisions against.")]
		public FsmInt layerMask;

		// Token: 0x040067FC RID: 26620
		[Tooltip("Height to check. The action will use a capsule of this height to check for collisions.")]
		public FsmFloat checkHeight;

		// Token: 0x040067FD RID: 26621
		[Tooltip("Set how often to check. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause collision checks can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		// Token: 0x040067FE RID: 26622
		[ActionSection("Output")]
		[Tooltip("Store if any collisions were found.")]
		public FsmBool didPass;

		// Token: 0x040067FF RID: 26623
		[Tooltip("Event to send if no collisions were found.")]
		public FsmEvent clearEvent;

		// Token: 0x04006800 RID: 26624
		[Tooltip("Event to send if collisions were found.")]
		public FsmEvent blockedEvent;

		// Token: 0x04006801 RID: 26625
		private int repeat;

		// Token: 0x04006802 RID: 26626
		private Collider[] colliders = new Collider[1];
	}
}
