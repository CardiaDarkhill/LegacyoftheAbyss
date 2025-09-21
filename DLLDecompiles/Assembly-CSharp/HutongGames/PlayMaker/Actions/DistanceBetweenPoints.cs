using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C19 RID: 3097
	[ActionCategory("Math")]
	[Tooltip("Calculate the distance between two points and store it as a float.")]
	public class DistanceBetweenPoints : FsmStateAction
	{
		// Token: 0x06005E55 RID: 24149 RVA: 0x001DBA64 File Offset: 0x001D9C64
		public override void Reset()
		{
			this.distanceResult = null;
			this.point1 = null;
			this.point2 = null;
			this.ignoreX = false;
			this.ignoreY = false;
			this.ignoreZ = false;
			this.everyFrame = false;
		}

		// Token: 0x06005E56 RID: 24150 RVA: 0x001DBA97 File Offset: 0x001D9C97
		public override void OnEnter()
		{
			this.DoCalcDistance();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005E57 RID: 24151 RVA: 0x001DBAAD File Offset: 0x001D9CAD
		public override void OnUpdate()
		{
			this.DoCalcDistance();
		}

		// Token: 0x06005E58 RID: 24152 RVA: 0x001DBAB8 File Offset: 0x001D9CB8
		private void DoCalcDistance()
		{
			if (this.distanceResult == null)
			{
				return;
			}
			if (this.ignoreX && this.ignoreY)
			{
				bool flag = this.ignoreZ;
			}
			if (this.ignoreX)
			{
				if (this.ignoreY)
				{
					this.distanceResult.Value = Mathf.Abs(this.point1.Value.z - this.point2.Value.z);
					return;
				}
				if (this.ignoreZ)
				{
					this.distanceResult.Value = Mathf.Abs(this.point1.Value.y - this.point2.Value.y);
					return;
				}
				Vector2 a = new Vector2(this.point1.Value.y, this.point1.Value.z);
				Vector2 b = new Vector2(this.point2.Value.y, this.point2.Value.z);
				this.distanceResult.Value = Vector2.Distance(a, b);
				return;
			}
			else if (this.ignoreY)
			{
				if (this.ignoreX)
				{
					this.distanceResult.Value = Mathf.Abs(this.point1.Value.z - this.point2.Value.z);
					return;
				}
				if (this.ignoreZ)
				{
					this.distanceResult.Value = Mathf.Abs(this.point1.Value.x - this.point2.Value.x);
					return;
				}
				Vector2 a2 = new Vector2(this.point1.Value.x, this.point1.Value.z);
				Vector2 b2 = new Vector2(this.point2.Value.x, this.point2.Value.z);
				this.distanceResult.Value = Vector2.Distance(a2, b2);
				return;
			}
			else
			{
				if (!this.ignoreZ)
				{
					Vector3 a3 = new Vector3(this.point1.Value.x, this.point1.Value.y, this.point1.Value.z);
					Vector2 v = new Vector3(this.point2.Value.x, this.point2.Value.y, this.point2.Value.z);
					this.distanceResult.Value = Vector3.Distance(a3, v);
					return;
				}
				if (this.ignoreX)
				{
					this.distanceResult.Value = Mathf.Abs(this.point1.Value.y - this.point2.Value.y);
					return;
				}
				if (this.ignoreY)
				{
					this.distanceResult.Value = Mathf.Abs(this.point1.Value.x - this.point2.Value.x);
					return;
				}
				Vector2 a4 = new Vector2(this.point1.Value.x, this.point1.Value.y);
				Vector2 b3 = new Vector2(this.point2.Value.x, this.point2.Value.y);
				this.distanceResult.Value = Vector2.Distance(a4, b3);
				return;
			}
		}

		// Token: 0x04005AA2 RID: 23202
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat distanceResult;

		// Token: 0x04005AA3 RID: 23203
		[RequiredField]
		public FsmVector3 point1;

		// Token: 0x04005AA4 RID: 23204
		[RequiredField]
		public FsmVector3 point2;

		// Token: 0x04005AA5 RID: 23205
		public bool ignoreX;

		// Token: 0x04005AA6 RID: 23206
		public bool ignoreY;

		// Token: 0x04005AA7 RID: 23207
		public bool ignoreZ;

		// Token: 0x04005AA8 RID: 23208
		public bool everyFrame;
	}
}
