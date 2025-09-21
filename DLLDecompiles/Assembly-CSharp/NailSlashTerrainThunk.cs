using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class NailSlashTerrainThunk : MonoBehaviour
{
	// Token: 0x1400002F RID: 47
	// (add) Token: 0x06000FD8 RID: 4056 RVA: 0x0004C5D4 File Offset: 0x0004A7D4
	// (remove) Token: 0x06000FD9 RID: 4057 RVA: 0x0004C608 File Offset: 0x0004A808
	public static event NailSlashTerrainThunk.TerrainThunkEvent AnyThunked;

	// Token: 0x14000030 RID: 48
	// (add) Token: 0x06000FDA RID: 4058 RVA: 0x0004C63C File Offset: 0x0004A83C
	// (remove) Token: 0x06000FDB RID: 4059 RVA: 0x0004C674 File Offset: 0x0004A874
	public event Action<Vector2> Thunked;

	// Token: 0x06000FDC RID: 4060 RVA: 0x0004C6A9 File Offset: 0x0004A8A9
	private void OnValidate()
	{
		if (this.cooldown < 0f)
		{
			this.cooldown = 0f;
		}
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x0004C6C4 File Offset: 0x0004A8C4
	private void Awake()
	{
		this.OnValidate();
		this.col = base.GetComponent<Collider2D>();
		if (!this.col)
		{
			return;
		}
		if (!this.col.isTrigger)
		{
			this.col.forceSendLayers = 0;
			this.col.forceReceiveLayers = 0;
		}
		if (this.generateChild)
		{
			GameObject gameObject = new GameObject("Terrain Thunker");
			gameObject.layer = 16;
			gameObject.transform.SetParentReset(base.transform);
			((Collider2D)gameObject.AddComponent(this.col.GetType())).CopyFrom(this.col);
			NailSlashTerrainThunk nailSlashTerrainThunk = gameObject.AddComponent<NailSlashTerrainThunk>();
			nailSlashTerrainThunk.doRecoil = this.doRecoil;
			nailSlashTerrainThunk.overrideTransform = this.overrideTransform;
			nailSlashTerrainThunk.multiHitter = this.multiHitter;
			nailSlashTerrainThunk.cooldown = this.cooldown;
			Object.Destroy(this);
			return;
		}
		this.hc = base.transform.GetComponentInParent<HeroController>();
		this.body = base.GetComponent<Rigidbody2D>();
		if (!this.body && this.col)
		{
			this.col.isTrigger = false;
			this.col.forceSendLayers = 0;
			this.col.forceReceiveLayers = 0;
			this.body = base.gameObject.AddComponent<Rigidbody2D>();
			this.body.bodyType = RigidbodyType2D.Kinematic;
			this.body.simulated = true;
			this.body.useFullKinematicContacts = true;
			return;
		}
		base.enabled = false;
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x0004C849 File Offset: 0x0004AA49
	private void OnEnable()
	{
		this.collisionQueueStepsLeft = 2;
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x0004C852 File Offset: 0x0004AA52
	private void OnDisable()
	{
		this.queuedCollisionGameObject = null;
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x0004C85B File Offset: 0x0004AA5B
	private void Start()
	{
		if (this.multiHitter && this.multiHitter.multiHitter)
		{
			this.multiHitter.MultiHitEvaluated += this.OnMultiHitEvaluated;
		}
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x0004C88E File Offset: 0x0004AA8E
	private void FixedUpdate()
	{
		if (this.collisionQueueStepsLeft <= 0)
		{
			return;
		}
		this.collisionQueueStepsLeft--;
		if (this.collisionQueueStepsLeft > 0 || this.queuedCollisionGameObject == null)
		{
			return;
		}
		this.HandleQueuedCollision();
		this.queuedCollisionGameObject = null;
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x0004C8D0 File Offset: 0x0004AAD0
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.handleTink)
		{
			TinkEffect component = collision.collider.GetComponent<TinkEffect>();
			if (component)
			{
				Vector2 zero = Vector2.zero;
				if (this.multiHitter)
				{
					component.TryDoTinkReactionNoDamager(this.multiHitter.gameObject, true, true, true, out zero);
				}
				else
				{
					component.TryDoTinkReactionNoDamager(collision.otherCollider.gameObject, true, true, true, out zero);
				}
				Action<Vector2> thunked = this.Thunked;
				if (thunked == null)
				{
					return;
				}
				thunked(zero);
				return;
			}
		}
		this.HandleCollision(collision);
		if (this.multiHitter && !this.multiHitter.multiHitter)
		{
			LagHitOptions lagHits = this.multiHitter.LagHits;
			if (lagHits != null && lagHits.ShouldDoLagHits())
			{
				base.StartCoroutine(this.RepeatCollision(collision, lagHits.HitCount, lagHits.StartDelay, lagHits.HitDelay));
			}
		}
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x0004C9A8 File Offset: 0x0004ABA8
	private void HandleCollision(Collision2D collision)
	{
		if (this.lastThunkFrame == Time.frameCount)
		{
			return;
		}
		if (Time.timeAsDouble < this.cooldownTime)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		if (!gameObject)
		{
			return;
		}
		ForceThunker component = gameObject.GetComponent<ForceThunker>();
		if (gameObject.layer != 8 && !component)
		{
			return;
		}
		if (this.collisionQueueStepsLeft > 0)
		{
			this.queuedCollisionGameObject = gameObject;
			this.queuedCollisionCount = collision.GetContacts(this.queuedContactsTempStore);
			return;
		}
		this.lastThunkFrame = Time.frameCount;
		TerrainThunkUtils.SlashDirection slashDirection;
		if (this.overrideSlashDirection)
		{
			slashDirection = this.slashDirectionOverride;
			if (base.transform.lossyScale.x < 0f)
			{
				TerrainThunkUtils.SlashDirection slashDirection2;
				if (slashDirection != TerrainThunkUtils.SlashDirection.Left)
				{
					if (slashDirection != TerrainThunkUtils.SlashDirection.Right)
					{
						slashDirection2 = slashDirection;
					}
					else
					{
						slashDirection2 = TerrainThunkUtils.SlashDirection.Left;
					}
				}
				else
				{
					slashDirection2 = TerrainThunkUtils.SlashDirection.Right;
				}
				slashDirection = slashDirection2;
			}
		}
		else if (this.hc.cState.upAttacking)
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Up;
		}
		else if (this.hc.cState.downAttacking)
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Down;
			this.hc.crestAttacksFSM.SendEvent("THUNKED GROUND");
		}
		else if (this.hc.cState.facingRight)
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Right;
		}
		else
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Left;
		}
		this.cooldownTime = Time.timeAsDouble + (double)this.cooldown;
		int num;
		int num2;
		Vector2 vector = TerrainThunkUtils.GenerateTerrainThunk(collision, this.contactsTempStore, slashDirection, this.overrideTransform ? this.overrideTransform.position : this.hc.transform.position, out num, out num2, null);
		if (!this.doRecoil || num < 0)
		{
			return;
		}
		if (this.hc.cState.downAttacking)
		{
			if (component && component.PreventDownBounce)
			{
				return;
			}
			this.hc.crestAttacksFSM.SendEvent("THUNKED DOWN");
		}
		else if (this.hc.cState.upAttacking)
		{
			if (num2 == 1 || num2 == 3)
			{
				this.hc.RecoilDown();
			}
		}
		else if (num2 == 2 || num2 == 0)
		{
			this.hc.SetAllowRecoilWhileRelinquished(true);
			if (this.hc.cState.facingRight)
			{
				this.hc.RecoilLeft();
			}
			else
			{
				this.hc.RecoilRight();
			}
			this.hc.SetAllowRecoilWhileRelinquished(false);
		}
		Action<Vector2> thunked = this.Thunked;
		if (thunked != null)
		{
			thunked(vector);
		}
		NailSlashTerrainThunk.TerrainThunkEvent anyThunked = NailSlashTerrainThunk.AnyThunked;
		if (anyThunked == null)
		{
			return;
		}
		anyThunked(vector, num2);
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x0004CBF8 File Offset: 0x0004ADF8
	private void HandleQueuedCollision()
	{
		if (this.lastThunkFrame == Time.frameCount)
		{
			return;
		}
		if (Time.timeAsDouble < this.cooldownTime)
		{
			return;
		}
		if (this.queuedCollisionGameObject == null)
		{
			return;
		}
		ForceThunker component = this.queuedCollisionGameObject.GetComponent<ForceThunker>();
		this.lastThunkFrame = Time.frameCount;
		TerrainThunkUtils.SlashDirection slashDirection;
		if (this.overrideSlashDirection)
		{
			slashDirection = this.slashDirectionOverride;
			if (base.transform.lossyScale.x < 0f)
			{
				TerrainThunkUtils.SlashDirection slashDirection2;
				if (slashDirection != TerrainThunkUtils.SlashDirection.Left)
				{
					if (slashDirection != TerrainThunkUtils.SlashDirection.Right)
					{
						slashDirection2 = slashDirection;
					}
					else
					{
						slashDirection2 = TerrainThunkUtils.SlashDirection.Left;
					}
				}
				else
				{
					slashDirection2 = TerrainThunkUtils.SlashDirection.Right;
				}
				slashDirection = slashDirection2;
			}
		}
		else if (this.hc.cState.upAttacking)
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Up;
		}
		else if (this.hc.cState.downAttacking)
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Down;
			this.hc.crestAttacksFSM.SendEvent("THUNKED GROUND");
		}
		else if (this.hc.cState.facingRight)
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Right;
		}
		else
		{
			slashDirection = TerrainThunkUtils.SlashDirection.Left;
		}
		this.cooldownTime = Time.timeAsDouble + (double)this.cooldown;
		int num;
		int num2;
		Vector2 vector = TerrainThunkUtils.GenerateTerrainThunk(this.queuedCollisionCount, this.queuedContactsTempStore, slashDirection, this.overrideTransform ? this.overrideTransform.position : this.hc.transform.position, out num, out num2, null);
		if (!this.doRecoil || num < 0)
		{
			return;
		}
		if (this.hc.cState.downAttacking)
		{
			if (component && component.PreventDownBounce)
			{
				return;
			}
			this.hc.crestAttacksFSM.SendEvent("THUNKED DOWN");
		}
		else if (this.hc.cState.upAttacking)
		{
			if (num2 == 1 || num2 == 3)
			{
				this.hc.RecoilDown();
			}
		}
		else if (num2 == 2 || num2 == 0)
		{
			this.hc.SetAllowRecoilWhileRelinquished(true);
			if (this.hc.cState.facingRight)
			{
				this.hc.RecoilLeft();
			}
			else
			{
				this.hc.RecoilRight();
			}
			this.hc.SetAllowRecoilWhileRelinquished(false);
		}
		Action<Vector2> thunked = this.Thunked;
		if (thunked != null)
		{
			thunked(vector);
		}
		NailSlashTerrainThunk.TerrainThunkEvent anyThunked = NailSlashTerrainThunk.AnyThunked;
		if (anyThunked == null)
		{
			return;
		}
		anyThunked(vector, num2);
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x0004CE1B File Offset: 0x0004B01B
	private IEnumerator RepeatCollision(Collision2D collision, int repeatCount, float startDelay, float repeatDelay)
	{
		yield return new WaitForSeconds(startDelay);
		WaitForSeconds wait = new WaitForSeconds(repeatDelay);
		int num;
		for (int i = 0; i < repeatCount; i = num + 1)
		{
			this.HandleCollision(collision);
			yield return wait;
			num = i;
		}
		yield break;
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x0004CE47 File Offset: 0x0004B047
	private void OnMultiHitEvaluated()
	{
		if (!this.col || !this.col.enabled)
		{
			return;
		}
		this.col.enabled = false;
		this.col.enabled = true;
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x0004CE7C File Offset: 0x0004B07C
	public bool WillHandleTink(Collider2D otherCol)
	{
		if (!this.handleTink)
		{
			return false;
		}
		int layerCollisionMask = Physics2D.GetLayerCollisionMask(this.col.gameObject.layer);
		int num = 1 << otherCol.gameObject.layer;
		return (layerCollisionMask & num) != 0;
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x0004CEBE File Offset: 0x0004B0BE
	public static void ReportDownspikeHitGround(Vector2 hitPoint)
	{
		NailSlashTerrainThunk.TerrainThunkEvent anyThunked = NailSlashTerrainThunk.AnyThunked;
		if (anyThunked == null)
		{
			return;
		}
		anyThunked(hitPoint, 1);
	}

	// Token: 0x04000F6B RID: 3947
	[SerializeField]
	private bool generateChild;

	// Token: 0x04000F6C RID: 3948
	[Space]
	[SerializeField]
	private bool doRecoil;

	// Token: 0x04000F6D RID: 3949
	[SerializeField]
	private Transform overrideTransform;

	// Token: 0x04000F6E RID: 3950
	[SerializeField]
	private DamageEnemies multiHitter;

	// Token: 0x04000F6F RID: 3951
	[SerializeField]
	private float cooldown;

	// Token: 0x04000F70 RID: 3952
	[SerializeField]
	private bool overrideSlashDirection;

	// Token: 0x04000F71 RID: 3953
	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideSlashDirection", true, false, false)]
	private TerrainThunkUtils.SlashDirection slashDirectionOverride;

	// Token: 0x04000F72 RID: 3954
	[SerializeField]
	private bool handleTink;

	// Token: 0x04000F73 RID: 3955
	private double cooldownTime;

	// Token: 0x04000F74 RID: 3956
	private readonly ContactPoint2D[] contactsTempStore = new ContactPoint2D[10];

	// Token: 0x04000F75 RID: 3957
	private readonly ContactPoint2D[] queuedContactsTempStore = new ContactPoint2D[10];

	// Token: 0x04000F76 RID: 3958
	private int lastThunkFrame;

	// Token: 0x04000F77 RID: 3959
	private HeroController hc;

	// Token: 0x04000F78 RID: 3960
	private Rigidbody2D body;

	// Token: 0x04000F79 RID: 3961
	private Collider2D col;

	// Token: 0x04000F7A RID: 3962
	private int collisionQueueStepsLeft;

	// Token: 0x04000F7B RID: 3963
	private GameObject queuedCollisionGameObject;

	// Token: 0x04000F7C RID: 3964
	private int queuedCollisionCount;

	// Token: 0x020014D7 RID: 5335
	// (Invoke) Token: 0x060084FF RID: 34047
	public delegate void TerrainThunkEvent(Vector2 thunkPos, int surfaceDir);
}
