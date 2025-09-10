using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001232 RID: 4658
	public class CharacterCheckForBump : FsmStateAction
	{
		// Token: 0x06007B5D RID: 31581 RVA: 0x0024F679 File Offset: 0x0024D879
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007B5E RID: 31582 RVA: 0x0024F687 File Offset: 0x0024D887
		public override void Awake()
		{
			this.OnPreprocess();
		}

		// Token: 0x06007B5F RID: 31583 RVA: 0x0024F68F File Offset: 0x0024D88F
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

		// Token: 0x06007B60 RID: 31584 RVA: 0x0024F6C4 File Offset: 0x0024D8C4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			this.bumpChecker = safe.GetComponent<CharacterBumpCheck>();
			if (!this.bumpChecker)
			{
				this.bumpChecker = safe.AddComponent<CharacterBumpCheck>();
			}
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007B61 RID: 31585 RVA: 0x0024F717 File Offset: 0x0024D917
		public override void OnFixedUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007B62 RID: 31586 RVA: 0x0024F720 File Offset: 0x0024D920
		private void DoAction()
		{
			bool flag;
			bool flag2;
			bool flag3;
			this.bumpChecker.CheckForBump((this.Direction.Value < 0f) ? CollisionSide.left : CollisionSide.right, out flag, out flag2, out flag3);
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

		// Token: 0x04007BA0 RID: 31648
		public FsmOwnerDefault Target;

		// Token: 0x04007BA1 RID: 31649
		public FsmFloat Direction;

		// Token: 0x04007BA2 RID: 31650
		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitBump;

		// Token: 0x04007BA3 RID: 31651
		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitWall;

		// Token: 0x04007BA4 RID: 31652
		public FsmEvent BumpEvent;

		// Token: 0x04007BA5 RID: 31653
		public FsmEvent NoBumpEvent;

		// Token: 0x04007BA6 RID: 31654
		public FsmEvent WallEvent;

		// Token: 0x04007BA7 RID: 31655
		public bool EveryFrame;

		// Token: 0x04007BA8 RID: 31656
		private CharacterBumpCheck bumpChecker;
	}
}
