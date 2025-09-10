using System;
using System.Linq;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000942 RID: 2370
	public abstract class SingletonMonoBehavior<TComponent> : MonoBehaviour where TComponent : MonoBehaviour
	{
		// Token: 0x17000B9A RID: 2970
		// (get) Token: 0x06005444 RID: 21572 RVA: 0x0017FE24 File Offset: 0x0017E024
		public static TComponent Instance
		{
			get
			{
				object obj = SingletonMonoBehavior<TComponent>.lockObject;
				TComponent result;
				lock (obj)
				{
					if (SingletonMonoBehavior<TComponent>.hasInstance)
					{
						result = SingletonMonoBehavior<TComponent>.instance;
					}
					else
					{
						SingletonMonoBehavior<TComponent>.instance = SingletonMonoBehavior<TComponent>.FindFirstInstance();
						if (SingletonMonoBehavior<TComponent>.instance == null)
						{
							string str = "The instance of singleton component ";
							Type typeFromHandle = typeof(TComponent);
							throw new Exception(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + " was requested, but it doesn't appear to exist in the scene.");
						}
						SingletonMonoBehavior<TComponent>.hasInstance = true;
						SingletonMonoBehavior<TComponent>.instanceId = SingletonMonoBehavior<TComponent>.instance.GetInstanceID();
						result = SingletonMonoBehavior<TComponent>.instance;
					}
				}
				return result;
			}
		}

		// Token: 0x17000B9B RID: 2971
		// (get) Token: 0x06005445 RID: 21573 RVA: 0x0017FED4 File Offset: 0x0017E0D4
		protected bool EnforceSingleton
		{
			get
			{
				if (base.GetInstanceID() == SingletonMonoBehavior<TComponent>.Instance.GetInstanceID())
				{
					return false;
				}
				if (Application.isPlaying)
				{
					base.enabled = false;
				}
				return true;
			}
		}

		// Token: 0x17000B9C RID: 2972
		// (get) Token: 0x06005446 RID: 21574 RVA: 0x0017FF00 File Offset: 0x0017E100
		protected bool IsTheSingleton
		{
			get
			{
				object obj = SingletonMonoBehavior<TComponent>.lockObject;
				bool result;
				lock (obj)
				{
					result = (base.GetInstanceID() == SingletonMonoBehavior<TComponent>.instanceId);
				}
				return result;
			}
		}

		// Token: 0x17000B9D RID: 2973
		// (get) Token: 0x06005447 RID: 21575 RVA: 0x0017FF48 File Offset: 0x0017E148
		protected bool IsNotTheSingleton
		{
			get
			{
				object obj = SingletonMonoBehavior<TComponent>.lockObject;
				bool result;
				lock (obj)
				{
					result = (base.GetInstanceID() != SingletonMonoBehavior<TComponent>.instanceId);
				}
				return result;
			}
		}

		// Token: 0x06005448 RID: 21576 RVA: 0x0017FF94 File Offset: 0x0017E194
		private static TComponent[] FindInstances()
		{
			TComponent[] array = Object.FindObjectsByType<TComponent>(FindObjectsSortMode.None);
			Array.Sort<TComponent>(array, (TComponent a, TComponent b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
			return array;
		}

		// Token: 0x06005449 RID: 21577 RVA: 0x0017FFC4 File Offset: 0x0017E1C4
		private static TComponent FindFirstInstance()
		{
			TComponent[] array = SingletonMonoBehavior<TComponent>.FindInstances();
			if (array.Length == 0)
			{
				return default(TComponent);
			}
			return array[0];
		}

		// Token: 0x0600544A RID: 21578 RVA: 0x0017FFEC File Offset: 0x0017E1EC
		protected virtual void Awake()
		{
			if (Application.isPlaying && SingletonMonoBehavior<TComponent>.Instance)
			{
				if (base.GetInstanceID() != SingletonMonoBehavior<TComponent>.instanceId)
				{
					base.enabled = false;
				}
				foreach (TComponent tcomponent in from o in SingletonMonoBehavior<TComponent>.FindInstances()
				where o.GetInstanceID() != SingletonMonoBehavior<TComponent>.instanceId
				select o)
				{
					tcomponent.enabled = false;
				}
			}
		}

		// Token: 0x0600544B RID: 21579 RVA: 0x0018008C File Offset: 0x0017E28C
		protected virtual void OnDestroy()
		{
			object obj = SingletonMonoBehavior<TComponent>.lockObject;
			lock (obj)
			{
				if (base.GetInstanceID() == SingletonMonoBehavior<TComponent>.instanceId)
				{
					SingletonMonoBehavior<TComponent>.hasInstance = false;
				}
			}
		}

		// Token: 0x0400539F RID: 21407
		private static TComponent instance;

		// Token: 0x040053A0 RID: 21408
		private static bool hasInstance;

		// Token: 0x040053A1 RID: 21409
		private static int instanceId;

		// Token: 0x040053A2 RID: 21410
		private static readonly object lockObject = new object();
	}
}
