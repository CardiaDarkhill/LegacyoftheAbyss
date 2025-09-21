using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002A5 RID: 677
public class ConstrainPosition : MonoBehaviour
{
	// Token: 0x06001807 RID: 6151 RVA: 0x0006DE24 File Offset: 0x0006C024
	private void Awake()
	{
		if (this.cutVelocityOnConstrain)
		{
			this.rb = base.GetComponent<Rigidbody2D>();
		}
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x0006DE3C File Offset: 0x0006C03C
	private void LateUpdate()
	{
		Vector3 vector;
		if (this.localSpace)
		{
			vector = base.transform.localPosition;
		}
		else
		{
			vector = base.transform.position;
		}
		bool flag = false;
		if (this.constrainX)
		{
			if (vector.x < this.xMin)
			{
				vector.x = this.xMin;
				flag = true;
			}
			else if (vector.x > this.xMax)
			{
				vector.x = this.xMax;
				flag = true;
			}
		}
		if (this.constrainY)
		{
			if (vector.y < this.yMin)
			{
				vector.y = this.yMin;
				flag = true;
			}
			else if (vector.y > this.yMax)
			{
				vector.y = this.yMax;
				flag = true;
			}
		}
		if (flag)
		{
			if (Time.timeScale <= Mathf.Epsilon)
			{
				return;
			}
			if (this.localSpace)
			{
				base.transform.localPosition = vector;
			}
			else
			{
				base.transform.position = vector;
			}
			if (this.cutVelocityOnConstrain)
			{
				this.rb.linearVelocity = new Vector2(0f, 0f);
			}
			if (this.OnConstrained != null)
			{
				this.OnConstrained.Invoke();
			}
		}
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x0006DF5C File Offset: 0x0006C15C
	public void SetXMin(float newValue)
	{
		this.xMin = newValue;
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x0006DF65 File Offset: 0x0006C165
	public void SetXMax(float newValue)
	{
		this.xMax = newValue;
	}

	// Token: 0x0600180B RID: 6155 RVA: 0x0006DF6E File Offset: 0x0006C16E
	public void SetYMin(float newValue)
	{
		this.yMin = newValue;
	}

	// Token: 0x0600180C RID: 6156 RVA: 0x0006DF77 File Offset: 0x0006C177
	public void SetYMax(float newValue)
	{
		this.yMax = newValue;
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x0006DF80 File Offset: 0x0006C180
	public void StartConstrainX()
	{
		this.constrainX = true;
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x0006DF89 File Offset: 0x0006C189
	public void EndConstrainX()
	{
		this.constrainX = false;
	}

	// Token: 0x0600180F RID: 6159 RVA: 0x0006DF92 File Offset: 0x0006C192
	public void StartConstrainY()
	{
		this.constrainY = true;
	}

	// Token: 0x06001810 RID: 6160 RVA: 0x0006DF9B File Offset: 0x0006C19B
	public void EndConstrainY()
	{
		this.constrainY = false;
	}

	// Token: 0x06001811 RID: 6161 RVA: 0x0006DFA4 File Offset: 0x0006C1A4
	public void EndConstrain()
	{
		this.constrainX = false;
		this.constrainY = false;
	}

	// Token: 0x040016DD RID: 5853
	public bool constrainX;

	// Token: 0x040016DE RID: 5854
	public float xMin;

	// Token: 0x040016DF RID: 5855
	public float xMax;

	// Token: 0x040016E0 RID: 5856
	public bool constrainY;

	// Token: 0x040016E1 RID: 5857
	public float yMin;

	// Token: 0x040016E2 RID: 5858
	public float yMax;

	// Token: 0x040016E3 RID: 5859
	public bool cutVelocityOnConstrain;

	// Token: 0x040016E4 RID: 5860
	public bool localSpace;

	// Token: 0x040016E5 RID: 5861
	private Rigidbody2D rb;

	// Token: 0x040016E6 RID: 5862
	[Space]
	public UnityEvent OnConstrained;
}
