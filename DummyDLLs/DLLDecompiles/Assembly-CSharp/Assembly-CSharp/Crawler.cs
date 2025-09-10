using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002BE RID: 702
[RequireComponent(typeof(BoxCollider2D))]
public class Crawler : MonoBehaviour, IEnemyMessageReceiver
{
	// Token: 0x17000289 RID: 649
	// (get) Token: 0x060018C4 RID: 6340 RVA: 0x0007184B File Offset: 0x0006FA4B
	// (set) Token: 0x060018C5 RID: 6341 RVA: 0x00071853 File Offset: 0x0006FA53
	public bool IsTurning { get; private set; }

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x060018C6 RID: 6342 RVA: 0x0007185C File Offset: 0x0006FA5C
	public bool IsCrawling
	{
		get
		{
			return this.crawlRoutine != null;
		}
	}

	// Token: 0x1700028B RID: 651
	// (get) Token: 0x060018C7 RID: 6343 RVA: 0x00071867 File Offset: 0x0006FA67
	// (set) Token: 0x060018C8 RID: 6344 RVA: 0x0007186F File Offset: 0x0006FA6F
	public float Speed
	{
		get
		{
			return this.speed;
		}
		set
		{
			this.speed = value;
			this.UpdateCrawlerType();
		}
	}

	// Token: 0x1700028C RID: 652
	// (get) Token: 0x060018C9 RID: 6345 RVA: 0x0007187E File Offset: 0x0006FA7E
	// (set) Token: 0x060018CA RID: 6346 RVA: 0x0007189E File Offset: 0x0006FA9E
	public Crawler.CrawlerTypes Type
	{
		get
		{
			if (this.type == null)
			{
				this.UpdateCrawlerType();
			}
			return this.type.Value;
		}
		private set
		{
			this.type = new Crawler.CrawlerTypes?(value);
		}
	}

