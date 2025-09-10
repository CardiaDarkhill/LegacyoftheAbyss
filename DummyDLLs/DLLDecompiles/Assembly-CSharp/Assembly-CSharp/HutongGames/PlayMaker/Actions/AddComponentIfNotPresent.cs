using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E9F RID: 3743
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Adds a Component to a Game Object if it doesn't already have the component. Use this to change the behaviour of objects on the fly. Optionally remove the Component on exiting the state.")]
	public class AddComponentIfNotPresent : FsmStateAction
	{
		// Token: 0x06006A2C RID: 27180 RVA: 0x00212FBF File Offset: 0x002111BF
		public override void Reset()
		{
			this.gameObject = null;
			this.component = null;
			this.storeComponent = null;
		}

		// Token: 0x06006A2D RID: 27181 RVA: 0x00212FD6 File Offset: 0x002111D6
		public override void OnEnter()
		{
			this.DoAddComponent();
			base.Finish();
		}

		// Token: 0x06006A2E RID: 27182 RVA: 0x00212FE4 File Offset: 0x002111E4
		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.addedComponent != null)
			{
				Object.Destroy(this.addedComponent);
			}
		}

		// Token: 0x06006A2F RID: 27183 RVA: 0x0021300C File Offset: 0x0021120C
		private void DoAddComponent()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Type globalType = ReflectionUtils.GetGlobalType(this.component.Value);
			this.addedComponent = (ownerDefaultTarget.GetComponent(globalType) ?? ownerDefaultTarget.AddComponent(globalType));
			this.storeComponent.Value = this.addedComponent;
			if (this.addedComponent == null)
			{
				base.LogError("Can't add component: " + this.component.Value);
			}
		}

		// Token: 0x04006983 RID: 27011
		[RequiredField]
		[Tooltip("The GameObject to add the Component to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006984 RID: 27012
		[RequiredField]
		[UIHint(UIHint.ScriptComponent)]
		[Title("Component Type")]
		[Tooltip("The type of Component to add to the Game Object.")]
		public FsmString component;

		// Token: 0x04006985 RID: 27013
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Component))]
		[Tooltip("Store the component in an Object variable. E.g., to use with Set Property.")]
		public FsmObject storeComponent;

		// Token: 0x04006986 RID: 27014
		[Tooltip("Remove the Component when this State is exited.")]
		public FsmBool removeOnExit;

		// Token: 0x04006987 RID: 27015
		private Component addedComponent;
	}
}
