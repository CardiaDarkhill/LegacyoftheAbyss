using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200123E RID: 4670
	[ActionCategory("Hollow Knight")]
	public class HeroCheckForBumpV2 : FsmStateAction
	{
		// Token: 0x06007B92 RID: 31634 RVA: 0x0024FE7F File Offset: 0x0024E07F
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007B93 RID: 31635 RVA: 0x0024FE8D File Offset: 0x0024E08D
		public override void Awake()
		{
			this.OnPreprocess();
		}

		// Token: 0x06007B94 RID: 31636 RVA: 0x0024FE98 File Offset: 0x0024E098
		public override void Reset()
		{
			this.Direction = null;
			this.StoreHitBump = null;
			this.StoreHitWall = null;
			this.StoreHitHighWall = null;
			this.BumpEvent = null;
			this.NoBumpEvent = null;
			this.WallEvent = null;
			this.HighWallEvent = null;
			this.EveryFrame = false;
		}

		// Token: 0x06007B95 RID: 31637 RVA: 0x0024FEE4 File Offset: 0x0024E0E4
		public override void OnEnter()
		{
			this.hc = HeroController.instance;
			if (this.hc)
			{
				this.DoAction();
				if (!this.EveryFrame)
				{
					base.Finish();
					return;
				}
			}
			else
			{
				Debug.LogError("HeroController was null", base.Owner);
			}
		}

		// Token: 0x06007B96 RID: 31638 RVA: 0x0024FF23 File Offset: 0x0024E123
		public override void OnFixedUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007B97 RID: 31639 RVA: 0x0024FF2C File Offset: 0x0024E12C
		private void DoAction()
		{
			bool flag;
			bool flag2;
			bool flag3;
			this.hc.CheckForBump((this.Direction.Value < 0f) ? CollisionSide.left : CollisionSide.right, out flag, out flag2, out flag3);
			this.StoreHitBump.Value = flag;
			this.StoreHitWall.Value = flag2;
			this.StoreHitHighWall.Value = flag3;
			if (flag)
			{
				base.Fsm.Event(this.BumpEvent);
				return;
			}
			if (flag3 && this.HighWallEvent != null)
			{
				base.Fsm.Event(this.HighWallEvent);
				return;
			}
			if (flag2)
			{
				base.Fsm.Event(this.WallEvent);
				return;
			}
			base.Fsm.Event(this.NoBumpEvent);
		}

		// Token: 0x04007BC8 RID: 31688
		public FsmFloat Direction;

		// Token: 0x04007BC9 RID: 31689
		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitBump;

		// Token: 0x04007BCA RID: 31690
		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitWall;

		// Token: 0x04007BCB RID: 31691
		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitHighWall;

		// Token: 0x04007BCC RID: 31692
		public FsmEvent BumpEvent;

		// Token: 0x04007BCD RID: 31693
		public FsmEvent NoBumpEvent;

		// Token: 0x04007BCE RID: 31694
		public FsmEvent WallEvent;

		// Token: 0x04007BCF RID: 31695
		public FsmEvent HighWallEvent;

		// Token: 0x04007BD0 RID: 31696
		public bool EveryFrame;

		// Token: 0x04007BD1 RID: 31697
		private HeroController hc;
	}
}
