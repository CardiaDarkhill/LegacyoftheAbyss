using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000344 RID: 836
public class CaptureAnimationEvent : MonoBehaviour, IMutable
{
	// Token: 0x14000053 RID: 83
	// (add) Token: 0x06001D10 RID: 7440 RVA: 0x00086EFC File Offset: 0x000850FC
	// (remove) Token: 0x06001D11 RID: 7441 RVA: 0x00086F34 File Offset: 0x00085134
	public event Action EventFired;

	// Token: 0x14000054 RID: 84
	// (add) Token: 0x06001D12 RID: 7442 RVA: 0x00086F6C File Offset: 0x0008516C
	// (remove) Token: 0x06001D13 RID: 7443 RVA: 0x00086FA4 File Offset: 0x000851A4
	public event Action EventFiredTemp;

	// Token: 0x06001D14 RID: 7444 RVA: 0x00086FD9 File Offset: 0x000851D9
	private void Start()
	{
		this.hc = HeroController.instance;
		this.playerData = this.hc.playerData;
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x00086FF7 File Offset: 0x000851F7
	public void FireEvent()
	{
		if (this.muted)
		{
			return;
		}
		if (this.EventFired != null)
		{
			this.EventFired();
		}
		if (this.EventFiredTemp != null)
		{
			this.EventFiredTemp();
			this.ClearTempEvent();
		}
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x0008702E File Offset: 0x0008522E
	public void ClearTempEvent()
	{
		this.EventFiredTemp = null;
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x00087037 File Offset: 0x00085237
	public void FireIndexedEvent(int index)
	{
		if (this.muted)
		{
			return;
		}
		if (this.indexedEvents == null || this.indexedEvents.Length <= index)
		{
			return;
		}
		this.indexedEvents[index].Invoke();
	}

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06001D18 RID: 7448 RVA: 0x00087063 File Offset: 0x00085263
	public bool Muted
	{
		get
		{
			return this.muted;
		}
	}

	// Token: 0x06001D19 RID: 7449 RVA: 0x0008706B File Offset: 0x0008526B
	public void SetMute(bool muted)
	{
		this.muted = muted;
	}

	// Token: 0x06001D1A RID: 7450 RVA: 0x00087074 File Offset: 0x00085274
	public void SetPlayerDataBoolTrue(string boolName)
	{
		this.playerData.SetBool(boolName, true);
	}

	// Token: 0x06001D1B RID: 7451 RVA: 0x00087083 File Offset: 0x00085283
	public void SetPlayerDataBoolFalse(string boolName)
	{
		this.playerData.SetBool(boolName, false);
	}

	// Token: 0x06001D1C RID: 7452 RVA: 0x00087092 File Offset: 0x00085292
	public void IncrementPlayerDataInt(string intName)
	{
		this.playerData.IncrementInt(intName);
	}

	// Token: 0x06001D1D RID: 7453 RVA: 0x000870A0 File Offset: 0x000852A0
	public void DecrementPlayerDataInt(string intName)
	{
		this.playerData.DecrementInt(intName);
	}

	// Token: 0x06001D1E RID: 7454 RVA: 0x000870AE File Offset: 0x000852AE
	public bool GetPlayerDataBool(string boolName)
	{
		return this.playerData.GetBool(boolName);
	}

	// Token: 0x06001D1F RID: 7455 RVA: 0x000870BC File Offset: 0x000852BC
	public int GetPlayerDataInt(string intName)
	{
		return this.playerData.GetInt(intName);
	}

	// Token: 0x06001D20 RID: 7456 RVA: 0x000870CA File Offset: 0x000852CA
	public float GetPlayerDataFloat(string floatName)
	{
		return this.playerData.GetFloat(floatName);
	}

	// Token: 0x06001D21 RID: 7457 RVA: 0x000870D8 File Offset: 0x000852D8
	public string GetPlayerDataString(string stringName)
	{
		return this.playerData.GetString(stringName);
	}

	// Token: 0x06001D22 RID: 7458 RVA: 0x000870E6 File Offset: 0x000852E6
	public void EquipCharm(int charmNum)
	{
		this.playerData.EquipCharm(charmNum);
	}

	// Token: 0x06001D23 RID: 7459 RVA: 0x000870F4 File Offset: 0x000852F4
	public void UnequipCharm(int charmNum)
	{
		this.playerData.UnequipCharm(charmNum);
	}

	// Token: 0x06001D24 RID: 7460 RVA: 0x00087102 File Offset: 0x00085302
	public void UpdateBlueHealth()
	{
		this.hc.UpdateBlueHealth();
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x0008710F File Offset: 0x0008530F
	public void SendEventRegister(string eventName)
	{
		EventRegister.SendEvent(eventName, null);
	}

	// Token: 0x04001C6F RID: 7279
	[SerializeField]
	private UnityEvent[] indexedEvents;

	// Token: 0x04001C70 RID: 7280
	private PlayerData playerData;

	// Token: 0x04001C71 RID: 7281
	private HeroController hc;

	// Token: 0x04001C72 RID: 7282
	private bool muted;
}
