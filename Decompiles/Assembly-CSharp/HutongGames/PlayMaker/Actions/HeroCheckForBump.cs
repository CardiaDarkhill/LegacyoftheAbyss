using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200123D RID: 4669
	[ActionCategory("Hollow Knight")]
	public class HeroCheckForBump : FsmStateAction
	{
		// Token: 0x06007B8B RID: 31627 RVA: 0x0024FD5E File Offset: 0x0024DF5E
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007B8C RID: 31628 RVA: 0x0024FD6C File Offset: 0x0024DF6C
		public override void Awake()
		{
			this.OnPreprocess();
		}

		// Token: 0x06007B8D RID: 31629 RVA: 0x0024FD74 File Offset: 0x0024DF74
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

		// Token: 0x06007B8E RID: 31630 RVA: 0x0024FDA7 File Offset: 0x0024DFA7
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

		// Token: 0x06007B8F RID: 31631 RVA: 0x0024FDE6 File Offset: 0x0024DFE6
		public override void OnFixedUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007B90 RID: 31632 RVA: 0x0024FDF0 File Offset: 0x0024DFF0
		private void DoAction()
		{
			bool flag;
			bool flag2;
			bool flag3;
			this.hc.CheckForBump((this.Direction.Value < 0f) ? CollisionSide.left : CollisionSide.right, out flag, out flag2, out flag3);
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

		// Token: 0x04007BC0 RID: 31680
		public FsmFloat Direction;

		// Token: 0x04007BC1 RID: 31681
		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitBump;

		// Token: 0x04007BC2 RID: 31682
		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitWall;

		// Token: 0x04007BC3 RID: 31683
		public FsmEvent BumpEvent;

		// Token: 0x04007BC4 RID: 31684
		public FsmEvent NoBumpEvent;

		// Token: 0x04007BC5 RID: 31685
		public FsmEvent WallEvent;

		// Token: 0x04007BC6 RID: 31686
		public bool EveryFrame;

		// Token: 0x04007BC7 RID: 31687
		private HeroController hc;
	}
}
