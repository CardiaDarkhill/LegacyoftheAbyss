using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3E RID: 3134
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the velocity of all children of chosen GameObject")]
	public class FlingObjects : RigidBody2dActionBase
	{
		// Token: 0x06005F35 RID: 24373 RVA: 0x001E3259 File Offset: 0x001E1459
		public override void Reset()
		{
			this.containerObject = null;
			this.adjustPosition = null;
			this.speedMin = null;
			this.speedMax = null;
			this.angleMin = null;
			this.angleMax = null;
			this.space = Space.World;
		}

		// Token: 0x06005F36 RID: 24374 RVA: 0x001E328C File Offset: 0x001E148C
		public override void OnEnter()
		{
			GameObject value = this.containerObject.Value;
			if (value != null)
			{
				int childCount = value.transform.childCount;
				for (int i = 1; i <= childCount; i++)
				{
					GameObject gameObject = value.transform.GetChild(i - 1).gameObject;
					base.CacheRigidBody2d(gameObject);
					if (this.rb2d != null)
					{
						float num = Random.Range(this.speedMin.Value, this.speedMax.Value);
						float num2 = Random.Range(this.angleMin.Value, this.angleMax.Value);
						this.vectorX = num * Mathf.Cos(num2 * 0.017453292f);
						this.vectorY = num * Mathf.Sin(num2 * 0.017453292f);
						Vector2 vector;
						vector.x = this.vectorX;
						vector.y = this.vectorY;
						if (this.space == Space.Self)
						{
							vector = value.transform.TransformVector(vector);
						}
						this.rb2d.linearVelocity = vector;
						if (!this.adjustPosition.IsNone)
						{
							if (this.randomisePosition.Value)
							{
								gameObject.transform.position = new Vector3(gameObject.transform.position.x + Random.Range(-this.adjustPosition.Value.x, this.adjustPosition.Value.x), gameObject.transform.position.y + Random.Range(-this.adjustPosition.Value.y, this.adjustPosition.Value.y), gameObject.transform.position.z);
							}
							else
							{
								gameObject.transform.position += this.adjustPosition.Value;
							}
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04005C39 RID: 23609
		[RequiredField]
		[Tooltip("Object containing the objects to be flung.")]
		public FsmGameObject containerObject;

		// Token: 0x04005C3A RID: 23610
		public FsmVector3 adjustPosition;

		// Token: 0x04005C3B RID: 23611
		public FsmBool randomisePosition;

		// Token: 0x04005C3C RID: 23612
		[Tooltip("Minimum speed clones are fired at.")]
		public FsmFloat speedMin;

		// Token: 0x04005C3D RID: 23613
		[Tooltip("Maximum speed clones are fired at.")]
		public FsmFloat speedMax;

		// Token: 0x04005C3E RID: 23614
		[Tooltip("Minimum angle clones are fired at.")]
		public FsmFloat angleMin;

		// Token: 0x04005C3F RID: 23615
		[Tooltip("Maximum angle clones are fired at.")]
		public FsmFloat angleMax;

		// Token: 0x04005C40 RID: 23616
		public Space space;

		// Token: 0x04005C41 RID: 23617
		private float vectorX;

		// Token: 0x04005C42 RID: 23618
		private float vectorY;

		// Token: 0x04005C43 RID: 23619
		private bool originAdjusted;
	}
}
