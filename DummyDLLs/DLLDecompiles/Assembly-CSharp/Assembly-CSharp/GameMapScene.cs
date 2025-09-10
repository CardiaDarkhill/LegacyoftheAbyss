using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x02000667 RID: 1639
public class GameMapScene : MonoBehaviour, IInitialisable
{
	// Token: 0x17000698 RID: 1688
	// (get) Token: 0x06003AAF RID: 15023 RVA: 0x00102684 File Offset: 0x00100884
	public string Name
	{
		get
		{
			if (this.isNameCached)
			{
				return this.cachedName;
			}
			this.isNameCached = true;
			this.cachedName = base.name;
			return this.cachedName;
		}
	}

	// Token: 0x17000699 RID: 1689
	// (get) Token: 0x06003AB0 RID: 15024 RVA: 0x001026AE File Offset: 0x001008AE
	public GameMapScene.States InitialState
	{
		get
		{
			return this.initialState;
		}
	}

	// Token: 0x1700069A RID: 1690
	// (get) Token: 0x06003AB1 RID: 15025 RVA: 0x001026B6 File Offset: 0x001008B6
	public Sprite BoundsSprite
	{
		get
		{
			if (this.unmappedNoBounds && !this.IsMapped)
			{
				return null;
			}
			if (this.IsInitialStateRough() && this.fullSprite)
			{
				return this.fullSprite;
			}
			if (!this.hasSpriteRenderer)
			{
				return null;
			}
			return this.initialSprite;
		}
	}

	// Token: 0x1700069B RID: 1691
	// (get) Token: 0x06003AB2 RID: 15026 RVA: 0x001026F6 File Offset: 0x001008F6
	// (set) Token: 0x06003AB3 RID: 15027 RVA: 0x0010271C File Offset: 0x0010091C
	public bool IsMapped
	{
		get
		{
			return this.isMapped || (this.mappedParent && this.mappedParent.IsMapped);
		}
		private set
		{
			this.isMapped = value;
		}
	}

	// Token: 0x1700069C RID: 1692
	// (get) Token: 0x06003AB4 RID: 15028 RVA: 0x00102725 File Offset: 0x00100925
	// (set) Token: 0x06003AB5 RID: 15029 RVA: 0x0010274B File Offset: 0x0010094B
	public bool IsVisited
	{
		get
		{
			return this.isVisited || (this.mappedParent && this.mappedParent.IsVisited);
		}
		private set
		{
			this.isVisited = value;
		}
	}

	// Token: 0x06003AB6 RID: 15030 RVA: 0x00102754 File Offset: 0x00100954
	[UsedImplicitly]
	private bool IsInitialStateRough()
	{
		return this.initialState == GameMapScene.States.Rough;
	}

