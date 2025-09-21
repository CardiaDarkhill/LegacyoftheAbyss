using System;
using UnityEngine;

// Token: 0x02000483 RID: 1155
public class ProjectileVelocityManager : MonoBehaviour
{
	// Token: 0x170004F7 RID: 1271
	// (get) Token: 0x060029B5 RID: 10677 RVA: 0x000B588C File Offset: 0x000B3A8C
	// (set) Token: 0x060029B6 RID: 10678 RVA: 0x000B5894 File Offset: 0x000B3A94
	public Vector2 DesiredVelocity
	{
		get
		{
			return this._desiredVelocity;
		}
		set
		{
			this._desiredVelocity = value;
			this.UpdateVelocity();
		}
	}

	// Token: 0x060029B7 RID: 10679 RVA: 0x000B58A3 File Offset: 0x000B3AA3
	private void Awake()
	{
		if (this.enemyDamager)
		{
			this.enemyDamager.DamagedEnemy += this.OnDamagedEnemy;
		}
	}

	// Token: 0x060029B8 RID: 10680 RVA: 0x000B58C9 File Offset: 0x000B3AC9
	private void OnEnable()
	{
		this.currentMultiplier = 1f;
	}

	// Token: 0x060029B9 RID: 10681 RVA: 0x000B58D6 File Offset: 0x000B3AD6
	private void OnDisable()
	{
		this.StopDampRoutine();
	}

	// Token: 0x060029BA RID: 10682 RVA: 0x000B58DE File Offset: 0x000B3ADE
	private void OnDestroy()
	{
		if (this.enemyDamager)
		{
			this.enemyDamager.DamagedEnemy -= this.OnDamagedEnemy;
		}
	}

	// Token: 0x060029BB RID: 10683 RVA: 0x000B5904 File Offset: 0x000B3B04
	private void OnDamagedEnemy()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		this.StopDampRoutine();
		this.dampRoutine = this.StartTimerRoutine(0f, this.hitDampDuration, delegate(float time)
		{
			this.currentMultiplier = Mathf.Lerp(this.hitDamping, 1f, this.hitDampReturnCurve.Evaluate(time));
			this.UpdateVelocity();
		}, null, delegate
		{
			this.dampRoutine = null;
		}, false);
	}

	// Token: 0x060029BC RID: 10684 RVA: 0x000B5951 File Offset: 0x000B3B51
	private void StopDampRoutine()
	{
		if (this.dampRoutine != null)
		{
			base.StopCoroutine(this.dampRoutine);
			this.dampRoutine = null;
		}
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x000B596E File Offset: 0x000B3B6E
	private void UpdateVelocity()
	{
		this.body.linearVelocity = this.DesiredVelocity * this.currentMultiplier;
	}

	// Token: 0x04002A3F RID: 10815
	[SerializeField]
	private DamageEnemies enemyDamager;

	// Token: 0x04002A40 RID: 10816
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x04002A41 RID: 10817
	[SerializeField]
	private float hitDamping;

	// Token: 0x04002A42 RID: 10818
	[SerializeField]
	private float hitDampDuration;

	// Token: 0x04002A43 RID: 10819
	[SerializeField]
	private AnimationCurve hitDampReturnCurve;

	// Token: 0x04002A44 RID: 10820
	private Coroutine dampRoutine;

	// Token: 0x04002A45 RID: 10821
	private float currentMultiplier;

	// Token: 0x04002A46 RID: 10822
	private Vector2 _desiredVelocity;
}
