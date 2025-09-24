using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BEF RID: 3055
	[ActionCategory("Enemy AI")]
	[Tooltip("Check whether target is left/right/up/down relative to object")]
	public class CheckTargetDirection : FsmStateAction
	{
		// Token: 0x06005D87 RID: 23943 RVA: 0x001D7C8E File Offset: 0x001D5E8E
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.everyFrame = false;
		}

		// Token: 0x06005D88 RID: 23944 RVA: 0x001D7CA8 File Offset: 0x001D5EA8
		public override void OnEnter()
		{
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			bool flag = false;
			if (this.target.Value == null || this.target == null)
			{
				base.Finish();
				flag = true;
			}
			this.DoCheckDirection();
			if (!flag && !this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005D89 RID: 23945 RVA: 0x001D7D0D File Offset: 0x001D5F0D
		public override void OnUpdate()
		{
			this.DoCheckDirection();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005D8A RID: 23946 RVA: 0x001D7D24 File Offset: 0x001D5F24
		private void DoCheckDirection()
		{
			if (this.target.Value == null || this.target == null)
			{
				return;
			}
			float num = this.self.Value.transform.position.x + this.selfOffsetX.Value;
			float num2 = this.self.Value.transform.position.y + this.selfOffsetY.Value;
			float x = this.target.Value.transform.position.x;
			float y = this.target.Value.transform.position.y;
			if (this.reverseIfNegativeScale && this.self.Value.transform.localScale.x < 0f)
			{
				if (num < x)
				{
					base.Fsm.Event(this.leftEvent);
					this.leftBool.Value = true;
				}
				else
				{
					this.leftBool.Value = false;
				}
				if (num >= x)
				{
					base.Fsm.Event(this.rightEvent);
					this.rightBool.Value = true;
				}
				else
				{
					this.rightBool.Value = false;
				}
			}
			else
			{
				if (num <= x)
				{
					base.Fsm.Event(this.rightEvent);
					this.rightBool.Value = true;
				}
				else
				{
					this.rightBool.Value = false;
				}
				if (num > x)
				{
					base.Fsm.Event(this.leftEvent);
					this.leftBool.Value = true;
				}
				else
				{
					this.leftBool.Value = false;
				}
			}
			if (num2 <= y)
			{
				base.Fsm.Event(this.aboveEvent);
				this.aboveBool.Value = true;
			}
			else
			{
				this.aboveBool.Value = false;
			}
			if (num2 > y)
			{
				base.Fsm.Event(this.belowEvent);
				this.belowBool.Value = true;
				return;
			}
			this.belowBool.Value = false;
		}

		// Token: 0x040059B5 RID: 22965
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040059B6 RID: 22966
		[RequiredField]
		public FsmGameObject target;

		// Token: 0x040059B7 RID: 22967
		public FsmEvent aboveEvent;

		// Token: 0x040059B8 RID: 22968
		public FsmEvent belowEvent;

		// Token: 0x040059B9 RID: 22969
		public FsmEvent rightEvent;

		// Token: 0x040059BA RID: 22970
		public FsmEvent leftEvent;

		// Token: 0x040059BB RID: 22971
		[UIHint(UIHint.Variable)]
		public FsmBool aboveBool;

		// Token: 0x040059BC RID: 22972
		[UIHint(UIHint.Variable)]
		public FsmBool belowBool;

		// Token: 0x040059BD RID: 22973
		[UIHint(UIHint.Variable)]
		public FsmBool rightBool;

		// Token: 0x040059BE RID: 22974
		[UIHint(UIHint.Variable)]
		public FsmBool leftBool;

		// Token: 0x040059BF RID: 22975
		public FsmFloat selfOffsetX;

		// Token: 0x040059C0 RID: 22976
		public FsmFloat selfOffsetY;

		// Token: 0x040059C1 RID: 22977
		public bool reverseIfNegativeScale;

		// Token: 0x040059C2 RID: 22978
		public bool everyFrame;

		// Token: 0x040059C3 RID: 22979
		private FsmGameObject self;
	}
}
