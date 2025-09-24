using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class CheckIsCharacterGrounded : FsmStateAction
{
	// Token: 0x06000141 RID: 321 RVA: 0x000078B4 File Offset: 0x00005AB4
	public override void Reset()
	{
		this.Target = null;
		this.RayCount = new FsmInt(3);
		this.GroundDistance = new FsmFloat(0.2f);
		this.SkinWidth = new FsmFloat(-0.05f);
		this.SkinHeight = new FsmFloat(0.1f);
		this.StoreResult = null;
		this.GroundedEvent = null;
		this.NotGroundedEvent = null;
		this.EveryFrame = false;
	}

	// Token: 0x06000142 RID: 322 RVA: 0x00007934 File Offset: 0x00005B34
	public override void OnEnter()
	{
		GameObject safe = this.Target.GetSafe(this);
		if (!safe)
		{
			return;
		}
		this.collider = safe.GetComponent<Collider2D>();
		this.hasCollider = this.collider;
		this.body = safe.GetComponent<Rigidbody2D>();
		this.hasRB = (this.body != null);
		if (!this.hasRB && this.hasCollider)
		{
			this.body = this.collider.attachedRigidbody;
			this.hasRB = (this.body != null);
		}
		this.hero = safe.GetComponent<HeroController>();
		this.isHero = this.hero;
		this.layerMask = 256;
		this.DoAction();
		if (!this.EveryFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x06000143 RID: 323 RVA: 0x00007A01 File Offset: 0x00005C01
	public override void OnUpdate()
	{
		this.DoAction();
	}

	// Token: 0x06000144 RID: 324 RVA: 0x00007A0C File Offset: 0x00005C0C
	private void DoAction()
	{
		if (this.hasRB && this.body.linearVelocity.y > 0.01f)
		{
			base.Fsm.Event(this.NotGroundedEvent);
			return;
		}
		bool flag = this.IsGrounded();
		this.StoreResult.Value = flag;
		base.Fsm.Event(flag ? this.GroundedEvent : this.NotGroundedEvent);
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00007A7C File Offset: 0x00005C7C
	private bool IsGrounded()
	{
		if (this.isHero)
		{
			return this.hero.CheckTouchingGround();
		}
		if (!this.hasCollider || !this.collider.enabled)
		{
			return false;
		}
		float value = this.SkinWidth.Value;
		float value2 = this.SkinHeight.Value;
		float length = (this.hasRB ? Mathf.Max(this.GroundDistance.Value, -this.body.linearVelocity.y * Time.deltaTime) : this.GroundDistance.Value) + value2;
		Bounds bounds = this.collider.bounds;
		Vector2 vector = bounds.min;
		float a = vector.x + value;
		float b = bounds.max.x - value;
		float y = vector.y + value2;
		int num = this.RayCount.Value - 1;
		for (int i = 0; i < this.RayCount.Value; i++)
		{
			if (Helper.IsRayHittingNoTriggers(new Vector2(Mathf.Lerp(a, b, (float)i / (float)num), y), Vector2.down, length, this.layerMask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040000DB RID: 219
	[CheckForComponent(typeof(Collider2D))]
	public FsmOwnerDefault Target;

	// Token: 0x040000DC RID: 220
	public FsmInt RayCount;

	// Token: 0x040000DD RID: 221
	public FsmFloat GroundDistance;

	// Token: 0x040000DE RID: 222
	public FsmFloat SkinWidth;

	// Token: 0x040000DF RID: 223
	public FsmFloat SkinHeight;

	// Token: 0x040000E0 RID: 224
	[UIHint(UIHint.Variable)]
	public FsmBool StoreResult;

	// Token: 0x040000E1 RID: 225
	public FsmEvent GroundedEvent;

	// Token: 0x040000E2 RID: 226
	public FsmEvent NotGroundedEvent;

	// Token: 0x040000E3 RID: 227
	public bool EveryFrame;

	// Token: 0x040000E4 RID: 228
	private Collider2D collider;

	// Token: 0x040000E5 RID: 229
	private Rigidbody2D body;

	// Token: 0x040000E6 RID: 230
	private bool hasCollider;

	// Token: 0x040000E7 RID: 231
	private bool hasRB;

	// Token: 0x040000E8 RID: 232
	private int layerMask;

	// Token: 0x040000E9 RID: 233
	private bool isHero;

	// Token: 0x040000EA RID: 234
	private HeroController hero;
}
