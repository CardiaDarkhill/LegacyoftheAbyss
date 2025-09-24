using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005BF RID: 1471
public class SetPlayerDataBool : TriggerEnterEvent
{
	// Token: 0x06003494 RID: 13460 RVA: 0x000E9AC1 File Offset: 0x000E7CC1
	protected override void Awake()
	{
		base.Awake();
		if (this.setOnTriggerEnter)
		{
			base.OnTriggerEntered += this.OnTriggerEnteredEvent;
		}
	}

	// Token: 0x06003495 RID: 13461 RVA: 0x000E9AE3 File Offset: 0x000E7CE3
	protected override void Start()
	{
		base.Start();
		if (this.setOnLoad)
		{
			this.SetBool();
		}
	}

	// Token: 0x06003496 RID: 13462 RVA: 0x000E9AF9 File Offset: 0x000E7CF9
	private void OnTriggerEnteredEvent(Collider2D collision, GameObject sender)
	{
		if (this.setOnTriggerEnter)
		{
			this.SetBool();
		}
	}

	// Token: 0x06003497 RID: 13463 RVA: 0x000E9B09 File Offset: 0x000E7D09
	public void SetBool()
	{
		PlayerData.instance.SetVariable(this.boolName, this.value);
	}

	// Token: 0x04003809 RID: 14345
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string boolName;

	// Token: 0x0400380A RID: 14346
	[SerializeField]
	private bool value;

	// Token: 0x0400380B RID: 14347
	[SerializeField]
	private bool setOnLoad;

	// Token: 0x0400380C RID: 14348
	[SerializeField]
	private bool setOnTriggerEnter;
}
