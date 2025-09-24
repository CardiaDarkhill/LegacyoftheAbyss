using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E9E RID: 3742
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Adds a Component to a Game Object. Use this to change the behaviour of objects on the fly. Optionally remove the Component on exiting the state.")]
	public class AddComponent : FsmStateAction
	{
		// Token: 0x06006A27 RID: 27175 RVA: 0x00212EEB File Offset: 0x002110EB
		public override void Reset()
		{
			this.gameObject = null;
			this.component = null;
			this.storeComponent = null;
		}

		// Token: 0x06006A28 RID: 27176 RVA: 0x00212F02 File Offset: 0x00211102
		public override void OnEnter()
		{
			this.DoAddComponent();
			base.Finish();
		}

		// Token: 0x06006A29 RID: 27177 RVA: 0x00212F10 File Offset: 0x00211110
		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.addedComponent != null)
			{
				Object.Destroy(this.addedComponent);
			}
		}

		// Token: 0x06006A2A RID: 27178 RVA: 0x00212F38 File Offset: 0x00211138
		private void DoAddComponent()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.addedComponent = ownerDefaultTarget.AddComponent(ReflectionUtils.GetGlobalType(this.component.Value));
			this.storeComponent.Value = this.addedComponent;
			if (this.addedComponent == null)
			{
				base.LogError("Can't add component: " + this.component.Value);
			}
		}

		// Token: 0x0400697E RID: 27006
		[RequiredField]
		[Tooltip("The GameObject to add the Component to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400697F RID: 27007
		[RequiredField]
		[UIHint(UIHint.ScriptComponent)]
		[Title("Component Type")]
		[Tooltip("The type of Component to add to the Game Object.")]
		public FsmString component;

		// Token: 0x04006980 RID: 27008
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Component))]
		[Tooltip("Store the component in an Object variable. E.g., to use with Set Property.")]
		public FsmObject storeComponent;

		// Token: 0x04006981 RID: 27009
		[Tooltip("Remove the Component when this State is exited.")]
		public FsmBool removeOnExit;

		// Token: 0x04006982 RID: 27010
		private Component addedComponent;
	}
}
