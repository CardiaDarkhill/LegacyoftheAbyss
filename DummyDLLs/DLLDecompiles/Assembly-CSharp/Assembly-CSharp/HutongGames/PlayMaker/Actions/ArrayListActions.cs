using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B61 RID: 2913
	public abstract class ArrayListActions : CollectionsActions
	{
		// Token: 0x06005A8B RID: 23179 RVA: 0x001C9ED1 File Offset: 0x001C80D1
		protected bool SetUpArrayListProxyPointer(GameObject aProxyGO, string nameReference)
		{
			if (aProxyGO == null)
			{
				return false;
			}
			this.proxy = base.GetArrayListProxyPointer(aProxyGO, nameReference, false);
			return this.proxy != null;
		}

		// Token: 0x06005A8C RID: 23180 RVA: 0x001C9EF9 File Offset: 0x001C80F9
		protected bool SetUpArrayListProxyPointer(PlayMakerArrayListProxy aProxy, string nameReference)
		{
			if (aProxy == null)
			{
				return false;
			}
			this.proxy = base.GetArrayListProxyPointer(aProxy.gameObject, nameReference, false);
			return this.proxy != null;
		}

		// Token: 0x06005A8D RID: 23181 RVA: 0x001C9F26 File Offset: 0x001C8126
		public bool isProxyValid()
		{
			if (this.proxy == null)
			{
				base.LogError("ArrayList proxy is null");
				return false;
			}
			if (this.proxy.arrayList == null)
			{
				base.LogError("ArrayList undefined");
				return false;
			}
			return true;
		}

		// Token: 0x04005633 RID: 22067
		internal PlayMakerArrayListProxy proxy;
	}
}
