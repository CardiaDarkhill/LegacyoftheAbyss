using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200036B RID: 875
public class TestGameObjectActivator : MonoBehaviour, ISceneLintUpgrader
{
	// Token: 0x06001E10 RID: 7696 RVA: 0x0008AF5D File Offset: 0x0008915D
	private void Start()
	{
		this.DoEvaluate();
		this.hasStarted = true;
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x0008AF6C File Offset: 0x0008916C
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.DoEvaluate();
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x0008AF7D File Offset: 0x0008917D
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06001E13 RID: 7699 RVA: 0x0008AF85 File Offset: 0x00089185
	public void DoEvaluate()
	{
		if (this.checkActive)
		{
			base.StartCoroutine(this.DelayEvaluate());
			return;
		}
		this.Evaluate();
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x0008AFA8 File Offset: 0x000891A8
	private IEnumerator DelayEvaluate()
	{
		yield return null;
		this.Evaluate();
		yield break;
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x0008AFB8 File Offset: 0x000891B8
	private void Evaluate()
	{
		bool flag = this.playerDataTest.IsFulfilled;
		if (flag)
		{
			foreach (QuestTest questTest in this.questTests)
			{
				if (!questTest.IsFulfilled)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			ToolBase[] array2 = this.equipTests;
			for (int i = 0; i < array2.Length; i++)
			{
				if (!array2[i].IsEquipped)
				{
					flag = false;
					break;
				}
			}
		}
		string entryGateName = GameManager.instance.GetEntryGateName();
		if (flag && this.entryGateWhitelist.Length != 0 && !entryGateName.IsAny(this.entryGateWhitelist))
		{
			flag = false;
		}
		if (flag && entryGateName.IsAny(this.entryGateBlacklist))
		{
			flag = false;
		}
		if (this.checkActive && this.checkActive.activeInHierarchy != this.expectedActive)
		{
			flag = false;
		}
		if (this.activateGameObject)
		{
			this.activateGameObject.SetActive(flag);
		}
		if (this.deactivateGameObject)
		{
			this.deactivateGameObject.SetActive(!flag);
		}
		EventRegister.SendEvent(flag ? this.activateEventRegister : this.deactivateEventRegister, null);
	}

	// Token: 0x06001E16 RID: 7702 RVA: 0x0008B0D0 File Offset: 0x000892D0
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		bool flag = false;
		if (this.activateGameObject && this.activateGameObject.activeSelf && this.activateGameObject != base.gameObject)
		{
			this.activateGameObject.SetActive(false);
			flag = true;
		}
		if (this.deactivateGameObject && this.deactivateGameObject.activeSelf && this.deactivateGameObject != base.gameObject)
		{
			this.deactivateGameObject.SetActive(false);
			flag = true;
		}
		if (!flag)
		{
			return null;
		}
		return "TestGameObjectActivator disabled targets";
	}

	// Token: 0x04001D29 RID: 7465
	[SerializeField]
	private GameObject activateGameObject;

	// Token: 0x04001D2A RID: 7466
	[SerializeField]
	private GameObject deactivateGameObject;

	// Token: 0x04001D2B RID: 7467
	[Space]
	[SerializeField]
	private string activateEventRegister;

	// Token: 0x04001D2C RID: 7468
	[SerializeField]
	private string deactivateEventRegister;

	// Token: 0x04001D2D RID: 7469
	[Header("Tests")]
	[SerializeField]
	private PlayerDataTest playerDataTest;

	// Token: 0x04001D2E RID: 7470
	[Space]
	[SerializeField]
	private QuestTest[] questTests;

	// Token: 0x04001D2F RID: 7471
	[SerializeField]
	private ToolBase[] equipTests;

	// Token: 0x04001D30 RID: 7472
	[Space]
	[SerializeField]
	private string[] entryGateWhitelist;

	// Token: 0x04001D31 RID: 7473
	[SerializeField]
	private string[] entryGateBlacklist;

	// Token: 0x04001D32 RID: 7474
	[Space]
	[SerializeField]
	private GameObject checkActive;

	// Token: 0x04001D33 RID: 7475
	[SerializeField]
	private bool expectedActive;

	// Token: 0x04001D34 RID: 7476
	private bool hasStarted;
}
