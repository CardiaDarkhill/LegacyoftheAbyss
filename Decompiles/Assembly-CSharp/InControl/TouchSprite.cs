using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200092E RID: 2350
	[Serializable]
	public class TouchSprite
	{
		// Token: 0x17000B71 RID: 2929
		// (get) Token: 0x06005362 RID: 21346 RVA: 0x0017D6D1 File Offset: 0x0017B8D1
		// (set) Token: 0x06005363 RID: 21347 RVA: 0x0017D6D9 File Offset: 0x0017B8D9
		public bool Dirty { get; set; }

		// Token: 0x17000B72 RID: 2930
		// (get) Token: 0x06005364 RID: 21348 RVA: 0x0017D6E2 File Offset: 0x0017B8E2
		// (set) Token: 0x06005365 RID: 21349 RVA: 0x0017D6EA File Offset: 0x0017B8EA
		public bool Ready { get; set; }

		// Token: 0x06005366 RID: 21350 RVA: 0x0017D6F4 File Offset: 0x0017B8F4
		public TouchSprite()
		{
		}

		// Token: 0x06005367 RID: 21351 RVA: 0x0017D764 File Offset: 0x0017B964
		public TouchSprite(float size)
		{
			this.size = Vector2.one * size;
		}

		// Token: 0x06005368 RID: 21352 RVA: 0x0017D7E4 File Offset: 0x0017B9E4
		public void Create(string gameObjectName, Transform parentTransform, int sortingOrder)
		{
			this.spriteGameObject = this.CreateSpriteGameObject(gameObjectName, parentTransform);
			this.spriteRenderer = this.CreateSpriteRenderer(this.spriteGameObject, this.idleSprite, sortingOrder);
			this.spriteRenderer.color = this.idleColor;
			this.Ready = true;
		}

		// Token: 0x06005369 RID: 21353 RVA: 0x0017D830 File Offset: 0x0017BA30
		public void Delete()
		{
			this.Ready = false;
			Object.Destroy(this.spriteGameObject);
		}

		// Token: 0x0600536A RID: 21354 RVA: 0x0017D844 File Offset: 0x0017BA44
		public void Update()
		{
			this.Update(false);
		}

		// Token: 0x0600536B RID: 21355 RVA: 0x0017D850 File Offset: 0x0017BA50
		public void Update(bool forceUpdate)
		{
			if (this.Dirty || forceUpdate)
			{
				if (this.spriteRenderer != null)
				{
					this.spriteRenderer.sprite = (this.State ? this.busySprite : this.idleSprite);
				}
				if (this.sizeUnitType == TouchUnitType.Pixels)
				{
					Vector2 a = TouchUtility.RoundVector(this.size);
					this.ScaleSpriteInPixels(this.spriteGameObject, this.spriteRenderer, a);
					this.worldSize = a * TouchManager.PixelToWorld;
				}
				else
				{
					this.ScaleSpriteInPercent(this.spriteGameObject, this.spriteRenderer, this.size);
					if (this.lockAspectRatio)
					{
						this.worldSize = this.size * TouchManager.PercentToWorld;
					}
					else
					{
						this.worldSize = Vector2.Scale(this.size, TouchManager.ViewSize);
					}
				}
				this.Dirty = false;
			}
			if (this.spriteRenderer != null)
			{
				Color color = this.State ? this.busyColor : this.idleColor;
				if (this.spriteRenderer.color != color)
				{
					this.spriteRenderer.color = Utility.MoveColorTowards(this.spriteRenderer.color, color, 5f * Time.unscaledDeltaTime);
				}
			}
		}

		// Token: 0x0600536C RID: 21356 RVA: 0x0017D990 File Offset: 0x0017BB90
		private GameObject CreateSpriteGameObject(string name, Transform parentTransform)
		{
			return new GameObject(name)
			{
				transform = 
				{
					parent = parentTransform,
					localPosition = Vector3.zero,
					localScale = Vector3.one
				},
				layer = parentTransform.gameObject.layer
			};
		}

		// Token: 0x0600536D RID: 21357 RVA: 0x0017D9E0 File Offset: 0x0017BBE0
		private SpriteRenderer CreateSpriteRenderer(GameObject spriteGameObject, Sprite sprite, int sortingOrder)
		{
			if (!TouchSprite.spriteRendererMaterial)
			{
				TouchSprite.spriteRendererShader = Shader.Find("Sprites/Default");
				TouchSprite.spriteRendererMaterial = new Material(TouchSprite.spriteRendererShader);
				TouchSprite.spriteRendererPixelSnapId = Shader.PropertyToID("PixelSnap");
			}
			SpriteRenderer spriteRenderer = spriteGameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingOrder = sortingOrder;
			spriteRenderer.sharedMaterial = TouchSprite.spriteRendererMaterial;
			spriteRenderer.sharedMaterial.SetFloat(TouchSprite.spriteRendererPixelSnapId, 1f);
			return spriteRenderer;
		}

		// Token: 0x0600536E RID: 21358 RVA: 0x0017DA5C File Offset: 0x0017BC5C
		private void ScaleSpriteInPixels(GameObject spriteGameObject, SpriteRenderer spriteRenderer, Vector2 size)
		{
			if (spriteGameObject == null || spriteRenderer == null || spriteRenderer.sprite == null)
			{
				return;
			}
			float num = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.bounds.size.x;
			float num2 = TouchManager.PixelToWorld * num;
			float x = num2 * size.x / spriteRenderer.sprite.rect.width;
			float y = num2 * size.y / spriteRenderer.sprite.rect.height;
			spriteGameObject.transform.localScale = new Vector3(x, y);
		}

		// Token: 0x0600536F RID: 21359 RVA: 0x0017DB0C File Offset: 0x0017BD0C
		private void ScaleSpriteInPercent(GameObject spriteGameObject, SpriteRenderer spriteRenderer, Vector2 size)
		{
			if (spriteGameObject == null || spriteRenderer == null || spriteRenderer.sprite == null)
			{
				return;
			}
			if (this.lockAspectRatio)
			{
				float num = Mathf.Min(TouchManager.ViewSize.x, TouchManager.ViewSize.y);
				float x = num * size.x / spriteRenderer.sprite.bounds.size.x;
				float y = num * size.y / spriteRenderer.sprite.bounds.size.y;
				spriteGameObject.transform.localScale = new Vector3(x, y);
				return;
			}
			float x2 = TouchManager.ViewSize.x * size.x / spriteRenderer.sprite.bounds.size.x;
			float y2 = TouchManager.ViewSize.y * size.y / spriteRenderer.sprite.bounds.size.y;
			spriteGameObject.transform.localScale = new Vector3(x2, y2);
		}

		// Token: 0x06005370 RID: 21360 RVA: 0x0017DC1C File Offset: 0x0017BE1C
		public bool Contains(Vector2 testWorldPoint)
		{
			if (this.shape == TouchSpriteShape.Oval)
			{
				float num = (testWorldPoint.x - this.Position.x) / this.worldSize.x;
				float num2 = (testWorldPoint.y - this.Position.y) / this.worldSize.y;
				return num * num + num2 * num2 < 0.25f;
			}
			float num3 = Utility.Abs(testWorldPoint.x - this.Position.x) * 2f;
			float num4 = Utility.Abs(testWorldPoint.y - this.Position.y) * 2f;
			return num3 <= this.worldSize.x && num4 <= this.worldSize.y;
		}

		// Token: 0x06005371 RID: 21361 RVA: 0x0017DCD7 File Offset: 0x0017BED7
		public bool Contains(Touch touch)
		{
			return this.Contains(TouchManager.ScreenToWorldPoint(touch.position));
		}

		// Token: 0x06005372 RID: 21362 RVA: 0x0017DCEF File Offset: 0x0017BEEF
		public void DrawGizmos(Vector3 position, Color color)
		{
			if (this.shape == TouchSpriteShape.Oval)
			{
				Utility.DrawOvalGizmo(position, this.WorldSize, color);
				return;
			}
			Utility.DrawRectGizmo(position, this.WorldSize, color);
		}

		// Token: 0x17000B73 RID: 2931
		// (get) Token: 0x06005373 RID: 21363 RVA: 0x0017DD1E File Offset: 0x0017BF1E
		// (set) Token: 0x06005374 RID: 21364 RVA: 0x0017DD26 File Offset: 0x0017BF26
		public bool State
		{
			get
			{
				return this.state;
			}
			set
			{
				if (this.state != value)
				{
					this.state = value;
					this.Dirty = true;
				}
			}
		}

		// Token: 0x17000B74 RID: 2932
		// (get) Token: 0x06005375 RID: 21365 RVA: 0x0017DD3F File Offset: 0x0017BF3F
		// (set) Token: 0x06005376 RID: 21366 RVA: 0x0017DD47 File Offset: 0x0017BF47
		public Sprite BusySprite
		{
			get
			{
				return this.busySprite;
			}
			set
			{
				if (this.busySprite != value)
				{
					this.busySprite = value;
					this.Dirty = true;
				}
			}
		}

		// Token: 0x17000B75 RID: 2933
		// (get) Token: 0x06005377 RID: 21367 RVA: 0x0017DD65 File Offset: 0x0017BF65
		// (set) Token: 0x06005378 RID: 21368 RVA: 0x0017DD6D File Offset: 0x0017BF6D
		public Sprite IdleSprite
		{
			get
			{
				return this.idleSprite;
			}
			set
			{
				if (this.idleSprite != value)
				{
					this.idleSprite = value;
					this.Dirty = true;
				}
			}
		}

		// Token: 0x17000B76 RID: 2934
		// (set) Token: 0x06005379 RID: 21369 RVA: 0x0017DD8B File Offset: 0x0017BF8B
		public Sprite Sprite
		{
			set
			{
				if (this.idleSprite != value)
				{
					this.idleSprite = value;
					this.Dirty = true;
				}
				if (this.busySprite != value)
				{
					this.busySprite = value;
					this.Dirty = true;
				}
			}
		}

		// Token: 0x17000B77 RID: 2935
		// (get) Token: 0x0600537A RID: 21370 RVA: 0x0017DDC5 File Offset: 0x0017BFC5
		// (set) Token: 0x0600537B RID: 21371 RVA: 0x0017DDCD File Offset: 0x0017BFCD
		public Color BusyColor
		{
			get
			{
				return this.busyColor;
			}
			set
			{
				if (this.busyColor != value)
				{
					this.busyColor = value;
					this.Dirty = true;
				}
			}
		}

		// Token: 0x17000B78 RID: 2936
		// (get) Token: 0x0600537C RID: 21372 RVA: 0x0017DDEB File Offset: 0x0017BFEB
		// (set) Token: 0x0600537D RID: 21373 RVA: 0x0017DDF3 File Offset: 0x0017BFF3
		public Color IdleColor
		{
			get
			{
				return this.idleColor;
			}
			set
			{
				if (this.idleColor != value)
				{
					this.idleColor = value;
					this.Dirty = true;
				}
			}
		}

		// Token: 0x17000B79 RID: 2937
		// (get) Token: 0x0600537E RID: 21374 RVA: 0x0017DE11 File Offset: 0x0017C011
		// (set) Token: 0x0600537F RID: 21375 RVA: 0x0017DE19 File Offset: 0x0017C019
		public TouchSpriteShape Shape
		{
			get
			{
				return this.shape;
			}
			set
			{
				if (this.shape != value)
				{
					this.shape = value;
					this.Dirty = true;
				}
			}
		}

		// Token: 0x17000B7A RID: 2938
		// (get) Token: 0x06005380 RID: 21376 RVA: 0x0017DE32 File Offset: 0x0017C032
		// (set) Token: 0x06005381 RID: 21377 RVA: 0x0017DE3A File Offset: 0x0017C03A
		public TouchUnitType SizeUnitType
		{
			get
			{
				return this.sizeUnitType;
			}
			set
			{
				if (this.sizeUnitType != value)
				{
					this.sizeUnitType = value;
					this.Dirty = true;
				}
			}
		}

		// Token: 0x17000B7B RID: 2939
		// (get) Token: 0x06005382 RID: 21378 RVA: 0x0017DE53 File Offset: 0x0017C053
		// (set) Token: 0x06005383 RID: 21379 RVA: 0x0017DE5B File Offset: 0x0017C05B
		public Vector2 Size
		{
			get
			{
				return this.size;
			}
			set
			{
				if (this.size != value)
				{
					this.size = value;
					this.Dirty = true;
				}
			}
		}

		// Token: 0x17000B7C RID: 2940
		// (get) Token: 0x06005384 RID: 21380 RVA: 0x0017DE79 File Offset: 0x0017C079
		public Vector2 WorldSize
		{
			get
			{
				return this.worldSize;
			}
		}

		// Token: 0x17000B7D RID: 2941
		// (get) Token: 0x06005385 RID: 21381 RVA: 0x0017DE81 File Offset: 0x0017C081
		// (set) Token: 0x06005386 RID: 21382 RVA: 0x0017DEA6 File Offset: 0x0017C0A6
		public Vector3 Position
		{
			get
			{
				if (!this.spriteGameObject)
				{
					return Vector3.zero;
				}
				return this.spriteGameObject.transform.position;
			}
			set
			{
				if (this.spriteGameObject)
				{
					this.spriteGameObject.transform.position = value;
				}
			}
		}

		// Token: 0x04005363 RID: 21347
		[SerializeField]
		private Sprite idleSprite;

		// Token: 0x04005364 RID: 21348
		[SerializeField]
		private Sprite busySprite;

		// Token: 0x04005365 RID: 21349
		[SerializeField]
		private Color idleColor = new Color(1f, 1f, 1f, 0.5f);

		// Token: 0x04005366 RID: 21350
		[SerializeField]
		private Color busyColor = new Color(1f, 1f, 1f, 1f);

		// Token: 0x04005367 RID: 21351
		[SerializeField]
		private TouchSpriteShape shape;

		// Token: 0x04005368 RID: 21352
		[SerializeField]
		private TouchUnitType sizeUnitType;

		// Token: 0x04005369 RID: 21353
		[SerializeField]
		private Vector2 size = new Vector2(10f, 10f);

		// Token: 0x0400536A RID: 21354
		[SerializeField]
		private bool lockAspectRatio = true;

		// Token: 0x0400536B RID: 21355
		[SerializeField]
		[HideInInspector]
		private Vector2 worldSize;

		// Token: 0x0400536C RID: 21356
		private GameObject spriteGameObject;

		// Token: 0x0400536D RID: 21357
		private SpriteRenderer spriteRenderer;

		// Token: 0x0400536E RID: 21358
		private bool state;

		// Token: 0x04005371 RID: 21361
		private static Shader spriteRendererShader;

		// Token: 0x04005372 RID: 21362
		private static Material spriteRendererMaterial;

		// Token: 0x04005373 RID: 21363
		private static int spriteRendererPixelSnapId;
	}
}
