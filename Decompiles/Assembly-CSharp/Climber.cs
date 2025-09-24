using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020002A2 RID: 674
public class Climber : MonoBehaviour
{
	// Token: 0x17000272 RID: 626
	// (get) Token: 0x060017B4 RID: 6068 RVA: 0x0006B558 File Offset: 0x00069758
	public bool IsTurning
	{
		get
		{
			return this.turnRoutine != null;
		}
	}

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x060017B5 RID: 6069 RVA: 0x0006B564 File Offset: 0x00069764
	public bool IsFacingRight
	{
		get
		{
			bool flag = base.transform.localScale.x > 0f;
			if (!this.SpriteFacesRight)
			{
				flag = !flag;
			}
			return flag;
		}
	}

	// Token: 0x060017B6 RID: 6070 RVA: 0x0006B598 File Offset: 0x00069798
	public bool? DoesAnimExist(string animName)
	{
		if (string.IsNullOrEmpty(animName))
		{
			return null;
		}
		tk2dSpriteAnimator component = base.GetComponent<tk2dSpriteAnimator>();
		if (!component)
		{
			return new bool?(false);
		}
		return new bool?(component.GetClipByName(animName) != null);
	}

	// Token: 0x060017B7 RID: 6071 RVA: 0x0006B5DC File Offset: 0x000697DC
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.col = base.GetComponent<BoxCollider2D>();
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.recoil = base.GetComponent<Recoil>();
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x0006B60E File Offset: 0x0006980E
	private void Start()
	{
		this.StickToGround();
		this.Setup();
		this.isFirstSetupDone = true;
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x0006B623 File Offset: 0x00069823
	private void OnEnable()
	{
		if (this.isFirstSetupDone)
		{
			this.Setup();
		}
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x0006B634 File Offset: 0x00069834
	private void Setup()
	{
		Vector3 localScale = base.transform.localScale;
		float num = Mathf.Sign(localScale.x);
		if (!this.SpriteFacesRight)
		{
			num *= -1f;
		}
		this.clockwise = (num > 0f);
		this.currentDirection = this.GetCurrentMoveDirection(this.clockwise);
		if (localScale.y < 0f)
		{
			this.clockwise = !this.clockwise;
		}
		if (this.recoil)
		{
			this.recoil.SkipFreezingByController = true;
			this.recoil.OnHandleFreeze += this.Stun;
		}
		this.previousTurnPos = Vector2.zero;
		this.col.enabled = true;
		if (this.ConcaveDamagerOverride)
		{
			this.ConcaveDamagerOverride.SetActive(false);
		}
		if (this.ConvexDamagerOverride)
		{
			this.ConvexDamagerOverride.SetActive(false);
		}
		base.StartCoroutine(this.Walk());
	}

	// Token: 0x060017BB RID: 6075 RVA: 0x0006B72C File Offset: 0x0006992C
	public Climber.Direction GetCurrentMoveDirection(bool clockwise)
	{
		float num = base.transform.eulerAngles.z % 360f;
		if (num >= 45f && num <= 135f)
		{
			if (!clockwise)
			{
				return Climber.Direction.Down;
			}
			return Climber.Direction.Up;
		}
		else if (num >= 135f && num <= 225f)
		{
			if (!clockwise)
			{
				return Climber.Direction.Right;
			}
			return Climber.Direction.Left;
		}
		else if (num >= 225f && num <= 315f)
		{
			if (!clockwise)
			{
				return Climber.Direction.Up;
			}
			return Climber.Direction.Down;
		}
		else
		{
			if (!clockwise)
			{
				return Climber.Direction.Left;
			}
			return Climber.Direction.Right;
		}
	}

	// Token: 0x060017BC RID: 6076 RVA: 0x0006B79B File Offset: 0x0006999B
	private void OnDisable()
	{
		this.turnRoutine = null;
		if (this.recoil)
		{
			this.recoil.OnHandleFreeze -= this.Stun;
		}
		base.StopAllCoroutines();
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x0006B7CE File Offset: 0x000699CE
	private IEnumerator Walk()
	{
		this.anim.Play(this.WalkAnim);
		this.body.linearVelocity = this.GetVelocity(this.currentDirection);
		for (;;)
		{
			if (Vector3.Distance(this.previousTurnPos, base.transform.position) >= this.minTurnDistance)
			{
				if (this.CheckWall())
				{
					Climber.WallBehaviours wallBehaviours = this.WallBehaviour;
					if (wallBehaviours != Climber.WallBehaviours.Continue)
					{
						if (wallBehaviours == Climber.WallBehaviours.Turn)
						{
							yield return this.turnRoutine = base.StartCoroutine(this.TurnAround());
						}
					}
					else
					{
						yield return this.turnRoutine = base.StartCoroutine(this.TurnContinue(!this.clockwise, false, true));
					}
				}
				else if (!this.CheckGround())
				{
					Climber.WallBehaviours wallBehaviours = this.EdgeBehaviour;
					if (wallBehaviours != Climber.WallBehaviours.Continue)
					{
						if (wallBehaviours == Climber.WallBehaviours.Turn)
						{
							yield return this.turnRoutine = base.StartCoroutine(this.TurnAround());
						}
					}
					else
					{
						yield return this.turnRoutine = base.StartCoroutine(this.TurnContinue(this.clockwise, false, false));
					}
				}
			}
			yield return this.moveYield;
		}
		yield break;
	}

	// Token: 0x060017BE RID: 6078 RVA: 0x0006B7DD File Offset: 0x000699DD
	private IEnumerator TurnContinue(bool turnClockwise, bool isImmediate, bool tweenPos)
	{
		this.body.linearVelocity = Vector2.zero;
		float currentRotation = base.transform.eulerAngles.z;
		float targetRotation = currentRotation + (float)(turnClockwise ? -90 : 90);
		Vector2 currentPos = base.transform.position;
		Vector2 targetPos = currentPos + this.GetTweenPos(this.currentDirection);
		string turnAnimation = null;
		GameObject damager = null;
		bool flag = (turnClockwise && this.IsFacingRight) || (!turnClockwise && !this.IsFacingRight);
		if (base.transform.localScale.y < 0f)
		{
			flag = !flag;
		}
		if (flag)
		{
			string text = (isImmediate && !string.IsNullOrEmpty(this.ContinueConvexImmediateAnim)) ? this.ContinueConvexImmediateAnim : this.ContinueConvexRegularAnim;
			if (!string.IsNullOrEmpty(text))
			{
				turnAnimation = text;
				damager = this.ConvexDamagerOverride;
			}
		}
		else if (!flag)
		{
			string text2 = (isImmediate && !string.IsNullOrEmpty(this.ContinueConcaveImmediateAnim)) ? this.ContinueConcaveImmediateAnim : this.ContinueConcaveRegularAnim;
			if (!string.IsNullOrEmpty(text2))
			{
				turnAnimation = text2;
				damager = this.ConcaveDamagerOverride;
			}
		}
		if (turnAnimation == null)
		{
			for (float elapsed = 0f; elapsed < this.spinTime; elapsed += Time.deltaTime)
			{
				float t = elapsed / this.spinTime;
				base.transform.SetRotation2D(Mathf.Lerp(currentRotation, targetRotation, t));
				if (tweenPos)
				{
					base.transform.SetPosition2D(Vector2.Lerp(currentPos, targetPos, t));
				}
				yield return null;
			}
		}
		else if (!this.RepositionBeforeTurn)
		{
			yield return base.StartCoroutine(this.PlayCornerAnim(turnAnimation, damager));
		}
		base.transform.SetRotation2D(targetRotation);
		if (tweenPos)
		{
			base.transform.SetPosition2D(targetPos);
		}
		if (turnAnimation != null)
		{
			Vector3 b = base.transform.TransformVector(this.RepositionRegularOffset);
			base.transform.position += b;
			if (!this.CheckGround())
			{
				base.transform.position -= b;
				b = base.transform.TransformVector(this.RepositionImmediateOffset);
				base.transform.position += b;
			}
			this.StickToGround();
			if (this.RepositionBeforeTurn)
			{
				yield return base.StartCoroutine(this.PlayCornerAnim(turnAnimation, damager));
			}
			this.anim.Play(this.WalkAnim);
		}
		int num = (int)this.currentDirection;
		num += (turnClockwise ? 1 : -1);
		int num2 = Enum.GetNames(typeof(Climber.Direction)).Length;
		if (num < 0)
		{
			num = num2 - 1;
		}
		else if (num >= num2)
		{
			num = 0;
		}
		this.currentDirection = (Climber.Direction)num;
		this.body.linearVelocity = this.GetVelocity(this.currentDirection);
		this.turnRoutine = null;
		yield break;
	}

	// Token: 0x060017BF RID: 6079 RVA: 0x0006B801 File Offset: 0x00069A01
	private IEnumerator PlayCornerAnim(string animName, GameObject damager)
	{
		bool triggered = false;
		bool finished = false;
		this.anim.Play(animName);
		this.anim.AnimationEventTriggered = delegate(tk2dSpriteAnimator _, tk2dSpriteAnimationClip _, int _)
		{
			triggered = true;
		};
		this.anim.AnimationCompleted = delegate(tk2dSpriteAnimator _, tk2dSpriteAnimationClip _)
		{
			finished = true;
		};
		yield return new WaitUntil(() => triggered);
		if (damager)
		{
			damager.SetActive(true);
			this.col.enabled = false;
		}
		yield return new WaitUntil(() => finished);
		if (damager)
		{
			this.col.enabled = true;
			damager.SetActive(false);
		}
		this.anim.AnimationEventTriggered = null;
		this.anim.AnimationCompleted = null;
		yield break;
	}

	// Token: 0x060017C0 RID: 6080 RVA: 0x0006B81E File Offset: 0x00069A1E
	private IEnumerator TurnAround()
	{
		this.body.linearVelocity = Vector2.zero;
		if (!string.IsNullOrEmpty(this.TurnAroundAnim))
		{
			if (this.FlipBeforeTurn)
			{
				this.FlipXScale();
			}
			yield return base.StartCoroutine(this.anim.PlayAnimWait(this.TurnAroundAnim, null));
			this.anim.Play(this.WalkAnim);
			if (!this.FlipBeforeTurn)
			{
				this.FlipXScale();
			}
		}
		switch (this.currentDirection)
		{
		case Climber.Direction.Right:
			this.currentDirection = Climber.Direction.Left;
			break;
		case Climber.Direction.Down:
			this.currentDirection = Climber.Direction.Up;
			break;
		case Climber.Direction.Left:
			this.currentDirection = Climber.Direction.Right;
			break;
		case Climber.Direction.Up:
			this.currentDirection = Climber.Direction.Down;
			break;
		}
		this.body.linearVelocity = this.GetVelocity(this.currentDirection);
		this.turnRoutine = null;
		yield break;
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x0006B830 File Offset: 0x00069A30
	private void FlipXScale()
	{
		base.transform.SetScaleX(-base.transform.localScale.x);
		this.clockwise = !this.clockwise;
		Vector3 b = base.transform.TransformVector(new Vector3(this.FlipXOffset, 0f, 0f));
		base.transform.position += b;
	}

	// Token: 0x060017C2 RID: 6082 RVA: 0x0006B8A0 File Offset: 0x00069AA0
	private Vector2 GetVelocity(Climber.Direction direction)
	{
		Vector2 zero = Vector2.zero;
		switch (direction)
		{
		case Climber.Direction.Right:
			zero = new Vector2(this.speed, 0f);
			break;
		case Climber.Direction.Down:
			zero = new Vector2(0f, -this.speed);
			break;
		case Climber.Direction.Left:
			zero = new Vector2(-this.speed, 0f);
			break;
		case Climber.Direction.Up:
			zero = new Vector2(0f, this.speed);
			break;
		}
		return zero;
	}

	// Token: 0x060017C3 RID: 6083 RVA: 0x0006B91C File Offset: 0x00069B1C
	private bool CheckGround()
	{
		return this.FireRayLocal(this.col.offset.Where(new float?(0f), null), Vector2.down, this.col.size.y / 2f + 0.5f).collider != null;
	}

	// Token: 0x060017C4 RID: 6084 RVA: 0x0006B984 File Offset: 0x00069B84
	private bool CheckWall()
	{
		return this.FireRayLocal(this.col.offset, this.SpriteFacesRight ? Vector2.right : Vector2.left, this.col.size.x / 2f + this.wallRayPadding).collider != null;
	}

	// Token: 0x060017C5 RID: 6085 RVA: 0x0006B9E1 File Offset: 0x00069BE1
	public void Stun()
	{
		if (this.turnRoutine == null)
		{
			base.StopAllCoroutines();
			base.StartCoroutine(this.DoStun());
		}
	}

	// Token: 0x060017C6 RID: 6086 RVA: 0x0006B9FE File Offset: 0x00069BFE
	private IEnumerator DoStun()
	{
		this.body.linearVelocity = Vector2.zero;
		yield return base.StartCoroutine(this.anim.PlayAnimWait(this.StunAnim, null));
		base.StartCoroutine(this.Walk());
		yield break;
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x0006BA10 File Offset: 0x00069C10
	private RaycastHit2D FireRayLocal(Vector2 origin, Vector2 direction, float length)
	{
		Vector2 vector = base.transform.TransformPoint(origin);
		Vector2 vector2 = base.transform.TransformVector(direction);
		RaycastHit2D result = Helper.Raycast2D(vector, vector2, length, 256);
		Debug.DrawLine(vector, vector + vector2 * length);
		return result;
	}

	// Token: 0x060017C8 RID: 6088 RVA: 0x0006BA78 File Offset: 0x00069C78
	private Vector2 GetTweenPos(Climber.Direction direction)
	{
		Vector2 result = Vector2.zero;
		float num = this.wallRayPadding - this.col.offset.x;
		switch (direction)
		{
		case Climber.Direction.Right:
			result = (this.clockwise ? new Vector2(this.col.size.x / 2f, this.col.size.y / 2f) : new Vector2(this.col.size.x / 2f, -(this.col.size.y / 2f)));
			result.x += num;
			break;
		case Climber.Direction.Down:
			result = (this.clockwise ? new Vector2(this.col.size.x / 2f, -(this.col.size.y / 2f)) : new Vector2(-(this.col.size.x / 2f), -(this.col.size.y / 2f)));
			result.y -= num;
			break;
		case Climber.Direction.Left:
			result = (this.clockwise ? new Vector2(-(this.col.size.x / 2f), -(this.col.size.y / 2f)) : new Vector2(-(this.col.size.x / 2f), this.col.size.y / 2f));
			result.x -= num;
			break;
		case Climber.Direction.Up:
			result = (this.clockwise ? new Vector2(-(this.col.size.x / 2f), this.col.size.y / 2f) : new Vector2(this.col.size.x / 2f, this.col.size.y / 2f));
			result.y += num;
			break;
		}
		return result;
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x0006BCB8 File Offset: 0x00069EB8
	private void StickToGround()
	{
		Vector2 offset = this.col.offset;
		offset.y -= this.col.size.y / 2f;
		RaycastHit2D raycastHit2D = this.FireRayLocal(offset + Vector2.up, Vector2.down, 2f);
		if (raycastHit2D.collider != null)
		{
			Vector2 b = base.transform.TransformPoint(offset) - base.transform.position;
			base.transform.SetPosition2D(raycastHit2D.point - b);
		}
	}

	// Token: 0x0400166B RID: 5739
	public string WalkAnim = "Walk";

	// Token: 0x0400166C RID: 5740
	public string StunAnim = "Stun";

	// Token: 0x0400166D RID: 5741
	[Space]
	public Climber.WallBehaviours WallBehaviour;

	// Token: 0x0400166E RID: 5742
	public Climber.WallBehaviours EdgeBehaviour;

	// Token: 0x0400166F RID: 5743
	[ModifiableProperty]
	[InspectorValidation("DoesAnimExist")]
	public string ContinueConvexRegularAnim;

	// Token: 0x04001670 RID: 5744
	[ModifiableProperty]
	[InspectorValidation("DoesAnimExist")]
	public string ContinueConvexImmediateAnim;

	// Token: 0x04001671 RID: 5745
	public GameObject ConvexDamagerOverride;

	// Token: 0x04001672 RID: 5746
	[ModifiableProperty]
	[InspectorValidation("DoesAnimExist")]
	public string ContinueConcaveRegularAnim;

	// Token: 0x04001673 RID: 5747
	[ModifiableProperty]
	[InspectorValidation("DoesAnimExist")]
	public string ContinueConcaveImmediateAnim;

	// Token: 0x04001674 RID: 5748
	public GameObject ConcaveDamagerOverride;

	// Token: 0x04001675 RID: 5749
	public bool RepositionBeforeTurn;

	// Token: 0x04001676 RID: 5750
	public Vector2 RepositionRegularOffset;

	// Token: 0x04001677 RID: 5751
	public Vector2 RepositionImmediateOffset;

	// Token: 0x04001678 RID: 5752
	[Space]
	[ModifiableProperty]
	[InspectorValidation("DoesAnimExist")]
	public string TurnAroundAnim;

	// Token: 0x04001679 RID: 5753
	public bool FlipBeforeTurn;

	// Token: 0x0400167A RID: 5754
	public float FlipXOffset;

	// Token: 0x0400167B RID: 5755
	[Space]
	[FormerlySerializedAs("startRight")]
	public bool SpriteFacesRight = true;

	// Token: 0x0400167C RID: 5756
	private bool clockwise = true;

	// Token: 0x0400167D RID: 5757
	public float speed = 2f;

	// Token: 0x0400167E RID: 5758
	public float spinTime = 0.25f;

	// Token: 0x0400167F RID: 5759
	[Space]
	public float wallRayPadding = 0.1f;

	// Token: 0x04001680 RID: 5760
	[Space]
	public float minTurnDistance = 0.25f;

	// Token: 0x04001681 RID: 5761
	private Vector2 previousTurnPos;

	// Token: 0x04001682 RID: 5762
	private Climber.Direction currentDirection;

	// Token: 0x04001683 RID: 5763
	private Coroutine turnRoutine;

	// Token: 0x04001684 RID: 5764
	private YieldInstruction moveYield = new WaitForFixedUpdate();

	// Token: 0x04001685 RID: 5765
	private Rigidbody2D body;

	// Token: 0x04001686 RID: 5766
	private BoxCollider2D col;

	// Token: 0x04001687 RID: 5767
	private tk2dSpriteAnimator anim;

	// Token: 0x04001688 RID: 5768
	private Recoil recoil;

	// Token: 0x04001689 RID: 5769
	private bool isFirstSetupDone;

	// Token: 0x0200157C RID: 5500
	public enum WallBehaviours
	{
		// Token: 0x04008760 RID: 34656
		Continue,
		// Token: 0x04008761 RID: 34657
		Turn
	}

	// Token: 0x0200157D RID: 5501
	public enum Direction
	{
		// Token: 0x04008763 RID: 34659
		Right,
		// Token: 0x04008764 RID: 34660
		Down,
		// Token: 0x04008765 RID: 34661
		Left,
		// Token: 0x04008766 RID: 34662
		Up
	}
}
