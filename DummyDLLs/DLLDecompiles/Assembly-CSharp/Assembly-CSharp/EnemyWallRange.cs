using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000330 RID: 816
public class EnemyWallRange : MonoBehaviour
{
	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06001C8E RID: 7310 RVA: 0x000851E0 File Offset: 0x000833E0
	// (set) Token: 0x06001C8F RID: 7311 RVA: 0x000851E8 File Offset: 0x000833E8
	public PlayMakerFSM TargetFSM
	{
		get
		{
			return this.targetFSM;
		}
		set
		{
			this.targetFSM = value;
			this.UpdateFSMVariables();
		}
	}

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06001C90 RID: 7312 RVA: 0x000851F7 File Offset: 0x000833F7
	// (set) Token: 0x06001C91 RID: 7313 RVA: 0x000851FF File Offset: 0x000833FF
	public string TargetBoolName
	{
		get
		{
			return this.targetBoolName;
		}
		set
		{
			this.targetBoolName = value;
			this.UpdateFSMVariables();
		}
	}

	// Token: 0x06001C92 RID: 7314 RVA: 0x0008520E File Offset: 0x0008340E
	public bool ValidateTargetBoolExists()
	{
		this.UpdateFSMVariables();
		return this.targetBool != null;
	}

	// Token: 0x06001C93 RID: 7315 RVA: 0x0008521F File Offset: 0x0008341F
	private void Start()
	{
		this.UpdateFSMVariables();
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x00085227 File Offset: 0x00083427
	private void OnTriggerEnter2D(Collider2D collision)
	{
		this.collidersInside.AddIfNotPresent(collision);
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x00085236 File Offset: 0x00083436
	private void OnTriggerExit2D(Collider2D collision)
	{
		this.collidersInside.Remove(collision);
	}

	// Token: 0x06001C96 RID: 7318 RVA: 0x00085248 File Offset: 0x00083448
	private void FixedUpdate()
	{
		if (this.targetBool == null)
		{
			return;
		}
		bool flag = this.collidersInside.Count > 0;
		if (this.targetBool.Value != flag)
		{
			this.targetBool.Value = flag;
		}
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x00085288 File Offset: 0x00083488
	private void UpdateFSMVariables()
	{
		this.targetBool = null;
		if (this.TargetFSM == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.TargetBoolName))
		{
			this.targetBool = this.TargetFSM.FsmVariables.FindFsmBool(this.TargetBoolName);
		}
	}

	// Token: 0x04001BBE RID: 7102
	[SerializeField]
	private PlayMakerFSM targetFSM;

	// Token: 0x04001BBF RID: 7103
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateTargetBoolExists")]
	private string targetBoolName;

	// Token: 0x04001BC0 RID: 7104
	private FsmBool targetBool;

	// Token: 0x04001BC1 RID: 7105
	private List<Collider2D> collidersInside = new List<Collider2D>();
}
