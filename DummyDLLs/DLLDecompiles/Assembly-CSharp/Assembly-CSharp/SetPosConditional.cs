using System;
using UnityEngine;

// Token: 0x0200071A RID: 1818
public class SetPosConditional : MonoBehaviour
{
	// Token: 0x060040AF RID: 16559 RVA: 0x0011C4A7 File Offset: 0x0011A6A7
	private void Awake()
	{
		this.initialPos = base.transform.localPosition;
	}

	// Token: 0x060040B0 RID: 16560 RVA: 0x0011C4C0 File Offset: 0x0011A6C0
	private void OnEnable()
	{
		bool flag = false;
		foreach (SetPosConditional.ConditionPos conditionPos in this.orderedConditions)
		{
			if (conditionPos.Condition.IsFulfilled)
			{
				base.transform.SetLocalPosition2D(conditionPos.Position);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			base.transform.SetLocalPosition2D(this.initialPos);
		}
	}

	// Token: 0x04004231 RID: 16945
	[SerializeField]
	private SetPosConditional.ConditionPos[] orderedConditions;

	// Token: 0x04004232 RID: 16946
	private Vector2 initialPos;

	// Token: 0x02001A04 RID: 6660
	[Serializable]
	private class ConditionPos
	{
		// Token: 0x0400981F RID: 38943
		public Vector2 Position;

		// Token: 0x04009820 RID: 38944
		public PlayerDataTest Condition;
	}
}
