using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004B6 RID: 1206
public class CogPlat : MonoBehaviour
{
	// Token: 0x06002B88 RID: 11144 RVA: 0x000BEF1F File Offset: 0x000BD11F
	private void OnEnable()
	{
		if (this.depressedCollision)
		{
			this.depressedCollision.CollisionEntered += this.OnCollisionEntered;
			this.depressedCollision.CollisionExited += this.OnCollisionExited;
		}
	}

	// Token: 0x06002B89 RID: 11145 RVA: 0x000BEF5C File Offset: 0x000BD15C
	private void OnDisable()
	{
		if (this.depressedCollision)
		{
			this.depressedCollision.CollisionEntered -= this.OnCollisionEntered;
			this.depressedCollision.CollisionExited -= this.OnCollisionExited;
		}
	}

	// Token: 0x06002B8A RID: 11146 RVA: 0x000BEF9C File Offset: 0x000BD19C
	private void OnCollisionEntered(Collision2D collision)
	{
		this.collisionsEntered++;
		if (!this.landedOnTop)
		{
			Collider2D otherCollider = collision.otherCollider;
			if (collision.collider.bounds.min.y >= otherCollider.bounds.max.y)
			{
				this.landedOnTop = true;
				this.SetAnimatorBool(this.depressedBool, true);
			}
		}
	}

	// Token: 0x06002B8B RID: 11147 RVA: 0x000BF007 File Offset: 0x000BD207
	private void OnCollisionExited(Collision2D collision)
	{
		this.collisionsEntered--;
		if (this.collisionsEntered == 0)
		{
			this.SetAnimatorBool(this.depressedBool, false);
			this.landedOnTop = false;
		}
	}

	// Token: 0x06002B8C RID: 11148 RVA: 0x000BF033 File Offset: 0x000BD233
	private void SetAnimatorBool(int hash, bool value)
	{
		if (!this.animator)
		{
			return;
		}
		this.animator.SetBool(hash, value);
	}

	// Token: 0x06002B8D RID: 11149 RVA: 0x000BF050 File Offset: 0x000BD250
	public void StartRotation()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		this.hasEnded = false;
		this.SetAnimatorBool(this.droppedBool, true);
		this.OnPlatDropped();
		Vector2 vector = this.centreMarker ? this.centreMarker.position : Vector2.zero;
		float x = vector.x;
		float x2 = base.transform.position.x;
		bool flag = base.transform.position.y > vector.y;
		bool arg;
		if (x2 == x)
		{
			arg = ((flag && this.movingClockWise) || (!flag && !this.movingClockWise));
		}
		else if (x2 > x)
		{
			arg = !this.movingClockWise;
		}
		else
		{
			arg = this.movingClockWise;
		}
		this.OnRotationStart.Invoke(arg);
	}

	// Token: 0x06002B8E RID: 11150 RVA: 0x000BF120 File Offset: 0x000BD320
	public void UpdateRotation(float time)
	{
		if (time > this.endPoint)
		{
			this.EndRotation();
		}
	}

	// Token: 0x06002B8F RID: 11151 RVA: 0x000BF131 File Offset: 0x000BD331
	public void EndRotation()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (!this.hasEnded)
		{
			this.hasEnded = true;
			this.SetAnimatorBool(this.droppedBool, false);
		}
	}

	// Token: 0x06002B90 RID: 11152 RVA: 0x000BF158 File Offset: 0x000BD358
	public void OnPlatDropped()
	{
		if (this.platform)
		{
			this.platform.SetLocalPositionZ(this.droppedZ);
		}
		if (this.platformCollider)
		{
			this.platformCollider.enabled = false;
		}
	}

	// Token: 0x06002B91 RID: 11153 RVA: 0x000BF191 File Offset: 0x000BD391
	public void OnPlatRaised()
	{
		if (this.platform)
		{
			this.platform.SetLocalPositionZ(this.raisedZ);
		}
		if (this.platformCollider)
		{
			this.platformCollider.enabled = true;
		}
	}

	// Token: 0x04002CD5 RID: 11477
	[SerializeField]
	private Animator animator;

	// Token: 0x04002CD6 RID: 11478
	private int droppedBool = Animator.StringToHash("Is Dropped");

	// Token: 0x04002CD7 RID: 11479
	private int depressedBool = Animator.StringToHash("Is Depressed");

	// Token: 0x04002CD8 RID: 11480
	[SerializeField]
	[Range(0f, 1f)]
	private float endPoint = 1f;

	// Token: 0x04002CD9 RID: 11481
	[Space]
	[SerializeField]
	private Transform platform;

	// Token: 0x04002CDA RID: 11482
	[SerializeField]
	private float droppedZ;

	// Token: 0x04002CDB RID: 11483
	[SerializeField]
	private float raisedZ;

	// Token: 0x04002CDC RID: 11484
	[SerializeField]
	private Collider2D platformCollider;

	// Token: 0x04002CDD RID: 11485
	[SerializeField]
	private CollisionEnterEvent depressedCollision;

	// Token: 0x04002CDE RID: 11486
	[Space]
	[SerializeField]
	private Transform centreMarker;

	// Token: 0x04002CDF RID: 11487
	[SerializeField]
	private bool movingClockWise;

	// Token: 0x04002CE0 RID: 11488
	[Space]
	public CogPlat.UnityEventBool OnRotationStart;

	// Token: 0x04002CE1 RID: 11489
	private int collisionsEntered;

	// Token: 0x04002CE2 RID: 11490
	private bool landedOnTop;

	// Token: 0x04002CE3 RID: 11491
	private bool hasEnded;

	// Token: 0x020017CA RID: 6090
	[Serializable]
	public class UnityEventBool : UnityEvent<bool>
	{
	}
}
