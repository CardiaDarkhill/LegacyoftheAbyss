using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200067E RID: 1662
public class InventoryCursor : MonoBehaviour
{
	// Token: 0x170006B6 RID: 1718
	// (get) Token: 0x06003B7A RID: 15226 RVA: 0x001057B2 File Offset: 0x001039B2
	private float MoveTime
	{
		get
		{
			if (!this.paneInput)
			{
				return this.moveTime;
			}
			return Mathf.Min(this.moveTime, this.paneInput.ListScrollSpeed);
		}
	}

	// Token: 0x06003B7B RID: 15227 RVA: 0x001057E0 File Offset: 0x001039E0
	private void Awake()
	{
		if (this.backGlow)
		{
			this.defaultGlowColor = this.backGlow.color;
		}
		InventoryItemManager componentInParent = base.GetComponentInParent<InventoryItemManager>(true);
		if (componentInParent)
		{
			this.paneInput = componentInParent.GetComponent<InventoryPaneInput>();
		}
		if (this.paneInput)
		{
			this.paneInput.OnActivated += delegate()
			{
				if (!this.currentTarget)
				{
					return;
				}
				this.skipNextLerp = false;
				this.queueActivate = true;
				this.SetTarget(this.currentTarget);
			};
			this.paneInput.OnDeactivated += this.Deactivate;
		}
		InventoryPane componentInParent2 = base.GetComponentInParent<InventoryPane>(true);
		if (componentInParent2)
		{
			this.Deactivate();
			if (this.paneInput)
			{
				componentInParent2.OnPaneStart += this.Deactivate;
			}
			else
			{
				componentInParent2.OnPaneStart += delegate()
				{
					this.queueActivate = true;
				};
			}
			componentInParent2.OnPaneEnd += this.Deactivate;
			this.queueActivate = true;
		}
	}

	// Token: 0x06003B7C RID: 15228 RVA: 0x001058C4 File Offset: 0x00103AC4
	public void ActivateIfNotActive()
	{
		if (!base.gameObject.activeSelf)
		{
			this.Activate(true);
		}
	}

	// Token: 0x06003B7D RID: 15229 RVA: 0x001058DA File Offset: 0x00103ADA
	public void Activate()
	{
		this.Activate(false);
	}

	// Token: 0x06003B7E RID: 15230 RVA: 0x001058E3 File Offset: 0x00103AE3
	public void Deactivate()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003B7F RID: 15231 RVA: 0x001058F1 File Offset: 0x00103AF1
	public void Activate(bool setPreviousTarget)
	{
		this.ResetAppearance();
		this.skipNextLerp = true;
		if (setPreviousTarget)
		{
			this.SetTarget(this.currentTarget);
		}
	}

	// Token: 0x06003B80 RID: 15232 RVA: 0x00105910 File Offset: 0x00103B10
	private void ResetAppearance()
	{
		if (this.back)
		{
			this.back.SetLocalPosition2D(0f, 0f);
			this.back.localScale = Vector3.one;
			this.back.gameObject.SetActive(true);
		}
		if (this.bottomLeft)
		{
			this.bottomLeft.SetLocalPosition2D(-0.5f, -0.5f);
			this.bottomLeft.gameObject.SetActive(true);
		}
		if (this.bottomRight)
		{
			this.bottomRight.SetLocalPosition2D(0.5f, -0.5f);
			this.bottomRight.gameObject.SetActive(true);
		}
		if (this.topLeft)
		{
			this.topLeft.SetLocalPosition2D(-0.5f, 0.5f);
			this.topLeft.gameObject.SetActive(true);
		}
		if (this.topRight)
		{
			this.topRight.SetLocalPosition2D(0.5f, 0.5f);
			this.topRight.gameObject.SetActive(true);
		}
		if (this.backGlow)
		{
			this.backGlow.color = this.defaultGlowColor;
		}
	}

	// Token: 0x06003B81 RID: 15233 RVA: 0x00105A4A File Offset: 0x00103C4A
	private void LateUpdate()
	{
		if (!this.currentTarget)
		{
			return;
		}
		base.transform.SetPosition2D(this.GetClampedTargetPos(this.currentTarget.position));
	}

	// Token: 0x06003B82 RID: 15234 RVA: 0x00105A7C File Offset: 0x00103C7C
	private Vector2 GetClampedTargetPos(Vector2 pos)
	{
		if (!this.isPosClamped)
		{
			return pos;
		}
		if (pos.x < this.minPos.x)
		{
			pos.x = this.minPos.x;
		}
		if (pos.y < this.minPos.y)
		{
			pos.y = this.minPos.y;
		}
		if (pos.x > this.maxPos.x)
		{
			pos.x = this.maxPos.x;
		}
		if (pos.y > this.maxPos.y)
		{
			pos.y = this.maxPos.y;
		}
		return pos;
	}

	// Token: 0x06003B83 RID: 15235 RVA: 0x00105B28 File Offset: 0x00103D28
	public void SetClampedPos(Vector2 min, Vector2 max)
	{
		this.isPosClamped = true;
		this.minPos = min;
		this.maxPos = max;
	}

