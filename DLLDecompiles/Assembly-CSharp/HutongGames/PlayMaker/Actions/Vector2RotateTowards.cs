using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001194 RID: 4500
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Rotates a Vector2 direction from Current towards Target.")]
	public class Vector2RotateTowards : FsmStateAction
	{
		// Token: 0x0600787D RID: 30845 RVA: 0x00247F18 File Offset: 0x00246118
		public override void Reset()
		{
			this.currentDirection = new FsmVector2
			{
				UseVariable = true
			};
			this.targetDirection = new FsmVector2
			{
				UseVariable = true
			};
			this.rotateSpeed = 360f;
		}

		// Token: 0x0600787E RID: 30846 RVA: 0x00247F50 File Offset: 0x00246150
		public override void OnEnter()
		{
			this.current = new Vector3(this.currentDirection.Value.x, this.currentDirection.Value.y, 0f);
			this.target = new Vector3(this.targetDirection.Value.x, this.targetDirection.Value.y, 0f);
		}

		// Token: 0x0600787F RID: 30847 RVA: 0x00247FC0 File Offset: 0x002461C0
		public override void OnUpdate()
		{
			this.current.x = this.currentDirection.Value.x;
			this.current.y = this.currentDirection.Value.y;
			this.current = Vector3.RotateTowards(this.current, this.target, this.rotateSpeed.Value * 0.017453292f * Time.deltaTime, 1000f);
			this.currentDirection.Value = new Vector2(this.current.x, this.current.y);
		}

		// Token: 0x040078EB RID: 30955
		[RequiredField]
		[Tooltip("The current direction. This will be the result of the rotation as well.")]
		public FsmVector2 currentDirection;

		// Token: 0x040078EC RID: 30956
		[RequiredField]
		[Tooltip("The direction to reach")]
		public FsmVector2 targetDirection;

		// Token: 0x040078ED RID: 30957
		[RequiredField]
		[Tooltip("Rotation speed in degrees per second")]
		public FsmFloat rotateSpeed;

		// Token: 0x040078EE RID: 30958
		private Vector3 current;

		// Token: 0x040078EF RID: 30959
		private Vector3 target;
	}
}
