using System;
using UnityEngine;

// Token: 0x02000785 RID: 1925
public sealed class SetRigidBody2D : MonoBehaviour
{
	// Token: 0x0600445F RID: 17503 RVA: 0x0012B94E File Offset: 0x00129B4E
	private void Awake()
	{
		this.FindRigidBody();
	}

	// Token: 0x06004460 RID: 17504 RVA: 0x0012B956 File Offset: 0x00129B56
	private void Start()
	{
		this.SetState(SetRigidBody2D.SetEvent.Start);
	}

	// Token: 0x06004461 RID: 17505 RVA: 0x0012B95F File Offset: 0x00129B5F
	private void OnValidate()
	{
		this.FindRigidBody();
	}

	// Token: 0x06004462 RID: 17506 RVA: 0x0012B967 File Offset: 0x00129B67
	private void OnEnable()
	{
		this.SetState(SetRigidBody2D.SetEvent.OnEnable);
	}

	// Token: 0x06004463 RID: 17507 RVA: 0x0012B970 File Offset: 0x00129B70
	private void OnDisable()
	{
		this.SetState(SetRigidBody2D.SetEvent.OnDisable);
	}

	// Token: 0x06004464 RID: 17508 RVA: 0x0012B979 File Offset: 0x00129B79
	private void FindRigidBody()
	{
		this.hasRb2d = this.rigidbody2D;
		if (!this.hasRb2d)
		{
			this.rigidbody2D = base.GetComponent<Rigidbody2D>();
			this.hasRb2d = this.rigidbody2D;
		}
	}

	// Token: 0x06004465 RID: 17509 RVA: 0x0012B9B1 File Offset: 0x00129BB1
	private void SetState(SetRigidBody2D.SetEvent state)
	{
		if (this.hasRb2d)
		{
			if ((this.setVelocity & state) != SetRigidBody2D.SetEvent.None)
			{
				this.rigidbody2D.linearVelocity = this.velocity;
			}
			if ((this.setAngularVelocity & state) != SetRigidBody2D.SetEvent.None)
			{
				this.rigidbody2D.angularVelocity = this.angularVelocity;
			}
		}
	}

	// Token: 0x04004577 RID: 17783
	[SerializeField]
	private Rigidbody2D rigidbody2D;

	// Token: 0x04004578 RID: 17784
	[Space]
	[SerializeField]
	private SetRigidBody2D.SetEvent setVelocity = SetRigidBody2D.SetEvent.All;

	// Token: 0x04004579 RID: 17785
	[SerializeField]
	private Vector2 velocity;

	// Token: 0x0400457A RID: 17786
	[Space]
	[SerializeField]
	private SetRigidBody2D.SetEvent setAngularVelocity = SetRigidBody2D.SetEvent.All;

	// Token: 0x0400457B RID: 17787
	[SerializeField]
	private float angularVelocity;

	// Token: 0x0400457C RID: 17788
	private bool hasRb2d;

	// Token: 0x02001A68 RID: 6760
	[Flags]
	[Serializable]
	private enum SetEvent
	{
		// Token: 0x0400995D RID: 39261
		None = 0,
		// Token: 0x0400995E RID: 39262
		Start = 1,
		// Token: 0x0400995F RID: 39263
		OnEnable = 2,
		// Token: 0x04009960 RID: 39264
		OnDisable = 4,
		// Token: 0x04009961 RID: 39265
		All = -1
	}
}
