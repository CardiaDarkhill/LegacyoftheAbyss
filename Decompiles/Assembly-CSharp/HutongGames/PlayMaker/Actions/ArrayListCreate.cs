using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B35 RID: 2869
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Adds a PlayMakerArrayList Component to a Game Object. Use this to create arrayList on the fly instead of during authoring.\n Optionally remove the ArrayList component on exiting the state.\n Simply point to existing if the reference exists already.")]
	public class ArrayListCreate : ArrayListActions
	{
		// Token: 0x060059C9 RID: 22985 RVA: 0x001C6E7C File Offset: 0x001C507C
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.alreadyExistsEvent = null;
		}

		// Token: 0x060059CA RID: 22986 RVA: 0x001C6E93 File Offset: 0x001C5093
		public override void OnEnter()
		{
			this.DoAddPlayMakerArrayList();
			base.Finish();
		}

		// Token: 0x060059CB RID: 22987 RVA: 0x001C6EA1 File Offset: 0x001C50A1
		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.addedComponent != null)
			{
				Object.Destroy(this.addedComponent);
			}
		}

		// Token: 0x060059CC RID: 22988 RVA: 0x001C6ECC File Offset: 0x001C50CC
		private void DoAddPlayMakerArrayList()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.GetArrayListProxyPointer(ownerDefaultTarget, this.reference.Value, true) != null)
			{
				base.Fsm.Event(this.alreadyExistsEvent);
				return;
			}
			this.addedComponent = ownerDefaultTarget.AddComponent<PlayMakerArrayListProxy>();
			if (this.addedComponent == null)
			{
				base.LogError("Can't add PlayMakerArrayListProxy");
				return;
			}
			this.addedComponent.referenceName = this.reference.Value;
		}

		// Token: 0x04005541 RID: 21825
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject to add the PlayMaker ArrayList Proxy component to")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005542 RID: 21826
		[Tooltip("Author defined Reference of the PlayMaker arrayList proxy component ( necessary if several component coexists on the same GameObject")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x04005543 RID: 21827
		[Tooltip("Remove the Component when this State is exited.")]
		public FsmBool removeOnExit;

		// Token: 0x04005544 RID: 21828
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the arrayList exists already")]
		public FsmEvent alreadyExistsEvent;

		// Token: 0x04005545 RID: 21829
		private PlayMakerArrayListProxy addedComponent;
	}
}