	// Token: 0x06003B84 RID: 15236 RVA: 0x00105B3F File Offset: 0x00103D3F
	public void ResetClampedPos()
	{
		this.isPosClamped = false;
	}

	// Token: 0x06003B85 RID: 15237 RVA: 0x00105B48 File Offset: 0x00103D48
	public void SetTarget(Transform target)
	{
		Transform y = this.currentTarget;
		this.currentTarget = target;
		if (!target)
		{
			return;
		}
		if (this.paneInput && !this.paneInput.isActiveAndEnabled)
		{
			return;
		}
		if (this.currentTarget != y)
		{
			this.changeSelectionSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		}
		Vector2 glowScale = Vector2.one;
		if (this.skipNextLerp)
		{
			BoxCollider2D component = target.GetComponent<BoxCollider2D>();
			Vector2 pos = component ? target.TransformPoint(component.offset) : this.currentTarget.position;
			base.transform.SetPosition2D(this.GetClampedTargetPos(pos));
			InventoryCursor.ICursorTarget component2 = target.GetComponent<InventoryCursor.ICursorTarget>();
			if (component2 != null && !component2.ShowCursor && this.group)
			{
				this.group.AlphaSelf = 0f;
			}
			base.gameObject.SetActive(true);
			if (this.backGlow)
			{
				Color color = this.defaultGlowColor;
				InventoryItemSelectable component3 = target.GetComponent<InventoryItemSelectable>();
				if (component3)
				{
					color = (component3.CursorColor ?? color);
					glowScale = component3.CursorGlowScale;
				}
				this.backGlow.color = color;
			}
			this.skipNextLerp = false;
		}
		if (this.moveRoutine != null)
		{
			base.StopCoroutine(this.moveRoutine);
			this.moveRoutine = null;
		}
		if (base.gameObject.activeInHierarchy)
		{
			this.moveRoutine = base.StartCoroutine(this.MoveTo(target, glowScale, true));
		}
		else
		{
			this.MoveTo(target, glowScale, false);
			if (this.queueActivate)
			{
				this.Activate();
			}
		}
		this.queueActivate = false;
	}

	// Token: 0x06003B86 RID: 15238 RVA: 0x00105D06 File Offset: 0x00103F06
	private IEnumerator MoveTo(Transform target, Vector2 glowScale, bool doAnim)
	{
		InventoryCursor.<>c__DisplayClass33_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.topRightInitialPos = (this.topRight ? this.topRight.position : Vector2.zero);
		CS$<>8__locals1.topLeftInitialPos = (this.topLeft ? this.topLeft.position : Vector2.zero);
		CS$<>8__locals1.bottomRightInitialPos = (this.bottomRight ? this.bottomRight.position : Vector2.zero);
		CS$<>8__locals1.bottomLeftInitialPos = (this.bottomLeft ? this.bottomLeft.position : Vector2.zero);
		CS$<>8__locals1.backInitialPos = (this.back ? this.back.position : Vector2.zero);
		base.transform.SetPosition2D(this.GetClampedTargetPos(target.position));
		if (this.topRight)
		{
			this.topRight.SetPosition2D(CS$<>8__locals1.topRightInitialPos);
			CS$<>8__locals1.topRightInitialPos = this.topRight.localPosition;
		}
		if (this.topLeft)
		{
			this.topLeft.SetPosition2D(CS$<>8__locals1.topLeftInitialPos);
			CS$<>8__locals1.topLeftInitialPos = this.topLeft.localPosition;
		}
		if (this.bottomRight)
		{
			this.bottomRight.SetPosition2D(CS$<>8__locals1.bottomRightInitialPos);
			CS$<>8__locals1.bottomRightInitialPos = this.bottomRight.localPosition;
		}
		if (this.bottomLeft)
		{
			this.bottomLeft.SetPosition2D(CS$<>8__locals1.bottomLeftInitialPos);
			CS$<>8__locals1.bottomLeftInitialPos = this.bottomLeft.localPosition;
		}
		if (this.back)
		{
			this.back.SetPosition2D(CS$<>8__locals1.backInitialPos);
			CS$<>8__locals1.backInitialPos = this.back.localPosition;
		}
		CS$<>8__locals1.backInitialScale = (this.back ? this.back.localScale : Vector3.one);
		BoxCollider2D component = target.GetComponent<BoxCollider2D>();
		CS$<>8__locals1.boxOffset = (component ? component.offset : Vector2.zero);
		if (component)
		{
			CS$<>8__locals1.boxScale = target.TransformVector(component.size);
			if (base.transform.parent)
			{
				CS$<>8__locals1.boxScale = CS$<>8__locals1.boxScale.MultiplyElements(Vector2.one.DivideElements(base.transform.parent.lossyScale));
			}
		}
		else
		{
			CS$<>8__locals1.boxScale = Vector2.one;
		}
		CS$<>8__locals1.cornerOffset = CS$<>8__locals1.boxScale / 2f;
		CS$<>8__locals1.startColor = (this.backGlow ? this.backGlow.color : Color.white);
		CS$<>8__locals1.newColor = this.defaultGlowColor;
		bool flag = true;
		InventoryCursor.ICursorTarget component2 = target.GetComponent<InventoryCursor.ICursorTarget>();
		if (component2 != null)
		{
			CS$<>8__locals1.newColor = (component2.CursorColor ?? CS$<>8__locals1.newColor);
			flag = component2.ShowCursor;
		}
		CS$<>8__locals1.boxScale = CS$<>8__locals1.boxScale.MultiplyElements(glowScale);
		CS$<>8__locals1.startGroupAlpha = (this.group ? this.group.AlphaSelf : 1f);
		CS$<>8__locals1.targetGroupAlpha = (float)(flag ? 1 : 0);
		if (doAnim)
		{
			for (float elapsed = 0f; elapsed < this.MoveTime; elapsed += Time.unscaledDeltaTime)
			{
				float time = elapsed / this.MoveTime;
				this.<MoveTo>g__SetPositions|33_0(time, ref CS$<>8__locals1);
				yield return null;
			}
		}
		this.<MoveTo>g__SetPositions|33_0(1f, ref CS$<>8__locals1);
		this.moveRoutine = null;
		yield break;
	}

