using System;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000CB RID: 203
public abstract class VectorCurveAnimator : BaseAnimator, IUpdateBatchableLateUpdate
{
	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000665 RID: 1637
	// (set) Token: 0x06000666 RID: 1638
	protected abstract Vector3 Vector { get; set; }

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000667 RID: 1639 RVA: 0x00020BA7 File Offset: 0x0001EDA7
	protected Transform CurrentTransform
	{
		get
		{
			if (!this.overrideTransform)
			{
				return base.transform;
			}
			return this.overrideTransform;
		}
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x06000668 RID: 1640 RVA: 0x00020BC4 File Offset: 0x0001EDC4
	public bool ShouldUpdate
	{
		get
		{
			if (this.cullingMode == VectorCurveAnimator.CullingModes.None)
			{
				return true;
			}
			if (this.hasRenderer)
			{
				return this.isVisible;
			}
			if (this.childRenderers == null || this.childRenderers.Length == 0)
			{
				return true;
			}
			Renderer[] array = this.childRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].isVisible)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x06000669 RID: 1641 RVA: 0x00020C1F File Offset: 0x0001EE1F
	// (set) Token: 0x0600066A RID: 1642 RVA: 0x00020C27 File Offset: 0x0001EE27
	public Vector3 Offset
	{
		get
		{
			return this.offset;
		}
		set
		{
			this.offset = value;
		}
	}

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x0600066B RID: 1643 RVA: 0x00020C30 File Offset: 0x0001EE30
	// (set) Token: 0x0600066C RID: 1644 RVA: 0x00020C40 File Offset: 0x0001EE40
	public float OffsetX
	{
		get
		{
			return this.offset.x;
		}
		set
		{
			this.offset = this.offset.Where(new float?(value), null, null);
		}
	}

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x0600066D RID: 1645 RVA: 0x00020C76 File Offset: 0x0001EE76
	// (set) Token: 0x0600066E RID: 1646 RVA: 0x00020C84 File Offset: 0x0001EE84
	public float OffsetY
	{
		get
		{
			return this.offset.y;
		}
		set
		{
			Vector3 original = this.offset;
			float? y = new float?(value);
			this.offset = original.Where(null, y, null);
		}
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x0600066F RID: 1647 RVA: 0x00020CBD File Offset: 0x0001EEBD
	// (set) Token: 0x06000670 RID: 1648 RVA: 0x00020CCC File Offset: 0x0001EECC
	public float OffsetZ
	{
		get
		{
			return this.offset.z;
		}
		set
		{
			Vector3 original = this.offset;
			float? z = new float?(value);
			this.offset = original.Where(null, null, z);
		}
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x06000671 RID: 1649 RVA: 0x00020D05 File Offset: 0x0001EF05
	// (set) Token: 0x06000672 RID: 1650 RVA: 0x00020D0D File Offset: 0x0001EF0D
	public float SpeedMultiplier
	{
		get
		{
			return this.speedMultiplier;
		}
		set
		{
			this.speedMultiplier = value;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000673 RID: 1651 RVA: 0x00020D16 File Offset: 0x0001EF16
	// (set) Token: 0x06000674 RID: 1652 RVA: 0x00020D1E File Offset: 0x0001EF1E
	public bool FreezePosition { get; set; }

	// Token: 0x06000675 RID: 1653 RVA: 0x00020D27 File Offset: 0x0001EF27
	protected virtual bool UsesSpace()
	{
		return true;
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00020D2C File Offset: 0x0001EF2C
	private void OnEnable()
	{
		Renderer component = base.GetComponent<Renderer>();
		bool flag;
		if (component)
		{
			SpriteRenderer spriteRenderer = component as SpriteRenderer;
			flag = (spriteRenderer == null || spriteRenderer.sprite);
		}
		else
		{
			flag = false;
		}
		this.hasRenderer = flag;
		this.childRenderers = ((!this.hasRenderer) ? base.GetComponentsInChildren<Renderer>(true) : null);
		if (!this.updateHandled)
		{
			Type type = base.GetType();
			this.gameObjectModifiers = (from c in base.GetComponents(type).OfType<VectorCurveAnimator>()
			where c.enabled
			select c).ToArray<VectorCurveAnimator>();
			VectorCurveAnimator[] array = this.gameObjectModifiers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].updateHandled = true;
			}
			this.updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
			this.updateBatcher.Add(this);
		}
		if (this.playOnEnable)
		{
			this.StartAnimation();
		}
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00020E1C File Offset: 0x0001F01C
	private void OnDisable()
	{
		if (this.updateBatcher != null)
		{
			if (this.updateBatcher.Remove(this))
			{
				VectorCurveAnimator[] array = this.gameObjectModifiers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].updateHandled = false;
				}
			}
			this.updateBatcher = null;
		}
		this.childRenderers = null;
		this.isAnimating = false;
		this.setLocalPosition = null;
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00020E7F File Offset: 0x0001F07F
	public void BatchedLateUpdate()
	{
		this.UpdateThis();
		this.UpdateWholeObject();
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00020E90 File Offset: 0x0001F090
	private void UpdateThis()
	{
		if (!this.isAnimating)
		{
			return;
		}
		if (this.delayLeft > 0f)
		{
			this.delayLeft -= Time.deltaTime;
			if (this.delayLeft > 0f)
			{
				return;
			}
		}
		if (this.duration <= 0f)
		{
			Debug.LogError("Vector Curve Animator duration can not be less than or equal to 0!", this);
			return;
		}
		float num = this.isRealtime ? Time.unscaledDeltaTime : Time.deltaTime;
		this.elapsed += num * this.speedMultiplier;
		bool flag;
		if (this.FreezePosition)
		{
			flag = false;
		}
		else
		{
			flag = true;
			double time = this.GetTime();
			if (this.framerate > 0f)
			{
				if (time >= this.nextUpdateTime)
				{
					this.nextUpdateTime = time + (double)(1f / this.framerate);
				}
				else
				{
					flag = false;
				}
			}
		}
		bool flag2 = this.elapsed >= this.duration && !this.loop;
		if (this.elapsed > this.duration)
		{
			this.elapsed %= this.duration;
		}
		if (flag)
		{
			this.setLocalPosition(this.elapsed / this.duration);
		}
		if (flag2)
		{
			this.setLocalPosition(1f);
			this.setLocalPosition = null;
			this.OnStop.Invoke();
			this.isAnimating = false;
			this.doExtraUpdate = true;
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x00020FE4 File Offset: 0x0001F1E4
	private void UpdateWholeObject()
	{
		if (this.gameObjectModifiers == null)
		{
			return;
		}
		VectorCurveAnimator x = null;
		foreach (VectorCurveAnimator vectorCurveAnimator in this.gameObjectModifiers)
		{
			bool flag = this.isAnimating || this.doExtraUpdate;
			vectorCurveAnimator.doExtraUpdate = false;
			if (flag)
			{
				x = vectorCurveAnimator;
			}
		}
		if (x == null)
		{
			return;
		}
		if (x != this)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		bool flag2 = false;
		foreach (VectorCurveAnimator vectorCurveAnimator2 in this.gameObjectModifiers)
		{
			if (vectorCurveAnimator2.hasCurrentOffsetChanged)
			{
				vector += vectorCurveAnimator2.currentOffset;
				this.hasCurrentOffsetChanged = false;
				flag2 = true;
			}
		}
		if (!flag2)
		{
			return;
		}
		this.Vector = this.initialVector.Value + vector;
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x000210AF File Offset: 0x0001F2AF
	private void OnBecameVisible()
	{
		this.isVisible = true;
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x000210B8 File Offset: 0x0001F2B8
	private void OnBecameInvisible()
	{
		this.isVisible = false;
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x000210C1 File Offset: 0x0001F2C1
	public override void StartAnimation()
	{
		this.StartAnimation(false);
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x000210CC File Offset: 0x0001F2CC
	public void StartAnimation(bool isFlipped)
	{
		if (!this.resetOnPlay || this.initialVector == null)
		{
			this.initialVector = new Vector3?(this.Vector);
		}
		this.isAnimating = false;
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.isAnimating = true;
		this.setLocalPosition = delegate(float time)
		{
			this.SetAnimTime(time, isFlipped);
		};
		this.elapsed = this.startElapsed.GetRandomValue();
		this.delayLeft = this.delay;
		this.OnStart.Invoke();
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00021169 File Offset: 0x0001F369
	public void StartAnimationFlipHeroSideX()
	{
		this.StartAnimation(HeroController.instance.transform.position.x > base.transform.position.x);
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00021197 File Offset: 0x0001F397
	public void StartAnimationRandomFlip()
	{
		this.StartAnimation(Random.Range(0, 2) == 0);
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x000211A9 File Offset: 0x0001F3A9
	public void SetAtStart()
	{
		this.SetAtEnd(false);
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x000211B2 File Offset: 0x0001F3B2
	public void SetAtStart(bool isFlipped)
	{
		if (this.initialVector == null)
		{
			this.initialVector = new Vector3?(this.Vector);
		}
		this.SetAnimTime(0f, isFlipped);
		this.doExtraUpdate = true;
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x000211E5 File Offset: 0x0001F3E5
	public void SetAtEnd()
	{
		this.SetAtEnd(false);
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x000211EE File Offset: 0x0001F3EE
	public void SetAtEnd(bool isFlipped)
	{
		if (this.initialVector == null)
		{
			this.initialVector = new Vector3?(this.Vector);
		}
		this.SetAnimTime(1f, isFlipped);
		this.doExtraUpdate = true;
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x00021221 File Offset: 0x0001F421
	public void ForceStop()
	{
		this.Stop(true);
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x0002122A File Offset: 0x0001F42A
	public void StopAtCurrentPoint()
	{
		this.Stop(false);
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00021233 File Offset: 0x0001F433
	private void Stop(bool setAtEnd)
	{
		this.isAnimating = false;
		if (this.setLocalPosition != null)
		{
			if (setAtEnd)
			{
				this.setLocalPosition(1f);
			}
			this.setLocalPosition = null;
			this.BatchedLateUpdate();
		}
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00021264 File Offset: 0x0001F464
	private void SetAnimTime(float time, bool isFlipped)
	{
		time = this.curve.Evaluate(time);
		if (isFlipped)
		{
			time *= -1f;
		}
		this.currentOffset = this.offset * time;
		this.hasCurrentOffsetChanged = true;
		if (this.UpdatedPosition != null)
		{
			this.UpdatedPosition();
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x000212B7 File Offset: 0x0001F4B7
	private double GetTime()
	{
		if (!this.isRealtime)
		{
			return Time.timeAsDouble;
		}
		return Time.unscaledTimeAsDouble;
	}

	// Token: 0x0400063D RID: 1597
	public Action UpdatedPosition;

	// Token: 0x0400063E RID: 1598
	[SerializeField]
	private Transform overrideTransform;

	// Token: 0x0400063F RID: 1599
	[SerializeField]
	private Vector3 offset;

	// Token: 0x04000640 RID: 1600
	[SerializeField]
	[ModifiableProperty]
	[Conditional("UsesSpace", true, true, true)]
	protected Space space = Space.Self;

	// Token: 0x04000641 RID: 1601
	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000642 RID: 1602
	[SerializeField]
	private float duration = 1f;

	// Token: 0x04000643 RID: 1603
	[SerializeField]
	private float delay;

	// Token: 0x04000644 RID: 1604
	[SerializeField]
	private MinMaxFloat startElapsed;

	// Token: 0x04000645 RID: 1605
	[SerializeField]
	private bool isRealtime;

	// Token: 0x04000646 RID: 1606
	private Vector3? initialVector;

	// Token: 0x04000647 RID: 1607
	private Vector3 currentOffset;

	// Token: 0x04000648 RID: 1608
	private bool hasCurrentOffsetChanged;

	// Token: 0x04000649 RID: 1609
	[SerializeField]
	private bool playOnEnable;

	// Token: 0x0400064A RID: 1610
	[SerializeField]
	private bool loop;

	// Token: 0x0400064B RID: 1611
	[SerializeField]
	private bool resetOnPlay = true;

	// Token: 0x0400064C RID: 1612
	[SerializeField]
	private float framerate;

	// Token: 0x0400064D RID: 1613
	[SerializeField]
	private VectorCurveAnimator.CullingModes cullingMode = VectorCurveAnimator.CullingModes.Visibility;

	// Token: 0x0400064E RID: 1614
	[Space]
	public UnityEvent OnStart;

	// Token: 0x0400064F RID: 1615
	public UnityEvent OnStop;

	// Token: 0x04000650 RID: 1616
	private bool isAnimating;

	// Token: 0x04000651 RID: 1617
	private float delayLeft;

	// Token: 0x04000652 RID: 1618
	private float elapsed;

	// Token: 0x04000653 RID: 1619
	private bool doExtraUpdate;

	// Token: 0x04000654 RID: 1620
	private double nextUpdateTime;

	// Token: 0x04000655 RID: 1621
	private float speedMultiplier = 1f;

	// Token: 0x04000656 RID: 1622
	private VectorCurveAnimator[] gameObjectModifiers;

	// Token: 0x04000657 RID: 1623
	private UpdateBatcher updateBatcher;

	// Token: 0x04000658 RID: 1624
	private bool updateHandled;

	// Token: 0x04000659 RID: 1625
	private Action<float> setLocalPosition;

	// Token: 0x0400065A RID: 1626
	private bool hasRenderer;

	// Token: 0x0400065B RID: 1627
	private bool isVisible;

	// Token: 0x0400065C RID: 1628
	private Renderer[] childRenderers;

	// Token: 0x02001437 RID: 5175
	private enum CullingModes
	{
		// Token: 0x04008265 RID: 33381
		None,
		// Token: 0x04008266 RID: 33382
		Visibility
	}
}
