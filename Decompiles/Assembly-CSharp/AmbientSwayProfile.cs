using System;
using UnityEngine;

// Token: 0x0200005D RID: 93
[CreateAssetMenu(menuName = "Profiles/Ambient Float")]
public class AmbientSwayProfile : ScriptableObject
{
	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000262 RID: 610 RVA: 0x0000E53A File Offset: 0x0000C73A
	public float Fps
	{
		get
		{
			return this.fps;
		}
	}

	// Token: 0x06000263 RID: 611 RVA: 0x0000E544 File Offset: 0x0000C744
	public Vector3 GetOffset(float time, float timeOffset)
	{
		Vector3 vector = Vector3.zero;
		time += timeOffset * this.randomOffset;
		foreach (AmbientSwayProfile.SineCurve sineCurve in this.sineCurves)
		{
			vector += sineCurve.MoveAmount * (Mathf.Sin(time * sineCurve.TimeScale * this.moveSpeed + sineCurve.Offset) * sineCurve.Magnitude);
		}
		if (this.sineCurves.Length != 0)
		{
			vector /= (float)this.sineCurves.Length;
		}
		return vector.MultiplyElements(this.totalMoveAmount);
	}

	// Token: 0x0400020B RID: 523
	[SerializeField]
	private Vector3 totalMoveAmount = Vector3.one;

	// Token: 0x0400020C RID: 524
	[SerializeField]
	private float moveSpeed;

	// Token: 0x0400020D RID: 525
	[SerializeField]
	[Range(-1f, 1f)]
	private float randomOffset;

	// Token: 0x0400020E RID: 526
	[SerializeField]
	private float fps;

	// Token: 0x0400020F RID: 527
	[Space]
	[SerializeField]
	private AmbientSwayProfile.SineCurve[] sineCurves;

	// Token: 0x020013D8 RID: 5080
	[Serializable]
	private struct SineCurve
	{
		// Token: 0x040080E2 RID: 32994
		public Vector3 MoveAmount;

		// Token: 0x040080E3 RID: 32995
		public float Magnitude;

		// Token: 0x040080E4 RID: 32996
		public float Offset;

		// Token: 0x040080E5 RID: 32997
		public float TimeScale;
	}
}
