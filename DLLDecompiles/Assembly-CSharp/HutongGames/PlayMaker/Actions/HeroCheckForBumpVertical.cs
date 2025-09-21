using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200123F RID: 4671
	[ActionCategory("Hollow Knight")]
	public class HeroCheckForBumpVertical : FsmStateAction
	{
		// Token: 0x06007B99 RID: 31641 RVA: 0x0024FFE4 File Offset: 0x0024E1E4
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007B9A RID: 31642 RVA: 0x0024FFF2 File Offset: 0x0024E1F2
		public override void Awake()
		{
			this.OnPreprocess();
		}

		// Token: 0x06007B9B RID: 31643 RVA: 0x0024FFFA File Offset: 0x0024E1FA
		public override void Reset()
		{
			this.Direction = null;
			this.StoreHitBump = null;
			this.StoreHitWall = null;
			this.BumpEvent = null;
			this.NoBumpEvent = null;
			this.WallEvent = null;
			this.EveryFrame = false;
		}

		// Token: 0x06007B9C RID: 31644 RVA: 0x0025002D File Offset: 0x0024E22D
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

		// Token: 0x06007B9D RID: 31645 RVA: 0x0025006C File Offset: 0x0024E26C
		public override void OnFixedUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007B9E RID: 31646 RVA: 0x00250074 File Offset: 0x0024E274
		private void DoAction()
		{
			bool flag;
			bool flag2;
			bool flag3;
			this.hc.CheckForBump((this.Direction.Value < 0f) ? CollisionSide.bottom : CollisionSide.top, out flag, out flag2, out flag3);
			this.StoreHitBump.Value = flag;
			this.StoreHitWall.Value = flag2;
			if (flag)
			{
				base.Fsm.Event(this.BumpEvent);
				return;
			}
			if (flag2)
			{
				base.Fsm.Event(this.WallEvent);
				return;
			}
			base.Fsm.Event(this.NoBumpEvent);
		}

		// Token: 0x04007BD2 RID: 31698
		public FsmFloat Direction;

		// Token: 0x04007BD3 RID: 31699
		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitBump;

		// Token: 0x04007BD4 RID: 31700
		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitWall;

		// Token: 0x04007BD5 RID: 31701
		public FsmEvent BumpEvent;

		// Token: 0x04007BD6 RID: 31702
		public FsmEvent NoBumpEvent;

		// Token: 0x04007BD7 RID: 31703
		public FsmEvent WallEvent;

		// Token: 0x04007BD8 RID: 31704
		public bool EveryFrame;

		// Token: 0x04007BD9 RID: 31705
		private HeroController hc;
	}
}
