using System;
using UnityEngine;

// Token: 0x02000329 RID: 809
[RequireComponent(typeof(tk2dSpriteAnimator), typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class WallClinger : MonoBehaviour
{
	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06001C60 RID: 7264 RVA: 0x000841E4 File Offset: 0x000823E4
	// (set) Token: 0x06001C61 RID: 7265 RVA: 0x000841EC File Offset: 0x000823EC
	public bool IsActive
	{
		get
		{
			return this._isActive;
		}
		set
		{
			if (this._isActive && !value)
			{
				Rigidbody2D rigidbody2D = this.body;
				float? y = new float?(0f);
				rigidbody2D.SetVelocity(null, y);
			}
			else if (!this._isActive && value)
			{
				this.StartMovingDirection(0);
			}
			this._isActive = value;
		}
	}

	// Token: 0x06001C62 RID: 7266 RVA: 0x00084242 File Offset: 0x00082442
	private void OnDrawGizmosSelected()
	{
		this.collider = base.GetComponent<BoxCollider2D>();
		if (this.collider)
		{
			this.DoRayCasts(true, true);
			this.DoRayCasts(true, false);
		}
	}

	// Token: 0x06001C63 RID: 7267 RVA: 0x0008426D File Offset: 0x0008246D
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.collider = base.GetComponent<BoxCollider2D>();
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x06001C64 RID: 7268 RVA: 0x00084293 File Offset: 0x00082493
	private void Start()
	{
		this.IsActive = !this.StartInactive;
	}

	// Token: 0x06001C65 RID: 7269 RVA: 0x000842A4 File Offset: 0x000824A4
	private void FixedUpdate()
	{
		if (!this.IsActive)
		{
			return;
		}
		this.DoRayCasts(false, false);
	}

	// Token: 0x06001C66 RID: 7270 RVA: 0x000842B8 File Offset: 0x000824B8
	private void DoRayCasts(bool drawingGizmos, bool gizmoDirection)
	{
		Vector2 b = this.collider.size / 2f;
		Vector2 vector = this.collider.offset - b;
		Vector2 vector2 = this.collider.offset + b;
		if (!this.IsSpriteFacingRight)
		{
			float x = vector.x;
			vector.x = vector2.x;
			vector2.x = x;
		}
		bool flag = (!drawingGizmos) ? (this.body.linearVelocity.y > 0f) : gizmoDirection;
		Vector2 vector3 = flag ? Vector2.up : Vector2.down;
		Vector2 vector4 = this.IsSpriteFacingRight ? Vector2.right : Vector2.left;
		Vector2 a = new Vector2(vector2.x, flag ? vector2.y : vector.y);
		float x2 = this.IsSpriteFacingRight ? -0.5f : 0.5f;
		Vector2 vector5 = a + new Vector2(x2, flag ? this.EdgePaddingTop : (-this.EdgePaddingBottom));
		Vector2 vector6 = a + new Vector2(x2, flag ? -0.5f : 0.5f);
		float num = 1f;
		float num2 = (flag ? this.PaddingTop : this.PaddingBottom) + 0.5f;
		if (!drawingGizmos)
		{
			num2 = Mathf.Max(num2, Mathf.Abs(this.body.linearVelocity.y * Time.deltaTime));
			bool flag2 = this.IsRayHittingLocal(vector6, vector3, num2);
			bool flag3 = this.IsRayHittingLocal(vector5, vector4, num);
			if (flag2 || !flag3)
			{
				this.StartMovingDirection((int)Mathf.Sign(-this.body.linearVelocity.y));
				return;
			}
		}
		else
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(vector5, vector5 + vector4 * num);
			Gizmos.DrawLine(vector6, vector6 + vector3 * num2);
		}
	}

	// Token: 0x06001C67 RID: 7271 RVA: 0x000844BB File Offset: 0x000826BB
	private bool IsRayHittingLocal(Vector2 originLocal, Vector2 directionLocal, float length)
	{
		return base.transform.IsRayHittingLocal(originLocal, directionLocal, length, 256);
	}

	// Token: 0x06001C68 RID: 7272 RVA: 0x000844D0 File Offset: 0x000826D0
	public void StartMovingDirection(int direction = 0)
	{
		this._isActive = true;
		if (direction == 0)
		{
			direction = ((Random.Range(0, 2) > 0) ? 1 : -1);
		}
		Rigidbody2D rigidbody2D = this.body;
		float? y = new float?((float)direction * this.MoveSpeed);
		rigidbody2D.SetVelocity(null, y);
		string text = (direction > 0) ? this.ClimbUpAnim : this.ClimbDownAnim;
		if (!string.IsNullOrEmpty(text))
		{
			this.animator.Play(text);
		}
	}

	// Token: 0x04001B7B RID: 7035
	private const float SKIN_WIDTH = 0.5f;

	// Token: 0x04001B7C RID: 7036
	public bool IsSpriteFacingRight;

	// Token: 0x04001B7D RID: 7037
	public bool StartInactive;

	// Token: 0x04001B7E RID: 7038
	[Space]
	public float MoveSpeed;

	// Token: 0x04001B7F RID: 7039
	[Space]
	public float EdgePaddingTop;

	// Token: 0x04001B80 RID: 7040
	public float EdgePaddingBottom;

	// Token: 0x04001B81 RID: 7041
	public float PaddingTop;

	// Token: 0x04001B82 RID: 7042
	public float PaddingBottom;

	// Token: 0x04001B83 RID: 7043
	[Space]
	public string ClimbUpAnim;

	// Token: 0x04001B84 RID: 7044
	public string ClimbDownAnim;

	// Token: 0x04001B85 RID: 7045
	private tk2dSpriteAnimator animator;

	// Token: 0x04001B86 RID: 7046
	private BoxCollider2D collider;

	// Token: 0x04001B87 RID: 7047
	private Rigidbody2D body;

	// Token: 0x04001B88 RID: 7048
	private bool _isActive;
}
