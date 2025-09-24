using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000752 RID: 1874
[DisallowMultipleComponent]
public sealed class AwakenChildren : MonoBehaviour
{
	// Token: 0x06004290 RID: 17040 RVA: 0x0012593C File Offset: 0x00123B3C
	private void Awake()
	{
		this.awakenTarget.RemoveAll((GameObject o) => o == null);
		this.activeState = new bool[this.awakenTarget.Count];
		for (int i = 0; i < this.awakenTarget.Count; i++)
		{
			GameObject gameObject = this.awakenTarget[i];
			bool activeSelf = gameObject.activeSelf;
			this.activeState[i] = activeSelf;
			if (!activeSelf)
			{
				gameObject.SetActive(true);
				this.didAwaken = true;
			}
		}
	}

	// Token: 0x06004291 RID: 17041 RVA: 0x001259D0 File Offset: 0x00123BD0
	private void Start()
	{
		if (this.didAwaken)
		{
			for (int i = 0; i < this.awakenTarget.Count; i++)
			{
				GameObject gameObject = this.awakenTarget[i];
				if (!this.activeState[i])
				{
					IInitialisable[] componentsInChildren = gameObject.GetComponentsInChildren<IInitialisable>(true);
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						componentsInChildren[j].OnStart();
					}
					gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06004292 RID: 17042 RVA: 0x00125A38 File Offset: 0x00123C38
	[ContextMenu("Gather FSMs")]
	private void GatherFSMs()
	{
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		foreach (PlayMakerFSM playMakerFSM in base.GetComponentsInChildren<PlayMakerFSM>(true))
		{
			hashSet.Add(playMakerFSM.gameObject);
		}
		hashSet.Remove(base.gameObject);
		this.awakenTarget.RemoveAll((GameObject o) => o == null);
		this.awakenTarget = this.awakenTarget.Union(hashSet).ToList<GameObject>();
	}

	// Token: 0x04004414 RID: 17428
	[SerializeField]
	private List<GameObject> awakenTarget = new List<GameObject>();

	// Token: 0x04004415 RID: 17429
	private bool didAwaken;

	// Token: 0x04004416 RID: 17430
	private bool[] activeState;
}
