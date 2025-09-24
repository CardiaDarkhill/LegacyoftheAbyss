using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200117D RID: 4477
	[ActionCategory(ActionCategory.UnityObject)]
	[Tooltip("Gets a Component attached to a GameObject and stores it in an Object variable. NOTE: Set the Object variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
	public class GetComponent : FsmStateAction
	{
		// Token: 0x06007819 RID: 30745 RVA: 0x00246F7A File Offset: 0x0024517A
		public override void Reset()
		{
			this.gameObject = null;
			this.storeComponent = null;
			this.everyFrame = false;
		}

		// Token: 0x0600781A RID: 30746 RVA: 0x00246F91 File Offset: 0x00245191
		public override void OnEnter()
		{
			this.DoGetComponent();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600781B RID: 30747 RVA: 0x00246FA7 File Offset: 0x002451A7
		public override void OnUpdate()
		{
			this.DoGetComponent();
		}

		// Token: 0x0600781C RID: 30748 RVA: 0x00246FB0 File Offset: 0x002451B0
		private void DoGetComponent()
		{
			if (this.storeComponent == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.storeComponent.IsNone)
			{
				return;
			}
			this.storeComponent.Value = ownerDefaultTarget.GetComponent(this.storeComponent.ObjectType);
		}

		// Token: 0x04007899 RID: 30873
		[Tooltip("The GameObject that owns the component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400789A RID: 30874
		[UIHint(UIHint.Variable)]
		[RequiredField]
		[Tooltip("Store the component in an Object variable.\nNOTE: Set theObject variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
		public FsmObject storeComponent;

		// Token: 0x0400789B RID: 30875
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
