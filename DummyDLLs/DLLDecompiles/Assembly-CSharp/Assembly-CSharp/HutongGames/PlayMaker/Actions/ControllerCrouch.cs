using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E54 RID: 3668
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Makes a CharacterController Crouch. Handles scaling the collider and transitions between standing and crouching.")]
	public class ControllerCrouch : ComponentAction<CharacterController>
	{
		// Token: 0x17000BE7 RID: 3047
		// (get) Token: 0x060068D0 RID: 26832 RVA: 0x0020E697 File Offset: 0x0020C897
		private CharacterController controller
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060068D1 RID: 26833 RVA: 0x0020E6A0 File Offset: 0x0020C8A0
		public override void Reset()
		{
			this.gameObject = null;
			this.isCrouching = new FsmBool
			{
				UseVariable = true
			};
			this.crouchHeight = new FsmFloat
			{
				Value = 0.5f
			};
			this.adjustChildren = new FsmBool
			{
				Value = true
			};
			this.transitionTime = new FsmFloat
			{
				Value = 0.2f
			};
			this.completeTransition = null;
			this.canStand = new FsmBool
			{
				Value = true
			};
			this.standToggle = null;
			this.standEvent = null;
			this.resetHeightOnExit = new FsmBool
			{
				Value = true
			};
		}

		// Token: 0x060068D2 RID: 26834 RVA: 0x0020E740 File Offset: 0x0020C940
		public override void OnEnter()
		{
			if (!base.UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			this.originalHeight = this.controller.height;
			this.crouchState = ControllerCrouch.CrouchState.stand;
			this.transitionTimeElapsed = 0f;
			this.childOffsets.Clear();
			foreach (object obj in base.cachedTransform)
			{
				Transform transform = (Transform)obj;
				this.childOffsets.Add(transform, transform.localPosition.y);
			}
		}

		// Token: 0x060068D3 RID: 26835 RVA: 0x0020E7F8 File Offset: 0x0020C9F8
		public override void OnUpdate()
		{
			if (!base.UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			switch (this.crouchState)
			{
			case ControllerCrouch.CrouchState.stand:
				if (this.isCrouching.Value)
				{
					this.crouchState = ControllerCrouch.CrouchState.standToCrouch;
					this.startTransitionHeight = this.controller.height;
					this.transitionTimeElapsed = 0f;
					return;
				}
				break;
			case ControllerCrouch.CrouchState.standToCrouch:
			{
				float height;
				if (this.transitionTimeElapsed < this.transitionTime.Value)
				{
					height = Mathf.Lerp(this.startTransitionHeight, this.crouchHeight.Value, this.transitionTimeElapsed / this.transitionTime.Value);
					this.transitionTimeElapsed += Time.deltaTime;
				}
				else
				{
					height = this.crouchHeight.Value;
					this.crouchState = ControllerCrouch.CrouchState.crouch;
				}
				this.SetHeight(height);
				if (!this.completeTransition.Value && !this.isCrouching.Value)
				{
					this.crouchState = ControllerCrouch.CrouchState.crouchToStand;
					this.startTransitionHeight = this.controller.height;
					this.transitionTimeElapsed = 0f;
					return;
				}
				break;
			}
			case ControllerCrouch.CrouchState.crouch:
				if (this.canStand.Value && (!this.isCrouching.Value || this.standToggle.Value))
				{
					this.crouchState = ControllerCrouch.CrouchState.crouchToStand;
					this.startTransitionHeight = this.controller.height;
					this.transitionTimeElapsed = 0f;
					return;
				}
				break;
			case ControllerCrouch.CrouchState.crouchToStand:
			{
				float height;
				if (this.transitionTimeElapsed < this.transitionTime.Value)
				{
					height = Mathf.Lerp(this.startTransitionHeight, this.originalHeight, this.transitionTimeElapsed / this.transitionTime.Value);
					this.transitionTimeElapsed += Time.deltaTime;
				}
				else
				{
					height = this.originalHeight;
					this.crouchState = ControllerCrouch.CrouchState.stand;
				}
				this.SetHeight(height);
				if (this.crouchState == ControllerCrouch.CrouchState.stand)
				{
					base.Fsm.Event(this.standEvent);
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x060068D4 RID: 26836 RVA: 0x0020E9F0 File Offset: 0x0020CBF0
		private void SetHeight(float newHeight)
		{
			float num = this.controller.height - newHeight;
			if (this.controller.isGrounded)
			{
				base.cachedTransform.Translate(0f, -num * 0.5f, 0f);
			}
			this.controller.height = newHeight;
			if (this.adjustChildren.Value)
			{
				float num2 = this.controller.height / this.originalHeight;
				foreach (KeyValuePair<Transform, float> keyValuePair in this.childOffsets)
				{
					Vector3 localPosition = keyValuePair.Key.localPosition;
					keyValuePair.Key.localPosition = new Vector3(localPosition.x, keyValuePair.Value * num2, localPosition.z);
				}
			}
		}

		// Token: 0x060068D5 RID: 26837 RVA: 0x0020EAD8 File Offset: 0x0020CCD8
		public override void OnExit()
		{
			if (this.resetHeightOnExit.Value)
			{
				this.SetHeight(this.originalHeight);
				foreach (KeyValuePair<Transform, float> keyValuePair in this.childOffsets)
				{
					Vector3 localPosition = keyValuePair.Key.localPosition;
					keyValuePair.Key.localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z);
				}
			}
			this.childOffsets.Clear();
		}

		// Token: 0x04006803 RID: 26627
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006804 RID: 26628
		[RequiredField]
		[Tooltip("Crouch while this to true. Normally set by an Input action like Get Key.\n\nNOTE: The controller might not be able to stand up when this is false if there's not enough headroom.")]
		public FsmBool isCrouching;

		// Token: 0x04006805 RID: 26629
		[RequiredField]
		[Tooltip("Height of capsule when crouching.")]
		public FsmFloat crouchHeight;

		// Token: 0x04006806 RID: 26630
		[Tooltip("Move children so their height scales with capsule. This is useful for weapon attach points etc.")]
		public FsmBool adjustChildren;

		// Token: 0x04006807 RID: 26631
		[RequiredField]
		[Tooltip("How long it takes to crouch/stand in seconds.")]
		public FsmFloat transitionTime;

		// Token: 0x04006808 RID: 26632
		[Tooltip("Always complete the full transition to crouching, even if the input is brief.")]
		public FsmBool completeTransition;

		// Token: 0x04006809 RID: 26633
		[Tooltip("Can the CharacterController stand if isCrouching is false (e.g. if the crouch button is released). Usually set by a some kind of raycast checking the headroom above the controller,but could also be set to false to prevent standing for other reasons, e.g., crouch because the ground is shaking.")]
		public FsmBool canStand;

		// Token: 0x0400680A RID: 26634
		[UIHint(UIHint.Variable)]
		[Tooltip("Try to stand if true. Useful if want to toggle crouch with a button.")]
		public FsmBool standToggle;

		// Token: 0x0400680B RID: 26635
		[Tooltip("Event to send when crouch button is released AND there is enough headroom.")]
		public FsmEvent standEvent;

		// Token: 0x0400680C RID: 26636
		[Tooltip("Reset the controller height if the State exits before crouch has finished. Also restores children to original offsets if Adjust Children was used.\n\nNOTE: You probably want to keep this checked most of the time.")]
		public FsmBool resetHeightOnExit;

		// Token: 0x0400680D RID: 26637
		private float originalHeight;

		// Token: 0x0400680E RID: 26638
		private float startTransitionHeight;

		// Token: 0x0400680F RID: 26639
		private float transitionTimeElapsed;

		// Token: 0x04006810 RID: 26640
		private Dictionary<Transform, float> childOffsets = new Dictionary<Transform, float>();

		// Token: 0x04006811 RID: 26641
		private ControllerCrouch.CrouchState crouchState;

		// Token: 0x02001BA2 RID: 7074
		private enum CrouchState
		{
			// Token: 0x04009DFD RID: 40445
			stand,
			// Token: 0x04009DFE RID: 40446
			standToCrouch,
			// Token: 0x04009DFF RID: 40447
			crouch,
			// Token: 0x04009E00 RID: 40448
			crouchToStand
		}
	}
}
