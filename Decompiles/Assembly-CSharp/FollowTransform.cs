using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020004E4 RID: 1252
public class FollowTransform : MonoBehaviour, IUpdateBatchableLateUpdate, IUpdateBatchableUpdate, ChildUpdateOrdered.IUpdateOrderUpdate
{
	// Token: 0x17000520 RID: 1312
	// (get) Token: 0x06002CE5 RID: 11493 RVA: 0x000C455D File Offset: 0x000C275D
	// (set) Token: 0x06002CE6 RID: 11494 RVA: 0x000C4565 File Offset: 0x000C2765
	public Vector3 Offset { get; set; }

	// Token: 0x17000521 RID: 1313
	// (get) Token: 0x06002CE7 RID: 11495 RVA: 0x000C456E File Offset: 0x000C276E
	// (set) Token: 0x06002CE8 RID: 11496 RVA: 0x000C4576 File Offset: 0x000C2776
	public Transform Target
	{
		get
		{
			return this.target;
		}
		set
		{
			this.target = value;
			this.UpdateInitialValues();
		}
	}

	// Token: 0x17000522 RID: 1314
	// (get) Token: 0x06002CE9 RID: 11497 RVA: 0x000C4588 File Offset: 0x000C2788
	public Transform CurrentTarget
	{
		get
		{
			if (this.target && this.target.gameObject.activeInHierarchy)
			{
				return this.target;
			}
			if (this.fallbackTargets != null)
			{
				foreach (Transform transform in this.fallbackTargets)
				{
					if (transform && transform.gameObject.activeInHierarchy)
					{
						return transform;
					}
				}
			}
			return null;
		}
	}

	// Token: 0x17000523 RID: 1315
	// (get) Token: 0x06002CEA RID: 11498 RVA: 0x000C45F4 File Offset: 0x000C27F4
	// (set) Token: 0x06002CEB RID: 11499 RVA: 0x000C45FC File Offset: 0x000C27FC
	public bool UseParent
	{
		get
		{
			return this.useParent;
		}
		set
		{
			this.useParent = value;
			this.UpdateInitialValues();
		}
	}

	// Token: 0x17000524 RID: 1316
	// (get) Token: 0x06002CEC RID: 11500 RVA: 0x000C460B File Offset: 0x000C280B
	public bool ShouldUpdate
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002CED RID: 11501 RVA: 0x000C460E File Offset: 0x000C280E
	private void OnValidate()
	{
		if (this.lerpSpeed < 0f)
		{
			this.lerpSpeed = 0f;
		}
	}

	// Token: 0x06002CEE RID: 11502 RVA: 0x000C4628 File Offset: 0x000C2828
	private void Awake()
	{
		if (this.useRigidbody)
		{
			this.body = base.GetComponent<Rigidbody2D>();
		}
	}

	// Token: 0x06002CEF RID: 11503 RVA: 0x000C463E File Offset: 0x000C283E
	private void OnEnable()
	{
		if (this.updateOrder != FollowTransform.UpdateOrder.Custom)
		{
			this.updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
			this.updateBatcher.Add(this);
		}
	}

	// Token: 0x06002CF0 RID: 11504 RVA: 0x000C4665 File Offset: 0x000C2865
	private void OnDisable()
	{
		if (this.updateBatcher)
		{
			this.updateBatcher.Remove(this);
			this.updateBatcher = null;
		}
		if (this.shouldClearTargetOnDisable)
		{
			this.shouldClearTargetOnDisable = false;
			this.target = null;
		}
	}

	// Token: 0x06002CF1 RID: 11505 RVA: 0x000C469E File Offset: 0x000C289E
	private void Start()
	{
		this.UpdateInitialValues();
	}

	// Token: 0x06002CF2 RID: 11506 RVA: 0x000C46A8 File Offset: 0x000C28A8
	private void UpdateInitialValues()
	{
		if (this.useParent)
		{
			this.target = base.transform.parent;
			base.transform.SetParent(null, true);
		}
		if (this.useHero)
		{
			HeroController instance = HeroController.instance;
			if (instance)
			{
				this.target = instance.transform;
			}
		}
		Transform currentTarget = this.CurrentTarget;
		if (!currentTarget)
		{
			return;
		}
		this.initialPosition = base.transform.position;
		if (!this.keepOffset)
		{
			return;
		}
		Vector3 offset = this.initialPosition - currentTarget.position;
		if (!this.keepOffsetZ)
		{
			offset.z = 0f;
		}
		this.Offset = offset;
	}

	// Token: 0x06002CF3 RID: 11507 RVA: 0x000C4754 File Offset: 0x000C2954
	public void BatchedUpdate()
	{
		if (this.updateOrder != FollowTransform.UpdateOrder.Update)
		{
			return;
		}
		this.DoUpdate();
	}

	// Token: 0x06002CF4 RID: 11508 RVA: 0x000C4765 File Offset: 0x000C2965
	public void BatchedLateUpdate()
	{
		if (this.updateOrder != FollowTransform.UpdateOrder.LateUpdate)
		{
			return;
		}
		this.DoUpdate();
	}

