using System;
using UnityEngine;

// Token: 0x02000589 RID: 1417
public class WeaverSpeedChallenge : MonoBehaviour
{
	// Token: 0x060032B7 RID: 12983 RVA: 0x000E1B9C File Offset: 0x000DFD9C
	private void Awake()
	{
		this.panels = this.panelsParent.GetComponentsInChildren<WeaverSpeedPanel>();
		this.maxTracker = new int[this.panels.Length];
		for (int i = 0; i < this.panels.Length; i++)
		{
			int index = i;
			this.panels[i].RecordedSpeedThreshold += delegate(int threshold)
			{
				WeaverSpeedPanel[] array = this.panels;
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].ForceStayLit)
					{
						return;
					}
				}
				this.maxTracker[index] = threshold;
				if (index >= this.panels.Length - 1)
				{
					this.CheckCompletion();
				}
			};
		}
	}

	// Token: 0x060032B8 RID: 12984 RVA: 0x000E1C10 File Offset: 0x000DFE10
	private void CheckCompletion()
	{
		bool flag = false;
		int num = int.MaxValue;
		foreach (int num2 in this.maxTracker)
		{
			if (!flag || num2 <= num)
			{
				flag = true;
				num = num2;
			}
		}
		WeaverSpeedChallenge.Target target = null;
		foreach (WeaverSpeedChallenge.Target target2 in this.targets)
		{
			if (target2.Threshold <= num && target2.Threshold > ((target != null) ? target.Threshold : 0))
			{
				target = target2;
			}
		}
		if (target == null)
		{
			return;
		}
		this.completionFsm.SendEvent(target.Event);
	}

	// Token: 0x060032B9 RID: 12985 RVA: 0x000E1CAC File Offset: 0x000DFEAC
	public void CapturePanels()
	{
		WeaverSpeedPanel[] array = this.panels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ForceStayLit = true;
		}
	}

	// Token: 0x060032BA RID: 12986 RVA: 0x000E1CD8 File Offset: 0x000DFED8
	public void ReleasePanels()
	{
		WeaverSpeedPanel[] array = this.panels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ForceStayLit = false;
		}
	}

	// Token: 0x040036A0 RID: 13984
	[SerializeField]
	private Transform panelsParent;

	// Token: 0x040036A1 RID: 13985
	[SerializeField]
	private PlayMakerFSM completionFsm;

	// Token: 0x040036A2 RID: 13986
	[SerializeField]
	private WeaverSpeedChallenge.Target[] targets;

	// Token: 0x040036A3 RID: 13987
	private WeaverSpeedPanel[] panels;

	// Token: 0x040036A4 RID: 13988
	private int[] maxTracker;

	// Token: 0x0200189F RID: 6303
	[Serializable]
	private class Target
	{
		// Token: 0x040092C6 RID: 37574
		public int Threshold;

		// Token: 0x040092C7 RID: 37575
		public string Event;
	}
}
