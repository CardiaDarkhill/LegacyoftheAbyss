using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001367 RID: 4967
	public class ActivateTalkAnimNPC : FSMUtility.GetComponentFsmStateAction<TalkAnimNPC>
	{
		// Token: 0x17000C41 RID: 3137
		// (get) Token: 0x06008016 RID: 32790 RVA: 0x0025D5EA File Offset: 0x0025B7EA
		protected override bool AutoFinish
		{
			get
			{
				return this.StopInstantly.Value || this.subbedNpc == null;
			}
		}

		// Token: 0x06008017 RID: 32791 RVA: 0x0025D607 File Offset: 0x0025B807
		public bool IsActivate()
		{
			return this.Activate.Value;
		}

		// Token: 0x06008018 RID: 32792 RVA: 0x0025D614 File Offset: 0x0025B814
		public override void Reset()
		{
			base.Reset();
			this.Activate = null;
			this.StopInstantly = null;
		}

		// Token: 0x06008019 RID: 32793 RVA: 0x0025D62C File Offset: 0x0025B82C
		protected override void DoAction(TalkAnimNPC npc)
		{
			if (this.Activate.Value)
			{
				npc.StartAnimation();
			}
			else
			{
				if (!this.StopInstantly.Value)
				{
					npc.Stopped += this.OnStopped;
					this.subbedNpc = npc;
					npc.StopAnimation(false);
					return;
				}
				npc.StopAnimation(true);
			}
			base.Finish();
		}

		// Token: 0x0600801A RID: 32794 RVA: 0x0025D68B File Offset: 0x0025B88B
		private void OnStopped()
		{
			this.subbedNpc.Stopped -= this.OnStopped;
			base.Finish();
		}

		// Token: 0x0600801B RID: 32795 RVA: 0x0025D6AA File Offset: 0x0025B8AA
		public override void OnExit()
		{
			if (this.subbedNpc)
			{
				this.subbedNpc.Stopped -= this.OnStopped;
				this.subbedNpc = null;
			}
		}

		// Token: 0x04007F82 RID: 32642
		public FsmBool Activate;

		// Token: 0x04007F83 RID: 32643
		[HideIf("IsActivate")]
		public FsmBool StopInstantly;

		// Token: 0x04007F84 RID: 32644
		private TalkAnimNPC subbedNpc;
	}
}
