using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B38 RID: 2872
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Adds a PlayMakerHashTableProxy Component to a Game Object. Use this to create arrayList on the fly instead of during authoring.\n Optionally remove the HashTable component on exiting the state.\n Simply point to existing if the reference exists already.")]
	public class HashTableCreate : HashTableActions
	{
		// Token: 0x060059D5 RID: 22997 RVA: 0x001C7102 File Offset: 0x001C5302
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.alreadyExistsEvent = null;
		}

		// Token: 0x060059D6 RID: 22998 RVA: 0x001C7119 File Offset: 0x001C5319
		public override void OnEnter()
		{
			this.DoAddPlayMakerHashTable();
			base.Finish();
		}

		// Token: 0x060059D7 RID: 22999 RVA: 0x001C7127 File Offset: 0x001C5327
		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.addedComponent != null)
			{
				Object.Destroy(this.addedComponent);
			}
		}

		// Token: 0x060059D8 RID: 23000 RVA: 0x001C7150 File Offset: 0x001C5350
		private void DoAddPlayMakerHashTable()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.GetHashTableProxyPointer(ownerDefaultTarget, this.reference.Value, true) != null)
			{
				base.Fsm.Event(this.alreadyExistsEvent);
				return;
			}
			this.addedComponent = ownerDefaultTarget.AddComponent<PlayMakerHashTableProxy>();
			if (this.addedComponent == null)
			{
				Debug.LogError("Can't add PlayMakerHashTableProxy");
				return;
			}
			this.addedComponent.referenceName = this.reference.Value;
		}

		// Token: 0x0400554E RID: 21838
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The Game Object to add the Hashtable Component to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400554F RID: 21839
		[Tooltip("Author defined Reference of the PlayMaker arrayList proxy component ( necessary if several component coexists on the same GameObject")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x04005550 RID: 21840
		[Tooltip("Remove the Component when this State is exited.")]
		public FsmBool removeOnExit;

		// Token: 0x04005551 RID: 21841
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the hashtable exists already")]
		public FsmEvent alreadyExistsEvent;

		// Token: 0x04005552 RID: 21842
		private PlayMakerHashTableProxy addedComponent;
	}
}
