using System;
using UnityEngine;

// Token: 0x02000355 RID: 853
public sealed class InitialiseChildren : MonoBehaviour
{
	// Token: 0x06001D91 RID: 7569 RVA: 0x00088764 File Offset: 0x00086964
	private void Awake()
	{
		this.children = base.GetComponentsInChildren<IInitialisable>(true);
		IInitialisable[] array = this.children;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnAwake();
		}
	}

	// Token: 0x06001D92 RID: 7570 RVA: 0x0008879C File Offset: 0x0008699C
	private void Start()
	{
		foreach (IInitialisable initialisable in this.children)
		{
			initialisable.OnStart();
			PersonalObjectPool.CreateIfRequired(initialisable.gameObject, this.forcePoolSpawn);
		}
		this.children = null;
	}

	// Token: 0x04001CC3 RID: 7363
	[SerializeField]
	private bool forcePoolSpawn = true;

	// Token: 0x04001CC4 RID: 7364
	private IInitialisable[] children;
}
