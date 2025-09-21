using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B62 RID: 2914
	[Tooltip("Collections base action - don't use!")]
	public abstract class CollectionsActions : FsmStateAction
	{
		// Token: 0x06005A8F RID: 23183 RVA: 0x001C9F68 File Offset: 0x001C8168
		protected PlayMakerHashTableProxy GetHashTableProxyPointer(GameObject aProxy, string nameReference, bool silent)
		{
			if (aProxy == null)
			{
				if (!silent)
				{
					Debug.LogError("Null Proxy");
				}
				return null;
			}
			PlayMakerHashTableProxy[] components = aProxy.GetComponents<PlayMakerHashTableProxy>();
			if (components.Length > 1)
			{
				if (nameReference == "" && !silent)
				{
					Debug.LogWarning("Several HashTable Proxies coexists on the same GameObject and no reference is given to find the expected HashTable");
				}
				foreach (PlayMakerHashTableProxy playMakerHashTableProxy in components)
				{
					if (playMakerHashTableProxy.referenceName == nameReference)
					{
						return playMakerHashTableProxy;
					}
				}
				if (nameReference != "")
				{
					if (!silent)
					{
						Debug.LogError("HashTable Proxy not found for reference <" + nameReference + ">");
					}
					return null;
				}
			}
			else if (components.Length != 0)
			{
				if (nameReference != "" && nameReference != components[0].referenceName)
				{
					if (!silent)
					{
						Debug.LogError("HashTable Proxy reference do not match");
					}
					return null;
				}
				return components[0];
			}
			if (!silent)
			{
				Debug.LogError("HashTable not found");
			}
			return null;
		}

		// Token: 0x06005A90 RID: 23184 RVA: 0x001CA044 File Offset: 0x001C8244
		protected PlayMakerArrayListProxy GetArrayListProxyPointer(GameObject aProxy, string nameReference, bool silent)
		{
			if (aProxy == null)
			{
				if (!silent)
				{
					Debug.LogError("Null Proxy");
				}
				return null;
			}
			PlayMakerArrayListProxy[] components = aProxy.GetComponents<PlayMakerArrayListProxy>();
			if (components.Length > 1)
			{
				if (nameReference == "" && !silent)
				{
					Debug.LogError("Several ArrayList Proxies coexists on the same GameObject and no reference is given to find the expected ArrayList");
				}
				foreach (PlayMakerArrayListProxy playMakerArrayListProxy in components)
				{
					if (playMakerArrayListProxy.referenceName == nameReference)
					{
						return playMakerArrayListProxy;
					}
				}
				if (nameReference != "")
				{
					if (!silent)
					{
						base.LogError("ArrayList Proxy not found for reference <" + nameReference + ">");
					}
					return null;
				}
			}
			else if (components.Length != 0)
			{
				if (nameReference != "" && nameReference != components[0].referenceName)
				{
					if (!silent)
					{
						Debug.LogError("ArrayList Proxy reference do not match");
					}
					return null;
				}
				return components[0];
			}
			if (!silent)
			{
				base.LogError("ArrayList proxy not found");
			}
			return null;
		}

		// Token: 0x02001B77 RID: 7031
		public enum FsmVariableEnum
		{
			// Token: 0x04009D30 RID: 40240
			FsmGameObject,
			// Token: 0x04009D31 RID: 40241
			FsmInt,
			// Token: 0x04009D32 RID: 40242
			FsmFloat,
			// Token: 0x04009D33 RID: 40243
			FsmString,
			// Token: 0x04009D34 RID: 40244
			FsmBool,
			// Token: 0x04009D35 RID: 40245
			FsmVector2,
			// Token: 0x04009D36 RID: 40246
			FsmVector3,
			// Token: 0x04009D37 RID: 40247
			FsmRect,
			// Token: 0x04009D38 RID: 40248
			FsmQuaternion,
			// Token: 0x04009D39 RID: 40249
			FsmColor,
			// Token: 0x04009D3A RID: 40250
			FsmMaterial,
			// Token: 0x04009D3B RID: 40251
			FsmTexture,
			// Token: 0x04009D3C RID: 40252
			FsmObject
		}
	}
}
