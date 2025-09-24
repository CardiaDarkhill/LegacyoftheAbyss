using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200066C RID: 1644
public class ScrollView : MonoBehaviour
{
	// Token: 0x170006AB RID: 1707
	// (get) Token: 0x06003B03 RID: 15107 RVA: 0x00103EF6 File Offset: 0x001020F6
	private float ScrollTime
	{
		get
		{
			if (!this.paneInput)
			{
				return this.scrollTime;
			}
			return this.paneInput.ListScrollSpeed;
		}
	}

	// Token: 0x170006AC RID: 1708
	// (get) Token: 0x06003B04 RID: 15108 RVA: 0x00103F17 File Offset: 0x00102117
	public Bounds ViewBounds
	{
		get
		{
			return this.viewBounds;
		}
	}

	// Token: 0x06003B05 RID: 15109 RVA: 0x00103F20 File Offset: 0x00102120
	private void OnDrawGizmosSelected()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		if (parent)
		{
			Vector3 position = parent.position;
			Vector3 vector = position + this.viewBounds.center;
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(vector, this.viewBounds.size);
			float y = position.y + this.viewBounds.center.y + this.scrollTopWidth;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(new Vector3(vector.x - 1f, y), new Vector3(vector.x + 1f, y));
			float y2 = position.y + this.viewBounds.center.y - this.scrollBottomWidth;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(new Vector3(vector.x - 1f, y2), new Vector3(vector.x + 1f, y2));
		}
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(transform.position + this.contentBounds.center, this.contentBounds.size);
	}

	// Token: 0x06003B06 RID: 15110 RVA: 0x0010404A File Offset: 0x0010224A
	private void OnValidate()
	{
		this.FullUpdate();
	}

	// Token: 0x06003B07 RID: 15111 RVA: 0x00104052 File Offset: 0x00102252
	private void OnEnable()
	{
		this.FullUpdate();
	}

	// Token: 0x06003B08 RID: 15112 RVA: 0x0010405C File Offset: 0x0010225C
	private void Start()
	{
		this.UpdateArrows(true);
		InventoryItemManager componentInParent = base.GetComponentInParent<InventoryItemManager>();
		if (componentInParent)
		{
			this.paneInput = componentInParent.GetComponent<InventoryPaneInput>();
		}
	}

	// Token: 0x06003B09 RID: 15113 RVA: 0x0010408C File Offset: 0x0010228C
	private void LateUpdate()
	{
		if (base.transform.localPosition == this.lastPosition)
		{
			return;
		}
		this.ClampPosition();
		if (base.transform.localPosition == this.lastPosition)
		{
			return;
		}
		this.UpdateArrows(false);
		this.lastPosition = base.transform.localPosition;
	}

	// Token: 0x06003B0A RID: 15114 RVA: 0x001040F8 File Offset: 0x001022F8
	private void OnTransformChildrenChanged()
	{
		this.FullUpdate();
	}

	// Token: 0x06003B0B RID: 15115 RVA: 0x00104100 File Offset: 0x00102300
	public void FullUpdate()
	{
		this.CalculateContentBounds();
		this.ClampPosition();
		this.UpdateArrows(true);
	}

	// Token: 0x06003B0C RID: 15116 RVA: 0x00104118 File Offset: 0x00102318
	private void CalculateContentBounds()
	{
		if (!this.useChildColliders)
		{
			return;
		}
		Collider2D[] componentsInChildren = base.GetComponentsInChildren<Collider2D>();
		if (componentsInChildren.Length == 0)
		{
			this.contentBounds.SetMinMax(Vector3.zero, Vector3.zero);
			return;
		}
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		Collider2D[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Bounds bounds = array[i].bounds;
			if (bounds.max.x > num2)
			{
				num2 = bounds.max.x;
			}
			if (bounds.min.x < num)
			{
				num = bounds.min.x;
			}
			if (bounds.max.y > num4)
			{
				num4 = bounds.max.y;
			}
			if (bounds.min.y < num3)
			{
				num3 = bounds.min.y;
			}
		}
		Vector2 vector = new Vector2(num, num3) - this.minMargins;
		Vector2 vector2 = new Vector2(num2, num4) + this.maxMargins;
		Vector2 b = base.transform.position;
		vector -= b;
		vector2 -= b;
		this.contentBounds.SetMinMax(vector, vector2);
	}

	// Token: 0x06003B0D RID: 15117 RVA: 0x0010426C File Offset: 0x0010246C
	private void ClampPosition()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		if (!parent)
		{
			return;
		}
		Bounds bounds = new Bounds(this.viewBounds.center + parent.position, this.viewBounds.size);
		Bounds bounds2 = new Bounds(this.contentBounds.center + transform.position, this.contentBounds.size);
		if (bounds2.size.y < bounds.size.y)
		{
			float num = bounds.size.y - bounds2.size.y;
			Vector2 v = bounds2.min;
			v.y -= num;
			bounds2.min = v;
		}
		float num2 = bounds.max.y - bounds2.max.y;
		float num3 = bounds2.min.y - bounds.min.y;
		num2 = Mathf.Max(0f, num2);
		num3 = Mathf.Max(0f, num3);
		float y = num3 - num2;
		transform.position -= new Vector3(0f, y, 0f);
	}

	// Token: 0x06003B0E RID: 15118 RVA: 0x001043BC File Offset: 0x001025BC
	public void ScrollTo(Vector2 localPosition, bool isInstant = false)
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		if (!parent)
		{
			return;
		}
		Vector2 b = transform.position;
		Vector2 a = parent.position;
		Vector2 a2 = a + localPosition;
		Vector2 b2 = a + this.viewBounds.center;
		Vector2 b3 = a2 - b2;
		Vector2 vector = a - b3 - b;
		float num = (vector.y > 0f) ? this.scrollBottomWidth : this.scrollTopWidth;
		float b4 = Mathf.Abs(vector.y) - num;
		vector.y = Mathf.Sign(vector.y) * Mathf.Max(0f, b4);
		if (this.scrollRoutine != null)
		{
			base.StopCoroutine(this.scrollRoutine);
			this.scrollRoutine = null;
		}
		if (!isInstant && base.gameObject.activeInHierarchy)
		{
			this.scrollRoutine = base.StartCoroutine(this.ScrollDistance(vector, false));
			return;
		}
		this.ScrollDistance(vector, true).MoveNext();
	}

	// Token: 0x06003B0F RID: 15119 RVA: 0x001044CF File Offset: 0x001026CF
	private IEnumerator ScrollDistance(Vector2 distance, bool isInstant)
	{
		ScrollView.<>c__DisplayClass31_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.startPosition = base.transform.localPosition;
		CS$<>8__locals1.targetPosition = CS$<>8__locals1.startPosition;
		CS$<>8__locals1.targetPosition.y = CS$<>8__locals1.targetPosition.y + distance.y;
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		if (!isInstant)
		{
			for (float elapsed = 0f; elapsed < this.ScrollTime; elapsed += Time.unscaledDeltaTime)
			{
				this.<ScrollDistance>g__UpdatePosition|31_0(elapsed / this.ScrollTime, ref CS$<>8__locals1);
				yield return wait;
			}
		}
		this.<ScrollDistance>g__UpdatePosition|31_0(1f, ref CS$<>8__locals1);
		yield break;
	}

	// Token: 0x06003B10 RID: 15120 RVA: 0x001044EC File Offset: 0x001026EC
	private void UpdateArrows(bool isInstant = false)
	{
		Transform transform = base.transform;
		Bounds bounds = new Bounds(this.viewBounds.center + transform.parent.position, this.viewBounds.size);
		Bounds bounds2 = new Bounds(this.contentBounds.center + transform.position, this.contentBounds.size);
		bool flag = bounds2.max.y > bounds.max.y + 0.01f;
		bool flag2 = bounds2.min.y < bounds.min.y - 0.01f;
		this.UpdateArrow(this.upArrow, isInstant, flag);
		this.UpdateArrow(this.downArrow, isInstant, flag2);
		if (this.topGradient && (isInstant || flag != this.wasOffTop))
		{
			this.topGradient.FadeTo((float)(flag ? 1 : 0), isInstant ? 0f : this.ScrollTime, null, true, null);
			this.wasOffTop = flag;
		}
		if (this.bottomGradient && (isInstant || flag2 != this.wasOffBottom))
		{
			this.bottomGradient.FadeTo((float)(flag2 ? 1 : 0), isInstant ? 0f : this.ScrollTime, null, true, null);
			this.wasOffBottom = flag2;
		}
	}

	// Token: 0x06003B11 RID: 15121 RVA: 0x00104646 File Offset: 0x00102846
	private void UpdateArrow(InvAnimateUpAndDown arrow, bool isInstant, bool offTop)
	{
		if (!arrow)
		{
			return;
		}
		if (offTop)
		{
			if (!arrow.IsLastAnimatedDown)
			{
				return;
			}
			if (isInstant)
			{
				arrow.Show();
				return;
			}
			arrow.AnimateUp();
			return;
		}
		else
		{
			if (isInstant)
			{
				arrow.Hide();
				return;
			}
			if (!arrow.IsLastAnimatedDown)
			{
				arrow.AnimateDown();
			}
			return;
		}
	}

	// Token: 0x06003B13 RID: 15123 RVA: 0x00104699 File Offset: 0x00102899
	[CompilerGenerated]
	private void <ScrollDistance>g__UpdatePosition|31_0(float time, ref ScrollView.<>c__DisplayClass31_0 A_2)
	{
		base.transform.localPosition = Vector3.Lerp(A_2.startPosition, A_2.targetPosition, time);
		this.ClampPosition();
	}

	// Token: 0x04003D53 RID: 15699
	[Header("View")]
	[SerializeField]
	private Bounds viewBounds;

	// Token: 0x04003D54 RID: 15700
	[SerializeField]
	private Bounds contentBounds;

	// Token: 0x04003D55 RID: 15701
	[Space]
	[SerializeField]
	private bool useChildColliders;

	// Token: 0x04003D56 RID: 15702
	[SerializeField]
	private Vector2 maxMargins;

	// Token: 0x04003D57 RID: 15703
	[SerializeField]
	private Vector2 minMargins;

	// Token: 0x04003D58 RID: 15704
	[Header("Scroll")]
	[SerializeField]
	private float scrollTopWidth;

	// Token: 0x04003D59 RID: 15705
	[SerializeField]
	private float scrollBottomWidth;

	// Token: 0x04003D5A RID: 15706
	[Space]
	[SerializeField]
	private float scrollTime = 0.2f;

	// Token: 0x04003D5B RID: 15707
	[SerializeField]
	private InvAnimateUpAndDown upArrow;

	// Token: 0x04003D5C RID: 15708
	[SerializeField]
	private NestedFadeGroupBase topGradient;

	// Token: 0x04003D5D RID: 15709
	[SerializeField]
	private InvAnimateUpAndDown downArrow;

	// Token: 0x04003D5E RID: 15710
	[SerializeField]
	private NestedFadeGroupBase bottomGradient;

	// Token: 0x04003D5F RID: 15711
	private Vector2 lastPosition;

	// Token: 0x04003D60 RID: 15712
	private Coroutine scrollRoutine;

	// Token: 0x04003D61 RID: 15713
	private InventoryPaneInput paneInput;

	// Token: 0x04003D62 RID: 15714
	private bool wasOffTop;

	// Token: 0x04003D63 RID: 15715
	private bool wasOffBottom;
}
