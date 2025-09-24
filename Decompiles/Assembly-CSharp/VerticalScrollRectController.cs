using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200070D RID: 1805
public sealed class VerticalScrollRectController : MonoBehaviour
{
	// Token: 0x0600406D RID: 16493 RVA: 0x0011B5E2 File Offset: 0x001197E2
	public void ScrollUp()
	{
		this.DoScroll(new Vector2(0f, -this.scrollAmount));
	}

	// Token: 0x0600406E RID: 16494 RVA: 0x0011B5FB File Offset: 0x001197FB
	public void ScrollDown()
	{
		this.DoScroll(new Vector2(0f, this.scrollAmount));
	}

	// Token: 0x0600406F RID: 16495 RVA: 0x0011B614 File Offset: 0x00119814
	private void Awake()
	{
		if (this.scrollRect)
		{
			this.scrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnScrollValueChanged));
			this.contentRT = this.scrollRect.content;
			if (this.contentRT)
			{
				this.contentChangedEvent = this.contentRT.gameObject.AddComponentIfNotPresent<RectTransformDimensionChangedEvent>();
				this.contentChangedEvent.DimensionsChanged += this.OnDimensionsChanged;
			}
		}
	}

	// Token: 0x06004070 RID: 16496 RVA: 0x0011B695 File Offset: 0x00119895
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			this.UpdateScrollValues();
		}
	}

	// Token: 0x06004071 RID: 16497 RVA: 0x0011B6A4 File Offset: 0x001198A4
	private void Update()
	{
		this.UpdateIfDirty();
	}

	// Token: 0x06004072 RID: 16498 RVA: 0x0011B6AC File Offset: 0x001198AC
	private void OnDimensionsChanged()
	{
		this.dirty = true;
	}

	// Token: 0x06004073 RID: 16499 RVA: 0x0011B6B8 File Offset: 0x001198B8
	private void OnScrollValueChanged(Vector2 newScroll)
	{
		this.UpdateIfDirty();
		newScroll = this.contentRT.localPosition;
		newScroll = this.ClampScroll(newScroll);
		this.scroll = newScroll.ClampVector2(this.scrollMin, this.scrollMax);
		this.contentRT.localPosition = this.scroll;
	}

	// Token: 0x06004074 RID: 16500 RVA: 0x0011B714 File Offset: 0x00119914
	private void UpdateIfDirty()
	{
		if (this.dirty)
		{
			this.dirty = false;
			this.UpdateScrollValues();
		}
	}

	// Token: 0x06004075 RID: 16501 RVA: 0x0011B72C File Offset: 0x0011992C
	private void UpdateScrollValues()
	{
		RectTransform rectTransform = base.transform as RectTransform;
		if (rectTransform)
		{
			Rect rect = rectTransform.rect;
			this.size = new Vector2(rect.width, rect.height);
			this.extent = this.size * 0.5f;
		}
		if (this.scrollRect)
		{
			this.contentRT = this.scrollRect.content;
		}
		if (this.contentRT)
		{
			Rect rect2 = this.contentRT.rect;
			this.contentSize = new Vector2(rect2.width, rect2.height);
			this.scroll = this.contentRT.localPosition;
		}
		this.scrollMax = this.contentSize - this.size;
		if (this.scrollMax.x < 0f)
		{
			this.scrollMax.x = 0f;
		}
		if (this.scrollMax.y < 0f)
		{
			this.scrollMax.y = 0f;
		}
	}

	// Token: 0x06004076 RID: 16502 RVA: 0x0011B844 File Offset: 0x00119A44
	private Vector2 ClampScroll(Vector2 value)
	{
		if (value.y <= this.scrollMin.y + 1f)
		{
			value.y = this.scrollMin.y;
			this.ToggleTopIndicators(false);
		}
		else if (this.contentSize.y > this.size.y && value.y > 0f)
		{
			this.ToggleTopIndicators(true);
		}
		if (value.y >= this.scrollMax.y - 1f)
		{
			value.y = this.scrollMax.y;
			this.ToggleBottomIndicators(false);
		}
		else if (this.contentSize.y > value.y && value.y < this.contentSize.y - this.extent.y)
		{
			this.ToggleBottomIndicators(true);
		}
		return value;
	}

	// Token: 0x06004077 RID: 16503 RVA: 0x0011B920 File Offset: 0x00119B20
	private void ToggleTopIndicators(bool show)
	{
		if (this.topShown == show)
		{
			return;
		}
		this.topShown = show;
		if (show)
		{
			using (List<ScrollIndicator>.Enumerator enumerator = this.topIndicators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ScrollIndicator scrollIndicator = enumerator.Current;
					scrollIndicator.Show();
				}
				return;
			}
		}
		foreach (ScrollIndicator scrollIndicator2 in this.topIndicators)
		{
			scrollIndicator2.Hide();
		}
	}

	// Token: 0x06004078 RID: 16504 RVA: 0x0011B9C4 File Offset: 0x00119BC4
	private void ToggleBottomIndicators(bool show)
	{
		if (this.bottomShown == show)
		{
			return;
		}
		this.bottomShown = show;
		if (show)
		{
			using (List<ScrollIndicator>.Enumerator enumerator = this.bottomIndicators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ScrollIndicator scrollIndicator = enumerator.Current;
					scrollIndicator.Show();
				}
				return;
			}
		}
		foreach (ScrollIndicator scrollIndicator2 in this.bottomIndicators)
		{
			scrollIndicator2.Hide();
		}
	}

	// Token: 0x06004079 RID: 16505 RVA: 0x0011BA68 File Offset: 0x00119C68
	private void DoScroll(Vector2 amount)
	{
		float y = this.contentRT.localPosition.y + amount.y;
		this.contentRT.localPosition = this.ClampScroll(new Vector2(0f, y));
	}

	// Token: 0x0600407A RID: 16506 RVA: 0x0011BAB0 File Offset: 0x00119CB0
	public void SetScrollTarget(Transform targetTransform, bool isMouse = false)
	{
		this.UpdateIfDirty();
		if (isMouse)
		{
			return;
		}
		float num = Mathf.Abs(targetTransform.localPosition.y);
		if (this.centreFocus)
		{
			num -= this.extent.y;
		}
		this.contentRT.localPosition = this.ClampScroll(new Vector2(0f, num));
	}

	// Token: 0x0600407B RID: 16507 RVA: 0x0011BB0F File Offset: 0x00119D0F
	public void ResetScroll()
	{
		if (this.contentRT)
		{
			this.contentRT.localPosition = this.ClampScroll(Vector2.zero);
		}
	}

	// Token: 0x04004201 RID: 16897
	[SerializeField]
	private ScrollRect scrollRect;

	// Token: 0x04004202 RID: 16898
	[SerializeField]
	private List<ScrollIndicator> topIndicators = new List<ScrollIndicator>();

	// Token: 0x04004203 RID: 16899
	[SerializeField]
	private List<ScrollIndicator> bottomIndicators = new List<ScrollIndicator>();

	// Token: 0x04004204 RID: 16900
	[Space]
	[SerializeField]
	private Vector2 size;

	// Token: 0x04004205 RID: 16901
	[SerializeField]
	private Vector2 extent;

	// Token: 0x04004206 RID: 16902
	[SerializeField]
	private Vector2 contentSize;

	// Token: 0x04004207 RID: 16903
	[SerializeField]
	private Vector2 scrollMin = Vector2.zero;

	// Token: 0x04004208 RID: 16904
	[SerializeField]
	private Vector2 scrollMax;

	// Token: 0x04004209 RID: 16905
	[Space]
	[SerializeField]
	private Vector2 scroll;

	// Token: 0x0400420A RID: 16906
	[SerializeField]
	private float scrollAmount = 260f;

	// Token: 0x0400420B RID: 16907
	[SerializeField]
	private bool centreFocus;

	// Token: 0x0400420C RID: 16908
	private RectTransform contentRT;

	// Token: 0x0400420D RID: 16909
	private RectTransformDimensionChangedEvent contentChangedEvent;

	// Token: 0x0400420E RID: 16910
	private bool topShown;

	// Token: 0x0400420F RID: 16911
	private bool bottomShown;

	// Token: 0x04004210 RID: 16912
	private bool dirty = true;
}
