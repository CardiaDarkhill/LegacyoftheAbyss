using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EA3 RID: 3747
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Destroys a Component of an Object.")]
	public class DestroyComponent : FsmStateAction
	{
		// Token: 0x06006A3B RID: 27195 RVA: 0x00213545 File Offset: 0x00211745
		public override void Reset()
		{
			this.aComponent = null;
			this.gameObject = null;
			this.component = null;
		}

		// Token: 0x06006A3C RID: 27196 RVA: 0x0021355C File Offset: 0x0021175C
		public override void OnEnter()
		{
			this.DoDestroyComponent((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
			base.Finish();
		}

		// Token: 0x06006A3D RID: 27197 RVA: 0x00213590 File Offset: 0x00211790
		private void DoDestroyComponent(GameObject go)
		{
			this.aComponent = go.GetComponent(ReflectionUtils.GetGlobalType(this.component.Value));
			if (this.aComponent == null)
			{
				base.LogError("No such component: " + this.component.Value);
				return;
			}
			Object.Destroy(this.aComponent);
		}

		// Token: 0x04006999 RID: 27033
		[RequiredField]
		[Tooltip("The GameObject that owns the Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400699A RID: 27034
		[RequiredField]
		[UIHint(UIHint.ScriptComponent)]
		[Tooltip("The name of the Component to destroy.")]
		public FsmString component;

		// Token: 0x0400699B RID: 27035
		private Component aComponent;
	}
}