	// Token: 0x06002CF5 RID: 11509 RVA: 0x000C4778 File Offset: 0x000C2978
	private void DoUpdate()
	{
		if (this.deparent && !this.didDeparent)
		{
			base.transform.SetParent(null);
			this.didDeparent = true;
		}
		Transform currentTarget = this.CurrentTarget;
		if (!currentTarget)
		{
			return;
		}
		Vector3 vector = base.transform.position;
		Vector3 vector2 = currentTarget.position + this.Offset;
		vector2.z += this.zOffset;
		if (this.lerpSpeed > Mathf.Epsilon)
		{
			vector2 = Vector3.Lerp(vector, vector2, this.lerpSpeed * Time.deltaTime);
		}
		if ((this.followAxes & FollowTransform.FollowAxes.X) == FollowTransform.FollowAxes.X)
		{
			vector.x = vector2.x;
		}
		if ((this.followAxes & FollowTransform.FollowAxes.Y) == FollowTransform.FollowAxes.Y)
		{
			vector.y = vector2.y;
		}
		if ((this.followAxes & FollowTransform.FollowAxes.Z) == FollowTransform.FollowAxes.Z)
		{
			vector.z = vector2.z;
		}
		if (this.lerpFromInitialPosition < 1f - Mathf.Epsilon)
		{
			vector = Vector3.Lerp(this.initialPosition, vector, this.lerpFromInitialPosition);
		}
		if (this.useRigidbody)
		{
			if (this.body)
			{
				this.body.MovePosition(vector);
				return;
			}
		}
		else
		{
			base.transform.position = vector;
		}
	}

	// Token: 0x06002CF6 RID: 11510 RVA: 0x000C48A9 File Offset: 0x000C2AA9
	public void ForceFollowUpdate()
	{
		this.DoUpdate();
	}

	// Token: 0x06002CF7 RID: 11511 RVA: 0x000C48B1 File Offset: 0x000C2AB1
	public void UpdateOrderUpdate()
	{
		this.DoUpdate();
	}

	// Token: 0x06002CF8 RID: 11512 RVA: 0x000C48B9 File Offset: 0x000C2AB9
	public void ClearTargetOnDisable()
	{
		this.shouldClearTargetOnDisable = true;
	}

	// Token: 0x04002E92 RID: 11922
	[SerializeField]
	private Transform target;

	// Token: 0x04002E93 RID: 11923
	[SerializeField]
	private Transform[] fallbackTargets;

	// Token: 0x04002E94 RID: 11924
	[SerializeField]
	private bool useParent;

	// Token: 0x04002E95 RID: 11925
	[SerializeField]
	private bool useHero;

	// Token: 0x04002E96 RID: 11926
	[SerializeField]
	private bool keepOffset = true;

	// Token: 0x04002E97 RID: 11927
	[SerializeField]
	[ModifiableProperty]
	[Conditional("keepOffset", true, false, false)]
	private bool keepOffsetZ = true;

	// Token: 0x04002E98 RID: 11928
	[SerializeField]
	private float zOffset;

	// Token: 0x04002E99 RID: 11929
	[SerializeField]
	[EnumPickerBitmask]
	private FollowTransform.FollowAxes followAxes = FollowTransform.FollowAxes.All;

	// Token: 0x04002E9A RID: 11930
	[SerializeField]
	private bool useRigidbody;

	// Token: 0x04002E9B RID: 11931
	[SerializeField]
	private FollowTransform.UpdateOrder updateOrder;

	// Token: 0x04002E9C RID: 11932
	[SerializeField]
	private float lerpSpeed;

	// Token: 0x04002E9D RID: 11933
	[SerializeField]
	private bool deparent;

	// Token: 0x04002E9E RID: 11934
	[SerializeField]
	[Range(0f, 1f)]
	private float lerpFromInitialPosition = 1f;

	// Token: 0x04002E9F RID: 11935
	private bool shouldClearTargetOnDisable;

	// Token: 0x04002EA0 RID: 11936
	private bool didDeparent;

	// Token: 0x04002EA1 RID: 11937
	private Vector3 initialPosition;

	// Token: 0x04002EA2 RID: 11938
	private UpdateBatcher updateBatcher;

	// Token: 0x04002EA4 RID: 11940
	private Rigidbody2D body;

	// Token: 0x020017EB RID: 6123
	[Flags]
	private enum FollowAxes
	{
		// Token: 0x04008FED RID: 36845
		None = 0,
		// Token: 0x04008FEE RID: 36846
		X = 1,
		// Token: 0x04008FEF RID: 36847
		Y = 2,
		// Token: 0x04008FF0 RID: 36848
		Z = 4,
		// Token: 0x04008FF1 RID: 36849
		All = -1
	}

	// Token: 0x020017EC RID: 6124
	private enum UpdateOrder
	{
		// Token: 0x04008FF3 RID: 36851
		Update,
		// Token: 0x04008FF4 RID: 36852
		LateUpdate,
		// Token: 0x04008FF5 RID: 36853
		Custom
	}
}