	// Token: 0x06003AB7 RID: 15031 RVA: 0x00102760 File Offset: 0x00100960
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.GetMissingComponents();
		this.PurgeNull();
		if (!this.isNameCached)
		{
			this.isNameCached = true;
			this.cachedName = base.name;
		}
		if (!this.IsMapped)
		{
			this.SetNotMapped();
		}
		if (!this.hasSpriteRenderer)
		{
			return true;
		}
		SpriteRenderer spriteRenderer = this.spriteRenderer;
		Color color = this.spriteRenderer.color;
		float? a = new float?(1f);
		spriteRenderer.color = color.Where(null, null, null, a);
		this.spriteRenderer.sortingOrder = 11;
		return true;
	}

	// Token: 0x06003AB8 RID: 15032 RVA: 0x0010280E File Offset: 0x00100A0E
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x06003AB9 RID: 15033 RVA: 0x00102829 File Offset: 0x00100A29
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x06003ABA RID: 15034 RVA: 0x00102832 File Offset: 0x00100A32
	private void OnEnable()
	{
		if (this.hideCondition.TestGroups != null && this.hideCondition.TestGroups.Length != 0 && this.hideCondition.IsFulfilled && this.hasSpriteRenderer)
		{
			this.spriteRenderer.enabled = false;
		}
	}

	// Token: 0x06003ABB RID: 15035 RVA: 0x00102870 File Offset: 0x00100A70
	public void ResetMapped()
	{
		this.IsMapped = false;
		this.IsVisited = false;
	}

	// Token: 0x06003ABC RID: 15036 RVA: 0x00102880 File Offset: 0x00100A80
	private void PurgeNull()
	{
		if (this.purgedNulls)
		{
			return;
		}
		this.purgedNulls = true;
		if (this.mappedIfAllMapped != null && this.mappedIfAllMapped.Length != 0)
		{
			List<GameMapScene> list = this.mappedIfAllMapped.ToList<GameMapScene>();
			list.RemoveAll((GameMapScene o) => o == null);
			this.mappedIfAllMapped = list.ToArray();
		}
	}

	// Token: 0x06003ABD RID: 15037 RVA: 0x001028EC File Offset: 0x00100AEC
	public void SetVisited()
	{
		this.IsVisited = true;
	}

	// Token: 0x06003ABE RID: 15038 RVA: 0x001028F8 File Offset: 0x00100AF8
	public void SetMapped()
	{
		GameObject gameObject = base.gameObject;
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
		if (!this.IsMapped || !this.hasBeenSet || this.altFullSprites.Length != 0 || this.altColors.Length != 0)
		{
			this.IsMapped = true;
			this.hasBeenSet = true;
			this.GetMissingComponents();
			if (!this.hasSpriteRenderer)
			{
				return;
			}
			this.cachedSpriteBounds = false;
			this.spriteRenderer.color = this.GetColor();
			Sprite sprite;
			if (this.initialState == GameMapScene.States.Rough)
			{
				sprite = (this.fullSprite ? this.fullSprite : this.initialSprite);
			}
			else
			{
				sprite = this.initialSprite;
			}
			Sprite sprite2 = sprite;
			foreach (GameMapScene.SpriteCondition spriteCondition in this.altFullSprites)
			{
				if (spriteCondition.Condition.IsFulfilled)
				{
					sprite2 = spriteCondition.Sprite;
					break;
				}
			}
			this.spriteRenderer.sprite = sprite2;
		}
	}

	// Token: 0x06003ABF RID: 15039 RVA: 0x001029EC File Offset: 0x00100BEC
	public void SetNotMapped()
	{
		if (this.IsMapped || !this.hasBeenSet)
		{
			this.IsMapped = false;
			this.hasBeenSet = true;
			this.GetMissingComponents();
			if (!this.hasSpriteRenderer)
			{
				return;
			}
			this.cachedSpriteBounds = false;
			GameMapScene.States states = this.initialState;
			if (states == GameMapScene.States.Hidden)
			{
				this.spriteRenderer.sprite = null;
				return;
			}
			if (states != GameMapScene.States.Rough)
			{
				return;
			}
			if (this.fullSprite)
			{
				this.spriteRenderer.sprite = this.initialSprite;
				return;
			}
			this.spriteRenderer.sprite = this.initialSprite;
			this.spriteRenderer.color = Color.grey;
		}
	}

	// Token: 0x06003AC0 RID: 15040 RVA: 0x00102A8C File Offset: 0x00100C8C
	private void GetMissingComponents()
	{
		if (this.hasSpriteRenderer)
		{
			return;
		}
		if (this.checkedSprite)
		{
			return;
		}
		this.checkedSprite = true;
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.hasSpriteRenderer = this.spriteRenderer;
		if (!this.hasSpriteRenderer)
		{
			return;
		}
		this.initialSprite = this.spriteRenderer.sprite;
		Color color = this.spriteRenderer.color;
		float? a = new float?(1f);
		this.initialColor = color.Where(null, null, null, a);
	}

	// Token: 0x06003AC1 RID: 15041 RVA: 0x00102B28 File Offset: 0x00100D28
	private Color GetColor()
	{
		foreach (GameMapScene.ColorCondition colorCondition in this.altColors)
		{
			if (colorCondition.Condition.IsFulfilled)
			{
				return colorCondition.Color;
			}
		}
		return this.initialColor;
	}

	// Token: 0x06003AC2 RID: 15042 RVA: 0x00102B6C File Offset: 0x00100D6C
	public bool IsOtherMapped(HashSet<string> scenesMapped)
	{
		this.PurgeNull();
		if (this.mappedIfAllMapped == null || this.mappedIfAllMapped.Length == 0)
		{
			return false;
		}
		foreach (GameMapScene gameMapScene in this.mappedIfAllMapped)
		{
			if (!scenesMapped.Contains(gameMapScene.name))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003AC3 RID: 15043 RVA: 0x00102BBC File Offset: 0x00100DBC
	private bool TryGetSpriteBounds(out Bounds bounds)
	{
		Transform transform = base.transform;
		if (this.hasValidBounds && this.currentSprite == this.spriteRenderer.sprite)
		{
			bounds = new Bounds(transform.TransformPoint(this.localSpriteBounds.center), transform.TransformVector(this.localSpriteBounds.size));
			return true;
		}
		this.CacheSpriteBounds();
		if (this.hasValidBounds && this.currentSprite == this.spriteRenderer.sprite)
		{
			bounds = new Bounds(transform.TransformPoint(this.localSpriteBounds.center), transform.TransformVector(this.localSpriteBounds.size));
			return true;
		}
		bounds = default(Bounds);
		return false;
	}

	// Token: 0x06003AC4 RID: 15044 RVA: 0x00102C7C File Offset: 0x00100E7C
	public bool TryGetSpriteBounds(Transform targetSpace, out Bounds bounds)
	{
		if (this.excludeBounds)
		{
			bounds = default(Bounds);
			return false;
		}
		Transform transform = base.transform;
		if (transform.IsChildOf(targetSpace) || transform == targetSpace)
		{
			if (this.hasValidBounds && this.currentSprite == this.spriteRenderer.sprite)
			{
				Vector3 a = transform.localPosition;
				Vector3 vector = transform.localScale;
				Transform parent = transform.parent;
				while (parent != null && parent != targetSpace)
				{
					a = Vector3.Scale(a, parent.localScale) + parent.localPosition;
					vector = Vector3.Scale(vector, parent.localScale);
					parent = parent.parent;
				}
				Vector3 center = a + Vector3.Scale(this.localSpriteBounds.center, vector);
				Vector3 size = Vector3.Scale(this.localSpriteBounds.size, vector);
				bounds = new Bounds(center, size);
				return true;
			}
			this.CacheSpriteBounds();
			if (this.hasValidBounds && this.currentSprite == this.spriteRenderer.sprite)
			{
				Vector3 a2 = transform.localPosition;
				Vector3 vector2 = transform.localScale;
				Transform parent2 = transform.parent;
				while (parent2 != null && parent2 != targetSpace)
				{
					a2 = Vector3.Scale(a2, parent2.localScale) + parent2.localPosition;
					vector2 = Vector3.Scale(vector2, parent2.localScale);
					parent2 = parent2.parent;
				}
				Vector3 center2 = a2 + Vector3.Scale(this.localSpriteBounds.center, vector2);
				Vector3 size2 = Vector3.Scale(this.localSpriteBounds.size, vector2);
				bounds = new Bounds(center2, size2);
				return true;
			}
		}
		else
		{
			if (this.hasValidBounds && this.currentSprite == this.spriteRenderer.sprite)
			{
				bounds = new Bounds(targetSpace.InverseTransformPoint(transform.TransformPoint(this.localSpriteBounds.center)), targetSpace.TransformVector(transform.TransformVector(this.localSpriteBounds.size)));
				return true;
			}
			this.CacheSpriteBounds();
			if (this.hasValidBounds && this.currentSprite == this.spriteRenderer.sprite)
			{
				bounds = new Bounds(targetSpace.InverseTransformPoint(transform.TransformPoint(this.localSpriteBounds.center)), targetSpace.TransformVector(transform.TransformVector(this.localSpriteBounds.size)));
				return true;
			}
		}
		bounds = default(Bounds);
		return false;
	}

	// Token: 0x06003AC5 RID: 15045 RVA: 0x00102F01 File Offset: 0x00101101
	private void CacheSpriteBounds()
	{
		if (this.cachedSpriteBounds)
		{
			return;
		}
		this.UpdateSpriteBounds();
	}

	// Token: 0x06003AC6 RID: 15046 RVA: 0x00102F12 File Offset: 0x00101112
	private void UpdateSpriteBounds()
	{
		this.GetMissingComponents();
		if (this.spriteRenderer != null)
		{
			this.UpdateSpriteBounds(this.spriteRenderer.sprite);
			return;
		}
		this.UpdateSpriteBounds(null);
	}

	// Token: 0x06003AC7 RID: 15047 RVA: 0x00102F41 File Offset: 0x00101141
	private void UpdateSpriteBounds(Sprite sprite)
	{
		this.cachedSpriteBounds = true;
		this.hasValidBounds = (sprite != null);
		this.currentSprite = sprite;
		if (this.hasValidBounds)
		{
			this.localSpriteBounds = GameMapScene.GetCroppedBounds(sprite);
		}
	}

	// Token: 0x06003AC8 RID: 15048 RVA: 0x00102F74 File Offset: 0x00101174
	private static Bounds GetCroppedBounds(Sprite sprite)
	{
		Vector2[] vertices = sprite.vertices;
		Vector2 vector = vertices[0];
		Vector2 vector2 = vertices[0];
		for (int i = 1; i < vertices.Length; i++)
		{
			Vector2 vector3 = vertices[i];
			if (vector3.x < vector.x)
			{
				vector.x = vector3.x;
			}
			if (vector3.y < vector.y)
			{
				vector.y = vector3.y;
			}
			if (vector3.x > vector2.x)
			{
				vector2.x = vector3.x;
			}
			if (vector3.y > vector2.y)
			{
				vector2.y = vector3.y;
			}
		}
		Vector2 v = vector2 - vector;
		Vector2 v2 = (vector + vector2) / 2f;
		return new Bounds(v2, v);
	}

	// Token: 0x06003AC9 RID: 15049 RVA: 0x0010305B File Offset: 0x0010125B
	private void OnDrawGizmosSelected()
	{
		if (GizmoUtility.IsSelfOrChildSelected(base.transform))
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x06003ACA RID: 15050 RVA: 0x00103070 File Offset: 0x00101270
	private void DrawGizmos()
	{
		this.CacheSpriteBounds();
		this.DrawBounds(this.localSpriteBounds, Color.yellow.SetAlpha(0.5f), true);
		Bounds bounds;
		if (this.TryGetSpriteBounds(out bounds))
		{
			this.DrawBounds(bounds, Color.magenta.SetAlpha(0.5f), false);
		}
	}

	// Token: 0x06003ACB RID: 15051 RVA: 0x001030C0 File Offset: 0x001012C0
	private void DrawBounds(Bounds bounds, Color color, bool useLocalMatrix = true)
	{
		Gizmos.color = color;
		if (useLocalMatrix)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
		}
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x06003ACD RID: 15053 RVA: 0x0010310C File Offset: 0x0010130C
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04003D12 RID: 15634
	[SerializeField]
	private GameMapScene mappedParent;

	// Token: 0x04003D13 RID: 15635
	[Space]
	[SerializeField]
	private GameMapScene.States initialState;

	// Token: 0x04003D14 RID: 15636
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsInitialStateRough", true, true, false)]
	private Sprite fullSprite;

	// Token: 0x04003D15 RID: 15637
	[SerializeField]
	private GameMapScene.SpriteCondition[] altFullSprites;

	// Token: 0x04003D16 RID: 15638
	[SerializeField]
	private GameMapScene.ColorCondition[] altColors = new GameMapScene.ColorCondition[0];

	// Token: 0x04003D17 RID: 15639
	[SerializeField]
	private bool unmappedNoBounds;

	// Token: 0x04003D18 RID: 15640
	[Space]
	[SerializeField]
	private GameMapScene[] mappedIfAllMapped;

	// Token: 0x04003D19 RID: 15641
	[Space]
	[Tooltip("Hides map while retaining mapped status")]
	[SerializeField]
	private PlayerDataTest hideCondition;

	// Token: 0x04003D1A RID: 15642
	[SerializeField]
	private bool excludeBounds;

	// Token: 0x04003D1B RID: 15643
	private bool isNameCached;

	// Token: 0x04003D1C RID: 15644
	private string cachedName;

	// Token: 0x04003D1D RID: 15645
	private bool checkedSprite;

	// Token: 0x04003D1E RID: 15646
	private bool hasSpriteRenderer;

	// Token: 0x04003D1F RID: 15647
	private bool hasBeenSet;

	// Token: 0x04003D20 RID: 15648
	private SpriteRenderer spriteRenderer;

	// Token: 0x04003D21 RID: 15649
	private Sprite initialSprite;

	// Token: 0x04003D22 RID: 15650
	private Color initialColor;

	// Token: 0x04003D23 RID: 15651
	private bool purgedNulls;

	// Token: 0x04003D24 RID: 15652
	private bool isMapped;

	// Token: 0x04003D25 RID: 15653
	private bool isVisited;

	// Token: 0x04003D26 RID: 15654
	private bool hasAwaken;

	// Token: 0x04003D27 RID: 15655
	private bool hasStarted;

	// Token: 0x04003D28 RID: 15656
	private bool cachedSpriteBounds;

	// Token: 0x04003D29 RID: 15657
	private bool hasValidBounds;

	// Token: 0x04003D2A RID: 15658
	private Bounds localSpriteBounds;

	// Token: 0x04003D2B RID: 15659
	private Sprite currentSprite;

	// Token: 0x02001978 RID: 6520
	public enum States
	{
		// Token: 0x040095F7 RID: 38391
		Hidden,
		// Token: 0x040095F8 RID: 38392
		Rough,
		// Token: 0x040095F9 RID: 38393
		Full
	}

	// Token: 0x02001979 RID: 6521
	[Serializable]
	private struct SpriteCondition
	{
		// Token: 0x040095FA RID: 38394
		public Sprite Sprite;

		// Token: 0x040095FB RID: 38395
		public PlayerDataTest Condition;
	}

	// Token: 0x0200197A RID: 6522
	[Serializable]
	private struct ColorCondition
	{
		// Token: 0x040095FC RID: 38396
		public Color Color;

		// Token: 0x040095FD RID: 38397
		public PlayerDataTest Condition;
	}
}
