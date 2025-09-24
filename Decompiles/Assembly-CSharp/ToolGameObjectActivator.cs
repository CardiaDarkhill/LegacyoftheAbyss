using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005CF RID: 1487
public sealed class ToolGameObjectActivator : MonoBehaviour
{
	// Token: 0x060034E2 RID: 13538 RVA: 0x000EAB01 File Offset: 0x000E8D01
	private void OnEnable()
	{
		this.Evaulate();
	}

	// Token: 0x060034E3 RID: 13539 RVA: 0x000EAB0C File Offset: 0x000E8D0C
	private void Evaulate()
	{
		this.toolTests.RemoveAll((ToolGameObjectActivator.ToolTest o) => o.tool == null);
		bool flag = false;
		foreach (ToolGameObjectActivator.ToolTest toolTest in this.toolTests)
		{
			if (!toolTest.IsFulfilled)
			{
				flag = false;
				break;
			}
			flag = true;
		}
		if (this.activateGameObject)
		{
			this.activateGameObject.SetActive(flag);
		}
		if (this.deactivateGameObject)
		{
			this.deactivateGameObject.SetActive(!flag);
		}
	}

	// Token: 0x04003853 RID: 14419
	[Tooltip("Activated if tests pass, deactivated if tests fail.")]
	[SerializeField]
	private GameObject activateGameObject;

	// Token: 0x04003854 RID: 14420
	[Tooltip("Deactivated if tests pass, activated if tests fail")]
	[SerializeField]
	private GameObject deactivateGameObject;

	// Token: 0x04003855 RID: 14421
	[SerializeField]
	private List<ToolGameObjectActivator.ToolTest> toolTests = new List<ToolGameObjectActivator.ToolTest>();

	// Token: 0x020018DE RID: 6366
	[Serializable]
	private struct ToolTest
	{
		// Token: 0x17001047 RID: 4167
		// (get) Token: 0x0600928F RID: 37519 RVA: 0x0029C371 File Offset: 0x0029A571
		public bool IsFulfilled
		{
			get
			{
				return !(this.tool == null) && this.tool.IsUnlocked == this.expectedUnlockedState;
			}
		}

		// Token: 0x0400938F RID: 37775
		public ToolItem tool;

		// Token: 0x04009390 RID: 37776
		public bool expectedUnlockedState;
	}
}
