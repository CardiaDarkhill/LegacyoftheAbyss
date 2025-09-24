using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000299 RID: 665
public class BattleSceneEnemy : MonoBehaviour, IInitialisable
{
	// Token: 0x06001740 RID: 5952 RVA: 0x00068E14 File Offset: 0x00067014
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.fsms = base.GetComponentsInChildren<PlayMakerFSM>(true).Except(this.exclude).OfType<PlayMakerFSM>().ToArray<PlayMakerFSM>();
		this.alertRanges = base.GetComponentsInChildren<AlertRange>(true).Except(this.exclude).OfType<AlertRange>().ToArray<AlertRange>();
		this.lineOfSightDetectors = base.GetComponentsInChildren<LineOfSightDetector>(true).Except(this.exclude).OfType<LineOfSightDetector>().ToArray<LineOfSightDetector>();
		return true;
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x00068E99 File Offset: 0x00067099
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x00068EB4 File Offset: 0x000670B4
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x00068EC0 File Offset: 0x000670C0
	public void SetActive(bool value)
	{
		this.OnAwake();
		PlayMakerFSM[] array = this.fsms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = value;
		}
		AlertRange[] array2 = this.alertRanges;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = value;
		}
		LineOfSightDetector[] array3 = this.lineOfSightDetectors;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].enabled = value;
		}
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x00068F36 File Offset: 0x00067136
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x040015E5 RID: 5605
	[SerializeField]
	private Component[] exclude;

	// Token: 0x040015E6 RID: 5606
	private PlayMakerFSM[] fsms;

	// Token: 0x040015E7 RID: 5607
	private AlertRange[] alertRanges;

	// Token: 0x040015E8 RID: 5608
	private LineOfSightDetector[] lineOfSightDetectors;

	// Token: 0x040015E9 RID: 5609
	private bool hasAwaken;

	// Token: 0x040015EA RID: 5610
	private bool hasStarted;
}
