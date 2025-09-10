using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
[CreateAssetMenu(menuName = "Profiles/Ambient Float")]
public class AmbientFloatProfile : ScriptableObject
{
	// Token: 0x06000255 RID: 597 RVA: 0x0000E288 File Offset: 0x0000C488
	public Vector3 GetOffset(float time, float timeOffset)
	{
		Vector3 vector = Vector3.zero;
		time += timeOffset * this.randomOffset;
		foreach (AmbientFloatProfile.SineCurve sineCurve in this.sineCurves)
		{
			vector += sineCurve.MoveAmount * Mathf.Sin(time * sineCurve.TimeScale * this.moveSpeed + sineCurve.Offset) * sineCurve.Magnitude;
		}
		if (this.sineCurves.Length != 0)
		{
			vector /= (float)this.sineCurves.Length;
		}
		return vector.MultiplyElements(this.totalMoveAmount);
	}

	// Token: 0x040001FF RID: 511
	[SerializeField]
	private Vector3 totalMoveAmount = Vector3.one;

	// Token: 0x04000200 RID: 512
	[SerializeField]
	private float moveSpeed;

	// Token: 0x04000201 RID: 513
	[SerializeField]
	[Range(-1f, 1f)]
	private float randomOffset;

	// Token: 0x04000202 RID: 514
	[Space]
	[SerializeField]
	private AmbientFloatProfile.SineCurve[] sineCurves;

	// Token: 0x020013D7 RID: 5079
	[Serializable]
	private struct SineCurve
	{
		// Token: 0x040080DE RID: 32990
		public Vector3 MoveAmount;

		// Token: 0x040080DF RID: 32991
		public float Magnitude;

		// Token: 0x040080E0 RID: 32992
		public float Offset;

		// Token: 0x040080E1 RID: 32993
		public float TimeScale;
	}
}
