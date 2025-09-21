using System;
using UnityEngine;

// Token: 0x02000290 RID: 656
public class EndBeta : MonoBehaviour
{
	// Token: 0x060016F4 RID: 5876 RVA: 0x00067E17 File Offset: 0x00066017
	private void Awake()
	{
		this.gm = GameManager.instance;
		this.isActive = true;
	}

	// Token: 0x060016F5 RID: 5877 RVA: 0x00067E2B File Offset: 0x0006602B
	private void Start()
	{
	}

	// Token: 0x060016F6 RID: 5878 RVA: 0x00067E2D File Offset: 0x0006602D
	private void Update()
	{
	}

	// Token: 0x060016F7 RID: 5879 RVA: 0x00067E2F File Offset: 0x0006602F
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.tag == "Player" && this.isActive)
		{
			this.isActive = false;
		}
	}

	// Token: 0x060016F8 RID: 5880 RVA: 0x00067E57 File Offset: 0x00066057
	public void Reactivate()
	{
		this.isActive = true;
	}

	// Token: 0x04001589 RID: 5513
	private GameManager gm;

	// Token: 0x0400158A RID: 5514
	private bool isActive;
}
