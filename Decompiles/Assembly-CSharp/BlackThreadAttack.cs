using System;
using UnityEngine;

// Token: 0x0200029D RID: 669
[CreateAssetMenu(menuName = "Hornet/Black Thread Attack", fileName = "New Attack")]
public class BlackThreadAttack : ScriptableObject
{
	// Token: 0x17000265 RID: 613
	// (get) Token: 0x0600175F RID: 5983 RVA: 0x00069660 File Offset: 0x00067860
	public GameObject Prefab
	{
		get
		{
			return this.prefab;
		}
	}

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x06001760 RID: 5984 RVA: 0x00069668 File Offset: 0x00067868
	public float Duration
	{
		get
		{
			return this.duration;
		}
	}

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x06001761 RID: 5985 RVA: 0x00069670 File Offset: 0x00067870
	public bool CounterRotate
	{
		get
		{
			return this.counterRotate;
		}
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x00069678 File Offset: 0x00067878
	private void OnValidate()
	{
		if (this.attackRangeSize.x < 0f)
		{
			this.attackRangeSize.x = 0f;
		}
		if (this.attackRangeSize.y < 0f)
		{
			this.attackRangeSize.y = 0f;
		}
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x000696CC File Offset: 0x000678CC
	public void DrawAttackRangeGizmos(Vector3 pos)
	{
		Gizmos.color = new Color(0.3f, 0.1f, 0.1f, 1f);
		BlackThreadAttack.RangeShape rangeShape = this.rangeShape;
		if (rangeShape == BlackThreadAttack.RangeShape.Rect)
		{
			Vector2 vector = this.attackRangeSize;
			if (Mathf.Abs(vector.x) <= Mathf.Epsilon)
			{
				vector.x = 1000f;
			}
			if (Mathf.Abs(vector.y) <= Mathf.Epsilon)
			{
				vector.y = 1000f;
			}
			Gizmos.DrawWireCube(pos, vector);
			return;
		}
		if (rangeShape != BlackThreadAttack.RangeShape.Circle)
		{
			throw new ArgumentOutOfRangeException();
		}
		float num = Mathf.Max(this.attackRangeSize.x, this.attackRangeSize.y);
		if (Mathf.Abs(num) > Mathf.Epsilon)
		{
			Gizmos.DrawWireSphere(pos, num);
			return;
		}
	}

	// Token: 0x06001764 RID: 5988 RVA: 0x00069794 File Offset: 0x00067994
	public bool IsInRange(Vector2 point, Vector2 refPoint)
	{
		BlackThreadAttack.RangeShape rangeShape = this.rangeShape;
		if (rangeShape == BlackThreadAttack.RangeShape.Rect)
		{
			Vector2 b = this.attackRangeSize * 0.5f;
			Vector2 vector = refPoint - b;
			Vector2 vector2 = refPoint + b;
			return (Mathf.Abs(this.attackRangeSize.x) <= Mathf.Epsilon || (point.x >= vector.x && point.x <= vector2.x)) && (Mathf.Abs(this.attackRangeSize.y) <= Mathf.Epsilon || (point.y >= vector.y && point.y <= vector2.y));
		}
		if (rangeShape != BlackThreadAttack.RangeShape.Circle)
		{
			throw new ArgumentOutOfRangeException();
		}
		float num = Mathf.Max(this.attackRangeSize.x, this.attackRangeSize.y);
		return Mathf.Abs(num) > Mathf.Epsilon && Vector2.SqrMagnitude(point - refPoint) <= num * num;
	}

	// Token: 0x0400160B RID: 5643
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject prefab;

	// Token: 0x0400160C RID: 5644
	[SerializeField]
	private float duration;

	// Token: 0x0400160D RID: 5645
	[SerializeField]
	private BlackThreadAttack.RangeShape rangeShape;

	// Token: 0x0400160E RID: 5646
	[SerializeField]
	private Vector2 attackRangeSize;

	// Token: 0x0400160F RID: 5647
	[SerializeField]
	private bool counterRotate;

	// Token: 0x02001569 RID: 5481
	private enum RangeShape
	{
		// Token: 0x04008711 RID: 34577
		Rect,
		// Token: 0x04008712 RID: 34578
		Circle
	}
}
