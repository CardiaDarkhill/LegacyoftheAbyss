using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000384 RID: 900
public class BossSequenceBindingsDisplay : MonoBehaviour
{
	// Token: 0x06001EA8 RID: 7848 RVA: 0x0008C87C File Offset: 0x0008AA7C
	private void Start()
	{
		int num = BossSequenceBindingsDisplay.CountCompletedBindings();
		for (int i = 0; i < this.bindingIcons.Length; i++)
		{
			this.bindingIcons[i].SetActive(i < num);
		}
	}

	// Token: 0x06001EA9 RID: 7849 RVA: 0x0008C8B4 File Offset: 0x0008AAB4
	public static void CountBindings(out int total, out int completed)
	{
		total = 0;
		completed = 0;
		foreach (FieldInfo fieldInfo in typeof(PlayerData).GetFields())
		{
			if (fieldInfo.FieldType == typeof(BossSequenceDoor.Completion))
			{
				BossSequenceDoor.Completion completion = (BossSequenceDoor.Completion)fieldInfo.GetValue(GameManager.instance.playerData);
				if (completion.completed)
				{
					if (completion.boundNail)
					{
						completed++;
					}
					if (completion.boundShell)
					{
						completed++;
					}
					if (completion.boundCharms)
					{
						completed++;
					}
					if (completion.boundSoul)
					{
						completed++;
					}
				}
				total += 4;
			}
		}
	}

	// Token: 0x06001EAA RID: 7850 RVA: 0x0008C960 File Offset: 0x0008AB60
	public static int CountCompletedBindings()
	{
		int num = 0;
		int result = 0;
		BossSequenceBindingsDisplay.CountBindings(out num, out result);
		return result;
	}

	// Token: 0x06001EAB RID: 7851 RVA: 0x0008C97C File Offset: 0x0008AB7C
	public static int CountTotalBindings()
	{
		int result = 0;
		int num = 0;
		BossSequenceBindingsDisplay.CountBindings(out result, out num);
		return result;
	}

	// Token: 0x04001D9F RID: 7583
	public GameObject[] bindingIcons;
}