	// Token: 0x06003B8A RID: 15242 RVA: 0x00105D70 File Offset: 0x00103F70
	[CompilerGenerated]
	private void <MoveTo>g__SetPositions|33_0(float time, ref InventoryCursor.<>c__DisplayClass33_0 A_2)
	{
		Vector2 cornerOffset = A_2.cornerOffset;
		if (this.topRight)
		{
			this.topRight.SetLocalPosition2D(Vector2.Lerp(A_2.topRightInitialPos, A_2.boxOffset + cornerOffset, time));
		}
		cornerOffset.x *= -1f;
		if (this.topLeft)
		{
			this.topLeft.SetLocalPosition2D(Vector2.Lerp(A_2.topLeftInitialPos, A_2.boxOffset + cornerOffset, time));
		}
		cornerOffset.y *= -1f;
		if (this.bottomLeft)
		{
			this.bottomLeft.SetLocalPosition2D(Vector2.Lerp(A_2.bottomLeftInitialPos, A_2.boxOffset + cornerOffset, time));
		}
		cornerOffset.x *= -1f;
		if (this.bottomRight)
		{
			this.bottomRight.SetLocalPosition2D(Vector2.Lerp(A_2.bottomRightInitialPos, A_2.boxOffset + cornerOffset, time));
		}
		if (this.back)
		{
			this.back.SetLocalPosition2D(Vector2.Lerp(A_2.backInitialPos, A_2.boxOffset, time));
			this.back.localScale = Vector3.Lerp(A_2.backInitialScale, A_2.boxScale, time);
		}
		if (this.backGlow)
		{
			this.backGlow.color = Color.Lerp(A_2.startColor, A_2.newColor, time);
		}
		if (this.group)
		{
			this.group.AlphaSelf = Mathf.Lerp(A_2.startGroupAlpha, A_2.targetGroupAlpha, time);
		}
	}

	// Token: 0x04003DB0 RID: 15792
	[SerializeField]
	private Transform topLeft;

	// Token: 0x04003DB1 RID: 15793
	[SerializeField]
	private Transform topRight;

	// Token: 0x04003DB2 RID: 15794
	[SerializeField]
	private Transform bottomLeft;

	// Token: 0x04003DB3 RID: 15795
	[SerializeField]
	private Transform bottomRight;

	// Token: 0x04003DB4 RID: 15796
	[SerializeField]
	private Transform back;

	// Token: 0x04003DB5 RID: 15797
	[SerializeField]
	private SpriteRenderer backGlow;

	// Token: 0x04003DB6 RID: 15798
	private Color defaultGlowColor;

	// Token: 0x04003DB7 RID: 15799
	[SerializeField]
	public float moveTime = 0.15f;

	// Token: 0x04003DB8 RID: 15800
	[SerializeField]
	private NestedFadeGroupBase group;

	// Token: 0x04003DB9 RID: 15801
	[Space]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	// Token: 0x04003DBA RID: 15802
	[SerializeField]
	private AudioEvent changeSelectionSound;

	// Token: 0x04003DBB RID: 15803
	private Coroutine moveRoutine;

	// Token: 0x04003DBC RID: 15804
	private Transform currentTarget;

	// Token: 0x04003DBD RID: 15805
	private InventoryPaneInput paneInput;

	// Token: 0x04003DBE RID: 15806
	private bool skipNextLerp;

	// Token: 0x04003DBF RID: 15807
	private bool queueActivate;

	// Token: 0x04003DC0 RID: 15808
	private bool isPosClamped;

	// Token: 0x04003DC1 RID: 15809
	private Vector2 minPos;

	// Token: 0x04003DC2 RID: 15810
	private Vector2 maxPos;

	// Token: 0x02001983 RID: 6531
	public interface ICursorTarget
	{
		// Token: 0x170010A4 RID: 4260
		// (get) Token: 0x0600946A RID: 37994
		bool ShowCursor { get; }

		// Token: 0x170010A5 RID: 4261
		// (get) Token: 0x0600946B RID: 37995
		Color? CursorColor { get; }
	}
}
