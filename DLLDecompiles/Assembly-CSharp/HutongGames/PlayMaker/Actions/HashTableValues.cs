using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B4F RID: 2895
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Store all the values of a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy) into a PlayMaker arrayList Proxy component (PlayMakerArrayListProxy).")]
	public class HashTableValues : HashTableActions
	{
		// Token: 0x06005A31 RID: 23089 RVA: 0x001C8481 File Offset: 0x001C6681
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.arrayListGameObject = null;
			this.arrayListReference = null;
		}

		// Token: 0x06005A32 RID: 23090 RVA: 0x001C849F File Offset: 0x001C669F
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doHashTableValues();
			}
			base.Finish();
		}

		// Token: 0x06005A33 RID: 23091 RVA: 0x001C84D4 File Offset: 0x001C66D4
		public void doHashTableValues()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.arrayListGameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			PlayMakerArrayListProxy arrayListProxyPointer = base.GetArrayListProxyPointer(ownerDefaultTarget, this.arrayListReference.Value, false);
			if (arrayListProxyPointer != null)
			{
				arrayListProxyPointer.arrayList.AddRange(this.proxy.hashTable.Values);
			}
		}

		// Token: 0x040055BE RID: 21950
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055BF RID: 21951
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055C0 RID: 21952
		[ActionSection("Result")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component that will store the values")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault arrayListGameObject;

		// Token: 0x040055C1 RID: 21953
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component that will store the values ( necessary if several component coexists on the same GameObject")]
		public FsmString arrayListReference;
	}
}
