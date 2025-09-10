using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FE1 RID: 4065
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
	public sealed class TranslateVelocity2dConditional : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006FEC RID: 28652 RVA: 0x00229788 File Offset: 0x00227988
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = new FsmVector2
			{
				UseVariable = true
			};
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.xCondition = new TranslateVelocity2dConditional.Condition
			{
				comparisonType = TranslateVelocity2dConditional.ComparisonType.None
			};
			this.yCondition = new TranslateVelocity2dConditional.Condition
			{
				comparisonType = TranslateVelocity2dConditional.ComparisonType.None
			};
			this.bothConditionsMustBeTrue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006FED RID: 28653 RVA: 0x00229804 File Offset: 0x00227A04
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006FEE RID: 28654 RVA: 0x00229812 File Offset: 0x00227A12
		public override void OnEnter()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FEF RID: 28655 RVA: 0x00229828 File Offset: 0x00227A28
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FF0 RID: 28656 RVA: 0x00229840 File Offset: 0x00227A40
		private void DoSetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 vector;
			if (this.vector.IsNone)
			{
				vector = base.rigidbody2d.linearVelocity;
			}
			else
			{
				vector = this.vector.Value;
			}
			bool flag = this.CheckCondition(vector.x, this.xCondition);
			bool flag2 = this.CheckCondition(vector.y, this.yCondition);
			if (this.bothConditionsMustBeTrue.Value)
			{
				if (flag && flag2)
				{
					if (!this.x.IsNone)
					{
						vector.x = this.x.Value;
					}
					if (!this.y.IsNone)
					{
						vector.y = this.y.Value;
					}
				}
			}
			else
			{
				if (flag && !this.x.IsNone)
				{
					vector.x = this.x.Value;
				}
				if (flag2 && !this.y.IsNone)
				{
					vector.y = this.y.Value;
				}
			}
			base.rigidbody2d.linearVelocity = vector;
			if (!flag)
			{
				vector.x = 0f;
			}
			if (!flag2)
			{
				vector.y = 0f;
			}
			base.rigidbody2d.transform.Translate(vector * Time.fixedDeltaTime);
		}

		// Token: 0x06006FF1 RID: 28657 RVA: 0x0022999C File Offset: 0x00227B9C
		private bool CheckCondition(float currentValue, TranslateVelocity2dConditional.Condition condition)
		{
			switch (condition.comparisonType)
			{
			case TranslateVelocity2dConditional.ComparisonType.GreaterThan:
				return currentValue > condition.value.Value;
			case TranslateVelocity2dConditional.ComparisonType.LessThan:
				return currentValue < condition.value.Value;
			case TranslateVelocity2dConditional.ComparisonType.EqualTo:
				return Mathf.Approximately(currentValue, condition.value.Value);
			case TranslateVelocity2dConditional.ComparisonType.GreaterThanOrEqualTo:
				return currentValue >= condition.value.Value;
			case TranslateVelocity2dConditional.ComparisonType.LessThanOrEqualTo:
				return currentValue <= condition.value.Value;
			}
			return true;
		}

		// Token: 0x04006FE3 RID: 28643
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006FE4 RID: 28644
		[Tooltip("A Vector2 value for the velocity")]
		public FsmVector2 vector;

		// Token: 0x04006FE5 RID: 28645
		[Tooltip("The x value of the velocity. Overrides 'Vector' x value if set")]
		public FsmFloat x;

		// Token: 0x04006FE6 RID: 28646
		[Tooltip("The y value of the velocity. Overrides 'Vector' y value if set")]
		public FsmFloat y;

		// Token: 0x04006FE7 RID: 28647
		[Tooltip("Conditions for the x value")]
		public TranslateVelocity2dConditional.Condition xCondition;

		// Token: 0x04006FE8 RID: 28648
		[Tooltip("Conditions for the y value")]
		public TranslateVelocity2dConditional.Condition yCondition;

		// Token: 0x04006FE9 RID: 28649
		[Tooltip("Both conditions must be true")]
		public FsmBool bothConditionsMustBeTrue;

		// Token: 0x04006FEA RID: 28650
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x02001BB6 RID: 7094
		public class Condition
		{
			// Token: 0x04009E57 RID: 40535
			public FsmFloat value;

			// Token: 0x04009E58 RID: 40536
			public TranslateVelocity2dConditional.ComparisonType comparisonType;
		}

		// Token: 0x02001BB7 RID: 7095
		public enum ComparisonType
		{
			// Token: 0x04009E5A RID: 40538
			None,
			// Token: 0x04009E5B RID: 40539
			GreaterThan,
			// Token: 0x04009E5C RID: 40540
			LessThan,
			// Token: 0x04009E5D RID: 40541
			EqualTo,
			// Token: 0x04009E5E RID: 40542
			GreaterThanOrEqualTo,
			// Token: 0x04009E5F RID: 40543
			LessThanOrEqualTo
		}
	}
}