	// Token: 0x1700028D RID: 653
	// (get) Token: 0x060018CB RID: 6347 RVA: 0x000718AC File Offset: 0x0006FAAC
	// (set) Token: 0x060018CC RID: 6348 RVA: 0x000718CC File Offset: 0x0006FACC
	public Crawler.TerrainSides TerrainSide
	{
		get
		{
			if (this.terrainSide == null)
			{
				this.UpdateCrawlerType();
			}
			return this.terrainSide.Value;
		}
		private set
		{
			this.terrainSide = new Crawler.TerrainSides?(value);
		}
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x000718DC File Offset: 0x0006FADC
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.recoil = base.GetComponent<Recoil>();
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.sprite = base.GetComponent<tk2dSprite>();
		this.collider = base.GetComponent<BoxCollider2D>();
		if (this.ambientIdle)
		{
			this.healthManager = base.GetComponent<HealthManager>();
			if (this.healthManager != null)
			{
				this.healthManager.TookDamage += this.EndAmbientIdle;
			}
		}
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x0007195E File Offset: 0x0006FB5E
	private void OnEnable()
	{
		if (!this.startInactive)
		{
			this.StartCrawling();
			return;
		}
		this.StopSound();
		if (this.ambientLoopAudioSource)
		{
			this.ambientLoopAudioSource.Stop();
		}
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x0007198D File Offset: 0x0006FB8D
	private void OnDisable()
	{
		this.StopCrawling();
	}

	// Token: 0x060018D0 RID: 6352 RVA: 0x00071995 File Offset: 0x0006FB95
	private IEnumerator Crawl()
	{
		double lastLoopTime = 0.0;
		this.anim.Play(this.crawlAnimName);
		for (;;)
		{
			this.StartSound();
			for (;;)
			{
				Crawler.CrawlerTypes crawlerTypes = this.Type;
				if (crawlerTypes > Crawler.CrawlerTypes.Roof)
				{
					if (crawlerTypes != Crawler.CrawlerTypes.Wall)
					{
						goto Block_2;
					}
					Rigidbody2D rigidbody2D = this.body;
					float? y = new float?(this.velocity.y);
					rigidbody2D.SetVelocity(null, y);
				}
				else
				{
					Rigidbody2D rigidbody2D2 = this.body;
					float? x = new float?(this.velocity.x);
					float? y = null;
					rigidbody2D2.SetVelocity(x, y);
				}
				if (this.ShouldTurn(false))
				{
					break;
				}
				while (this.recoil.IsRecoiling)
				{
					yield return this.updateWait;
				}
				yield return this.updateWait;
			}
			IEnumerator turn = this.Turn();
			while (turn.MoveNext())
			{
				object obj = turn.Current;
				CustomYieldInstruction customYieldInstruction = obj as CustomYieldInstruction;
				if (customYieldInstruction != null && !customYieldInstruction.keepWaiting)
				{
					break;
				}
				yield return obj;
			}
			if (Math.Abs(Time.timeAsDouble - lastLoopTime) <= (double)Mathf.Epsilon)
			{
				yield return this.updateWait;
			}
			lastLoopTime = Time.timeAsDouble;
			turn = null;
		}
		Block_2:
		throw new ArgumentOutOfRangeException();
		yield break;
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x000719A4 File Offset: 0x0006FBA4
	private void PlayExtraAnim(string animName)
	{
		this.anim.Play(animName);
		this.currentClipWait = new WaitForTk2dAnimatorClipFinish(this.anim, delegate(tk2dSpriteAnimator _, tk2dSpriteAnimationClip _)
		{
			this.anim.Play(this.crawlAnimName);
		});
	}

	// Token: 0x060018D2 RID: 6354 RVA: 0x000719CF File Offset: 0x0006FBCF
	private IEnumerator Turn()
	{
		this.IsTurning = true;
		Crawler.CrawlerTypes crawlerTypes = this.Type;
		if (crawlerTypes > Crawler.CrawlerTypes.Roof)
		{
			if (crawlerTypes == Crawler.CrawlerTypes.Wall)
			{
				this.body.linearVelocity = new Vector3(this.body.linearVelocity.x, 0f);
			}
		}
		else
		{
			this.body.linearVelocity = new Vector3(0f, this.body.linearVelocity.y);
		}
		if (this.flipBeforeTurn)
		{
			this.FlipScaleX();
			if (this.doTurnAnim)
			{
				if (this.moveWhileTurning)
				{
					this.PlayExtraAnim(this.turnAnimName);
				}
				else
				{
					this.StopSound();
					this.anim.Play(this.turnAnimName);
					this.currentClipWait = new WaitForTk2dAnimatorClipFinish(this.anim, delegate(tk2dSpriteAnimator _, tk2dSpriteAnimationClip _)
					{
						this.anim.Play(this.crawlAnimName);
					});
					yield return this.currentClipWait;
					this.StartSound();
				}
			}
		}
		else if (this.doTurnAnim)
		{
			if (this.moveWhileTurning)
			{
				this.PlayExtraAnim(this.turnAnimName);
				this.FlipScaleX();
			}
			else
			{
				this.StopSound();
				this.anim.Play(this.turnAnimName);
				this.currentClipWait = new WaitForTk2dAnimatorClipFinish(this.anim, delegate(tk2dSpriteAnimator _, tk2dSpriteAnimationClip _)
				{
					this.FlipScaleX();
					this.anim.Play(this.crawlAnimName);
				});
				yield return this.currentClipWait;
				this.StartSound();
			}
		}
		else
		{
			this.FlipScaleX();
		}
		this.velocity.x = this.velocity.x * -1f;
		this.velocity.y = this.velocity.y * -1f;
		this.IsTurning = false;
		yield break;
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x000719E0 File Offset: 0x0006FBE0
	private void FlipScaleX()
	{
		base.transform.SetScaleX(base.transform.localScale.x * -1f);
		if (this.sprite && this.keepSpriteFacing)
		{
			this.sprite.FlipX = (base.transform.localScale.x < 0f);
		}
	}

	// Token: 0x060018D4 RID: 6356 RVA: 0x00071A48 File Offset: 0x0006FC48
	private bool ShouldTurn(bool drawGizmos = false)
	{
		if (this.isTurnScheduled)
		{
			this.isTurnScheduled = false;
			return true;
		}
		Vector2 offset = this.collider.offset;
		Vector2 self = this.collider.size / 2f;
		int num = this.spriteFacesRight ? 1 : -1;
		Vector2 vector = offset - self.MultiplyElements(new float?((float)num), null);
		Vector2 vector2 = offset + self.MultiplyElements(new float?((float)num), null);
		Vector2 vector3 = new Vector2(vector2.x - 0.1f * (float)num, vector.y + 0.01f);
		Vector2 vector4 = Vector2.right.MultiplyElements(new float?((float)num), null);
		float num2 = Mathf.Max(this.rayFrontDistance, Mathf.Abs(this.velocity.x * Time.deltaTime)) + 0.1f;
		float y = vector.y + 0.5f + 0.1f;
		Vector2 vector5 = new Vector2(vector2.x + this.rayDownFrontPadding * (float)num, y);
		Vector2 vector6 = new Vector2(vector.x, y);
		Vector2 down = Vector2.down;
		float num3 = 1.1f;
		if (drawGizmos)
		{
			Gizmos.DrawLine(vector3, vector3 + vector4 * num2);
			Gizmos.DrawLine(vector5, vector5 + down * num3);
			Gizmos.DrawLine(vector6, vector6 + down * num3);
		}
		else
		{
			if (!this.IsRayHittingLocal(vector6, down, num3))
			{
				return false;
			}
			if (this.IsRayHittingLocal(vector3, vector4, num2))
			{
				return true;
			}
			if (!this.IsRayHittingLocal(vector5, down, num3))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060018D5 RID: 6357 RVA: 0x00071C1A File Offset: 0x0006FE1A
	public bool IsRayHittingLocal(Vector2 originLocal, Vector2 directionLocal, float length)
	{
		return base.transform.IsRayHittingLocalNoTriggers(originLocal, directionLocal, length, 33024);
	}

	// Token: 0x060018D6 RID: 6358 RVA: 0x00071C30 File Offset: 0x0006FE30
	private void OnDrawGizmosSelected()
	{
		if (this.collider == null)
		{
			this.collider = base.GetComponent<BoxCollider2D>();
		}
		if (this.collider == null)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		this.ShouldTurn(true);
	}

	// Token: 0x060018D7 RID: 6359 RVA: 0x00071C7E File Offset: 0x0006FE7E
	public void StartCrawling()
	{
		this.StartCrawling(false);
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x00071C88 File Offset: 0x0006FE88
	public void StartCrawling(bool scheduleTurn)
	{
		this.isTurnScheduled = scheduleTurn;
		if (!this.isSetup)
		{
			this.isSetup = true;
			this.recoil.OnCancelRecoil += delegate()
			{
				if (this.IsCrawling)
				{
					this.body.linearVelocity = this.velocity;
				}
			};
		}
		this.UpdateCrawlerType();
		Crawler.CrawlerTypes crawlerTypes = this.Type;
		if (crawlerTypes != Crawler.CrawlerTypes.Floor)
		{
			if (crawlerTypes - Crawler.CrawlerTypes.Roof <= 1)
			{
				this.body.gravityScale = 0f;
				if (!this.allowRoofRecoil)
				{
					this.recoil.FreezeInPlace = true;
					this.recoil.SetRecoilSpeed(0f);
				}
				else
				{
					this.recoil.IsDownBlocked = true;
					this.recoil.IsUpBlocked = true;
				}
			}
		}
		else
		{
			this.body.gravityScale = 1f;
			this.recoil.FreezeInPlace = false;
		}
		if (!this.ambientIdle)
		{
			this.EndAmbientIdle();
			return;
		}
		this.anim.Play(this.idleAnimName);
		this.StopSound();
		if (this.ambientLoopAudioSource)
		{
			this.ambientLoopAudioSource.Play();
		}
	}

	// Token: 0x060018D9 RID: 6361 RVA: 0x00071D84 File Offset: 0x0006FF84
	private void UpdateCrawlerType()
	{
		Transform transform = base.transform;
		float z = transform.eulerAngles.z;
		Vector3 localScale = transform.localScale;
		float num = Mathf.Sign(localScale.x) * (float)(this.spriteFacesRight ? 1 : -1);
		if (z <= 225f)
		{
			if (z >= 45f)
			{
				if (z > 135f)
				{
					this.Type = ((localScale.y > 0f) ? Crawler.CrawlerTypes.Roof : Crawler.CrawlerTypes.Floor);
					this.velocity = new Vector2(-num * this.speed, 0f);
					this.TerrainSide = ((localScale.y > 0f) ? Crawler.TerrainSides.Roof : Crawler.TerrainSides.Floor);
					goto IL_162;
				}
				this.Type = Crawler.CrawlerTypes.Wall;
				this.velocity = new Vector2(0f, num * this.speed);
				this.TerrainSide = ((localScale.y > 0f) ? Crawler.TerrainSides.RightWall : Crawler.TerrainSides.LeftWall);
				goto IL_162;
			}
		}
		else if (z <= 315f)
		{
			this.Type = Crawler.CrawlerTypes.Wall;
			this.velocity = new Vector2(0f, -num * this.speed);
			this.TerrainSide = ((localScale.y > 0f) ? Crawler.TerrainSides.LeftWall : Crawler.TerrainSides.RightWall);
			goto IL_162;
		}
		this.Type = ((localScale.y > 0f) ? Crawler.CrawlerTypes.Floor : Crawler.CrawlerTypes.Roof);
		this.velocity = new Vector2(num * this.speed, 0f);
		this.TerrainSide = ((localScale.y > 0f) ? Crawler.TerrainSides.Floor : Crawler.TerrainSides.Roof);
		IL_162:
		this.recoil.IsUpBlocked = false;
		this.recoil.IsDownBlocked = false;
		this.recoil.IsLeftBlocked = false;
		this.recoil.IsRightBlocked = false;
		this.body.constraints = RigidbodyConstraints2D.FreezeRotation;
		switch (this.TerrainSide)
		{
		case Crawler.TerrainSides.Floor:
			return;
		case Crawler.TerrainSides.Roof:
			this.recoil.IsUpBlocked = true;
			this.recoil.IsDownBlocked = true;
			this.body.constraints |= RigidbodyConstraints2D.FreezePositionY;
			return;
		case Crawler.TerrainSides.RightWall:
		case Crawler.TerrainSides.LeftWall:
			this.recoil.IsLeftBlocked = true;
			this.recoil.IsRightBlocked = true;
			this.body.constraints |= RigidbodyConstraints2D.FreezePositionX;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060018DA RID: 6362 RVA: 0x00071FAC File Offset: 0x000701AC
	public void StopCrawling()
	{
		this.StopSound();
		if (this.crawlRoutine != null)
		{
			base.StopCoroutine(this.crawlRoutine);
			Crawler.CrawlerTypes crawlerTypes = this.Type;
			if (crawlerTypes > Crawler.CrawlerTypes.Roof)
			{
				if (crawlerTypes != Crawler.CrawlerTypes.Wall)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.body.linearVelocity = new Vector3(this.body.linearVelocity.x, 0f);
			}
			else
			{
				this.body.linearVelocity = new Vector3(0f, this.body.linearVelocity.y);
			}
			this.crawlRoutine = null;
		}
		this.body.constraints = RigidbodyConstraints2D.FreezeRotation;
		WaitForTk2dAnimatorClipFinish waitForTk2dAnimatorClipFinish = this.currentClipWait;
		if (waitForTk2dAnimatorClipFinish == null)
		{
			return;
		}
		waitForTk2dAnimatorClipFinish.Cancel();
	}

	// Token: 0x060018DB RID: 6363 RVA: 0x00072068 File Offset: 0x00070268
	public void EndAmbientIdle()
	{
		this.ambientIdle = false;
		if (this.ambientLoopAudioSource)
		{
			this.ambientLoopAudioSource.Stop();
		}
		if (this.crawlRoutine == null)
		{
			this.crawlRoutine = base.StartCoroutine(this.Crawl());
		}
	}

	// Token: 0x060018DC RID: 6364 RVA: 0x000720A3 File Offset: 0x000702A3
	private void StartSound()
	{
		if (!this.loopAudioSource)
		{
			return;
		}
		this.loopAudioSource.Play();
		if (this.pauseLoopDontStop)
		{
			this.loopAudioSource.UnPause();
		}
	}

	// Token: 0x060018DD RID: 6365 RVA: 0x000720D1 File Offset: 0x000702D1
	private void StopSound()
	{
		if (!this.loopAudioSource)
		{
			return;
		}
		if (this.pauseLoopDontStop)
		{
			this.loopAudioSource.Pause();
			return;
		}
		this.loopAudioSource.Stop();
	}

	// Token: 0x060018DE RID: 6366 RVA: 0x00072100 File Offset: 0x00070300
	public void ReceiveEnemyMessage(string eventName)
	{
		if (this.IsTurning)
		{
			return;
		}
		if (!this.IsCrawling)
		{
			return;
		}
		Vector2 linearVelocity = this.body.linearVelocity;
		Crawler.CrawlerTypes crawlerTypes = this.Type;
		if (crawlerTypes > Crawler.CrawlerTypes.Roof)
		{
			if (crawlerTypes != Crawler.CrawlerTypes.Wall)
			{
				return;
			}
			if (linearVelocity.y > 0f)
			{
				if (eventName == "GO DOWN")
				{
					this.isTurnScheduled = true;
					return;
				}
			}
			else if (eventName == "GO UP")
			{
				this.isTurnScheduled = true;
			}
		}
		else if (linearVelocity.x > 0f)
		{
			if (eventName == "GO LEFT")
			{
				this.isTurnScheduled = true;
				return;
			}
		}
		else if (eventName == "GO RIGHT")
		{
			this.isTurnScheduled = true;
			return;
		}
	}

	// Token: 0x040017B8 RID: 6072
	private const float RAY_SKIN_WIDTH = 0.1f;

	// Token: 0x040017B9 RID: 6073
	private const float RAY_FRONT_HEIGHT = 0.01f;

	// Token: 0x040017BA RID: 6074
	private const float RAY_DOWN_HEIGHT = 0.5f;

	// Token: 0x040017BB RID: 6075
	private const float RAY_DOWN_DISTANCE = 1f;

	// Token: 0x040017BC RID: 6076
	[SerializeField]
	private float speed = 2f;

	// Token: 0x040017BD RID: 6077
	[Space]
	[SerializeField]
	private float rayFrontDistance = 0.2f;

	// Token: 0x040017BE RID: 6078
	[SerializeField]
	private float rayDownFrontPadding = 0.2f;

	// Token: 0x040017BF RID: 6079
	[Space]
	[SerializeField]
	public string crawlAnimName = "Walk";

	// Token: 0x040017C0 RID: 6080
	[SerializeField]
	private bool spriteFacesRight;

	// Token: 0x040017C1 RID: 6081
	[SerializeField]
	private bool flipBeforeTurn;

	// Token: 0x040017C2 RID: 6082
	[SerializeField]
	private bool moveWhileTurning;

	// Token: 0x040017C3 RID: 6083
	[SerializeField]
	private bool doTurnAnim = true;

	// Token: 0x040017C4 RID: 6084
	[SerializeField]
	[ModifiableProperty]
	[Conditional("doTurnAnim", true, false, false)]
	public string turnAnimName = "Turn";

	// Token: 0x040017C5 RID: 6085
	[SerializeField]
	private bool keepSpriteFacing;

	// Token: 0x040017C6 RID: 6086
	[SerializeField]
	private bool startInactive;

	// Token: 0x040017C7 RID: 6087
	[SerializeField]
	private bool ambientIdle;

	// Token: 0x040017C8 RID: 6088
	[SerializeField]
	private bool allowRoofRecoil;

	// Token: 0x040017C9 RID: 6089
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ambientIdle", true, false, false)]
	private string idleAnimName = "Ambient Idle";

	// Token: 0x040017CA RID: 6090
	[SerializeField]
	private AudioSource loopAudioSource;

	// Token: 0x040017CB RID: 6091
	[SerializeField]
	private bool pauseLoopDontStop;

	// Token: 0x040017CC RID: 6092
	[SerializeField]
	private AudioSource ambientLoopAudioSource;

	// Token: 0x040017CD RID: 6093
	private Vector2 velocity;

	// Token: 0x040017CE RID: 6094
	private Coroutine crawlRoutine;

	// Token: 0x040017CF RID: 6095
	private WaitForTk2dAnimatorClipFinish currentClipWait;

	// Token: 0x040017D0 RID: 6096
	private readonly WaitForFixedUpdate updateWait = new WaitForFixedUpdate();

	// Token: 0x040017D1 RID: 6097
	private bool isSetup;

	// Token: 0x040017D2 RID: 6098
	private bool isTurnScheduled;

	// Token: 0x040017D3 RID: 6099
	private Rigidbody2D body;

	// Token: 0x040017D4 RID: 6100
	private Recoil recoil;

	// Token: 0x040017D5 RID: 6101
	private tk2dSpriteAnimator anim;

	// Token: 0x040017D6 RID: 6102
	private HealthManager healthManager;

	// Token: 0x040017D7 RID: 6103
	private tk2dSprite sprite;

	// Token: 0x040017D8 RID: 6104
	private BoxCollider2D collider;

	// Token: 0x040017DA RID: 6106
	private Crawler.CrawlerTypes? type;

	// Token: 0x040017DB RID: 6107
	private Crawler.TerrainSides? terrainSide;

	// Token: 0x020015A4 RID: 5540
	public enum CrawlerTypes
	{
		// Token: 0x0400881A RID: 34842
		Floor,
		// Token: 0x0400881B RID: 34843
		Roof,
		// Token: 0x0400881C RID: 34844
		Wall
	}

	// Token: 0x020015A5 RID: 5541
	public enum TerrainSides
	{
		// Token: 0x0400881E RID: 34846
		Floor,
		// Token: 0x0400881F RID: 34847
		Roof,
		// Token: 0x04008820 RID: 34848
		RightWall,
		// Token: 0x04008821 RID: 34849
		LeftWall
	}
}
