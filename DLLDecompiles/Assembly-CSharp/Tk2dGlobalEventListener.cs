using System;
using UnityEngine;

// Token: 0x02000792 RID: 1938
public class Tk2dGlobalEventListener : Tk2dGlobalEvents.IListener
{
	// Token: 0x06004494 RID: 17556 RVA: 0x0012C5B0 File Offset: 0x0012A7B0
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		Tk2dGlobalEvents.AddListener(new Tk2dGlobalEventListener());
	}

	// Token: 0x06004495 RID: 17557 RVA: 0x0012C5BC File Offset: 0x0012A7BC
	public void ColliderUpdated(GameObject gameObject)
	{
		DebugDrawColliderRuntime.AddOrUpdate(gameObject, DebugDrawColliderRuntime.ColorType.None, false);
	}

	// Token: 0x06004496 RID: 17558 RVA: 0x0012C5C6 File Offset: 0x0012A7C6
	public void TilemapChunkCreated(Transform grandChild)
	{
		grandChild.gameObject.AddComponent<DebugDrawColliderRuntime>();
	}

	// Token: 0x06004497 RID: 17559 RVA: 0x0012C5D4 File Offset: 0x0012A7D4
	public bool IsFrozenCameraRendering()
	{
		return DisplayFrozenCamera.IsRendering;
	}
}
