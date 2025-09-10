using System;
using UnityEngine;

// Token: 0x0200036A RID: 874
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06001E0B RID: 7691 RVA: 0x0008ADA8 File Offset: 0x00088FA8
	public static T instance
	{
		get
		{
			T result;
			if (Singleton<T>.applicationIsQuitting)
			{
				string str = "[Singleton] Instance '";
				Type typeFromHandle = typeof(T);
				Debug.LogWarning(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + "' already destroyed on application quit. Won't create again - returning null.");
				result = default(T);
				return result;
			}
			object @lock = Singleton<T>._lock;
			lock (@lock)
			{
				if (Singleton<T>._instance == null)
				{
					Singleton<T>._instance = (T)((object)Object.FindObjectOfType(typeof(T)));
					if (Object.FindObjectsOfType(typeof(T)).Length > 1)
					{
						Debug.LogError("[Singleton] Something went really wrong  - there should never be more than one singleton! Reopening the scene might fix it.");
						return Singleton<T>._instance;
					}
					if (Singleton<T>._instance == null)
					{
						GameObject gameObject = new GameObject();
						Singleton<T>._instance = gameObject.AddComponent<T>();
						gameObject.name = "(singleton) " + typeof(T).ToString();
						Object.DontDestroyOnLoad(gameObject);
					}
				}
				result = Singleton<T>._instance;
			}
			return result;
		}
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x0008AEC0 File Offset: 0x000890C0
	public void Awake()
	{
		if (Singleton<T>._instance == null)
		{
			Singleton<T>._instance = (base.gameObject as T);
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		if (this != Singleton<T>._instance)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
	}

	// Token: 0x06001E0D RID: 7693 RVA: 0x0008AF1E File Offset: 0x0008911E
	public void OnDestroy()
	{
		if (Singleton<T>._instance == this)
		{
			Singleton<T>._instance = default(T);
		}
		Singleton<T>.applicationIsQuitting = true;
	}

	// Token: 0x04001D26 RID: 7462
	private static T _instance;

	// Token: 0x04001D27 RID: 7463
	private static object _lock = new object();

	// Token: 0x04001D28 RID: 7464
	private static bool applicationIsQuitting = false;
}
