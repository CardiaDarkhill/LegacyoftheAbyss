using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B39 RID: 2873
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Destroys a PlayMakerHashTableProxy Component of a Game Object.")]
	public class DestroyHashTable : HashTableActions
	{
		// Token: 0x060059DA RID: 23002 RVA: 0x001C71DF File Offset: 0x001C53DF
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.successEvent = null;
			this.notFoundEvent = null;
		}

		// Token: 0x060059DB RID: 23003 RVA: 0x001C7200 File Offset: 0x001C5400
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.SetUpHashTableProxyPointer(ownerDefaultTarget, this.reference.Value))
			{
				this.DoDestroyHashTable(ownerDefaultTarget);
			}
			else
			{
				base.Fsm.Event(this.notFoundEvent);
			}
			base.Finish();
		}

		// Token: 0x060059DC RID: 23004 RVA: 0x001C7254 File Offset: 0x001C5454
		private void DoDestroyHashTable(GameObject go)
		{
			foreach (PlayMakerHashTableProxy playMakerHashTableProxy in this.proxy.GetComponents<PlayMakerHashTableProxy>())
			{
				if (playMakerHashTableProxy.referenceName == this.reference.Value)
				{
					Object.Destroy(playMakerHashTableProxy);
					base.Fsm.Event(this.successEvent);
					return;
				}
			}
			base.Fsm.Event(this.notFoundEvent);
		}

		// Token: 0x04005553 RID: 21843
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005554 RID: 21844
		[Tooltip("Author defined Reference of the PlayMaker HashTable proxy component ( necessary if several component coexists on the same GameObject")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x04005555 RID: 21845
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the HashTable proxy component is destroyed")]
		public FsmEvent successEvent;

		// Token: 0x04005556 RID: 21846
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the HashTable proxy component was not found")]
		public FsmEvent notFoundEvent;
	}
}
