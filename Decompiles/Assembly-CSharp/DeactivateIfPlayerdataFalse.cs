using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020001FB RID: 507
public class DeactivateIfPlayerdataFalse : MonoBehaviour, GameMapPinLayout.IEvaluateHook
{
	// Token: 0x0600135D RID: 4957 RVA: 0x0005883D File Offset: 0x00056A3D
	private void Start()
	{
		this.hasStarted = true;
		this.ForceEvaluate();
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x0005884C File Offset: 0x00056A4C
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.ForceEvaluate();
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x00058860 File Offset: 0x00056A60
	public void ForceEvaluate()
	{
		if (this.gm == null)
		{
			this.gm = GameManager.instance;
		}
		if (this.gm.playerData.GetVariable(this.boolName))
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

	// Token: 0x040011CC RID: 4556
	[PlayerDataField(typeof(bool), true)]
	public string boolName;

	// Token: 0x040011CD RID: 4557
	public GameObject objectToDeactivate;

	// Token: 0x040011CE RID: 4558
	private bool hasStarted;

	// Token: 0x040011CF RID: 4559
	private GameManager gm;
}
