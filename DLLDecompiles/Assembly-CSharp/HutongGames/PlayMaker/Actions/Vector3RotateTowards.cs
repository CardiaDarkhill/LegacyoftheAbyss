using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011AB RID: 4523
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Rotates a Vector3 direction from Current towards Target.")]
	public class Vector3RotateTowards : FsmStateAction
	{
		// Token: 0x060078E4 RID: 30948 RVA: 0x00249204 File Offset: 0x00247404
		public override void Reset()
		{
			this.currentDirection = new FsmVector3
			{
				UseVariable = true
			};
			this.targetDirection = new FsmVector3
			{
				UseVariable = true
			};
			this.rotateSpeed = 360f;
			this.maxMagnitude = 1f;
		}

		// Token: 0x060078E5 RID: 30949 RVA: 0x00249258 File Offset: 0x00247458
		public override void OnUpdate()
		{
			this.currentDirection.Value = Vector3.RotateTowards(this.currentDirection.Value, this.targetDirection.Value, this.rotateSpeed.Value * 0.017453292f * Time.deltaTime, this.maxMagnitude.Value);
		}

		// Token: 0x04007943 RID: 31043
		[RequiredField]
		[Tooltip("The current direction vector.")]
		public FsmVector3 currentDirection;

		// Token: 0x04007944 RID: 31044
		[RequiredField]
		[Tooltip("The target direction vector.")]
		public FsmVector3 targetDirection;

		// Token: 0x04007945 RID: 31045
		[RequiredField]
		[Tooltip("Rotation speed in degrees per second.")]
		public FsmFloat rotateSpeed;

		// Token: 0x04007946 RID: 31046
		[RequiredField]
		[Tooltip("Max Magnitude per second")]
		public FsmFloat maxMagnitude;
	}
}
