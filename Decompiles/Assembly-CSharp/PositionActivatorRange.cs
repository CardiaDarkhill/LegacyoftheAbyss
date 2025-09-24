using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200047C RID: 1148
public class PositionActivatorRange : MonoBehaviour
{
	// Token: 0x06002992 RID: 10642 RVA: 0x000B4FC0 File Offset: 0x000B31C0
	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		float? y = new float?(this.GetRelativeYPosition(this.range.Start));
		Gizmos.DrawWireSphere(position.Where(null, y, null), 0.25f);
		y = new float?(this.GetRelativeYPosition(this.range.End));
		Gizmos.DrawWireSphere(position.Where(null, y, null), 0.25f);
	}

	// Token: 0x06002993 RID: 10643 RVA: 0x000B504D File Offset: 0x000B324D
	private void OnEnable()
	{
		this.Evaluate(true);
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x000B5056 File Offset: 0x000B3256
	private void Update()
	{
		this.Evaluate(false);
	}

	// Token: 0x06002995 RID: 10645 RVA: 0x000B5060 File Offset: 0x000B3260
	private void Evaluate(bool force)
	{
		MinMaxFloat minMaxFloat = new MinMaxFloat(this.GetRelativeYPosition(this.range.Start), this.GetRelativeYPosition(this.range.End));
		bool flag = minMaxFloat.IsInRange(base.transform.position.y);
		if (flag != this.wasInside || force)
		{
			this.IsNowInside.Invoke(flag);
			this.IsNowOutside.Invoke(!flag);
			if (flag)
			{
				this.IsJustInside.Invoke();
			}
			else
			{
				this.IsJustOutside.Invoke();
			}
			this.wasInside = flag;
		}
	}

	// Token: 0x06002996 RID: 10646 RVA: 0x000B50FC File Offset: 0x000B32FC
	private float GetRelativeYPosition(float fromPos)
	{
		Vector2 vector = new Vector2(0f, fromPos);
		return (this.relativeTo ? this.relativeTo.TransformPoint(vector) : vector).y;
	}

	// Token: 0x04002A25 RID: 10789
	[SerializeField]
	private Transform relativeTo;

	// Token: 0x04002A26 RID: 10790
	[SerializeField]
	private MinMaxFloat range;

	// Token: 0x04002A27 RID: 10791
	[Space]
	public PositionActivatorRange.UnityBoolEvent IsNowInside;

	// Token: 0x04002A28 RID: 10792
	public UnityEvent IsJustInside;

	// Token: 0x04002A29 RID: 10793
	public PositionActivatorRange.UnityBoolEvent IsNowOutside;

	// Token: 0x04002A2A RID: 10794
	public UnityEvent IsJustOutside;

	// Token: 0x04002A2B RID: 10795
	private bool wasInside;

	// Token: 0x0200178A RID: 6026
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}
}
