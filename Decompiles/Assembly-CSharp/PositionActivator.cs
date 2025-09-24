using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200047B RID: 1147
public class PositionActivator : MonoBehaviour
{
	// Token: 0x0600298C RID: 10636 RVA: 0x000B4E98 File Offset: 0x000B3098
	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		float? y = new float?(this.GetRelativePosition().y);
		Gizmos.DrawWireSphere(position.Where(null, y, null), 0.25f);
	}

	// Token: 0x0600298D RID: 10637 RVA: 0x000B4EE4 File Offset: 0x000B30E4
	private void OnEnable()
	{
		this.Evaluate(true);
	}

	// Token: 0x0600298E RID: 10638 RVA: 0x000B4EED File Offset: 0x000B30ED
	private void Update()
	{
		this.Evaluate(false);
	}

	// Token: 0x0600298F RID: 10639 RVA: 0x000B4EF8 File Offset: 0x000B30F8
	private void Evaluate(bool force)
	{
		Vector2 relativePosition = this.GetRelativePosition();
		bool flag = base.transform.position.y > relativePosition.y;
		if (flag != this.wasAbove || force)
		{
			this.IsNowAbove.Invoke(flag);
			this.IsNowNotAbove.Invoke(!flag);
			if (flag)
			{
				this.IsJustAbove.Invoke();
			}
			else
			{
				this.IsJustNotAbove.Invoke();
			}
			this.wasAbove = flag;
		}
	}

	// Token: 0x06002990 RID: 10640 RVA: 0x000B4F74 File Offset: 0x000B3174
	private Vector2 GetRelativePosition()
	{
		Vector2 vector = new Vector2(0f, this.positionY);
		if (!this.relativeTo)
		{
			return vector;
		}
		return this.relativeTo.TransformPoint(vector);
	}

	// Token: 0x04002A1E RID: 10782
	[SerializeField]
	private Transform relativeTo;

	// Token: 0x04002A1F RID: 10783
	[SerializeField]
	private float positionY;

	// Token: 0x04002A20 RID: 10784
	[Space]
	public PositionActivator.UnityBoolEvent IsNowAbove;

	// Token: 0x04002A21 RID: 10785
	public UnityEvent IsJustAbove;

	// Token: 0x04002A22 RID: 10786
	public PositionActivator.UnityBoolEvent IsNowNotAbove;

	// Token: 0x04002A23 RID: 10787
	public UnityEvent IsJustNotAbove;

	// Token: 0x04002A24 RID: 10788
	private bool wasAbove;

	// Token: 0x02001789 RID: 6025
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}
}
