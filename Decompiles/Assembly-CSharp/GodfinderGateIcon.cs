using System;
using UnityEngine;

// Token: 0x020003AC RID: 940
public class GodfinderGateIcon : MonoBehaviour
{
	// Token: 0x06001FA2 RID: 8098 RVA: 0x00090A5B File Offset: 0x0008EC5B
	private void Reset()
	{
		this.OnValidate();
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x00090A64 File Offset: 0x0008EC64
	private void OnValidate()
	{
		int num = Enum.GetNames(typeof(GodfinderGateIcon.IconType)).Length;
		if (this.icons.Length != num)
		{
			GameObject[] array = this.icons;
			this.icons = new GameObject[num];
			for (int i = 0; i < array.Length; i++)
			{
				this.icons[i] = array[i];
			}
		}
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x00090ABC File Offset: 0x0008ECBC
	private void SetIcon(GodfinderGateIcon.IconType type)
	{
		for (int i = 0; i < this.icons.Length; i++)
		{
			if (this.icons[i])
			{
				this.icons[i].SetActive(i == (int)type);
			}
		}
	}

	// Token: 0x06001FA5 RID: 8101 RVA: 0x00090AFC File Offset: 0x0008ECFC
	public void Evaluate()
	{
		if (!string.IsNullOrEmpty(this.requiredPDBool) && !GameManager.instance.GetPlayerDataBool(this.requiredPDBool))
		{
			base.gameObject.SetActive(false);
			return;
		}
		BossSequenceDoor.Completion playerDataVariable = GameManager.instance.GetPlayerDataVariable<BossSequenceDoor.Completion>(this.completionPD);
		if (playerDataVariable.allBindings)
		{
			this.SetIcon(GodfinderGateIcon.IconType.CompleteRadiant);
		}
		else if (playerDataVariable.completed)
		{
			this.SetIcon(GodfinderGateIcon.IconType.Complete);
		}
		else if (playerDataVariable.unlocked || playerDataVariable.canUnlock || (this.unlockedSequence && this.unlockedSequence.IsUnlocked()))
		{
			this.SetIcon(GodfinderGateIcon.IconType.Unbound);
		}
		else
		{
			this.SetIcon(GodfinderGateIcon.IconType.Bound);
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x04001EB2 RID: 7858
	[ArrayForEnum(typeof(GodfinderGateIcon.IconType))]
	public GameObject[] icons;

	// Token: 0x04001EB3 RID: 7859
	public string completionPD;

	// Token: 0x04001EB4 RID: 7860
	[Tooltip("If assigned, icon will show unlocked when tier CAN be unlocked, rather than when the lock has been broken.")]
	public BossSequence unlockedSequence;

	// Token: 0x04001EB5 RID: 7861
	public string requiredPDBool;

	// Token: 0x02001666 RID: 5734
	public enum IconType
	{
		// Token: 0x04008AA8 RID: 35496
		Bound,
		// Token: 0x04008AA9 RID: 35497
		Unbound,
		// Token: 0x04008AAA RID: 35498
		Complete,
		// Token: 0x04008AAB RID: 35499
		CompleteRadiant
	}
}
