using System;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class DeactivateIfPlayerdataTrue : MonoBehaviour
{
	// Token: 0x06001365 RID: 4965 RVA: 0x0005897A File Offset: 0x00056B7A
	private void Start()
	{
		this.hasStarted = true;
		this.ForceEvaluate();
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x00058989 File Offset: 0x00056B89
	private void OnEnable()
	{
		if (this.waitForStart && !this.hasStarted)
		{
			return;
		}
		this.ForceEvaluate();
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x000589A2 File Offset: 0x00056BA2
	private void ForceEvaluate()
	{
		if (!PlayerData.instance.GetBool(this.boolName))
		{
			return;
		}
		if (this.objectToDeactivate)
		{
			this.objectToDeactivate.SetActive(false);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x040011D4 RID: 4564
	[PlayerDataField(typeof(bool), true)]
	public string boolName;

	// Token: 0x040011D5 RID: 4565
	public GameObject objectToDeactivate;

	// Token: 0x040011D6 RID: 4566
	[SerializeField]
	private bool waitForStart;

	// Token: 0x040011D7 RID: 4567
	private bool hasStarted;
}
