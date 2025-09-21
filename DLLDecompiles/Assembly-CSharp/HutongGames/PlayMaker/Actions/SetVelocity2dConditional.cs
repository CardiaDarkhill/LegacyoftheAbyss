using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FE0 RID: 4064
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
	public sealed class SetVelocity2dConditional : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006FE5 RID: 28645 RVA: 0x0022952C File Offset: 0x0022772C
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.xCondition = new SetVelocity2dConditional.Condition
			{
				comparisonType = SetVelocity2dConditional.ComparisonType.None
			};
			this.yCondition = new SetVelocity2dConditional.Condition
			{
				comparisonType = SetVelocity2dConditional.ComparisonType.None
			};
			this.bothConditionsMustBeTrue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006FE6 RID: 28646 RVA: 0x0022959D File Offset: 0x0022779D
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006FE7 RID: 28647 RVA: 0x002295AB File Offset: 0x002277AB
		public override void OnEnter()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FE8 RID: 28648 RVA: 0x002295C1 File Offset: 0x002277C1
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FE9 RID: 28649 RVA: 0x002295D8 File Offset: 0x002277D8
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
		}

		// Token: 0x06006FEA RID: 28650 RVA: 0x002296F4 File Offset: 0x002278F4
		private bool CheckCondition(float currentValue, SetVelocity2dConditional.Condition condition)
		{
			switch (condition.comparisonType)
			{
			case SetVelocity2dConditional.ComparisonType.GreaterThan:
				return currentValue > condition.value.Value;
			case SetVelocity2dConditional.ComparisonType.LessThan:
				return currentValue < condition.value.Value;
			case SetVelocity2dConditional.ComparisonType.EqualTo:
				return Mathf.Approximately(currentValue, condition.value.Value);
			case SetVelocity2dConditional.ComparisonType.GreaterThanOrEqualTo:
				return currentValue >= condition.value.Value;
			case SetVelocity2dConditional.ComparisonType.LessThanOrEqualTo:
				return currentValue <= condition.value.Value;
			}
			return true;
		}

		// Token: 0x04006FDB RID: 28635
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006FDC RID: 28636
		[Tooltip("A Vector2 value for the velocity")]
		public FsmVector2 vector;

		// Token: 0x04006FDD RID: 28637
		[Tooltip("The x value of the velocity. Overrides 'Vector' x value if set")]
		public FsmFloat x;

		// Token: 0x04006FDE RID: 28638
		[Tooltip("The y value of the velocity. Overrides 'Vector' y value if set")]
		public FsmFloat y;

		// Token: 0x04006FDF RID: 28639
		[Tooltip("Conditions for the x value")]
		public SetVelocity2dConditional.Condition xCondition;

		// Token: 0x04006FE0 RID: 28640
		[Tooltip("Conditions for the y value")]
		public SetVelocity2dConditional.Condition yCondition;

		// Token: 0x04006FE1 RID: 28641
		[Tooltip("Both conditions must be true")]
		public FsmBool bothConditionsMustBeTrue;

		// Token: 0x04006FE2 RID: 28642
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x02001BB4 RID: 7092
		public class Condition
		{
			// Token: 0x04009E4E RID: 40526
			public FsmFloat value;

			// Token: 0x04009E4F RID: 40527
			public SetVelocity2dConditional.ComparisonType comparisonType;
		}

		// Token: 0x02001BB5 RID: 7093
		public enum ComparisonType
		{
			// Token: 0x04009E51 RID: 40529
			None,
			// Token: 0x04009E52 RID: 40530
			GreaterThan,
			// Token: 0x04009E53 RID: 40531
			LessThan,
			// Token: 0x04009E54 RID: 40532
			EqualTo,
			// Token: 0x04009E55 RID: 40533
			GreaterThanOrEqualTo,
			// Token: 0x04009E56 RID: 40534
			LessThanOrEqualTo
		}
	}
}
