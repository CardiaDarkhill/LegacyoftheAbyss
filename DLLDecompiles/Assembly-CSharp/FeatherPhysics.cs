using System;
using UnityEngine;

// Token: 0x020004DE RID: 1246
public class FeatherPhysics : MonoBehaviour
{
	// Token: 0x06002CC4 RID: 11460 RVA: 0x000C3BA5 File Offset: 0x000C1DA5
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.groundRayOrigin), 0.2f);
	}

	// Token: 0x06002CC5 RID: 11461 RVA: 0x000C3BC8 File Offset: 0x000C1DC8
	private void Awake()
	{
		SurfaceWaterFloater component = base.GetComponent<SurfaceWaterFloater>();
		if (component)
		{
			component.OnLandInWater.AddListener(delegate()
			{
				base.enabled = false;
			});
			component.OnExitWater.AddListener(delegate()
			{
				base.enabled = true;
			});
		}
		if (this.body)
		{
			this.recordedGravity = true;
			this.gravityScale = this.body.gravityScale;
		}
	}

	// Token: 0x06002CC6 RID: 11462 RVA: 0x000C3C38 File Offset: 0x000C1E38
	private void OnEnable()
	{
		if (!this.body)
		{
			base.enabled = false;
		}
		else if (this.recordedGravity)
		{
			this.body.gravityScale = this.gravityScale;
		}
		this.elapsedTime = Random.Range(0f, this.curveTime);
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x000C3C8A File Offset: 0x000C1E8A
	private void OnDisable()
	{
		if (this.body)
		{
			this.body.linearVelocity = Vector2.zero;
		}
	}

	// Token: 0x06002CC8 RID: 11464 RVA: 0x000C3CAC File Offset: 0x000C1EAC
	private void FixedUpdate()
	{
		if (this.body.linearVelocity.y > 0f)
		{
			return;
		}
		if (this.body.gravityScale > 0f)
		{
			this.body.gravityScale = 0f;
			this.transitionTimeLeft = this.transitionTime;
		}
		float num = 1f;
		Vector2 origin = base.transform.TransformPoint(this.groundRayOrigin) + new Vector2(0f, 0.1f);
		Vector2 down = Vector2.down;
		float distance = this.groundRayLength + 0.1f;
		RaycastHit2D raycastHit2D = Helper.Raycast2D(origin, down, distance, 256);
		if (raycastHit2D.collider != null)
		{
			num = Mathf.Clamp01((raycastHit2D.distance - 1f - Physics2D.defaultContactOffset * 2f) / this.groundRayLength);
		}
		float time = this.elapsedTime / this.curveTime;
		float x = this.velocityCurveX.Evaluate(time) * this.velocityMagnitudeX;
		float y;
		if (this.doNotAnimateY)
		{
			y = this.body.linearVelocity.y;
		}
		else
		{
			y = this.velocityCurveY.Evaluate(time) * this.velocityMagnitudeY;
		}
		float num2;
		if (this.transitionTimeLeft > 0f)
		{
			num2 = (this.transitionTime - this.transitionTimeLeft) / this.transitionTime;
			this.transitionTimeLeft -= Time.deltaTime;
		}
		else
		{
			num2 = 1f;
		}
		this.body.linearVelocity = new Vector2(x, y) * (num * num2 * Time.timeScale);
		this.elapsedTime += Time.deltaTime;
		if (this.elapsedTime >= this.curveTime)
		{
			this.elapsedTime %= this.curveTime;
		}
	}

	// Token: 0x04002E5E RID: 11870
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x04002E5F RID: 11871
	[SerializeField]
	private float transitionTime;

	// Token: 0x04002E60 RID: 11872
	[Space]
	[SerializeField]
	private float curveTime = 1f;

	// Token: 0x04002E61 RID: 11873
	[SerializeField]
	private AnimationCurve velocityCurveX;

	// Token: 0x04002E62 RID: 11874
	[SerializeField]
	private float velocityMagnitudeX = 1f;

	// Token: 0x04002E63 RID: 11875
	[SerializeField]
	private AnimationCurve velocityCurveY;

	// Token: 0x04002E64 RID: 11876
	[SerializeField]
	private float velocityMagnitudeY = 1f;

	// Token: 0x04002E65 RID: 11877
	[SerializeField]
	private bool doNotAnimateY;

	// Token: 0x04002E66 RID: 11878
	[Space]
	[SerializeField]
	private Vector2 groundRayOrigin;

	// Token: 0x04002E67 RID: 11879
	[SerializeField]
	private float groundRayLength = 0.5f;

	// Token: 0x04002E68 RID: 11880
	private bool recordedGravity;

	// Token: 0x04002E69 RID: 11881
	private float elapsedTime;

	// Token: 0x04002E6A RID: 11882
	private float transitionTimeLeft;

	// Token: 0x04002E6B RID: 11883
	private float gravityScale;
}
