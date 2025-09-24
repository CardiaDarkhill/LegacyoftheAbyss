using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000213 RID: 531
public class ActivationRange : MonoBehaviour
{
	// Token: 0x060013B5 RID: 5045 RVA: 0x00059D58 File Offset: 0x00057F58
	private void Awake()
	{
		if (this.initialiseChildren && !this.reverseActivation)
		{
			this.ActivateChildren();
			this.initialisers.AddRange(base.GetComponentsInChildren<IInitialisable>(true));
			foreach (IInitialisable initialisable in this.initialisers)
			{
				initialisable.OnAwake();
			}
		}
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x00059DD4 File Offset: 0x00057FD4
	private void Start()
	{
		if (!this.reverseActivation)
		{
			if (this.initialiseChildren)
			{
				foreach (IInitialisable initialisable in this.initialisers)
				{
					initialisable.OnStart();
				}
			}
			this.DeactivateChildren();
		}
		else
		{
			this.ActivateChildren();
		}
		this.initialisers.Clear();
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x00059E50 File Offset: 0x00058050
	private void OnDestroy()
	{
		this.initialisers.Clear();
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x00059E5D File Offset: 0x0005805D
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.reverseActivation)
		{
			this.ActivateChildren();
			return;
		}
		this.DeactivateChildren();
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x00059E74 File Offset: 0x00058074
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!this.reverseActivation)
		{
			this.DeactivateChildren();
			return;
		}
		this.ActivateChildren();
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x00059E8C File Offset: 0x0005808C
	private void ActivateChildren()
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(true);
		}
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x00059EE8 File Offset: 0x000580E8
	private void DeactivateChildren()
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
	}

	// Token: 0x04001225 RID: 4645
	public bool reverseActivation;

	// Token: 0x04001226 RID: 4646
	[FormerlySerializedAs("initChildren")]
	[SerializeField]
	private bool initialiseChildren;

	// Token: 0x04001227 RID: 4647
	private List<IInitialisable> initialisers = new List<IInitialisable>();
}
