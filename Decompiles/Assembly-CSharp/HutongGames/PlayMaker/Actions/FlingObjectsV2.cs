using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C45 RID: 3141
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the velocity of all children of chosen GameObject")]
	public class FlingObjectsV2 : RigidBody2dActionBase
	{
		// Token: 0x06005F52 RID: 24402 RVA: 0x001E48C2 File Offset: 0x001E2AC2
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

		// Token: 0x06005F53 RID: 24403 RVA: 0x001E48F8 File Offset: 0x001E2AF8
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
						if (this.unsetKinematic & this.rb2d.isKinematic)
						{
							this.rb2d.isKinematic = false;
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

		// Token: 0x04005CAA RID: 23722
		[RequiredField]
		[Tooltip("Object containing the objects to be flung.")]
		public FsmGameObject containerObject;

		// Token: 0x04005CAB RID: 23723
		public FsmVector3 adjustPosition;

		// Token: 0x04005CAC RID: 23724
		public FsmBool randomisePosition;

		// Token: 0x04005CAD RID: 23725
		[Tooltip("Minimum speed clones are fired at.")]
		public FsmFloat speedMin;

		// Token: 0x04005CAE RID: 23726
		[Tooltip("Maximum speed clones are fired at.")]
		public FsmFloat speedMax;

		// Token: 0x04005CAF RID: 23727
		[Tooltip("Minimum angle clones are fired at.")]
		public FsmFloat angleMin;

		// Token: 0x04005CB0 RID: 23728
		[Tooltip("Maximum angle clones are fired at.")]
		public FsmFloat angleMax;

		// Token: 0x04005CB1 RID: 23729
		public Space space;

		// Token: 0x04005CB2 RID: 23730
		public bool unsetKinematic;

		// Token: 0x04005CB3 RID: 23731
		private float vectorX;

		// Token: 0x04005CB4 RID: 23732
		private float vectorY;

		// Token: 0x04005CB5 RID: 23733
		private bool originAdjusted;
	}
}
