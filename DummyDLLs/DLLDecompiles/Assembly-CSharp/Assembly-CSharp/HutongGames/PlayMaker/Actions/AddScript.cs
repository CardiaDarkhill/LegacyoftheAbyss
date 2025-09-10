using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001058 RID: 4184
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Adds a Script to a Game Object. Use this to change the behaviour of objects on the fly. Optionally remove the Script on exiting the state.")]
	public class AddScript : FsmStateAction
	{
		// Token: 0x06007271 RID: 29297 RVA: 0x002326D8 File Offset: 0x002308D8
		public override void Reset()
		{
			this.gameObject = null;
			this.script = null;
		}

		// Token: 0x06007272 RID: 29298 RVA: 0x002326E8 File Offset: 0x002308E8
		public override void OnEnter()
		{
			this.DoAddComponent((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
			base.Finish();
		}

		// Token: 0x06007273 RID: 29299 RVA: 0x0023271B File Offset: 0x0023091B
		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.addedComponent != null)
			{
				Object.Destroy(this.addedComponent);
			}
		}

		// Token: 0x06007274 RID: 29300 RVA: 0x00232744 File Offset: 0x00230944
		private void DoAddComponent(GameObject go)
		{
			this.addedComponent = go.AddComponent(ReflectionUtils.GetGlobalType(this.script.Value));
			if (this.addedComponent == null)
			{
				base.LogError("Can't add script: " + this.script.Value);
			}
		}

		// Token: 0x0400725E RID: 29278
		[RequiredField]
		[Tooltip("The GameObject to add the script to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400725F RID: 29279
		[RequiredField]
		[Tooltip("Select any script in your project. The script will be added to the Game Object when the state is entered.")]
		[UIHint(UIHint.ScriptComponent)]
		public FsmString script;

		// Token: 0x04007260 RID: 29280
		[Tooltip("Remove the script from the GameObject when this State is exited.")]
		public FsmBool removeOnExit;

		// Token: 0x04007261 RID: 29281
		private Component addedComponent;
	}
}
