using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x02000056 RID: 86
[Serializable]
public class AddressableReferenceGameObject<T> : AssetReferenceGameObject, IDisposable where T : MonoBehaviour
{
	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000238 RID: 568 RVA: 0x0000DCA4 File Offset: 0x0000BEA4
	public T Component
	{
		get
		{
			if (!this.hasCachedComponent && this.instantiateOperationHandle.IsValid() && this.instantiateOperationHandle.Status == AsyncOperationStatus.Succeeded)
			{
				this.component = this.instantiateOperationHandle.Result.GetComponent<T>();
				this.hasCachedComponent = this.component;
			}
			return this.component;
		}
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000239 RID: 569 RVA: 0x0000DD06 File Offset: 0x0000BF06
	// (set) Token: 0x0600023A RID: 570 RVA: 0x0000DD0E File Offset: 0x0000BF0E
	public bool InstantiateSuccess { get; private set; }

	// Token: 0x0600023B RID: 571 RVA: 0x0000DD17 File Offset: 0x0000BF17
	public AddressableReferenceGameObject(string guid) : base(guid)
	{
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000DD20 File Offset: 0x0000BF20
	public override AsyncOperationHandle<GameObject> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
	{
		if (this.hasInstantiated)
		{
			return this.instantiateOperationHandle;
		}
		this.hasInstantiated = true;
		this.instantiateOperationHandle = base.InstantiateAsync(parent, instantiateInWorldSpace);
		this.instantiateOperationHandle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				handle.Result.AddInstanceHelper(handle);
				this.component = handle.Result.GetComponent<T>();
				this.hasCachedComponent = this.component;
				this.InstantiateSuccess = true;
				return;
			}
			Addressables.Release<GameObject>(handle);
		};
		return this.instantiateOperationHandle;
	}

	// Token: 0x0600023D RID: 573 RVA: 0x0000DD70 File Offset: 0x0000BF70
	public override AsyncOperationHandle<GameObject> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
	{
		if (this.hasInstantiated)
		{
			return this.instantiateOperationHandle;
		}
		this.hasInstantiated = true;
		this.instantiateOperationHandle = base.InstantiateAsync(position, rotation, parent);
		this.instantiateOperationHandle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				handle.Result.AddInstanceHelper(handle);
				this.component = handle.Result.GetComponent<T>();
				this.hasCachedComponent = this.component;
				this.InstantiateSuccess = true;
				return;
			}
			Addressables.Release<GameObject>(handle);
		};
		return this.instantiateOperationHandle;
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000DDBF File Offset: 0x0000BFBF
	public override AsyncOperationHandle<GameObject> LoadAssetAsync()
	{
		if (this.hasLoaded)
		{
			return this.loadOperationHandle;
		}
		this.hasLoaded = true;
		this.loadOperationHandle = base.LoadAssetAsync();
		return this.loadOperationHandle;
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000DDEC File Offset: 0x0000BFEC
	public AsyncOperationHandle<GameObject> InstantiateAsyncCustom(Transform parent, Action<bool> callback = null)
	{
		if (this.hasInstantiated)
		{
			this.instantiateOperationHandle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
			{
				bool obj = handle.Status == AsyncOperationStatus.Succeeded;
				Action<bool> callback2 = callback;
				if (callback2 == null)
				{
					return;
				}
				callback2(obj);
			};
			return this.instantiateOperationHandle;
		}
		this.instantiateOperationHandle = this.InstantiateAsync(parent, false);
		int orderHandle;
		AsyncLoadOrderingManager.OnStartedLoad(this.instantiateOperationHandle, out orderHandle);
		this.instantiateOperationHandle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
		{
			AsyncLoadOrderingManager.OnCompletedLoad(handle, orderHandle);
			bool flag = handle.Status == AsyncOperationStatus.Succeeded;
			if (flag)
			{
				this.component = handle.Result.GetComponent<T>();
				this.hasCachedComponent = this.component;
				this.InstantiateSuccess = true;
			}
			else
			{
				Addressables.ReleaseInstance(this.instantiateOperationHandle);
			}
			Action<bool> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(flag);
		};
		return this.instantiateOperationHandle;
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0000DE74 File Offset: 0x0000C074
	private void ReleaseUnmanagedResources()
	{
		if (this.isDisposed)
		{
			return;
		}
		this.isDisposed = true;
		this.component = default(T);
		if (this.loadOperationHandle.IsValid())
		{
			Addressables.Release<GameObject>(this.loadOperationHandle);
		}
		if (this.instantiateOperationHandle.IsValid())
		{
			Addressables.ReleaseInstance(this.instantiateOperationHandle);
		}
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0000DECE File Offset: 0x0000C0CE
	public void Dispose()
	{
		this.ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000DEDC File Offset: 0x0000C0DC
	~AddressableReferenceGameObject()
	{
		this.ReleaseUnmanagedResources();
	}

	// Token: 0x040001E9 RID: 489
	private readonly string address;

	// Token: 0x040001EA RID: 490
	private T component;

	// Token: 0x040001EC RID: 492
	private AsyncOperationHandle<GameObject> loadOperationHandle;

	// Token: 0x040001ED RID: 493
	private AsyncOperationHandle<GameObject> instantiateOperationHandle;

	// Token: 0x040001EE RID: 494
	private bool hasLoaded;

	// Token: 0x040001EF RID: 495
	private bool hasInstantiated;

	// Token: 0x040001F0 RID: 496
	private bool hasCachedComponent;

	// Token: 0x040001F1 RID: 497
	private bool isDisposed;
}
