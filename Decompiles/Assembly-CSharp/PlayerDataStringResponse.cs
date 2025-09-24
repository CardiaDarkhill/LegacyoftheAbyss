using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000478 RID: 1144
public class PlayerDataStringResponse : MonoBehaviour
{
	// Token: 0x06002982 RID: 10626 RVA: 0x000B4D4E File Offset: 0x000B2F4E
	private void Start()
	{
		this.hasStarted = true;
		this.OnEnable();
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x000B4D5D File Offset: 0x000B2F5D
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.Evaluate();
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x000B4D70 File Offset: 0x000B2F70
	private void Evaluate()
	{
		string variable = PlayerData.instance.GetVariable(this.fieldName);
		this.OnValue.Invoke(variable);
	}

	// Token: 0x04002A16 RID: 10774
	[SerializeField]
	[PlayerDataField(typeof(string), true)]
	private string fieldName;

	// Token: 0x04002A17 RID: 10775
	[Space]
	[SerializeField]
	private PlayerDataStringResponse.UnityStringEvent OnValue;

	// Token: 0x04002A18 RID: 10776
	private bool hasStarted;

	// Token: 0x02001785 RID: 6021
	[Serializable]
	private class UnityStringEvent : UnityEvent<string>
	{
	}
}
