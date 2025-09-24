using System;
using UnityEngine;

// Token: 0x020006F6 RID: 1782
public class PositionConditions : MonoBehaviour
{
	// Token: 0x06003FD5 RID: 16341 RVA: 0x001196F8 File Offset: 0x001178F8
	private void OnDrawGizmosSelected()
	{
		if (this.positionsOrdered == null)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		PositionConditions.Position[] array = this.positionsOrdered;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawWireSphere(array[i].Offset, 0.2f);
		}
	}

	// Token: 0x06003FD6 RID: 16342 RVA: 0x0011974A File Offset: 0x0011794A
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.Evaluate();
	}

	// Token: 0x06003FD7 RID: 16343 RVA: 0x0011975B File Offset: 0x0011795B
	private void Start()
	{
		this.Evaluate();
	}

	// Token: 0x06003FD8 RID: 16344 RVA: 0x00119764 File Offset: 0x00117964
	public void Evaluate()
	{
		Transform transform = base.transform;
		if (!this.hasStarted)
		{
			this.initialLocalPos = transform.localPosition;
		}
		this.hasStarted = true;
		Vector2 position = this.initialLocalPos;
		Vector3 localScale = Vector3.one;
		foreach (PositionConditions.Position position2 in this.positionsOrdered)
		{
			if (position2.Condition.IsFulfilled)
			{
				position = this.initialLocalPos + position2.Offset;
				localScale = position2.Scale;
				break;
			}
		}
		transform.SetLocalPosition2D(position);
		transform.localScale = localScale;
	}

	// Token: 0x04004177 RID: 16759
	[SerializeField]
	private PositionConditions.Position[] positionsOrdered;

	// Token: 0x04004178 RID: 16760
	private bool hasStarted;

	// Token: 0x04004179 RID: 16761
	private Vector3 initialLocalPos;

	// Token: 0x020019E6 RID: 6630
	[Serializable]
	private class Position
	{
		// Token: 0x040097A2 RID: 38818
		public PlayerDataTest Condition;

		// Token: 0x040097A3 RID: 38819
		public Vector2 Offset;

		// Token: 0x040097A4 RID: 38820
		public Vector3 Scale = Vector3.one;
	}
}
