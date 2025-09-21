using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9E RID: 3486
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Translates a Game Object per FixedUpdate, and also raycasts to detect if terrain is passed through. Move on EITHER x or y only! Ie cardinal directions ")]
	public class TranslateContinuous : FsmStateAction
	{
		// Token: 0x0600653A RID: 25914 RVA: 0x001FE846 File Offset: 0x001FCA46
		public override void Reset()
		{
			this.gameObject = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x0600653B RID: 25915 RVA: 0x001FE873 File Offset: 0x001FCA73
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600653C RID: 25916 RVA: 0x001FE881 File Offset: 0x001FCA81
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600653D RID: 25917 RVA: 0x001FE890 File Offset: 0x001FCA90
		public override void OnEnter()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
			}
			this.collider = this.go.GetComponent<BoxCollider2D>();
			if (this.x.Value < 0f)
			{
				this.moveDirection = 2;
				this.rayCastDirection = new Vector2(1f, 0f);
				this.moveDistance = this.x.Value - this.collider.bounds.size.x;
				this.debugDirection = new Vector2(this.x.Value, 0f);
			}
			else if (this.x.Value > 0f)
			{
				this.moveDirection = 0;
				this.rayCastDirection = new Vector2(1f, 0f);
				this.moveDistance = this.x.Value + this.collider.bounds.size.x;
				this.debugDirection = new Vector2(this.x.Value, 0f);
			}
			else if (this.y.Value < 0f)
			{
				this.moveDirection = 3;
				this.rayCastDirection = new Vector2(0f, 1f);
				this.moveDistance = this.y.Value - this.collider.bounds.size.y;
				this.debugDirection = new Vector2(0f, this.y.Value);
			}
			else if (this.y.Value > 0f)
			{
				this.moveDirection = 1;
				this.rayCastDirection = new Vector2(0f, 1f);
				this.moveDistance = this.y.Value + this.collider.bounds.size.y;
				this.debugDirection = new Vector2(0f, this.y.Value);
			}
			if (this.moveDirection == 0 || this.moveDirection == 3)
			{
				this.point1Offset = new Vector2(this.collider.offset.x - this.collider.bounds.size.x / 2f, this.collider.offset.y + this.collider.bounds.size.y / 2f);
			}
			if (this.moveDirection == 1 || this.moveDirection == 2)
			{
				this.point1Offset = new Vector2(this.collider.offset.x + this.collider.bounds.size.x / 2f, this.collider.offset.y - this.collider.bounds.size.y / 2f);
			}
			if (this.moveDirection == 2 || this.moveDirection == 3)
			{
				this.point2Offset = new Vector2(this.collider.offset.x + this.collider.bounds.size.x / 2f, this.collider.offset.y + this.collider.bounds.size.y / 2f);
			}
			if (this.moveDirection == 0 || this.moveDirection == 1)
			{
				this.point2Offset = new Vector2(this.collider.offset.x - this.collider.bounds.size.x / 2f, this.collider.offset.y - this.collider.bounds.size.y / 2f);
			}
			if (this.moveDirection == 0)
			{
				this.point3Offset = new Vector2(this.collider.offset.x - this.collider.bounds.size.x / 2f, this.collider.offset.y);
			}
			else if (this.moveDirection == 1)
			{
				this.point3Offset = new Vector2(this.collider.offset.x, this.collider.offset.y - this.collider.bounds.size.y / 2f);
			}
			else if (this.moveDirection == 2)
			{
				this.point3Offset = new Vector2(this.collider.offset.x + this.collider.bounds.size.x / 2f, this.collider.offset.y);
			}
			else if (this.moveDirection == 3)
			{
				this.point3Offset = new Vector2(this.collider.offset.x, this.collider.offset.y + this.collider.bounds.size.y / 2f);
			}
			this.DoTranslate();
		}

		// Token: 0x0600653E RID: 25918 RVA: 0x001FEDF8 File Offset: 0x001FCFF8
		public override void OnFixedUpdate()
		{
			this.DoTranslate();
		}

		// Token: 0x0600653F RID: 25919 RVA: 0x001FEE00 File Offset: 0x001FD000
		private void DoTranslate()
		{
			this.hitWall = false;
			this.translate = new Vector2(this.x.Value, this.y.Value);
			this.rayOrigin1 = new Vector2(this.go.transform.position.x + this.point1Offset.x, this.go.transform.position.y + this.point1Offset.y);
			this.rayOrigin2 = new Vector2(this.go.transform.position.x + this.point2Offset.x, this.go.transform.position.y + this.point2Offset.y);
			this.rayOrigin3 = new Vector2(this.go.transform.position.x + this.point3Offset.x, this.go.transform.position.y + this.point3Offset.y);
			Debug.DrawLine(this.rayOrigin2, new Vector2(this.rayOrigin2.x + this.moveDistance, this.rayOrigin2.y), Color.yellow);
			RaycastHit2D raycastHit2D = Helper.Raycast2D(this.rayOrigin1, this.rayCastDirection, this.moveDistance, 256);
			RaycastHit2D raycastHit2D2 = Helper.Raycast2D(this.rayOrigin2, this.rayCastDirection, this.moveDistance, 256);
			RaycastHit2D raycastHit2D3 = Helper.Raycast2D(this.rayOrigin3, this.rayCastDirection, this.moveDistance, 256);
			bool flag = raycastHit2D.collider != null;
			bool flag2 = raycastHit2D2.collider != null;
			bool flag3 = raycastHit2D3.collider != null;
			if (flag || flag2 || flag3)
			{
				float num = 0f;
				if (this.moveDirection == 2)
				{
					if (flag)
					{
						num = raycastHit2D.point.x;
						if (flag2)
						{
							num = Mathf.Max(num, raycastHit2D2.point.x);
						}
						if (flag3)
						{
							num = Mathf.Max(num, raycastHit2D3.point.x);
						}
					}
					else if (flag2)
					{
						num = raycastHit2D2.point.x;
						if (flag3)
						{
							num = Mathf.Max(num, raycastHit2D3.point.x);
						}
					}
					else if (flag3)
					{
						num = raycastHit2D3.point.x;
					}
					this.translate.x = this.translate.x + (num - (this.rayOrigin1.x + this.rayCastDirection.x * this.moveDistance));
					this.hitWall = true;
				}
				if (this.moveDirection == 0)
				{
					if (flag)
					{
						num = raycastHit2D.point.x;
						if (flag2)
						{
							num = Mathf.Min(num, raycastHit2D2.point.x);
						}
						if (flag3)
						{
							num = Mathf.Min(num, raycastHit2D3.point.x);
						}
					}
					else if (flag2)
					{
						num = raycastHit2D2.point.x;
						if (flag3)
						{
							num = Mathf.Min(num, raycastHit2D3.point.x);
						}
					}
					else if (flag3)
					{
						num = raycastHit2D3.point.x;
					}
					this.translate.x = this.translate.x + (num - (this.rayOrigin1.x + this.rayCastDirection.x * this.moveDistance));
					this.hitWall = true;
				}
				if (this.moveDirection == 1)
				{
					if (flag)
					{
						num = raycastHit2D.point.y;
						if (flag2)
						{
							num = Mathf.Min(num, raycastHit2D2.point.y);
						}
						if (flag3)
						{
							num = Mathf.Min(num, raycastHit2D3.point.y);
						}
					}
					else if (flag2)
					{
						num = raycastHit2D2.point.y;
						if (flag3)
						{
							num = Mathf.Min(num, raycastHit2D3.point.y);
						}
					}
					else if (flag3)
					{
						num = raycastHit2D3.point.y;
					}
					this.translate.y = this.translate.y + (num - (this.rayOrigin1.y + this.rayCastDirection.y * this.moveDistance));
					this.hitWall = true;
				}
				if (this.moveDirection == 3)
				{
					if (flag)
					{
						num = raycastHit2D.point.y;
						if (flag2)
						{
							num = Mathf.Max(num, raycastHit2D2.point.y);
						}
						if (flag3)
						{
							num = Mathf.Max(num, raycastHit2D3.point.y);
						}
					}
					else if (flag2)
					{
						num = raycastHit2D2.point.y;
						if (flag3)
						{
							num = Mathf.Max(num, raycastHit2D3.point.y);
						}
					}
					else if (flag3)
					{
						num = raycastHit2D3.point.y;
					}
					this.translate.y = this.translate.y + (num - (this.rayOrigin1.y + this.rayCastDirection.y * this.moveDistance));
					this.hitWall = true;
				}
			}
			else
			{
				this.hitWall = false;
			}
			if (this.hitWall)
			{
				base.Finish();
				return;
			}
			this.go.transform.Translate(this.translate, Space.World);
		}

		// Token: 0x0400642C RID: 25644
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400642D RID: 25645
		[Tooltip("Translation along x axis.")]
		public FsmFloat x;

		// Token: 0x0400642E RID: 25646
		[Tooltip("Translation along y axis.")]
		public FsmFloat y;

		// Token: 0x0400642F RID: 25647
		public FsmInt[] layerMask;

		// Token: 0x04006430 RID: 25648
		private GameObject go;

		// Token: 0x04006431 RID: 25649
		private int moveDirection;

		// Token: 0x04006432 RID: 25650
		private BoxCollider2D collider;

		// Token: 0x04006433 RID: 25651
		private Vector2 point1Offset;

		// Token: 0x04006434 RID: 25652
		private Vector2 point2Offset;

		// Token: 0x04006435 RID: 25653
		private Vector2 point3Offset;

		// Token: 0x04006436 RID: 25654
		private Vector2 rayOrigin1;

		// Token: 0x04006437 RID: 25655
		private Vector2 rayOrigin2;

		// Token: 0x04006438 RID: 25656
		private Vector2 rayOrigin3;

		// Token: 0x04006439 RID: 25657
		private Vector2 rayCastDirection;

		// Token: 0x0400643A RID: 25658
		private Vector2 debugDirection;

		// Token: 0x0400643B RID: 25659
		private float moveDistance;

		// Token: 0x0400643C RID: 25660
		private Vector2 translate;

		// Token: 0x0400643D RID: 25661
		private bool hitWall;
	}
}
