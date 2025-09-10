using System;
using UnityEngine;

// Token: 0x02000204 RID: 516
public abstract class DebugDrawColliderRuntimeAdder : MonoBehaviour
{
	// Token: 0x0600137B RID: 4987 RVA: 0x00058C93 File Offset: 0x00056E93
	protected virtual void Awake()
	{
		this.AddDebugDrawComponent();
	}

	// Token: 0x0600137C RID: 4988
	public abstract void AddDebugDrawComponent();
}
