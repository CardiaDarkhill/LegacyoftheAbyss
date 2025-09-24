using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B63 RID: 2915
	public abstract class HashTableActions : CollectionsActions
	{
		// Token: 0x06005A92 RID: 23186 RVA: 0x001CA127 File Offset: 0x001C8327
		protected bool SetUpHashTableProxyPointer(GameObject aProxyGO, string nameReference)
		{
			if (aProxyGO == null)
			{
				return false;
			}
			this.proxy = base.GetHashTableProxyPointer(aProxyGO, nameReference, false);
			return this.proxy != null;
		}

		// Token: 0x06005A93 RID: 23187 RVA: 0x001CA14F File Offset: 0x001C834F
		protected bool SetUpHashTableProxyPointer(PlayMakerHashTableProxy aProxy, string nameReference)
		{
			if (aProxy == null)
			{
				return false;
			}
			this.proxy = base.GetHashTableProxyPointer(aProxy.gameObject, nameReference, false);
			return this.proxy != null;
		}

		// Token: 0x06005A94 RID: 23188 RVA: 0x001CA17C File Offset: 0x001C837C
		protected bool isProxyValid()
		{
			if (this.proxy == null)
			{
				Debug.LogError("HashTable proxy is null");
				return false;
			}
			if (this.proxy.hashTable == null)
			{
				Debug.LogError("HashTable undefined");
				return false;
			}
			return true;
		}

		// Token: 0x04005634 RID: 22068
		internal PlayMakerHashTableProxy proxy;
	}
}
