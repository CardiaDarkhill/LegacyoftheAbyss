using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000313 RID: 787
public class Recoil : MonoBehaviour
{
	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06001BC9 RID: 7113 RVA: 0x0008194F File Offset: 0x0007FB4F
	public float RecoilSpeedBase
	{
		get
		{
			return this.recoilSpeedBase;
		}
	}

	// Token: 0x1400004F RID: 79
	// (add) Token: 0x06001BCA RID: 7114 RVA: 0x00081958 File Offset: 0x0007FB58
	// (remove) Token: 0x06001BCB RID: 7115 RVA: 0x00081990 File Offset: 0x0007FB90
	public event Recoil.FreezeEvent OnHandleFreeze;

	// Token: 0x14000050 RID: 80
	// (add) Token: 0x06001BCC RID: 7116 RVA: 0x000819C8 File Offset: 0x0007FBC8
	// (remove) Token: 0x06001BCD RID: 7117 RVA: 0x00081A00 File Offset: 0x0007FC00
	public event Recoil.CancelRecoilEvent OnCancelRecoil;

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x06001BCE RID: 7118 RVA: 0x00081A35 File Offset: 0x0007FC35
	// (set) Token: 0x06001BCF RID: 7119 RVA: 0x00081A3D File Offset: 0x0007FC3D
	public bool SkipFreezingByController
	{
		get
		{
			return this.skipFreezingByController;
		}
		set
		{
			this.skipFreezingByController = value;
		}
	}

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06001BD0 RID: 7120 RVA: 0x00081A46 File Offset: 0x0007FC46
	public bool IsRecoiling
	{
		get
		{
			return this.state == Recoil.States.Recoiling || this.state == Recoil.States.Frozen;
		}
	}

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06001BD1 RID: 7121 RVA: 0x00081A5C File Offset: 0x0007FC5C
	private float RecoilMultiplier
	{
		get
		{
			if (this.recoilMultipliers == null)
			{
				return 1f;
			}
			float num = 1f;
			foreach (Recoil.IRecoilMultiplier recoilMultiplier in this.recoilMultipliers)
			{
				num *= recoilMultiplier.GetRecoilMultiplier();
			}
			return num;
		}
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x00081AC8 File Offset: 0x0007FCC8
	protected void Reset()
	{
		this.FreezeInPlace = false;
		this.recoilDuration = 0.5f;
		this.recoilSpeedBase = 15f;
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x00081AE7 File Offset: 0x0007FCE7
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.bodyCollider = base.GetComponent<Collider2D>();
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x00081B01 File Offset: 0x0007FD01
	private void OnEnable()
	{
		this.CancelRecoil();
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x00081B0C File Offset: 0x0007FD0C
	public void RecoilByHealthManagerFSMParameters()
	{
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "health_manager_enemy");
		int cardinalDirection = DirectionUtils.GetCardinalDirection(playMakerFSM.FsmVariables.GetFsmFloat("Attack Direction").Value);
		int value = playMakerFSM.FsmVariables.GetFsmInt("Attack Type").Value;
		float value2 = playMakerFSM.FsmVariables.GetFsmFloat("Attack Magnitude").Value;
		this.RecoilByDirection(cardinalDirection, value2);
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x00081B78 File Offset: 0x0007FD78
	public void RecoilByDamage(HitInstance damageInstance)
	{
		int cardinalDirection = DirectionUtils.GetCardinalDirection(damageInstance.Direction);
		this.RecoilByDirection(cardinalDirection, damageInstance.MagnitudeMultiplier);
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x00081B9E File Offset: 0x0007FD9E
	public void RecoilDirectly(int attackDirection)
	{
		this.RecoilByDirection(attackDirection, 1f);
	}

	// Token: 0x06001BD8 RID: 7128 RVA: 0x00081BAC File Offset: 0x0007FDAC
	public void RecoilByDirection(int attackDirection, float attackMagnitude)
	{
		float num = this.recoilSpeedBase * attackMagnitude * this.RecoilMultiplier;
		if (this.state != Recoil.States.Ready)
		{
			float num2 = this.recoilTimeRemaining / this.recoilDuration;
			float num3 = this.recoilSpeed * num2;
			if (num < num3)
			{
				return;
			}
		}
		if (this.FreezeInPlace)
		{
			this.Freeze();
			return;
		}
		if (attackDirection == 1 && this.IsUpBlocked)
		{
			return;
		}
		if (attackDirection == 3 && this.IsDownBlocked)
		{
			return;
		}
		if (attackDirection == 2 && this.IsLeftBlocked)
		{
			return;
		}
		if (attackDirection == 0 && this.IsRightBlocked)
		{
			return;
		}
		switch (attackDirection)
		{
		case 0:
			FSMUtility.SendEventToGameObject(base.gameObject, "RECOIL HORIZONTAL", false);
			FSMUtility.SendEventToGameObject(base.gameObject, "HIT RIGHT", false);
			this.previousRecoilAngle = 0f;
			break;
		case 1:
			FSMUtility.SendEventToGameObject(base.gameObject, "HIT UP", false);
			this.previousRecoilAngle = 90f;
			break;
		case 2:
			FSMUtility.SendEventToGameObject(base.gameObject, "RECOIL HORIZONTAL", false);
			FSMUtility.SendEventToGameObject(base.gameObject, "HIT LEFT", false);
			this.previousRecoilAngle = 180f;
			break;
		case 3:
			FSMUtility.SendEventToGameObject(base.gameObject, "HIT DOWN", false);
			this.previousRecoilAngle = 270f;
			break;
		default:
			return;
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "RECOIL", false);
		if (this.bodyCollider == null)
		{
			this.bodyCollider = base.GetComponent<Collider2D>();
		}
		this.state = Recoil.States.Recoiling;
		this.recoilSpeed = num;
		this.recoilSweep = new Sweep(this.bodyCollider, attackDirection, 3, 0.1f, 0.01f);
		this.isRecoilSweeping = true;
		this.recoilTimeRemaining = this.recoilDuration;
		this.UpdatePhysics(0f);
	}

	// Token: 0x06001BD9 RID: 7129 RVA: 0x00081D52 File Offset: 0x0007FF52
	public void CancelRecoil()
	{
		if (this.state != Recoil.States.Ready)
		{
			this.state = Recoil.States.Ready;
			if (this.OnCancelRecoil != null)
			{
				this.OnCancelRecoil();
			}
			FSMUtility.SendEventToGameObject(base.gameObject, "RECOIL END", false);
		}
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x00081D88 File Offset: 0x0007FF88
	private void Freeze()
	{
		if (this.skipFreezingByController)
		{
			if (this.OnHandleFreeze != null)
			{
				this.OnHandleFreeze();
			}
			this.state = Recoil.States.Ready;
			return;
		}
		this.state = Recoil.States.Frozen;
		if (this.body != null)
		{
			this.body.linearVelocity = Vector2.zero;
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "FREEZE IN PLACE", false);
		this.recoilTimeRemaining = this.recoilDuration;
		this.UpdatePhysics(0f);
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x00081E05 File Offset: 0x00080005
	protected void FixedUpdate()
	{
		this.UpdatePhysics(Time.fixedDeltaTime);
	}

	// Token: 0x06001BDC RID: 7132 RVA: 0x00081E14 File Offset: 0x00080014
	private void UpdatePhysics(float deltaTime)
	{
		if (this.state == Recoil.States.Frozen)
		{
			if (this.body != null)
			{
				this.body.linearVelocity = Vector2.zero;
			}
			this.recoilTimeRemaining -= deltaTime;
			if (this.recoilTimeRemaining <= 0f)
			{
				this.CancelRecoil();
				return;
			}
		}
		else if (this.state == Recoil.States.Recoiling)
		{
			if (this.isRecoilSweeping)
			{
				float num;
				if (this.recoilSweep.Check(this.recoilSpeed * deltaTime, 256, out num))
				{
					this.isRecoilSweeping = false;
				}
				if (num > Mathf.Epsilon)
				{
					Vector2 vector = this.recoilSweep.Direction * num;
					if (this.body)
					{
						this.body.position += vector;
					}
					else
					{
						base.transform.Translate(vector, Space.World);
					}
				}
			}
			this.recoilTimeRemaining -= deltaTime;
			if (this.recoilTimeRemaining <= 0f)
			{
				this.CancelRecoil();
			}
		}
	}

	// Token: 0x06001BDD RID: 7133 RVA: 0x00081F17 File Offset: 0x00080117
	public void SetRecoilSpeed(float newSpeed)
	{
		this.recoilSpeedBase = newSpeed;
	}

	// Token: 0x06001BDE RID: 7134 RVA: 0x00081F20 File Offset: 0x00080120
	public bool GetIsRecoiling()
	{
		return this.state == Recoil.States.Recoiling;
	}

	// Token: 0x06001BDF RID: 7135 RVA: 0x00081F2B File Offset: 0x0008012B
	public float GetPreviousRecoilAngle()
	{
		return this.previousRecoilAngle;
	}

	// Token: 0x06001BE0 RID: 7136 RVA: 0x00081F33 File Offset: 0x00080133
	public void AddRecoilMultiplier(Recoil.IRecoilMultiplier multiplier)
	{
		if (this.recoilMultipliers == null)
		{
			this.recoilMultipliers = new List<Recoil.IRecoilMultiplier>();
		}
		this.recoilMultipliers.Add(multiplier);
	}

	// Token: 0x06001BE1 RID: 7137 RVA: 0x00081F54 File Offset: 0x00080154
	public void RemoveRecoilMultiplier(Recoil.IRecoilMultiplier multiplier)
	{
		List<Recoil.IRecoilMultiplier> list = this.recoilMultipliers;
		if (list == null)
		{
			return;
		}
		list.Remove(multiplier);
	}

	// Token: 0x04001AD3 RID: 6867
	[FormerlySerializedAs("freezeInPlace")]
	public bool FreezeInPlace;

	// Token: 0x04001AD4 RID: 6868
	[SerializeField]
	[ModifiableProperty]
	[Conditional("FreezeInPlace", false, false, false)]
	private float recoilSpeedBase;

	// Token: 0x04001AD5 RID: 6869
	[SerializeField]
	[ModifiableProperty]
	[Conditional("FreezeInPlace", false, false, false)]
	private float recoilDuration;

	// Token: 0x04001AD6 RID: 6870
	[Space]
	public bool IsUpBlocked;

	// Token: 0x04001AD7 RID: 6871
	public bool IsDownBlocked;

	// Token: 0x04001AD8 RID: 6872
	public bool IsLeftBlocked;

	// Token: 0x04001AD9 RID: 6873
	public bool IsRightBlocked;

	// Token: 0x04001ADC RID: 6876
	private bool skipFreezingByController;

	// Token: 0x04001ADD RID: 6877
	private Recoil.States state;

	// Token: 0x04001ADE RID: 6878
	private float recoilTimeRemaining;

	// Token: 0x04001ADF RID: 6879
	private float recoilSpeed;

	// Token: 0x04001AE0 RID: 6880
	private Sweep recoilSweep;

	// Token: 0x04001AE1 RID: 6881
	private bool isRecoilSweeping;

	// Token: 0x04001AE2 RID: 6882
	private float previousRecoilAngle;

	// Token: 0x04001AE3 RID: 6883
	private List<Recoil.IRecoilMultiplier> recoilMultipliers;

	// Token: 0x04001AE4 RID: 6884
	private Rigidbody2D body;

	// Token: 0x04001AE5 RID: 6885
	private Collider2D bodyCollider;

	// Token: 0x04001AE6 RID: 6886
	private const int SweepLayerMask = 256;

	// Token: 0x020015EF RID: 5615
	public interface IRecoilMultiplier
	{
		// Token: 0x0600887A RID: 34938
		float GetRecoilMultiplier();
	}

	// Token: 0x020015F0 RID: 5616
	private enum States
	{
		// Token: 0x04008939 RID: 35129
		Ready,
		// Token: 0x0400893A RID: 35130
		Frozen,
		// Token: 0x0400893B RID: 35131
		Recoiling
	}

	// Token: 0x020015F1 RID: 5617
	// (Invoke) Token: 0x0600887C RID: 34940
	public delegate void FreezeEvent();

	// Token: 0x020015F2 RID: 5618
	// (Invoke) Token: 0x06008880 RID: 34944
	public delegate void CancelRecoilEvent();
}
